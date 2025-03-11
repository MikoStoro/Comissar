using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Comissar2
{
    public class TasksViewModel : BaseViewModel
    {
        List<TaskModel> _tasks;
        public List<TaskModel> TasksList
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(TasksList));
            }
        }

        Task runner;
        internal ListSortDirection? gridSortDirection = null;
        internal string gridSortColumn = null;

        public CollectionViewSource ViewSource { get; set; } = new CollectionViewSource();

        public ICommand refreshCommand { get; set; }
        public ICommand createTask { get; set; }
        public ICommand removeTask { get; set; }

        public bool currentTaskCyclic { get; set; } = false;
        public int currentTaskStart { get; set; } = 0;
        public string curentTaskStartString { get { return currentTaskStart.ToString(); } set { this.currentTaskStart = int.Parse(value); } }
        public int currentTaskRepeat { get; set; } = 60;
        public string currentTaskRepeatString { get { return currentTaskRepeat.ToString(); } set { this.currentTaskRepeat = int.Parse(value); } }
        public string currentTaskCommand { get; set; }

        private static readonly object tasksLock = new object();

        public TasksViewModel(){
            var tempTasks = new List<TaskModel>();

            this.refreshCommand = new RelayCommand(o => RefreshCollection());
            this.createTask = new RelayCommand(o => CreateTask());
            this.removeTask = new RelayCommand(o => RemoveTask(o));

            tempTasks.Add(new TaskModel()
            {
                command = "tree",
                nextExecution = DateTime.Now,
                cyclic = false,
                parent = this,
            });
            tempTasks.Add(new TaskModel()
            {
                command = "cd /",
                nextExecution = DateTime.Now + TimeSpan.FromMinutes(5),
                cyclic = true,
                executionInterval = TimeSpan.FromMinutes(5),
                parent = this,
            });

            tempTasks.Add(new TaskModel()
            {
                command = "TIMEOUT /T 1",
                nextExecution = DateTime.Now + TimeSpan.FromSeconds(3),
                cyclic = true,
                executionInterval = TimeSpan.FromSeconds(1),
                parent = this,
            });
            this.TasksList = new List<TaskModel>(tempTasks);

            this.ViewSource.Source = this.TasksList;
            runner = new Task(RunnerFunc);
            runner.Start();

        }

        private void CreateTask()
        {
            lock (this.TasksList)
            {
                var newTask = new TaskModel();
                newTask.command = currentTaskCommand;
                newTask.nextExecution = DateTime.Now + TimeSpan.FromSeconds(currentTaskStart);
                newTask.cyclic = currentTaskCyclic;
                newTask.executionInterval = TimeSpan.FromSeconds(currentTaskRepeat);
                newTask.parent = this;
                this.TasksList.Add(newTask);
                RefreshCollection();
            }
          
        }

        void RefreshCollection()
        {
            var templist = this.TasksList.ToList();
            this.TasksList = new List<TaskModel>(templist);
            this.ViewSource.Source = this.TasksList;
        }

        void RemoveTask(object t)
        {
            lock (this.TasksList)
            {
                TaskModel task = (TaskModel)t;
                this.TasksList.Remove(task);
                RefreshCollection();
            }
        }

        void RunnerFunc()
        {
            while (true)
            {
                Thread.Sleep(1000);
                lock(this.TasksList)
                foreach (var task in this.TasksList)
                {
                    if (task.ShouldBeExecuted())
                    {
                        task.execute();
                        OnPropertyChanged(nameof(this.TasksList));
                    }
                }
                
            }
        }

}
}
