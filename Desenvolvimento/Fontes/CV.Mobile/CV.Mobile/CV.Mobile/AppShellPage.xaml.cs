using CV.Mobile.ViewModels;
using CV.Mobile.Views;
using CV.Mobile.Views.Amigos;
using CV.Mobile.Views.Atracoes;
using CV.Mobile.Views.Comentarios;
using CV.Mobile.Views.Consultas;
using CV.Mobile.Views.Deslocamentos;
using CV.Mobile.Views.Gastos;
using CV.Mobile.Views.Hoteis;
using CV.Mobile.Views.Mapas;
using CV.Mobile.Views.Moedas;
using CV.Mobile.Views.Refeicoes;
using CV.Mobile.Views.Viagens;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace CV.Mobile
{
    public partial class AppShellPage : Xamarin.Forms.Shell
    {
        public AppShellPage()
        {

            InitializeComponent();
            Routing.RegisterRoute(nameof(ConfiguracaoPage), typeof(ConfiguracaoPage));
            Routing.RegisterRoute(nameof(AmigosPage), typeof(AmigosPage));
            Routing.RegisterRoute(nameof(AmigoAdicaoPage), typeof(AmigoAdicaoPage));
            Routing.RegisterRoute(nameof(ViagemListaPage), typeof(ViagemListaPage));
            Routing.RegisterRoute(nameof(ViagemFiltroPage), typeof(ViagemFiltroPage));
            Routing.RegisterRoute(nameof(MoedaCotacaoEdicaoPage), typeof(MoedaCotacaoEdicaoPage));
            Routing.RegisterRoute(nameof(MoedaCotacaoFiltroPage), typeof(MoedaCotacaoFiltroPage));
            Routing.RegisterRoute(nameof(MoedaFiltroPage), typeof(MoedaFiltroPage));
            Routing.RegisterRoute(nameof(MoedaEdicaoPage), typeof(MoedaEdicaoPage));
            Routing.RegisterRoute(nameof(ComentarioEdicaoPage), typeof(ComentarioEdicaoPage));
            Routing.RegisterRoute(nameof(ComentarioFiltroPage), typeof(ComentarioFiltroPage));
            Routing.RegisterRoute(nameof(MapaSelecaoPage), typeof(MapaSelecaoPage));
            Routing.RegisterRoute(nameof(MapaExibicaoPage), typeof(MapaExibicaoPage));
            Routing.RegisterRoute(nameof(GastoEdicaoPage), typeof(GastoEdicaoPage));
            Routing.RegisterRoute(nameof(GastoFiltroPage), typeof(GastoFiltroPage));
            Routing.RegisterRoute(nameof(GastoSelecaoPage), typeof(GastoSelecaoPage));
            Routing.RegisterRoute(nameof(AtracaoEdicaoPage), typeof(AtracaoEdicaoPage));
            Routing.RegisterRoute(nameof(AtracaoFiltroPage), typeof(AtracaoFiltroPage));
            Routing.RegisterRoute(nameof(HotelEdicaoPage), typeof(HotelEdicaoPage));
            Routing.RegisterRoute(nameof(HotelFiltroPage), typeof(HotelFiltroPage));
            Routing.RegisterRoute(nameof(DeslocamentoEdicaoPage), typeof(DeslocamentoEdicaoPage));
            Routing.RegisterRoute(nameof(DeslocamentoFiltroPage), typeof(DeslocamentoFiltroPage));
            Routing.RegisterRoute(nameof(RefeicaoEdicaoPage), typeof(RefeicaoEdicaoPage));
            Routing.RegisterRoute(nameof(RefeicaoFiltroPage), typeof(RefeicaoFiltroPage));
            Routing.RegisterRoute(nameof(SincronizacaoPage), typeof(SincronizacaoPage));
            Routing.RegisterRoute(nameof(ExtratoDinheiroFiltroPage), typeof(ExtratoDinheiroFiltroPage));
            Routing.RegisterRoute(nameof(AcertoContaFiltroPage), typeof(AcertoContaFiltroPage));
            Routing.RegisterRoute(nameof(RelatorioGastoFiltroPage), typeof(RelatorioGastoFiltroPage));
            Routing.RegisterRoute(nameof(CalendarioRealizadoFiltroPage), typeof(CalendarioRealizadoFiltroPage));
            Routing.RegisterRoute(nameof(GaleriaFotoFiltroPage), typeof(GaleriaFotoFiltroPage));
            Routing.RegisterRoute(nameof(LocaisFiltroPage), typeof(LocaisFiltroPage));
            Routing.RegisterRoute(nameof(ResumoFiltroPage), typeof(ResumoFiltroPage));
            Routing.RegisterRoute(nameof(TimelineFiltroPage), typeof(TimelineFiltroPage));
            Routing.RegisterRoute(nameof(LocaisDetalhePage), typeof(LocaisDetalhePage));
            Routing.RegisterRoute(nameof(ConsultaFotoDetalhePage), typeof(ConsultaFotoDetalhePage));
            Routing.RegisterRoute(nameof(ConsultaMapaFiltroPage), typeof(ConsultaMapaFiltroPage));

            Routing.RegisterRoute(nameof(ComentarioFotoPage), typeof(ComentarioFotoPage));
            Routing.RegisterRoute(nameof(ViagemCriacaoPage), typeof(ViagemCriacaoPage));
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

        }

       
    }
}
