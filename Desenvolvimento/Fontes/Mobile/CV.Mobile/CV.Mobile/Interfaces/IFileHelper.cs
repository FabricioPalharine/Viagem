using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Interfaces
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
        byte[] CarregarDadosFile(string filename);
        DateTime RetornarDataArquivo(string filename);

        Stream CarregarStreamFile(string filename);
        System.Globalization.NumberFormatInfo GetLocale();
    }
}
