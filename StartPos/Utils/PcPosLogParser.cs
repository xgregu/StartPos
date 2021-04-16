using StartPos.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StartPos.Utils
{
    public class PcPosLogParser : IPcPosLogParser
    {
        private const int _numberCharToSkip = 62;

        public List<string> GetSerializationInfo(string path, int eventLogCount)
        {
            var serializationInfo = new List<string>();

            for (var i = 0; i <= eventLogCount; i++)
            {
                var logFile = Path.Combine(path, "Log", $"Pos_Log_{i}.log");
                if (!File.Exists(logFile))
                    break;

                const string checkPoint = "----------------------------------------------------------------------";
                var checkPointFinded = false;

                try
                {
                    var lines = File.ReadAllLines(logFile, Encoding.Default);

                    foreach (var line in lines)
                    {
                        if (line.Contains(checkPoint) && checkPointFinded)
                            return serializationInfo;

                        if (line.Contains(checkPoint)) checkPointFinded = true;

                        if (!line.Contains("Serialization Info"))
                            continue;

                        var value = line.Substring(_numberCharToSkip);
                        for (var x = 0; x < 10; x++) value = value.Replace("  ", " ");
                        serializationInfo.Add(value);
                    }
                }
                catch (Exception)
                {
                }
            }

            return serializationInfo;
        }
    }
}