using CV.Mobile.Models;
using CV.Mobile.Views;
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
        private ItemMenu _ItemMenuSelecionado;
        private Viagem _ItemViagem;

        public delegate Task ItemMenuSelecionadoDelegate(Page pagina);
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
            }
        }

        public async Task OnItemMenuSelecionado(Page pagina)
        {
            if (ItemMenuSelecionado != null)
                await ItemMenuSelecionado(pagina);
        }

        public MenuViewModel(UsuarioLogado itemUsuario)
        {
            _ItemUsuario = itemUsuario;
            ItensMenu = new ObservableCollection<ItemMenu>();
            CarregarItensMenu();
        }

        private void CarregarItensMenu()
        {
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 0,
                Title = "Home",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 1,
                Title = "Atração",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 2,
                Title = "Hotel",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 3,
                Title = "Refeição",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 4,
                Title = "Loja",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 5,
                Title = "Carro",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 6,
                Title = "Deslocamento",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 7,
                Title = "Gasto",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 8,
                Title = "Moeda Estrangeira",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 9,
                Title = "Calendário Previsto",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 10,
                Title = "Grupo de Cidade",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 11,
                Title = "Comentário",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 12,
                Title = "Cotação Moeda",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 13,
                Title = "Foto",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 14,
                Title = "Lista de Compra",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 15,
                Title = "Consultar Sugestão",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 16,
                Title = "Pedido de Compra",
                IconSource = "Dados.png",
                Visible = true
            });
            ItensMenu.Add(new ItemMenu
            {
                Codigo = 17,
                Title = "Sugestão",
                IconSource = "Dados.png",
                Visible = true
            });
            
        }

        public async Task ExecutarAcao()
        {
            if (_ItemMenuSelecionado != null)
            {
                if (_ItemMenuSelecionado.Codigo == 0)
                {
                    await OnItemMenuSelecionado(new MenuInicialPage() { BindingContext = new MenuInicialViewModel() });
                }
                else
                {
                    Page pagina = new Page();
                    await OnItemMenuSelecionado(pagina);
                }
                ItemSelecionado = null;
            }
        }
    }
}
