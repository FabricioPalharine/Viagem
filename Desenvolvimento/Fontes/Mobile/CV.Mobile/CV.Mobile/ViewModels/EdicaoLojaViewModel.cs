using CV.Mobile.Helpers;
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

    public class EdicaoLojaViewModel : BaseNavigationViewModel
    {
        private Loja _ItemLoja;
        private MapSpan _Bounds;
        private bool _PermiteExcluir = true;
        private bool _PossoComentar = false;

        private AvaliacaoLoja _ItemAvaliacao = new AvaliacaoLoja();
        private readonly DateTime _dataMinima = new DateTime(1900, 01, 01);
        public EdicaoLojaViewModel(Loja pItemLoja, Viagem pItemViagem)
        {
            if (pItemLoja.IdentificadorAtracao == null)
                pItemLoja.IdentificadorAtracao = 0;

            ItemLoja = pItemLoja;

            PossoComentar = true;
            ItemViagem = pItemViagem;


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
            AbrirCustosCommand = new Command(async () => await AbrirJanelaCustos());

        }
        public Command SalvarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command AbrirCustosCommand { get; set; }
        public Command<GmsSearchResults> PlaceSelectedCommand
        {
            get
            {
                return new Command<GmsSearchResults>(p =>
                {
                    ItemLoja.Nome = p.name;
                    ItemLoja.CodigoPlace = p.place_id;

                });
            }
        }

        public Viagem ItemViagem { get; set; }

        public async Task CarregarPosicao()
        {
            await Task.Delay(100);
            if (ItemLoja.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).Any())
                ItemAvaliacao = ItemLoja.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).FirstOrDefault();
            PermiteExcluir = ItemLoja.Id.HasValue || ItemLoja.Identificador.HasValue;
            CarregarAtracoesPai();
            if (_ItemLoja.Latitude.HasValue && _ItemLoja.Longitude.HasValue)
            {
                Bounds = MapSpan.FromCenterAndRadius(new Position(_ItemLoja.Latitude.Value, _ItemLoja.Longitude.Value), new Distance(5000));


            }
            else
            {
                var posicao = await RetornarPosicao();
                if (posicao == null)
                    posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = 0, Longitude = 0 };
                Bounds = MapSpan.FromCenterAndRadius(new Position(posicao.Latitude, posicao.Longitude), new Distance(5000));
                ItemLoja.Longitude = posicao.Longitude;
                ItemLoja.Latitude = posicao.Latitude;
            }



        }

        public async void CarregarAtracoesPai()
        {
            List<Atracao> ListaDados = new List<Atracao>();
            bool Executado = false;
            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        ListaDados = await srv.ListarAtracao(new CriterioBusca());

                    }
                    Executado = true;
                }
                catch { Executado = false; }
            }
            if (!Executado)
                ListaDados = await DatabaseService.Database.ListarAtracao(new CriterioBusca());
            ListaDados.Insert(0, new Atracao() { Identificador = 0, Nome = "Sem Atração Pai" });
            ListaAtracaoPai = new ObservableCollection<Atracao>(ListaDados);
            OnPropertyChanged("ListaAtracaoPai");
            int? IdentificadorLojaSelecionada = ItemLoja.IdentificadorAtracao;
            ItemLoja.IdentificadorAtracao = int.MinValue;
            await Task.Delay(100);
            ItemLoja.IdentificadorAtracao = IdentificadorLojaSelecionada.GetValueOrDefault(0);

        }


        public Loja ItemLoja
        {
            get
            {
                return _ItemLoja;
            }

            set
            {
                SetProperty(ref _ItemLoja, value);
            }
        }

        public ObservableCollection<Atracao> ListaAtracaoPai { get; set; }

        public MapSpan Bounds
        {
            get
            {
                return _Bounds;
            }

            set
            {
                SetProperty(ref _Bounds, value);
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

        public bool PossoComentar
        {
            get
            {
                return _PossoComentar;
            }

            set
            {
                SetProperty(ref _PossoComentar, value);
            }
        }



        public AvaliacaoLoja ItemAvaliacao
        {
            get
            {
                return _ItemAvaliacao;
            }

            set
            {
                SetProperty(ref _ItemAvaliacao, value);
            }
        }



        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                ItemAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                var itemAvaliacaoAtual = ItemLoja.Avaliacoes.Where(d => d.IdentificadorUsuario == ItemUsuarioLogado.Codigo).FirstOrDefault();

                if (ItemAvaliacao.Nota.HasValue || !string.IsNullOrWhiteSpace(ItemAvaliacao.Comentario))
                {
                    if (itemAvaliacaoAtual == null)
                    {
                        var itemNovaAvaliacao = new AvaliacaoLoja() { IdentificadorUsuario = ItemUsuarioLogado.Codigo };

                        itemNovaAvaliacao.Nota = ItemAvaliacao.Nota;
                        itemNovaAvaliacao.Comentario = ItemAvaliacao.Comentario;
                        itemNovaAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                        ItemLoja.Avaliacoes.Add(itemNovaAvaliacao);
                    }
                }
                else
                {
                    if (itemAvaliacaoAtual != null)
                    {
                        ItemLoja.Avaliacoes.Remove(itemAvaliacaoAtual);
                    }
                }

                if (ItemLoja.IdentificadorAtracao == 0)
                    ItemLoja.IdentificadorAtracao = null;

                ResultadoOperacao Resultado = new ResultadoOperacao();
                Loja pItemLoja = null;
                bool Executado = false;
                if (Conectado)
                {
                    try
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.SalvarLoja(ItemLoja);
                            if (Resultado.Sucesso)
                            {
                                pItemLoja = await srv.CarregarLoja(Resultado.IdentificadorRegistro);
                                AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "L", Resultado.IdentificadorRegistro.GetValueOrDefault(), !ItemLoja.Identificador.HasValue);
                                await DatabaseService.SalvarLojaReplicada(pItemLoja);
                            }
                        }
                        Executado = true;
                    }
                    catch { Executado = false; }
                }
                if (!Executado)
                {
                    Resultado = await DatabaseService.SalvarLoja(ItemLoja);
                    if (Resultado.Sucesso)
                        pItemLoja = await DatabaseService.CarregarLoja(ItemLoja.Identificador);
                }


                if (Resultado.Sucesso)
                {

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    ItemLoja.Identificador = Resultado.IdentificadorRegistro;
                    // var Jresultado = (JObject)Resultado.ItemRegistro;
                    // Loja pItemLoja = Jresultado.ToObject<Loja>();

                    if (pItemLoja.IdentificadorAtracao == null)
                        pItemLoja.IdentificadorAtracao = 0;

                    ItemLoja = pItemLoja;
                    MessagingService.Current.SendMessage<Loja>(MessageKeys.ManutencaoLoja, ItemLoja);
                    PermiteExcluir = true;
                }
                else if (Resultado.Mensagens != null && Resultado.Mensagens.Any())
                {
                    if (ItemLoja.IdentificadorAtracao == null)
                        ItemLoja.IdentificadorAtracao = 0;
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
                Question = String.Format("Deseja excluir essa Loja e todas as compras nela realizada?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;

                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    ItemLoja.DataExclusao = DateTime.Now.ToUniversalTime();
                    bool Executado = false;
                    if (Conectado)
                    {
                        try
                        {
                            using (ApiService srv = new ApiService())
                            {
                                Resultado = await srv.ExcluirLoja(ItemLoja.Identificador);
                                await DatabaseService.ExcluirLoja(ItemLoja.Identificador, true);
                                AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "L", ItemLoja.Identificador.GetValueOrDefault(), false);
                            }
                            Executado = true;
                        }
                        catch { Executado = false; }
                    }
                    if (!Executado)
                    {
                        await DatabaseService.ExcluirLoja(ItemLoja.Identificador, false);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Loja Excluída com Sucesso " } };

                    }


                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });

                    MessagingService.Current.SendMessage<Loja>(MessageKeys.ManutencaoLoja, ItemLoja);
                    await PopAsync();

                })
            });


        }

        private async Task AbrirJanelaCustos()
        {
            var Pagina = new ListagemCompraCustoPage() { BindingContext = new ListagemCompraCustoViewModel(ItemViagem, ItemLoja) };
            await PushAsync(Pagina);
        }
    }
}
