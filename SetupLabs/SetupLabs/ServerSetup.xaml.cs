using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace SetupLabs
{
    /// <summary>
    /// Interaction logic for ServerSetup.xaml
    /// </summary>
    public partial class ServerSetup : UserControl
    {
        private string _FQDN;
        private string _Credential;
        private string _Version;
        private StringBuilder _Logging = new StringBuilder();

        public string FQDN_Prop {
            get { return _FQDN; }
            set { _FQDN = value; }
        }
        public string Credential_Prop
        {
            get { return _Credential; }
            set { _Credential = value; }
        }
        public string Version_Prop
        {
            get { return _Version; }
            set { _Version = value; }
        }

        private StringBuilder Logging_Prop
        {
            get { return _Logging; }
            set { _Logging = value; }
        }

        public void UpdateLogView(string LogMessage)
        {
            DateTime CurrentTime = DateTime.Now;
            Logging_Prop.AppendLine();
            Logging_Prop.Append(CurrentTime.ToLocalTime().ToString() + ": ");
            Logging_Prop.Append(LogMessage);
            InstallLogs.Text = Logging_Prop.ToString();
        }

        public ServerSetup()
        {
            InitializeComponent();
            DataManager.ServerSetup_Prop = this;
            DataManager.UpgradeFileManager_Prop.ListRemoteVersions();
            Version.ItemsSource = DataManager.UpgradeFileManager_Prop.Versions;
        }

        private void FQDN_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox FQDN_Text = (TextBox)sender;
            FQDN_Prop = FQDN_Text.Text;
        }

        private void Credential_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox Credential_Text = (TextBox)sender;
            Credential_Prop = Credential_Text.Text;
        }

        private void Begin_Install_Click(object sender, RoutedEventArgs e)
        {
            if (_FQDN == null || _FQDN == string.Empty)
            {
                // Input Validation
                return;
            }

            if (_Version == null || _Version == string.Empty)
            {
                // Input Validation
                return;
            }

            Button Sender_InstallButton = (Button)sender;
            // Format Database names from the Data input
            DataManager.Sql.FormatDatabaseName();
            // Unzip checks if folder is unzipped in the method

            DataManager.UpgradeFileManager_Prop.UnzipUpgradeFolder(Version_Prop);

            if (!DataManager.Sql.CheckCreateSuccess())
            {
                UpdateLogView("Running Create Database query");
                Sender_InstallButton.Content = DataManager.CurrentStatus.ToString();
                DataManager.Sql.CreateDB_Query();
                UpdateLogView("Done Running Create Database Query");
            }

            if (DataManager.Sql.CheckCreateSuccess())
            {
                if (!DataManager.Sql.CheckScriptSuccess())
                {
                    UpdateLogView("Running CreateDatabase.sql");
                    Sender_InstallButton.Content = DataManager.CurrentStatus.ToString();
                    DataManager.Sql.RunCreate_Query();
                    UpdateLogView("Done Running CreateDatabase.sql");
                }
            }

            if (DataManager.Sql.CheckScriptSuccess()) // && CheckMSISuccess
            {
                if (!DataManager.ConnectionManager_Prop.CheckMSISuccess())
                {
                    UpdateLogView("Running MSI");
                    Sender_InstallButton.Content = DataManager.CurrentStatus.ToString();
                    DataManager.ConnectionManager_Prop.RunMSIRemote();
                    Sender_InstallButton.Content = DataManager.CurrentStatus.ToString();
                    UpdateLogView("Done Running MSI");
                }
                else
                {
                    DataManager.CurrentStatus = DataManager.InstallStatus.AwaitingVCC;
                    Sender_InstallButton.Content = DataManager.CurrentStatus.ToString();
                }
            }

            if (DataManager.CurrentStatus.ToString() == "AwaitingVCC")
            {
                UpdateLogView("Waiting for user to run VCC");
                if (DataManager.Sql.CheckVCCSuccess())
                {
                    UpdateLogView("VCC Completed by the user");
                    DataManager.CurrentStatus = DataManager.InstallStatus.InstallComplete;
                    Sender_InstallButton.Content = DataManager.CurrentStatus.ToString();
                }
            }
        }

        private void Version_TextChanged(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox Version_Text = (AutoCompleteBox)sender;
            Version_Prop = Version_Text.Text;
        }
    }
}
