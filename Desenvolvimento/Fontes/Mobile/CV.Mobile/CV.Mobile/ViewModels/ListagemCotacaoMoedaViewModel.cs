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
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class ListagemCotacaoMoedaViewModel : BaseNavigationViewModel
    {
        public Viagem ItemViagem { get; set; }
        public ListagemCotacaoMoedaViewModel(Viagem pItemViagem)
        {
            ItemViagem = pItemViagem;
            Cotacoes = new ObservableCollection<CotacaoMoeda>();
            PageAppearingCommand = new Command(
                                                       async () =>
                                                       {
                                                           await CarregarListaCotacoes();
                                                       },
                                                       () => true);
            RecarregarListaCommand = new Command(
                                                       async () =>
                                                       {
                                                           await CarregarListaCotacoes();
                                                       },
                                                       () => true);

            ExcluirCommand = new Command<CotacaoMoeda>((itemCotacao) => Excluir(itemCotacao));
            EditarCommand = new Command<ItemTappedEventArgs>(async (itemCotacao) => await Editar(itemCotacao));
            AdicionarCommand = new Command(async () => await AbrirInclusao(), () => true);
            MessagingService.Current.Unsubscribe<CotacaoMoeda>(MessageKeys.ManutencaoCotacaoMoeda);
            MessagingService.Current.Subscribe<CotacaoMoeda>(MessageKeys.ManutencaoCotacaoMoeda, (service, cotacao) =>
           {
               IsBusy = true;

               if (Cotacoes.Where(d => d.Identificador == cotacao.Identificador).Any())
               {
                   var Posicao = Cotacoes.IndexOf(Cotacoes.Where(d => d.Identificador == cotacao.Identificador).FirstOrDefault());
                   Cotacoes.RemoveAt(Posicao);
                   Cotacoes.Insert(Posicao, cotacao);
               }
               else
                   Cotacoes.Add(cotacao);

               IsBusy = false;
           });
        }



        private bool _IsLoadingCotacao;
        private CotacaoMoeda _CotacaoSelecionada;


        public ObservableCollection<CotacaoMoeda> Cotacoes { get; set; }
        public Command AdicionarCommand { get; set; }
        public Command EditarCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command RecarregarListaCommand { get; set; }

        public bool IsLoadingCotacao
        {
            get
            {
                return _IsLoadingCotacao;
            }

            set
            {
                SetProperty(ref _IsLoadingCotacao, value);
            }
        }

        public CotacaoMoeda CotacaoSelecionada
        {
            get
            {
                return _CotacaoSelecionada;
            }

            set
            {
                _CotacaoSelecionada = null;
                OnPropertyChanged();
            }
        }


        private async Task CarregarListaCotacoes()
        {
            List<CotacaoMoeda> Dados = new List<CotacaoMoeda>();
            bool Executado = false;
            if (Conectado)
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        Dados = await srv.ListarCotacaoMoeda();

                    }
                    Executado = true;
                }
                catch { Executado = false; }
            }
            if (!Executado)
                Dados = await DatabaseService.Database.ListarCotacaoMoeda(new CriterioBusca());
            Cotacoes = new ObservableCollection<CotacaoMoeda>(Dados);
            OnPropertyChanged("Cotacoes");
            IsLoadingCotacao = false;
        }

        private void Excluir(CotacaoMoeda itemCotacao)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Excluir cotação de {0} para {1}", itemCotacao.SiglaMoeda, itemCotacao.DataCotacao.GetValueOrDefault().ToString("dd/MM/yyyy")),
                Positive = "Confirmar",
                Negative = "Cancelar",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    bool Executado = false;
                    if (Conectado)
                    {
                        try
                        {
                            using (ApiService srv = new ApiService())
                            {
                                Resultado = await srv.ExcluirCotacaoMoeda(itemCotacao.Identificador);
                                base.AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "CM", itemCotacao.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()), false);

                                var itemAjustar = await DatabaseService.Database.RetornarCotacaoMoeda(itemCotacao.Identificador);
                                if (itemAjustar != null)
                                    await DatabaseService.Database.ExcluirCotacaoMoeda(itemAjustar);


                            }
                            Executado = true;
                        }
                        catch { Executado = false; }
                    }
                    if (!Executado)
                    {
                        if (itemCotacao.Identificador > 0)
                        {
                            itemCotacao.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemCotacao.AtualizadoBanco = false;
                            await DatabaseService.Database.SalvarCotacaoMoeda(itemCotacao);
                        }
                        else
                        {
                            await DatabaseService.Database.ExcluirCotacaoMoeda(itemCotacao);

                        }
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Cotação de Moeda excluida com sucesso" } };


                    }

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    if (Cotacoes.Where(d => d.Identificador == itemCotacao.Identificador).Any())
                    {
                        var Posicao = Cotacoes.IndexOf(Cotacoes.Where(d => d.Identificador == itemCotacao.Identificador).FirstOrDefault());
                        Cotacoes.RemoveAt(Posicao);
                    }

                })
            });
        }

        private async Task Editar(ItemTappedEventArgs itemCotacao)
        {

            var itemEditar = ((CotacaoMoeda)itemCotacao.Item).Clone();
            EdicaoCotacaoPage pagina = new EdicaoCotacaoPage() { BindingContext = new EditarCotacaoViewModel(itemEditar) };
            await PushAsync(pagina);

        }

        private async Task AbrirInclusao()
        {
            var itemEditar = new CotacaoMoeda() { DataCotacao = DateTime.Today, IdentificadorViagem = ItemViagem.Identificador, Moeda = ItemViagem.Moeda, ValorCotacao = 0 };
            EdicaoCotacaoPage pagina = new EdicaoCotacaoPage() { BindingContext = new EditarCotacaoViewModel(itemEditar) };
            await PushAsync(pagina);

        }
    }
}
