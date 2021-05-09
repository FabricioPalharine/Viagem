using CV.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace CV.Mobile.Selector
{
    public class DetalheLocalTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CabecalhoTemplate { get; set; }
        public DataTemplate DetalheLocalTemplate { get; set; }
        public DataTemplate FotoTemplate { get; set; }
        public DataTemplate RelatorioGastoTemplate { get; set; }
        public DataTemplate LocaisFilhoTemplate { get; set; }
        public DataTemplate ItemLocalTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item is Cabecalho ? CabecalhoTemplate : item is LocaisDetalhes ? DetalheLocalTemplate : item is ObservableCollection<Foto> ? FotoTemplate
                : item is RelatorioGastos ? RelatorioGastoTemplate : item is LocaisVisitados ? LocaisFilhoTemplate : ItemLocalTemplate;
        }
    }
}
