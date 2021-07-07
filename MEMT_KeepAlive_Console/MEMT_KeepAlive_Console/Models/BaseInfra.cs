using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMT_KeepAlive.Models
{
    class BaseInfra
    {
        public void TableExists()
        {

            SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["IONData"].ConnectionString);
            Tables t = new Tables();
            using (SqlCommand command = new SqlCommand(t.KeepTables, sqlConn))
            {
                command.Connection.Open();
                var result = command.ExecuteScalar();
                command.Connection.Close();
                instertKeepAlive();
                ProcedureExists();


            }
        }
        static void ProcedureExists()
        {
            try
            {

                SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["IONData"].ConnectionString);
                var commandStr = "If not exists (select name from sysobjects where name = 'sp_MEMT_ChangeServerName') SELECT 1 AS res ELSE SELECT 0 AS res;";

                using (SqlCommand command = new SqlCommand(commandStr, sqlConn))
                {
                    Procedure p = new Procedure();
                    command.Connection.Open();
                    var result = command.ExecuteScalar();
                    command.Connection.Close();
                    if (result.Equals(1))
                    {
                        StringBuilder sbSP = new StringBuilder();

                        sbSP.AppendLine(p.MyProcedure);


                        using (SqlCommand cmd = new SqlCommand(sbSP.ToString(), sqlConn))
                        {
                            sqlConn.Open();                           
                            cmd.ExecuteNonQuery();
                            sqlConn.Close();
                        }

                    }


                }


            }
            catch
            {

            }
        }
        static void instertKeepAlive()
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["IONData"].ConnectionString);
                var commandStr = "IF(NOT EXISTS(SELECT 1 FROM dbo.KeepAlive))SELECT 1 AS res ELSE SELECT 0 AS res";
                using (SqlCommand command = new SqlCommand(commandStr, sqlConn))
                {
                    command.Connection.Open();
                    var result = command.ExecuteScalar();
                    command.Connection.Close();

                    if (result.Equals(1))

                    {List<string> _keepAlive = new List<string>();
                        string remote = ConfigurationManager.AppSettings["RemoteServerIP"];
                        string local = ConfigurationManager.AppSettings["LocalServerIP"];
                        _keepAlive.Add(local);
                        _keepAlive.Add(remote);

                        foreach (var keep in _keepAlive)
                        {
                            var commandInsert = "INSERT INTO [dbo].[KeepAlive]([ServerIP])" + "VALUES(" + "'" + keep + "'" + ")";

                            using (SqlCommand commandIn = new SqlCommand(commandInsert, sqlConn))
                            {
                                commandIn.Connection.Open();
                                commandIn.ExecuteScalar();
                                commandIn.Connection.Close();

                            }


                        }


                    }
                    command.Connection.Close();
                }
            }
            catch (Exception e)
            {

            }

        }
    }
}
