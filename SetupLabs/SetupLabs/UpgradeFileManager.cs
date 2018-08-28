using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace SetupLabs
{
    public class UpgradeFileManager
    {

        private string _InstallFiles;
        private string _MSIFile;
        private List<string> _Versions;
        private string[] _CreateDatabase_Query;

        public string InstallFiles
        {
            get { return _InstallFiles; }
            private set { _InstallFiles = value; }
        }
        public string MSIFile
        {
            get { return _MSIFile; }
            private set { _MSIFile = value; }
        }
        public List<string> Versions
        {
            get { return _Versions; }
            private set { _Versions = value; }
        }
        public string[] CreateDatabase_Query
        {
            get { return _CreateDatabase_Query; }
            private set { _CreateDatabase_Query = value;}
        }

        private string RemotePath = @"\\PRIMARY\Users\administrator\Desktop\AutoVersionUpdate";
        private string DefaultSQLFolder = @"\Database Scripts\MSSQL\";
        private string MSIFileName = "VenafiTPPInstallx64.msi";

        public void ListRemoteVersions()
        {
            Versions = new List<string>();
            if (Directory.Exists(RemotePath))
            {
                Versions = Directory.GetFiles(RemotePath,"*" +DataManager.ServerSetup_Prop.Version_Prop + "*").ToList();
            }
        }

        public void UnzipUpgradeFolder(string Filename)
        {
            string FolderName = RemotePath + "\\" + Filename.Substring(Filename.LastIndexOf('\\') + 1).Replace(".zip", "");
            if (!Directory.Exists(FolderName))
            { 
                try
                {
                    ZipFile.ExtractToDirectory(Filename, FolderName);
                    DataManager.CurrentStatus = DataManager.InstallStatus.Unzipping;
                    InstallFiles = FolderName;
                }
                catch (Exception EX)
                {
                    Console.WriteLine(EX.Message);
                } finally
                {
                    GetDatabaseCreateScript();
                }
            }
            else
            {
                InstallFiles = FolderName;
                GetMSILocation();
                GetDatabaseCreateScript();
            }
        }

        private string[] ReadContent(string CreateScript_Path)
        {
            string[] FileLines;
            FileLines = File.ReadAllLines(CreateScript_Path);
            return FileLines;
        }

        private void GetDatabaseCreateScript()
        {
            string SqlDir = InstallFiles + DefaultSQLFolder;
            string[] CreateScript;
            if (Directory.Exists(SqlDir))
            {
                CreateScript = Directory.GetFiles(SqlDir,"CreateDB.sql");
                if (CreateScript.Length >= 1)
                {
                    if (File.Exists(CreateScript[0]))
                    {
                        CreateDatabase_Query = ReadContent(CreateScript[0]);
                    }
                }
            }
        }

        private void GetMSILocation()
        {
            if (File.Exists(InstallFiles + "\\" + MSIFileName))
            {
                string FormatInstallFiles = InstallFiles.Replace("\\\\PRIMARY\\", "C:\\");
                MSIFile = FormatInstallFiles + "\\" +  MSIFileName;
            }
        }

        public UpgradeFileManager()
        {

        }
    }
}
