using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiCamera2.Models;
using MauiCamera2.Services;
using MauiCamera2.Sqlite.Entitys;
using System.Collections.ObjectModel;
using Yitter.IdGenerator;
namespace MauiCamera2.Pages
{
    public partial class MainPageModel : BaseViewModel
    {
        /// <summary>
        /// 登录用户
        /// </summary>
        [ObservableProperty] SysConfig loginInfo;

        /// <summary>
        /// 在线状态
        /// </summary>
        [ObservableProperty] bool isOnLine;
        /// <summary>
        /// 系统版本
        /// </summary>
        [ObservableProperty] string version;
        /// <summary>
        /// 语言
        /// </summary>
        [ObservableProperty]
        ObservableCollection<LanguageOption> langs = new ObservableCollection<LanguageOption>();
        /// <summary>
        /// 選中的語言
        /// </summary>
        [ObservableProperty]
        LanguageOption? currentLang;

        public MainPageModel(IPlatformService platformService, IDbService dbService, IHttpService httpService)
        {
            Langs.Clear();
            Langs = new ObservableCollection<LanguageOption>
            {
                new LanguageOption("简体中文", "zh-cn"),
                new LanguageOption("繁體中文", "zh-tw"),
            };
            Constants.Plateform = platformService;
            Constants.LocalDb = dbService;
            Constants.Api = httpService;
        }
        public override async Task OnAppearing()
        {
            IsOnLine = Constants.HasNetWork;
            await Constants.Plateform.CheckPermissions();
            await Constants.LocalDb.Connect();
            Version = AppInfo.Current.VersionString;

            //加载配置信息
            Constants.SysConfig = await Constants.LocalDb.GetAsync<SysConfig>(p => p.Id > 0);
            if (Constants.SysConfig == null)
                Constants.SysConfig = new SysConfig()
                {
                    Id = YitIdHelper.NextId(),
                    UserId = 1,
                    LastLoginTime = DateTime.Now,
                    IsRemenber = false,
                    Account = "admin",
                    Password = "123456",
                    LangIndex = 0,//默认

                    OrgName = "lingopen",
                    RealName = "系统管理员",
                    Email = "xlingopen@126.com",
                    Phone = "18981794811",
#if DEBUG
                    ServerUrl = "http://192.168.31.187:5566"
#else
                    ServerUrl = "http://118.24.37.67:5566"
#endif
                };

            LoginInfo = Constants.SysConfig;
            if (Langs.Any())
            {
                CurrentLang = Langs[Constants.SysConfig.LangIndex];//选中第一个
                await LocalizationHelper.ChangeLanguage(CurrentLang.Code);
            }
            Constants.ServerUrl = Constants.SysConfig.ServerUrl ?? "";

            await Constants.LocalDb.SaveAsync<SysConfig>(Constants.SysConfig);//保存
            ////校对APP系统时间，差异10分钟以上需要提示修改时间
            //var sys_datetime = await Constants.Api.GetAsync<DateTime?>("/api/app/time");
            //if (sys_datetime.code == 200 && sys_datetime.result.HasValue
            //    && DateTime.Now.ToString("yyyyMMddHH") != sys_datetime.result.Value.ToString("yyyyMMddHH"))
            //{
            //    string text = "警告,系统时间不符，请在设置中进行修改!".Translate();
            //    await SnackEx.ShowAlarmAsync(text, 60, "OK", () =>
            //    {
            //        Constants.Plateform.OpenTimeSetting();
            //    });
            //} 
        }

        [RelayCommand]
        async Task OpenCamera2()
        {
            try
            {
                IsWaitting = true;
                await Shell.Current.GoToAsync($"///MainPage/Camera2Page");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("提示".Translate(), ex.Message, "确定".Translate(), "取消".Translate());
                return;
            }
            finally
            {
                IsWaitting = false;
            }
        }

        [RelayCommand]
        async Task OpenFaceDetector()
        {
            try
            {
                IsWaitting = true;
                await Shell.Current.GoToAsync($"///MainPage/Camera2FaceDetectorPage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("提示".Translate(), ex.Message, "确定".Translate(), "取消".Translate());
                return;
            }
            finally
            {
                IsWaitting = false;
            }
        }
        [RelayCommand]
        async Task OpenBLEPage()
        {
            try
            {
                IsWaitting = true;
                await Shell.Current.GoToAsync($"///MainPage/BLEPage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("提示".Translate(), ex.Message, "确定".Translate(), "取消".Translate());
                return;
            }
            finally
            {
                IsWaitting = false;
            }
        }
    }
}
