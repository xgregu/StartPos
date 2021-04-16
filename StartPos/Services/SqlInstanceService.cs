using NLog;
using StartPos.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace StartPos.Services
{
    internal class SqlInstanceService : ISqlInstanceService
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public bool IsInstanceRunning(string serviceName)
        {
            var nameSqlInstanceService = $"MSSQL${serviceName}";

            if (!IsInstanceExist(serviceName))
                return false;

            var service = ServiceController.GetServices()
                .FirstOrDefault(x => x.ServiceName.ToUpper().Contains(nameSqlInstanceService.ToUpper()));

            return service?.Status == ServiceControllerStatus.Running;
        }

        public void TryRunInstance(string serviceName)
        {
            var nameSqlInstanceService = $"MSSQL${serviceName}";

            if (!IsInstanceExist(serviceName))
                return;

            var service = ServiceController.GetServices()
                .FirstOrDefault(x => x.ServiceName.ToUpper().Contains(nameSqlInstanceService.ToUpper()));

            try
            {
                _logger.Info($"{nameof(TryRunInstance)} | Instance {serviceName} trying force startup");
                service?.Start();
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Instance {serviceName} force startup failed");
            }
        }

        public bool IsInstanceExist(string serviceName)
        {
            var nameSqlInstanceService = $"MSSQL${serviceName}";

            if (string.IsNullOrEmpty(nameSqlInstanceService))
                throw new ArgumentNullException(nameof(nameSqlInstanceService));

            var service = ServiceController.GetServices()
                .FirstOrDefault(x => x.ServiceName.ToUpper().Equals(nameSqlInstanceService.ToUpper()));

            return service != null;
        }

        public List<string> GetAllInstance()
        {
            return ServiceController.GetServices()
                .Where(x => x.ServiceName.ToUpper().Contains("MSSQL$"))
                .Select(x => x.ServiceName.Split('$')[1])
                .ToList();
        }
    }
}