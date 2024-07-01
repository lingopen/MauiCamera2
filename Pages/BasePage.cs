namespace MauiCamera2.Pages
{
    public abstract class BasePage<TViewModel> : BasePage where TViewModel : BaseViewModel
    {
        public new TViewModel BindingContext => (TViewModel)base.BindingContext;

        protected BasePage(TViewModel viewModel) : base(viewModel)
        {
        }

        protected override bool OnBackButtonPressed()
        {
            return true;//禁止返回 
        }
        protected override async void OnAppearing()
        {
            await BindingContext.OnAppearing();
        }
        protected override async void OnDisappearing()
        {
            await BindingContext.OnDisappearing();
        }
    }
    public abstract class BasePage : ContentPage
    {
        protected BasePage(object? viewModel = null)
        {
            BindingContext = viewModel;
        }
    }
}
