using StartPos.Enums;

namespace StartPos.Interfaces
{
    public interface ITaskShedulerService
    {
        void ConfigureTaskSheduler();

        bool CheckIsExist(TaskSchedulerType typeName);
    }
}