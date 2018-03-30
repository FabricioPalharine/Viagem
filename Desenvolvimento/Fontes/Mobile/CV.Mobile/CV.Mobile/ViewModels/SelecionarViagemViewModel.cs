using CV.Mobile.Models;
using CV.Mobile.Services;
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
    public class SelecionarViagemViewModel: BaseNavigationViewModel
    {
        public SelecionarViagemViewModel()
        {
            _itemCriterioBusca = new CriterioBusca() { Aberto = true};
            Viagens = new ObservableCollection<Viagem>();
            PageAppearingCommand = new Command(
                                                                    async () =>
                                                                    {
                                                                        await CarregarListaAmigos();
                                                                        await CarregarListaViagens();
                                                                    },
                                                                    () => true);
            SelecionarCommand = new Command<ItemTappedEventArgs>(async (Identificador) => await Selecionar(Identificador));
            PesquisarCommand = new Command(
                                                                    async () => await VerificarPesquisa(),
                                                                    () => true);
            AtualizarListaCommand = new Command(
                                                        async () => await CarregarListaViagens(),
                                                        () => true);

            ListaSituacao = new ObservableCollection<ItemLista>();
            ListaSituacao.Add(new ItemLista() { Codigo="1",Descricao="Aberta" });
            ListaSituacao.Add(new ItemLista() { Codigo = "2", Descricao = "Fechada" });
            ListaSituacao.Add(new ItemLista() { Codigo = "3", Descricao = "Todas" });
            ItemSituacao = ListaSituacao[0];
        }
        private ItemLista _ItemSituacao;

        private CriterioBusca _itemCriterioBusca;
        public ObservableCollection<Viagem> Viagens { get; set; }
        public ObservableCollection<Usuario> ListaAmigos { get; set; }
        public ObservableCollection<ItemLista> ListaSituacao { get; set; }

        private Usuario _ItemAmigo;
        public Command PageAppearingCommand { get; set; }
        public Command PesquisarCommand { get; set; }
        public Command SelecionarCommand { get; set; }
        public Command AtualizarListaCommand { get; set; }

        private Viagem _ViagemSelecionada;

        private bool _ModoPesquisa = false;

        private async Task CarregarListaViagens()
        {
            IsBusy = true;
            if (AtualizarListaCommand.CanExecute(null))
                AtualizarListaCommand.ChangeCanExecute();
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarViagens(_itemCriterioBusca);
                Viagens.Clear();
                foreach (var itemViagem in Dados)
                    Viagens.Add(itemViagem);
            }
            AtualizarListaCommand.ChangeCanExecute();
            IsBusy = false;
        }

        private async Task VerificarPesquisa()
        {
            if (ModoPesquisa)
            {
                if (PesquisarCommand.CanExecute(null))
                    PesquisarCommand.ChangeCanExecute();
                await CarregarListaViagens();
                PesquisarCommand.ChangeCanExecute();
            }
            ModoPesquisa = !ModoPesquisa;
           
        }

        private async Task CarregarListaAmigos()
        {
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarAmigosComigo();
                ListaAmigos = new ObservableCollection<Usuario>(Dados);
                OnPropertyChanged("ListaAmigos");
            }
        }

        private async Task Selecionar(ItemTappedEventArgs Item)
        {
            await PararGPS();
            await AtualizarViagem(((Viagem)Item.Item).Identificador.GetValueOrDefault());
            await PopAsync();
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

        public Viagem ViagemSelecionada
        {
            get
            {
                return _ViagemSelecionada;
            }

            set
            {
                _ViagemSelecionada = null;
                OnPropertyChanged("ViagemSelecionada");
            }
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
                if (value != null)
                    ItemCriterioBusca.IdentificadorParticipante = value.Identificador;
                else
                    ItemCriterioBusca.IdentificadorParticipante = null;
                _ItemAmigo = value;
            }
        }

        public ItemLista ItemSituacao
        {
            get
            {
                return _ItemSituacao;
            }

            set
            {
                if (value != null)
                {
                    if (value.Codigo == "3")
                        ItemCriterioBusca.Aberto = null;
                    else
                        ItemCriterioBusca.Aberto = value.Codigo == "1";
                }
                else
                    ItemCriterioBusca.Aberto = null;
                    _ItemSituacao = value;
            }
        }
    }
}
