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

            if (DataManager.Sql.CheckCreateSuccess())
            {
                Sender_InstallButton.Content = DataManager.CurrentStatus.ToString();
                DataManager.Sql.CreateDB_Query();
            }
            if (DataManager.Sql.CheckCreateSuccess())
            {
                Sender_InstallButton.Content = DataManager.CurrentStatus.ToString();
                DataManager.Sql.RunCreate_Query();
            }

            if (DataManager.Sql.CheckScriptSuccess()) // && CheckMSISuccess
            {
                if (!DataManager.ConnectionManager_Prop.CheckMSISuccess())
                {
                    Sender_InstallButton.Content = DataManager.CurrentStatus.ToString();
                    DataManager.ConnectionManager_Prop.RunMSIRemote();
                    Sender_InstallButton.Content = DataManager.CurrentStatus.ToString();
                }
                else
                {
                    DataManager.CurrentStatus = DataManager.InstallStatus.AwaitingVCC;
                    Sender_InstallButton.Content = DataManager.CurrentStatus.ToString();
                }
            }

            if (DataManager.CurrentStatus.ToString() == "AwaitingVCC")
            {
                if (DataManager.Sql.CheckVCCSuccess())
                {
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
