using CV.Mobile.Helpers;
using CV.Mobile.Models;
using CV.Mobile.Services;
using FormsToolkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TK.CustomMap;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.ViewModels
{
    public class EdicaoAgendarSugestaoViewModel : BaseNavigationViewModel
    {
        private Sugestao _ItemSugestao;
        private CalendarioPrevisto _ItemAgenda;


        public EdicaoAgendarSugestaoViewModel(Sugestao pItemSugestao, CalendarioPrevisto pItemCalendario )
        {
            ItemSugestao = pItemSugestao;
            ItemAgenda = pItemCalendario;
            PageAppearingCommand = new Command(async () => await CarregarPagina(), () => true);
            ConfirmarCommand = new Command(async () => await SalvarAgenda(), () => true);

            ListaStatus = new ObservableCollection<ItemLista>();
            ListaStatus.Add(new ItemLista() { Codigo = "1", Descricao = "Baixa" });
            ListaStatus.Add(new ItemLista() { Codigo = "2", Descricao = "Média" });
            ListaStatus.Add(new ItemLista() { Codigo = "3", Descricao = "Alta" });
        }
        public ObservableCollection<ItemLista> ListaStatus { get; set; }

        public Command PageAppearingCommand { get; set; }

        public Command ConfirmarCommand { get; set; }

        public async Task CarregarPagina()
        {
            await Task.Delay(100);
        }

        public async Task SalvarAgenda()
        {
          
         

            IsBusy = true;
            ConfirmarCommand.ChangeCanExecute();
            try
            {
                AgendarSugestao itemAgenda = new AgendarSugestao();
                itemAgenda.itemSugestao = ItemSugestao;
                ItemAgenda.DataInicio = ItemAgenda.DataInicio.GetValueOrDefault().Date.Add(ItemAgenda.HoraInicio.Value);
                ItemAgenda.DataFim = ItemAgenda.DataFim.GetValueOrDefault().Date.Add(ItemAgenda.HoraFim.Value);
                itemAgenda.itemCalendario = ItemAgenda;
                ResultadoOperacao Resultado = new ResultadoOperacao();
                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {
                        Resultado = await srv.SalvarAgendarSugestao(itemAgenda);
                        if (Resultado.Sucesso)
                        {
                            base.AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "CP", Resultado.IdentificadorRegistro.GetValueOrDefault(), true);
                            ItemAgenda.Identificador = Resultado.IdentificadorRegistro;

                            await DatabaseService.Database.SalvarCalendarioPrevisto(ItemAgenda);
                            var itemSugestao = await DatabaseService.Database.RetornarSugestao(ItemSugestao.Identificador);
                            if (itemSugestao != null)
                            {
                                ItemSugestao.Id = itemSugestao.Id;
                            }
                            ItemSugestao.Status = 2;
                            ItemSugestao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                            await DatabaseService.Database.SalvarSugestao(ItemSugestao);
                          
                        }
                    }
                }
                else
                {
                    Resultado = await DatabaseService.SalvarAgendamentoSugestao(itemAgenda);
                }

                if (Resultado.Sucesso)
                    {

                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        ItemSugestao.Status = 2;
                        // ItemListaCompra = JsonConvert.DeserializeXNode < ListaCompra >()
                        MessagingService.Current.SendMessage<Sugestao>(MessageKeys.ManutencaoSugestao, ItemSugestao);
                        MessagingService.Current.SendMessage<CalendarioPrevisto>(MessageKeys.ManutencaoCalendario, ItemAgenda);

                        await PopAsync();
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
            finally
            {
                ConfirmarCommand.ChangeCanExecute();
                IsBusy = false;
            }

        }

        public Sugestao ItemSugestao
        {
            get
            {
                return _ItemSugestao;
            }

            set
            {
                SetProperty(ref _ItemSugestao, value);
            }
        }


       

        public CalendarioPrevisto ItemAgenda
        {
            get
            {
                return _ItemAgenda;
            }

            set
            {
                SetProperty(ref _ItemAgenda, value);
            }
        }

    }
}
