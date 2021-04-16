using NLog;
using StartPos.Interfaces;
using StartPos.Shared;
using StartPos.Shared.Enums;
using StartPos.Shared.Extesions;
using StartPos.Shared.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StartPos.Services
{
    internal class BackupService : IBackupService
    {
        private readonly IConfig _config;
        private readonly IContext _context;
        private readonly string _dirBackupDay;
        private readonly bool _isWriteAccessToObligatoryBackupFolder;
        private readonly ILogger _logger;
        private readonly IRemoteServerService _remoteServerService;
        private readonly ILogger _windowLogger;
        private readonly ISystemService _systemService;

        public BackupService(IConfig config, IContext context, IRemoteServerService remoteServerService, ISystemService systemService)
        {
            _config = config;
            _context = context;
            _remoteServerService = remoteServerService;
            _logger = LogManager.GetCurrentClassLogger();
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _dirBackupDay = DateTime.Now.GetShortDay();
            _systemService = systemService;

            _isWriteAccessToObligatoryBackupFolder = IsWriteAccessToFolder(_config.ObligatoryBackup.Path);
        }

        public Task ObligatoryBackup()
        {
            if (!_isWriteAccessToObligatoryBackupFolder)
                return Task.CompletedTask;
            if (IsLowDiskSpace(_config.ObligatoryBackup.Path))
            {
                _windowLogger.Warn($"Mało miejsca na dysku dla backup obowiązkowy: {_config.ObligatoryBackup.Path}");
                _logger.Warn($"{nameof(BackupFile)} | Low disk space: {_config.ObligatoryBackup.Path}");
                _context.TsReport.Warrning += $" ► Mało miejsca na dysku dla backup obowiązkowy: {_config.ObligatoryBackup.Path}{ Environment.NewLine}";
            }

            _windowLogger.Info("Backup kluczowych danych...");

            var destinationDirBackup = Path.Combine(_config.ObligatoryBackup.Path, "Pliki", _dirBackupDay);

            if (!Directory.Exists(destinationDirBackup))
                Directory.CreateDirectory(destinationDirBackup);

            _config.DataBackupItems.FindAll(x => x.Type == BackupItemType.File)
                .ForEach(i => BackupFile(i.Path, destinationDirBackup));
            _config.DataBackupItems.FindAll(x => x.Type == BackupItemType.Directory)
                .ForEach(i => BackupDirectory(i.Path, destinationDirBackup));
            return Task.CompletedTask;
        }

        public Task BackupPcPosDirectory()
        {
            if (!_isWriteAccessToObligatoryBackupFolder)
            {
                _windowLogger.Error(
                    $"Backup obowiązkowy przerwany. Brak dostępu do: {_config.ObligatoryBackup.Path}");
                _context.TsReport.Fatal += $" ► Backup obowiązkowy przerwany. Brak dostępu do: {_config.ObligatoryBackup.Path}{ Environment.NewLine}";
                return Task.CompletedTask;
            }

            _windowLogger.Info("Pełny backup katalogu PcPos...");
            var destinationDirBackup =
                Path.Combine(_config.ObligatoryBackup.Path, "Pliki", "Full_pcpos7_backup_folder");

            if (!Directory.Exists(destinationDirBackup))
                Directory.CreateDirectory(destinationDirBackup);

            BackupDirectory(_context.PcPosInfo.InstalationDir,
                Path.Combine(_config.ObligatoryBackup.Path, "Pliki", "Full_pcpos7_backup_folder"));

            return Task.CompletedTask;
        }

        public Task AlternativeBackup()
        {
            if (!_config.AlternativeBackup.Active)
                return Task.CompletedTask;

            if (!IsWriteAccessToFolder(_config.AlternativeBackup.Path))
            {
                _windowLogger.Error(
                    $"Backup alternatywny przerwany. Brak dostępu do: {_config.AlternativeBackup.Path}");
                _context.TsReport.Fatal += $" ► Backup alternatywny przerwany. Brak dostępu do: {_config.AlternativeBackup.Path}{ Environment.NewLine}";
                return Task.CompletedTask;
            }

            if (IsLowDiskSpace(_config.AlternativeBackup.Path))
            {
                _windowLogger.Warn($"Mało miejsca na dysku dla backup alternatywny: {_config.AlternativeBackup.Path}");
                _logger.Warn($"{nameof(BackupFile)} | Low disk space: {_config.AlternativeBackup.Path}");
                _context.TsReport.Warrning += $" ► Mało miejsca na dysku dla backup alternatywny: {_config.AlternativeBackup.Path}{ Environment.NewLine}";
            }

            _windowLogger.Info("Przenoszenie kopii zapasowej do lokalizacji alternatywnej...");
            BackupDirectory(_config.ObligatoryBackup.Path,
                Path.Combine(_config.AlternativeBackup.Path, _config.Contractor.LocationName));

            return Task.CompletedTask;
        }

        public Task AlternativeBackupOnlyToday()
        {
            if (!_config.AlternativeBackup.Active)
                return Task.CompletedTask;

            if (!IsWriteAccessToFolder(_config.AlternativeBackup.Path))
            {
                _windowLogger.Error(
                    $"Backup alternatywny przerwany. Brak dostępu do: {_config.AlternativeBackup.Path}");
                _context.TsReport.Fatal += $" ► Backup alternatywny przerwany. Brak dostępu do: {_config.AlternativeBackup.Path}{ Environment.NewLine}";
                return Task.CompletedTask;
            }

            if (IsLowDiskSpace(_config.AlternativeBackup.Path))
            {
                _windowLogger.Warn($"Mało miejsca na dysku dla backup alternatywny: {_config.AlternativeBackup.Path}");
                _logger.Warn($"{nameof(BackupFile)} | Low disk space: {_config.AlternativeBackup.Path}");
                _context.TsReport.Warrning += $" ► Mało miejsca na dysku dla backup alternatywny: {_config.AlternativeBackup.Path}{ Environment.NewLine}";
            }

            var sourceDirBackup = Path.Combine(_config.ObligatoryBackup.Path, "Pliki", _dirBackupDay);
            var destinationDirBackup = Path.Combine(_config.AlternativeBackup.Path, _config.Contractor.LocationName, new DirectoryInfo(sourceDirBackup).Parent.Parent.ToString(), "Pliki");

            _windowLogger.Info("Przenoszenie kopii zapasowej do lokalizacji alternatywnej...");
            BackupDirectory(sourceDirBackup, destinationDirBackup);
            return Task.CompletedTask;
        }

        private void BackupFile(string sourcePath, string destinationPath)
        {
            if (!File.Exists(sourcePath))
            {
                _logger.Info($"{nameof(BackupFile)} | File no exist: {sourcePath}.");
                _windowLogger.Info($"Plik nie istnieje:  {sourcePath} ");
                return;
            }

            var sourceFile = new FileInfo(sourcePath);
            var destinationFile = new FileInfo(Path.Combine(destinationPath, sourceFile.Name));

            try
            {
                if (IsNeedsUpdate(sourceFile, destinationFile))
                {
                    _logger.Info($"{nameof(BackupFile)} | Copying file: {sourceFile.FullName} to {destinationFile.FullName}");
                    _windowLogger.Info($"   {sourceFile.Name} => {destinationFile.FullName}");
                    File.Copy(sourceFile.FullName, destinationFile.FullName, true);
                }
            }
            catch (Exception ex)
            {
                _windowLogger.Warn($"...Błąd podczas kopiowania {sourceFile.FullName}. Szczegóły w log.");
                _logger.Warn(ex, "Copying file error");
            }
        }

        private void BackupDirectory(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                _logger.Info($"{nameof(BackupDirectory)} | Directory no exist: {sourcePath}");
                _windowLogger.Info($"Folder nie istnieje: {sourcePath}");
                return;
            }
            var sourceDir = new DirectoryInfo(sourcePath);

            var destinationDir = new DirectoryInfo(Path.Combine(destinationPath, sourceDir.Name));
            try
            {
                //_logger.Info($"{nameof(CheckInstanceIsRunning)} | Copying directory: {sourceDir.FullName} to {destinationDir.FullName}");
                //_windowLogger.Info($"   {sourceDir.FullName} => {destinationDir.FullName}");
                CopyDirectoryTree(sourceDir, destinationDir);
            }
            catch (Exception ex)
            {
                _windowLogger.Warn($"...Błąd podczas kopiowania {sourceDir.FullName}. Szczegóły w log.");
                _logger.Warn(ex, "Copying directory error");
            }
        }

        private void CopyDirectoryTree(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.Name == "Log")
            {
                if (Directory.Exists(target.FullName))
                    Directory.Delete(target.FullName, true);
                return;
            }

            Directory.CreateDirectory(target.FullName);

            foreach (var fi in source.GetFiles())
            {
                var targetFile = new FileInfo(Path.Combine(target.FullName, fi.Name));
                try
                {
                    if (IsNeedsUpdate(fi, targetFile))
                    {
                        _logger.Info($"{nameof(CopyDirectoryTree)} | Copying file: {fi.FullName} to {Path.Combine(target.FullName, fi.Name)}");
                        _windowLogger.Info($"    {fi.Name} => {Path.Combine(target.FullName, fi.Name)}");
                        fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                    }
                }
                catch (Exception ex)
                {
                    _windowLogger.Warn($"...Błąd podczas kopiowania {target.FullName}. Szczegóły w log.");
                    _logger.Warn(ex, "Copying file error");
                }
            }

            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectoryTree(diSourceSubDir, nextTargetSubDir);
            }
        }

        public bool IsWriteAccessToFolder(string folderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                    return false;

                // folderPath = new DirectoryInfo(folderPath).Root.ToString();

                var host = new Uri(folderPath).Host;
                if (!string.IsNullOrEmpty(host))
                    if (!_remoteServerService.IsActive(host, 445))
                    {
                        _logger.Error($"{nameof(IsWriteAccessToFolder)} | No write access to network folder {folderPath}");
                        return false;
                    }

                if (!Directory.Exists(folderPath))
                {
                    _logger.Error($"{nameof(IsWriteAccessToFolder)} | Directory no exist {folderPath}");
                    return false;
                }

                try
                {
                    Directory.GetAccessControl(folderPath);
                    _logger.Info($"{nameof(IsWriteAccessToFolder)} | Ability to write access to folder {folderPath}");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"{nameof(IsWriteAccessToFolder)} | No write access to folder {folderPath}");
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool IsNeedsUpdate(FileInfo localFile, FileInfo backUpFile)
        {
            if (!File.Exists(backUpFile.FullName))
                return true;

            if (localFile.Length != backUpFile.Length)
                return true;

            if (string.Equals(localFile.FullName, backUpFile.FullName, StringComparison.OrdinalIgnoreCase))
                return false;

            using (var fs1 = localFile.OpenRead())
            using (var fs2 = backUpFile.OpenRead())
            {
                for (int i = 0; i < localFile.Length; i++)
                {
                    if (fs1.ReadByte() != fs2.ReadByte())
                        return true;
                }
            }

            return false;
        }

        private bool IsLowDiskSpace(string backupPath)
        {
            if (_systemService.GetDirectorySizeMb(backupPath) < 0 || _systemService.GetFreeDiskSpaceMb(backupPath) < 0)
                return false;

            if (_systemService.GetDirectorySizeMb(backupPath) * 1.5 <= _systemService.GetFreeDiskSpaceMb(backupPath))
                return false;

            return true;
        }
    }
}