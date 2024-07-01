using Android.Media;
using Stream = Android.Media.Stream;
using MauiCamera2.Services;

namespace MauiCamera2.Platforms.Droid.Services
{
    public class SoundService : ISoundService
    {
        private SoundPool soundPool;
        private int soundPoolId_SUCCESS, soundPoolId_FAIL;

        [Obsolete]
        public SoundService()
        {
            soundPool = new SoundPool(10, Stream.Music, 0);
            soundPoolId_SUCCESS = soundPool.Load(MauiApplication.Context, CommunityToolkit.Maui.Resource.Raw.beeps, 1);
            soundPoolId_FAIL = soundPool.Load(MauiApplication.Context, CommunityToolkit.Maui.Resource.Raw.error, 1);
        }

        public void PlayFail()
        {
            //第一个参数为id
            //第二个和第三个参数为左右声道的音量控制
            //第四个参数为优先级，由于只有这一个声音，因此优先级在这里并不重要

            //第五个参数为是否循环播放，0为不循环，-1为循环
            //
            //最后一个参数为播放比率，从0.5到2，一般为1，表示正常播放。
            soundPool.Play(soundPoolId_FAIL, 1, 1, 0, 0, 1);
        }

        public void PlaySuccess()
        {
            //第一个参数为id
            //第二个和第三个参数为左右声道的音量控制
            //第四个参数为优先级，由于只有这一个声音，因此优先级在这里并不重要

            //第五个参数为是否循环播放，0为不循环，-1为循环
            //
            //最后一个参数为播放比率，从0.5到2，一般为1，表示正常播放。
            soundPool.Play(soundPoolId_SUCCESS, 1, 1, 0, 0, 1);
        }
    }
}
