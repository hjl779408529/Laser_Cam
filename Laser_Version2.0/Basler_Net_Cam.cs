using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PylonC.NET;
using PylonC.NETSupportLibrary;

namespace Laser_Version2._0
{
    
    class Basler_Net_Cam
    {
        
        private ImageProvider m_imageProvider = new ImageProvider(); /* Create one image provider. */
        public Bitmap m_bitmap = null; /* The bitmap is used for displaying the image. */
        public List<DeviceEnumerator.Device> Device_list = new List<DeviceEnumerator.Device>();
        public bool Finish;//拍照完成标志
        //构造函数
        public Basler_Net_Cam()
        {
            //pylon初始化
            PylonC.NET.Pylon.Initialize();

            /* Register for the events of the image provider needed for proper operation. */
            m_imageProvider.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback);
            m_imageProvider.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback);
            m_imageProvider.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback);
            m_imageProvider.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback);
            m_imageProvider.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback);
            m_imageProvider.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback);
            m_imageProvider.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback);
            
            /* Provide the controls in the lower left area with the image provider object. */
            //sliderGain.MyImageProvider = m_imageProvider;
            //sliderExposureTime.MyImageProvider = m_imageProvider;
            //sliderHeight.MyImageProvider = m_imageProvider;
            //sliderWidth.MyImageProvider = m_imageProvider;
            //comboBoxTestImage.MyImageProvider = m_imageProvider;
            //comboBoxPixelFormat.MyImageProvider = m_imageProvider;

            /* Update the list of available devices in the upper left area. */
            UpdateDeviceList();

            /* Enable the tool strip buttons according to the state of the image provider. */
            //EnableButtons(m_imageProvider.IsOpen, false);
        }
        //打开相机
        public void Open_Cam()
        {
            /* Close the currently open image provider. */
            /* Stops the grabbing of images. */
            Stop();
            /* Close the image provider. */
            CloseTheImageProvider();

            //选择相机
            if (Device_list.Count > 0)
            {
                m_imageProvider.Open(Device_list[0].Index);
            }
            else
            {
                MessageBox.Show("无可用相机！！");
            }
        }
        //拍摄单张图片
        public void OneShot() 
        {
            Finish = false;
            m_imageProvider.OneShot(); /* Starts the grabbing of one image. */
        }
        //连续拍照
        public void ContinuousShot()
        {
            Finish = false;
            m_imageProvider.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
        }
        //停止
        public void Stop() 
        {
            m_imageProvider.Stop();
        }
        //释放句柄元素
        private void CloseTheImageProvider()
        {
            /* Close the image provider. */
            m_imageProvider.Close();
        }
        /* 错误和调试信息输出 */
        private void ShowException(Exception e, string additionalErrorMessage)
        {
            string more = "\n\nLast error message (may not belong to the exception):\n" + additionalErrorMessage;
            MessageBox.Show("Exception caught:\n" + e.Message + (additionalErrorMessage.Length > 0 ? more : ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        //释放资源
        public void Dispose()
        {
            /* Stops the grabbing of images. */
            Stop();
            /* Close the image provider. */
            CloseTheImageProvider();
        }
        /* Updates the list of available devices in the upper left area. */
        private void UpdateDeviceList()
        {
            /* Ask the device enumerator for a list of devices. */
            Device_list.Clear();
            Device_list =new List<DeviceEnumerator.Device>(DeviceEnumerator.EnumerateDevices());
        }
        /* Handles the event related to the occurrence of an error while grabbing proceeds. */
        private void OnGrabErrorEventCallback(Exception grabException, string additionalErrorMessage)
        {
            MessageBox.Show("抓取失败");
        }
        /* Handles the event related to the removal of a currently open device. */
        private void OnDeviceRemovedEventCallback()
        {
            MessageBox.Show("设备移除");
            /* Since one device is gone, the list needs to be updated. */
            UpdateDeviceList();
        }
        /* Handles the event related to a device being open. */
        private void OnDeviceOpenedEventCallback()
        {
            MessageBox.Show("设备打开");
        }
        /* Handles the event related to a device being closed. */
        private void OnDeviceClosedEventCallback()
        {
            MessageBox.Show("设备关闭");
        }
        /* Handles the event related to the image provider executing grabbing. */
        private void OnGrabbingStartedEventCallback()
        {
            //MessageBox.Show("启动抓取");
        }
        /* Handles the event related to an image having been taken and waiting for processing. */
        private void OnImageReadyEventCallback()
        {

            /* Acquire the image from the image provider. Only show the latest image. The camera may acquire images faster than images can be displayed*/
            ImageProvider.Image image = m_imageProvider.GetLatestImage();

            /* Check if the image has been removed in the meantime. */
            if (image != null)
            {
                /* Check if the image is compatible with the currently used bitmap. */
                if (BitmapFactory.IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                {
                    /* Update the bitmap with the image data. */
                    BitmapFactory.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    Finish = true;
                }
                else /* A new bitmap is required. */
                {
                    BitmapFactory.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                    BitmapFactory.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                    Finish = true;
                }
                /* The processing of the image is done. Release the image buffer. */
                m_imageProvider.ReleaseImage();
                /* The buffer can be used for the next image grabs. */
            }
        }
        /* Handles the event related to the image provider having stopped grabbing. */
        private void OnGrabbingStoppedEventCallback()
        {
            //MessageBox.Show("启动停止");   
        }
    }
}
