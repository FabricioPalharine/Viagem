using System;
using CV.Mobile.Droid.Services;
using CV.Mobile.Services.PlatformSpecifcs;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(FileHelper))]
namespace CV.Mobile.Droid.Services
{
    public class FileHelper: IFileHelper
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

        public FileInfo RetornarFile(string filename)
        {
            return new FileInfo(filename);
        }
    }
}