using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MauiCamera2.Platforms.Droid.Controls;
using static Android.Hardware.Camera2.CameraCaptureSession;
using Color = Android.Graphics.Color;
using Face = Android.Hardware.Camera2.Params.Face;
using Matrix = Android.Graphics.Matrix;
using Platform = Microsoft.Maui.ApplicationModel.Platform;
using Point = Android.Graphics.Point;
using Rect = Android.Graphics.Rect;
using RectF = Android.Graphics.RectF;
using Size = Android.Util.Size;
using MauiCamera2.Services;
namespace MauiCamera2.Platforms.Droid.Services
{
    public class Camera2Service : ICamera2Service
    {
        public Camera2Service()
        {
            ORIENTATIONS.Append((int)DisplayRotation.Rotation0, 90);
            ORIENTATIONS.Append((int)DisplayRotation.Rotation90, 0);
            ORIENTATIONS.Append((int)DisplayRotation.Rotation180, 270);
            ORIENTATIONS.Append((int)DisplayRotation.Rotation270, 180);
        }


        public event EventHandler<byte[]?>? CallBack;//拍照回调
        static SparseIntArray ORIENTATIONS = new SparseIntArray();
        private TextureViewEx? mPreviewSurfaceView;//预览视图

        private TextureViewEx? mFacesSurfaceView; //人脸矩形视图
        private StatisticsFaceDetectMode mFaceDetectMode = StatisticsFaceDetectMode.Off;     //人脸检测模式
        private List<RectF> mFacesRect = new List<RectF>();                                //保存人脸坐标信息


        string mCameraId = "0";//相机ID 

        const int MAX_PREVIEW_WIDTH = 1920;    //预览的宽度
        const int MAX_PREVIEW_HEIGHT = 1080;      //预览的高度
        private Size? mPreviewSize;//预览大小

        protected TextureViewEx? mTextureView; //UI视图 
        CameraManager? mCameraManager;//相机管理器
        protected ImageReader? mImageReader;//读取器
        CameraDevice? mCameraDevice;//相机设备
        protected CameraCaptureSession? mCameraCaptureSession;//相机会话
        protected CameraCharacteristics? mCharacteristics;//相机特征  

        protected int mSensorOrientation = 0;//相机方向
        protected int mCameraFacing = (int)LensFacing.Front; //默认使用前置相机  

        protected bool canTakePic = true; //是否可以拍照
        protected bool canExchangeCamera = false;//是否可以切换相机

        protected Handler? mCameraHandler;//相机处理程序
        protected HandlerThread? handlerThread = new HandlerThread("Camera2Thread");//相机线程  
        private Android.Graphics.Paint mFacePaint = new Android.Graphics.Paint();
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
            mPreviewSize = new Size(MAX_PREVIEW_WIDTH, MAX_PREVIEW_HEIGHT); //重置预览大小 
            CloseCamera();
            OpenCameraInfo(mWidth, mHeight);
        }
        /// <summary>
        /// 传入2个视图
        /// </summary>
        /// <param name="view">预览视图</param>
        /// <param name="faceView">人脸绘制视图</param>
        public void CreateCamera(object? view, object? faceView)
        {
            if (view == null) return;
            if (handlerThread == null) handlerThread = new HandlerThread("Camera2Thread");//相机线程
            handlerThread.Start();
            if (handlerThread?.Looper != null)
                mCameraHandler = new Handler(handlerThread.Looper);
            mTextureView = view as TextureViewEx;
            mFacesSurfaceView = faceView as TextureViewEx;
            mFacesSurfaceView?.SetOpaque(false);
            mFacePaint.SetStyle(Android.Graphics.Paint.Style.Stroke);
            mFacePaint.Color = Color.ParseColor("#d7e2f9");
            if (mTextureView != null)
                mTextureView.SurfaceTextureListener = new SurfaceTextureListener(this);
        }
        /**
    * Retrieves the JPEG orientation from the specified screen rotation.
    *
    * @param rotation The screen rotation.
    * @return The JPEG orientation (one of 0, 90, 270, and 360)
    */
        private int GetOrientation(int rotation)
        {
            // Sensor orientation is 90 for most devices, or 270 for some devices (eg. Nexus 5X)
            // We have to take that into account and rotate JPEG properly.
            // For devices with orientation of 90, we simply return our mapping from ORIENTATIONS.
            // For devices with orientation of 270, we need to rotate the JPEG 180 degrees.
            return (ORIENTATIONS.Get(rotation) + mSensorOrientation + 270) % 360;
        }


        public void CloseCamera()
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
            if (mImageReader == null || mImageReader.Surface == null) return;
            captureRequestBuilder.AddTarget(mImageReader.Surface);
            if (CaptureRequest.ControlAfMode != null)
                captureRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture); // 自动对焦
            if (CaptureRequest.ControlAeMode != null)
                captureRequestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);  // 闪光灯
            if (CaptureRequest.JpegOrientation != null)
            {
                var orientation = mCharacteristics?.Get(CameraCharacteristics.SensorOrientation);
                if (orientation != null)
                    mSensorOrientation = (int)orientation;
                captureRequestBuilder.Set(CaptureRequest.JpegOrientation, GetOrientation(mSensorOrientation));     //根据摄像头方向对保存的照片进行旋转，使其为"自然方向"
            }
            if (mCameraCaptureSession != null)
                mCameraCaptureSession.Capture(captureRequestBuilder.Build(), null, mCameraHandler);
            else Toast.MakeText(Platform.CurrentActivity, "拍照异常!", ToastLength.Short)?.Show();
        }

        /// <summary>
        /// 创建预览会话
        /// </summary>
        void CreateCameraPreviewSession()
        {
            SurfaceTexture? texture = mTextureView?.SurfaceTexture;
            if (texture == null) return;
            // We configure the size of default buffer to be the size of camera preview we want.
            texture.SetDefaultBufferSize(mPreviewSize.Width, mPreviewSize.Height);
            // This is the output Surface we need to start preview.
            Surface surface = new Surface(texture);
            // We set up a CaptureRequest.Builder with the output Surface.
            var captureRequestBuilder = mCameraDevice?.CreateCaptureRequest(CameraTemplate.Preview);
            if (captureRequestBuilder == null) return;
            captureRequestBuilder.AddTarget(surface); // 将CaptureRequest的构建器与Surface对象绑定在一起
            if (CaptureRequest.ControlAeMode != null)
                captureRequestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);      // 闪光灯
            if (CaptureRequest.ControlAfMode != null)
                captureRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture); // 自动对焦 
            if (mFaceDetectMode != StatisticsFaceDetectMode.Off)
            {
                if (CaptureRequest.ControlMode != null)
                    captureRequestBuilder.Set(CaptureRequest.ControlMode, (int)ControlMode.Auto);
                if (CaptureRequest.StatisticsFaceDetectMode != null)
                    captureRequestBuilder.Set(CaptureRequest.StatisticsFaceDetectMode, (int)mFaceDetectMode);//人脸检测
            }
            if (mImageReader != null && mImageReader.Surface != null)
                // 为相机预览，创建一个CameraCaptureSession对象 
                mCameraDevice?.CreateCaptureSession([surface, mImageReader.Surface],
                    new CameraCaptureStateListener()
                    {
                        OnConfiguredAction = session =>
                        {
                            mCameraCaptureSession = session;
                            session.SetRepeatingRequest(captureRequestBuilder.Build(), new CameraCaptureCallbackListener(this), mCameraHandler);
                        }
                    }, mCameraHandler);
        }

        /**
        * Compares two {@code Size}s based on their areas.
   */
        class CompareSizesByArea : IComparer<Size>
        {
            public int Compare(Size? lhs, Size? rhs)
            {
                if (lhs == null || rhs == null) return 0;
                return Java.Lang.Long.Signum((long)lhs.Width * lhs.Height - (long)rhs.Width * rhs.Height);
            }
        }

        /**
             * Given {@code choices} of {@code Size}s supported by a camera, choose the smallest one that
             * is at least as large as the respective texture view size, and that is at most as large as the
             * respective max size, and whose aspect ratio matches with the specified value. If such size
             * doesn't exist, choose the largest one that is at most as large as the respective max size,
             * and whose aspect ratio matches with the specified value.
             *
             * @param choices           The list of sizes that the camera supports for the intended output
             *                          class
             * @param textureViewWidth  The width of the texture view relative to sensor coordinate
             * @param textureViewHeight The height of the texture view relative to sensor coordinate
             * @param maxWidth          The maximum width that can be chosen
             * @param maxHeight         The maximum height that can be chosen
             * @param largest           The aspect ratio
             * @return The optimal {@code Size}, or an arbitrary one if none were big enough
        */
        private Size? ChooseOptimalSize(Size[]? choices, int textureViewWidth,
                                              int textureViewHeight, int maxWidth, int maxHeight, Size largest)
        {
            // Collect the supported resolutions that are at least as big as the preview Surface
            List<Size> bigEnough = new List<Size>();
            // Collect the supported resolutions that are smaller than the preview Surface
            List<Size> notBigEnough = new List<Size>();
            int w = largest.Width;
            int h = largest.Height;
            foreach (Size option in choices)
            {
                if (option.Width <= maxWidth && option.Height <= maxHeight &&
                        option.Height == option.Width * h / w)
                {
                    if (option.Width >= textureViewWidth &&
                            option.Height >= textureViewHeight)
                    {
                        bigEnough.Add(option);
                    }
                    else
                    {
                        notBigEnough.Add(option);
                    }
                }
            }

            // Pick the smallest of those big enough. If there is no one big enough, pick the
            // largest of those not big enough.
            if (bigEnough.Any())
            {
                return bigEnough.Min(new CompareSizesByArea());
            }
            else if (notBigEnough.Any())
            {
                return notBigEnough.Max(new CompareSizesByArea());
            }
            else
            {
                return choices[0];
            }
        }
        int mWidth = 0;
        int mHeight = 0;
        /// <summary>
        /// 打开相机
        /// </summary> 
        public void OpenCameraInfo(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            var status = Permissions.RequestAsync<Permissions.Camera>().Result;
            if (status != PermissionStatus.Granted)
            {
                Toast.MakeText(Platform.CurrentActivity, "未取得相机权限!", ToastLength.Short)?.Show();
                return;
            }
            SetUpCameraOutputs(width, height);
            ConfigureTransform(width, height);

            mCameraManager?.OpenCamera(mCameraId, new CameraStateCallback(this), mCameraHandler);
        }
        /**
   * Sets up member variables related to camera.
   *
   * @param width  The width of available size for camera preview
   * @param height The height of available size for camera preview
   */
        public void SetUpCameraOutputs(int width, int height)
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
                    mCharacteristics = cameraCharacteristics;
                }
            }
            var faceDetectModes = (int[]?)mCharacteristics?.Get(CameraCharacteristics.StatisticsInfoAvailableFaceDetectModes);  //人脸检测的模式 
            if (faceDetectModes != null)
            {
                if (faceDetectModes.Any())
                {
                    mFaceDetectMode = faceDetectModes.Contains(2) ? StatisticsFaceDetectMode.Full :
                        faceDetectModes.Contains(1) ? StatisticsFaceDetectMode.Simple :
                        StatisticsFaceDetectMode.Off; //设置人脸检测
                }
            }

            //获取StreamConfigurationMap，它是管理摄像头支持的所有输出格式和尺寸
            var configurationMap = mCharacteristics?.Get(CameraCharacteristics.ScalerStreamConfigurationMap) as StreamConfigurationMap;
            var largest = configurationMap?.GetOutputSizes((int)ImageFormatType.Jpeg)?.Max(new CompareSizesByArea());   //可用的最大照片尺寸 
            if (largest == null) return;
            mImageReader = ImageReader.NewInstance(largest.Width, largest.Height, ImageFormatType.Jpeg, 2);
            mImageReader?.SetOnImageAvailableListener(new ImageAvailableListener(this), mCameraHandler);

            // Find out if we need to swap dimension to get the preview size relative to sensor
            // coordinate.

            var displayRotation = Platform.CurrentActivity?.WindowManager?.DefaultDisplay?.Rotation;
            //noinspection ConstantConditions
            var orientation = mCharacteristics?.Get(CameraCharacteristics.SensorOrientation);
            if (orientation != null)
                mSensorOrientation = (int)orientation;
            bool swappedDimensions = false;
            switch (displayRotation)
            {
                case SurfaceOrientation.Rotation0:
                case SurfaceOrientation.Rotation180:
                    if (mSensorOrientation == 90 || mSensorOrientation == 270)
                    {
                        swappedDimensions = true;
                    }
                    break;
                case SurfaceOrientation.Rotation90:
                case SurfaceOrientation.Rotation270:
                    if (mSensorOrientation == 0 || mSensorOrientation == 180)
                    {
                        swappedDimensions = true;
                    }
                    break;
            }
            Point displaySize = new Point();
            Platform.CurrentActivity?.WindowManager?.DefaultDisplay?.GetSize(displaySize);
            int rotatedPreviewWidth = width;
            int rotatedPreviewHeight = height;
            int maxPreviewWidth = displaySize.X;
            int maxPreviewHeight = displaySize.Y;

            if (swappedDimensions)
            {
                rotatedPreviewWidth = height;
                rotatedPreviewHeight = width;
                maxPreviewWidth = displaySize.Y;
                maxPreviewHeight = displaySize.X;
            }

            if (maxPreviewWidth > MAX_PREVIEW_WIDTH)
            {
                maxPreviewWidth = MAX_PREVIEW_WIDTH;
            }

            if (maxPreviewHeight > MAX_PREVIEW_HEIGHT)
            {
                maxPreviewHeight = MAX_PREVIEW_HEIGHT;
            }

            var previewSize = configurationMap?.GetOutputSizes(Class.FromType(typeof(SurfaceTexture))); //预览尺寸 
            // Danger, W.R.! Attempting to use too large a preview size could  exceed the camera
            // bus' bandwidth limitation, resulting in gorgeous previews but the storage of
            // garbage capture data.
            mPreviewSize = ChooseOptimalSize(previewSize,
                        rotatedPreviewWidth, rotatedPreviewHeight, maxPreviewWidth,
                        maxPreviewHeight, largest) ?? largest;

            // We fit the aspect ratio of TextureView to the size of preview we picked. 
            //根据预览的尺寸大小调整TextureView的大小，保证画面不被拉伸 
            var screenOrientation = Platform.CurrentActivity?.Resources?.Configuration?.Orientation;
            if (screenOrientation != null && screenOrientation == Android.Content.Res.Orientation.Landscape) //横向
            {
                mTextureView?.SetAspectRatio(mPreviewSize.Width, mPreviewSize.Height);
                mFacesSurfaceView?.SetAspectRatio(mPreviewSize.Width, mPreviewSize.Height);
            }
            else
            {
                mTextureView?.SetAspectRatio(mPreviewSize.Height, mPreviewSize.Width);
                mFacesSurfaceView?.SetAspectRatio(mPreviewSize.Height, mPreviewSize.Width);
            }
        }
        /**
             * Configures the necessary {@link Matrix} transformation to `mPreviewSurfaceView`.
             * This method should be called after the camera preview size is determined in
             * setUpCameraOutputs and also the size of `mPreviewSurfaceView` is fixed.
             *
             * @param viewWidth  The width of `mPreviewSurfaceView`
             * @param viewHeight The height of `mPreviewSurfaceView`
     */
        public void ConfigureTransform(int viewWidth, int viewHeight)
        {
            if (mPreviewSurfaceView == null || mPreviewSize == null || Platform.CurrentActivity == null) return;
            var rotation = Platform.CurrentActivity?.WindowManager?.DefaultDisplay?.Rotation;//手机方向
            var matrix = new Matrix();
            var viewRect = new Android.Graphics.RectF(0f, 0f, viewWidth, viewHeight);
            var bufferRect = new RectF(0f, 0f, mPreviewSize.Height, mPreviewSize.Width);
            var centerX = viewRect.CenterX();
            var centerY = viewRect.CenterY();
            if (rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                var scale = Java.Lang.Math.Max
                        (
                            (float)viewHeight / mPreviewSize.Height,
                            (float)viewWidth / mPreviewSize.Width
                        );
                matrix.PostScale(scale, scale, centerX, centerY);
                matrix.PostRotate((float)(90 * ((int)rotation - 2)), centerX, centerY);
            }
            else if (rotation == SurfaceOrientation.Rotation180)
            {
                matrix.PostRotate(180f, centerX, centerY);
            }
            mTextureView?.SetTransform(matrix);
            mFacesSurfaceView?.SetTransform(matrix);
        }



        #region 处理人脸 

        private Face[] GetFacesFromJavaArray(IntPtr arrayPtr)
        {
            int length = JNIEnv.GetArrayLength(arrayPtr);
            List<Face> faces = new List<Face>();

            for (int i = 0; i < length; i++)
            {
                IntPtr facePtr = JNIEnv.GetObjectArrayElement(arrayPtr, i);
                Face face = Java.Lang.Object.GetObject<Face>(facePtr, JniHandleOwnership.TransferLocalRef);
                faces.Add(face);
            }

            return faces.ToArray();
        }

        static Rect? mCameraRect;
        private static RectF RectToRectF(Rect r)
        {
            return new RectF(r.Left, r.Top, r.Right, r.Bottom);
        }

        private static Rect RectFToRect(RectF r)
        {
            return new Rect((int)r.Left, (int)r.Top, (int)r.Right, (int)r.Bottom);
        }


        /// <summary>
        /// 处理人脸
        /// </summary>
        /// <param name="result"></param>
        private void HandleFace(CaptureResult result)
        {
            Face[]? faces = null;
            // 使用 JNI 获取 StatisticsFaces
            var facesJavaObject = result.Get(CaptureResult.StatisticsFaces) as Java.Lang.Object;//这里不能直接转换为 Face[]，会报类型转换失败
            if (facesJavaObject != null)
            {
                // 获取人脸数据的 Java 数组 
                faces = GetFacesFromJavaArray(facesJavaObject.Handle);
            }

            mFacesRect.Clear();

            if (faces != null)
            {
                foreach (var face in faces)
                {
                    var bounds = face.Bounds;
                    if (bounds == null) continue;

                    mCameraRect = (Rect?)mCharacteristics?.Get(CameraCharacteristics.SensorInfoActiveArraySize); //获取成像区域
                    if (mCameraRect != null && mFacesSurfaceView != null)
                    {
                        RectF mappedRect = new RectF();
                        Matrix mCameraToPreviewMatrix = new Matrix();
                        mCameraToPreviewMatrix.MapRect(mappedRect, RectToRectF(bounds));
                        Rect auxRect = new Rect(RectFToRect(mappedRect));

                        var orientation = mCharacteristics?.Get(CameraCharacteristics.SensorOrientation);
                        if (orientation != null)
                            mSensorOrientation = (int)orientation;

                        float x = (mCameraRect.Bottom - mCameraRect.Top) / (float)mFacesSurfaceView.GetmRealWidth();
                        float y = (mCameraRect.Right - mCameraRect.Left) / (float)mFacesSurfaceView.GetmRealHeight();
                        if (mCameraFacing == (int)LensFacing.Back)
                        {
                            switch (mSensorOrientation)
                            {
                                case 90:
                                    mappedRect.Left = (mCameraRect.Bottom - auxRect.Bottom) / x;
                                    mappedRect.Top = auxRect.Left / y;
                                    mappedRect.Right = (mCameraRect.Bottom - auxRect.Top) / x;
                                    mappedRect.Bottom = auxRect.Right / y;
                                    break;
                                case 270:
                                    mappedRect.Left = auxRect.Top / x;
                                    mappedRect.Top = (mCameraRect.Right - auxRect.Right) / y;
                                    mappedRect.Right = auxRect.Bottom / x;
                                    mappedRect.Bottom = (mCameraRect.Right - auxRect.Left) / y;
                                    break;
                            }
                        }
                        else if (mCameraFacing == (int)LensFacing.Front)
                        {
                            switch (mSensorOrientation)
                            {
                                case 270:
                                    mappedRect.Left = (mCameraRect.Bottom - auxRect.Top) / x;
                                    mappedRect.Top = (mCameraRect.Right - auxRect.Right) / y;
                                    mappedRect.Right = (mCameraRect.Bottom - auxRect.Bottom) / x;
                                    mappedRect.Bottom = (mCameraRect.Right - auxRect.Left) / y;
                                    break;

                                case 90:
                                    mappedRect.Left = (auxRect.Bottom-mCameraRect.Top) / x;
                                    mappedRect.Top = (auxRect.Left-mCameraRect.Left ) / y;
                                    mappedRect.Right = (auxRect.Top-mCameraRect.Top  ) / x;
                                    mappedRect.Bottom = ( auxRect.Right- mCameraRect.Left) / y;

                                    break;
                            }
                        }
                        mFacesRect.Add(mappedRect);
                    }
                }
            }
            if (faces != null && mFacesRect.Any())
            {
                Platform.CurrentActivity?.RunOnUiThread(() =>
                {
                    mFacesSurfaceView?.SetFaces(mFacesRect);
                     
                });
            } 
        }
        #endregion


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
                mCamera2Service.mCameraDevice = null;
            }

            public override void OnError(CameraDevice camera, [GeneratedEnum] CameraError error)
            {
                Toast.MakeText(Platform.CurrentActivity, "打开相机失败", ToastLength.Short)?.Show();
                camera.Close();
                mCamera2Service.mCameraDevice = null;
            }

            public override void OnOpened(CameraDevice camera)
            {
                mCamera2Service.mCameraDevice = camera;
                mCamera2Service.CreateCameraPreviewSession();
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
                mCamera2Service.OpenCameraInfo(width, height);
            }

            public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
            {
                mCamera2Service.CloseCamera();
                return true;
            }

            public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
            {
                mCamera2Service.ConfigureTransform(width, height);
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


        class CameraCaptureCallbackListener : CaptureCallback
        {
            Camera2Service mCamera2Service;
            public CameraCaptureCallbackListener(Camera2Service camera2Service)
            {
                mCamera2Service = camera2Service;
            }
            public Action<CameraCaptureSession>? OnConfiguredAction { get; set; }
            public override void OnCaptureProgressed(CameraCaptureSession session, CaptureRequest request, CaptureResult partialResult)
            {
                if (mCamera2Service.mFaceDetectMode != StatisticsFaceDetectMode.Off)
                {
                    mCamera2Service.HandleFace(partialResult);
                }
            }

            public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
            {
                base.OnCaptureCompleted(session, request, result);
                if (mCamera2Service.mFaceDetectMode != StatisticsFaceDetectMode.Off)
                {
                    mCamera2Service.HandleFace(result);
                }
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


    }
}
