using IWshRuntimeLibrary;
using NLog;
using StartPos.Enums;
using StartPos.Interfaces;
using StartPos.Models;
using StartPos.Shared;
using System;
using System.IO;
using File = System.IO.File;

namespace StartPos.Services
{
    internal class ShortcutService : IShortcutService
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _shorcutIcoDir = Path.Combine(Constants.BaseDir, "Icons");
        private readonly ILogger _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);

        public void CreateShorcut(Shortcut shortcut)
        {
            if (!Directory.Exists(shortcut.ShortcutDir))
                Directory.CreateDirectory(shortcut.ShortcutDir);

            var shortcutFullName = Path.Combine(shortcut.ShortcutDir, shortcut.ShortcutFile + ".lnk");

            if (File.Exists(shortcutFullName))
                File.Delete(shortcutFullName);
            try
            {
                _windowLogger.Info($"Tworzenie skrótu {shortcutFullName}");
                _logger.Info($"{nameof(CreateShorcut)} | Create shortcut {shortcutFullName}");
                var shell = new WshShell();
                var newShortcut = (IWshShortcut)shell.CreateShortcut(shortcutFullName);

                newShortcut.Arguments = shortcut.Arguments;
                newShortcut.TargetPath = shortcut.TargetPath;
                newShortcut.WindowStyle = 1;
                newShortcut.Description = "";
                newShortcut.WorkingDirectory = shortcut.WorkingDirectory;
                newShortcut.IconLocation = GetShortcutDir(shortcut.IconLocation);
                newShortcut.Save();
            }
            catch (Exception ex)
            {
                _windowLogger.Warn($"Błąd podczas tworzenia skrótu {shortcutFullName}. Szczegóły w log.");
                _logger.Warn(ex, $"{nameof(CreateShorcut)} | Error while create shortcut {shortcutFullName}");
            }
        }

        private string GetShortcutDir(ShortcutType shortcutType)
        {
            switch (shortcutType)
            {
                case ShortcutType.PcPos7:
                    return Path.Combine(_shorcutIcoDir, "pcpos.ico");

                case ShortcutType.Diagnostic:
                    return Path.Combine(_shorcutIcoDir, "postest.ico");

                case ShortcutType.Repair:
                    return Path.Combine(_shorcutIcoDir, "repair.ico");

                case ShortcutType.Restart:
                    return Path.Combine(_shorcutIcoDir, "restart.ico");

                case ShortcutType.Setup:
                    return Path.Combine(_shorcutIcoDir, "setup.ico");

                case ShortcutType.Shutdown:
                    return Path.Combine(_shorcutIcoDir, "shutdown.ico");

                case ShortcutType.Disable:
                    return Path.Combine(_shorcutIcoDir, "disable.ico");

                default:
                    return string.Empty;
            }
        }
    }
}