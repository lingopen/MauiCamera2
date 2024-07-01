using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiCamera2.Pages
{
    /// <summary>
    /// 视图模型基类
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        /// <summary>
        /// 在页面显示时执行逻辑
        /// </summary>
        public virtual Task OnAppearing()
        {

            // 在页面显示时执行逻辑
            return Task.CompletedTask;
        }

        /// <summary>
        /// 在页面隐藏时执行逻辑
        /// </summary>
        public virtual Task OnDisappearing()
        {
            // 在页面隐藏时执行逻辑
            return Task.CompletedTask;
        }

        /// <summary>
        /// 正在加载
        /// </summary> 
        [ObservableProperty]
        bool isWaitting;

        /// <summary>
        /// 正在运行提示
        /// </summary>
        [ObservableProperty]
        string? waittingText;

        /// <summary>
        /// 返回上一级页面前事件
        /// </summary>
        protected virtual void OnBacking()
        {

        }

        /// <summary>
        /// 回退上一个视图
        /// </summary> 
        [RelayCommand]
        protected virtual async Task OnBack()
        {
            try
            {
                OnBacking();
                await Shell.Current.GoToAsync(".."); //返回上一级页面
            }
            catch (Exception)
            {

            }

        }
    }
}
