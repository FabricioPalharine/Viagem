using CV.Mobile.Models;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.Helpers
{
    public class ConsultaFotoTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AtracaoTemplate { get; set; }
        public DataTemplate HotelTemplate { get; set; }
        public DataTemplate RestauranteTemplate { get; set; }
        public DataTemplate CabecalhoTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item is Cabecalho ? CabecalhoTemplate : item is Atracao ? AtracaoTemplate : item is Hotel ? HotelTemplate : RestauranteTemplate;
        }
    }
}
