using CV.Mobile.Models;
using FormsToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class TextPopupViewModel: BaseNavigationViewModel
    {
        public string Texto { get; set; }
        public string Valor { get; set; }

        public bool Confirmado { get; set; }

        public Command ConfirmarCommand
        {
            get
            {
                return new Command(async () =>
                {
                    Confirmado = true;
                    await PopModalAsync(true);
                    MessagingService.Current.SendMessage(MessageKeys.MensagemModalConfirmada);
                });
            }
        }

        public Command CancelarCommand
        {
            get
            {
                return new Command(async () =>
                {
                    Confirmado = true;
                    await PopModalAsync(true);

                    MessagingService.Current.SendMessage(MessageKeys.MensagemModalConfirmada);
                });
            }
        }
    }
}
