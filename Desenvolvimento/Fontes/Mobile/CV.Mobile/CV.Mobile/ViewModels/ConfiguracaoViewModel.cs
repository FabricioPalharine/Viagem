using CV.Mobile.Helpers;
using CV.Mobile.Models;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class ConfiguracaoViewModel: BaseNavigationViewModel
    {
        public ObservableRangeCollection<ItemLista> TiposAtualizacao { get; set; }
        public ObservableRangeCollection<ItemLista> ListaTipoOnline { get; set; }

        public string SincronizarDados { get; set; }
        public string EnviarImagens { get; set; }
        public string EnviarVideos { get; set; }
        public string ManterOnline { get; set; }

        public ConfiguracaoViewModel()
        {
            TiposAtualizacao = new ObservableRangeCollection<ItemLista>();
            TiposAtualizacao.Add(new ItemLista() { Codigo = "1", Descricao = "Rede Móvel" });
            TiposAtualizacao.Add(new ItemLista() { Codigo = "2", Descricao = "Rede WiFi" });
            TiposAtualizacao.Add(new ItemLista() { Codigo = "3", Descricao = "Comando Manual" });

            ListaTipoOnline = new ObservableRangeCollection<ItemLista>();
            ListaTipoOnline.Add(new ItemLista() { Codigo = "1", Descricao = "Rede Móvel" });
            ListaTipoOnline.Add(new ItemLista() { Codigo = "2", Descricao = "Rede WiFi" });
            ListaTipoOnline.Add(new ItemLista() { Codigo = "3", Descricao = "Nunca" });

            SincronizarDados = Settings.ModoSincronizacao;
            ManterOnline = Settings.AcompanhamentoOnline;
            EnviarImagens = Settings.ModoImagem;
            EnviarVideos = Settings.ModoVideo;
        }

        public Command SalvarCommand
        {
            get
            {
                return new Command(async () =>
                {
                    Settings.ModoImagem = EnviarImagens;
                    Settings.ModoSincronizacao = SincronizarDados;
                    Settings.ModoVideo = EnviarVideos;
                    Settings.AcompanhamentoOnline = ManterOnline;
                    await PopAsync();
                });
            }
        }
    }
}
