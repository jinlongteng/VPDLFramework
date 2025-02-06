using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPDLFramework.Models
{
    public class ECBufferQueue:ObservableObject
    {
        public ECBufferQueue(int bufferCount) 
        {
            MaxBufferCount = bufferCount;
            BufferImages = new BindingList<ECWorkImageSourceOutput>();
            ImagesTriggerIndex = new BindingList<int>();
            ImagesTriggerTime = new BindingList<DateTime>();
        }

        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool AddImage(ECWorkImageSourceOutput imageSourceOutput,int triggerIndex,DateTime triggrTime)
        {
            try
            {
                if (BufferImages.Count() < MaxBufferCount)
                {
                    BufferImages.Add(imageSourceOutput);
                    ImagesTriggerIndex.Add(triggerIndex);
                    ImagesTriggerTime.Add(triggrTime);
                    return true;
                }
                else  
                    return false;
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                return false;
            }
            
        }

        /// <summary>
        /// 获取队列最前面的图像
        /// </summary>
        /// <returns></returns>
        public ECWorkImageSourceOutput GetNextImage()
        {
            ECWorkImageSourceOutput imageSourceOutput = null;
            if (BufferImages.Count() > 0)
            {
                imageSourceOutput =BufferImages[0];
                BufferImages.RemoveAt(0);
            }
            return imageSourceOutput;
        }

        /// <summary>
        /// 获取队列最前面的图像触发序号
        /// </summary>
        /// <returns></returns>
        public int GetNextImageTriggerIndex()
        {
            int index = -1;
            if (ImagesTriggerIndex.Count() > 0)
            {
                index=ImagesTriggerIndex[0];
                ImagesTriggerIndex.RemoveAt(0);
            }
            return index;
        }

        /// <summary>
        /// 获取队列最前面的图像触发时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetNextImageTriggerTime()
        {
            DateTime time = DateTime.Now;
            if (ImagesTriggerTime.Count() > 0)
            {
                time = ImagesTriggerTime[0];
                ImagesTriggerTime.RemoveAt(0);
            }
            return time;
        }

        /// <summary>
        /// 缓存的图片
        /// </summary>
        private BindingList<ECWorkImageSourceOutput> _bufferImages;

        public BindingList<ECWorkImageSourceOutput> BufferImages
        {
            get { return _bufferImages; }
            set
            {
                _bufferImages = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 最大缓存数量
        /// </summary>
        private int _maxBufferCount;

        public int MaxBufferCount
        {
            get { return _maxBufferCount; }
            set { _maxBufferCount = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 缓存图像的触发序号
        /// </summary>
        private BindingList<int> _imagesTriggerIndex;

        public BindingList<int> ImagesTriggerIndex
        {
            get { return _imagesTriggerIndex; }
            set { _imagesTriggerIndex = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 缓存图像的触发时间
        /// </summary>
        private BindingList<DateTime> _imagesTriggerTime;

        public BindingList<DateTime> ImagesTriggerTime
        {
            get { return _imagesTriggerTime; }
            set
            {
                _imagesTriggerTime = value;
                RaisePropertyChanged();
            }
        }
    }
}
