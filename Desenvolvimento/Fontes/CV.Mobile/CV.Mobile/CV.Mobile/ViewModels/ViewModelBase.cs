using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CV.Mobile.Models;
using CV.Mobile.Services.Dialog;
using CV.Mobile.Services.Navigation;
using System.Reflection;
using System.Linq;
using CV.Mobile.Validations;
using System.Windows.Input;
using Xamarin.Forms;
using CV.Mobile.Resources;

namespace CV.Mobile.ViewModels.Base
{
    public abstract class ViewModelBase : ExtendedBindableObject
    {
        protected readonly IDialogService DialogService;
        protected readonly INavigationService NavigationService;
        private bool _exibirMensagemAlerta = false;
        private bool _exibirBotaoConfirmar = false;
        private string _mensagem = string.Empty;
        private string _titulo = string.Empty;
        private string _textoBotaoCancelar = AppResource.Cancelar;
        private string _textoBotaoConfirmar = AppResource.Confirmar;
        private bool _exibirBotaoCancelar = true;
        private ICommand _commandCancelar = null;
        private ICommand _commandConfirmar = null;
        private bool _exibirTokenConfirmacao = false;
        protected ValidatableObject<string> _tokenConfirmacao = new ValidatableObject<string>();


        private bool _isBusy;

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public ViewModelBase()
        {
            DialogService = ViewModelLocator.Resolve<IDialogService>();
            NavigationService = ViewModelLocator.Resolve<INavigationService>();

            //var settingsService = ViewModelLocator.Resolve<ISettingsService>();

            //GlobalSetting.Instance.BaseIdentityEndpoint = settingsService.IdentityEndpointBase;
            //GlobalSetting.Instance.BaseGatewayShoppingEndpoint = settingsService.GatewayShoppingEndpointBase;
            //GlobalSetting.Instance.BaseGatewayMarketingEndpoint = settingsService.GatewayMarketingEndpointBase;
        }

        public ICommand VoltarCommand => new Command(async () => await NavigationService.ReturnAsync());
        public ICommand MenuCommand => new Command(async () => await NavigationService.ReturnFirstAsync());


        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }

        public string Mensagem
        {
            get { return _mensagem; }
            set
            {
                _mensagem = value;
                RaisePropertyChanged(() => Mensagem);
            }
        }

        public string Titulo
        {
            get { return _titulo; }
            set
            {
                _titulo = value;
                RaisePropertyChanged(() => Titulo);
            }
        }
        public string TextoBotaoCancelar
        {
            get { return _textoBotaoCancelar; }
            set
            {
                _textoBotaoCancelar = value;
                RaisePropertyChanged(() => TextoBotaoCancelar);
            }
        }


        public ValidatableObject<string> TokenConfirmacao
        {
            get { return _tokenConfirmacao; }
            set
            {
                _tokenConfirmacao = value;
                RaisePropertyChanged(() => TokenConfirmacao);
            }
        }

        public bool ExibirTokenConfirmacao
        {
            get { return _exibirTokenConfirmacao; }
            set
            {
                _exibirTokenConfirmacao = value;
                RaisePropertyChanged(() => ExibirTokenConfirmacao);
            }
        }

        public bool ExibirMensagemAlerta
        {
            get { return _exibirMensagemAlerta; }
            set
            {
                _exibirMensagemAlerta = value;
                RaisePropertyChanged(() => ExibirMensagemAlerta);
            }
        }

        public bool ExibirBotaoConfirmar
        {
            get { return _exibirBotaoConfirmar; }
            set
            {
                _exibirBotaoConfirmar = value;
                RaisePropertyChanged(() => ExibirBotaoConfirmar);

            }
        }

        public bool ExibirBotaoCancelar
        {
            get { return _exibirBotaoCancelar; }
            set
            {
                _exibirBotaoCancelar = value;
                RaisePropertyChanged(() => ExibirBotaoCancelar);

            }
        }

        public string TextoBotaoConfirmar
        {
            get { return _textoBotaoConfirmar; }
            set
            {
                _textoBotaoConfirmar = value;
                RaisePropertyChanged(() => TextoBotaoConfirmar);
            }
        }

        public List<MensagemErro> AjustarErros(IEnumerable<MensagemErro> mensagens)
        {
            List<MensagemErro> mensagensPost = new List<MensagemErro>();
            foreach (var mensagem in mensagens)
            {
                bool mensagemUtilizada = false;
                if (!string.IsNullOrEmpty(mensagem.Campo))
                {
                 
                        var propriedade = this.GetType().GetProperty(mensagem.Campo);

                        if (propriedade != null && propriedade.PropertyType.IsGenericType && propriedade.PropertyType.GetGenericTypeDefinition() == typeof(ValidatableObject<>))
                        {
                            var valor = propriedade.GetValue(this);
                            var funcao = propriedade.PropertyType.GetMethod("AdicionarErro");
                            funcao.Invoke(valor, new object[] { mensagem.Mensagem });
                            mensagemUtilizada = true;

                        }
                    

                }
                if (!mensagemUtilizada)
                    mensagensPost.Add(mensagem);
            }

            return mensagensPost;
        }

        public void AbrirAlerta(string Titulo, 
            string Mensagem, 
            bool? ExibeBotaoCancelar = null, 
            bool? ExibeBotaoConfirmar = null, 
            string TextoBotaoConfirmar = null, 
            string TextoBotaoCancelar = null,
            ICommand commandCancelar = null,
            ICommand commandConfirmar = null,
            bool tokenConfimacao = false)
        {
            this.Titulo = Titulo;
            this.Mensagem = Mensagem;
            if (ExibeBotaoCancelar.HasValue)
                ExibirBotaoCancelar = ExibeBotaoCancelar.Value;
            if (ExibeBotaoConfirmar.HasValue)
                ExibirBotaoConfirmar = ExibeBotaoConfirmar.Value;
            if (!string.IsNullOrEmpty(TextoBotaoConfirmar))
                this.TextoBotaoConfirmar = TextoBotaoConfirmar;
            if (!string.IsNullOrEmpty(TextoBotaoCancelar))
                this.TextoBotaoCancelar = TextoBotaoCancelar;
            if (commandCancelar != null)
                CancelarCommand = commandCancelar;
            if (commandConfirmar != null)
                ConfirmarCommand = commandConfirmar;
            ExibirMensagemAlerta = true;
            TokenConfirmacao.Value = string.Empty;
        }

       

        public virtual ICommand CancelarCommand
        {
            get { return _commandCancelar; }
            set
            {
                _commandCancelar = value;
                RaisePropertyChanged(() => CancelarCommand);
            }                
        }

        public virtual ICommand ConfirmarCommand
        {
            get { return _commandConfirmar; }
            set
            {
                _commandConfirmar = value;
                RaisePropertyChanged(() => ConfirmarCommand);
            }
        }
    }
}
