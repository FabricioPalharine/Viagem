using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels.Amigos
{
    public class AmigoAdicaoViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private ValidatableObject<string> _mail = new ValidatableObject<string>();
        private ConsultaAmigo _itemAmigo = null;
        private bool _seguido = true;
        private bool _seguidor = true;
        public AmigoAdicaoViewModel(IApiService apiService)
        {
            _apiService = apiService;
            AdicionarValidacoes();
        }

        public ConsultaAmigo ItemAmigo
        {
            get { return _itemAmigo; }
            set { SetProperty(ref _itemAmigo, value); } 
        }



        public ValidatableObject<string> EMail
        {
            get { return _mail; }
            set { SetProperty(ref _mail, value); }
        }

        public bool Seguido
        {
            get { return _seguido; }
            set { SetProperty(ref _seguido, value); }
        }

        public bool Seguidor
        {
            get { return _seguidor; }
            set { SetProperty(ref _seguidor, value); }
        }

        public ICommand MailChangedCommand => new Command(async () => await TrocarTexto());
        public ICommand SalvarCommand => new Command(async () => await Salvar());
        public ICommand CancelarCommand => new Command(async () => await NavigationService.TrocarPaginaShell(".."));
        private async Task TrocarTexto()
        {
            ValidarMail();
            await VerificarTempoBusca();
        }

        private CancellationTokenSource throttleCts = new CancellationTokenSource();
        /// <summary>
        /// Runs in a background thread, checks for new Query and runs current one
        /// </summary>
        private async Task VerificarTempoBusca()
        {
            try
            {
                Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
                await Task.Delay(TimeSpan.FromMilliseconds(500), this.throttleCts.Token)
              .ContinueWith(async task => await Pesquisar(),
                            CancellationToken.None,
                            TaskContinuationOptions.OnlyOnRanToCompletion,
                            TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch
            {
                //Ignore any Threading errors
            }
        }

        private async Task Pesquisar()
        {
            if (ValidarMail())
            {
                CriterioBusca busca = new CriterioBusca() { EMail = EMail.Value };
                var lista = await _apiService.ListarUsuarios(busca);
                var usuario = lista.FirstOrDefault();
                if (usuario != null)
                {
                    ItemAmigo = new ConsultaAmigo() { IdentificadorUsuario = usuario.Identificador, Nome = usuario.Nome };
                }
                else
                    ItemAmigo = new ConsultaAmigo();
            }
            else
                ItemAmigo = new ConsultaAmigo();
        }

        private bool ValidarMail()
        {
            return EMail.Validate();
        }
        private void AdicionarValidacoes()
        {
            EMail.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = AppResource.CampoObrigatorio });
            EMail.Validations.Add(new RegularExpressionRule<string>() { RegularExpression = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ValidationMessage = AppResource.MailInvalido });
        }

        private async Task Salvar()
        {
            bool Valido = ValidarMail();
            if (Valido)
            {
                IsBusy = true;
                try
                {
                    var itemAmigo = ItemAmigo ?? new ConsultaAmigo();
                    itemAmigo.EMail = EMail.Value;
                    itemAmigo.Seguido = Seguido;
                    itemAmigo.Seguidor = Seguidor;
                    var resultado = await _apiService.SalvarAmigo(itemAmigo);
                    if (resultado.Sucesso)
                    {
                        await DialogService.ShowAlertAsync(String.Join(Environment.NewLine, resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            AppResource.Sucesso, AppResource.Ok);
                        MessagingCenter.Send<AmigoAdicaoViewModel>(this, MessageKeys.AmigoAdicionado);
                        await NavigationService.TrocarPaginaShell("..");
                    }
                    else
                    {
                        var mensagens = AjustarErros(resultado.Mensagens);
                        if (mensagens.Any())
                            await DialogService.ShowAlertAsync(String.Join(Environment.NewLine, mensagens.Select(d => d.Mensagem).ToArray()),
                            AppResource.Problemas, AppResource.Ok);

                    }

                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}
