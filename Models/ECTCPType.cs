using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VPDLFramework.Models
{
    public class ECTCPType
    {
        /// <summary>
        /// TCP类型枚举,0服务器,1客户端
        /// </summary>
        public enum TCPTypeConstants
        {
            TCPServer,
            TCPClient,
        }
    }
}
