using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comissar2
{
    public class TaskModel
    {
        public string command { get; set; } = "";
        public DateTime nextExecution { get; set; }
        public bool cyclic { get; set; } = false;
        public TimeSpan executionInterval { get; set; }
        public int executionCount { get; set; } = 0;
        public string lastResult { get; set; } = "Not yet executed";
        public TasksViewModel parent { get; set; }

        public string ToolTipText { get { return (this.nextExecution - DateTime.Now).ToString();  } }

        public TaskModel() { }

        public bool ShouldBeExecuted()
        {
            return nextExecution <= DateTime.Now && (cyclic || executionCount==0);
        }

        public void scheduleNextExecution()
        {
            nextExecution = nextExecution + executionInterval;
        }

        public async void execute()
        {
            var proc = new Process();
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.FileName = "CMD.exe";
            proc.StartInfo.Arguments = "/C " + command;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();
            string output = proc.StandardOutput.ReadToEnd();
            lastResult = output;
            executionCount++;
            if (cyclic) {
                scheduleNextExecution();
            }

        }
    }
}
