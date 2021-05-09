using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.ViewModels.Amigos
{
    public class AmigosViewModel: BaseViewModel
    {

        private int _indiceTabSelecionada = 0;
        public AmigosViewModel()
        {

        }

        public int IndiceTabSelecionada
        {
            get { return _indiceTabSelecionada; }
            set { SetProperty(ref _indiceTabSelecionada, value); }
        }
    }
}
