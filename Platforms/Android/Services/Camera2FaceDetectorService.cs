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
using MauiCamera2.Services;
using static Android.Hardware.Camera2.CameraCaptureSession;
using static Android.Views.TextureView;
using Color = Android.Graphics.Color;
using Face = Android.Hardware.Camera2.Params.Face;
using Matrix = Android.Graphics.Matrix;
using Paint = Android.Graphics.Paint;
using Rect = Android.Graphics.Rect;
using RectF = Android.Graphics.RectF;
using Size = Android.Util.Size;
namespace MauiCamera2.Platforms.Droid.Services
{
    public class Camera2FaceDetectorService : Camera2Service, ICamera2FaceDetectorService
    {
        private bool openFaceDetect = true;      //是否开启人脸检测
        private StatisticsFaceDetectMode mFaceDetectMode = StatisticsFaceDetectMode.Off;     //人脸检测模式
        private Matrix mFaceDetectMatrix = new Matrix();                                            //人脸检测坐标转换矩阵
        private List<RectF> mFacesRect = new List<RectF>();                                //保存人脸坐标信息

        FaceViewEx? mFaceView = null;//人脸视图控件


        public void InitFaceDetect(object? view)
        {
            mFaceView = view as FaceViewEx;
        }
        protected override void OnInitFaceDetect()
        {
            if (!openFaceDetect) return;

            //var faceDetectCount = mCameraCharacteristics?.Get(CameraCharacteristics.StatisticsInfoMaxFaceCount);    //同时检测到人脸的数量
            var faceDetectModes = (int[]?)mCameraCharacteristics?.Get(CameraCharacteristics.StatisticsInfoAvailableFaceDetectModes);  //人脸检测的模式

            if (faceDetectModes == null)
            {
                Toast.MakeText(Platform.CurrentActivity, "相机硬件不支持人脸检测!", ToastLength.Short)?.Show();
                return;
            }
            if (faceDetectModes.Any())
            {
                mFaceDetectMode = faceDetectModes.Contains(2) ? StatisticsFaceDetectMode.Full :
                    faceDetectModes.Contains(1) ? StatisticsFaceDetectMode.Simple :
                    StatisticsFaceDetectMode.Off;
            }
            if (mFaceDetectMode == StatisticsFaceDetectMode.Off)
            {
                Toast.MakeText(Platform.CurrentActivity, "相机硬件不支持人脸检测!", ToastLength.Short)?.Show();
                return;
            }

            var cameraSizeRectObj = mCameraCharacteristics?.Get(CameraCharacteristics.SensorInfoActiveArraySize); //获取成像区域
            if (cameraSizeRectObj != null && cameraSizeRectObj is Rect cameraSizeRect)
            {
                mFaceDetectMatrix.Reset();

                int cameraWidth = cameraSizeRect.Width();
                int cameraHeight = cameraSizeRect.Height();
                // 计算缩放因子
                var scaleX = (float)mPreviewSize.Width / cameraWidth;
                var scaleY = (float)mPreviewSize.Height / cameraHeight;

                var mirror = mCameraFacing == (int)LensFacing.Front;//镜像模式

                mFaceDetectMatrix.SetRotate((float)mCameraSensorOrientation);//旋转   
                // 进行缩放
                mFaceDetectMatrix.PostScale((mirror ? -1 : 1) * scaleX, scaleY);
            }
        }
        protected override void CreateCaptureSession(CameraDevice cameraDevice)
        {

            mTextureView?.SurfaceTexture?.SetDefaultBufferSize(mPreviewSize.Width, mPreviewSize.Height);
            var surface = new Surface(mTextureView?.SurfaceTexture);

            var captureRequestBuilder = cameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
            captureRequestBuilder.AddTarget(surface); // 将CaptureRequest的构建器与Surface对象绑定在一起

            captureRequestBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.OnAutoFlash);      // 闪光灯
            captureRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture); // 自动对焦

            if (openFaceDetect && mFaceDetectMode != StatisticsFaceDetectMode.Off)
            {
                captureRequestBuilder.Set(CaptureRequest.ControlMode, (int)ControlMode.Auto);
                captureRequestBuilder.Set(CaptureRequest.StatisticsFaceDetectMode, (int)mFaceDetectMode);//人脸检测
            }
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

        class CameraCaptureCallbackListener : CaptureCallback
        {
            Camera2FaceDetectorService mCamera2FaceDetectorService;
            public CameraCaptureCallbackListener(Camera2FaceDetectorService camera2Service)
            {
                mCamera2FaceDetectorService = camera2Service;
            }
            public Action<CameraCaptureSession>? OnConfiguredAction { get; set; }

            public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
            {
                base.OnCaptureCompleted(session, request, result);
                if (mCamera2FaceDetectorService.openFaceDetect && mCamera2FaceDetectorService.mFaceDetectMode != StatisticsFaceDetectMode.Off)
                {
                    mCamera2FaceDetectorService.HandleFace(result);
                }
                mCamera2FaceDetectorService.canExchangeCamera = true;
                mCamera2FaceDetectorService.canTakePic = true;
            }
            public override void OnCaptureFailed(CameraCaptureSession session, CaptureRequest request, CaptureFailure failure)
            {
                base.OnCaptureFailed(session, request, failure);
                Toast.MakeText(Platform.CurrentActivity, "开启预览失败!", ToastLength.Short)?.Show();
            }
        }
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

        /// <summary>
        /// 处理人脸
        /// </summary>
        /// <param name="result"></param>
        private void HandleFace(TotalCaptureResult result)
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
                    var left = bounds.Left;
                    var top = bounds.Top;
                    var right = bounds.Right;
                    var bottom = bounds.Bottom;

                    var rawFaceRect = new RectF(left, top, right, bottom);

                    // 应用矩阵将矩形转换为屏幕坐标 
                    mFaceDetectMatrix.MapRect(rawFaceRect);
                    mFacesRect.Add(rawFaceRect);
                }
            }
            if (faces != null && mFacesRect.Any())
            {
                Platform.CurrentActivity?.RunOnUiThread(() =>
                {

                    mFaceView?.SetFaces(mFacesRect); //ui显示
                });
            }
        }


        class SurfaceTextureListener : Java.Lang.Object, ISurfaceTextureListener
        {
            Camera2FaceDetectorService mCamera2FaceDetectorService;
            public SurfaceTextureListener(Camera2FaceDetectorService camera2Service)
            {
                mCamera2FaceDetectorService = camera2Service;
            }

            public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
            {
                mCamera2FaceDetectorService.ConfigureTransform(width, height);
                mCamera2FaceDetectorService.InitCameraInfo();
            }

            public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
            {
                mCamera2FaceDetectorService.ReleaseCamera();
                return true;
            }

            public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
            {
                mCamera2FaceDetectorService.ConfigureTransform(width, height);
            }

            public void OnSurfaceTextureUpdated(SurfaceTexture surface)
            {
            }
        }

        void ConfigureTransform(int viewWidth, int viewHeight)
        {
            var rotation = Platform.CurrentActivity?.WindowManager?.DefaultDisplay?.Rotation;//手机方向
            var matrix = new Matrix();
            var viewRect = new RectF(0f, 0f, viewWidth, viewHeight);
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
        } 
    }
}
