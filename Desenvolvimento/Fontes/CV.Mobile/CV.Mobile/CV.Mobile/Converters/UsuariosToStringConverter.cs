using CV.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace CV.Mobile.Converters
{
	public class UsuariosToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ICollection<ParticipanteViagem> usuarios = (ICollection<ParticipanteViagem>)value;
			string textoUsuario = string.Join(", ", usuarios.Select(d => d.ItemUsuario.Nome));
			return textoUsuario;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
