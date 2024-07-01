using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCamera2.Converters
{
    /// <summary>
    /// Converts the incoming value from <see cref="Color"/> and returns the object of a type <see cref="Color"/>.
    /// </summary>
    public class ColorToColorForTextConverter : BaseConverterOneWay<Color, Color>
    {
        /// <inheritdoc/>
        public override Color DefaultConvertReturnValue { get; set; } = Colors.Transparent;

        /// <inheritdoc/>
        public override Color ConvertFrom(Color value, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(value);
            return value.ToBlackOrWhiteForText();
        }
    }
}
