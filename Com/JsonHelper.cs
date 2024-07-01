using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace MauiCamera2
{
    /// <summary>
    /// JSON帮助
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 转换为json格式
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public static string ToJson<TType>(this TType entity, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(entity, formatting); // new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
        }
        /// <summary>
        /// 转换为对象 T
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="json">json数据</param>
        /// <returns></returns>
        public static T? FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <summary>
        /// 输入净化 input = 空，则返回""
        /// </summary>
        /// <param name="input">字符串</param>
        /// <returns></returns>
        public static string Value(this string input)
        {
            return string.IsNullOrEmpty(input) ? "" : input.Trim();
        }
        /// <summary>
        /// 获取json特定值
        /// </summary>
        /// <param name="json">json数据</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static string GetJsonValue(this string json, string fieldName)
        {
            try
            {
                if (string.IsNullOrEmpty(json)) return "";
                JObject jo = JObject.Parse(json);
                return jo[fieldName].ToString();
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// json格式化
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string JsonFormatting(this string json)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(json);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return json;
            }
        }

        ///// <summary>
        ///// 加载本地json文件，并进行反序列化
        ///// </summary>
        ///// <typeparam name="T">反序列化类型</typeparam>
        ///// <param name="name">json文件名</param>
        ///// <returns></returns>
        //public static T LoadFile<T>(this string name)
        //{
        //    var lockPath = Path.Combine(AppContext.BaseDirectory, "Configs", name);
        //    if (string.IsNullOrEmpty(lockPath)) return default(T);

        //    var json = System.IO.File.ReadAllText(lockPath, Encoding.UTF8);
        //    if (string.IsNullOrEmpty(json)) return default(T);
        //    return json.FromJson<T>();
        //}
        //public static string LoadFileContent(this string lockPath)
        //{
        //    //var lockPath = Path.Combine(AppContext.BaseDirectory, "Configs", name);
        //    if (string.IsNullOrEmpty(lockPath)) return "";

        //    string json = System.IO.File.ReadAllText(lockPath, System.Text.Encoding.UTF8);
        //    if (string.IsNullOrEmpty(json)) return "";
        //    return json;
        //}
        //public static void ToFile<T>(string path, T t)
        //{
        //    var json = JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.Indented, JSONSerializeSettings);
        //    var fileInfo = new FileInfo(path);
        //    if (!fileInfo.Directory.Exists)
        //    {
        //        fileInfo.Directory.Create();
        //    }
        //    File.WriteAllText(path, json);
        //} 
        /// <summary>
        /// JSON序列化设置
        /// </summary>
        public static JsonSerializerSettings JSONSerializeSettings
        {
            get
            {
                return new JsonSerializerSettings()
                {
                    DateFormatString = "yyyy-MM-dd HH:mm:ss",
                    NullValueHandling = NullValueHandling.Ignore,
                };
            }
        }

        public static T ToAnonymous<T>(this object obj, T t)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(Newtonsoft.Json.JsonConvert.SerializeObject(obj), t);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("《JsonHelper.ToAnonymous》:" + ex.Message);
                return t;
            }


        }

    }
}
