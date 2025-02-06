using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using NLog;

namespace VPDLFramework.Models
{
    public class ECSQLiteDataManager
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        public enum DataType
        {
            WorkLog,
            ResultData
        }

        /// <summary>
        /// 创建数据库文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        private static bool CreateFile(string path,DataType type)
        {
            try
            {
                if (!File.Exists(path))
                {
                    if(!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
                    SQLiteConnection.CreateFile(path);
                    string connectionString = "Data Source=" + path + ";Version=3;";
                    SQLiteConnection conn = new SQLiteConnection(connectionString);
                    conn.Open();

                    string cmdText = "";
                    if(type == DataType.ResultData) 
                        cmdText = @"CREATE TABLE DATA (
                                                        Time TEXT,
                                                        TriggerIndex INT, 
                                                        ResultForDisplay TEXT,
                                                        ResultForSend TEXT,
                                                        ResultStatus INT)";
                    else
                        cmdText = @"CREATE TABLE DATA (
                                                        Time TEXT,
                                                        LogType TEXT, 
                                                        LogContent TEXT)";


                    SQLiteCommand cmd = new SQLiteCommand(cmdText,conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                return true;
            }
            catch(Exception ex) 
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message+"Create Database File Failed", LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 添加数据到数据库文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static bool AddData(string path,DataType type, object[] data)
        {
            try
            {
                if(!File.Exists(path)) CreateFile(path,type);

                if (File.Exists(path))
                {
                    string connectionString = "Data Source=" + path + ";Version=3;";
                    SQLiteConnection conn = new SQLiteConnection(connectionString);
                    conn.Open();

                    string cmdText = "";
                    if(type == DataType.ResultData)
                        cmdText = $"INSERT INTO DATA VALUES('{(string)data[0]}', '{(int)data[1]}', '{(string)data[2]}', '{(string)data[3]}','{(int)data[4]}')";
                    else
                        cmdText = $"INSERT INTO DATA VALUES('{(string)data[0]}', '{(string)data[1]}', '{(string)data[2]}')";

                    SQLiteCommand cmd = new SQLiteCommand(cmdText, conn);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                return true;
            }
            catch(Exception ex) 
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message + "Add Data To Database Failed", LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 查询工作流数据库文件中对应结果状态的数据
        /// </summary>
        /// <param name="_path">文件路径</param>
        /// <returns></returns>
        public static DataView QueryByResultStatus(string _path, DataType type=DataType.ResultData,int status=1)
        {
            if (type != DataType.ResultData) return null;
            DataView data = null;
            if (File.Exists(_path))
            {
                string connectionString = "Data Source=" + _path + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connectionString);
                conn.Open();

                string cmdText = $"SELECT * FROM DATA WHERE ResultStatus = {status}";

                try
                {
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmdText, conn))
                    {
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        data = dataTable.AsDataView();
                    }
                }
                catch (Exception ex)
                {
                    ECLog.WriteToLog(ex.StackTrace+ex.Message, LogLevel.Error);
                }
                conn.Close();
            }
            return data;
        }

        /// <summary>
        /// 查询数据库文件所有数据
        /// </summary>
        /// <param name="_path">文件路径</param>
        /// <returns></returns>
        public static DataView QueryAll(string _path)
        {
            DataView data=null;
            if (File.Exists(_path))
            {
                string connectionString= "Data Source=" + _path + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connectionString);
                conn.Open();

                string cmdText = @"SELECT * FROM DATA";

                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmdText, conn))
                {
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    data = dataTable.AsDataView();
                }
                conn.Close();
            }
            return data;
        }

        /// <summary>
        /// 删除数据库文件所有数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ClearAll(string path)
        {
            if (File.Exists(path))
            {
                string connectionString = "Data Source=" + path + ";Version=3;";
                SQLiteConnection _conn = new SQLiteConnection(connectionString);
                _conn.Open();

                string cmdText = @"DElETE FROM DATA";

                SQLiteCommand cmd = new SQLiteCommand(cmdText, _conn);
                cmd.ExecuteNonQuery();
                _conn.Close();
            }
            return true;
        }
    }
}
