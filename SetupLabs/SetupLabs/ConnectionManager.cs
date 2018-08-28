using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Management.Instrumentation;

namespace SetupLabs
{
    public class ConnectionManager
    {
        private string _FQDN_Remote;
        private ManagementScope _Scope;


        private string Username = "Administrator";
        private string Password = "Passw0rd";
        private string Domain = "Training1";

        public ManagementScope Scope
        {
            get { return _Scope; }
            set { _Scope = value; }
        }
        public string FQDN_Remote
        {
            get { return _FQDN_Remote; }
            set { _FQDN_Remote = value; }
        }

        public void BuildConnection()
        {
            ConnectionOptions Options = new ConnectionOptions();
            Options.Username = Username;
            Options.Password = Password;
            Options.Impersonation = ImpersonationLevel.Impersonate;
            Scope = new ManagementScope(@"\\" + DataManager.ServerSetup_Prop.FQDN_Prop + "\\root\\cimv2", Options);
            Scope.Connect();
        }

        public void RunMSIRemote()
        {
            if (Scope == null)
            {
                BuildConnection();
            }
            if (!Scope.IsConnected)
            {
                BuildConnection();
            }
            try
            {
                if (Scope.IsConnected)
                {
                    DataManager.CurrentStatus = DataManager.InstallStatus.RunningMSI;
                    ManagementPath Path = new ManagementPath("Win32_Product");
                    using (ManagementClass Class = new ManagementClass(Scope, Path, null))
                    {
                        using (ManagementBaseObject inParam = Class.GetMethodParameters("Install"))
                        {
                            inParam["AllUsers"] = true;
                            inParam["Options"] = string.Empty;
                            inParam["PackageLocation"] = DataManager.UpgradeFileManager_Prop.MSIFile;
                            using (ManagementBaseObject outParams = Class.InvokeMethod("Install", inParam, null))
                            {
                                Console.WriteLine(outParams["ReturnValue"].ToString());
                            };
                        };
                    };
                    DataManager.CurrentStatus = DataManager.InstallStatus.AwaitingVCC;
                }
            }
            catch (ManagementException ME)
            {
                Console.WriteLine(ME.Message);
            } 
        }
        public bool CheckMSISuccess()
        {
            bool Status;
            if (Scope == null)
            {
                BuildConnection();
            }
            if (!Scope.IsConnected)
            {
                BuildConnection();
            }
            try
            {
                if (Scope.IsConnected)
                {
                    ManagementPath Path = new ManagementPath("Win32_Service");
                    ObjectQuery Query =  new ObjectQuery("SELECT * FROM Win32_Service WHERE Name LIKE '%Venafi%'");
                    using (ManagementObjectSearcher Service = new ManagementObjectSearcher(Scope,Query,null))
                    {
                        using (ManagementObjectCollection Services = Service.Get())
                        {
                            if (Services.Count >= 1)
                            {
                                Status = true;
                            }
                            else
                            {
                                Status = false;
                            }
                        }
                    }
                }
                else
                {
                    Status = false;
                }
            }
            catch (ManagementException ME)
            {
                Console.WriteLine(ME.Message);
                Status = false;
            }
            return Status;
        }
        public ConnectionManager()
        {
        }
    }
}
