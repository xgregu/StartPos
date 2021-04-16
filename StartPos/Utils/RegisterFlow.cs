using StartPos.Flows;
using StartPos.Interfaces;
using StartPos.Setup;
using StartPos.Shared;
using Unity;

namespace StartPos.Utils
{
    internal class RegisterFlow
    {
        private readonly IUnityContainer _container;
        private readonly IContext _context;

        public RegisterFlow(IUnityContainer container, IContext context)
        {
            _container = container;
            _context = context;
        }

        public void RegistrationFlow()
        {
            switch (_context.ActiveFlow)
            {
                case Constants.Flow.Main:
                    _container.RegisterSingleton<IFlow, MainFlow>();
                    break;

                case Constants.Flow.Diagnostic:
                    _container.RegisterSingleton<IFlow, DiagnosticFlow>();
                    break;

                case Constants.Flow.Repair:
                    _container.RegisterSingleton<IFlow, RepairFlow>();
                    break;

                case Constants.Flow.Restart:
                case Constants.Flow.Auto:
                case Constants.Flow.Shutdown:
                    _container.RegisterSingleton<IFlow, RestartFlow>();
                    break;

                case Constants.Flow.Disable:
                    _container.RegisterSingleton<IFlow, DisableFlow>();
                    break;

                case Constants.Flow.Setup:
                case Constants.Flow.SetupSilent:
                    _container.RegisterSingleton<IFlow, SetupFlow>();
                    break;

                case Constants.Flow.Test:
                    _container.RegisterSingleton<IFlow, TestFlow>();
                    break;

                case Constants.Flow.BuildInstaller:
                    _container.RegisterSingleton<IFlow, BuildInstallerFlow>();
                    break;

                default:
                    _container.RegisterSingleton<IFlow, MainFlow>();
                    break;
            }
        }
    }
}