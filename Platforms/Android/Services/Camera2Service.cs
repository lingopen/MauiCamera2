using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MauiCamera2.Platforms.Droid.Controls;
using MauiCamera2.Services;
using static Android.Hardware.Camera2.CameraCaptureSession;
using Platform = Microsoft.Maui.ApplicationModel.Platform;
using Size = Android.Util.Size;
namespace MauiCamera2.Platforms.Droid.Services
{
    public class Camera2Service : ICamera2Service
    {
        public event EventHandler<byte[]?>? CallBack;


        const int PREVIEW_WIDTH = 1080;    //预览的宽度
        const int PREVIEW_HEIGHT = 1440;      //预览的高度
        const int SAVE_WIDTH = 720;     //保存图片的宽度
        const int SAVE_HEIGHT = 1280;        //保存图片的高度

        protected TextureViewEx? mTextureView; //UI视图
        CameraManager? mCameraManager;//相机管理器
        protected ImageReader? mImageReader;//读取器
        CameraDevice? mCameraDevice;//相机设备
        protected CameraCaptureSession? mCameraCaptureSession;//相机会话
        protected CameraCharacteristics? mCameraCharacteristics;//相机特征 
        string mCameraId = "0";//相机ID

        protected int mCameraSensorOrientation = 0;//相机方向
        protected int mCameraFacing = (int)LensFacing.Front; //默认使用前置相机 
        protected SurfaceOrientation? mDisplayRotation = Platform.CurrentActivity?.WindowManager?.DefaultDisplay?.Rotation;//手机方向

        protected bool canTakePic = true; //是否可以拍照
        protected bool canExchangeCamera = false;//是否可以切换相机

        protected Handler? mCameraHandler;//相机处理程序
        protected HandlerThread? handlerThread = new HandlerThread("Camera2Thread");//相机线程

        protected Size mPreviewSize = new Size(PREVIEW_WIDTH, PREVIEW_HEIGHT); //预览大小
        Size mSavePicSize = new Size(SAVE_WIDTH, SAVE_HEIGHT);//保存图片大小

        public void ChangeCamera()
        {
            if (mCameraDevice == null || !canExchangeCamera || (mTextureView != null && !mTextureView.IsAvailable)) return;
            if (mCameraFacing == (int)LensFacing.Front)
            {
                mCameraFacing = (int)LensFacing.Back;
            }
            else
            {
                mCameraFacing = (int)LensFacing.Front;
            }
            mPreviewSize = new Size(PREVIEW_WIDTH, PREVIEW_HEIGHT); //重置预览大小

            ReleaseCamera();
            InitCameraInfo();
        }
        public virtual void InitCamera(object? view)
        {
            if (view == null) return;
            if (handlerThread == null) handlerThread = new HandlerThread("Camera2Thread");//相机线程
            handlerThread.Start();
            if (handlerThread?.Looper != null)
                mCameraHandler = new Handler(handlerThread.Looper);
            mTextureView = view as TextureViewEx;
            if (mTextureView != null)
                mTextureView.SurfaceTextureListener = new SurfaceTextureListener(this);
        }



        public void ReleaseCamera()
        {
            mCameraCaptureSession?.Close();
            mCameraCaptureSession = null;

            mCameraDevice?.Close();
            mCameraDevice = null;

            mImageReader?.Close();
            mImageReader = null;

            canExchangeCamera = false;

            handlerThread?.QuitSafely();
            try
            {
                handlerThread?.Join();
                handlerThread = null;
                mCameraHandler = null;
            }
            catch (InterruptedException e)
            {
                // Handle exception
                Console.WriteLine($"InterruptedException: {e.Message}");
            }
        }

        public void TakePicture()
        {
            if (mCameraDevice == null || (mTextureView != null && !mTextureView.IsAvailable) || !canTakePic) return;

            var captureRequestBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);

            captureRequestBuilder.AddTarget(mImageReader?.Surface);
            captureRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture); // 自动对焦
            captureRequestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);  // 闪光灯
            captureRequestBuilder.Set(CaptureRequest.JpegOrientation, mCameraSensorOrientation);     //根据摄像头方向对保存的照片进行旋转，使其为"自然方向"
            if (mCameraCaptureSession != null)
                mCameraCaptureSession.Capture(captureRequestBuilder.Build(), null, mCameraHandler);
            else Toast.MakeText(Platform.CurrentActivity, "拍照异常!", ToastLength.Short)?.Show();
        }

        /// <summary>
        /// 创建预览会话
        /// </summary>
        protected virtual void CreateCaptureSession(CameraDevice cameraDevice)
        {
            var captureRequestBuilder = cameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
            var surface = new Surface(mTextureView?.SurfaceTexture);
            captureRequestBuilder.AddTarget(surface); // 将CaptureRequest的构建器与Surface对象绑定在一起

            captureRequestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);      // 闪光灯
            captureRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture); // 自动对焦

            // 为相机预览，创建一个CameraCaptureSession对象
#pragma warning disable CA1422 // 验证平台兼容性
            cameraDevice.CreateCaptureSession([surface, mImageReader?.Surface],
                new CameraCaptureStateListener()
                {
                    OnConfiguredAction = session =>
                    {
                        mCameraCaptureSession = session;
                        session.SetRepeatingRequest(captureRequestBuilder.Build(), new CameraCaptureCallbackListener(this), mCameraHandler);
                    }
                }, mCameraHandler);
#pragma warning restore CA1422 // 验证平台兼容性
        }


        /// <summary>
        /// 根据提供的屏幕方向 [displayRotation] 和相机方向 [sensorOrientation] 返回是否需要交换宽高
        /// </summary>
        /// <returns></returns>
        protected bool ExchangeWidthAndHeight(SurfaceOrientation? displayRotation, int sensorOrientation)
        {
            var exchange = false;
            switch (displayRotation)
            {
                case SurfaceOrientation.Rotation0:
                case SurfaceOrientation.Rotation180:
                    if (sensorOrientation == 90 || sensorOrientation == 270)
                    {
                        exchange = true;
                    }
                    break;
                case SurfaceOrientation.Rotation90:
                case SurfaceOrientation.Rotation270:
                    if (sensorOrientation == 0 || sensorOrientation == 180)
                    {
                        exchange = true;
                    }
                    break;
            }
            return exchange;
        }
        /**
        *
        * 根据提供的参数值返回与指定宽高相等或最接近的尺寸
        *
        * @param targetWidth   目标宽度
        * @param targetHeight  目标高度
        * @param maxWidth      最大宽度(即TextureView的宽度)
        * @param maxHeight     最大高度(即TextureView的高度)
        * @param sizeList      支持的Size列表
        *
        * @return  返回与指定宽高相等或最接近的尺寸
        *
*/
        private Android.Util.Size? GetBestSize(int targetWidth, int targetHeight, int maxWidth, int maxHeight, List<Android.Util.Size> sizeList)
        {
            var bigEnough = new List<Size>();    //比指定宽高大的Size列表
            var notBigEnough = new List<Size>(); //比指定宽高小的Size列表

            foreach (var size in sizeList)
            {

                //宽<=最大宽度  &&  高<=最大高度  &&  宽高比 == 目标值宽高比
                if (size.Width <= maxWidth && size.Height <= maxHeight
                        && size.Width == size.Height * targetWidth / targetHeight)
                {

                    if (size.Width >= targetWidth && size.Height >= targetHeight)
                        bigEnough.Add(size);
                    else
                        notBigEnough.Add(size);
                }
                //log("系统支持的尺寸: ${size.width} * ${size.height} ,  比例 ：${size.width.toFloat() / size.height}")
            }

            //log("最大尺寸 ：$maxWidth * $maxHeight, 比例 ：${targetWidth.toFloat() / targetHeight}")
            //log("目标尺寸 ：$targetWidth * $targetHeight, 比例 ：${targetWidth.toFloat() / targetHeight}")

            //选择bigEnough中最小的值  或 notBigEnough中最大的值
            if (bigEnough.Any())
            {
                return bigEnough.OrderBy(p => p.Width).FirstOrDefault();
            }
            if (notBigEnough.Any())
            {
                return notBigEnough.OrderByDescending(p => p.Width).FirstOrDefault();
            }
            return sizeList[0];
        }
        /// <summary>
        /// 初始化相机
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void InitCameraInfo()
        {
            mCameraManager = Platform.CurrentActivity?.GetSystemService(Context.CameraService) as CameraManager;
            var cameraIdList = mCameraManager?.GetCameraIdList();
            if (cameraIdList == null || cameraIdList.Length <= 0)
            {
                Toast.MakeText(Platform.CurrentActivity, "没有可用的相机", ToastLength.Short)?.Show();
                return;
            }
            foreach (var id in cameraIdList)
            {
                var cameraCharacteristics = mCameraManager?.GetCameraCharacteristics(id);
                if (cameraCharacteristics == null) continue;
                var facing = cameraCharacteristics.Get(CameraCharacteristics.LensFacing);
                if (facing != null && (int)facing == mCameraFacing)
                {
                    mCameraId = id;
                    mCameraCharacteristics = cameraCharacteristics;
                }
            }

            var supportLevel = mCameraCharacteristics?.Get(CameraCharacteristics.InfoSupportedHardwareLevel);
            if (supportLevel != null && (int)supportLevel == (int)InfoSupportedHardwareLevel.Legacy)
            {
                Toast.MakeText(Platform.CurrentActivity, "相机硬件不支持新特性", ToastLength.Short)?.Show();
            }

            //获取摄像头方向
            var orientation = mCameraCharacteristics?.Get(CameraCharacteristics.SensorOrientation);
            if (orientation != null)
                mCameraSensorOrientation = (int)orientation;

            //获取StreamConfigurationMap，它是管理摄像头支持的所有输出格式和尺寸
            var configurationMap = mCameraCharacteristics?.Get(CameraCharacteristics.ScalerStreamConfigurationMap) as StreamConfigurationMap;


            var savePicSize = configurationMap?.GetOutputSizes((int)Android.Graphics.ImageFormatType.Jpeg);   //保存照片尺寸
            var previewSize = configurationMap?.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))); //预览尺寸


            var exchange = ExchangeWidthAndHeight(mDisplayRotation, mCameraSensorOrientation);

            if (savePicSize != null)
            {
                if (exchange)
                    mSavePicSize = GetBestSize(mSavePicSize.Height, mSavePicSize.Width, mSavePicSize.Height, mSavePicSize.Width, savePicSize.ToList()) ?? savePicSize[0];
                else
                    mSavePicSize = GetBestSize(mSavePicSize.Width, mSavePicSize.Height, mSavePicSize.Width, mSavePicSize.Height, savePicSize.ToList()) ?? savePicSize[0];
            }
            if (previewSize != null)
            {
                if (exchange)
                    mPreviewSize = GetBestSize(mPreviewSize.Height, mPreviewSize.Width, mTextureView?.Height??1280, mTextureView?.Width??720, previewSize.ToList()) ?? previewSize[0];
                else
                    mPreviewSize = GetBestSize(mPreviewSize.Width, mPreviewSize.Height, mTextureView?.Width ?? 720, mTextureView?.Height ?? 1280, previewSize.ToList()) ?? previewSize[0];
            } 

            mTextureView?.SurfaceTexture?.SetDefaultBufferSize(mPreviewSize.Width, mPreviewSize.Height); 
            
            //根据预览的尺寸大小调整TextureView的大小，保证画面不被拉伸 
            var screenOrientation = Platform.CurrentActivity?.Resources?.Configuration?.Orientation;
            if (screenOrientation != null && screenOrientation == Android.Content.Res.Orientation.Landscape) //横向
                mTextureView?.SetAspectRatio(mPreviewSize.Width, mPreviewSize.Height);
            else
                mTextureView?.SetAspectRatio(mPreviewSize.Height, mPreviewSize.Width);


            mImageReader = ImageReader.NewInstance(mPreviewSize.Width, mPreviewSize.Height, ImageFormatType.Jpeg, 1);
            mImageReader?.SetOnImageAvailableListener(new ImageAvailableListener(this), mCameraHandler);

            OnInitFaceDetect();

            OpenCamera();
        }
        /// <summary>
        /// 相机启动前，初始化人脸识别模块
        /// </summary>
        protected virtual void OnInitFaceDetect()
        {

        }
        /// <summary>
        /// 打开相机
        /// </summary>
        private void OpenCamera()
        {
            var status = Permissions.RequestAsync<Permissions.Camera>().Result;
            if (status != PermissionStatus.Granted)
            {
                Toast.MakeText(Platform.CurrentActivity, "未取得相机权限!", ToastLength.Short)?.Show();
                return;
            }
            mCameraManager?.OpenCamera(mCameraId, new CameraStateCallback(this), mCameraHandler);
        }
        class CameraStateCallback : CameraDevice.StateCallback
        {
            Camera2Service mCamera2Service;
            public CameraStateCallback(Camera2Service camera2Service)
            {
                mCamera2Service = camera2Service;
            }

            public override void OnDisconnected(CameraDevice camera)
            {
                camera.Close();
            }

            public override void OnError(CameraDevice camera, [GeneratedEnum] CameraError error)
            {
                Toast.MakeText(Platform.CurrentActivity, "打开相机失败", ToastLength.Short)?.Show();
                camera.Close();
            }

            public override void OnOpened(CameraDevice camera)
            {
                mCamera2Service.mCameraDevice = camera;
                mCamera2Service.CreateCaptureSession(camera);
            }
        }
        class CameraCaptureCallbackListener : CaptureCallback
        {
            Camera2Service mCamera2Service;
            public CameraCaptureCallbackListener(Camera2Service camera2Service)
            {
                mCamera2Service = camera2Service;
            }
            public Action<CameraCaptureSession>? OnConfiguredAction { get; set; }
            public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
            {
                base.OnCaptureCompleted(session, request, result);
                mCamera2Service.canExchangeCamera = true;
                mCamera2Service.canTakePic = true;
            }
            public override void OnCaptureFailed(CameraCaptureSession session, CaptureRequest request, CaptureFailure failure)
            {
                base.OnCaptureFailed(session, request, failure);
                Toast.MakeText(Platform.CurrentActivity, "开启预览失败!", ToastLength.Short)?.Show();
            }
        }
        protected class CameraCaptureStateListener : StateCallback
        {
            public Action<CameraCaptureSession>? OnConfiguredAction { get; set; }
            public override void OnConfigured(CameraCaptureSession session)
            {
                OnConfiguredAction?.Invoke(session);
            }

            public override void OnConfigureFailed(CameraCaptureSession session)
            {
                Toast.MakeText(Platform.CurrentActivity, "开启预览会话失败!", ToastLength.Short)?.Show();
            }
        }
        class SurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
        {
            Camera2Service mCamera2Service;
            public SurfaceTextureListener(Camera2Service camera2Service)
            {
                mCamera2Service = camera2Service;
            }

            public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
            {
                mCamera2Service.InitCameraInfo();
            }

            public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
            {
                mCamera2Service.ReleaseCamera();
                return true;
            }

            public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
            {

            }

            public void OnSurfaceTextureUpdated(SurfaceTexture surface)
            {
            }
        }
        class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
        {
            Camera2Service mCamera2Service;
            public ImageAvailableListener(Camera2Service camera2Service)
            {
                mCamera2Service = camera2Service;
            }

            public void OnImageAvailable(ImageReader? reader)
            {
                var image = reader?.AcquireNextImage();
                var byteBuffer = image?.GetPlanes()?[0].Buffer;
                if (byteBuffer == null) return;
                var byteArray = new byte[byteBuffer.Remaining()];
                byteBuffer.Get(byteArray);
                image?.Close();

                mCamera2Service.CallBack?.Invoke(mCamera2Service, byteArray);
            }
        }
    }
}
