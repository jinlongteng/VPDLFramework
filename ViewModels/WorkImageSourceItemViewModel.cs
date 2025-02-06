using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VPDLFramework.Models;
using System.IO;
using NLog;
using Cognex.VisionPro;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.ComponentModel;
using System.Runtime.Remoting.Contexts;
using System.Windows.Forms;
using Cognex.VisionPro.ImageFile;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Messaging;
using SharpDX.Direct3D9;
using System.Web.UI.WebControls;

namespace VPDLFramework.ViewModels
{
    public class WorkImageSourceItemViewModel:ViewModelBase
    {
        public WorkImageSourceItemViewModel(string workName, string imageSourceName)
        {
            _workName = workName;
            WorkImageSource = new ECWorkImageSource(workName, imageSourceName);
            CurrentImageIndex = 0;
            ImageCount = 0;
            BindCmd();
            GetLocalIPs();
        }

        #region 字段

        /// <summary>
        /// 工作名称
        /// </summary>
        private string _workName;

        #endregion 字段

        #region 命令

        /// <summary>
        /// 命令：打开图像文件夹
        /// </summary>
        public RelayCommand CmdOpenImageFile { get; set; }

        /// <summary>
        /// 命令：编辑采集Toolblock
        /// </summary>
        public RelayCommand CmdEditTBAcq { get; set; }

        /// <summary>
        /// 命令：运行一次
        /// </summary>
        public RelayCommand CmdRunOnce { get; set; }

        /// <summary>
        /// 命令：运行选择的图片
        /// </summary>
        public RelayCommand<int> CmdRunSelectedImage { get; set; }

        /// <summary>
        /// 命令：重置图像源图像顺序,从第一张图像开始
        /// </summary>
        public RelayCommand CmdResetImageSeqConfig { get; set; }

        /// <summary>
        /// 命令：打开图像文件夹
        /// </summary>
        public RelayCommand CmdLoadImages { get; set; }

        /// <summary>
        /// 命令：播放
        /// </summary>
        public RelayCommand CmdPlay { get; set; }

        /// <summary>
        /// 命令：下一张
        /// </summary>
        public RelayCommand CmdNext { get; set; }

        /// <summary>
        /// 命令：上一张
        /// </summary>
        public RelayCommand CmdPrevious { get; set; }

        /// <summary>
        /// 命令：第一张
        /// </summary>
        public RelayCommand CmdFirst { get; set; }

        /// <summary>
        /// 命令：最后一张
        /// </summary>
        public RelayCommand CmdLast { get; set; }

        #endregion 命令

        #region 方法
        /// <summary>
        /// 获取本机可用的IP
        /// </summary>
        private void GetLocalIPs()
        {
            List<string> ips = ECGeneric.GetLocalIPs();
            LocalIPs = new BindingList<string>();
            foreach (var ip in ips)
            {
                LocalIPs.Add(ip);
            }
        }

        /// <summary>
        /// 打开图像源文件夹
        /// </summary>
        private void OpenImageSourceFile()
        {
            MessageBoxResult result =System.Windows.MessageBox.Show(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.FileChooseYesFolderChooseNo),ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseSelect), MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result==MessageBoxResult.Yes)
            {
                var dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Multiselect = false;
                dialog.Filter = "Image File(*.idb,*.cdb,*.bmp,*.png,*.jpg,*.tiff)|*.idb;*.cdb;*.bmp;*.png;*.jpg;*.tiff";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    WorkImageSource.ImageSourceInfo.ImageFilePath = dialog.FileName;
                }
            }
            else if(result==MessageBoxResult.No)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    WorkImageSource.ImageSourceInfo.ImageFilePath = dialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 重置图像源图像顺序
        /// </summary>
        private void ResetImageSeqConfig()
        {
            ECFileImageSource fileImageSource = new ECFileImageSource();
            fileImageSource.ResetImageIndexOfConfig(WorkImageSource.ImageSourceInfo.ImageFilePath);
        }

        /// <summary>
        /// 编辑采集Toolblock
        /// </summary>
        private void EditAcqTB()
        {
            WorkImageSource.ToolBlock= ECDialogManager.EditToolBlock(WorkImageSource.ToolBlock);
        }

        /// <summary>
        /// 运行一次
        /// </summary>
        private void RunOnce()
        {
           OutputImage=ECGeneric.BitmapToBitmapImage(WorkImageSource.GetImage().Image?.ToBitmap());
        }

        /// <summary>
        /// 运行选择的图片
        /// </summary>
        private void RunSelectedImage(int index)
        {
            if (ImageList == null || ImageList.Count == 0) return;
            try
            {
                ImageList[CurrentImageIndex].IsProcessedImage = false;
                SelectedImageIndex = index;
                ICogImage image = ImageList[index].Image;
                Messenger.Default.Send<ICogImage>(image, ECMessengerManager.ExpandedWorkStreamFilmstripMessengerKeys.RunSelectedImage);
                CurrentImageIndex = SelectedImageIndex;
                ImageList[CurrentImageIndex].IsProcessedImage = true;
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,LogLevel.Error);
            }
        }

        /// <summary>
        /// 加载图像
        /// </summary>
        private void LoadImages()
        {
            if (WorkImageSource.ImageSourceInfo.ImageFilePath == null) return;
            try
            {
                BindingList<FilmstripImage> tempImageList = new BindingList<FilmstripImage>();

                ECDialogManager.LoadWithAnimation(() =>
                {
                    if (WorkImageSource.ImageSourceInfo.ImageFilePath.EndsWith(".idb") || WorkImageSource.ImageSourceInfo.ImageFilePath.EndsWith(".cdb"))
                    {
                        try
                        {
                            CogImageFileTool tool = new CogImageFileTool();
                            tool.Operator.Open(WorkImageSource.ImageSourceInfo.ImageFilePath, CogImageFileModeConstants.Read);
                            for (int i = 0; i < tool.Operator.Count; i++)
                            {
                                tool.Run();
                                tempImageList.Add(new FilmstripImage() { Image = tool.OutputImage, IsProcessedImage = false });
                            }
                        }
                        catch(Exception ex) 
                        {
                            ECDialogManager.ShowMsg(ex.Message);
                        }
                    }
                    else
                    {
                        string[] files = Directory.GetFiles(WorkImageSource.ImageSourceInfo.ImageFilePath);
                        CogImageFileTool tool = new CogImageFileTool();

                        foreach (string path in files)
                        {
                            if (path.EndsWith(".bmp", StringComparison.InvariantCultureIgnoreCase) || path.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase)
                           || path.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) || path.EndsWith(".tiff", StringComparison.InvariantCultureIgnoreCase))
                            {
                                try
                                {
                                    tool.Operator.Open(path, CogImageFileModeConstants.Read);
                                    tool.Run();
                                    tempImageList.Add(new FilmstripImage() { Image = tool.OutputImage, IsProcessedImage = false });
                                }
                                catch(Exception ex)
                                {
                                    ECDialogManager.ShowMsg(path+ex.Message);
                                }
                            }
                        }
                    }
                    DispatcherHelper.UIDispatcher.Invoke(() =>
                    {
                        ImageList?.Clear();
                        ImageList=new BindingList<FilmstripImage>();
                        foreach(FilmstripImage image in tempImageList)
                        {
                            ImageList.Add(image);
                        }
                        tempImageList.Clear();
                        CurrentImageIndex = 0;
                        ImageCount = ImageList.Count;
                    });
                }, ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.Importing));
                
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            CmdOpenImageFile = new RelayCommand(OpenImageSourceFile);
            CmdEditTBAcq = new RelayCommand(EditAcqTB);
            CmdRunOnce = new RelayCommand(RunOnce);
            CmdRunSelectedImage = new RelayCommand<int>(RunSelectedImage);
            CmdResetImageSeqConfig = new RelayCommand(ResetImageSeqConfig);
            CmdLoadImages = new RelayCommand(LoadImages);
            CmdNext = new RelayCommand(DoNext);
            CmdPrevious = new RelayCommand(DoPrevious);
            CmdFirst = new RelayCommand(DoFirst);
            CmdLast = new RelayCommand(DoLast);
        }

        /// <summary>
        /// 查看最后一张
        /// </summary>
        private void DoLast()
        {
            if (ImageList != null)
                SelectedImageIndex = ImageList.Count - 1;
            RunSelectedImage(SelectedImageIndex);
        }

        /// <summary>
        /// 查看第一张
        /// </summary>
        private void DoFirst()
        {
            if (ImageList != null)
                SelectedImageIndex = 0;
            RunSelectedImage(SelectedImageIndex);
        }

        /// <summary>
        /// 查看下一张
        /// </summary>
        private void DoNext()
        {
            if (ImageList?.Count > 0 && SelectedImageIndex < ImageList.Count - 1)
            {
                SelectedImageIndex += 1;
            }
            else
                SelectedImageIndex = 0;
            RunSelectedImage(SelectedImageIndex);
        }

        /// <summary>
        /// 查看上一张
        /// </summary>
        private void DoPrevious()
        {
            if (ImageList?.Count > 0 && SelectedImageIndex >= 1)
            {
                SelectedImageIndex -= 1;
            }
            else if (ImageList?.Count > 0)
                SelectedImageIndex = ImageList.Count - 1;
            RunSelectedImage(SelectedImageIndex);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Cleanup()
        {
            ImageList?.Clear();
            WorkImageSource.Dispose();
        }

        #endregion

        #region 属性
        /// <summary>
        /// 图像源
        /// </summary>
        private ECWorkImageSource _workImageSource;

        public ECWorkImageSource WorkImageSource
        {
            get { return _workImageSource; }
            set { _workImageSource = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 输出的图像
        /// </summary>
        private BitmapImage outputImage;

        public BitmapImage OutputImage
        {
            get { return outputImage; }
            set
            {
                outputImage = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 图像列表
        /// </summary>
        private BindingList<FilmstripImage> _imageList;

        public BindingList<FilmstripImage> ImageList
        {
            get { return _imageList; }
            set { _imageList = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 选择图像的索引
        /// </summary>
        private int _selectedImageIndex;

        public int SelectedImageIndex
        {
            get { return _selectedImageIndex; }
            set
            {
                _selectedImageIndex = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 当前索引
        /// </summary>
        private int _currentImageIndex;

        public int CurrentImageIndex
        {
            get { return _currentImageIndex; }
            set {
                _currentImageIndex = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// 图像数量
        /// </summary>
        private int _imageCount;

        public int ImageCount
        {
            get { return _imageCount; }
            set { _imageCount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 本机可用的IP地址
        /// </summary>
        private BindingList<string> _localIPs;

        public BindingList<string> LocalIPs
        {
            get { return _localIPs; }
            set
            {
                _localIPs = value;
                RaisePropertyChanged();
            }
        }

        #endregion 属性

    }

    public class FilmstripImage : ObservableObject
    {
        public FilmstripImage()
        {

        }

        /// <summary>
        /// 图片
        /// </summary>
        private ICogImage _image;

        public ICogImage Image
        {
            get { return _image; }
            set { _image = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 是否是当前处理的图片
        /// </summary>
        private bool _isProcessedImage;

        public bool IsProcessedImage
        {
            get { return _isProcessedImage; }
            set { _isProcessedImage = value;
                RaisePropertyChanged();
            }
        }

    }
}
