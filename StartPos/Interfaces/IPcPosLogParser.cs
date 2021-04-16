using System.Collections.Generic;

namespace StartPos.Interfaces
{
    public interface IPcPosLogParser
    {
        List<string> GetSerializationInfo(string path, int eventLogCount);
    }
}