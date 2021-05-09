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

namespace CV.Mobile.ViewModels.Atracoes
{
    public class AtracaoEdicaoViewModel : BaseViewModel
    {
        private ValidatableObject<string> _nome = new ValidatableObject<string>();
        private ValidatableObject<string> _tipo = new ValidatableObject<string>();
        private ValidatableObject<DateTime> _dataInicio = new ValidatableObject<DateTime>();
        private ValidatableObject<DateTime> _dataFim = new ValidatableObject<DateTime>();
        private ValidatableObject<TimeSpan> _HoraInicio = new ValidatableObject<TimeSpan>();
        private ValidatableObject<TimeSpan> _HoraFim = new ValidatableObject<TimeSpan>();

        private ValidatableObject<Atracao> _atracaoPai = new ValidatableObject<Atracao>();
        private bool _visitaIniciada = false;
        private bool _visitaConcluida = false;
        private int _tabSelecionada = 0;
        private ObservableCollection<Atracao> _atracoes = new ObservableCollection<Atracao>();
        private ValidatableObject<string> _comentario = new ValidatableObject<string>();
        private bool _avaliar = false;
        private bool carregando = false;
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

        private Atracao atracao = new Atracao()
        {
            IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador,
            Avaliacoes = new ObservableCollection<AvaliacaoAtracao>() { new AvaliacaoAtracao() { IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo, NomeUsuario = GlobalSetting.Instance.UsuarioLogado.Nome } },
            Gastos = new ObservableCollection<GastoAtracao>(),
            Distancia = 0
        };

        public AtracaoEdicaoViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService, IGPSService gPSService)
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
            IsBusy = true;
            try
            {
                carregando = true;
                await PreencherAtracao(navigationData);
                carregando = false;
            }
            finally
            {
                IsBusy = false;
            }

            MessagingCenter.Unsubscribe<GastoEdicaoViewModel, Gasto>(this, MessageKeys.SalvarCusto);
            MessagingCenter.Subscribe<GastoEdicaoViewModel, Gasto>(this, MessageKeys.SalvarCusto, (sender, custo) =>
            {
                GastoAtracao gastoAtracao = new GastoAtracao() { IdentificadorAtracao = atracao.Identificador, ItemGasto = custo, IdentificadorGasto = custo.Identificador, DataAtualizacao = DateTime.UtcNow, Identificador = custo.Atracoes.Select(d => d.Identificador).FirstOrDefault() };
                atracao.Gastos.Add(gastoAtracao);
                Gastos.Add(custo);
            });
            MessagingCenter.Unsubscribe<GastoSelecaoViewModel, Gasto>(this, MessageKeys.SelecionarGasto); ;
            MessagingCenter.Subscribe<GastoSelecaoViewModel, Gasto>(this, MessageKeys.SelecionarGasto, async (sender, custo) =>
            {
                await SelecionarGasto(custo);

            });

            MessagingCenter.Unsubscribe<IGPSService, Atracao>(this, MessageKeys.AjustarDistanciaAtracao); ;
            MessagingCenter.Subscribe<IGPSService, Atracao>(this, MessageKeys.AjustarDistanciaAtracao, (sender, at) =>
           {
               if (atracao.Identificador == at.Identificador)
                   Distancia = at.Distancia.GetValueOrDefault();

           });
        }

        private async Task PreencherAtracao(object navigationData)
        {
            Atracao item = null;
            if (navigationData != null && navigationData is Atracao)
            {
                item = navigationData as Atracao;
                atracao = item;
            }

            await CarregarAtracoesPai();
            if (item != null)
            {
                Identificador = item.Identificador;
                item = await _dataService.CarregarAtracao(item.Identificador);
                atracao = item;
                Nome.Value = item.Nome;
                Tipo.Value = item.Tipo;
                VisitaIniciada = item.Chegada.HasValue;
                if (item.Chegada.HasValue)
                {
                    DataInicio.Value = item.Chegada.Value;
                    HoraInicio.Value = item.HoraChegada.Value;
                }
                VisitaConcluida = VisitaIniciada && item.Partida.HasValue;
                if (VisitaConcluida)
                {
                    DataFim.Value = item.Partida.Value;
                    HoraFim.Value = item.HoraPartida.Value;
                }
                var itemAvaliacao = item.Avaliacoes.Where(d => d.IdentificadorUsuario == GlobalSetting.Instance.UsuarioLogado.Codigo).FirstOrDefault();
                if (itemAvaliacao != null)
                {
                    Comentario.Value = itemAvaliacao.Comentario;
                    Avaliar = itemAvaliacao.Nota.HasValue;
                    if (Avaliar)
                        Nota = itemAvaliacao.Nota.Value;
                }
                Gastos = new ObservableCollection<Gasto>(item.Gastos.Select(d => d.ItemGasto));
                if (item.IdentificadorAtracaoPai.HasValue)
                    AtracaoPai.Value = Atracoes.Where(d => d.Identificador == item.Identificador).FirstOrDefault();

                Distancia = item.Distancia.GetValueOrDefault(0);
                _latitude = item.Latitude;
                _longitude = item.Longitude;
            }
            else
            {
                var itemAtracaoAberta = Atracoes.Where(d => d.Chegada.HasValue).Where(d => !d.Partida.HasValue).OrderByDescending(d => d.Chegada).ThenByDescending(d => d.HoraChegada).FirstOrDefault();
                if (itemAtracaoAberta != null)
                {
                    var resultado = await DialogService.ShowConfirmAsync(String.Format(AppResource.AtracaoAberta, itemAtracaoAberta.Nome), AppResource.Confirmacao, AppResource.Sim, AppResource.Nao);
                    if (resultado)
                    {
                        AtracaoPai.Value = itemAtracaoAberta;

                    }
                }
            }
        }

        private async Task SelecionarGasto(Gasto custo)
        {
            var itemGravar = new GastoAtracao() { IdentificadorAtracao = atracao.Identificador, IdentificadorGasto = custo.Identificador, DataAtualizacao = DateTime.UtcNow };
            bool Executado = false;
            if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
            {
                ResultadoOperacao Resultado = null;

                Resultado = await _apiService.SalvarGastoAtracao(itemGravar);
                if (Resultado != null)
                {
                    Executado = true;
                    if (Resultado.Sucesso)
                    {
                        itemGravar.Identificador = Resultado.IdentificadorRegistro;
                        itemGravar.ItemGasto = custo;
                        await _database.SalvarGastoAtracao(itemGravar);
                        itemGravar.Identificador = Resultado.IdentificadorRegistro;
                    }
                }
            }
            if (!Executado)
            {

                itemGravar.ItemGasto = custo;
                itemGravar.AtualizadoBanco = false;
                await _database.SalvarGastoAtracao(itemGravar);

            }
            atracao.Gastos.Add(itemGravar);
            Gastos.Add(custo);
        }

        private async Task CarregarAtracoesPai()
        {
            List<Atracao> ListaDados = new List<Atracao>();
            ListaDados = await _database.ListarAtracao(new CriterioBusca());

            ListaDados = ListaDados.Where(d => !atracao.Identificador.HasValue || d.Identificador != atracao.Identificador).ToList();
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

        public bool VisitaIniciada
        {
            get { return _visitaIniciada; }
            set { SetProperty(ref _visitaIniciada, value); }
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
        public ICommand ValidarNomeCommad => new Command(() => ValidarNome());
        public ICommand ValidarDataInicioCommad => new Command(() => ValidarDataInicio());
        public ICommand ValidarDataFimCommad => new Command(() => ValidarDataFim());
        public ICommand TrocarVisitaIniciadaCommad => new Command(async () => await TrocarVisitaIniciada());
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

        private async Task TrocarVisitaIniciada()
        {
            if (VisitaIniciada)
            {
                this.DataInicio.Value = DateTime.Today;
                this.HoraInicio.Value = DateTime.Now.TimeOfDay;
                if (!carregando)
                {
                    var posicao = await _gps.RetornarPosicao();
                    if (posicao != null)
                    {
                        _latitude = posicao.Latitude;
                        _longitude = posicao.Longitude;
                    }
                }
            }
            else
                this.VisitaConcluida = false;

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

            bool valido = ValidarNome() &
                (!VisitaIniciada || ValidarDataInicio()) &
                (!VisitaConcluida || ValidarDataFim());
            valido = valido && (!VisitaConcluida || ValidarHoraFim());
            if (valido)
            {
                atracao.Nome = Nome.Value;
                atracao.Tipo = Tipo.Value;
                atracao.Latitude = _latitude;
                atracao.Longitude = _longitude;
                if (AtracaoPai.Value != null && AtracaoPai.Value.Identificador.HasValue)
                    atracao.IdentificadorAtracaoPai = AtracaoPai.Value.Identificador;
                else
                    atracao.IdentificadorAtracaoPai = null;
                if (VisitaIniciada)
                {
                    atracao.Chegada = DateTime.SpecifyKind(DataInicio.Value.Date.Add(HoraInicio.Value), DateTimeKind.Unspecified);
                    atracao.HoraChegada = HoraInicio.Value;
                }
                else
                    atracao.Chegada = null;
                if (VisitaConcluida)
                {
                    atracao.Partida = DateTime.SpecifyKind(DataFim.Value.Date.Add(HoraFim.Value), DateTimeKind.Unspecified);
                    atracao.HoraPartida = HoraFim.Value;
                }
                else
                    atracao.Partida = null;
                var itemAvaliacao = atracao.Avaliacoes.Where(d => d.IdentificadorUsuario == GlobalSetting.Instance.UsuarioLogado.Codigo).FirstOrDefault();
                if (itemAvaliacao != null)
                {
                    itemAvaliacao.Comentario = Comentario.Value;
                    if (VisitaConcluida && Avaliar)
                    {
                        itemAvaliacao.Nota = Nota;
                    }
                    else
                        itemAvaliacao.Nota = null;
                    itemAvaliacao.DataAtualizacao = DateTime.UtcNow;
                }
                atracao.Distancia = Distancia;

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
                var id = atracao.Id;
                Resultado = await _apiService.SalvarAtracao(atracao);
                if (Resultado != null)
                {
                    SalvaExecucao = true;
                    if (Resultado.Sucesso)
                    {
                        atracao = await _apiService.CarregarAtracao(Resultado.IdentificadorRegistro);
                        if (id.HasValue)
                            atracao.Id = id;
                        await _dataService.SalvarAtracaoReplicada(atracao);
                    }
                }

            }
            if (!SalvaExecucao)
            {
                Resultado = await _dataService.SalvarAtracao(atracao);
                if (Resultado.Sucesso)
                    atracao = await _dataService.CarregarAtracao(atracao.Identificador);
            }
            if (Resultado.Sucesso)
            {
                Identificador = atracao.Identificador;
                MessagingCenter.Send<AtracaoEdicaoViewModel, Atracao>(this, MessageKeys.SalvarAtracao, atracao);

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
            string Nome = $"{nameof(AtracaoEdicaoViewModel)}_{MessageKeys.SelecionarPosicao}";
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
                    IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador,
                    IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo,
                    Moeda = GlobalSetting.Instance.ViagemSelecionado.Moeda,
                    Atracoes = new ObservableCollection<GastoAtracao>() { new GastoAtracao() { IdentificadorAtracao = atracao.Identificador, DataAtualizacao = DateTime.UtcNow } }
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
                var obj = atracao.Gastos.Where(d => d.IdentificadorGasto == g.Identificador).FirstOrDefault();
                if (obj != null)
                {
                    obj.DataExclusao = DateTime.Now.ToUniversalTime();
                    bool Executado = false;
                    if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline) && obj.Identificador > 0)
                    {
                        Resultado = await _apiService.SalvarGastoAtracao(obj);
                        if (Resultado != null)
                        {
                            var itemBase = await _database.RetornarGastoAtracao(obj.Identificador);
                            if (itemBase != null)
                                await _database.ExcluirGastoAtracao(itemBase);

                            Executado = true;
                        }
                    }
                    if (!Executado)
                    {
                        obj.AtualizadoBanco = false;
                        if (obj.Identificador > 0)
                        {
                            await _database.SalvarGastoAtracao(obj);
                        }
                        else
                            await _database.ExcluirGastoAtracao(obj);
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
