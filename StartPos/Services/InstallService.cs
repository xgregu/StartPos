using StartPos.Interfaces;
using StartPos.Shared;
using StartPos.Shared.Utils;
using System.IO;

namespace StartPos.Services
{
    public class InstallService : IInstallService
    {
        private readonly IContext _context;
        private readonly string _issSettingFile = Path.Combine(Constants.BaseDir, "Setup", "Inno", "InstallerStartPosSettingFile.iss");
        private readonly string _issCompiler = Path.Combine(Constants.BaseDir, "Setup", "Inno", "ISCC.exe");

        public InstallService(IContext context)
        {
            _context = context;
        }

        public void BuildInstaller()
        {
            if (!File.Exists(_issCompiler))
                return;

            PrepareIssSettingFile();

            if (File.Exists(_issSettingFile))
                AppsOperations.StartAndWait(_issCompiler, _issSettingFile);
        }

        private void PrepareIssSettingFile()
        {
            var bodyXml = string.Format(_issBody,
                                        Constants.CleanAppName,
                                        _context.AppVersion,
                                        Constants.BaseDir,
                                        "{uninstallexe}",
                                        "{app}",
                                        "{group}",
                                        $"{{cm:UninstallProgram,{Constants.CleanAppName}}}",
                                        "{{75039B9C-9283-4D37-BF13-61863CB2675B}");
            if (File.Exists(_issSettingFile))
                File.Delete(_issSettingFile);

            File.WriteAllText(_issSettingFile, bodyXml);
        }

        private const string _issBody = @"
[Setup]
AppId={7}
AppName={0}
AppVersion={1}
VersionInfoVersion={1}
VersionInfoProductVersion={1}
AppPublisher=COMA Adam Kosma
DefaultDirName=C:\Pcmwin\{0}
DefaultGroupName={0}
DisableProgramGroupPage=yes
OutputDir={2}Setup
OutputBaseFilename=StartPos_Installer
SetupIconFile={2}Icons\startpos.ico
Compression=lzma
SolidCompression=yes
AppCopyright=COMA Adam Kosma
UsePreviousPrivileges=False
RestartIfNeededByRun=False
ShowLanguageDialog=no
AppPublisherURL=www.coma.tychy.pl
UninstallDisplayIcon={3}
DisableReadyPage=True
DisableReadyMemo=True
Password=10coma123

[Files]
Source: ""{2}*""; DestDir: ""{4}""; Flags: ignoreversion
Source: ""{2}Icons\*""; DestDir: ""{4}\Icons""; Flags: ignoreversion
Source: ""{2}Installers\*""; DestDir: ""{4}\Installers""; Flags: ignoreversion

[Icons]
Name: ""{5}\{0}""; Filename: ""{4}\{0}.exe""
Name: ""{5}\{6}""; Filename: ""{3}""

[Run]
Filename: ""{4}\StartPos.exe""; Parameters: ""setup""; WorkingDir: ""{4}""; Flags: nowait postinstall

";
    }
}