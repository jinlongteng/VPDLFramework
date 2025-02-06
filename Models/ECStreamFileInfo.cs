using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECStreamFileInfo
    {
        public ECStreamFileInfo(string streamName, string spaceOccupiedByImages, string databaseSize)
        {
            StreamName = streamName;
            SpaceOccupiedByImages = spaceOccupiedByImages;
            DatabaseSize = databaseSize;
        }

        /// <summary>
        /// 工作流名称
        /// </summary>
        public string StreamName { get; set; }

        /// <summary>
        /// 存储的图像空间占用
        /// </summary>
        public string SpaceOccupiedByImages { get; set; }

        /// <summary>
        /// 数据库占用空间大小
        /// </summary>
        public string DatabaseSize { get; set; }
    }
}
