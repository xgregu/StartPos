using NLog;
using System;
using System.Diagnostics;

namespace StartPos.Shared.Utils
{
    public static class AppsOperations
    {
        public static bool Start(string appName, string arg, bool isSilent = false) => IternalStart(appName, arg, string.Empty, isSilent, false, ProcessWindowStyle.Normal);

        public static bool Start(string appName, string arg, ProcessWindowStyle windowStyle, bool isSilent = false) => IternalStart(appName, arg, string.Empty, isSilent, false, windowStyle);

        public static bool Start(string appName, string arg, string workingDirectory, ProcessWindowStyle windowStyle, bool isSilent = false) => IternalStart(appName, arg, workingDirectory, isSilent, false, windowStyle);

        public static bool StartAndWait(string appName, string arg, bool isSilent = false) => IternalStart(appName, arg, string.Empty, isSilent, true, ProcessWindowStyle.Normal);

        public static bool StartAndWait(string appName, string arg, ProcessWindowStyle windowStyle, bool isSilent = false) => IternalStart(appName, arg, string.Empty, isSilent, true, windowStyle);

        public static bool StartAndWait(string appName, string arg, string workingDirectory, ProcessWindowStyle windowStyle, bool isSilent = false) => IternalStart(appName, arg, workingDirectory, isSilent, true, windowStyle);

        private static bool IternalStart(string appName, string arg, string workingDirectory, bool isSilent, bool waitForExit, ProcessWindowStyle windowStyle)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);

            using (var process = new Process())
            {
                try
                {
                    windowLogger.Info($"Uruchamiam {appName} {arg}...");
                    logger.Info($"{nameof(IternalStart)} | Start the {appName} {arg}.");

                    if (isSilent)
                    {
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                    }

                    process.StartInfo.FileName = appName;
                    process.StartInfo.Arguments = arg;
                    process.StartInfo.WindowStyle = windowStyle;
                    process.StartInfo.WorkingDirectory = workingDirectory;
                    process.Start();
                    if (waitForExit)
                        process.WaitForExit();
                }
                catch (Exception ex)
                {
                    windowLogger.Error($"Błąd podczas uruchamiania {appName} {arg}. Szczegóły w log.");
                    logger.Error(ex, $"{nameof(IternalStart)} | Error start {appName} {arg}.");
                    return false;
                }
                return true;
            }
        }
    }
}