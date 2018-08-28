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

namespace SetupLabs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeModules();
            InitializeComponent();
        }
        public void InitializeModules()
        {
            if (DataManager.Sql == null)
            {
                DataManager.Sql = new SQLManagement();
            }
            if (DataManager.UpgradeFileManager_Prop == null)
            {
                DataManager.UpgradeFileManager_Prop = new UpgradeFileManager();
            }
            if (DataManager.ProcessManager_Prop == null)
            {
                DataManager.ProcessManager_Prop = new ProcessManager();
            }
            if (DataManager.ConnectionManager_Prop == null)
            {
                DataManager.ConnectionManager_Prop = new ConnectionManager();
            }
        }
    }
}
