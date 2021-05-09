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


namespace CV.Mobile.ViewModels.Hoteis
{
    public class HotelEdicaoViewModel : BaseViewModel
    {
        private ValidatableObject<string> _nome = new ValidatableObject<string>();
        private ValidatableObject<int> _tamanho = new ValidatableObject<int>();
        private ValidatableObject<DateTime> _dataInicio = new ValidatableObject<DateTime>();
        private ValidatableObject<DateTime> _dataFim = new ValidatableObject<DateTime>();
        private ValidatableObject<TimeSpan> _HoraInicio = new ValidatableObject<TimeSpan>();
        private ValidatableObject<TimeSpan> _HoraFim = new ValidatableObject<TimeSpan>();

        private bool _visitaIniciada = false;
        private bool _visitaConcluida = false;
        private bool _movel = false;
        private int _tabSelecionada = 0;
        private ValidatableObject<string> _comentario = new ValidatableObject<string>();
        private bool _avaliar = false;
        private int _nota = 3;
        private int? _identificador = null;
        private bool _foraHotel = true;
        private bool _permiteTrocaHotel = false;

        private ObservableCollection<Gasto> _gastos = new ObservableCollection<Gasto>();
        private ObservableCollection<HotelEvento> _eventos = new ObservableCollection<HotelEvento>();

        private double? _latitude = null;
        private double? _longitude = null;
        private readonly IGPSService _gps;
        private readonly IApiService _apiService;
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private bool Carregando = false;

        private Hotel Hotel = new Hotel()
        {
            IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador,
            Avaliacoes = new ObservableCollection<HotelAvaliacao>() { new HotelAvaliacao() { IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo, NomeUsuario = GlobalSetting.Instance.UsuarioLogado.Nome } },
            Gastos = new ObservableCollection<GastoHotel>(),
            Movel = false,
            Raio = 0,
            Eventos = new ObservableCollection<HotelEvento>(),

        };

        public HotelEdicaoViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService, IGPSService gPSService)
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
                await PreencherHotel(navigationData);
            }
            finally
            {
                IsBusy = false;
            }

            MessagingCenter.Unsubscribe<GastoEdicaoViewModel, Gasto>(this, MessageKeys.SalvarCusto);
            MessagingCenter.Subscribe<GastoEdicaoViewModel, Gasto>(this, MessageKeys.SalvarCusto, (sender, custo) =>
            {
                GastoHotel gastoHotel = new GastoHotel() { IdentificadorHotel = Hotel.Identificador, ItemGasto = custo, IdentificadorGasto = custo.Identificador, DataAtualizacao = DateTime.UtcNow, Identificador = custo.Atracoes.Select(d => d.Identificador).FirstOrDefault() };
                Hotel.Gastos.Add(gastoHotel);
                Gastos.Add(custo);
            });
            MessagingCenter.Unsubscribe<GastoSelecaoViewModel, Gasto>(this, MessageKeys.SelecionarGasto); ;
            MessagingCenter.Subscribe<GastoSelecaoViewModel, Gasto>(this, MessageKeys.SelecionarGasto, async (sender, custo) =>
            {
                await SelecionarGasto(custo);

            });

            MessagingCenter.Unsubscribe<IGPSService, HotelEvento>(this, MessageKeys.AjustarEventoHotel); ;
            MessagingCenter.Subscribe<IGPSService, HotelEvento>(this, MessageKeys.AjustarEventoHotel, (sender, at) =>
            {
                if (Hotel.Identificador == at.IdentificadorHotel)
                {
                    var hotelEvento = Eventos.Where(d => d.Identificador == at.Identificador).FirstOrDefault();
                    if (hotelEvento == null)
                    {
                        if (!at.DataExclusao.HasValue)
                        {
                            Eventos.Insert(0, at);
                            Hotel.Eventos.Add(hotelEvento);
                        }
                    }
                    else
                    {

                        if (at.DataExclusao.HasValue)
                        {
                            hotelEvento.DataExclusao = at.DataExclusao;
                            Eventos.Remove(hotelEvento);
                        }
                        else
                        {
                            hotelEvento.DataSaida = at.DataSaida;
                            hotelEvento.HoraSaida = at.HoraSaida;
                            hotelEvento.LatitudeSaida = at.LatitudeSaida;
                            hotelEvento.LongitudeSaida = at.LongitudeSaida;
                        }
                    }
                    ForaHotel = !Eventos.Where(d => !d.DataSaida.HasValue).Any();

                }
            });
        }

        private async Task PreencherHotel(object navigationData)
        {
            Hotel item = null;
            if (navigationData != null && navigationData is Hotel)
            {
                item = navigationData as Hotel;
                Hotel = item;
            }

            if (item != null)
            {
                Carregando = true;
                Identificador = item.Identificador;
                item = await _dataService.CarregarHotel(item.Identificador);
                Hotel = item;
                Nome.Value = item.Nome;
                Movel = item.Movel;
                Tamanho.Value = item.Raio.GetValueOrDefault();
                VisitaIniciada = item.DataEntrada.HasValue;
                if (item.DataEntrada.HasValue)
                {
                    DataInicio.Value = item.DataEntrada.Value;
                    HoraInicio.Value = item.HoraEntrada.Value;
                }
                VisitaConcluida = VisitaIniciada && item.DataSaidia.HasValue;
                if (VisitaConcluida)
                {
                    DataFim.Value = item.DataSaidia.Value;
                    HoraFim.Value = item.HoraSaida.Value;
                }
                var itemAvaliacao = item.Avaliacoes.Where(d => d.IdentificadorUsuario == GlobalSetting.Instance.UsuarioLogado.Codigo).FirstOrDefault();
                if (itemAvaliacao != null)
                {
                    Comentario.Value = itemAvaliacao.Comentario;
                    Avaliar = itemAvaliacao.Nota.HasValue;
                    if (Avaliar)
                        Nota = itemAvaliacao.Nota.Value;
                }
                PermiteTrocaHotel = VisitaIniciada;
                Gastos = new ObservableCollection<Gasto>(item.Gastos.Select(d => d.ItemGasto));
                Eventos = new ObservableCollection<HotelEvento>(item.Eventos.Where(d => !d.DataExclusao.HasValue).OrderByDescending(d => d.DataEntrada));
                ForaHotel = !Eventos.Where(d => !d.DataSaida.HasValue).Any();

                _latitude = item.Latitude;
                _longitude = item.Longitude;
                Carregando = false;
            }
            else
            {

            }
        }

        private async Task SelecionarGasto(Gasto custo)
        {
            var itemGravar = new GastoHotel() { IdentificadorHotel = Hotel.Identificador, IdentificadorGasto = custo.Identificador, DataAtualizacao = DateTime.UtcNow };
            bool Executado = false;
            if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
            {
                ResultadoOperacao Resultado = null;

                Resultado = await _apiService.SalvarGastoHotel(itemGravar);
                if (Resultado != null)
                {
                    Executado = true;
                    if (Resultado.Sucesso)
                    {
                        itemGravar.Identificador = Resultado.IdentificadorRegistro;
                        itemGravar.ItemGasto = custo;
                        await _database.SalvarGastoHotel(itemGravar);
                        itemGravar.Identificador = Resultado.IdentificadorRegistro;
                    }
                }
            }
            if (!Executado)
            {

                itemGravar.ItemGasto = custo;
                itemGravar.AtualizadoBanco = false;
                await _database.SalvarGastoHotel(itemGravar);

            }
            Hotel.Gastos.Add(itemGravar);
            Gastos.Add(custo);
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


        public ObservableCollection<Gasto> Gastos
        {
            get { return _gastos; }
            set { SetProperty(ref _gastos, value); }
        }

        public ObservableCollection<HotelEvento> Eventos
        {
            get { return _eventos; }
            set { SetProperty(ref _eventos, value); }
        }
        public bool ForaHotel
        {
            get { return _foraHotel; }
            set { SetProperty(ref _foraHotel, value); }
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
        public bool Movel
        {
            get { return _movel; }
            set { SetProperty(ref _movel, value); }
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
        public ValidatableObject<int> Tamanho
        {
            get { return _tamanho; }
            set { SetProperty(ref _tamanho, value); }
        }
        public ValidatableObject<string> Comentario
        {
            get { return _comentario; }
            set { SetProperty(ref _comentario, value); }
        }

        public bool PermiteTrocaHotel
        {
            get { return _permiteTrocaHotel; }
            set { SetProperty(ref _permiteTrocaHotel, value); }
        }
        public ICommand ValidarNomeCommad => new Command(() => ValidarNome());
        public ICommand ValidarDataInicioCommad => new Command(() => ValidarDataInicio());
        public ICommand ValidarDataFimCommad => new Command(() => ValidarDataFim());
        public ICommand TrocarVisitaIniciadaCommad => new Command(async () => await TrocarVisitaIniciada());
        public ICommand TrocarVisitaConcluidaCommad => new Command(() => TrocarVisitaConcluida());
        public ICommand AdicionarGastoCommand => new Command(async () => await AdicionarGasto());

        public ICommand RemoverGastoCommand => new Command<Gasto>(async (d) => await RemoverGasto(d));

        public ICommand DeixarHotelCommand => new Command<HotelEvento>(async (d) => await DeixarHotel(d));



        public ICommand RetornarHotelCommand => new Command(async () => await RetornarHotel());

        public ICommand TrocarMovelCommad => new Command(async () => await TrocarMovel());

        private async Task TrocarMovel()
        {
            if (Movel)
            {
                _latitude = null;
                _longitude = null;
            }
            else if (!Carregando && VisitaIniciada)
            {
                var posicao = await _gps.RetornarPosicao();
                if (posicao != null)
                {
                    _longitude = posicao.Longitude;
                    _latitude = posicao.Latitude;
                }
            }
        }

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
                if (!Carregando)
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
        private async Task DeixarHotel(HotelEvento d)
        {
            await AjustarEventoSaida(d);
            await SalvarEvento(d);
            ForaHotel = true;

        }


        private async Task RetornarHotel()
        {
            HotelEvento evento = await GerarEventoEntrada();
            Eventos.Insert(0, evento);
            Hotel.Eventos.Add(evento);
            await SalvarEvento(evento);
            ForaHotel = false;
        }

        private async Task SalvarEvento(HotelEvento evento)
        {
            ResultadoOperacao Resultado = new ResultadoOperacao();
            bool SalvaExecucao = false;
            if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
            {

                Resultado = await _apiService.SalvarHotelEvento(evento);
                if (Resultado != null)
                {
                    SalvaExecucao = true;

                    if (Resultado.Sucesso)
                    {
                        evento.Identificador = Resultado.IdentificadorRegistro;
                        await _database.SalvarHotelEvento(evento);
                    }
                }
            }
            if (!SalvaExecucao)
            {
                evento.AtualizadoBanco = false;
                await _database.SalvarHotelEvento(evento);

            }
        }

        private async Task<HotelEvento> GerarEventoEntrada()
        {
            HotelEvento evento = new HotelEvento()
            {
                IdentificadorHotel = Hotel.Identificador,
                IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo,
                DataEntrada = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                HoraEntrada = DateTime.Now.TimeOfDay,
                DataAtualizacao = DateTime.UtcNow
            };
            if (Movel)
            {
                var posicao = await _gps.RetornarPosicao();
                if (posicao != null)
                {
                    evento.LatitudeEntrada = posicao.Latitude;
                    evento.LongitudeEntrada = posicao.Longitude;
                }
            }

            return evento;
        }

        private async Task AjustarEventoSaida(HotelEvento evento)
        {
            evento.DataSaida = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            evento.HoraSaida = DateTime.Now.TimeOfDay;
            evento.DataAtualizacao = DateTime.UtcNow;
            if (Movel)
            {
                var posicao = await _gps.RetornarPosicao();
                if (posicao != null)
                {
                    evento.LatitudeSaida = posicao.Latitude;
                    evento.LongitudeSaida = posicao.Longitude;
                }
            }
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
                    Hotel.Nome = Nome.Value;
                    Hotel.Movel = Movel;
                    Hotel.Raio = Tamanho.Value;

                    if (VisitaIniciada)
                    {
                        Hotel.DataEntrada = DateTime.SpecifyKind(DataInicio.Value.Date.Add(HoraInicio.Value), DateTimeKind.Unspecified);
                        Hotel.HoraEntrada = HoraInicio.Value;
                        if (!Eventos.Any())
                        {
                            var evento = await GerarEventoEntrada();
                            Eventos.Add(evento);
                            Hotel.Eventos.Add(evento);
                        }
                        if (!Movel)
                        {
                            Hotel.Latitude = _latitude;
                            Hotel.Longitude = _longitude;
                        }
                        else
                        {
                            Hotel.Latitude = Hotel.Longitude = null;
                        }
                    }
                    else
                    {
                        Hotel.DataEntrada = null;
                        Hotel.Latitude = Hotel.Longitude = null;

                    }
                    if (VisitaConcluida)
                    {
                        Hotel.DataSaidia = DateTime.SpecifyKind(DataFim.Value.Date.Add(HoraFim.Value), DateTimeKind.Unspecified);
                        Hotel.HoraSaida = HoraFim.Value;
                        var ultimoEvento = Hotel.Eventos.Where(d => !d.DataSaida.HasValue).OrderByDescending(d => d.DataEntrada).FirstOrDefault();
                        if (ultimoEvento != null)
                            await AjustarEventoSaida(ultimoEvento);

                    }
                    else
                    {
                        Hotel.HoraSaida = null;

                    }
                    //Hotel.Eventos = Eventos;
                    var itemAvaliacao = Hotel.Avaliacoes.Where(d => d.IdentificadorUsuario == GlobalSetting.Instance.UsuarioLogado.Codigo).FirstOrDefault();
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

                Resultado = await _apiService.SalvarHotel(Hotel);
                if (Resultado != null)
                {
                    SalvaExecucao = true;
                    if (Resultado.Sucesso)
                    {
                        var idHotel = Hotel.Id;

                        Hotel = await _apiService.CarregarHotel(Resultado.IdentificadorRegistro);
                        if (idHotel.HasValue)
                            Hotel.Id = idHotel;

                        await _dataService.SalvarHotelReplicada(Hotel);
                    }
                }

            }
            if (!SalvaExecucao)
            {
                Resultado = await _dataService.SalvarHotel(Hotel);
                if (Resultado.Sucesso)
                    Hotel = await _dataService.CarregarHotel(Hotel.Identificador);
            }
            if (Resultado.Sucesso)
            {
                PermiteTrocaHotel = VisitaIniciada;
                Identificador = Hotel.Identificador;
                Eventos = new ObservableCollection<HotelEvento>(Hotel.Eventos.Where(d => !d.DataExclusao.HasValue).OrderByDescending(d => d.DataEntrada));
                ForaHotel = !Eventos.Where(d => !d.DataSaida.HasValue).Any();
                MessagingCenter.Send<HotelEdicaoViewModel, Hotel>(this, MessageKeys.SalvarHotel, Hotel);

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
            string Nome = $"{nameof(HotelEdicaoViewModel)}_{MessageKeys.SelecionarPosicao}";
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
                    Hoteis = new ObservableCollection<GastoHotel>() { new GastoHotel() { IdentificadorHotel = Hotel.Identificador, DataAtualizacao = DateTime.UtcNow } }
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
                var obj = Hotel.Gastos.Where(d => d.IdentificadorGasto == g.Identificador).FirstOrDefault();
                if (obj != null)
                {
                    obj.DataExclusao = DateTime.Now.ToUniversalTime();
                    bool Executado = false;
                    if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
                    {
                        Resultado = await _apiService.SalvarGastoHotel(obj);
                        if (Resultado != null)
                        {
                            var itemBase = await _database.RetornarGastoHotel(obj.Identificador);
                            if (itemBase != null)
                                await _database.ExcluirGastoHotel(itemBase);

                            Executado = true;
                        }
                    }
                    if (!Executado)
                    {
                        obj.AtualizadoBanco = false;
                        if (obj.Identificador > 0)
                        {
                            await _database.SalvarGastoHotel(obj);
                        }
                        else
                            await _database.ExcluirGastoHotel(obj);
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


