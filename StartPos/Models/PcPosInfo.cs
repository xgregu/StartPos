using NLog;
using StartPos.Interfaces;
using StartPos.Shared.Extesions;
using StartPos.Shared.Interfaces;
using StartPos.Shared.Utils;
using System.Collections.Generic;
using System.IO;

namespace StartPos.Models
{
    public class PcPosInfo : IPcPosInfo
    {
        private const string _pcPosRegistryKey = @"SOFTWARE\Insoft sp. z o.o.\PC-POS 7";
        private const string _pcPosVeriosnRegistryKeyValue = "Version";
        private const string PcPosLocationRegistryKeyValue = "Location";

        private readonly IPcPosConfigParser _configParser;
        private readonly ILogger _logger;
        private readonly IPcPosLogParser _logParser;

        public PcPosInfo(IPcPosConfigParser configParser, IPcPosLogParser logParser)
        {
            _configParser = configParser;
            _logParser = logParser;
            _logger = LogManager.GetCurrentClassLogger();
            Version = GetPosVersion();
            InstalationDir = GetPosDir();
            ReadPcPosConfig();
            ReadLogPosConfig();
            JavaPath = Path.Combine(InstalationDir, "Java", "jre", "bin", "java.exe");
        }

        public bool IsInstalled => !string.IsNullOrEmpty(Version) && !string.IsNullOrEmpty(InstalationDir);
        public string Version { get; }
        public string InstalationDir { get; }
        public string DataBase { get; private set; }
        public int Port { get; private set; }
        public string PosNumber { get; private set; }
        public int EventLogCount { get; private set; }
        public string RemoteServerIP { get; private set; }
        public int RemoteServerPort { get; private set; }
        public List<string> SerializationInfo { get; private set; }
        public int MutexPort { get; private set; }
        public string JavaPath { get; }

        private void ReadLogPosConfig()
        {
            if (!IsInstalled)
            {
                _logger.Warn($"{nameof(ReadLogPosConfig)} | PcPos software installed not detected");
                return;
            }

            SerializationInfo = _logParser.GetSerializationInfo(InstalationDir, EventLogCount);
        }

        private void ReadPcPosConfig()
        {
            if (!IsInstalled)
            {
                _logger.Warn($"{nameof(ReadPcPosConfig)} | PcPos software installed not detected");
                return;
            }

            DataBase = _configParser.GetValue(InstalationDir, "LocalDatabase", "UserName");
            Port = GetPort();
            PosNumber = _configParser.GetValue(InstalationDir, "LocalDatabase", "POSName");
            EventLogCount = GetEventLogCount();
            RemoteServerIP = GetServerIp();
            RemoteServerPort = GetServerPort();
            MutexPort = GetMutexPort();
        }

        private string GetPosVersion()
        {
            var posVersion = RegistryOperations.GetValueRegistry(_pcPosRegistryKey, _pcPosVeriosnRegistryKeyValue);
            _logger.Info($"{ nameof(GetPosVersion)} | {posVersion}");
            return posVersion;
        }

        private string GetPosDir()
        {
            var pcPosPath = RegistryOperations.GetValueRegistry(_pcPosRegistryKey, PcPosLocationRegistryKeyValue);
            _logger.Info($"{ nameof(GetPosDir)} | {pcPosPath}");
            return Directory.Exists(pcPosPath) ? pcPosPath : string.Empty;
        }

        private int GetPort()
        {
            var value = _configParser.GetValue(InstalationDir, "LocalDatabase", "ConnectString");

            if (string.IsNullOrEmpty(value))
                return 0;

            var tmp = value.Split('/');
            var tmp2 = tmp[2].Split(':');
            int.TryParse(tmp2[1], out var port);
            _logger.Info($"{ nameof(GetPort)} | {port}");
            return port;
        }

        private int GetEventLogCount()
        {
            var tmp = _configParser.GetValue(InstalationDir, "PrimaryFiscalPrinterParams", "EventLogSize")
                .Split(',')[0];

            int.TryParse(tmp, out var eventLogCount);
            _logger.Info($"{ nameof(GetEventLogCount)} | {eventLogCount}");
            return eventLogCount;
        }

        private string GetServerIp()
        {
            var scserverIp = GetScServerIpIfActive();
            if (!string.IsNullOrEmpty(scserverIp))
                return scserverIp;

            var value = _configParser.GetValue(InstalationDir, "ExchangeEngine", "ConnectString");
            _logger.Info($"{ nameof(GetServerIp)} | {value.GetIpAdress()}");
            return value.GetIpAdress();
        }

        private int GetMutexPort()
        {
            var tmp = _configParser.GetValue(InstalationDir, "LocalResources", "MutexPort");
            int.TryParse(tmp, out var mutexPort);
            _logger.Info($"{ nameof(GetMutexPort)} | {mutexPort}");
            return mutexPort;
        }

        private int GetServerPort()
        {
            var scserverPort = GetScServerPortIfActive();
            if (!string.IsNullOrEmpty(scserverPort))
            {
                _logger.Info($"{ nameof(GetServerPort)} | {int.Parse(scserverPort)}");
                return int.Parse(scserverPort);
            }

            var value = _configParser.GetValue(InstalationDir, "ExchangeEngine", "ConnectString");
            if (string.IsNullOrEmpty(value))
                return 0;

            var tmp = value.Split('/');
            var tmp2 = tmp[2].Split(':');
            int.TryParse(tmp2[1], out var serverPort);
            _logger.Info($"{ nameof(GetServerPort)} | {serverPort}");
            return serverPort;
        }

        private string GetScServerIpIfActive()
        {
            var serverActive = _configParser.GetValue(InstalationDir, "StoreChainServer", "ServerActive");
            return serverActive != "Y"
                ? string.Empty
                : _configParser.GetValue(InstalationDir, "StoreChainServer", "ServerIP");
        }

        private string GetScServerPortIfActive()
        {
            var serverActive = _configParser.GetValue(InstalationDir, "StoreChainServer", "ServerActive");
            return serverActive != "Y"
                ? string.Empty
                : _configParser.GetValue(InstalationDir, "StoreChainServer", "ServerPortNumber");
        }
    }
}