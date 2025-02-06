using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECDatabaseHelper
    {
        /// <summary>
        /// 写入CSV
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="dt">要写入的datatable</param>
        public static void WriteCSV(string fileName, DataTable dt)
        {
            FileStream fs;
            StreamWriter sw;
            string data = null;

            //判断文件是否存在,存在就不再次写入列名
            if (!File.Exists(fileName))
            {
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.UTF8);

                //写出列名称
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    data += dt.Columns[i].ColumnName.ToString();
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";//中间用，隔开
                    }
                }
                sw.WriteLine(data);
            }
            else
            {
                fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.UTF8);
            }

            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = null;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";//中间用，隔开
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }



        /// <summary>
        /// 读取CSV文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        public static DataTable ReadCSV(string fileName)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);

            //记录每次读取的一行记录
            string strLine = null;
            //记录每行记录中的各字段内容
            string[] arrayLine = null;
            //分隔符
            string[] separators = { "," };
            //判断，若是第一次，建立表头
            bool isFirst = true;

            //列的个数
            int dtColumns = 0;

            //逐行读取CSV文件
            while ((strLine = sr.ReadLine()) != null)
            {
                strLine = strLine.Trim();//去除头尾空格
                arrayLine = strLine.Split(separators, StringSplitOptions.RemoveEmptyEntries);//分隔字符串，返回数组

                if (isFirst)  //建立表头
                {
                    dtColumns = arrayLine.Length;//列的个数
                    for (int i = 0; i < dtColumns; i++)
                    {
                        dt.Columns.Add(arrayLine[i]);//每一列名称
                    }
                    isFirst = false;
                }
                else   //表内容
                {
                    DataRow dataRow = dt.NewRow();//新建一行
                    for (int j = 0; j < dtColumns; j++)
                    {
                        dataRow[j] = arrayLine[j];
                    }
                    dt.Rows.Add(dataRow);//添加一行
                }
            }
            sr.Close();
            fs.Close();

            return dt;
        }
    }
}

