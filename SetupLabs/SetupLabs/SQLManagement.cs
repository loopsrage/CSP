using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace SetupLabs
{
    class SQLManagement
    {
        private string _ConnectionString;
        private string _DatabaseName;
        public void FormatDatabaseName()
        {
            string SelectedVersion = DataManager.ServerSetup_Prop.Version_Prop;
            string FolderName = SelectedVersion.Substring(SelectedVersion.LastIndexOf('\\') + 1);
            string VersionName = FolderName.Substring(FolderName.LastIndexOf(' ') + 1).Replace(".zip", "").Replace('.','_');
            _DatabaseName = "Director_" + VersionName;
        }
        private string FormatQueryFromFile()
        {
            StringBuilder QueryBuilder = new StringBuilder();
            foreach (String Line in DataManager.UpgradeFileManager_Prop.CreateDatabase_Query)
            {
                QueryBuilder.AppendLine(Line);
            }
            return QueryBuilder.ToString();
        }

        public void CreateDB_Query()
        {
            _ConnectionString = "Server="+ DataManager.ServerSetup_Prop.FQDN_Prop +";Database=Master;User ID=sa;Password=Passw0rd";
            string CreateDatabase_Query = "Create Database " + _DatabaseName;
            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand CMD = new SqlCommand())
                    {
                        DataManager.CurrentStatus = DataManager.InstallStatus.CreateDatabase;
                        CMD.Connection = Conn;
                        CMD.CommandType = System.Data.CommandType.Text;
                        CMD.CommandText = CreateDatabase_Query;
                        CMD.ExecuteReader();
                    }
                }
                catch (SqlException SQE)
                {
                    Console.WriteLine(SQE.Message);
                }
                finally
                {
                    Conn.Close();
                }
            }
        }

       
        public void RunCreate_Query()
        {
            _ConnectionString = "Server="+ DataManager.ServerSetup_Prop.FQDN_Prop +";Database=" + _DatabaseName + ";User ID=sa;Password=Passw0rd";
            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            {
                try
                {
                    Conn.Open();
                    Server DB = new Server(new ServerConnection(Conn));
                    string Script = FormatQueryFromFile();
                    DB.ConnectionContext.ExecuteNonQuery(Script);
                    //using (SqlCommand CMD = new SqlCommand())
                    //{
                    //    DataManager.CurrentStatus = DataManager.InstallStatus.CreateDatabaseTables;
                    //    CMD.Connection = Conn;
                    //    CMD.CommandType = System.Data.CommandType.Text;
                    //    CMD.CommandText = FormatQueryFromFile();
                    //    CMD.ExecuteNonQuery();
                    //}
                }
                catch (SqlException SQE)
                {
                    Console.WriteLine(SQE.Message);
                }
                finally
                {
                    Conn.Close();
                }
            }
        }
        public bool CheckCreateSuccess()
        {
            bool Status;
            _ConnectionString = "Server="+ DataManager.ServerSetup_Prop.FQDN_Prop +";Database=Master;User ID=sa;Password=Passw0rd";
            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            {
                string CheckDBQuery = "Select db_id('{"+_DatabaseName+"}')";
                try
                {
                    Conn.Open();
                    using (SqlCommand CMD = new SqlCommand())
                    {
                        CMD.Connection = Conn;
                        CMD.CommandType = System.Data.CommandType.Text;
                        CMD.CommandText = CheckDBQuery;
                        return (CMD.ExecuteScalar() == DBNull.Value);
                    }
                }
                catch (SqlException SQE)
                {
                    Status = false;
                    Console.WriteLine(SQE.Message);
                }
                finally
                {
                    Conn.Close();
                }
            }
            return Status;
        }
        public bool CheckScriptSuccess()
        {
            bool Status;
            _ConnectionString = "Server="+ DataManager.ServerSetup_Prop.FQDN_Prop +";Database=" + _DatabaseName + ";User ID=sa;Password=Passw0rd";
            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand CMD = new SqlCommand())
                    {
                        CMD.Connection = Conn;
                        CMD.CommandType = System.Data.CommandType.Text;
                        CMD.CommandText = "Select * From DB_Schema_Version WHERE ComponentName = 'CORE'";
                        using (SqlDataReader SDR = CMD.ExecuteReader())
                        {
                            int Stage = SDR.GetOrdinal("Stage");
                            int TgtStage = SDR.GetOrdinal("TgtStage");
                            if (SDR.Read())
                            {
                                if (SDR.GetInt32(Stage) == SDR.GetInt32(TgtStage))
                                {
                                    Status = true;
                                }
                                else
                                {
                                    Status = false;
                                }
                            }
                            else
                            {
                                Status = false;
                            }
                        }
                    }
                }
                catch (SqlException SQE)
                {
                    Status = false;
                    Console.WriteLine(SQE.Message);
                } finally
                {
                    Conn.Close();
                }
            }
            return Status;
        }

        public bool CheckVCCSuccess()
        {
            bool Status;
            _ConnectionString = "Server="+ DataManager.ServerSetup_Prop.FQDN_Prop +";Database=" + _DatabaseName + ";User ID=sa;Password=Passw0rd";
            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            {
                try
                {
                    Conn.Open();
                    using (SqlCommand CMD = new SqlCommand())
                    {
                        CMD.Connection = Conn;
                        CMD.CommandType = System.Data.CommandType.Text;
                        CMD.CommandText = "SELECT Count(*) as ItemCount FROM Config_Objects where Name Like '%Operational Certificate%'";
                        using (SqlDataReader SQR = CMD.ExecuteReader())
                        {
                            int ItemCount;
                            int ItemIndex = SQR.GetOrdinal("ItemCount");
                            if (SQR.Read())
                            {
                                ItemCount = (int)SQR.GetValue(ItemIndex);
                            }
                            else
                            {
                                ItemCount = 0;
                            }
                            if (ItemCount == 1)
                            {
                                Status = true;
                            }
                            else
                            {
                                Status = false;
                            }
                        }
                    }
                    return Status;
                }
                catch (SqlException SQE)
                {
                    Status = false;
                    Console.WriteLine(SQE.Message);
                }
                finally
                {
                    Conn.Close();
                }
                return Status;
            }
        }
    }
}
