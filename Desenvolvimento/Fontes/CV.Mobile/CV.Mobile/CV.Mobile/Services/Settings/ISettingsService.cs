using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Services.Settings
{
    public interface ISettingsService
    {
       
        bool GetValueOrDefault(string key, bool defaultValue);
        string GetValueOrDefault(string key, string defaultValue);
        Task AddOrUpdateValue(string key, bool value);
        Task AddOrUpdateValue(string key, string value);

        string ModoSincronizacao { get; set; }
        string ModoImagem { get; set; }
        string ModoVideo { get; set; }
        string AcompanhamentoOnline { get; set; }

       
    }
}
