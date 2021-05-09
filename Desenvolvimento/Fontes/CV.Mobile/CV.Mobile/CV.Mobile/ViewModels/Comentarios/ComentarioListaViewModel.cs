using CV.Mobile.Enums;
using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels.Comentarios
{
    public class ComentarioListaViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private ObservableCollection<Comentario> _comentarios = new ObservableCollection<Comentario>();
        private readonly ISettingsService _settingsService;
        public ComentarioListaViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
        }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            MessagingCenter.Unsubscribe<ComentarioFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarComentario);
            MessagingCenter.Subscribe<ComentarioFiltroViewModel, CriterioBusca>(this, MessageKeys.FiltrarComentario,  (sender, obj) =>
            {
                criterioBusca.Moeda = obj.Moeda;
                criterioBusca.DataInicioAte = obj.DataInicioAte;
                criterioBusca.DataInicioDe = obj.DataInicioDe;

            });
            MessagingCenter.Unsubscribe<ComentarioEdicaoViewModel, Comentario>(this, MessageKeys.SalvarComentario);
            MessagingCenter.Subscribe<ComentarioEdicaoViewModel, Comentario>(this, MessageKeys.SalvarComentario,  (sender, obj) =>
            {
                //await CarregarLista();

            });
            await CarregarLista();
        });

        public ICommand RecarregarListaCommand => new Command(async () =>
        {
            await CarregarLista();

        });



        public ICommand FiltrarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("ComentarioFiltroPage", criterioBusca);

        });

        public ICommand EditarCommand => new Command<Comentario>(async (d) =>
        {

            await NavigationService.TrocarPaginaShell("ComentarioEdicaoPage", d);

        });

        public ICommand IncluirCommand => new Command(async () =>
        {

            await NavigationService.TrocarPaginaShell("ComentarioEdicaoPage", null);

        });

        public ICommand ExcluirCommand => new Command<Comentario>(async (d) =>
        {
            if (await DialogService.ShowConfirmAsync(AppResource.ExcluirComentario,
                    AppResource.Confirmacao,
                    AppResource.Confirmar, AppResource.Cancelar))
            {
                await Excluir(d);
                Comentarios.Remove(d);
            }

        }, (d) => !IsBusy);

        private async Task Excluir(Comentario d)
        {
            IsBusy = true;
            try
            {
                ResultadoOperacao resultado = new ResultadoOperacao();
                bool atualizadoBanco = false;

                if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline) && d.Identificador > 0)
                {
                    resultado = await _apiService.ExcluirComentario(d.Identificador);
                    if (resultado != null)
                    {
                        atualizadoBanco = true;
                        if (resultado.Sucesso)
                        {
                            var itemAjustar = await _database.RetornarComentario(d.Identificador);
                            if (itemAjustar != null)
                                await _database.ExcluirComentario(itemAjustar);
                        }
                    }
                }
                if (!atualizadoBanco)
                {

                    if (d.Identificador > 0)
                    {
                        var Comentario = await _database.RetornarComentario(d.Identificador);
                        Comentario.DataExclusao = DateTime.UtcNow;
                        Comentario.AtualizadoBanco = false;
                        await _database.SalvarComentario(Comentario);
                    }
                    else
                        await _database.ExcluirComentario(d);
                    resultado = new ResultadoOperacao()
                    {
                        Sucesso = true,
                        Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = AppResource.ExclusaoSucesso } }
                    };

                }
                if (resultado != null)
                    await base.ExibirResultado(resultado);

            }
            finally
            {
                IsBusy = false;
            }
        }

        public ObservableCollection<Comentario> Comentarios
        {
            get { return _comentarios; }
            set { SetProperty(ref _comentarios, value); }
        }

        private async Task CarregarLista()
        {
            IsBusy = true;
            try
            {
                IList<Comentario> lista = await _database.ListarComentario(criterioBusca);
                Comentarios = new ObservableCollection<Comentario>(lista.OrderBy(d => d.Data).ThenBy(d=>d.Hora));

            }
            finally
            {
                IsBusy = false;
            }


        }
    }
   
}
