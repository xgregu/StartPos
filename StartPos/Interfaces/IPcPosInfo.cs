using System.Collections.Generic;

namespace StartPos.Interfaces
{
    public interface IPcPosInfo
    {
        bool IsInstalled { get; }
        string Version { get; }
        string InstalationDir { get; }
        string DataBase { get; }
        int Port { get; }
        string PosNumber { get; }
        int EventLogCount { get; }
        string RemoteServerIP { get; }
        int RemoteServerPort { get; }
        List<string> SerializationInfo { get; }
        int MutexPort { get; }
        string JavaPath { get; }
    }
}