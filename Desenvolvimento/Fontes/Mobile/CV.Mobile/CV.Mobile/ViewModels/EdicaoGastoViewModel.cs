﻿using CV.Mobile.Helpers;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.ViewModels
{

    public class EdicaoGastoViewModel : BaseNavigationViewModel
    {
        private Gasto _ItemGasto;
        private bool _PermiteExcluir = true;
        private bool _PagamentoMECartao = false;
        private bool _ExibeHora = false;
        private bool _VoltarPagina = false;
        private Usuario _ParticipanteSelecionado;
        private double _TamanhoGrid;
        public EdicaoGastoViewModel(Gasto pItemGasto)
        {

            ItemGasto = pItemGasto;

            Participantes = new ObservableCollection<Usuario>();
            PagamentoMECartao = !pItemGasto.Especie && pItemGasto.Moeda != (int)enumMoeda.BRL;
            ItemGasto.PropertyChanged += ItemGasto_PropertyChanged;

            SalvarCommand = new Command(
                                async () => await Salvar(),
                                () => !IsBusy);
            PageAppearingCommand = new Command(
                                                                  async () =>
                                                                  {
                                                                      await CarregarPosicao();
                                                                  },
                                                                  () => true);

            ExcluirCommand = new Command(() => Excluir());
            ListaMoeda = new ObservableCollection<ItemLista>();
            List<ItemLista> lista = new List<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumMoeda)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumMoeda)enumerador).Descricao() };
                ListaMoeda.Add(item);
            }
            ListaMoeda = new ObservableCollection<ItemLista>(ListaMoeda.OrderBy(d => d.Descricao));
        }

        private void ItemGasto_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Especie" || e.PropertyName == "Moeda")
                PagamentoMECartao = !ItemGasto.Especie && ItemGasto.Moeda != (int)enumMoeda.BRL;

        }

        public Command SalvarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command AbrirCustosCommand { get; set; }

        public ObservableCollection<ItemLista> ListaMoeda { get; set; }

        public ObservableCollection<Usuario> Participantes { get; set; }

        public async Task CarregarPosicao()
        {
            await Task.Delay(100);
            PermiteExcluir = ItemGasto.Identificador.HasValue;
            await CarregarParticipantesViagem();


            if (ItemGasto.Latitude.HasValue && ItemGasto.Longitude.HasValue)
            {

            }
            else
            {
                var posicao = await RetornarPosicao();
                if (posicao == null)
                    posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = 0, Longitude = 0 };
                ItemGasto.Longitude = posicao.Longitude;
                ItemGasto.Latitude = posicao.Latitude;
            }
        }

        private async Task CarregarParticipantesViagem()
        {
            if (!Participantes.Any())
            {
                Participantes.Clear();
                List<Usuario> ListaUsuario = new List<Usuario>();
                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {

                        ListaUsuario = await srv.ListarParticipantesViagem();
                        if (!ListaUsuario.Any())
                            ListaUsuario = await DatabaseService.Database.ListarParticipanteViagem();
                    }
                }
                else
                {
                    ListaUsuario = await DatabaseService.Database.ListarParticipanteViagem();
                }
                foreach (var itemUsuario in ListaUsuario)
                {
                    if (!ItemGasto.Identificador.HasValue || ItemGasto.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                        itemUsuario.Selecionado = true;
                    Participantes.Add(itemUsuario);
                }
                TamanhoGrid = Participantes.Count() * 24;

            }
        }


        public Gasto ItemGasto
        {
            get
            {
                return _ItemGasto;
            }

            set
            {
                SetProperty(ref _ItemGasto, value);
            }
        }



        public bool PermiteExcluir
        {
            get
            {
                return _PermiteExcluir;
            }

            set
            {
                SetProperty(ref _PermiteExcluir, value);
            }
        }

        public bool PagamentoMECartao
        {
            get
            {
                return _PagamentoMECartao;
            }

            set
            {
                SetProperty(ref _PagamentoMECartao, value);
            }
        }


        public Usuario ParticipanteSelecionado
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

        public bool ExibeHora
        {
            get
            {
                return _ExibeHora;
            }

            set
            {
                _ExibeHora = value;
            }
        }

        public bool VoltarPagina
        {
            get
            {
                return _VoltarPagina;
            }

            set
            {
                _VoltarPagina = value;
            }
        }

        public double TamanhoGrid
        {
            get
            {
                return _TamanhoGrid;
            }

            set
            {
                SetProperty(ref _TamanhoGrid, value);
            }
        }

        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                if (ItemGasto.Dividido)
                {
                    foreach (Usuario itemUsuario in Participantes)
                    {
                        if (itemUsuario.Selecionado)
                        {
                            if (!ItemGasto.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                            {
                                var itemNovaAvaliacao = new GastoDividido() { IdentificadorUsuario = itemUsuario.Identificador };
                                ItemGasto.Usuarios.Add(itemNovaAvaliacao);
                            }
                        }
                        else
                        {
                            if (ItemGasto.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).Any())
                            {
                                ItemGasto.Usuarios.Remove(ItemGasto.Usuarios.Where(d => d.IdentificadorUsuario == itemUsuario.Identificador).FirstOrDefault());
                            }
                        }
                    }
                }
                else
                    ItemGasto.Usuarios.Clear();
                if (ExibeHora)
                {
                    ItemGasto.Data = DateTime.SpecifyKind(ItemGasto.Data.GetValueOrDefault().Date.Add(ItemGasto.Hora.GetValueOrDefault()), DateTimeKind.Unspecified);
                }
                ResultadoOperacao Resultado = null;
                Gasto pItemGasto = null;
                bool Executado = false;
                if (Conectado)
                {
                    try
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.SalvarGasto(ItemGasto);
                            if (Resultado.Sucesso)
                            {
                                var Jresultado = (JObject)Resultado.ItemRegistro;
                                pItemGasto = Jresultado.ToObject<Gasto>();
                                AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "G", pItemGasto.Identificador.GetValueOrDefault(), !ItemGasto.Identificador.HasValue);
                                DatabaseService.SalvarGastoSincronizado(pItemGasto);
                            }

                        }
                        Executado = true;
                    }
                    catch { Executado = false; }
                }
                if (!Executado)
                {
                    if (!ItemGasto.IdentificadorUsuario.HasValue)
                        ItemGasto.IdentificadorUsuario = ItemUsuarioLogado.Codigo;
                    Resultado = await DatabaseService.SalvarGasto(ItemGasto);
                    pItemGasto = ItemGasto;
                }
                if (Resultado.Sucesso)
                {
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    //ItemGasto.Identificador = Resultado.IdentificadorRegistro;


                    ItemGasto = pItemGasto;
                    MessagingService.Current.SendMessage<Gasto>(MessageKeys.ManutencaoGasto, ItemGasto);
                    PermiteExcluir = true;
                    if (VoltarPagina)
                    {
                        await PopAsync();
                        MessagingService.Current.SendMessage<Gasto>(MessageKeys.GastoIncluido, ItemGasto);
                    }
                }
                else if (Resultado.Mensagens != null && Resultado.Mensagens.Any())
                {

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Problemas Validação",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });

                }


            }
            finally
            {
                IsBusy = false;

                SalvarCommand.ChangeCanExecute();
            }
        }

        private void Excluir()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir esse Gasto?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    ItemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                    bool Executado = false;
                    if (Conectado)
                    {
                        try
                        {
                            using (ApiService srv = new ApiService())
                            {
                                Resultado = await srv.ExcluirGasto(ItemGasto.Identificador);
                                AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "G", ItemGasto.Identificador.GetValueOrDefault(), false);
                                var itemBase = await DatabaseService.CarregarGasto(ItemGasto.Identificador);
                                if (itemBase != null)
                                    await DatabaseService.ExcluirGasto(itemBase, true);
                            }
                            Executado = true;
                        }
                        catch { Executado = false; }
                    }
                    if (!Executado)
                    {
                        await DatabaseService.ExcluirGasto(ItemGasto, false);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Gasto excluído com sucesso " } };

                    }


                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    MessagingService.Current.SendMessage<Gasto>(MessageKeys.ManutencaoGasto, ItemGasto);
                    await PopAsync();

                })
            });


        }


    }
}
