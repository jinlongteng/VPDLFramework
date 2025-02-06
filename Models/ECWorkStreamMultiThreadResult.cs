using Cognex.VisionPro.Interop;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECWorkStreamMultiThreadResult
    {
        public int TriggerIndex { get; set; }

        public DateTime TriggerTime { get; set; }

        public int TreadID { get; set; }

        public CogToolBlock ToolBlock { get; set; }

        public ECWorkImageSourceOutput ImageSourceOutput { get; set; }
    }
}
