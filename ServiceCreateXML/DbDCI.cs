using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Octopus.Library.Utils;

namespace ServiceCreateXML
{
    internal class DbDCI
    {
        public static string Server;
        public static string Database;
        public static string User;
        public static string Password;
        public static string LastErrMsg;

        public static bool isIntegrated;

        public static int CommandTimeout = 30;

        public static void LoadParameter()
        {
#if DEBUG
            Config config = new Config(AppDomain.CurrentDomain.BaseDirectory + @"CONFIG.xml");

#else
            Config config = new Config(AppDomain.CurrentDomain.BaseDirectory + @"CONFIG.xml");

#endif

            string integrated = config.GetValue("Integrated").ToLower();

            if (integrated == "true")
            {
                isIntegrated = true;
                Server = config.GetValue("RemoteServer");
                Database = config.GetValue("RemoteDatabase");
            }
            else
            {
                isIntegrated = false;
                Server = config.GetValue("LocalServerDCI");
                Database = config.GetValue("LocalDatabaseDCI");
                User = config.GetValue("LocalUserDCI");
                Password = config.GetValue("LocalPasswordDCI");
            }
        }

        public static string getConnectionString()
        {
            if (isIntegrated == true)
                return "Data Source=" + Server +
                    ";Database=" + Database +
                    ";Integrated Security=SSPI;";
            else
                return "Data Source=" + Server +
                    ";Initial Catalog=" + Database +
                    ";User ID=" + User +
                    ";Password=" + Password;
        }

        public static int runCommandNonTransactional(string queryString)
        {
            int ret = 0;

            //clear last error
            LastErrMsg = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(getConnectionString()))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                ret = -1;
                LastErrMsg = ex.Message;
            }

            return ret;
        }

        public static long getNewTranNumber(string Table, string Field)
        {
            long TranNr = 0;

            //clear last error
            LastErrMsg = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(getConnectionString()))
                {
                    string queryString = "SELECT MAX(" + Field + ") AS MaxTran FROM " + Table;
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader.IsDBNull(reader.GetOrdinal("MaxTran")) == true)
                            TranNr = 0;
                        else
                            TranNr = Convert.ToInt64(reader["MaxTran"]) + 1;
                    }

                    reader.Close();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                LastErrMsg = ex.Message;
            }

            return TranNr;
        }

        public static int runCommand(List<string> queryCommand)
        {
            string[] tempComand = new string[queryCommand.Count];

            for (int i = 0; i < queryCommand.Count; i++)
                tempComand[i] = queryCommand[i];

            return runCommand(tempComand);
        }

        public static int runCommand(string queryCommand)
        {
            string[] tempComand = new string[1];

            tempComand[0] = queryCommand;

            return runCommand(tempComand);
        }

        public static int runCommand(string[] queryCommand)
        {
            int ErrNumber = 0;
            int idx = 0;

            //clear last error
            LastErrMsg = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(getConnectionString()))
                {
                    connection.Open();

                    // Start a local transaction.
                    SqlTransaction SqlTran = connection.BeginTransaction();

                    // Enlist the command in the current transaction.
                    SqlCommand command = connection.CreateCommand();
                    command.CommandTimeout = CommandTimeout; //default 30 seconds                   
                    command.Transaction = SqlTran;

                    try
                    {
                        for (int i = 0; i < queryCommand.Length; i++)
                        {
                            if (string.IsNullOrEmpty(queryCommand[i]) == false)
                            {
                                idx = i; //current pos

                                command.CommandText = queryCommand[i];
                                command.ExecuteNonQuery();
                            }
                        }

                        SqlTran.Commit();
                    }
                    catch (Exception ex)
                    {
                        SqlTran.Rollback();

                        ErrNumber = -1;
                        LastErrMsg = ex.Message;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                ErrNumber = -2;
                LastErrMsg = ex.Message;
            }

            return ErrNumber;
        }

        //safe from sql injection
        public static bool isRecordExist(string Table, string Field, string Key)
        {
            string queryString = "SELECT TOP 1 * FROM " + Table + " WHERE " + Field + "=@" + Field;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("@" + Field, Key);

            return isRecordExist(queryString, parameter);
        }

        //safe from sql injection
        public static bool isRecordExist(string queryString, Dictionary<string, string> parameter)
        {
            bool result = false;

            //clear last error
            LastErrMsg = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(getConnectionString()))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.CommandTimeout = CommandTimeout; //default 30 seconds    
                    connection.Open();

                    //add parameter
                    foreach (var p in parameter)
                    {
                        command.Parameters.Add(new SqlParameter(p.Key, p.Value));
                    }

                    SqlDataReader reader = command.ExecuteReader();

                    result = reader.HasRows;

                    reader.Close();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                LastErrMsg = ex.Message;
            }

            return result;
        }

        public static bool isRecordExist(string queryString)
        {
            bool result = false;

            //clear last error
            LastErrMsg = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(getConnectionString()))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.CommandTimeout = CommandTimeout; //default 30 seconds    
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    result = reader.HasRows;

                    reader.Close();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                LastErrMsg = ex.Message;
            }

            return result;
        }

        //safe from sql injection
        public static string getFieldValue(string Table, string Field, string FieldCriteria, string Key)
        {
            string queryString = "SELECT TOP 1 " + Field + " FROM " + Table + " WHERE " + FieldCriteria + "=@" + FieldCriteria;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("@" + FieldCriteria, Key);

            return getFieldValue(queryString, parameter);
        }

        //safe from sql injection
        public static string getFieldValue(string queryString, Dictionary<string, string> parameter)
        {
            string result = "";

            //clear last error
            LastErrMsg = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(getConnectionString()))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.CommandTimeout = CommandTimeout; //default 30 seconds    
                    connection.Open();

                    //add parameter
                    foreach (var p in parameter)
                    {
                        command.Parameters.Add(new SqlParameter(p.Key, p.Value));
                    }

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        result = reader[0].ToString();
                    }

                    reader.Close();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                LastErrMsg = ex.Message;
            }

            return result;
        }

        public static string getFieldValue(string queryString)
        {
            string result = "";

            //clear last error
            LastErrMsg = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(getConnectionString()))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.CommandTimeout = CommandTimeout; //default 30 seconds    
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        result = reader[0].ToString();
                    }

                    reader.Close();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                LastErrMsg = ex.Message;
            }

            return result;
        }

        public static string[] getColumns(string queryString)
        {
            string[] colArray = new string[1];

            //clear last error
            LastErrMsg = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(getConnectionString()))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.CommandTimeout = CommandTimeout; //default 30 seconds    
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    Array.Resize(ref colArray, reader.FieldCount);

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        colArray[i] = reader.GetName(i);
                    }

                    reader.Close();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                LastErrMsg = ex.Message;
            }

            return colArray;
        }

        //safe from sql injection
        public static DataRow getRow(string query, Dictionary<string, string> parameter)
        {
            DataTable dt = getRecords(query, parameter);
            DataRow rec = null;

            if (dt.Rows.Count > 0)
            {
                rec = dt.Rows[0];
            }

            return rec;
        }

        public static DataRow getRow(string query)
        {
            DataTable dt = getRecords(query);
            DataRow rec = null;

            if (dt.Rows.Count > 0)
            {
                rec = dt.Rows[0];
            }

            return rec;
        }

        public static DataTable getRecordFromTable(string tbl)
        {
            return getRecords("SELECT * FROM " + tbl);
        }

        // safe from sql injection 
        public static DataTable getRecords(string queryString, Dictionary<string, string> parameter)
        {
            DataTable result = new DataTable();

            //clear last error
            LastErrMsg = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(getConnectionString()))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.CommandTimeout = CommandTimeout; //default 30 seconds    
                    connection.Open();

                    //add parameter
                    foreach (var p in parameter)
                    {
                        command.Parameters.Add(new SqlParameter(p.Key, p.Value));
                    }

                    //Create a SqlDataAdapter
                    SqlDataAdapter adapter = new SqlDataAdapter();

                    // Set the SqlDataAdapter's SelectCommand.
                    adapter.SelectCommand = command;

                    // Fill the DataSet.
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    result = dataSet.Tables[0];

                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                LastErrMsg = ex.Message;
            }

            return result;
        }

        public static DataTable getRecords(string queryString)
        {
            DataTable result = new DataTable();

            //clear last error
            LastErrMsg = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(getConnectionString()))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.CommandTimeout = CommandTimeout; //default 30 seconds                   
                    connection.Open();

                    //Create a SqlDataAdapter
                    SqlDataAdapter adapter = new SqlDataAdapter();

                    // Set the SqlDataAdapter's SelectCommand.
                    adapter.SelectCommand = command;

                    // Fill the DataSet.
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    result = dataSet.Tables[0];

                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                LastErrMsg = ex.Message;
            }

            return result;
        }
    }
}
