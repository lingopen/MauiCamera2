namespace MauiCamera2
{
    /// <summary>
    /// 多语言切换
    /// </summary>
    public static class LocalizationHelper
    {
        /// <summary>
        /// 加载语言资源文件
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        async static Task<Dictionary<string, string>?> LoadLangResource(string languageCode)
        {
            string fileName = $"lang.{languageCode}.json";
            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            if (string.IsNullOrEmpty(json)) new Dictionary<string, string>();
            return json.FromJson<Dictionary<string, string>>();
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="languageCode"></param>
        public static async Task ChangeLanguage(string languageCode)
        {
            Dictionary<string, string>? languageResource = await LoadLangResource(languageCode);

            if (languageResource != null)
            {
                // 清空现有的资源
                Application.Current?.Resources.Clear();

                // 将加载的语言资源逐个添加到全局资源中
                if (Application.Current != null)
                    foreach (var entry in languageResource)
                    {
                        Application.Current.Resources[entry.Key] = entry.Value;
                    }
            }
        }
        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Translate(this string key)
        {
            // 根据键值获取消息文本
            if (Application.Current != null && Application.Current.Resources.ContainsKey(key))
            {
                return Application.Current?.Resources[key].ToString() ?? key;
            }
            else
            {
                return key;
            }

        }
    }
}
