using NLog;
using StartPos.Shared.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace StartPos.Shared.Utils
{
    public class PcPosConfigParser : IPcPosConfigParser
    {
        private readonly IConfig _config;

        public PcPosConfigParser(IConfig config)
        {
            _config = config;
        }

        public string GetValue(string path, string section, string parameter)
        {
            section = section.ToLower();
            parameter = parameter.ToLower();

            var logger = LogManager.GetCurrentClassLogger();

            var confFile = Path.Combine(path, $"{_config.AppToRun}.conf");
            var sectionCorrect = false;

            try
            {
                using (var sr = File.OpenText(confFile))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.ToLower().Equals($"section: {section}"))
                            sectionCorrect = true;

                        if (sectionCorrect && line.ToLower().Contains(parameter))
                        {
                            var splited = line.Split(':');
                            {
                                var result = string.Join(":", splited.Skip(1).ToArray()).Trim();
                                logger.Info($"{nameof(GetValue)} | Parsing the PcPos configuration file {_config.AppToRun}.conf. Section: {section}, Parameter: {parameter}. Result: {result}");
                                return result;
                            }
                        }

                        if (line.ToLower().Equals($"endsection: {section}"))
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{nameof(GetValue)} | Error parsing the PcPos configuration file. Section: {section}, Parameter: {parameter}");
            }

            return string.Empty;
        }
    }
}