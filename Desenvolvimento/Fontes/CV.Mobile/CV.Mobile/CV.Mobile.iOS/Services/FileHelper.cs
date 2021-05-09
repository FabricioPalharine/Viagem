using CoreGraphics;
using CV.Mobile.iOS.Services;
using CV.Mobile.Services.PlatformSpecifcs;
using Foundation;
using System;
using System.Drawing;
using UIKit;
using Xamarin.Forms;
using System.IO;

[assembly: Dependency(typeof(FileHelper))]

namespace CV.Mobile.iOS.Services
{
    public class FileHelper : IFileHelper
    {
        public FileHelper()
        {
            
        }
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

        public DateTime RetornarDataArquivo(string filename)
        {
            return File.GetLastWriteTime(filename);
        }

        public string GetLocalFilePath(string filename)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return Path.Combine(libFolder, filename);
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