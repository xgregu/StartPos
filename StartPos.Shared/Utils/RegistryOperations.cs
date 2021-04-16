using Microsoft.Win32;
using NLog;
using System;
using System.Linq;

namespace StartPos.Shared.Utils
{
    public static class RegistryOperations
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Logger WindowLogger = LogManager.GetLogger(Constants.WindowLoggerName);

        public static string GetValueRegistry(string key, string value)
        {
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                using (var subKey = Registry.LocalMachine.OpenSubKey(key))
                {
                    var subKeyValue = subKey?.GetValue(value);
                    if (subKeyValue != null)
                    {
                        logger.Info($"{nameof(GetValueRegistry)} | Key: {key}, {value} - {subKeyValue}");
                        return subKeyValue as string;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{nameof(GetValueRegistry)} | Unable to read data from registry. Key: {key}, {value}");
            }

            return string.Empty;
        }

        public static void SetValueRegistry(string key, string name, string value)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Info($"{nameof(SetValueRegistry)} | {key}, {name}, {value}");
            try
            {
                using (var subKey = Registry.LocalMachine.OpenSubKey(key, true))
                {
                    subKey?.SetValue(name, value);
                }
            }
            catch (Exception ex)
            {
                WindowLogger.Error($"Błąd podczas zapisu do rejestru. Klucz: {key}, {name}, {value}. Szczegóły w log.");
                Logger.Error(ex, $"{nameof(SetValueRegistry)} | Unable to write data to registry. Key: {key}, {name}, {value}");
            }
        }

        public static void DeleteValueRegistry(string key, string name)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Info($"{nameof(DeleteValueRegistry)} | {key}, {name}");

            try
            {
                using (var subKey = Registry.LocalMachine.OpenSubKey(key, true))
                {
                    subKey.GetValueNames()
                        .Where(x => x.ToLower().Contains(name.ToLower()))
                        .ToList()
                        .ForEach(subKey.DeleteValue);
                }
            }
            catch (Exception ex)
            {
                WindowLogger.Error($"Błąd podczas usuwania rejestru. Klucz: {key}, {name}. Szczegóły w log.");
                Logger.Error(ex, $"{nameof(DeleteValueRegistry)} | to delete data to registry. Key: {key}, {name}");
            }
        }
    }
}