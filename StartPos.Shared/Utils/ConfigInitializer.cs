using Csc.ApiClient;
using NLog;
using StartPos.Shared.Configs;
using StartPos.Shared.Interfaces;
using System.IO;
using Unity;

namespace StartPos.Shared.Utils
{
    public class ConfigInitializer : IConfigInitializer
    {
        public void Initialize(IUnityContainer container)
        {
            IConfig config;
            ILogger logger = LogManager.GetCurrentClassLogger();

            if (!Directory.Exists(Constants.ConfigDir))
                Directory.CreateDirectory(Constants.ConfigDir);

            if (!File.Exists(Constants.ConfigFile))
            {
                logger.Info($"{nameof(Initialize)} | Save new {Constants.ConfigFile}");
                config = new Config();
            }
            else
            {
                logger.Info($"{nameof(Initialize)} | Reading {Constants.ConfigFile}");
                config = XmlHelpers.DeserializeXml<Config>(Constants.ConfigFile);
            }
            config.UpdateConfig();
            config.SaveSettings();

            container.RegisterInstance(config);
            container.RegisterInstance<ICscApiClientConfig>(config.ApiClientConfig);
        }
    }
}