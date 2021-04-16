using NLog;
using StartPos.Enums;
using StartPos.Interfaces;
using StartPos.Shared;
using StartPos.Shared.Interfaces;
using System.Threading.Tasks;

namespace StartPos.Flows
{
    internal class DisableFlow : IFlow
    {
        private readonly IConfig _config;
        private readonly ILogger _windowLogger;
        private readonly ILogger _logger;

        public DisableFlow(IConfig config)
        {
            _config = config;
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _logger = LogManager.GetCurrentClassLogger();
        }

        public string Name { get; } = Constants.Flow.Disable;
        public UpdaterMode UpdateMode { get; } = UpdaterMode.None;

        public async Task Run()
        {
            await Task.Run(FlowTask);
        }

        private void FlowTask()
        {
            _windowLogger.Error("Oprogramowanie StartPos zostanie wyłaczone.");
            _logger.Error($"{nameof(FlowTask)} | The StartPos software will be terminated");
            _config.IsStartPosEnabled = false;
            _config.SaveSettings();
        }
    }
}