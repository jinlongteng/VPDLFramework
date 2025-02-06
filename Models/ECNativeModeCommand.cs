using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECNativeModeCommand
    {
        /// <summary>
        /// 检查字符串是否是本地化指令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static NativeModeCommandTypeConstants CheckCommandString(string command)
        {
            string[] strings = command.Split(',');
            int validLength=strings.Length;
            foreach (string s in strings)
            {
                if (s.Trim().Length == 0)
                    validLength--;
            }
            if(validLength>0)
            {
                NativeModeCommandTypeConstants constant;
                if (Enum.TryParse(strings[0], out constant))
                    return constant;
            }

            return NativeModeCommandTypeConstants.ERR;
        }

        /// <summary>
        /// 命令类型常数
        /// </summary>
        public enum NativeModeCommandTypeConstants
        {
            // Error
            ERR, 

            // Work
            LW, // Load Work
            UW, // UnLoad Work

            // WorkStream
            TS, // Trigger Stream
            TMS, // Trigger Multi Streams
            LR, // Load Recipe
            SUD, // Set User Data
            SET, // Set Exposure Time
            SIS, // Set Image Source
            TSB, // Trigger Stream Beigin, For Internal Trigger
            TSE, // Trigger Stream End, For Internal Trigger
            TWE, // Trigger Stream With Exposure
            TWD, // Trigger Stream With User Data
            TWED, // Trigger Stream With Exposure and User Data
            TWI, // Trigger Stream With Image Source
            TI, // Trigger Image Source
            TIWE, // Trigger Image Source With Exposure
            

            // Set
            SO, // Set OnlineMode
            
            // Get
            GO, // Get OnlineMode
            GW, // Get Loaded Work's WorkName
            GS, // Get Loaded Work's Streams
            GAW, // Get All Work
        }
    }
}
