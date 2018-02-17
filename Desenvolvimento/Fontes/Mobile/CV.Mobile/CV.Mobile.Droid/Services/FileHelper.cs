using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;

using Android.Runtime;
using Android.Views;
using Android.Widget;
using CV.Mobile.Interfaces;
using System.IO;

namespace CV.Mobile.Droid.Services
{
    public class FileHelper : IFileHelper
    {
        public byte[] CarregarDadosFile(string filename)
        {
            byte[] retorno = null;
            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        retorno = ms.ToArray();
                    }
                }
            }

            return retorno;
        }

        public string GetLocalFilePath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }

        public DateTime RetornarDataArquivo(string filename)
        {
            return File.GetLastWriteTime(filename);
        }

        public Stream CarregarStreamFile(string filename)
        {
            return new FileStream(filename, FileMode.Open, FileAccess.Read);
        }

        public System.Globalization.NumberFormatInfo GetLocale()
        {
            return System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
        }
    }
}