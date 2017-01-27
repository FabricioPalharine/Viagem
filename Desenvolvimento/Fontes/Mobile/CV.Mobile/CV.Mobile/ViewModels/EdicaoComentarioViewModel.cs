using CV.Mobile.Helpers;
using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CV.Mobile.ViewModels
{

    public class EdicaoComentarioViewModel : BaseNavigationViewModel
    {
        private Comentario _ItemComentario;
        private MapSpan _Bounds;
        private bool _PermiteExcluir = true;

   
        public EdicaoComentarioViewModel(Comentario pItemComentario, Viagem pItemViagem)
        {
          
             ItemComentario = pItemComentario;

            ItemViagem = pItemViagem;
            
            
            SalvarCommand = new Command(
                                async () => await Salvar(),
                                () => true);
            PageAppearingCommand = new Command(
                                                                  async () =>
                                                                  {
                                                                      await CarregarPosicao();
                                                                  },
                                                                  () => true);

 
            ExcluirCommand = new Command(() =>  Excluir());
           
        }
      public Command SalvarCommand { get; set; }
        public Command PageAppearingCommand { get; set; }
        public Command ExcluirCommand { get; set; }
      

        public Viagem ItemViagem { get; set; }

        public async Task CarregarPosicao()
        {
            await Task.Delay(100);
            PermiteExcluir = ItemComentario.Id.HasValue || ItemComentario.Identificador.HasValue;
            if (_ItemComentario.Latitude.HasValue && _ItemComentario.Longitude.HasValue)
            {
                Bounds = MapSpan.FromCenterAndRadius(new Position(_ItemComentario.Latitude.Value, _ItemComentario.Longitude.Value), new Distance(5000));
               

            }
            else
            {
                var posicao = await RetornarPosicao();
                if (posicao == null)
                    posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = 0, Longitude = 0 };
                Bounds = MapSpan.FromCenterAndRadius(new Position(posicao.Latitude, posicao.Longitude), new Distance(5000));
                ItemComentario.Longitude = posicao.Longitude;
                ItemComentario.Latitude = posicao.Latitude;
            }
           
            

        }

        public Comentario ItemComentario
        {
            get
            {
                return _ItemComentario;
            }

            set
            {
                SetProperty(ref _ItemComentario, value);
            }
        }

     
        public MapSpan Bounds
        {
            get
            {
                return _Bounds;
            }

            set
            {
                SetProperty(ref _Bounds, value);
            }
        }

        public bool PermiteExcluir
        {
            get
            {
                return _PermiteExcluir;
            }

            set
            {
                SetProperty(ref _PermiteExcluir, value);
            }
        }

        
        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                
                ItemComentario.Data = ItemComentario.Data.GetValueOrDefault().Date.Add(ItemComentario.Hora.GetValueOrDefault());
                ResultadoOperacao Resultado = new ResultadoOperacao();
                if (Conectado)
                {
                    using (ApiService srv = new ApiService())
                    {
                        bool Inclusao = !ItemComentario.Identificador.HasValue;
                        Resultado = await srv.SalvarComentario(ItemComentario);
                        if (Resultado.Sucesso)
                        {
                            base.AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "T", ItemComentario.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()), Inclusao);
                            var itembase = await srv.CarregarComentario(ItemComentario.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()));
                            var itemAjustar = await DatabaseService.Database.RetornarComentario(ItemComentario.Identificador);
                            if (itemAjustar != null)
                                itembase.Id = itemAjustar.Id;
                            itemAjustar.DataAtualizacao = DateTime.Now.ToUniversalTime();                           
                            await DatabaseService.Database.SalvarComentario(itembase);

                        }
                    }
                    
                }
                else
                {
                    ItemComentario.AtualizadoBanco = false;
                    Resultado = await DatabaseService.SalvarComentario(ItemComentario);
                }

                if (Resultado.Sucesso)
                {

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    ItemComentario.Identificador = Resultado.IdentificadorRegistro;

                    MessagingService.Current.SendMessage<Comentario>(MessageKeys.ManutencaoComentario, ItemComentario);
                    PermiteExcluir = true;
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
                SalvarCommand.ChangeCanExecute();
                IsBusy = false;
            }
        }

        private void Excluir()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
            {
                Title = "Confirmação",
                Question = String.Format("Deseja excluir esse Comentário?"),
                Positive = "Sim",
                Negative = "Não",
                OnCompleted = new Action<bool>(async result =>
                {
                    if (!result) return;
                    ItemComentario.DataExclusao = DateTime.Now.ToUniversalTime();
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    if (Conectado)
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.ExcluirComentario(ItemComentario.Identificador);
                            base.AtualizarViagem(ItemViagem.Identificador.GetValueOrDefault(), "T", ItemComentario.Identificador.GetValueOrDefault(), false);
                            var itemAjustar = await DatabaseService.Database.RetornarComentario(ItemComentario.Identificador);
                            if (itemAjustar != null )
                             await DatabaseService.Database.ExcluirComentario(itemAjustar);
                            
                        }
                    }
                    else
                    {
                        if (ItemComentario.Identificador > 0)
                        {
                            ItemComentario.AtualizadoBanco = false;
                            await DatabaseService.Database.SalvarComentario(ItemComentario);
                        }
                        else
                            await DatabaseService.Database.ExcluirComentario(ItemComentario);

                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = "Comentário excluído com sucesso " } };
                    }

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    MessagingService.Current.SendMessage<Comentario>(MessageKeys.ManutencaoComentario, ItemComentario);
                    await PopAsync();

                })
            });


        }

      
    }
}
