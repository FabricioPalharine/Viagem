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
using System.ComponentModel;
using System.Collections.Specialized;

namespace CV.Mobile.ViewModels
{
    public class ListagemCalendarioRealizadoViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        private bool _ModoPesquisa = false;

        private bool _IsLoadingLista;
        private CalendarioPrevisto _ItemSelecionado;
        private DateTime _Data;
        private int _Posicao;
        private bool _PrimeiroLoad = true;
        public ObservableRangeCollection<Usuario> ListaUsuario { get; set; }
        public ObservableCollection<DataCalendario> DatasCalendario { get; set; }
        public ListagemCalendarioRealizadoViewModel()
        {
            _Posicao = 1;
            _Data = DateTime.Today;
            ItemCriterioBusca = new CriterioBusca() { };
            PageAppearingCommand = new Command(
                                                                   async () =>
                                                                   {

                                                                       await CarregarListaUsuarios();
                                                                   },
                                                                   () => true);
            PesquisarCommand = new Command(
                                                                   async  () =>
                                                                     {
                                                                         await CarregarListaDados();
                                                                         if (ListaDados.Any())
                                                                             Data = ListaDados.Min(d => d.DataInicio).GetValueOrDefault().Date;
                                                                         var Pagina = new ListagemCalendarioRealizadoPage() { BindingContext = this };
                                                                         await PushAsync(Pagina);
                                                                     },
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


            DatasCalendario = new ObservableCollection<Models.DataCalendario>(new DataCalendario[] { new DataCalendario() { Data = DateTime.Today.AddDays(-1) }, new DataCalendario() { Data = DateTime.Today }, new DataCalendario() { Data = DateTime.Today.AddDays(1) } });

          }

        public Command TrocarExibicaoCommand
        {
            get
            {
                return new Command(
                                                                     () => VerificarPesquisa(),
                                                                    () => true);
            }
        }

        private async Task CarregarListaUsuarios()
        {
            if (ListaUsuario == null)
            {
                using (ApiService srv = new ApiService())
                {
                    ListaUsuario = new ObservableRangeCollection<Usuario>(await srv.CarregarParticipantesAmigo());
                    OnPropertyChanged("ListaUsuario");

                    if (ListaUsuario.Where(d => d.Identificador == ItemUsuarioLogado.Codigo).Any())
                        ItemCriterioBusca.IdentificadorParticipante = ItemUsuarioLogado.Codigo;
                    else
                        ItemCriterioBusca.IdentificadorParticipante = ListaUsuario.Select(d => d.Identificador).FirstOrDefault();
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

        public ObservableCollection<Cidade> ListaCidades { get; set; }
        private bool AlterandoPosicao = false;
        public Command<SelectedPositionChangedEventArgs> TrocarPosicaoCommand
        {
            get
            {
                return new Command<SelectedPositionChangedEventArgs>(async (obj) =>
                {
                    int _posicao = (int)obj.SelectedPosition;
                    await AjustarPosicaoCalendario(_posicao);

                });
            }
        }

        private async Task AjustarPosicaoCalendario( int _posicao)
        {
            AlterandoPosicao = true;
            var item = DatasCalendario[_posicao];
            Data = item.Data;
            await Task.Delay(200);
            if (_posicao == 0)
                DatasCalendario.Insert(0, new DataCalendario() { Data = Data.AddDays(-1) });
            else if (_posicao == DatasCalendario.Count - 1)
                DatasCalendario.Add(new DataCalendario() { Data = Data.AddDays(+1) });

            AlterandoPosicao = false;
        }

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

        public DateTime Data
        {
            get
            {
                return _Data;
            }

            set
            {
                bool Prosseguir = (_Data != value) && !AlterandoPosicao;
                
                 SetProperty(ref _Data, value);
                if (Prosseguir)
                {
                    DatasCalendario = new ObservableCollection<Models.DataCalendario>(new DataCalendario[] { new DataCalendario() { Data = _Data.AddDays(-1) }, new DataCalendario() { Data = _Data }, new DataCalendario() { Data = _Data.AddDays(1) } });
                    OnPropertyChanged("DatasCalendario");
                    Posicao = 1;
                }
            }
        }

        public int Posicao
        {
            get
            {
                return _Posicao;
            }

            set
            {
                SetProperty(ref _Posicao, value);
                if (value == 0 && _PrimeiroLoad)
                {
                    AjustarPosicaoCalendario(value);
                    _PrimeiroLoad = false;
                }
            }
        }

        private void VerificarPesquisa()
        {

            ModoPesquisa = !ModoPesquisa;

        }



        private async Task CarregarListaDados()
        {
            List<CalendarioRealizado> Dados = new List<CalendarioRealizado>();

                using (ApiService srv = new ApiService())
                {
                    Dados = await srv.ConsultarCalendarioRealizado(ItemCriterioBusca);

                }
          
            ListaDados = new ObservableRangeCollection<CalendarioPrevisto>(Dados.Select(d=>new CalendarioPrevisto() { Nome = d.Titulo, DataInicio = d.DataInicio, DataFim = d.DataFim }));
            OnPropertyChanged("ListaDados");
            IsLoadingLista = false;
        }

     

         private async Task Adicionar()
        {
            var ItemCalendarioPrevisto = new CalendarioPrevisto() { DataInicio = DateTime.Today, HoraInicio = new TimeSpan(), DataFim = DateTime.Today, HoraFim = new TimeSpan(), Prioridade = 1, AvisarHorario=false };
            

                var Pagina = new EdicaoCalendarioPrevistoPage() { BindingContext = new EdicaoCalendarioPrevistoViewModel(ItemCalendarioPrevisto) };
                await PushAsync(Pagina);

            


        }
    }
}
