using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Reflection.Metadata;

namespace Comissar2
{
    public class ProcessViewModel: BaseViewModel
    {
        

        List<ProcessModel> _processes;
        public List<ProcessModel> Processes
        {
            get { return _processes; }
            set
            {
                _processes = value;
                OnPropertyChanged(nameof(Processes));
            }
        }

        List<Process> baseProcesses;
        string filterString = "";

        internal ListSortDirection? gridSortDirection = null;
        internal string gridSortColumn = null;

        public ICommand refreshCommand { get; }
        public ICommand killCommand { get; }
        public ICommand changePriorityCommand { get; }
        public ICommand detailsCommand { get; }
        public ICommand filterCommand { get; }

        public int refreshTimer { get; set; } = 5;
        public bool autorefresh { get; set; } = false;
        Task autoRefresher;

        public ProcessPriorityClass[] priorityClasses { get; } = [ProcessPriorityClass.Idle,
                ProcessPriorityClass.BelowNormal,
                ProcessPriorityClass.Normal,
                ProcessPriorityClass.AboveNormal,
                ProcessPriorityClass.High,
                ProcessPriorityClass.RealTime
        ];

        public ProcessViewModel()
        {
            Refresh();
            this.refreshCommand = new RelayCommand(o => Refresh(),  o => true);
            this.killCommand = new RelayCommand( o => Kill(o), o => o as Process != null);
            this.changePriorityCommand = new RelayCommand(o => ChangePriority(o), o => true);
            this.detailsCommand = new RelayCommand(o => Details(o), o => true);
            this.filterCommand = new RelayCommand(o =>  Filter(o), o=>true);

            autoRefresher = new Task(AutorefreshFunc);
            autoRefresher.Start();

        }

        void AutorefreshFunc()
        {
            int ticks = 0;
            while (true)
            {
                Thread.Sleep(1000);
                if (autorefresh)
                {
                    ticks++;
                    if (ticks % refreshTimer == 0)
                    {
                        ticks = 0;
                        Trace.WriteLine("Next turn called");
                        Refresh();
                    }
                }
            }
        }


        private void SINNER() {
            Window window = new Window
            {
                Title = "SINNER!",
                Content = new YouHaveNoRight(),
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize
            };

            window.ShowDialog();
        }

        private void Details(object o)
        {
            var process = o as Process;
            try
            {
                Window window = new Window
                {
                    Title = "Details: " + process.ProcessName,
                    Content = new Threads(process),
                    SizeToContent = SizeToContent.Width,
                    ResizeMode = ResizeMode.NoResize,
                    Height = 450                };
                window.ShowDialog();
            }
            catch
            {
                if (process == null)
                {
                    SINNER();
                    return;
                }
            }
           
           
        }

        private void ChangePriority(object p)
        {
            try
            {
                var values = (object[])p;
                if (values[0] == null || values[1] == null) { return; }
                Process process = values[0] as Process;
                ProcessPriorityClass desiredClass = (ProcessPriorityClass)values[1];
                //process.PriorityClass = ProcessPriorityClass.Normal;
                if (desiredClass != process.PriorityClass)
                {
                    process.PriorityClass = desiredClass;
                }
            }catch(Exception) { }
           
        }

        private void Filter(object o)
        {
            this.filterString = (string)o;
            Refresh();
        }


        private void Refresh()
        {
            var tempProcesses = new List<ProcessModel>();
            baseProcesses = Process.GetProcesses().ToList();
            foreach (Process p in baseProcesses)
            {
                ProcessModel model = new()
                {
                    parent = this,
                    processObject = p
                };
                tempProcesses.Add(model);
            }
            if(filterString != "")
            {
                tempProcesses = tempProcesses.Where(p => p.processObject.ProcessName.ToLower().Contains(filterString.ToLower().Trim())).ToList();
            }
            if(this.gridSortColumn != null && this.gridSortDirection != null)
            {
                var property = gridSortColumn.Split('.')[1];
                System.Reflection.PropertyInfo prop = typeof(Process).GetProperty(property);
                if (this.gridSortDirection == ListSortDirection.Ascending){
                    tempProcesses = tempProcesses.OrderBy(i =>  prop.GetValue(i.processObject,null) ).ToList();
                }
                else{
                    tempProcesses = tempProcesses.OrderByDescending(i => prop.GetValue(i.processObject, null)).ToList();
                }
            }
            this.Processes = tempProcesses;
        }

        private void Kill(object p)
        {
            var process = (p as Process);
            if (process != null)
            {
                try
                {
                    process.Kill();
                    Refresh();
                }catch (Exception) {
                    SINNER();

               }
    
            }
        }


    }

    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        public void Execute()
        {

        }
    }

    public class MultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

