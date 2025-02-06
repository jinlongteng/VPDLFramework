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
        public static T LoadObjectFromJson<T>(string jsonFilePath)
        {
            try
            {

                if (File.Exists(jsonFilePath))
                {
                    using (System.IO.StreamReader file = System.IO.File.OpenText(jsonFilePath))
                    {
                        using (JsonTextReader reader = new JsonTextReader(file))
                        {
                            var o = JToken.ReadFrom(reader);
                            if (o.Type == JTokenType.Object)
                            {
                                reader.Close();
                                //反序列化Json文件
                                return ((JObject)o).ToObject<T>();
                            }
                            else if (o.Type == JTokenType.Array)
                            {
                                reader.Close ();
                                return ((JArray)o).ToObject<T>();
                            }
                            else
                                reader.Close ();
                        }
                    }
                }
                return default;
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return default;
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
