// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace CV.Mobile.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string CVBaseWebApiKey = "CVBaseWebApiKey";
#if !DEBUG
        private static readonly string CVBaseWebApiDefault = "http://sites.architettura.com.br:5131/";
       // private static readonly string CVBaseWebApiDefault = "http://curtindoviaggem.azurewebsites.net/";

#else
        private static readonly string CVBaseWebApiDefault = "http://192.168.15.11/CV/";
        //private static readonly string CVBaseWebApiDefault = "http://fabriciopalharine.sytes.net:40020/";
#endif
        private const string CVModoSincronizacaoKey = "CVModoSincronizacaoKey";
        private static readonly string CVModoSincronizacaoDefault = "1";

        private const string CVModoImagemKey = "CVModoImagemKey";
        private static readonly string CVModoImagemDefault = "1";

        private const string CVModoVideoKey = "CVModoVideoKey";
        private static readonly string CVModoVideoDefault = "2";

        private const string CVAcompanhamnentoOnlineKey = "CVAcompanhamnentoOnlineKey";
        private static readonly string CVAcompanhamnentoOnlineDefault = "1";

       

#endregion


        public static string BaseWebApi
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(CVBaseWebApiKey, CVBaseWebApiDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(CVBaseWebApiKey, value);
            }
        }

        public static string ModoSincronizacao
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(CVModoSincronizacaoKey, CVModoSincronizacaoDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(CVModoSincronizacaoKey, value);
            }
        }

        public static string ModoImagem
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(CVModoImagemKey, CVModoImagemDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(CVModoImagemKey, value);
            }
        }

        public static string ModoVideo
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(CVModoVideoKey, CVModoVideoDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(CVModoVideoKey, value);
            }
        }

       

        public static string AcompanhamentoOnline
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(CVAcompanhamnentoOnlineKey, CVAcompanhamnentoOnlineDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(CVAcompanhamnentoOnlineKey, value);
            }
        }

    }
}