using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
	public class SVGReplaceMapConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is Color color))
				return null;

			var ReplaceMap = new Dictionary<string, string>()
			{
				{"#000000", color.ToHex() }
			};
			return ReplaceMap;

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
