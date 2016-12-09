using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CV.Mobile.Models;
using CV.Mobile.Services;
using FormsToolkit;
using CV.Mobile.Views;

namespace CV.Mobile.ViewModels
{
    public class ListagemAmigoViewModel : BaseNavigationViewModel
    {
        public ListagemAmigoViewModel()
        {
            Amigos = new ObservableCollection<ConsultaAmigo>();
            Requisicoes = new ObservableCollection<RequisicaoAmizade>();
            PageAppearingCommand = new Command(
                                                        async () =>
                                                        {
                                                            await CarregarListaAmigos();
                                                            await CarregarListaRequisicoes();
                                                        },
                                                        () => true);
            AtualizarAmigoCommand = new Command(async () => await CarregarListaAmigos(), () => true);
            MarcarSeguidorCommand = new Command<ConsultaAmigo>( (itemAmigo) =>  MarcarSeguidor(itemAmigo));
            MarcarSeguidoCommand = new Command<ConsultaAmigo>( (itemAmigo) =>  MarcarSeguido(itemAmigo));
            AtualizarRequisicaoCommand = new Command(async () => await CarregarListaRequisicoes(), () => true);
            ReprovarAmizadeCommand = new Command<RequisicaoAmizade>((itemAmigo) => ReprovarAmizade(itemAmigo));
            AprovarAmizadeCommand = new Command<RequisicaoAmizade>(async (itemAmigo) => await AprovarAmizade(itemAmigo));
            AdicionarAmigoCommand = new Command(async () => await AdicionarAmigo(), () => true);

            MessagingService.Current.Subscribe(MessageKeys.AdicionarAmigo, async (service) =>
            {
                IsBusy = true;

                await CarregarListaAmigos();

                IsBusy = false;
            });

        }

        private bool _IsLoadingAmigo = false;
        private bool _IsLoadingRequisicao = false;

        private ConsultaAmigo _AmigoSelecionado;
        private RequisicaoAmizade _RequisicaoSelecionada;

        public ObservableCollection<ConsultaAmigo> Amigos { get; set; }
        public ObservableCollection<RequisicaoAmizade> Requisicoes { get; set; }


        public Command PageAppearingCommand { get; set; }
        public Command AdicionarAmigoCommand { get; set; }
        public Command AtualizarAmigoCommand { get; set; }
        public Command MarcarSeguidorCommand { get; set; }
        public Command MarcarSeguidoCommand { get; set; }
        public Command AtualizarRequisicaoCommand { get; set; }
        public Command AprovarAmizadeCommand { get; set; }
        public Command ReprovarAmizadeCommand { get; set; }

        public bool IsLoadingAmigo
        {
            get
            {
                return _IsLoadingAmigo;
            }

            set
            {
                SetProperty(ref _IsLoadingAmigo, value);
            }
        }

        public ConsultaAmigo AmigoSelecionado
        {
            get
            {
                return _AmigoSelecionado;
            }

            set
            {
                _AmigoSelecionado = null;
                OnPropertyChanged("AmigoSelecionado");
            }
        }

        public bool IsLoadingRequisicao
        {
            get
            {
                return _IsLoadingRequisicao;
            }

            set
            {
                SetProperty(ref _IsLoadingRequisicao, value);
            }
        }

        public RequisicaoAmizade RequisicaoSelecionada
        {
            get
            {
                return _RequisicaoSelecionada;
            }

            set
            {
                _RequisicaoSelecionada = null;
                OnPropertyChanged("RequisicaoSelecionada");
            }
        }

        private async Task CarregarListaAmigos()
        {
            //IsLoadingAmigo = true;
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarAmigosConsulta();
                Amigos = new ObservableCollection<ConsultaAmigo>(Dados);
                OnPropertyChanged("Amigos");
            }
            IsLoadingAmigo = false;
        }

        private async Task AdicionarAmigo()
        {
            AdicionarAmigoPage pagina = new AdicionarAmigoPage() { BindingContext = new AdicionarAmigoViewModel() };
            await PushAsync(pagina);
        }

        private async Task CarregarListaRequisicoes()
        {
            IsLoadingRequisicao = true;
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarRequisicaoAmizade();
                Requisicoes = new ObservableCollection<RequisicaoAmizade>(Dados);
                OnPropertyChanged("Requisicoes");
            }
            IsLoadingRequisicao = false;
        }

        private  void MarcarSeguidor(ConsultaAmigo itemAmigo)
        {
            string Mensagem = itemAmigo.Seguidor?"Ao desmarcar essa pessoa como seguidora ela deixará de visualizar suas viagens. Deseja desmarcá-la?": "Ao marcar essa pessoa como seguidora ela poderá visualizar suas viagens. Deseja marcá-la?";
            itemAmigo.Acao = itemAmigo.Seguidor ? 2 : 1;
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = Mensagem,
                Positive = "Confirmar",
                Negative = "Cancelar",
                OnCompleted = new Action<bool>(async result => {
                    if (!result) return;
                    await GravarAlteracaoAmigo(itemAmigo);
                })
            });
        }

        private void MarcarSeguido(ConsultaAmigo itemAmigo)
        {
            string Mensagem = itemAmigo.Seguido ? "Ao desmarcar essa pessoa você deixará de acompanhar suas viagens. Deseja remover?" : "Ao marcar essa pessoa será enviada uma solicitação para você visualizar suas viagens. Deseja continuar?";
            itemAmigo.Acao = itemAmigo.Seguido ? 4 : 3;
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = Mensagem,
                Positive = "Confirmar",
                Negative = "Cancelar",
                OnCompleted = new Action<bool>(async result => {
                    if (!result) return;
                    await GravarAlteracaoAmigo(itemAmigo);
                })
            });
        }

        private async Task GravarAlteracaoAmigo(ConsultaAmigo itemAmigo)
        {
            using (ApiService srv = new ApiService())
            {
                var Resultado = await srv.RequisicaoAmizade(itemAmigo);
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                {
                    Title = "Sucesso",
                    Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                    Cancel = "OK"
                });
            }
            await CarregarListaAmigos();

        }

        private void ReprovarAmizade(RequisicaoAmizade itemAmigo)
        {
            string Mensagem = "Caso seja reprovada a requisição essa pessoa só poderá visualizar suas viagens se fizer uma nova solicitação. Deseja continuar?";
            itemAmigo.Status = 3;
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = Mensagem,
                Positive = "Confirmar",
                Negative = "Cancelar",
                OnCompleted = new Action<bool>(async result => {
                    if (!result) return;
                    using (ApiService srv = new ApiService())
                    {
                        var Resultado = await srv.SalvarRequisicaoAmizade(itemAmigo);
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                    }
                    await CarregarListaRequisicoes();
                })
            });
        }

        private async Task AprovarAmizade(RequisicaoAmizade itemAmigo)
        {
            itemAmigo.Status = 2;
            using (ApiService srv = new ApiService())
            {
                var Resultado = await srv.SalvarRequisicaoAmizade(itemAmigo);
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                {
                    Title = "Sucesso",
                    Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                    Cancel = "OK"
                });
            }
            await CarregarListaRequisicoes();
            await CarregarListaAmigos();
        }

    }
}
