using CV.Mobile.Services.Api;
using CV.Mobile.Services.Sincronizacao;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.ViewModels
{
    public class SincronizacaoViewModel: BaseViewModel
    {
        private IApiService _apiService;
        private ISincronizacao _sincronizacao;

        public SincronizacaoViewModel(IApiService apiService,ISincronizacao sincronizacao)
        {
            _apiService = apiService;
            _sincronizacao = sincronizacao;
        }


        public override async Task InitializeAsync(object navigationData)
        {

            IsBusy = true;
            try
            {
                try
                {
                    await _sincronizacao.Sincronizar(true);
                    await NavigationService.TrocarPaginaShell("..");
                }
                catch (Exception)
                {
                    await DialogService.ShowAlertAsync("Ocorreu um erro inexperado na comunicação com o servidor. Tente Novamente mais tarde", "Erro Comunicação", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }

        }
    }
}
