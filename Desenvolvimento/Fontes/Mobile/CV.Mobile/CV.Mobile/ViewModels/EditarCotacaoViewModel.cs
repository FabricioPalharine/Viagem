﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CV.Mobile.Helpers;
using CV.Mobile.Models;
using System.Collections.ObjectModel;
using CV.Mobile.Services;
using FormsToolkit;

namespace CV.Mobile.ViewModels
{
    public class EditarCotacaoViewModel : BaseNavigationViewModel
    {
        private CotacaoMoeda _ItemCotacao;

        public EditarCotacaoViewModel(CotacaoMoeda pItemCotacao)
        {
            _ItemCotacao = pItemCotacao;
            ListaMoeda = new ObservableCollection<ItemLista>();
            List<ItemLista> lista = new List<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumMoeda)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumMoeda)enumerador).Descricao() };
                ListaMoeda.Add(item);
            }
            ListaMoeda = new ObservableCollection<ItemLista>(ListaMoeda.OrderBy(d => d.Descricao));
            SalvarCommand = new Command(
                                            async () => await Salvar(),
                                            () => true);

        }

        public Command SalvarCommand { get; set; }

        public ObservableCollection<ItemLista> ListaMoeda { get; set; }

        public CotacaoMoeda ItemCotacao
        {
            get
            {
                return _ItemCotacao;
            }

            set
            {
                SetProperty(ref _ItemCotacao, value);
            }
        }

        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                ResultadoOperacao Resultado = new ResultadoOperacao();
                bool Executado = false;
                if (Conectado)
                {
                    try
                    {
                        using (ApiService srv = new ApiService())
                        {
                            Resultado = await srv.SalvarCotacaoMoeda(ItemCotacao);
                            if (Resultado.Sucesso)
                            {
                                base.AtualizarViagem(ItemViagemSelecionada.Identificador.GetValueOrDefault(), "CM", ItemCotacao.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()), !ItemCotacao.Identificador.HasValue);
                                ItemCotacao = await srv.CarregarCotacaoMoeda(ItemCotacao.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()));

                                var itemAjustar = await DatabaseService.Database.RetornarCotacaoMoeda(ItemCotacao.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()));
                                if (itemAjustar != null)
                                    ItemCotacao.Id = itemAjustar.Id;
                                itemAjustar.DataExclusao = DateTime.Now.ToUniversalTime();
                                await DatabaseService.Database.SalvarCotacaoMoeda(ItemCotacao);


                            }

                        }
                        Executado = true;
                    }
                    catch { Executado = false; }
                }
                if (!Executado)
                {
                    ItemCotacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    ItemCotacao.AtualizadoBanco = false;
                    Resultado = await DatabaseService.SalvarCotacaoMoeda(ItemCotacao);
                }
                if (Resultado.Sucesso)
                {

                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Sucesso",
                        Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                        Cancel = "OK"
                    });
                    ItemCotacao.Identificador = Resultado.IdentificadorRegistro;
                    MessagingService.Current.SendMessage<CotacaoMoeda>(MessageKeys.ManutencaoCotacaoMoeda, ItemCotacao);
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

    }
}
