namespace MauiCamera2.Pages
{
    public partial class Camera2FaceDetectorPage : BasePage<Camera2FaceDetectorPageModel>
    {
        public Camera2FaceDetectorPage(Camera2FaceDetectorPageModel vm) : base(vm)
        {
            InitializeComponent();
            this.textureView.HandlerChanged += vm.TextureView_HandlerChanged;
            this.faceView.HandlerChanged += vm.FaceView_HandlerChanged;
        }
         
    }
}