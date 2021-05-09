using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CV.Mobile.Services.PlatformSpecifcs
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
        byte[] CarregarDadosFile(string filename);
        FileInfo RetornarFile(string filename);
        DateTime RetornarDataArquivo(string filename);

        Stream CarregarStreamFile(string filename);
    }
}
