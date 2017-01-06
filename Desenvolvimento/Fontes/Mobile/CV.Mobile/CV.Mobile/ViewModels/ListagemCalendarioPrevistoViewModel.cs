﻿using CV.Mobile.Models;
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
    public class ListagemCalendarioPrevistoViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;
        private CalendarioPrevisto _ItemSelecionado;
        private DateTime _DataCalendario;

        public ListagemCalendarioPrevistoViewModel(Viagem pitemViagem)
        {
            DataCalendario = DateTime.Today;
            ItemViagem = pitemViagem;
            ItemCriterioBusca = new CriterioBusca() { };
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {
                                                                       await Task.Delay(100);
                                                                       OnPropertyChanged("ModoLista");
                                                                       await CarregarListaDados();
                                                                   },
                                                                   () => true);
            PesquisarCommand = new Command(
                                                                     () => VerificarPesquisa(),
                                                                    () => true);
            AdicionarCommand = new Command(
                                                                   async () => await Adicionar(),
                                                                   () => true);
            RecarregarListaCommand = new Command(
                                                      async () =>
                                                      {
                                                          await CarregarListaDados();
                                                      },
                                                      () => true);

            ItemTappedCommand = new Command<ItemTappedEventArgs>(
                                                                  async (obj) => await VerificarAcaoItem(obj));
            AbrirAgendaCommand = new Command<int?>(
                                                      async (obj) => await VerificarAcaoItem(obj));

            MessagingService.Current.Unsubscribe<CalendarioPrevisto>(MessageKeys.ManutencaoCalendarioPrevisto);
            MessagingService.Current.Subscribe<CalendarioPrevisto>(MessageKeys.ManutencaoCalendarioPrevisto, (service, item) =>
            {
                IsBusy = true;

                if (ListaDados.Where(d => d.Identificador == item.Identificador).Any())
                {
                    var Posicao = ListaDados.IndexOf(ListaDados.Where(d => d.Identificador == item.Identificador).FirstOrDefault());
                    ListaDados.RemoveAt(Posicao);
                    if (!item.DataExclusao.HasValue)
                        ListaDados.Insert(Posicao, item);
                }
                else if (!item.DataExclusao.HasValue)
                    ListaDados.Add(item);

                IsBusy = false;
            });
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

        public Viagem ItemViagem { get; set; }
        public ObservableCollection<Cidade> ListaCidades { get; set; }

        public bool ModoPesquisa
        {
            get
            {
                return _ModoPesquisa;
            }

            set
            {
                SetProperty(ref _ModoPesquisa, value);
                OnPropertyChanged("ModoLista");
            }
        }

        public bool ModoLista
        {
            get
            {
                return !ModoPesquisa;
            }
        }
        public ObservableRangeCollection<CalendarioPrevisto> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command RecarregarListaCommand { get; set; }
        public Command PesquisarCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }
        public Command<int?> AbrirAgendaCommand { get; set; }
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

        public CalendarioPrevisto ItemSelecionado
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

        public DateTime DataCalendario
        {
            get
            {
                return _DataCalendario;
            }

            set
            {
                SetProperty(ref _DataCalendario, value);
            }
        }

        private void VerificarPesquisa()
        {

            ModoPesquisa = !ModoPesquisa;

        }



        private async Task CarregarListaDados()
        {
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarCalendarioPrevisto(ItemCriterioBusca);
                ListaDados = new ObservableRangeCollection<CalendarioPrevisto>(Dados);
                OnPropertyChanged("ListaDados");
            }
            IsLoadingLista = false;
        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            using (ApiService srv = new ApiService())
            {
                var ItemCalendarioPrevisto = await srv.CarregarCalendarioPrevisto(((CalendarioPrevisto)itemSelecionado.Item).Identificador);
                var Pagina = new EdicaoCalendarioPrevistoPage() { BindingContext = new EdicaoCalendarioPrevistoViewModel(ItemCalendarioPrevisto) };
                await PushAsync(Pagina);
            }
        }

        private async Task VerificarAcaoItem(int? itemSelecionado)
        {
            using (ApiService srv = new ApiService())
            {
                var ItemCalendarioPrevisto = await srv.CarregarCalendarioPrevisto(itemSelecionado);
                var Pagina = new EdicaoCalendarioPrevistoPage() { BindingContext = new EdicaoCalendarioPrevistoViewModel(ItemCalendarioPrevisto) };
                await PushAsync(Pagina);
            }
        }
        private async Task Adicionar()
        {
            var ItemCalendarioPrevisto = new CalendarioPrevisto() { DataInicio = DateTime.Today, HoraInicio = new TimeSpan(), DataFim = DateTime.Today, HoraFim = new TimeSpan(), Prioridade = 1, AvisarHorario=false };
            using (ApiService srv = new ApiService())
            {

                var Pagina = new EdicaoCalendarioPrevistoPage() { BindingContext = new EdicaoCalendarioPrevistoViewModel(ItemCalendarioPrevisto) };
                await PushAsync(Pagina);

            }


        }
    }
}