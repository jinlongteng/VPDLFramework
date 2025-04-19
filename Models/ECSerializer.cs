using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VPDLFramework.Models
{
    public class ECSerializer
    {
        /// <summary>
        /// 从Json文件加载对象
        /// </summary>
        public static T LoadObjectFromJson<T>(string jsonFilePath) where T : class
        {
            if (!File.Exists(jsonFilePath))
            {
                return default;
            }
            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonFilePath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    var serializer = JsonSerializer.CreateDefault();
                    var ret = serializer.Deserialize<T>(reader);
                    return ret;
                }
            }
        }

        /// <summary>
        /// 保存对象到Json文件
        /// </summary>
        public static bool SaveObjectToJson(string jsonFilePath, object savedObject)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(savedObject);
                File.WriteAllText(jsonFilePath, jsonData);
                return true;
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }

        }
    }
}
