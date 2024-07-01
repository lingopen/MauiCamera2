using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCamera2.Converters
{
    public class ImageUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string uri)
            {
                if (uri.StartsWith("/"))
                    return ImageSource.FromUri(new Uri($"{Constants.ServerUrl}{uri}"));
                else
                    return ImageSource.FromUri(new Uri($"{Constants.ServerUrl}/{uri}"));
            }
            else return ImageSource.FromFile(@"photo.svg");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
