using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using GalaSoft.MvvmLight;
using NLog;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECWorkImageSource : ObservableObject
    {
		public ECWorkImageSource(string workName,string imageSourceName) 
		{ 
            _workName = workName;
            _imageSourceName = imageSourceName;
            IsRunning = false;
		}

        #region 方法

        /// <summary>
        /// 加载工作流配置文件，检查配置是否正常,正常返回True，否则返回False
        /// </summary>
        /// <returns>初始化成功返回True，否则返回False</returns>
        public bool LoadConfig()
        {
            try
            {
                // 加载配置信息
                string folder = $"{ECFileConstantsManager.RootFolder}\\{_workName}\\{ECFileConstantsManager.ImageSourceFolderName}\\{_imageSourceName}";

                string jsonPath = folder + @"\" + ECFileConstantsManager.ImageSourceConfigName;
                string tbPath=folder + @"\" + ECFileConstantsManager.ImageSourceAcqTBName;

                ImageSourceInfo = ECSerializer.LoadObjectFromJson<ECImageSourceInfo>(jsonPath);
                if (ImageSourceInfo == null)
                    return false;

                // 加载ToolBlock
                if (File.Exists(tbPath))
                {
                    ToolBlock = CogSerializer.LoadObjectFromFile(tbPath) as CogToolBlock;
                    InitialHardwareTrigger();
                }
                else
                    return false;

                GetCameraSerialNumName();

                // 创建文件图像对象
                if(ImageSourceInfo.ImageFilePath!=null)
                {
                    if(Directory.Exists(ImageSourceInfo.ImageFilePath))
                        _fileImageSource=new ECFileImageSource(ImageSourceInfo.ImageFilePath);
                }

                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// 创建新的配置
        /// </summary>
        /// <returns>初始化成功返回True，否则返回False</returns>
        public bool CreateNewConfig()
        {
            try
            {
                // 加载配置信息
                ImageSourceInfo = new ECImageSourceInfo(_imageSourceName);

                // 加载ToolBlock
                if (File.Exists(ECFileConstantsManager.StdTB_AcqPath))
                    ToolBlock = CogSerializer.LoadObjectFromFile(ECFileConstantsManager.StdTB_AcqPath) as CogToolBlock;
                else
                    return false;

                return true;
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
                return false;
            }

        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <returns></returns>
        public ECWorkImageSourceOutput GetImage()
        {
            if (IsRunning) return null;

            IsRunning = true;
            _imageSourceOutput=new ECWorkImageSourceOutput();

            try
            {
                OpenLight();
                if (ImageSourceInfo.IsUseCam)
                {             
                    if (ToolBlock != null)
                    {
                        ToolBlock.Run();
                        _imageSourceOutput = CreateImageSourceOutput();
                    }
                }
                else
                {
                    if ((_fileImageSource == null && ImageSourceInfo.ImageFilePath != null) || (_fileImageSource != null && ImageSourceInfo.ImageFilePath != _fileImageSource.ImagePath))
                    {
                        _fileImageSource = new ECFileImageSource(ImageSourceInfo.ImageFilePath);
                    }
                    if (_fileImageSource != null)
                        _imageSourceOutput.Image = _fileImageSource.RunOnce();
                }
                CloseLight();
                IsRunning = false;
                Completed?.Invoke(this, new KeyValuePair<string, ECWorkImageSourceOutput>(ImageSourceInfo.ImageSourceName,_imageSourceOutput));
            }
            catch (Exception ex)
            {
                IsRunning = false;
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);

                Completed?.Invoke(this, new KeyValuePair<string, ECWorkImageSourceOutput>(ImageSourceInfo.ImageSourceName,null));
            }

            return _imageSourceOutput;
        }

        /// <summary>
        /// 获取相机序列号：名称组成的字符串
        /// </summary>
        /// <returns></returns>
        private void GetCameraSerialNumName()
        {
            try
            {
                if (ToolBlock.Tools.Contains("CogAcqFifoTool1"))
                {
                    CogAcqFifoTool tool = ToolBlock.Tools["CogAcqFifoTool1"] as CogAcqFifoTool;
                    if (tool.Operator != null)
                    {
                        CameraSerialNumName = tool.Operator.FrameGrabber?.SerialNumber + ":" + tool.Operator.FrameGrabber?.Name;
                    }
                }

            }
            catch (Exception ex)
            {
                CameraSerialNumName = ":";
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
        /// <returns></returns>
        public bool SetExposure(double exposureTime)
        {
            bool result=false;
            try
            {
                if(_fifoTrigger!=null&&_fifoTrigger.TriggerModel==CogAcqTriggerModelConstants.Auto)
                {
                    _cogFifoTool.Operator.OwnedExposureParams.Exposure = exposureTime;
                    result = true;
                }
                else if (ToolBlock != null && ToolBlock.Inputs.Contains("ExposureTime"))
                {
                    if (ToolBlock.Inputs["ExposureTime"].ValueType == typeof(double))
                    {
                        ToolBlock.Inputs["ExposureTime"].Value = exposureTime;
                        result = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ECGeneric.GetExceptionMethodName(ex)+ex.Message, NLog.LogLevel.Warn);
            }
            return result;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if(_fifoTrigger != null)
                _fifoTrigger.TriggerEnabled=false;
            _udpClient?.Dispose();
            _tcpClient?.Dispose();
            ToolBlock?.Dispose();
            ToolBlock=null;
        }
        
        /// <summary>
        /// 连接光源控制器
        /// </summary>
        private void ConnectLightController()
        {
            try
            {
                // UDP协议
                if (ImageSourceInfo.LightEthernetType == 0 && _udpClient == null)
                    _udpClient = new UdpClient();
                // TCP协议
                else if ((ImageSourceInfo.LightEthernetType == 1) &&_tcpClient == null)
                {
                    if (_tcpClient != null) _tcpClient.Dispose();
                    if (ImageSourceInfo.ControllerIPPort != null)
                    {
                        string[] ipPort = ImageSourceInfo.ControllerIPPort.Split(':');
                        if (ipPort.Length == 2)
                        {
                            IPAddress iPAddress = IPAddress.Parse(ipPort[0]);
                            int port = Convert.ToInt16(ipPort[1]);
                            _tcpClient = new SimpleTcpClient();
                            _tcpClient.Connect(iPAddress.ToString(), port);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 连接光源控制器
        /// </summary>
        private void DisconnectLightController()
        {
            try
            {
                // UDP协议
                if (ImageSourceInfo.LightEthernetType == 0 && _udpClient != null)
                {
                    _udpClient.Dispose();
                    _udpClient=null;
                }

                // TCP协议
                else if ((ImageSourceInfo.LightEthernetType == 1) && _tcpClient != null)
                {
                    _tcpClient.Disconnect();
                    _tcpClient.Dispose();
                    _tcpClient=null;
                }
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 打开光源
        /// </summary>
        public void OpenLight()
        {
            // 判断是否启用光源控制
            if (!ImageSourceInfo.IsLightControlEnbale) return;
            try
            {
                // 连接光源控制器
                ConnectLightController();

                // 判断指令不为null
                if (ImageSourceInfo.LightOnCommand != null)
                {
                    byte[] data;
                    if(ImageSourceInfo.IsHex)
                        data=HexStringToByteArray(ImageSourceInfo.LightOnCommand);
                    else
                        data = Encoding.ASCII.GetBytes(ImageSourceInfo.LightOnCommand);

                    if (data == null) return;

                    if (ImageSourceInfo.LightEthernetType == 0)
                    {
                        // UDP发送
                        string[] ipPort = ImageSourceInfo.ControllerIPPort.Split(':');
                        _udpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Parse(ipPort[0]), Convert.ToInt16(ipPort[1])));
                    }
                    else
                    {
                        // TCP发送
                        if (_tcpClient != null && _tcpClient.TcpClient.Connected)
                            _tcpClient.Write(data);
                    }     
                }
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 关闭光源
        /// </summary>
        public void CloseLight()
        {
            // 判断是否启用光源控制
            if (!ImageSourceInfo.IsLightControlEnbale) return;
            try
            {
                // 连接光源控制器
                ConnectLightController();

                // 判断指令不为null
                if (ImageSourceInfo.LightOffCommand != null)
                {
                    byte[] data;
                    if(ImageSourceInfo.IsHex)
                        data=HexStringToByteArray(ImageSourceInfo.LightOffCommand); 
                    else
                        data = Encoding.ASCII.GetBytes(ImageSourceInfo.LightOffCommand);

                    if (data == null) return;

                    if (ImageSourceInfo.LightEthernetType == 0)
                    {
                        // UDP发送
                        string[] ipPort = ImageSourceInfo.ControllerIPPort.Split(':');
                        _udpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Parse(ipPort[0]), Convert.ToInt16(ipPort[1])));
                    }
                    else
                    {
                        // TCP发送
                        if (_tcpClient != null && _tcpClient.TcpClient.Connected)
                            _tcpClient.Write(data);
                    }
                }

                DisconnectLightController();
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 将十六进制字符串转换为字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public byte[] HexStringToByteArray(string hexString)
        {
            if (!IsHexStringValid(hexString))
                return null;

            return Enumerable.Range(0, hexString.Length / 2).Select(x => Convert.ToByte(hexString.Substring(x * 2, 2), 16)).ToArray();
        }

        /// <summary>
        /// 验证十六进制字符串格式是否正确
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private bool IsHexStringValid(string hexString)
        {
            Regex regex = new Regex(@"^[A-Fa-f\d]+$");
            return regex.Match(hexString).Success && hexString.Length % 2 == 0;
        }

        /// <summary>
        /// 初始化硬件触发
        /// </summary>
        private void InitialHardwareTrigger()
        {
            foreach(ICogTool cogTool in ToolBlock.Tools)
            {
                if(cogTool.GetType()==typeof(CogAcqFifoTool))
                {
                    _cogFifoTool = cogTool as CogAcqFifoTool;

                    if(_cogFifoTool.Operator!=null&&_cogFifoTool.Operator.FrameGrabber!=null&&_cogFifoTool.Operator.OwnedTriggerParams!=null)
                    {
                        if ((_cogFifoTool.Operator.OwnedTriggerParams.TriggerModel == CogAcqTriggerModelConstants.Auto))
                        {
                            _fifoTrigger = _cogFifoTool.Operator.OwnedTriggerParams;
                            _cogFifoTool.Operator.Complete += Operator_Complete;
                            _cogFifoTool.Operator.Flush();
                            _fifoTrigger.TriggerEnabled = true;
                        }
                    }
                    break;    
                }
            }
        }

        /// <summary>
        /// 硬触发采集完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Operator_Complete(object sender, CogCompleteEventArgs e)
        {
            int numPending, numReady;
            bool busy;
            _cogFifoTool.Operator.GetFifoState(out numPending, out numReady, out busy);

            if(numReady>0)
            {
                ICogImage image = _cogFifoTool.Operator.CompleteAcquireEx(new CogAcqInfo()).CopyBase(CogImageCopyModeConstants.CopyPixels);
                _cogFifoTool.Operator.Flush();
                GC.Collect();
                Completed?.Invoke(this,new KeyValuePair<string, ECWorkImageSourceOutput>(ImageSourceInfo.ImageSourceName, CreateImageSourceOutput()));
            }
        }

        /// <summary>
        /// 生成图像源输出
        /// </summary>
        /// <returns></returns>
        private ECWorkImageSourceOutput CreateImageSourceOutput()
        {
            try
            {
                ECWorkImageSourceOutput  output = new ECWorkImageSourceOutput();
                if (ToolBlock.Outputs.Contains("IsAcqSucceed") && ToolBlock.Outputs.Contains("OutputImage"))
                {
                    output.IsAcqSucceed = (bool)ToolBlock.Outputs["IsAcqSucceed"].Value;
                    if (output.IsAcqSucceed)
                        output.Image = (ToolBlock.Outputs["OutputImage"].Value as ICogImage).CopyBase(CogImageCopyModeConstants.CopyPixels);
                }
                output.OtherOutputs = new CogToolBlockTerminalCollection();
                foreach (CogToolBlockTerminal terminal in ToolBlock.Outputs)
                {
                    if (terminal.Name != "IsAcqSucceed" && terminal.Name != "OutputImage")
                    {
                        if (terminal.ValueType.IsValueType)
                            output.OtherOutputs.Add(new CogToolBlockTerminal(terminal.Name, terminal.Value));
                        else
                            output.OtherOutputs.Add(new CogToolBlockTerminal(terminal.Name, ECGeneric.DeepCopy(terminal.Value)));
                    }
                }
                return output;
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message,LogLevel.Error);
                return null;
            }
        }
        #endregion

        #region 字段

        /// <summary>
        /// 工作名称
        /// </summary>
        private string _workName;

        /// <summary>
        /// 图像源名称
        /// </summary>
        private string _imageSourceName;

        /// <summary>
        /// 文件图像源
        /// </summary>
        private ECFileImageSource _fileImageSource;

        /// <summary>
        /// UDP客户端,用于控制光源
        /// </summary>
        private UdpClient _udpClient;

        /// <summary>
        /// TCP客户端,用于控制光源
        /// </summary>
        private SimpleTcpClient _tcpClient;

        /// <summary>
        /// 图像源采集完成
        /// </summary>
        public event EventHandler<KeyValuePair<string,ECWorkImageSourceOutput>> Completed;
    
        /// <summary>
        /// 图像采集器工具
        /// </summary>
        private CogAcqFifoTool _cogFifoTool;

        /// <summary>
        /// 图像源采集器触发参数
        /// </summary>
        private ICogAcqTrigger _fifoTrigger;

        /// <summary>
        /// 图像源输出
        /// </summary>
        private ECWorkImageSourceOutput _imageSourceOutput;
        #endregion

        /// <summary>
        /// 取图ToolBlock
        /// </summary>
        public CogToolBlock ToolBlock { get; set; }

        /// <summary>
        /// 正在运行
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// 图像源信息
        /// </summary>
        private ECImageSourceInfo _imageSourceInfo;

		public ECImageSourceInfo ImageSourceInfo
        {
			get { return _imageSourceInfo; }
			set { _imageSourceInfo = value;
				RaisePropertyChanged();
			}
		}

        /// <summary>
        /// 相机唯一ID
        /// </summary>
        private string _cameraSerialNumName;

        public string CameraSerialNumName
        {
            get { return _cameraSerialNumName; }
            set {
                _cameraSerialNumName = value;
                RaisePropertyChanged();
            }
        }

    }
}
