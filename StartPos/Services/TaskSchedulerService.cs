using Microsoft.Win32.TaskScheduler;
using NLog;
using StartPos.Enums;
using StartPos.Interfaces;
using StartPos.Shared;
using System;
using System.IO;
using System.Linq;

namespace StartPos.Services
{
    internal class TaskSchedulerService : ITaskShedulerService
    {
        private readonly IContext _context;
        private readonly ILogger _logger;
        private readonly ILogger _windowLogger;

        public TaskSchedulerService(IContext context)
        {
            _context = context;
            _logger = LogManager.GetCurrentClassLogger();
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
        }

        public void ConfigureTaskSheduler()
        {
            AddTaskSchedulerAutoStart();
            AddTaskSchedulerRestart();
        }

        private void AddTaskSchedulerAutoStart() => CreateTaskSheduler(TaskSchedulerType.AutoStart, new LogonTrigger(), Path.Combine(Constants.BaseDir, "StartPos.exe"), null, Constants.BaseDir);

        private void AddTaskSchedulerRestart()
        {
            var dailyTrigger = new DailyTrigger
            {
                StartBoundary = DateTime.Today + TimeSpan.FromHours(1),
                DaysInterval = 1
            };
            CreateTaskSheduler(TaskSchedulerType.Restart, dailyTrigger, Path.Combine(Constants.BaseDir, "StartPos.exe"), "auto", Constants.BaseDir);
        }

        private void CreateTaskSheduler(TaskSchedulerType typeName, Trigger trigger, string appToStartName, string appToStartArgs = null, string appToStartWorkingDir = null)
        {
            try
            {
                _windowLogger.Info($"Konfiguracja harmonogramu zadań Windows - {typeName}");
                _logger.Info($"{nameof(CreateTaskSheduler)} | Windows task schedule configuration - {typeName}");
                using (var taskScheduler = TaskService.Instance.NewTask())
                {
                    taskScheduler.RegistrationInfo.Description = $"StartPos v.{_context.AppVersion} - {typeName}";
                    taskScheduler.Triggers.Add(trigger);
                    taskScheduler.Actions.Add(appToStartName, appToStartArgs, appToStartWorkingDir);
                    taskScheduler.Settings.StopIfGoingOnBatteries = false;
                    taskScheduler.Settings.DisallowStartIfOnBatteries = false;
                    TaskService.Instance.RootFolder.RegisterTaskDefinition($"StartPos - {typeName}", taskScheduler);
                }
            }
            catch (Exception ex)
            {
                _windowLogger.Error($"Błąd konfiguracji harmonogramu zadań Windows - {typeName}. Szczegóły w log.");
                _logger.Error(ex, $"{nameof(CreateTaskSheduler)} | Error configuration scheduler task Windows - {typeName}.");
            }
        }

        public bool CheckIsExist(TaskSchedulerType typeName)
        {
            using (var service = new TaskService())
            {
                var test = service.RootFolder.AllTasks.Any(t => t.Name == $"StartPos - {typeName}");
                return service.RootFolder.AllTasks.Any(t => t.Name == $"StartPos - {typeName}");
            }
        }
    }
}