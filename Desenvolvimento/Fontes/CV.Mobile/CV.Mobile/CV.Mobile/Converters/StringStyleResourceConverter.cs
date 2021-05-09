using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
	public class StringStyleResourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var someValue = (string)value; // Convert 'object' to whatever type you are expecting

			if (string.IsNullOrEmpty(someValue) || !App.Current.Resources.ContainsKey(someValue))
				return null;
	
			return (Style)App.Current.Resources[someValue];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
