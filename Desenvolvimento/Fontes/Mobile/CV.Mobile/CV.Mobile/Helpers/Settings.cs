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
        private static readonly string CVBaseWebApiDefault = "http://192.168.0.188/CV/";

        private const string CVModoAtualizacaoKey = "CVModoAtualizacaoKey";
        private static readonly string CVModoAtualizacaoDefault = "N";

        private const string CVModoImagemKey = "CVModoImagemKey";
        private static readonly string CVModoImagemDefault = "N";

        private const string CVControleGPSKey = "CVControleGPSKey";
        private static readonly string CVControleGPSDefault = "A";

        private const string CVAcompanhamnentoOnlineKey = "CVAcompanhamnentoOnlineKey";
        private static readonly string CVAcompanhamnentoOnlineDefault = "S";

        private const string CVGeofencingHotelKey = "CVGeofencingHotelKey";
        private static readonly string CVGeofencingHotelDefault = "S";

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

        public static string ModoAtualizacao
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(CVModoAtualizacaoKey, CVModoAtualizacaoDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(CVModoAtualizacaoKey, value);
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

        public static string ControleGPS
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(CVControleGPSKey, CVControleGPSDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(CVControleGPSKey, value);
            }
        }

        public static string GeofencingHotel
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(CVGeofencingHotelKey, CVGeofencingHotelDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(CVGeofencingHotelKey, value);
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