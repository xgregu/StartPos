using StartPos.Enums;
using StartPos.Interfaces;
using StartPos.Shared;
using StartPos.Shared.Interfaces;
using System.Threading.Tasks;

namespace StartPos.Flows
{
    internal class TestFlow : IFlow
    {
        private readonly IConfig _config;
        private readonly IBackupService _backupService;
        private readonly ITaskShedulerService _taskShedulerService;

        public TestFlow(IConfig config, IBackupService backupService, ITaskShedulerService taskShedulerService)
        {
            _config = config;
            _backupService = backupService;
            _taskShedulerService = taskShedulerService;
        }

        public string Name { get; } = Constants.Flow.Test;
        public UpdaterMode UpdateMode { get; } = UpdaterMode.None;

        public async Task Run() => await Task.Run(FlowTask);

        private void FlowTask()
        {
            _taskShedulerService.CheckIsExist(TaskSchedulerType.AutoStart);
            _taskShedulerService.CheckIsExist(TaskSchedulerType.Restart);
        }
    }
}