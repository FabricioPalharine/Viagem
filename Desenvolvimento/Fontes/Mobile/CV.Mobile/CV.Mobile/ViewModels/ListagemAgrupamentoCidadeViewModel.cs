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
    public class ListagemAgrupamentoCidadeViewModel : BaseNavigationViewModel
    {
        public Viagem ItemViagem { get; set; }
        public ListagemAgrupamentoCidadeViewModel(Viagem pItemViagem)
        {
            ItemViagem = pItemViagem;
            ListaDados = new ObservableCollection<Cidade>();
            PageAppearingCommand = new Command(
                                                       async () =>
                                                       {
                                                           await CarregarListaListaDados();
                                                       },
                                                       () => true);
            RecarregarListaCommand = new Command(
                                                       async () =>
                                                       {
                                                           await CarregarListaListaDados();
                                                       },
                                                       () => true);

            ExcluirCommand = new Command<Cidade>((item) => Excluir(item));
            EditarCommand = new Command<Cidade>(async (item) => await Editar(item));
            AdicionarCommand = new Command(async () => await AbrirInclusao(), () => true);
            MessagingService.Current.Unsubscribe<Cidade>(MessageKeys.ManutencaoAgrupamentoCidade);
            MessagingService.Current.Subscribe<Cidade>(MessageKeys.ManutencaoAgrupamentoCidade, (service, cotacao) =>
           {
               IsBusy = true;

               if (!ListaDados.Where(d => d.Identificador == cotacao.Identificador).Any())
               {

                   ListaDados.Add(cotacao);
               }

               IsBusy = false;
           });
        }

       

        private bool _IsLoadingLista;
        private Cidade _ItemSelecionado;


        public ObservableCollection<Cidade> ListaDados { get; set; }
        public Command AdicionarCommand { get; set; }
        public Command EditarCommand { get; set; }
        public Command ExcluirCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command RecarregarListaCommand { get; set; }

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

        public Cidade ItemSelecionado
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


        private async Task CarregarListaListaDados()
        {
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarCidadePai();
                ListaDados = new ObservableCollection<Cidade>(Dados);
                OnPropertyChanged("ListaDados");
            }
            IsLoadingLista = false;
        }

        private void Excluir(Cidade item)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Excluir grupo de Cidades {0}", item.Nome),
                Positive = "Confirmar",
                Negative = "Cancelar",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    using (ApiService srv = new ApiService())
                    {
                        var Resultado = await srv.ExcluirCidadeGrupo(item.Identificador);
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        if (ListaDados.Where(d => d.Identificador == item.Identificador).Any())
                        {
                            var Posicao = ListaDados.IndexOf(ListaDados.Where(d => d.Identificador == item.Identificador).FirstOrDefault());
                            ListaDados.RemoveAt(Posicao);
                        }
                    }

                })
            });
        }

        private async Task Editar(Cidade item)
        {
            using (ApiService srv = new ApiService())
            {
                var itemEditar = await srv.CarregarCidadeGrupo(item.Identificador);
                await AbirTela(itemEditar);
                //EdicaoCotacaoPage pagina = new EdicaoCotacaoPage() { BindingContext = new EditarCotacaoViewModel(itemEditar) };
                //await PushAsync(pagina);
            }
        }

        private async Task AbrirInclusao()
        {
            var itemEditar = new ManutencaoCidadeGrupo() {  Edicao=false, CidadesFilhas = new ObservableCollection<int?>(), IdentificadorCidade=null};
            await AbirTela(itemEditar);

        }

        private async Task AbirTela(ManutencaoCidadeGrupo itemManutencao)
        {
            
            using (ApiService srv = new ApiService())
            {
                var ListaCidadesPai = await srv.ListarCidadeNaoAssociadasFilho();
                var ListaCidadesFilha = await srv.ListarCidadeNaoAssociadasPai(itemManutencao.IdentificadorCidade.GetValueOrDefault(-1));
                var vm = new EdicaoAgrupamentoCidadeViewModel(itemManutencao, ListaCidadesPai, ListaCidadesFilha);
                var pagina = new EdicaoAgrupamentoCidadePage() { BindingContext = vm };
                await PushAsync(pagina);
            }
        }
    }
}
