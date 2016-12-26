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
                    posicao = new Plugin.Geolocator.Abstractions.Position() { Latitude = -23.6040963, Longitude = -46.6178018 };
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

                using (ApiService srv = new ApiService())
                {
                    var Resultado = await srv.SalvarComentario(ItemComentario);
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
                    using (ApiService srv = new ApiService())
                    {
                        ItemComentario.DataExclusao = DateTime.Now;
                        var Resultado = await srv.ExcluirComentario(ItemComentario.Identificador);
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                    }
                    MessagingService.Current.SendMessage<Comentario>(MessageKeys.ManutencaoComentario, ItemComentario);
                    await PopAsync();

                })
            });


        }

      
    }
}
