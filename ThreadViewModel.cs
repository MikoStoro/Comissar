using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comissar2
{
    public class ThreadViewModel : BaseViewModel
    {
        Process process;
        public ProcessThreadCollection threads;
        public ThreadViewModel(Process p) {
            this.process = p;
            this.threads = p.Threads;
        }
        public ThreadViewModel() { }
    }
}
