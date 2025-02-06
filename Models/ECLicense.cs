using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECLicense
    {
        public ECLicense() { }

        /// <summary>
        /// 检测License
        /// </summary>
        /// <returns></returns>
        public bool CheckLicense()
        {
            try
            {
                string licensePath = ECFileConstantsManager.LicenseFolder + @"\" + ECFileConstantsManager.LicenseFileName;
                if (File.Exists(licensePath))
                {
                    ECLicenseInfo licenseInfo = ECSerializer.LoadObjectFromJson<ECLicenseInfo>(licensePath);
                    if (licenseInfo != null && licenseInfo.Key != null)
                    {
                        string decryptID = Decrypt(licenseInfo.Key);
                        if (licenseInfo.UUID == GetUUID())
                        {
                            if (decryptID == licenseInfo.UUID)
                                return true;
                        }
                        else
                        {
                            CreateNewLicenseFile();
                        }
                    }
                }
                else
                {
                    CreateNewLicenseFile();
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Trace);
            }
            return false;
        }

        /// <summary>
        /// 创建新的授权文件
        /// </summary>
        private void CreateNewLicenseFile()
        {
            string licensePath = ECFileConstantsManager.LicenseFolder + @"\" + ECFileConstantsManager.LicenseFileName;
            ECLicenseInfo licenseInfo = new ECLicenseInfo();
            licenseInfo.UUID = GetUUID();
            if (!Directory.Exists(Path.GetDirectoryName(licensePath))) Directory.CreateDirectory(Path.GetDirectoryName(licensePath));
            ECSerializer.SaveObjectToJson(licensePath, licenseInfo);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        private string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(_key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(_iv);

            using (AesManaged aes = new AesManaged())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        /// <summary>
        /// 获取UUID
        /// </summary>
        /// <returns></returns>
        private string GetUUID()
        {
            string code = null;
            try
            {
                SelectQuery query = new SelectQuery("select * from Win32_ComputerSystemProduct");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (var item in searcher.Get())
                    {
                        using (item)
                            code = item["UUID"].ToString();
                    }
                }
            }
            catch(Exception ex) 
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Trace);
            }
            return code;
        }

        #region 字段
        /// <summary>
        /// 对称算法密匙
        /// </summary>
        private string _key = "ec cognex visionproframework lic";

        /// <summary>
        /// 对称算法向量
        /// </summary>
        private string _iv = "license creator ";

        #endregion
    }
}
