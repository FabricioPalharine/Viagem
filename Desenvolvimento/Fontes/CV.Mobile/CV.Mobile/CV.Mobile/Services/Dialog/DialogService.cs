using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string message, string title, string buttonLabel)
        {
            return UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        public Task<PromptResult> ShowPromptAsync(string message, string title, string confirmButton, string cancelButton, InputType inputType)
        {
            return UserDialogs.Instance.PromptAsync(message, title, confirmButton, cancelButton, inputType: inputType);
        }

        public Task<bool> ShowConfirmAsync(string message, string title, string confirmButton, string cancelButton)
        {
            return UserDialogs.Instance.ConfirmAsync(message, title, confirmButton, cancelButton);
            
        }

        public Task<string> ShowActionList(string title, string cancel, string destrucitve, List<string> opcoes)
        {
            return UserDialogs.Instance.ActionSheetAsync(title, cancel, destrucitve, null, opcoes.ToArray());
        }
    }
}
