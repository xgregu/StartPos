using StartPos.Enums;
using StartPos.Interfaces;
using StartPos.Shared;
using System.Threading.Tasks;

namespace StartPos.Flows
{
    internal class DiagnosticFlow : IFlow
    {
        private readonly IPcPosService _pcPosService;

        public DiagnosticFlow(IPcPosService pcPosService)
        {
            _pcPosService = pcPosService;
        }

        public string Name { get; } = Constants.Flow.Diagnostic;
        public UpdaterMode UpdateMode { get; } = UpdaterMode.None;

        public async Task Run()
        {
            await Task.Run(FlowTask);
        }

        private void FlowTask()
        {
            _pcPosService.StartDiagnostic();
        }
    }
}