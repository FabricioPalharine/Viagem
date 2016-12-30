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
            EditarCommand = new Command<CotacaoMoeda>(async (itemCotacao) => await Editar(itemCotacao));
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
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarCotacaoMoeda();
                Cotacoes = new ObservableCollection<CotacaoMoeda>(Dados);
                OnPropertyChanged("Cotacoes");
            }
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
                    using (ApiService srv = new ApiService())
                    {
                        var Resultado = await srv.ExcluirCotacaoMoeda(itemCotacao.Identificador);
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
                    }

                })
            });
        }

        private async Task Editar(CotacaoMoeda itemCotacao)
        {
            using (ApiService srv = new ApiService())
            {
                var itemEditar = await srv.CarregarCotacaoMoeda(itemCotacao.Identificador);
                EdicaoCotacaoPage pagina = new EdicaoCotacaoPage() { BindingContext = new EditarCotacaoViewModel(itemEditar) };
                await PushAsync(pagina);
            }
        }

        private async Task AbrirInclusao()
        {
            var itemEditar = new CotacaoMoeda() { DataCotacao = DateTime.Today, IdentificadorViagem = ItemViagem.Identificador, Moeda = ItemViagem.Moeda , ValorCotacao=0};
            EdicaoCotacaoPage pagina = new EdicaoCotacaoPage() { BindingContext = new EditarCotacaoViewModel(itemEditar) };
            await PushAsync(pagina);

        }
    }
}
