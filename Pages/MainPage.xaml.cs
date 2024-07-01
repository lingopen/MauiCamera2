namespace MauiCamera2.Pages;

public partial class MainPage : BasePage<MainPageModel>
{
    public MainPage(MainPageModel vm) : base(vm) => InitializeComponent();

    protected override bool OnBackButtonPressed()
    {
        base.OnBackButtonPressed();
        MainThread.BeginInvokeOnMainThread(new Action(async () =>
        {
            if (await DisplayAlert("退出提醒".Translate(),
                "您将要退出程序,确定要退出吗？".Translate(),
                "确定",
                "取消"))
            {
                App.Current?.Quit();
            }
        }));
        return true;
    }
    private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        if (picker != null && BindingContext != null && BindingContext is MainPageModel _viewModel)
        {
            try
            {
                if (_viewModel.Langs != null && _viewModel.Langs.Any())
                {
                    Constants.SysConfig.LangIndex = picker.SelectedIndex;
                    var currentLang = _viewModel.Langs[Constants.SysConfig.LangIndex];//选中第一个
                    await LocalizationHelper.ChangeLanguage(currentLang.Code);
                }
            }
            catch  
            {
 
            }
           
        }

    }
}