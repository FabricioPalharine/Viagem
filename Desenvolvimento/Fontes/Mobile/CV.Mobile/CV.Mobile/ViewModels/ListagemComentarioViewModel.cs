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
    public class ListagemComentarioViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;
        private Comentario _ItemSelecionado;


        public ListagemComentarioViewModel(Viagem pitemViagem)
        {
            ItemViagem = pitemViagem;
            ItemCriterioBusca = new CriterioBusca() { };
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {
                                                                       await CarregarListaCidades();
                                                                       await CarregarListaDados();
                                                                   },
                                                                   () => true);
            PesquisarCommand = new Command(
                                                                    async () => await VerificarPesquisa(),
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

            MessagingService.Current.Unsubscribe<Comentario>(MessageKeys.ManutencaoComentario);
            MessagingService.Current.Subscribe<Comentario>(MessageKeys.ManutencaoComentario, (service, item) =>
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
            }
        }


        public ObservableCollection<Comentario> ListaDados { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command RecarregarListaCommand { get; set; }
        public Command PesquisarCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command AdicionarCommand { get; set; }

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

        public Comentario ItemSelecionado
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


        private async Task VerificarPesquisa()
        {
            if (ModoPesquisa)
            {
                if (PesquisarCommand.CanExecute(null))
                    PesquisarCommand.ChangeCanExecute();
                await CarregarListaDados();
                PesquisarCommand.ChangeCanExecute();
            }
            ModoPesquisa = !ModoPesquisa;

        }

        private async Task CarregarListaCidades()
        {
            List<Cidade> Dados = new List<Cidade>();
            bool Executado = false;
            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        Dados = await srv.ListarCidadeComentario();

                    }
                    Executado = true;
                }
                catch { Executado = false; }
            }
            if (!Executado)
            {
                Dados = await DatabaseService.Database.ListarCidade_Tipo("CO");
            }

            ListaCidades = new ObservableCollection<Cidade>(Dados);
            OnPropertyChanged("ListaCidades");
        }


        private async Task CarregarListaDados()
        {
            List<Comentario> Dados = new List<Comentario>();
            bool Executado = false;
            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        Dados = await srv.ListarComentario(ItemCriterioBusca);

                    }
                    Executado = true;
                }
                catch { Executado = false; }
            }
            if (!Executado)
            {
                Dados = await DatabaseService.Database.ListarComentario(ItemCriterioBusca);
            }

            ListaDados = new ObservableCollection<Comentario>(Dados);
            OnPropertyChanged("ListaDados");
            IsLoadingLista = false;
        }

        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            Comentario ItemComentario = ((Comentario)itemSelecionado.Item).Clone();

            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        ItemComentario = await srv.CarregarComentario(((Comentario)itemSelecionado.Item).Identificador);

                    }
                }
                catch
                {

                }
            }
            var Pagina = new EdicaoComentarioPage() { BindingContext = new EdicaoComentarioViewModel(ItemComentario, ItemViagem) };
            await PushAsync(Pagina);
        }
        private async Task Adicionar()
        {
            var ItemComentario = new Comentario() { Data = DateTime.Now, Hora = DateTime.Now.TimeOfDay };

            var Pagina = new EdicaoComentarioPage() { BindingContext = new EdicaoComentarioViewModel(ItemComentario, ItemViagem) };
            await PushAsync(Pagina);


        }

    }
}
