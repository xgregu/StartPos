using StartPos.Forms;
using StartPos.Setup;
using StartPos.Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;
using Unity;

namespace StartPos
{
    internal static class Program
    {
        private static readonly bool _isRunAsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        private static readonly string _runAsAdminControlFile = Path.Combine(Constants.BaseDir, "IsAdmin.ControlFile");

        [STAThread]
        private static void Main(string[] args)
        {
            var arg = args.Length == 0 ? string.Empty : args[0].ToLower();

            if (!_isRunAsAdmin)
                RunAsAdmin(arg);

            if (File.Exists(_runAsAdminControlFile))
                File.Delete(_runAsAdminControlFile);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new Bootstrapper(arg);
        }

        internal static void RunApp(IUnityContainer container)
        {
            Application.Run(container.Resolve<MainForm>());
        }

        internal static void RunSetup(IUnityContainer container)
        {
            Application.Run(container.Resolve<SetupForm>());
        }

        private static void RunAsAdmin(string argument)
        {
            if (File.Exists(_runAsAdminControlFile))
                return;

            using (var sw = File.AppendText(_runAsAdminControlFile))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"));
                sw.Flush();
            }

            var proc = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                Arguments = argument,
                Verb = "runas"
            };

            try
            {
                Process.Start(proc);
                Environment.Exit(0);
            }
            catch { }
            return;
        }
    }
}