using CV.Mobile.Models;
using CV.Mobile.Services;
using CV.Mobile.Views;
using FormsToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels
{
    public class MenuViewModel:BaseNavigationViewModel
    {
        private readonly TaskScheduler _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private UsuarioLogado _ItemUsuario;

        public ObservableCollection<ItemMenu> ItensMenu { get; set; }
        public ObservableCollection<ItemMenu> ItensMenuCompleto { get; set; }

        private ItemMenu _ItemMenuSelecionado;
        private Viagem _ItemViagem;

        public delegate Task ItemMenuSelecionadoDelegate(Page pagina, bool NovoStack);
        public event ItemMenuSelecionadoDelegate ItemMenuSelecionado;

        public UsuarioLogado ItemUsuario
        {
            get
            {
                return _ItemUsuario;
            }

            set
            {
                SetProperty(ref _ItemUsuario, value);
            }
        }

        public ItemMenu ItemSelecionado
        {
            get
            {
                return _ItemMenuSelecionado;
            }

            set
            {
                if (_ItemMenuSelecionado != value)
                {
                    SetProperty(ref _ItemMenuSelecionado, value);
                }
            }
        }


        public Viagem ItemViagem
        {
            get
            {
                return _ItemViagem;
            }

            set
            {
                SetProperty(ref _ItemViagem, value);
                if (value != null)
                {
                    AjustarVisibilidadeItens();
                }
                else
                {
                    foreach (var itemMenu in ItensMenuCompleto)
                        if (itemMenu.Codigo != 0)
                            itemMenu.Visible = false;
                   
                }
                ItensMenu = new ObservableCollection<ItemMenu>(ItensMenuCompleto.Where(d => d.Visible));
                OnPropertyChanged("ItensMenu");

            }
        }

        private void AjustarVisibilidadeItens()
        {
            foreach (var itemMenu in ItensMenuCompleto)
            {
                if (itemMenu.ApenasAmigo)
                    itemMenu.Visible = !ItemViagem.Edicao;
                else if (itemMenu.ApenasParticipante)
                    itemMenu.Visible = ItemViagem.Edicao;
                else
                    itemMenu.Visible = true;

                if (itemMenu.Visible)
                {
                    if (itemMenu.ViagemAberta)
                        itemMenu.Visible = ItemViagem.Aberto;
                }

                if (itemMenu.Codigo == 19)
                {
                    if (ItemViagem.Aberto)
                        itemMenu.Title = "Fechar Viagem";
                    else
                        itemMenu.Title = "Reabrir Viagem";
                }
                else if (itemMenu.Codigo == 20)
                {
                    if (ItemViagem.ControlaPosicaoGPS)
                        itemMenu.Title = "Parar Rota";
                    else
                        itemMenu.Title = "Iniciar Rota";
                }
            }
        }

        public async Task OnItemMenuSelecionado(Page pagina, bool NovoStack)
        {
            if (ItemMenuSelecionado != null)
                await ItemMenuSelecionado(pagina, NovoStack);
        }

        public MenuViewModel(UsuarioLogado itemUsuario)
        {
            _ItemUsuario = itemUsuario;
            ItensMenuCompleto = new ObservableCollection<ItemMenu>();
            CarregarItensMenu();
            ItensMenu = new ObservableCollection<ItemMenu>(ItensMenuCompleto.Where(d => d.Visible));
            OnPropertyChanged("ItensMenu");
        }

        private void CarregarItensMenu()
        {
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 0,
                Title = "Home",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 1,
                Title = "Atração",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 2,
                Title = "Hotel",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 3,
                Title = "Refeição",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 4,
                Title = "Loja",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 5,
                Title = "Carro",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 6,
                Title = "Deslocamento",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 7,
                Title = "Gasto",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 8,
                Title = "Aquisição Moeda",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
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
                Title = "Grupo de Cidade",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 11,
                Title = "Comentário",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 12,
                Title = "Cotação Moeda",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 13,
                Title = "Foto",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 14,
                Title = "Lista de Compra",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 15,
                Title = "Consultar Sugestão",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 16,
                Title = "Pedido de Compra",
                IconSource = "Dados.png",
                Visible = false,
                ApenasAmigo = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 17,
                Title = "Sugestão",
                IconSource = "Dados.png",
                Visible = false,
                ApenasAmigo = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 18,
                Title = "Editar Viagem",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 19,
                Title = "Fechar Viagem",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true
            });
            ItensMenuCompleto.Add(new ItemMenu
            {
                Codigo = 20,
                Title = "Iniciar Rota",
                IconSource = "Dados.png",
                Visible = false,
                ApenasParticipante = true,
                ViagemAberta = true
            });

        }

        public async Task ExecutarAcao()
        {
            if (_ItemMenuSelecionado != null)
            {
                if (_ItemMenuSelecionado.Codigo == 0)
                {
                    await OnItemMenuSelecionado(new MenuInicialPage() { BindingContext = new MenuInicialViewModel() }, true);
                }
                else if (_ItemMenuSelecionado.Codigo == 1)
                {
                    await AbrirAtracao();
                }
                else if (_ItemMenuSelecionado.Codigo == 2)
                {
                    await AbrirHotel();
                }
                else if (_ItemMenuSelecionado.Codigo == 3)
                {
                    await AbrirRefeicao();
                }
                else if (_ItemMenuSelecionado.Codigo == 4)
                {
                    await AbrirLoja();
                }
                else if (_ItemMenuSelecionado.Codigo == 8)
                {
                    await AbrirCompraMoeda();
                }
                else if (_ItemMenuSelecionado.Codigo == 10)
                {
                    await AbrirGrupoCidade();
                }
                else if (_ItemMenuSelecionado.Codigo == 11)
                {
                    await AbrirComentario();
                }
                else if (_ItemMenuSelecionado.Codigo == 12)
                {
                    await AbrirCotacaoMoeda();
                }
                else if (_ItemMenuSelecionado.Codigo == 14)
                {
                    await AbrirListaCompra();
                }
                else if (_ItemMenuSelecionado.Codigo == 15)
                {
                    await AbrirConsultaSugestao();
                }
                else if (_ItemMenuSelecionado.Codigo == 16)
                {
                    await AbrirPedidoCompra();
                }
                else if (_ItemMenuSelecionado.Codigo == 17)
                {
                    await AbrirSugestao();
                }
                else if (_ItemMenuSelecionado.Codigo == 18)
                {
                    await EditarViagem();
                }
                else if (_ItemMenuSelecionado.Codigo == 19)
                {
                    await TrocarSituacaoViagem();
                }
                else if (_ItemMenuSelecionado.Codigo == 20)
                {
                    await TrocarControleGPS();
                }
                else
                {
                    Page pagina = new Page();
                    await OnItemMenuSelecionado(pagina, true);
                }
                ItemSelecionado = null;
            }
        }

        private async Task AbrirCotacaoMoeda()
        {
            if (_ItemViagem != null && await VerificarOnline())
            {
                ListagemCotacaoMoedaPage pagina = new ListagemCotacaoMoedaPage() { BindingContext = new ListagemCotacaoMoedaViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina,false);

            }
        }

        private async Task AbrirSugestao()
        {
            if (_ItemViagem != null && await VerificarOnline())
            {
                var pagina = new ListagemSugestaoPage() { BindingContext = new ListagemSugestaoViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina, false);

            }
        }

        private async Task AbrirAtracao()
        {
            if (_ItemViagem != null)
            {
                var pagina = new ListagemAtracaoPage() { BindingContext = new ListagemAtracaoViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina, false);

            }
        }
        private async Task AbrirHotel()
        {
            if (_ItemViagem != null)
            {
                var pagina = new ListagemHotelPage() { BindingContext = new ListagemHotelViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina, false);

            }
        }

        private async Task AbrirComentario()
        {
            if (_ItemViagem != null)
            {
                var pagina = new ListagemComentarioPage() { BindingContext = new ListagemComentarioViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina, false);

            }
        }

        private async Task AbrirRefeicao()
        {
            if (_ItemViagem != null)
            {
                var pagina = new ListagemRefeicaoPage() { BindingContext = new ListagemRefeicaoViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina, false);

            }
        }

        private async Task AbrirLoja()
        {
            if (_ItemViagem != null)
            {
                var pagina = new ListagemLojaPage() { BindingContext = new ListagemLojaViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina, false);

            }
        }
        private async Task AbrirCompraMoeda()
        {
            if (_ItemViagem != null)
            {
                var pagina = new ListagemAporteDinheiroPage() { BindingContext = new ListagemAporteDinheiroViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina, false);

            }
        }

        private async Task AbrirGrupoCidade()
        {
            if (_ItemViagem != null && await VerificarOnline())
            {
                var pagina = new ListagemAgrupamentoCidadePage() { BindingContext = new ListagemAgrupamentoCidadeViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina, false);

            }
        }

        private async Task AbrirPedidoCompra()
        {
            if (_ItemViagem != null && await VerificarOnline())
            {
                ListagemPedidoCompraPage pagina = new ListagemPedidoCompraPage() { BindingContext = new ListagemPedidoCompraViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina, false);

            }
        }

        private async Task AbrirConsultaSugestao()
        {
            if (_ItemViagem != null )
            {
                var pagina = new ListagemSugestaoRecebidaPage() { BindingContext = new ListagemSugestaoRecebidaViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina, false);

            }
        }

        private async Task AbrirListaCompra()
        {
            if (_ItemViagem != null && await VerificarOnline())
            {
                var pagina = new ListagemListaCompraPage() { BindingContext = new ListagemListaCompraViewModel(_ItemViagem) };

                await OnItemMenuSelecionado(pagina, false);

            }
        }

        private async Task TrocarSituacaoViagem()
        {
            if (ItemViagem.Aberto)
            {
                ItemViagem.Aberto = false;
                ItemViagem.ControlaPosicaoGPS = false;
            }
            else
                ItemViagem.Aberto = true;
            using (ApiService srv = new ApiService())
            {
                await srv.SalvarViagem(ItemViagem);
            }
            AjustarVisibilidadeItens();
            ItensMenu = new ObservableCollection<ItemMenu>(ItensMenuCompleto.Where(d => d.Visible));
            OnPropertyChanged("ItensMenu");
        }

        private async Task TrocarControleGPS()
        {
            ItemViagem.ControlaPosicaoGPS = !ItemViagem.ControlaPosicaoGPS;
            var itemMenu = ItensMenu.Where(d => d.Codigo == 20).FirstOrDefault();
            if (ItemViagem.ControlaPosicaoGPS)
                itemMenu.Title = "Parar Rota";
            else
                itemMenu.Title = "Iniciar Rota";
            var VMPai = (MasterDetailViewModel)(Application.Current?.MainPage.BindingContext);
            await VMPai.IniciarControlePosicao();
        }

        private async Task<bool> VerificarOnline()
        {
            bool Online = false;
            using (ApiService srv = new ApiService())
            {
                Online = await srv.VerificarOnLine();
            }
            if (!Online)
                ExibirAlertaOffLine();
            return Online;
        }

        private async Task EditarViagem()
        {
            Viagem itemEditar = null;
            using (ApiService srv = new ApiService())
            {
                itemEditar = await srv.CarregarViagem(_ItemViagem.Identificador);
            }
            if (itemEditar != null && itemEditar.Identificador.HasValue)
            {
                foreach (var itemParticipante in itemEditar.Participantes)
                    itemParticipante.PermiteExcluir = itemParticipante.IdentificadorUsuario != itemEditar.IdentificadorUsuario;
                await OnItemMenuSelecionado(new EditarViagemPage() { BindingContext = new EditarViagemViewModel(itemEditar), Title="Editar" },false);
            }
            else
                ExibirAlertaOffLine();
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
    }
}
