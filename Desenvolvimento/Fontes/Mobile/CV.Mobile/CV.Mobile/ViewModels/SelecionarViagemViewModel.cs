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
        private CriterioBusca _itemCriterioBusca;
        public ObservableCollection<Viagem> Viagens { get; set; }

        public ICommand PageAppearingCommand { get; set; }
        public ICommand PesquisarCommand { get; set; }
        public ICommand SelecionarCommand { get; set; }

        private Viagem _ViagemSelecionada;

        public SelecionarViagemViewModel()
        {
            _itemCriterioBusca = new CriterioBusca() { Aberto = true };
            Viagens = new ObservableCollection<Viagem>();
            PageAppearingCommand = new Command(
                                                                    async () => await CarregarListaViagens(),
                                                                    () => true);
            SelecionarCommand = new Command<int>(async (Identificador) => await Selecionar(Identificador));
        }

        private async Task CarregarListaViagens()
        {
            using (ApiService srv = new ApiService())
            {
                var Dados = await srv.ListarViagens(_itemCriterioBusca);
                Viagens.Clear();
                foreach (var itemViagem in Dados)
                    Viagens.Add(itemViagem);
            }
        }

        private async Task Selecionar(int Identificador)
        { 
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
    }
}
