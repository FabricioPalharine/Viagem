using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Services.Dialog
{
    public interface IDialogService
    {
        Task<string> ShowActionList(string title, string cancel, string destrucitve, List<string> opcoes);
        Task ShowAlertAsync(string message, string title, string buttonLabel);
        Task<PromptResult> ShowPromptAsync(string message, string title, string confirmButton, string cancelButton, InputType inputType);
        Task<bool> ShowConfirmAsync(string message, string title, string confirmButton, string cancelButton);
    }
}
