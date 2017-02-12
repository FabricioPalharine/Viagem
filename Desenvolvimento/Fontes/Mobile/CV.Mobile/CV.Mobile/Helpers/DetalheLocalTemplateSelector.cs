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
    public class DetalheLocalTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CabecalhoTemplate { get; set; }
        public DataTemplate DetalheLocalTemplate { get; set; }
        public DataTemplate FotoTemplate { get; set; }
        public DataTemplate RelatorioGastoTemplate { get; set; }
        public DataTemplate LocaisFilhoTemplate { get; set; }
        public DataTemplate ItemLocalTemplate { get; set; }
        public DataTemplate ItemCompraTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item is Cabecalho ? CabecalhoTemplate : item is LocaisDetalhes ? DetalheLocalTemplate : item is ObservableRangeCollection<Foto> ? FotoTemplate
                : item is RelatorioGastos ? RelatorioGastoTemplate : item is LocaisVisitados ? LocaisFilhoTemplate : item is LojaItens ? ItemCompraTemplate : ItemLocalTemplate;
        }
    }
}
