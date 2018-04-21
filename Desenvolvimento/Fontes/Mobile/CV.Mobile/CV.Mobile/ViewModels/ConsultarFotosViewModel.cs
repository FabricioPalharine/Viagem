using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using CV.Mobile.Helpers;
using FormsToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MvvmHelpers;

namespace CV.Mobile.ViewModels
{
    public class ConsultarFotosViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;

        private bool _IsLoadingLista;
        private object _ItemSelecionado;
        public ObservableRangeCollection<object> ListaFiltro { get; set; }
        public ConsultarFotosViewModel()
        {
            ItemCriterioBusca = new CriterioBusca() { Count = 500000, Index=0 };
           
            PesquisarCommand = new Command(
                                                                    async () => { await CarregarListaDados();
                                                                        var Pagina = new ConsultarFotoListaPage() { BindingContext = this };
                                                                        await PushAsync(Pagina);
                                                                        },
                                                                    () => true);
           
      

         
        }

        public Command PageAppearingCommand
        {
            get
            {
                return new Command(
                                async () =>
                                {
                                    await CarregarListaFiltro();
                                },
                                () => true);
            }
        }

        private async Task CarregarListaFiltro()
        {
           if (ListaFiltro == null)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        ListaFiltro = new ObservableRangeCollection<object>();
                        var ListaAtracao = await srv.CarregarFotoAtracao();
                        if (ListaAtracao.Any())
                        {
                            ListaFiltro.Add(new Cabecalho() { Texto = "Atrações" });
                            foreach (var item in ListaAtracao)
                                ListaFiltro.Add(item);
                        }
                        var ListaRefeicao = await srv.CarregarFotoRefeicao();
                        if (ListaRefeicao.Any())
                        {
                            ListaFiltro.Add(new Cabecalho() { Texto = "Refeições" });
                            foreach (var item in ListaRefeicao)
                                ListaFiltro.Add(item);
                        }
                        var ListaHotel = await srv.CarregarFotoHotel();
                        if (ListaHotel.Any())
                        {
                            ListaFiltro.Add(new Cabecalho() { Texto = "Hotéis" });
                            foreach (var item in ListaHotel)
                                ListaFiltro.Add(item);
                        }
                        OnPropertyChanged("ListaFiltro");
                    }
                }
                catch
                {
                    ApiService.ExibirMensagemErro();
                }
            }
        }

        public CriterioBusca ItemCriterioBusca
        {
            get
            {
                return _itemCriterioBusca;
            }

            set
            {
                SetProperty(ref _itemCriterioBusca, value);
            }
        }


        public ObservableCollection<Foto> ListaDados { get; set; }
        public Command PesquisarCommand { get; set; }

        public bool IsLoadingLista
        {
            get
            {
                return _IsLoadingLista;
            }

            set
            {
                SetProperty(ref _IsLoadingLista, value);
            }
        }

        public object ItemSelecionado
        {
            get
            {
                return _ItemSelecionado;
            }

            set
            {
                _ItemSelecionado = null;
                OnPropertyChanged();
            }
        }




        private async Task CarregarListaDados()
        {
            List<Foto> Dados = new List<Foto>();
            ItemCriterioBusca.ListaAtracoes = ListaFiltro.OfType<Atracao>().Where(d => d.Selecionado).Select(d => d.Identificador).ToList();
            ItemCriterioBusca.ListaHoteis = ListaFiltro.OfType<Hotel>().Where(d => d.Selecionado).Select(d => d.Identificador).ToList();

            ItemCriterioBusca.ListaRefeicoes = ListaFiltro.OfType<Refeicao>().Where(d => d.Selecionado).Select(d => d.Identificador).ToList();
            try
            {
                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ListarFoto(ItemCriterioBusca);
                }
            }
            catch
            {
                ApiService.ExibirMensagemErro();
            }
            ListaDados = new ObservableRangeCollection<Foto>(Dados);
            OnPropertyChanged("ListaDados");

            IsLoadingLista = false;
        }

  
    }
}
