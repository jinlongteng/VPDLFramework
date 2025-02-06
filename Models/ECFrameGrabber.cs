using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace VPDLFramework.Models
{
    public class ECFrameGrabber
    {
        public ECFrameGrabber(Dictionary<string,bool> cameras) 
        {
            _grabbers = new CogFrameGrabbers();
            MonitoredCamerasStatus = cameras; 
        }

        /// <summary>
        /// 刷新图像采集器状态
        /// </summary>
        public void UpdateGrabbersStatus()
        {
            try
            {
                List<string> cameras = MonitoredCamerasStatus.Keys.ToList();
                foreach (var cam in cameras)
                {
                    foreach (ICogFrameGrabber frameGrabber in _grabbers)
                    {
                        if (frameGrabber.SerialNumber != "" && frameGrabber.Name != "")
                        {
                            string serialNumName = frameGrabber.SerialNumber + ":" + frameGrabber.Name;
                            if (MonitoredCamerasStatus.ContainsKey(serialNumName))
                            {
                                try
                                {
                                    var s = frameGrabber.GetStatus(false);

                                    if (frameGrabber.GetStatus(false) == Cognex.VisionPro.CogFrameGrabberStatusConstants.Active || frameGrabber.GetStatus(false) == Cognex.VisionPro.CogFrameGrabberStatusConstants.NotSupported)
                                        MonitoredCamerasStatus[serialNumName] = true;
                                    else
                                    {
                                        MonitoredCamerasStatus[serialNumName] = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string msg = ex.Message;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 释放图像采集器资源
        /// </summary>
        public void Dispose()
        {
            foreach (ICogFrameGrabber fg in _grabbers)
            {
                fg.Disconnect(false);
            }
            _grabbers.Dispose();
        }

        /// <summary>
        /// 图像采集器
        /// </summary>
        private CogFrameGrabbers _grabbers;

        /// <summary>
        /// 相机信息<UniqueID:型号,在线状态>
        /// </summary>
        public Dictionary<string,bool> MonitoredCamerasStatus { get; set; }
    }
}
