using CV.Mobile.Enums;
using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.Fotos;
using CV.Mobile.Services.Settings;
using CV.Mobile.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;


namespace CV.Mobile.ViewModels.Consultas
{
    public class LocaisDetalheViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private ObservableCollection<object> _dados = new ObservableCollection<object>();
        private LocaisVisitados localVisitado = null;
        private bool _verCustos = false;
        private readonly IFoto _foto;
        public LocaisDetalheViewModel(IApiService apiService, IFoto foto)
        {
            _apiService = apiService;
            _foto = foto;
        }
        public ObservableCollection<object> Dados
        {
            get { return _dados; }
            set { SetProperty(ref _dados, value); }
        }

        public bool VerCustos
        {
            get { return _verCustos; }
            set { SetProperty(ref _verCustos, value); }
        }

        public async override Task InitializeAsync(object navigationData)
        {
            if (navigationData != null && navigationData is LocaisVisitados item)
            {
                VerCustos = GlobalSetting.Instance.ViagemSelecionado.VejoGastos;
                IsBusy = true;
                try
                {
                    localVisitado = item;
                    localVisitado.itemBusca.Tipo = localVisitado.Tipo;
                    localVisitado.itemBusca.Comentario = localVisitado.CodigoCoogle;
                    localVisitado.itemBusca.Nome = localVisitado.Nome;
                    LocaisVisitados itemDetalhe = null;
                    if (item.Tipo == "A")
                        itemDetalhe = await _apiService.ConsultarDetalheAtracao(localVisitado.itemBusca);
                    else if (item.Tipo == "H")
                        itemDetalhe = await _apiService.ConsultarDetalheHotel(localVisitado.itemBusca);
                    else if (item.Tipo == "R")
                        itemDetalhe = await _apiService.ConsultarDetalheRestaurante(localVisitado.itemBusca);
                    if (itemDetalhe != null)
                    {
                        itemDetalhe.Nome = item.Nome;
                        itemDetalhe.Tipo = item.Tipo;
                        itemDetalhe.CodigoCoogle = item.CodigoCoogle;
                        itemDetalhe.Latitude = item.Latitude;
                        itemDetalhe.Longitude = item.Longitude;
                        Dados.Add(new LocalSelecionado() { ItemLocal = itemDetalhe });
                        if (itemDetalhe.Detalhes != null && itemDetalhe.Detalhes.Any())
                        {
                            Dados.Add(new Cabecalho() { Texto = "Visitas" });
                            foreach (var itemFilho in itemDetalhe.Detalhes)
                                Dados.Add(itemFilho);
                        }
                        if (itemDetalhe.Gastos != null && itemDetalhe.Gastos.Any())
                        {

                            Dados.Add(new Cabecalho() { Texto = "Gastos" });
                            foreach (var itemFilho in itemDetalhe.Gastos)
                            {
                                Dados.Add(itemFilho);

                            }
                        }
                        if (itemDetalhe.Fotos != null && itemDetalhe.Fotos.Any())
                        {
                            await _foto.UpdateMediaData(itemDetalhe.Fotos.ToList());
                            Dados.Add(itemDetalhe.Fotos);
                        }
                        if (itemDetalhe.LocaisFilho != null && itemDetalhe.LocaisFilho.Any())
                        {
                            Dados.Add(new Cabecalho() { Texto = "Locais Filho" });
                            foreach (var itemFilho in itemDetalhe.LocaisFilho)
                                Dados.Add(itemFilho);
                        }
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task AbrirMapa(LocalSelecionado item)
        {
            PosicaoMapa posicao = new PosicaoMapa() { Latitude = item.ItemLocal.Latitude.GetValueOrDefault(), Longitude = item.ItemLocal.Longitude.GetValueOrDefault() };
            await NavigationService.TrocarPaginaShell("MapaExibicaoPage", posicao);
        }

        public ICommand AbrirDetalheLocalCommand => new Command<LocaisVisitados>(async (d) =>
        {
            d.itemBusca = localVisitado.itemBusca.Clone();
            await NavigationService.TrocarPaginaShell("LocaisDetalhePage", d);
        });

        public ICommand AbrirMapaCommand => new Command<LocalSelecionado>(async (d) =>
        {
            await AbrirMapa(d);
        });


        public ICommand AbrirImagemCommand => new Command<Foto>(async (d) =>
        {
            await NavigationService.TrocarPaginaShell("ConsultaFotoDetalhePage", d);
        });

    }
}
