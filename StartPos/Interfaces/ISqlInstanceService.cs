using System.Collections.Generic;

namespace StartPos.Interfaces
{
    public interface ISqlInstanceService
    {
        bool IsInstanceRunning(string serviceName);

        void TryRunInstance(string serviceName);

        bool IsInstanceExist(string serviceName);

        List<string> GetAllInstance();
    }
}