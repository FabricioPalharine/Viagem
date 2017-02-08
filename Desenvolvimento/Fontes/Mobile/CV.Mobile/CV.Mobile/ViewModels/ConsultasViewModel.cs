﻿using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class ConsultasViewModel : BaseNavigationViewModel
    {
        public ObservableCollection<ItemMenu> ItensMenu { get; set; }
        public ObservableCollection<ItemMenu> ItensMenuCompleto { get; set; }
        private ItemMenu _ItemMenuSelecionado;

        public ConsultasViewModel()
        {
            ItensMenuCompleto = new ObservableCollection<ItemMenu>();
            CarregarItensMenu();
            ItensMenu = new ObservableCollection<ItemMenu>(ItensMenuCompleto.Where(d => d.Visible));
            OnPropertyChanged("ItensMenu");
        }

        private void CarregarItensMenu()
        {

            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 1,
                Title = "Extrato Dinheiro",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 2,
                Title = "Acerto Contas",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 3,
                Title = "Relatório Gastos",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = false,
                VerCustos = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 4,
                Title = "Timeline",
                IconSource = "Dados.png",
                Visible = true,
                ApenasParticipante = false
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 5,
                Title = "Locais",
                IconSource = "Dados.png",
                Visible = true,
                ApenasParticipante = false
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 6,
                Title = "Mapa",
                IconSource = "Dados.png",
                Visible = true,
                ApenasParticipante = false
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 7,
                Title = "Foto",
                IconSource = "Dados.png",
                Visible = true,
                ApenasParticipante = false
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 8,
                Title = "Calendário Realizado",
                IconSource = "Dados.png",
                Visible = true,
                ApenasParticipante = false
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 9,
                Title = "Calendário Previsto",
                IconSource = "Dados.png",
                Visible = false
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 10,
                Title = "Resumo",
                IconSource = "Dados.png",
                Visible = true,
                ApenasParticipante = false
            });

            foreach (var itemMenu in ItensMenuCompleto)
            {
                if (itemMenu.ApenasParticipante)
                    itemMenu.Visible = ItemViagemSelecionada.Edicao;
                else if (itemMenu.VerCustos)
                    itemMenu.Visible = ItemViagemSelecionada.Edicao || ItemViagemSelecionada.VejoGastos;
                else
                    itemMenu.Visible = true;

            }
            ItensMenu = new ObservableCollection<ItemMenu>(ItensMenuCompleto.Where(d => d.Visible));
            OnPropertyChanged("ItensMenu");
        }

        public ItemMenu ItemSelecionado
        {
            get
            {
                return _ItemMenuSelecionado;
            }

            set
            {
                _ItemMenuSelecionado = null;
                OnPropertyChanged();
            }
        }

        public Command<ItemTappedEventArgs> ItemTappedCommand
        {
            get
            {
                return new Command<ItemTappedEventArgs>(
                                                                 async (obj) => await VerificarAcaoItem(obj));
            }
        }
        private async Task VerificarAcaoItem(ItemTappedEventArgs itemSelecionado)
        {
            ItemMenu itemMenu = (ItemMenu)itemSelecionado.Item;
            if (itemMenu.Codigo == 1)
            {
                await AbrirExtrato();
            }
            else if (itemMenu.Codigo == 2)
            {
                await AbrirAjusteGasto();
            }

        }

        private async Task AbrirAjusteGasto()
        {
            if (await VerificarOnline())
            {
                var pagina = new ConsultarGastoDivididoPage() { BindingContext = new ConsultarGastoDivididoViewModel() };

                await PushAsync(pagina);
            }

        }

        private async Task<bool> VerificarOnline()
        {
            bool Online = false;
            using (ApiService srv = new ApiService())
            {
                Online = CrossConnectivity.Current.IsConnected && await srv.VerificarOnLine();
            }
            if (!Online)
                ExibirAlertaOffLine();
            return Online;
        }
        private void ExibirAlertaOffLine()
        {
            MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
            {
                Title = "Conexão Off-Line",
                Message = "Essa funcionalidade necessita que a conexão esteja On-Line",
                Cancel = "OK"
            });
        }
        private async Task AbrirExtrato()
        {
            var pagina = new ConsultarExtratoPage() { BindingContext = new ConsultarExtratoViewModel() };
            await PushAsync(pagina);
        }
    }
}
