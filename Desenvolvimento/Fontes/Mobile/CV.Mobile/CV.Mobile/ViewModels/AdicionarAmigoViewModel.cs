using CV.Mobile.Models;
using CV.Mobile.Services;
using FormsToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class AdicionarAmigoViewModel: BaseNavigationViewModel
    {
        public AdicionarAmigoViewModel()
        {
            ItemAmigo = new ConsultaAmigo();
            EMailTextChangedCommand = new Command(async () => await PesquisarUsuarioMail());
            SalvarCommand = new Command(async () => await Salvar(),()=>true);
        }


        private ConsultaAmigo _itemAmigo;

        public Command EMailTextChangedCommand { get; set; }
        public Command SalvarCommand { get; set; }
        public ConsultaAmigo ItemAmigo
        {
            get
            {
                return _itemAmigo;
            }

            set
            {
                SetProperty(ref _itemAmigo, value);
            }
        }



        private async Task PesquisarUsuarioMail()
        {
            if (!string.IsNullOrWhiteSpace(ItemAmigo.EMail))
            {
                CriterioBusca busca = new CriterioBusca() { EMail = ItemAmigo.EMail };
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        var Lista = await srv.ListarUsuarios(busca);
                        if (Lista.Any())
                        {
                            ItemAmigo.IdentificadorUsuario = Lista[0].Identificador;
                            ItemAmigo.Nome = Lista[0].Nome;
                        }
                        else
                        {
                            ItemAmigo.IdentificadorUsuario = null;
                            ItemAmigo.Nome = null;
                        }
                    }
                }
                catch
                {
                    ApiService.ExibirMensagemErro();
                }
            }
            else
            {
                ItemAmigo.IdentificadorUsuario = null;
                ItemAmigo.Nome = null;
            }
        }

        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                try
                {
                    using (ApiService srv = new ApiService())
                    {
                        var Resultado = await srv.SalvarAmigo(ItemAmigo);
                        if (Resultado.Sucesso)
                        {
                            if (ItemAmigo.Seguidor && ItemAmigo.IdentificadorUsuario.HasValue)
                            {
                                DatabaseService.AdicionarAmigoBase(ItemAmigo);
                            }
                            MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                            {
                                Title = "Sucesso",
                                Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                                Cancel = "OK"
                            });
                            MessagingService.Current.SendMessage(MessageKeys.AdicionarAmigo);
                            await PopAsync();
                        }
                        else if (Resultado.Mensagens != null && Resultado.Mensagens.Any())
                        {
                            MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                            {
                                Title = "Problemas Validação",
                                Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                                Cancel = "OK"
                            });

                        }
                    }
                }
                catch
                {
                    ApiService.ExibirMensagemErro();
                }
            }
            finally
            {
                SalvarCommand.ChangeCanExecute();
                IsBusy = false;
            }
        }
    }
}
