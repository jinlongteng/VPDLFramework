using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECWorkOptionManager
    {
        /// <summary>
        /// 结果图形选项
        /// </summary>

        public enum ResultGraphiConstants
        {
            Default,
            Custom,
        }

        /// <summary>
        /// 图像记录选项
        /// </summary>
        public enum ImageRecordConstants
        {
            Original,
            Graphic,
            OriginalAndGraphic,
            OriginalAndSVG
        }

        /// <summary>
        /// 图像记录条件选项
        /// </summary>
        public enum ImageRecordConditionConstants
        {
            All,
            True,
            False
        }

        /// <summary>
        /// 触发类型选项
        /// </summary>
        public enum TriggerTypeConstants
        {
            IO,
            TCP,
            FFP,
            ImageSource,
        }

        /// <summary>
        /// 结果发送类型选项
        /// </summary>
        public enum ResultSendTypeConstants
        {
            IO,
            TCP,
            FFP,
            Script,
        }

        /// <summary>
        /// 原图类型
        /// </summary>
        public enum QriginalImageTypeContstans
        {
            PNG,
            BMP
        }

        /// <summary>
        /// IO输入选项
        /// </summary>
        public enum IOInputConstants
        {
            Line0,
            Line1,
            Line2,
            Line3,
            Line4,
            Line5,
            Line6,
            Line7,
        }

        /// <summary>
        /// IO输出选项
        /// </summary>
        public enum IOOutputConstants
        {
            Output0,
            Output1,
            Output2,
            Output3, 
            Output4,
            Output5, 
            Output6,
            Output7,
            Output8,
            Output9,
            Output10,
            Output11,
            Output12,
            Output13,
            Output14,
            Output15,
        }

        /// <summary>
        /// IO信号类型
        /// </summary>
        public enum IOInputSignalTypeConstants
        {
            Any,
            Rise,
            Fall
        }

        /// <summary>
        /// IO信号类型
        /// </summary>
        public enum IOOutputSignalTypeConstants
        {
            High,
            Low,
        }

        /// <summary>
        /// SoftEvent选项
        /// </summary>
        public enum SoftEventConstants
        {
            SoftEvent0,
            SoftEvent1,
            SoftEvent2,
            SoftEvent3,
            SoftEvent4,
            SoftEvent5,
            SoftEvent6,
            SoftEvent7,
            SoftEvent8,
            SoftEvent9,
            SoftEvent10,
            SoftEvent11,
            SoftEvent12,
            SoftEvent13,
            SoftEvent14,
            SoftEvent15,
            SoftEvent16,
            SoftEvent17,
            SoftEvent18,
            SoftEvent19,
            SoftEvent20,
            SoftEvent21,
            SoftEvent22,
            SoftEvent23,
            SoftEvent24,
            SoftEvent25,
            SoftEvent26,
            SoftEvent27,
            SoftEvent28,
            SoftEvent29,
            SoftEvent30,
            SoftEvent31,
        }
    }
}
