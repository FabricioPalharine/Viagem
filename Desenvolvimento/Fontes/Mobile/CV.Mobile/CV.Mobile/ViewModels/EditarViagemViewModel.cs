﻿using CV.Mobile.Helpers;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class EditarViagemViewModel : BaseNavigationViewModel
    {

        public ObservableCollection<Page> ListaPaginas { get; set; }

        public EditarViagemViewModel()
        {
            var ListaAtual = new List<Page>();
            var ItemUsuario = ((MasterDetailViewModel)Application.Current?.MainPage.BindingContext).ItemUsuario;
            _ViagemSelecionada = new Viagem() {IdentificadorUsuario = ItemUsuario.Codigo, PublicaGasto =false, DataInicio=DateTime.Today, DataFim = DateTime.Today, QuantidadeParticipantes=1, UnidadeMetrica=true, Participantes = new MvvmHelpers.ObservableRangeCollection<ParticipanteViagem>(), UsuariosGastos = new MvvmHelpers.ObservableRangeCollection<UsuarioGasto>() , Moeda=790};
            _ViagemSelecionada.Participantes.Add(new ParticipanteViagem() { IdentificadorUsuario = ItemUsuario.Codigo, NomeUsuario = ItemUsuario.Nome, ItemUsuario = new Usuario() { Identificador = ItemUsuario.Codigo, Nome = ItemUsuario.Nome }, PermiteExcluir = false });
            _ViagemSelecionada.PropertyChanged += _ViagemSelecionada_PropertyChanged;
            ListaAtual.Add(new EdicaoViagemDados());
            PageAppearingCommand = new Command(
                                                                    async () => await CarregarListaAmigos(),
                                                                    () => true);
            SelecionarCommand = new Command<ParticipanteViagem>( (Identificador) =>  ExcluirParticipante(Identificador));
            
            AdicionarCustoCommand = new Command(() => AdicionarCusto(), () => true);
            AdicionarParticipanteCommand = new Command(() => AdicionarParticipante(), () => true);
            ExcluirCustoCommand = new Command<UsuarioGasto>((Item) => ExcluirGasto(Item));

            SalvarCommand = new Command(
                                                        async () => await Salvar(),
                                                        () => true);

            ListaMoeda = new ObservableCollection<ItemLista>();
            List<ItemLista> lista = new List<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumMoeda)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumMoeda)enumerador).Descricao() };
                ListaMoeda.Add(item);
            }
            ListaMoeda = new ObservableCollection<ItemLista>( ListaMoeda.OrderBy(d => d.Descricao));
            ListaPaginas = new ObservableCollection<Page>(ListaAtual);

        }

        private void AdicionarParticipante()
        {
            if (ItemParticipante != null)
            {
                if (ItemViagem.Participantes.Where(d=>d.IdentificadorUsuario == ItemParticipante.Identificador).Any())
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Participante já Adicionado",
                        Message = "O Participante selecionado já está adicionado a Viagem",
                        Cancel = "OK"
                    });
                }
                else
                {
                    ItemViagem.Participantes.Add(new ParticipanteViagem() { IdentificadorUsuario = ItemParticipante.Identificador, NomeUsuario = ItemParticipante.Nome, PermiteExcluir = true, ItemUsuario = ItemParticipante });
                    if (ItemViagem.UsuariosGastos.Where(d=>d.IdentificadorUsuario == ItemParticipante.Identificador).Any())
                    {
                        ItemViagem.UsuariosGastos.Remove(ItemViagem.UsuariosGastos.Where(d => d.IdentificadorUsuario == ItemParticipante.Identificador).First());
                    }
                }
            }
        }

        private void AdicionarCusto()
        {
            if (ItemAmigo != null)
            {
                if (ItemViagem.UsuariosGastos.Where(d => d.IdentificadorUsuario == ItemAmigo.Identificador).Any())
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Amigo já Adicionado",
                        Message = "O Amigo selecionado já pode ver os gastos da viagem",
                        Cancel = "OK"
                    });
                }
                else if (ItemViagem.Participantes.Where(d => d.IdentificadorUsuario == ItemAmigo.Identificador).Any())
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Amigo Participante",
                        Message = "O Amigo não pode ser adicionado, pois ele é um participante da viagem",
                        Cancel = "OK"
                    });
                }
                else
                {
                    ItemViagem.UsuariosGastos.Add(new UsuarioGasto() { IdentificadorUsuario = ItemParticipante.Identificador, NomeUsuario = ItemParticipante.Nome, ItemUsuario = ItemParticipante });
                   
                }
            }
        }

        private void _ViagemSelecionada_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           if (e.PropertyName == "PublicaGasto")
            {
                var ListaAtual = ListaPaginas.ToList();
                if (!ItemViagem.PublicaGasto)
                    ListaAtual.Add(new EdicaoViagemDados());
                else if (ListaAtual.Any())
                    ListaAtual.RemoveAt(0);
                ListaPaginas = new ObservableCollection<Page>(ListaAtual);
                OnPropertyChanged("ListaPaginas");

            }
        }

        public ObservableCollection<Usuario> ListaAmigos { get; set; }
        public ObservableCollection<ItemLista> ListaMoeda { get; set; }

        private Usuario _ItemAmigo;
        private Usuario _ItemParticipante;
        private ParticipanteViagem _ParticipanteSelecionado;
        private UsuarioGasto _UsuarioGastoSelecionado;

        public Command PageAppearingCommand { get; set; }
        public Command PesquisarCommand { get; set; }
        public Command SelecionarCommand { get; set; }
        public Command ExcluirCustoCommand { get; set; }

        public Command AdicionarParticipanteCommand { get; set; }
        public Command AdicionarCustoCommand { get; set; }

        public Command SalvarCommand { get; set; }

        private Viagem _ViagemSelecionada;

        private bool _ModoPesquisa = false;

        private async Task CarregarListaAmigos()
        {
            IsBusy = true;
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarAmigos();
                ListaAmigos = new ObservableCollection<Usuario>(Dados);
                OnPropertyChanged("ListaAmigos");
            }
            IsBusy = false;
        }

        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                await AtualizarViagem(ItemViagem.Identificador);
                await PopAsync();
            }
            finally
            {
                SalvarCommand.ChangeCanExecute();
                IsBusy = false;
            }
        }


        private void ExcluirParticipante(ParticipanteViagem ItemParticipante)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = string.Format("Excluir Participante {0}?", ItemParticipante.NomeUsuario),
                Question = null,
                Positive = "Excluir",
                Negative = "Cancelar",
                OnCompleted = new Action<bool>( result => {
                    if (!result) return;

                    // send a message that we want the given acquaintance to be deleted
                    ItemViagem.Participantes.Remove(ItemParticipante);
                })
            });
        }


        private void ExcluirGasto(UsuarioGasto ItemParticipante)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = string.Format("Excluir Amigo {0}?", ItemParticipante.NomeUsuario),
                Question = null,
                Positive = "Excluir",
                Negative = "Cancelar",
                OnCompleted = new Action<bool>(result => {
                    if (!result) return;

                    // send a message that we want the given acquaintance to be deleted
                    ItemViagem.UsuariosGastos.Remove(ItemParticipante);
                })
            });
        }


        public Viagem ItemViagem
        {
            get
            {
                return _ViagemSelecionada;
            }

            set
            {

                SetProperty(ref _ViagemSelecionada, value);
            }
        }

     

        public Usuario ItemAmigo
        {
            get
            {
                return _ItemAmigo;
            }

            set
            {
                              _ItemAmigo = value;
            }
        }

        public Usuario ItemParticipante
        {
            get
            {
                return _ItemParticipante;
            }

            set
            {
                _ItemParticipante = value;
            }
        }

        public ParticipanteViagem ParticipanteSelecionado
        {
            get
            {
                return _ParticipanteSelecionado;
            }

            set
            {
                _ParticipanteSelecionado = null;
                OnPropertyChanged();
            }
        }

        public UsuarioGasto UsuarioGastoSelecionado
        {
            get
            {
                return _UsuarioGastoSelecionado;
            }

            set
            {
                _UsuarioGastoSelecionado = null;
                OnPropertyChanged();
            }
        }
    }
}
