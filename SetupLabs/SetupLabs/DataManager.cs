using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupLabs
{
    static class DataManager
    {
        private static SQLManagement _Sql;
        private static ServerSetup _ServerSetup;
        private static UpgradeFileManager _UpgradeFileManager;
        private static ProcessManager _ProcessManager;
        private static ConnectionManager _ConnectionManager;
        private static InstallStatus _CurrentStatus;

        public static SQLManagement Sql {
            get { return _Sql; }
            set { _Sql = value; }
        }
        public static ServerSetup ServerSetup_Prop
        {
            get { return _ServerSetup; }
            set { _ServerSetup = value; }
        }
        public static UpgradeFileManager UpgradeFileManager_Prop
        {
            get { return _UpgradeFileManager; }
            set { _UpgradeFileManager = value; }
        }
        public static ProcessManager ProcessManager_Prop
        {
            get { return _ProcessManager; }
            set { _ProcessManager = value; }
        }
        public static ConnectionManager ConnectionManager_Prop
        {
            get { return _ConnectionManager; }
            set { _ConnectionManager = value; }
        }
        public static InstallStatus CurrentStatus
        {
            get { return _CurrentStatus; }
            set { _CurrentStatus = value; }
        }

        public enum InstallStatus
        {
            Unzipping,
            CreateDatabase,
            CreateDatabaseTables,
            RunningMSI,
            AwaitingVCC,
            InstallComplete
        }
    }
}
