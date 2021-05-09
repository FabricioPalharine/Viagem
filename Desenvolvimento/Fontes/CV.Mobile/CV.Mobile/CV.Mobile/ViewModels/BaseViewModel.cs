using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services;
using CV.Mobile.Services.Dialog;
using CV.Mobile.Services.Navigation;
using CV.Mobile.Validations;
using CV.Mobile.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();
        protected readonly IDialogService DialogService;
        protected readonly INavigationService NavigationService;

        public BaseViewModel()
        {
            DialogService = ViewModelLocator.Resolve<IDialogService>();
            NavigationService = ViewModelLocator.Resolve<INavigationService>();
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
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



        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion

        internal async Task ExibirResultado(ResultadoOperacao resultado, bool Voltar=false)
        {
            if (resultado.Sucesso)
            {
                await DialogService.ShowAlertAsync(String.Join(Environment.NewLine, resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                    AppResource.Sucesso, AppResource.Ok);
                if (Voltar)
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
    }
}
