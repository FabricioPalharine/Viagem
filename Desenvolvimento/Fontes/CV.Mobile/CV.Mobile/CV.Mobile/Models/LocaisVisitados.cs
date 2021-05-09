using CV.Mobile.Resources;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class LocaisVisitados
    {
        public string CodigoCoogle { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public int? IdentificadorCidade { get; set; }
        public string NomeCidade { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public bool Refeicao
        {
            get
            { return Tipo == "R"; }
        }
        public string TipoDescricao
        {
            get
            {
                return Tipo == "A" ? AppResource.Atracao : Tipo == "R" ? AppResource.Restaurante : Tipo == "H" ? AppResource.Hotel : AppResource.Loja;
            }
        }

        public ObservableCollection<LocaisVisitados> LocaisFilho { get; set; }
        public ObservableCollection<LocaisDetalhes> Detalhes { get; set; }

        public double? Media
        {
            get
            {
                if (Detalhes != null && Detalhes.Where(d => d.Nota.HasValue).Any())
                {
                    return Math.Round(Detalhes.Where(d => d.Nota.HasValue).Average(d => d.Nota.GetValueOrDefault()), 1);
                }
                else
                    return null;
            }
        }

        public ObservableCollection<Foto> Fotos { get; set; }
        public ObservableCollection<RelatorioGastos> Gastos { get; set; }
        public CriterioBusca itemBusca { get; set; }
        public string URLMapa
        {
            get
            {
                string url = null;
                if (Latitude.GetValueOrDefault(0) != 0 || Longitude.GetValueOrDefault(0) != 0)
                    url = "https://maps.googleapis.com/maps/api/staticmap?center=" + Latitude.GetValueOrDefault().ToString("F8", new System.Globalization.CultureInfo("en-US")) + "," + Longitude.GetValueOrDefault().ToString("F8", new System.Globalization.CultureInfo("en-US")) +
                  "&zoom=16&size=400x400&maptype=roadmap&markers=color:blue%7C" + Latitude.GetValueOrDefault().ToString("F8", new System.Globalization.CultureInfo("en-US")) + "," + Longitude.GetValueOrDefault().ToString("F8", new System.Globalization.CultureInfo("en-US")) + "&key=" + GlobalSetting.ClientAPI;
                return url;
            }
        }
        public bool TemPosicao
        {
            get
            {
                return Latitude.GetValueOrDefault(0) != 0 || Longitude.GetValueOrDefault(0) != 0;
            }
        }
    }
}
