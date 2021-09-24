﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using ActivityTracker.Engine;
using ActivityTracker.Models;

namespace ActivityTracker.ViewModels
{
    public class TaskSwitchLog
    {
        public ObservableCollection<TaskSwitch> TaskSwitchLogEntries { get; } =
            new ObservableCollection<TaskSwitch>();

        private TaskSwitch LatestTaskSwitch => 
            TaskSwitchLogEntries.LastOrDefault();

        public void RecordTaskSwitch()
        {
            using Process process = ProcessService.GetActiveProcess();

            if(process == null)
            {
                EndCurrentTask();
                return;
            }

            // Don't record time for the ActivityTracker app
            // TODO: Maybe make this a configurable setting
#if DEBUG
            if(process.Id == System.Environment.ProcessId)
            {
                return;
            }
#endif

            // Don't add an entry if we are "switching" to the same process
            // or switching to a program with no title (window)
            if(LatestTaskSwitch?.ProcessId == process.Id ||
               string.IsNullOrWhiteSpace(process.MainWindowTitle))
            {
                return;
            }

            EndCurrentTask();

            TaskSwitchLogEntries.Add(new TaskSwitch(process.Id, process.ProcessName, process.MainWindowTitle));
        }

        public void EndCurrentTask()
        {
            LatestTaskSwitch?.EndTaskFocus();
        }
    }
}