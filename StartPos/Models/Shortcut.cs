using StartPos.Enums;
using StartPos.Shared;
using System;
using System.IO;

namespace StartPos.Models
{
    internal class Shortcut
    {
        public string ShortcutDir { get; private set; }
        public string ShortcutFile { get; private set; }
        public string TargetPath { get; private set; }
        public string Arguments { get; private set; }
        public string WorkingDirectory { get; private set; }
        public ShortcutType IconLocation { get; private set; }

        private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static readonly string TaskBandPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Internet Explorer", "Quick Launch", "User Pinned", "TaskBar");
        private static readonly string ShortcutsPath = Path.Combine(Constants.BaseDir, "Shortcuts");
        private static readonly string StartPosPath = Path.Combine(Constants.BaseDir, "StartPos.exe");

        public static readonly Shortcut PcPosDesktop = new Shortcut
        {
            ShortcutDir = DesktopPath,
            ShortcutFile = "PC-POS 7",
            TargetPath = StartPosPath,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.PcPos7
        };

        public static readonly Shortcut DiagnosticDesktop = new Shortcut
        {
            ShortcutDir = DesktopPath,
            ShortcutFile = "Diagnostyka PC-POS",
            TargetPath = StartPosPath,
            Arguments = Constants.Flow.Diagnostic,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.Diagnostic
        };

        public static readonly Shortcut PcPosTaskband = new Shortcut
        {
            ShortcutDir = TaskBandPath,
            ShortcutFile = "PC-POS 7",
            TargetPath = StartPosPath,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.PcPos7
        };

        public static readonly Shortcut DiagnosticTaskband = new Shortcut
        {
            ShortcutDir = TaskBandPath,
            ShortcutFile = "Diagnostyka PC-POS",
            TargetPath = StartPosPath,
            Arguments = Constants.Flow.Diagnostic,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.Diagnostic
        };

        public static readonly Shortcut RepairDesktop = new Shortcut
        {
            ShortcutDir = DesktopPath,
            ShortcutFile = "StartPos - Zestaw naprawczy",
            TargetPath = StartPosPath,
            Arguments = Constants.Flow.Repair,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.Repair
        };

        public static readonly Shortcut SetupDesktop = new Shortcut
        {
            ShortcutDir = DesktopPath,
            ShortcutFile = "StartPos - Setup",
            TargetPath = StartPosPath,
            Arguments = Constants.Flow.Setup,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.Setup
        };

        public static readonly Shortcut PcPos7 = new Shortcut
        {
            ShortcutDir = ShortcutsPath,
            ShortcutFile = "PC-POS 7",
            TargetPath = StartPosPath,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.PcPos7
        };

        public static readonly Shortcut Diagnostic = new Shortcut
        {
            ShortcutDir = ShortcutsPath,
            ShortcutFile = "Diagnostyka PC-POS",
            TargetPath = StartPosPath,
            Arguments = Constants.Flow.Diagnostic,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.Diagnostic
        };

        public static readonly Shortcut Repair = new Shortcut
        {
            ShortcutDir = ShortcutsPath,
            ShortcutFile = "StartPos - Zestaw naprawczy",
            TargetPath = StartPosPath,
            Arguments = Constants.Flow.Repair,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.Repair
        };

        public static readonly Shortcut Setup = new Shortcut
        {
            ShortcutDir = ShortcutsPath,
            ShortcutFile = "StartPos - Setup",
            TargetPath = StartPosPath,
            Arguments = Constants.Flow.Setup,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.Setup
        };

        public static readonly Shortcut Restart = new Shortcut
        {
            ShortcutDir = ShortcutsPath,
            ShortcutFile = "StartPos - Restart",
            TargetPath = StartPosPath,
            Arguments = Constants.Flow.Restart,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.Restart
        };

        public static readonly Shortcut Shutdown = new Shortcut
        {
            ShortcutDir = ShortcutsPath,
            ShortcutFile = "StartPos - Shutdown",
            TargetPath = StartPosPath,
            Arguments = Constants.Flow.Shutdown,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.Shutdown
        };

        public static readonly Shortcut Disable = new Shortcut
        {
            ShortcutDir = ShortcutsPath,
            ShortcutFile = "StartPos - Disable",
            TargetPath = StartPosPath,
            Arguments = Constants.Flow.Disable,
            WorkingDirectory = Constants.BaseDir,
            IconLocation = ShortcutType.Disable
        };
    }
}