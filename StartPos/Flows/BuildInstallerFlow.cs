using StartPos.Enums;
using StartPos.Interfaces;
using StartPos.Shared;
using StartPos.Shared.Interfaces;
using System.Threading.Tasks;

namespace StartPos.Flows
{
    internal class BuildInstallerFlow : IFlow
    {
        private readonly IInstallService _installService;

        public BuildInstallerFlow(IInstallService installService)
        {
            _installService = installService;
        }

        public string Name { get; } = Constants.Flow.BuildInstaller;
        public UpdaterMode UpdateMode { get; } = UpdaterMode.None;

        public async Task Run() => await Task.Run(FlowTask);

        private void FlowTask() => _installService.BuildInstaller();
    }
}