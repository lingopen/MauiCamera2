namespace MauiCamera2.Pages
{
    public partial class Camera2Page : BasePage<Camera2PageModel>
    {
        public Camera2Page(Camera2PageModel vm) : base(vm)
        {
            InitializeComponent();
            this.textureView.HandlerChanged += vm.TextureView_HandlerChanged;
        } 
    }
}