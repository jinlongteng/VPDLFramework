using Analogy.CommonUtilities.Github;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NLog;

namespace VPDLFramework.Models
{
    public class ECWMI:ObservableObject
    {
        public ECWMI() 
        {
           // Initial();
            //_timer=new System.Timers.Timer(1000);
            //_timer.Elapsed += _timer_Elapsed;
           // _timer.Start();
        }

        #region 方法
        /// <summary>
        /// 定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GetCPUUsage();
            GetRAMUsage();
            GetDiskAvaliableSize();
        }

        /// <summary>
        /// 关闭系统资源监视
        /// </summary>
        public void Dispose()
        {
            //_timer?.Stop();
            //_timer?.Dispose();
            //_cpuCounter.Dispose();
            //_ramCounter.Dispose();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initial()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            _disk0 = DriveInfo.GetDrives()[0];
        }

        /// <summary>
        /// 获取CPU占用率
        /// </summary>
        private void GetCPUUsage()
        {
           float usage= _cpuCounter.NextValue();
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                CPUUsage = (int)usage;
            });
            WarningOccupancy(CPUUsage, WMIType.CPU);
        }

        /// <summary>
        /// 获取内存占用率
        /// </summary>
        private void GetRAMUsage()
        {
            float usage=_ramCounter.NextValue();
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                RAMUsage = (int)usage;
            });
            WarningOccupancy(RAMUsage,WMIType.RAM);
        }

        /// <summary>
        /// 获取磁盘可用大小
        /// </summary>
        private void GetDiskAvaliableSize()
        {
            DiskName = ECFileConstantsManager.RootDisk;
            DriveInfo[] infos = DriveInfo.GetDrives();
            foreach (var d in infos)
            {
                if (d.Name.Equals(DiskName, StringComparison.OrdinalIgnoreCase))
                {
                    _disk0 = d;
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        DiskUsedSize = (int)((_disk0.TotalSize - _disk0.AvailableFreeSpace) / 1024 / 1024 / 1024);
                        DiskTotalSize = (int)(_disk0.TotalSize / 1024 / 1024 / 1024);
                        DiskUsage = (int)(((double)DiskUsedSize / (double)DiskTotalSize) * 100);
                    });
                    break;
                }
            }
            WarningOccupancy(DiskUsage, WMIType.Disk);
        }

        /// <summary>
        /// 占用率过高报警
        /// </summary>
        private void WarningOccupancy(int rate,WMIType type)
        {
            if(rate>=99)
            {
                ECLog.WriteToLog($"{Enum.GetName(typeof(WMIType),type)} {ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.OccupancyTooHigh)}", NLog.LogLevel.Warn);
            }
        }

        #endregion

        #region 字段
        /// <summary>
        /// 定时器
        /// </summary>
        private System.Timers.Timer _timer;

        // 获取系统CPU利用率
        private PerformanceCounter _cpuCounter;

        // 获取系统内存使用情况
        private PerformanceCounter _ramCounter;

        // 磁盘0
        private DriveInfo _disk0;
        #endregion

        #region 属性
        /// <summary>
        /// CPU占用率
        /// </summary>
        private int _CPUUsage;

		public int CPUUsage
        {
			get { return _CPUUsage; }
			set { _CPUUsage = value;
				RaisePropertyChanged();
			}
		}

        /// <summary>
        /// 内存占用率
        /// </summary>
        private int _RAMUsage;

        public int RAMUsage
        {
            get { return _RAMUsage; }
            set
            {
                _RAMUsage = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 磁盘使用率
        /// </summary>
        private int _diskUsage;

        public int DiskUsage
        {
            get { return _diskUsage; }
            set
            {
                _diskUsage = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 磁盘已使用内存大小
        /// </summary>
        private int _diskUsedSize;

		public int DiskUsedSize
        {
			get { return _diskUsedSize; }
			set {
                _diskUsedSize = value;
				RaisePropertyChanged();
			}
		}

        /// <summary>
        /// 磁盘总内存dax
        /// </summary>
        private int _diskTotalSize;

        public int DiskTotalSize
        {
            get { return _diskTotalSize; }
            set
            {
                _diskTotalSize = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 磁盘名称
        /// </summary>
        private string _diskName;

        public string DiskName
        {
            get { return _diskName; }
            set { _diskName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// WMI类型
        /// </summary>
        public enum WMIType
        {
            CPU,
            RAM,
            Disk
        }

        #endregion


    }
}
