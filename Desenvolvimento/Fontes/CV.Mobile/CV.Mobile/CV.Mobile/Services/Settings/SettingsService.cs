using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CV.Mobile.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        #region Setting Constants

       
        private const string IdModoSincronizacao = "ModoSincronizacao";
        private const string IdModoImagem = "ModoImagem";
        private const string IdModoVideo = "ModoVideo";
        private const string IdAcompanhamentoOnline = "AcompanhamentoOnline";
       
        private readonly string IdModoSincronizacaoDefault = "1";
        private readonly string IdModoImagemDefault = "1";
        private readonly string IdModoVideoDefault = "2";
        private readonly string IdAcompanhamentoOnlineDefault = "1";

        #endregion

        #region Settings Properties

      


        public string ModoSincronizacao
        {
            get => GetValueOrDefault(IdModoSincronizacao, IdModoSincronizacaoDefault);
            set => AddOrUpdateValue(IdModoSincronizacao, value);
        }

        public string ModoImagem
        {
            get => GetValueOrDefault(IdModoImagem, IdModoImagemDefault);
            set => AddOrUpdateValue(IdModoImagem, value);
        }


        public string ModoVideo
        {
            get => GetValueOrDefault(IdModoVideo, IdModoVideoDefault);
            set => AddOrUpdateValue(IdModoVideo, value);
        }

        public string AcompanhamentoOnline
        {
            get => GetValueOrDefault(IdAcompanhamentoOnline, IdAcompanhamentoOnlineDefault);
            set => AddOrUpdateValue(IdAcompanhamentoOnline, value);
        }

        #endregion

        #region Public Methods

        public Task AddOrUpdateValue(string key, bool value) => AddOrUpdateValueInternal(key, value);
        public Task AddOrUpdateValue(string key, string value) => AddOrUpdateValueInternal(key, value);
        public bool GetValueOrDefault(string key, bool defaultValue) => GetValueOrDefaultInternal(key, defaultValue);
        public string GetValueOrDefault(string key, string defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        #endregion

        #region Internal Implementation

        async Task AddOrUpdateValueInternal<T>(string key, T value)
        {
            if (value == null)
            {
                await Remove(key);
            }

            Application.Current.Properties[key] = value;
            try
            {
                await Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
            }
        }

        T GetValueOrDefaultInternal<T>(string key, T defaultValue = default(T))
        {
            object value = null;
            if (Application.Current.Properties.ContainsKey(key))
            {
                value = Application.Current.Properties[key];
            }
            return null != value ? (T)value : defaultValue;
        }

        async Task Remove(string key)
        {
            try
            {
                if (Application.Current.Properties[key] != null)
                {
                    Application.Current.Properties.Remove(key);
                    await Application.Current.SavePropertiesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to remove: " + key, " Message: " + ex.Message);
            }
        }

        #endregion
    }
}
