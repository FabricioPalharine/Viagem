using CV.Mobile.Enums;
using CV.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using Xamarin.Essentials;

namespace CV.Mobile.Helper
{
    public class Funcoes
    {
        public static bool AcessoInternet
        {
            get
            {
                return Connectivity.NetworkAccess == NetworkAccess.Internet;
            }
        }

        public static bool AcessoRede
        {
            get
            {
                return Connectivity.ConnectionProfiles.Where(d => d == ConnectionProfile.WiFi || d == ConnectionProfile.Ethernet ).Any();
            }
        }

        public static ObservableCollection<ItemLista> RetornarMoedas()
        {
            var ListaMoeda = new ObservableCollection<ItemLista>();
            List<ItemLista> lista = new List<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumMoeda)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumMoeda)enumerador).Descricao() };
                ListaMoeda.Add(item);
            }
            ListaMoeda = new ObservableCollection<ItemLista>(ListaMoeda.OrderBy(d => d.Descricao));
            string[] MoedasPrincipais = new string[] { "165", "745", "785", "540", "706", "978", "220", "790" };
            foreach (var codigoMoeda in MoedasPrincipais)
            {
                var itemMoeda = ListaMoeda.Where(d => d.Codigo == codigoMoeda).FirstOrDefault();
                if (itemMoeda != null)
                {
                    ListaMoeda.Remove(itemMoeda);
                    ListaMoeda.Insert(0, itemMoeda);
                }
            }
            return ListaMoeda;
        }

        public static ObservableCollection<ItemLista> RetornarTiposDeslocamento()
        {
            var Lista = new ObservableCollection<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumTipoTransporte)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumTipoTransporte)enumerador).Descricao() };
                Lista.Add(item);
            }
         
            return Lista;
        }

        internal static bool VerificarConsultaInternet(string acompanhamentoOnline)
        {
            return (acompanhamentoOnline == "1" && AcessoInternet)
                ||  (acompanhamentoOnline == "2" && AcessoInternet && AcessoRede);

        }
    }

    public static class EnumHelperExtension
    {
        public static string Descricao(this Enum enumValue)
        {
            ResourceManager _resources = new ResourceManager("CV.Mobile.Resources.EnumDescription", typeof(enumMoeda).GetTypeInfo().Assembly);


            string rk = String.Format("{0}_{1}", enumValue.GetType().Name, enumValue);
            string localizedDescription = _resources.GetString(rk);

            if (localizedDescription == null)
            {
                return enumValue.ToString();

            }
            else
                return localizedDescription;



        }
    }
}
