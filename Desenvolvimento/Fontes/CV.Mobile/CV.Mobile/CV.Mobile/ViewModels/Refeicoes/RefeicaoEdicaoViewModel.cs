using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.GPS;
using CV.Mobile.Services.Settings;
using CV.Mobile.Validations;
using CV.Mobile.ViewModels.Gastos;
using CV.Mobile.ViewModels.Mapas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels.Refeicoes
{
    public class RefeicaoEdicaoViewModel : BaseViewModel
    {
        private ValidatableObject<string> _nome = new ValidatableObject<string>();
        private ValidatableObject<string> _tipo = new ValidatableObject<string>();
        private ValidatableObject<DateTime> _dataInicio = new ValidatableObject<DateTime>();
        private ValidatableObject<DateTime> _dataFim = new ValidatableObject<DateTime>();
        private ValidatableObject<TimeSpan> _HoraInicio = new ValidatableObject<TimeSpan>();
        private ValidatableObject<TimeSpan> _HoraFim = new ValidatableObject<TimeSpan>();

        private ValidatableObject<Atracao> _atracaoPai = new ValidatableObject<Atracao>();
        private bool _visitaConcluida = false;
        private int _tabSelecionada = 0;
        private ObservableCollection<Atracao> _atracoes = new ObservableCollection<Atracao>();
        private ValidatableObject<string> _comentario = new ValidatableObject<string>();
        private ValidatableObject<string> _pedido = new ValidatableObject<string>();

        private bool _avaliar = false;
        private int _nota = 3;
        private decimal _distancia = 0;
        private int? _identificador = null;

        private ObservableCollection<Gasto> _gastos = new ObservableCollection<Gasto>();

        private double? _latitude = null;
        private double? _longitude = null;
        private readonly IGPSService _gps;
        private readonly IApiService _apiService;
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;

        private Refeicao Refeicao = new Refeicao()
        {
            IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador,
            Pedidos = new ObservableCollection<RefeicaoPedido>() { new RefeicaoPedido() { IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo, NomeUsuario = GlobalSetting.Instance.UsuarioLogado.Nome } },
            Gastos = new ObservableCollection<GastoRefeicao>(),
        };

        public RefeicaoEdicaoViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService, IGPSService gPSService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
            _gps = gPSService;
            AdicionarValidacoes();
        }

        public override async Task InitializeAsync(object navigationData)
        {
            
                await PreencherRefeicao(navigationData);


            MessagingCenter.Unsubscribe<GastoEdicaoViewModel, Gasto>(this, MessageKeys.SalvarCusto);
            MessagingCenter.Subscribe<GastoEdicaoViewModel, Gasto>(this, MessageKeys.SalvarCusto, (sender, custo) =>
            {
                GastoRefeicao gastoRefeicao = new GastoRefeicao() { IdentificadorRefeicao = Refeicao.Identificador, ItemGasto = custo, IdentificadorGasto = custo.Identificador, DataAtualizacao = DateTime.UtcNow, Identificador = custo.Atracoes.Select(d => d.Identificador).FirstOrDefault() };
                Refeicao.Gastos.Add(gastoRefeicao);
                Gastos.Add(custo);
            });
            MessagingCenter.Unsubscribe<GastoSelecaoViewModel, Gasto>(this, MessageKeys.SelecionarGasto); ;
            MessagingCenter.Subscribe<GastoSelecaoViewModel, Gasto>(this, MessageKeys.SelecionarGasto, async (sender, custo) =>
            {
                await SelecionarGasto(custo);

            });


        }

        private async Task PreencherRefeicao(object navigationData)
        {
            Refeicao item = null;
            if (navigationData != null && navigationData is Refeicao)
            {
                item = navigationData as Refeicao;
                Refeicao = item;
            }

            await CarregarAtracoesPai();
            if (item != null)
            {
                Identificador = item.Identificador;
                item = await _dataService.CarregarRefeicao(item.Identificador);
                Refeicao = item;
                Nome.Value = item.Nome;
                Tipo.Value = item.Tipo;

                DataInicio.Value = item.Data.Value.Date;
                HoraInicio.Value = item.Hora.Value;

                VisitaConcluida = item.DataTermino.HasValue;
                if (VisitaConcluida)
                {
                    DataFim.Value = item.DataTermino.Value.Date;
                    HoraFim.Value = item.HoraTermino.Value;
                }
                var itemAvaliacao = item.Pedidos.Where(d => d.IdentificadorUsuario == GlobalSetting.Instance.UsuarioLogado.Codigo).FirstOrDefault();
                if (itemAvaliacao != null)
                {
                    Pedido.Value = itemAvaliacao.Pedido;
                    Comentario.Value = itemAvaliacao.Comentario;
                    Avaliar = itemAvaliacao.Nota.HasValue;
                    if (Avaliar)
                        Nota = itemAvaliacao.Nota.Value;
                }
                Gastos = new ObservableCollection<Gasto>(item.Gastos.Select(d => d.ItemGasto));
                if (item.IdentificadorAtracao.HasValue)
                    AtracaoPai.Value = Atracoes.Where(d => d.Identificador == item.Identificador).FirstOrDefault();

                _latitude = item.Latitude;
                _longitude = item.Longitude;
            }
            else
            {
                var itemRefeicaoAberta = Atracoes.Where(d => d.Chegada.HasValue).Where(d => !d.Partida.HasValue).OrderByDescending(d => d.Chegada).ThenByDescending(d => d.HoraChegada).FirstOrDefault();
                if (itemRefeicaoAberta != null)
                {
                    var resultado = await DialogService.ShowConfirmAsync(String.Format(AppResource.RefeicaoAtracaoAberta, itemRefeicaoAberta.Nome), AppResource.Confirmacao, AppResource.Sim, AppResource.Nao);
                    if (resultado)
                    {
                        AtracaoPai.Value = itemRefeicaoAberta;

                    }
                }
                DataInicio.Value = DateTime.Today;
                HoraInicio.Value = DateTime.Now.TimeOfDay;
                var posicao = await _gps.RetornarPosicao();
                if (posicao != null)
                {
                    _latitude = posicao.Latitude;
                    _longitude = posicao.Longitude;
                }
            }
        }

        private async Task SelecionarGasto(Gasto custo)
        {
            var itemGravar = new GastoRefeicao() { IdentificadorRefeicao = Refeicao.Identificador, IdentificadorGasto = custo.Identificador, DataAtualizacao = DateTime.UtcNow };
            bool Executado = false;
            if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
            {
                ResultadoOperacao Resultado = null;

                Resultado = await _apiService.SalvarGastoRefeicao(itemGravar);
                if (Resultado != null)
                {
                    Executado = true;
                    if (Resultado.Sucesso)
                    {
                        itemGravar.Identificador = Resultado.IdentificadorRegistro;
                        itemGravar.ItemGasto = custo;
                        await _database.SalvarGastoRefeicao(itemGravar);
                        itemGravar.Identificador = Resultado.IdentificadorRegistro;
                    }
                }
            }
            if (!Executado)
            {

                itemGravar.ItemGasto = custo;
                itemGravar.AtualizadoBanco = false;
                await _database.SalvarGastoRefeicao(itemGravar);

            }
            Refeicao.Gastos.Add(itemGravar);
            Gastos.Add(custo);
        }

        private async Task CarregarAtracoesPai()
        {
            List<Atracao> ListaDados = new List<Atracao>();
            ListaDados = await _database.ListarAtracao(new CriterioBusca());

            ListaDados = ListaDados.Where(d => !Refeicao.Identificador.HasValue || d.Identificador != Refeicao.Identificador).ToList();
            ListaDados.Insert(0, new Atracao() { Identificador = null, Nome = "Sem Atração Pai" });
            Atracoes = new ObservableCollection<Atracao>(ListaDados);

        }

        public int TabSelecionada
        {
            get { return _tabSelecionada; }
            set { SetProperty(ref _tabSelecionada, value); }
        }

        public int? Identificador
        {
            get { return _identificador; }
            set { SetProperty(ref _identificador, value); }
        }

        public ObservableCollection<Atracao> Atracoes
        {
            get { return _atracoes; }
            set { SetProperty(ref _atracoes, value); }
        }
        public ObservableCollection<Gasto> Gastos
        {
            get { return _gastos; }
            set { SetProperty(ref _gastos, value); }
        }
        public ValidatableObject<Atracao> AtracaoPai
        {
            get { return _atracaoPai; }
            set { SetProperty(ref _atracaoPai, value); }
        }



        public bool VisitaConcluida
        {
            get { return _visitaConcluida; }
            set { SetProperty(ref _visitaConcluida, value); }
        }

        public bool Avaliar
        {
            get { return _avaliar; }
            set { SetProperty(ref _avaliar, value); }
        }

        public int Nota
        {
            get { return _nota; }
            set { SetProperty(ref _nota, value); }
        }

        public decimal Distancia
        {
            get { return _distancia; }
            set { SetProperty(ref _distancia, value); }
        }
        public ValidatableObject<DateTime> DataInicio
        {
            get { return _dataInicio; }
            set { SetProperty(ref _dataInicio, value); }
        }

        public ValidatableObject<DateTime> DataFim
        {
            get { return _dataFim; }
            set { SetProperty(ref _dataFim, value); }
        }
        public ValidatableObject<TimeSpan> HoraFim
        {
            get { return _HoraFim; }
            set { SetProperty(ref _HoraFim, value); }
        }

        public ValidatableObject<TimeSpan> HoraInicio
        {
            get { return _HoraInicio; }
            set { SetProperty(ref _HoraInicio, value); }
        }

        public ValidatableObject<string> Nome
        {
            get { return _nome; }
            set { SetProperty(ref _nome, value); }
        }
        public ValidatableObject<string> Tipo
        {
            get { return _tipo; }
            set { SetProperty(ref _tipo, value); }
        }
        public ValidatableObject<string> Comentario
        {
            get { return _comentario; }
            set { SetProperty(ref _comentario, value); }
        }
        public ValidatableObject<string> Pedido
        {
            get { return _pedido; }
            set { SetProperty(ref _pedido, value); }
        }

        public ICommand ValidarNomeCommad => new Command(() => ValidarNome());
        public ICommand ValidarDataInicioCommad => new Command(() => ValidarDataInicio());
        public ICommand ValidarDataFimCommad => new Command(() => ValidarDataFim());
        public ICommand TrocarVisitaConcluidaCommad => new Command(() => TrocarVisitaConcluida());
        public ICommand AdicionarGastoCommand => new Command(async () => await AdicionarGasto());

        public ICommand RemoverGastoCommand => new Command<Gasto>(async (d) => await RemoverGasto(d));


        private bool ValidarNome()
        {
            return Nome.Validate();
        }

        private bool ValidarDataInicio()
        {
            return DataInicio.Validate();
        }

        private bool ValidarDataFim()
        {
            return DataInicio.Validate();
        }



        private void TrocarVisitaConcluida()
        {
            if (VisitaConcluida)
            {
                this.DataFim.Value = DateTime.Today;
                this.HoraFim.Value = DateTime.Now.TimeOfDay;
            }
        }
        private bool ValidarHoraFim()
        {
            var dataInicio = DateTime.SpecifyKind(DataInicio.Value.Date.Add(HoraInicio.Value), DateTimeKind.Unspecified);
            var dataFim = DateTime.SpecifyKind(DataFim.Value.Date.Add(HoraFim.Value), DateTimeKind.Unspecified);
            bool valido = dataFim >= dataInicio;
            HoraFim.Validations.Clear();
            if (!valido)
            {
                HoraFim.AdicionarErro(AppResource.HoraFimMaiorDataInicio);

            }
            return valido;

        }

        public ICommand SalvarCommand => new Command(async () => await Salvar());

        private async Task Salvar()
        {
            bool valido = ValidarNome() & ValidarDataInicio() &
                (!VisitaConcluida || ValidarDataFim());
            valido = valido && (!VisitaConcluida || ValidarHoraFim());
            if (valido)
            {
                Refeicao.Nome = Nome.Value;
                Refeicao.Tipo = Tipo.Value;
                Refeicao.Latitude = _latitude;
                Refeicao.Longitude = _longitude;
                if (AtracaoPai.Value != null && AtracaoPai.Value.Identificador.HasValue)
                    Refeicao.IdentificadorAtracao = AtracaoPai.Value.Identificador;
                else
                    Refeicao.IdentificadorAtracao = null;

                Refeicao.Data = DateTime.SpecifyKind(DataInicio.Value.Date.Add(HoraInicio.Value), DateTimeKind.Unspecified);
                Refeicao.Hora = HoraInicio.Value;

                if (VisitaConcluida)
                {
                    Refeicao.DataTermino = DateTime.SpecifyKind(DataFim.Value.Date.Add(HoraFim.Value), DateTimeKind.Unspecified);
                    Refeicao.HoraTermino = HoraFim.Value;
                }
                else
                    Refeicao.HoraTermino = null;
                var itemAvaliacao = Refeicao.Pedidos.Where(d => d.IdentificadorUsuario == GlobalSetting.Instance.UsuarioLogado.Codigo).FirstOrDefault();
                if (itemAvaliacao != null)
                {
                    itemAvaliacao.Comentario = Comentario.Value;
                    itemAvaliacao.Pedido = Pedido.Value;
                    if (VisitaConcluida && Avaliar)
                    {
                        itemAvaliacao.Nota = Nota;
                    }
                    else
                        itemAvaliacao.Nota = null;
                    itemAvaliacao.DataAtualizacao = DateTime.UtcNow;
                }

                IsBusy = true;
                try
                {
                    await SalvarRegistro();
                }
                finally
                {
                    IsBusy = false;
                }

            }
        }

        private async Task SalvarRegistro()
        {
            ResultadoOperacao Resultado = new ResultadoOperacao();
            bool SalvaExecucao = false;
            if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
            {
                var id = Refeicao.Id;
                Resultado = await _apiService.SalvarRefeicao(Refeicao);
                if (Resultado != null)
                {
                    SalvaExecucao = true;
                    if (Resultado.Sucesso)
                    {
                        Refeicao = await _apiService.CarregarRefeicao(Resultado.IdentificadorRegistro);
                        Refeicao.Id = id;
                        await _dataService.SalvarRefeicaoReplicada(Refeicao);
                    }
                }

            }
            if (!SalvaExecucao)
            {
                Resultado = await _dataService.SalvarRefeicao(Refeicao);
                if (Resultado.Sucesso)
                    Refeicao = await _dataService.CarregarRefeicao(Refeicao.Identificador);
            }
            if (Resultado.Sucesso)
            {
                Identificador = Refeicao.Identificador;
                MessagingCenter.Send<RefeicaoEdicaoViewModel, Refeicao>(this, MessageKeys.SalvarRestaurante, Refeicao);

            }
            if (Resultado != null)
                await base.ExibirResultado(Resultado, false);

        }

        public ICommand CancelarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("..");
        });

        public ICommand SelecionarMapaCommand => new Command(() =>
        {
            string Nome = $"{nameof(RefeicaoEdicaoViewModel)}_{MessageKeys.SelecionarPosicao}";
            MessagingCenter.Unsubscribe<MapaSelecaoViewModel, PosicaoMapa>(this, Nome);
            PosicaoMapa posicao = new PosicaoMapa()
            {
                Latitude = _latitude.GetValueOrDefault(),
                Longitude = _longitude.GetValueOrDefault(),
                NomeMensagem = Nome
            };
            MessagingCenter.Subscribe<MapaSelecaoViewModel, PosicaoMapa>(this, Nome, (sender, d) =>
            {
                _latitude = d.Latitude;
                _longitude = d.Longitude;

            });
            NavigationService.TrocarPaginaShell("MapaSelecaoPage", posicao);

        });

        private async Task AdicionarGasto()
        {
            string[] Opcoes = new string[] { AppResource.NovoCusto, AppResource.CustoExistente };
            var acao = await DialogService.ShowActionList(AppResource.TipoCustoAdicionar, AppResource.Cancelar, string.Empty, Opcoes.ToList());
            if (acao == AppResource.NovoCusto)
            {
                Gasto gasto = new Gasto()
                {
                    Moeda = GlobalSetting.Instance.ViagemSelecionado.Moeda,
                    IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador,
                    IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo,
                    Refeicoes = new ObservableCollection<GastoRefeicao>() { new GastoRefeicao() { IdentificadorRefeicao = Refeicao.Identificador, DataAtualizacao = DateTime.UtcNow } }
                };
                await NavigationService.TrocarPaginaShell("GastoEdicaoPage", gasto);

            }
            else if (acao == AppResource.CustoExistente)
            {
                await NavigationService.TrocarPaginaShell("GastoSelecaoPage", MessageKeys.SelecionarGasto);

            }
        }
        private async Task RemoverGasto(Gasto g)
        {
            if (await DialogService.ShowConfirmAsync(string.Format(AppResource.ExcluirGasto, g.Descricao, g.SiglaMoeda, g.Valor.GetValueOrDefault().ToString("N2")),
                    AppResource.Confirmacao,
                    AppResource.Confirmar, AppResource.Cancelar))
            {
                ResultadoOperacao Resultado = new ResultadoOperacao();
                var obj = Refeicao.Gastos.Where(d => d.IdentificadorGasto == g.Identificador).FirstOrDefault();
                if (obj != null)
                {
                    obj.DataExclusao = DateTime.Now.ToUniversalTime();
                    bool Executado = false;
                    if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
                    {
                        Resultado = await _apiService.SalvarGastoRefeicao(obj);
                        if (Resultado != null)
                        {
                            var itemBase = await _database.RetornarGastoRefeicao(obj.Identificador);
                            if (itemBase != null)
                                await _database.ExcluirGastoRefeicao(itemBase);

                            Executado = true;
                        }
                    }
                    if (!Executado)
                    {
                        obj.AtualizadoBanco = false;
                        if (obj.Identificador > 0)
                        {
                            await _database.SalvarGastoRefeicao(obj);
                        }
                        else
                            await _database.ExcluirGastoRefeicao(obj);
                        Resultado.Mensagens = new MensagemErro[] { new MensagemErro() { Mensagem = AppResource.ExclusaoSucesso } };

                    }
                    if (Resultado != null)
                        await base.ExibirResultado(Resultado);

                }
                Gastos.Remove(g);
            }
        }

        private void AdicionarValidacoes()
        {
            Nome.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = AppResource.CampoObrigatorio });
            DataInicio.Validations.Add(new MaiorZeroRule<DateTime> { ValidationMessage = AppResource.CampoObrigatorio });
            DataFim.Validations.Add(new MaiorDataRule<DateTime> { ValidationMessage = AppResource.MaiorDataInicio, DataLimite = DataInicio });

        }

    }
}

