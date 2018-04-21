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

namespace CV.Mobile.ViewModels
{
    public class ConsultarResumoViewModel : BaseNavigationViewModel
    {
        private CriterioBusca _itemCriterioBusca;
        public bool VerGastos { get; set; }

        private ResumoViagem _ItemResumo;
        public ObservableRangeCollection<Usuario> ListaUsuario { get; set; }
        public ConsultarResumoViewModel()
        {
            ItemCriterioBusca = new CriterioBusca() { DataInicioDe = ItemViagemSelecionada.DataInicio };

            PesquisarCommand = new Command(
                                                                    async () =>
                                                                    {
                                                                        await CarregarListaDados();
                                                                        var Pagina = new ConsultarResumoDadosPage() { BindingContext = this };
                                                                        await PushAsync(Pagina);
                                                                    },
                                                                    () => true);




            VerGastos = ItemViagemSelecionada.VejoGastos;
        }

        public Command PageAppearingCommand
        {
            get
            {
                return new Command(
                                async () =>
                                {
                                    await CarregarListaUsuarios();
                                },
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


        public Command PesquisarCommand { get; set; }
        public Command AdicionarCommand { get; set; }


        public ResumoViagem ItemResumo
        {
            get
            {
                return _ItemResumo;
            }

            set
            {
                SetProperty(ref _ItemResumo, value);
            }
        }



        private async Task CarregarListaDados()
        {

            try
            {
                using (ApiService srv = new ApiService())
                {
                    ItemResumo = await srv.CarregarResumo(ItemCriterioBusca);
                }
            }
            catch
            {
                ApiService.ExibirMensagemErro();
            }


        }


    }
}
