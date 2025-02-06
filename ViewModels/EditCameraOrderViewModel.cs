using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IniParser;
using IniParser.Model;
using VPDLFramework.Models;

namespace VPDLFramework.ViewModels
{
    public class EditCameraOrderViewModel : ViewModelBase
    {
        public EditCameraOrderViewModel()
        {
            CamerasInfo = new BindingList<ECCameraIniInfo>();
            BindCmd();
            ReadIniFile();
        }

        #region 命令
        /// <summary>
        /// 命令：刷新相机信息
        /// </summary>
        public RelayCommand CmdUpdateCamerasInfo { get; set; }

        /// <summary>
        /// 命令：刷新相机信息
        /// </summary>
        public RelayCommand CmdClearIniFileCamerasInfo { get; set; }

        /// <summary>
        /// 命令：相机顺序上升一级
        /// </summary>
        public RelayCommand<object> CmdItemUp { get; set; }

        /// <summary>
        /// 命令：相机顺序下降一级
        /// </summary>
        public RelayCommand<object> CmdItemDown { get; set; }

        #endregion

        #region 方法
        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            CmdUpdateCamerasInfo = new RelayCommand(UpdateCamerasInfo);
            CmdClearIniFileCamerasInfo = new RelayCommand(ClearIniFileCamerasInfo);
            CmdItemUp = new RelayCommand<object>(ItemUp);
            CmdItemDown = new RelayCommand<object>(ItemDown);
        }

        /// <summary>
        /// 刷新相机信息
        /// </summary>
        private void UpdateCamerasInfo()
        {
            EnabelPCE();
            UpdateIniFile();
            ReadIniFile();
        }

        /// <summary>
        /// 刷新相机信息
        /// </summary>
        private void ClearIniFileCamerasInfo()
        {
            // 创建ini文件段落集合
            SectionDataCollection sectionDatas = new SectionDataCollection();

            // 使能PCE
            SectionData sectionPCE = new SectionData("enable PCE");
            KeyData keyData = new KeyData("enable bit");
            keyData.Value = "true";
            sectionPCE.Keys.AddKey(keyData);
            sectionDatas.Add(sectionPCE);
            IniData iniData = new IniData();
            iniData.Sections = sectionDatas;
            StreamWriter sw = new StreamWriter(ECFileConstantsManager.VproCameraOrderIniFilePtah);
            IniParser.StreamIniDataParser streamIniDataParser = new StreamIniDataParser();
            streamIniDataParser.WriteData(sw, iniData);
            sw.Close();

            // 读取清空后的ini文件
            ReadIniFile();

            ECDialogManager.ShowMsg("Clear Finished,Please Restart Program And Click Search Button");
           
        }

        /// <summary>
        /// 相机顺序上升一级
        /// </summary>
        private void ItemUp(object obj)
        {
            if (obj == null) return;
            ECCameraIniInfo cameraInfo= obj as ECCameraIniInfo;
           int oldIndex=CamerasInfo.IndexOf(cameraInfo);
           if (oldIndex == 0) return;
           int newIndex = oldIndex - 1;
           CamerasInfo.RemoveAt(oldIndex);
           CamerasInfo.Insert(newIndex, cameraInfo);
        }

        /// <summary>
        /// 相机顺序上升一级
        /// </summary>
        private void ItemDown(object obj)
        {
            if(obj==null) return;
            ECCameraIniInfo cameraInfo = obj as ECCameraIniInfo;
            int oldIndex = CamerasInfo.IndexOf(cameraInfo);
            if (oldIndex == CamerasInfo.Count-1) return;
            int newIndex = oldIndex+1;
            CamerasInfo.RemoveAt(oldIndex);
            CamerasInfo.Insert(newIndex, cameraInfo);
        }

        /// <summary>
        /// 使能VProCameraOrder.ini文件中的enable PCE
        /// </summary>
        private void EnabelPCE()
        {
            // 读取文件
            StreamReader streamReader = new StreamReader(ECFileConstantsManager.VproCameraOrderIniFilePtah);
            IniParser.StreamIniDataParser streamIniDataParser = new StreamIniDataParser();
            IniData data = streamIniDataParser.ReadData(streamReader);
            streamReader.Close();

            if (data.Sections.ContainsSection("enable PCE"))
            {
                SectionData sectionPCE = data.Sections.GetSectionData("enable PCE");
                if (sectionPCE.Keys["enable bit"] != "true")
                {
                    sectionPCE.Keys["enable bit"] = "true";
                    data.Sections.SetSectionData("enable PCE", sectionPCE);
                    StreamWriter sw = new StreamWriter(ECFileConstantsManager.VproCameraOrderIniFilePtah);
                    streamIniDataParser.WriteData(sw, data);
                    sw.Close();
                }
            }
            else
            {
                // 创建ini文件段落集合
                SectionDataCollection sectionDatas = new SectionDataCollection();

                // 使能PCE
                SectionData sectionPCE = new SectionData("enable PCE");
                KeyData keyData = new KeyData("enable bit");
                keyData.Value = "true";
                sectionPCE.Keys.AddKey(keyData);
                sectionDatas.Add(sectionPCE);
                IniData iniData = new IniData();
                iniData.Sections = sectionDatas;
                StreamWriter sw = new StreamWriter(ECFileConstantsManager.VproCameraOrderIniFilePtah);
                streamIniDataParser.WriteData(sw, iniData);
                sw.Close();
            }
        }

        /// <summary>
        /// 获取当前可用的硬件相机信息
        /// </summary>
        private void UpdateIniFile()
        {
            // 添加没有在ini文件中的相机
            CogFrameGrabbers.Refresh();
        }

        /// <summary>
        /// 读取VPCameraOrder.ini文件
        /// </summary>
        private void ReadIniFile()
		{
            // 读取文件
            StreamReader streamReader = new StreamReader(ECFileConstantsManager.VproCameraOrderIniFilePtah);
            IniParser.StreamIniDataParser streamIniDataParser=new StreamIniDataParser();
            IniData data= streamIniDataParser.ReadData(streamReader);
            streamReader.Close();

            // 解析文件
            Sections= data.Sections;
            CamerasInfo = new BindingList<ECCameraIniInfo>();
            foreach(SectionData section in Sections)
            {
                if (section.SectionName.Contains("GigE Camera"))
                {
                    KeyDataCollection keyDatas= section.Keys;
                    if (keyDatas.ContainsKey("serialNo") && keyDatas.ContainsKey("name"))
                    {
                        if (keyDatas["serialNo"] != null && keyDatas["name"]!=null)
                        {
                            int index=Convert.ToInt16(section.SectionName.Replace("GigE Camera","").Trim());
                            CamerasInfo.Add(new ECCameraIniInfo() { 
                                Index = index,
                                IP_Addr = keyDatas["IP_Addr"],
                                subnet_mask = keyDatas["subnet_mask"],
                                IPCurrentConfig = keyDatas["IPCurrentConfig"],
                                MacAddr = keyDatas["MacAddr"],
                                Host_IPAddr = keyDatas["Host_IPAddr"],
                                Host_subnet_mask = keyDatas["Host_subnet_mask"],
                                Host_macAddr = keyDatas["Host_macAddr"],
                                Host_mtu = keyDatas["Host_mtu"],
                                name = keyDatas["name"],
                                serialNo = keyDatas["serialNo"],
                                bigEndian = keyDatas["bigEndian"]
                            }) ;
                        }
                    }
                }
            }
        }

		/// <summary>
		/// 写入VPCameraOrder.ini
		/// </summary>
		public void WriteIniFile()
		{
            // 创建ini文件段落集合
            SectionDataCollection sectionDatas = new SectionDataCollection();
            
            // 使能PCE
            SectionData sectionPCE = new SectionData("enable PCE");
            KeyData keyData = new KeyData("enable bit");
            keyData.Value = "true";
            sectionPCE.Keys.AddKey(keyData);
            sectionDatas.Add(sectionPCE);

            // 相机段落
            for(int i=0;i< CamerasInfo.Count;i++) 
            {
                SectionData sectionData = Sections.GetSectionData($"GigE Camera {CamerasInfo[i].Index}");
                sectionData.SectionName = $"GigE Camera {i}";
                sectionDatas.Add(sectionData);
            }
            IniData iniData = new IniData();
            iniData.Sections = sectionDatas;
            StreamWriter sw = new StreamWriter(ECFileConstantsManager.VproCameraOrderIniFilePtah);
            IniParser.StreamIniDataParser streamIniDataParser = new StreamIniDataParser();
            streamIniDataParser.WriteData(sw, iniData);
            sw.Close();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 相机信息列表
        /// </summary>
        private BindingList<ECCameraIniInfo> _camerasInfo;

		public BindingList<ECCameraIniInfo> CamerasInfo
        {
			get { return _camerasInfo; }
			set {
                _camerasInfo = value;
				RaisePropertyChanged();
			}
		}

        /// <summary>
        ///  ini文件段落信息
        /// </summary>
        private SectionDataCollection _sections;

        public SectionDataCollection Sections
        {
            get { return _sections; }
            set { _sections = value;
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}
