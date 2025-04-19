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
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    SQLiteConnection.CreateFile(path);
                    using (SQLiteConnection conn = new SQLiteConnection($"Data Source={path};Version=3;"))
                    {
                        conn.Open();
                        string cmdText = type == DataType.ResultData ? @"CREATE TABLE DATA (Time TEXT,TriggerIndex INT, ResultForDisplay TEXT,ResultForSend TEXT,ResultStatus INT)" : @"CREATE TABLE DATA (Time TEXT, LogType TEXT, LogContent TEXT)";
                        new SQLiteCommand(cmdText, conn).ExecuteNonQuery();
                    }
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
                CreateFile(path, type);
                using (SQLiteConnection conn = new SQLiteConnection($"Data Source={path};Version=3;"))
                {
                    conn.Open();
                    string cmdText = type == DataType.ResultData?$"INSERT INTO DATA VALUES('{(string)data[0]}', '{(int)data[1]}', '{(string)data[2]}', '{(string)data[3]}','{(int)data[4]}')":
                        $"INSERT INTO DATA VALUES('{(string)data[0]}', '{(string)data[1]}', '{(string)data[2]}')";
                    new SQLiteCommand(cmdText, conn).ExecuteNonQuery();

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
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static DataView QueryByResultStatus(string path, DataType type=DataType.ResultData,int status=1)
        {
            if (File.Exists(path))
            {
                using (SQLiteConnection conn = new SQLiteConnection($"Data Source={path};Version=3;"))
                {
                    using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter($"SELECT * FROM DATA WHERE ResultStatus = {status}", conn))
                    {
                        conn.Open();
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        return dataTable.AsDataView();
                    }
                }
            }
            ECLog.WriteToLog($"Can not found:{path}", LogLevel.Error);
            return null;
        }

        /// <summary>
        /// 查询数据库文件所有数据
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static DataView QueryAll(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection($"Data Source={path};Version=3;"))
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("SELECT * FROM DATA", conn))
                {
                    DataTable dataTable = new DataTable();
                    conn.Open();
                    dataAdapter.Fill(dataTable);
                    return dataTable.AsDataView();
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog($"查询数据时发生异常: {ex.Message}",LogLevel.Error);
                return null;
            }
        }

        /// <summary>
        /// 删除数据库文件所有数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ClearAll(string path)
        {
            if (!File.Exists(path)) return false;

            string connectionString = $"Data Source={path};Version=3;";
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM DATA", conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
