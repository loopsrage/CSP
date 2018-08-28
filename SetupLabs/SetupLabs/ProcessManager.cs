using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

namespace SetupLabs
{
    class ProcessManager
    {
        public Process process;
        public void RunMSI()
        {
            process = new Process();
            process.StartInfo.FileName = DataManager.UpgradeFileManager_Prop.MSIFile;
            process.Start();
            process.WaitForExit(60000);

        }
        public ProcessManager()
        {

        }
    }
}
