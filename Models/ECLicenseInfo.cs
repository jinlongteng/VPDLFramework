using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECLicenseInfo
    {
        public ECLicenseInfo() { }

        /// <summary>
        /// 电脑唯一标识码
        /// </summary>
        public string UUID { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string Key { get; set; }
    }
}
