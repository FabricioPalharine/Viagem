using CV.Mobile.Enums;
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

namespace CV.Mobile.ViewModels.Deslocamentos
{
    public class DeslocamentoEdicaoViewModel : BaseViewModel
    {
        private ValidatableObject<string> _nome = new ValidatableObject<string>();
        private ValidatableObject<string> _companhia = new ValidatableObject<string>();

        private ValidatableObject<DateTime> _dataInicio = new ValidatableObject<DateTime>();
        private ValidatableObject<DateTime> _dataFim = new ValidatableObject<DateTime>();
        private ValidatableObject<TimeSpan> _HoraInicio = new ValidatableObject<TimeSpan>();
        private ValidatableObject<TimeSpan> _HoraFim = new ValidatableObject<TimeSpan>();

        private bool _visitaIniciada = false;
        private bool _visitaConcluida = false;
        private int _tabSelecionada = 0;
        private bool Carregando = false;
        private ValidatableObject<string> _comentario = new ValidatableObject<string>();
        private bool _avaliar = false;
        private int _nota = 3;
        private decimal _distancia = 0;
        private int? _identificador = null;
        private bool _rodando = false;
        private ObservableCollection<ItemLista> _tipos = new ObservableCollection<ItemLista>();
        private ObservableCollection<Gasto> _gastos = new ObservableCollection<Gasto>();
        private ObservableCollection<ViagemAereaAeroporto> _paradas = new ObservableCollection<ViagemAereaAeroporto>();
        private ValidatableObject<ItemLista> _tipo = new ValidatableObject<ItemLista>();

        private double? _latitudeInicio = null;
        private double? _longitudeInicio = null;

        private double? _latitudeFim = null;
        private double? _longitudeFim = null;

        private ValidatableObject<string> _nomePontoInicio = new ValidatableObject<string>();
        private ValidatableObject<string> _nomePontoFim = new ValidatableObject<string>();

        private readonly IGPSService _gps;
        private readonly IApiService _apiService;
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;

        private ViagemAerea deslocamento = new ViagemAerea()
        {
            IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador,
            Avaliacoes = new ObservableCollection<AvaliacaoAerea>() { new AvaliacaoAerea() { IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo, NomeUsuario = GlobalSetting.Instance.UsuarioLogado.Nome } },
            Gastos = new ObservableCollection<GastoViagemAerea>(),
            Aeroportos = new ObservableCollection<ViagemAereaAeroporto>()
            {
                new ViagemAereaAeroporto() { DataAtualizacao = DateTime.UtcNow, TipoPonto = (int)enumTipoParada.Origem},
                new ViagemAereaAeroporto() { DataAtualizacao = DateTime.UtcNow, TipoPonto = (int)enumTipoParada.Destino},
            },
            Distancia = 0,

        };

        public DeslocamentoEdicaoViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService, IGPSService gPSService)
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
            Tipos = Funcoes.RetornarTiposDeslocamento();
            Tipo.Value = Tipos.FirstOrDefault();
            IsBusy = true;
            try
            {
                Carregando = true;
                await PreencherDeslocamento(navigationData);
                Carregando = false;
            }
            finally
            {
                IsBusy = false;
            }

            MessagingCenter.Unsubscribe<GastoEdicaoViewModel, Gasto>(this, MessageKeys.SalvarCusto);
            MessagingCenter.Subscribe<GastoEdicaoViewModel, Gasto>(this, MessageKeys.SalvarCusto, (sender, custo) =>
            {
                GastoViagemAerea GastoViagemAerea = new GastoViagemAerea() { IdentificadorViagemAerea = deslocamento.Identificador, ItemGasto = custo, IdentificadorGasto = custo.Identificador, DataAtualizacao = DateTime.UtcNow, Identificador = custo.Atracoes.Select(d => d.Identificador).FirstOrDefault() };
                deslocamento.Gastos.Add(GastoViagemAerea);
                Gastos.Add(custo);
            });
            MessagingCenter.Unsubscribe<GastoSelecaoViewModel, Gasto>(this, MessageKeys.SelecionarGasto); ;
            MessagingCenter.Subscribe<GastoSelecaoViewModel, Gasto>(this, MessageKeys.SelecionarGasto, async (sender, custo) =>
            {
                await SelecionarGasto(custo);

            });

            MessagingCenter.Unsubscribe<IGPSService, ViagemAerea>(this, MessageKeys.AjustarDistanciaDeslocamento); ;
            MessagingCenter.Subscribe<IGPSService, ViagemAerea>(this, MessageKeys.AjustarDistanciaDeslocamento, (sender, at) =>
            {
                if (deslocamento.Identificador == at.Identificador)
                {
                    Distancia = at.Distancia.GetValueOrDefault(0);
                }
            });
        }

        private async Task PreencherDeslocamento(object navigationData)
        {
            ViagemAerea item = null;
            if (navigationData != null && navigationData is ViagemAerea)
            {
                item = navigationData as ViagemAerea;
                deslocamento = item;
            }

            if (item != null)
            {
                Identificador = item.Identificador;
                item = await _dataService.CarregarViagemAerea(item.Identificador);
                deslocamento = item;
                Nome.Value = item.Descricao;
                Companhia.Value = item.CompanhiaAerea;
                Tipo.Value = Tipos.Where(d => d.Codigo == Convert.ToString(item.Tipo)).FirstOrDefault();
                var itemPartida = deslocamento.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Origem).FirstOrDefault();
                var itemChegada = deslocamento.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Destino).FirstOrDefault();
                NomePartida.Value = itemPartida.Aeroporto;
                NomeDestino.Value = itemChegada.Aeroporto;
                VisitaIniciada = itemPartida.DataChegada.HasValue;
                if (VisitaIniciada)
                {
                    DataInicio.Value = itemPartida.DataChegada.Value.Date;
                    HoraInicio.Value = itemPartida.HoraChegada.Value;
                }
                VisitaConcluida = VisitaIniciada && itemChegada.DataPartida.HasValue;
                if (VisitaConcluida)
                {
                    DataFim.Value = itemChegada.DataPartida.Value.Date;
                    HoraFim.Value = itemChegada.HoraPartida.Value;
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
                Paradas = new ObservableCollection<ViagemAereaAeroporto>(item.Aeroportos.Where(d=>d.TipoPonto==(int) enumTipoParada.Escala).Where(d=>!d.DataExclusao.HasValue).OrderByDescending(d => d.DataChegada));
                _latitudeInicio = itemPartida.Latitude;
                _longitudeInicio = itemPartida.Longitude;

                _latitudeFim = itemChegada.Latitude;
                _longitudeFim = itemChegada.Longitude;
                Rodando = VisitaIniciada && !Paradas.Where(d => !d.DataPartida.HasValue).Any() && !VisitaConcluida;

            }
            else
            {
               

            }
        }

        private async Task SelecionarGasto(Gasto custo)
        {
            var itemGravar = new GastoViagemAerea() { IdentificadorViagemAerea = deslocamento.Identificador, IdentificadorGasto = custo.Identificador, DataAtualizacao = DateTime.UtcNow };
            bool Executado = false;
            if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
            {
                ResultadoOperacao Resultado = null;

                Resultado = await _apiService.SalvarGastoViagemAerea(itemGravar);
                if (Resultado != null)
                {
                    Executado = true;
                    if (Resultado.Sucesso)
                    {
                        itemGravar.Identificador = Resultado.IdentificadorRegistro;
                        itemGravar.ItemGasto = custo;
                        await _database.SalvarGastoViagemAerea(itemGravar);
                        itemGravar.Identificador = Resultado.IdentificadorRegistro;
                    }
                }
            }
            if (!Executado)
            {

                itemGravar.ItemGasto = custo;
                itemGravar.AtualizadoBanco = false;
                await _database.SalvarGastoViagemAerea(itemGravar);

            }
            deslocamento.Gastos.Add(itemGravar);
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
        public ObservableCollection<ItemLista> Tipos
        {
            get { return _tipos; }
            set { SetProperty(ref _tipos, value); }
        }


        public ObservableCollection<Gasto> Gastos
        {
            get { return _gastos; }
            set { SetProperty(ref _gastos, value); }
        }

        public ObservableCollection<ViagemAereaAeroporto> Paradas
        {
            get { return _paradas; }
            set { SetProperty(ref _paradas, value); }
        }
        public bool Rodando
        {
            get { return _rodando; }
            set { SetProperty(ref _rodando, value); }
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
        public ValidatableObject<string> Companhia
        {
            get { return _companhia; }
            set { SetProperty(ref _companhia, value); }
        }
        public ValidatableObject<string> NomePartida
        {
            get { return _nomePontoInicio; }
            set { SetProperty(ref _nomePontoInicio, value); }
        }
        public ValidatableObject<string> NomeDestino
        {
            get { return _nomePontoFim; }
            set { SetProperty(ref _nomePontoFim, value); }
        }

        public ValidatableObject<ItemLista> Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }


        public decimal Distancia
        {
            get { return _distancia; }
            set { SetProperty(ref _distancia, value); }
        }

        public ValidatableObject<string> Comentario
        {
            get { return _comentario; }
            set { SetProperty(ref _comentario, value); }
        }
        public ICommand ValidarNomeCommad => new Command(() => ValidarNome());
        public ICommand ValidarTipoCommad => new Command(() => ValidarTipo());

        public ICommand ValidarDataInicioCommad => new Command(() => ValidarDataInicio());
        public ICommand ValidarDataFimCommad => new Command(() => ValidarDataFim());
        public ICommand TrocarVisitaIniciadaCommad => new Command(async () => await TrocarVisitaIniciada());
        public ICommand TrocarVisitaConcluidaCommad => new Command(() => TrocarVisitaConcluida());
        public ICommand AdicionarGastoCommand => new Command(async () => await AdicionarGasto());

        public ICommand RemoverGastoCommand => new Command<Gasto>(async (d) => await RemoverGasto(d));

        public ICommand DeixarParadaCommand => new Command<ViagemAereaAeroporto>((d) => DeixarParada(d));


        public ICommand IniciarParadaCommand => new Command(async () => await IniciarParada());

        private bool ValidarTipo()
        {
            return Tipo.Validate();
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
                    Rodando = true;
                    var posicao = await _gps.RetornarPosicao();
                    if (posicao != null)
                    {
                        _latitudeInicio = posicao.Latitude;
                        _longitudeInicio = posicao.Longitude;
                    }
                }
            }
            else
            {
                Rodando = false;
                this.VisitaConcluida = false;
            }

        }

        private async void TrocarVisitaConcluida()
        {
            if (VisitaConcluida)
            {
                this.DataFim.Value = DateTime.Today;
                this.HoraFim.Value = DateTime.Now.TimeOfDay;
                if (!Carregando)
                {
                    var posicao = await _gps.RetornarPosicao();
                    if (posicao != null)
                    {
                        _latitudeFim = posicao.Latitude;
                        _longitudeFim = posicao.Longitude;
                    }
                }
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
        private void DeixarParada(ViagemAereaAeroporto d)
        {
            d.DataPartida = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            d.HoraPartida = DateTime.Now.TimeOfDay;
            d.DataAtualizacao = DateTime.UtcNow;
            Rodando = true;
        }


        private async Task IniciarParada()
        {
            ViagemAereaAeroporto evento = await GerarParada();
            Paradas.Insert(0, evento);
            deslocamento.Aeroportos.Add(evento);
            Rodando = false;
        }


        private Task<ViagemAereaAeroporto> GerarParada()
        {
            ViagemAereaAeroporto evento = new ViagemAereaAeroporto()
            {
                IdentificadorViagemAerea = deslocamento.Identificador,
                DataAtualizacao = DateTime.UtcNow,
                DataChegada = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                HoraChegada = DateTime.Now.TimeOfDay,
                TipoPonto = (int)enumTipoParada.Escala
            };
            var task = GerarPosicao(evento);

            return Task.FromResult( evento);
        }

        private async Task GerarPosicao(ViagemAereaAeroporto evento)
        {
            var posicao = await _gps.RetornarPosicao();
            if (posicao != null)
            {
                evento.Latitude = posicao.Latitude;
                evento.Longitude = posicao.Longitude;

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
                deslocamento.Descricao = Nome.Value;
                deslocamento.Tipo = Convert.ToInt32(Tipo.Value.Codigo);
                deslocamento.DataAtualizacao = DateTime.UtcNow;
                deslocamento.CompanhiaAerea = Companhia.Value;
                var itemPartida = deslocamento.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Origem).FirstOrDefault();
                var itemChegada = deslocamento.Aeroportos.Where(d => d.TipoPonto == (int)enumTipoParada.Destino).FirstOrDefault();
                itemPartida.Aeroporto = NomePartida.Value;
                itemChegada.Aeroporto = NomeDestino.Value;
                if (VisitaIniciada)
                {                    
                    deslocamento.DataPrevista = itemPartida.DataChegada = itemPartida.DataPartida = DateTime.SpecifyKind(DataInicio.Value.Date.Add(HoraInicio.Value), DateTimeKind.Unspecified);
                    itemPartida.HoraChegada = itemPartida.HoraPartida = HoraInicio.Value;
                    itemPartida.Latitude = _latitudeInicio;
                    itemPartida.Longitude = _longitudeInicio;

                }
                else
                {
                    itemPartida.Latitude = itemPartida.Longitude = null;
                    itemPartida.DataChegada = itemPartida.DataPartida = null;
                }
                if (VisitaConcluida)
                {
                    itemChegada.DataChegada = itemChegada.DataPartida = DateTime.SpecifyKind(DataFim.Value.Date.Add(HoraFim.Value), DateTimeKind.Unspecified);
                    itemChegada.HoraChegada = itemChegada.HoraPartida = HoraFim.Value;
                    itemChegada.Latitude = _latitudeInicio;
                    itemChegada.Longitude = _longitudeInicio;


                }
                else
                {
                    itemChegada.DataChegada  = null;
                    itemChegada.DataPartida = null;
                    itemChegada.HoraChegada = itemChegada.HoraPartida = null;

                }
                var itemAvaliacao = deslocamento.Avaliacoes.Where(d => d.IdentificadorUsuario == GlobalSetting.Instance.UsuarioLogado.Codigo).FirstOrDefault();
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
            try
            {
                ResultadoOperacao Resultado = new ResultadoOperacao();
                bool SalvaExecucao = false;
                if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
                {
                    var id = deslocamento.Id;
                    var identificador = deslocamento.Identificador;
                    Resultado = await _apiService.SalvarViagemAerea(deslocamento);
                    if (Resultado != null)
                    {
                        SalvaExecucao = true;
                        if (Resultado.Sucesso)
                        {
                            deslocamento = await _apiService.CarregarViagemAerea(Resultado.IdentificadorRegistro);
                            deslocamento.Id = id;
                            await _dataService.SalvarViagemAereaReplicada(deslocamento, identificador);
                        }
                    }

                }
                if (!SalvaExecucao)
                {
                    Resultado = await _dataService.SalvarViagemAerea(deslocamento);
                    if (Resultado.Sucesso)
                        deslocamento = await _dataService.CarregarViagemAerea(deslocamento.Identificador);
                }
                if (Resultado.Sucesso)
                {
                    Identificador = deslocamento.Identificador;
                    Paradas = new ObservableCollection<ViagemAereaAeroporto>(deslocamento.Aeroportos.Where(d => !d.DataExclusao.HasValue).Where(d => d.TipoPonto == (int)enumTipoParada.Escala).OrderByDescending(d => d.DataChegada));
                    Rodando = VisitaIniciada && !Paradas.Where(d => !d.DataPartida.HasValue).Any() && !VisitaConcluida;
                    MessagingCenter.Send<DeslocamentoEdicaoViewModel, ViagemAerea>(this, MessageKeys.SalvarDeslocamento, deslocamento);

                }
                if (Resultado != null)
                    await base.ExibirResultado(Resultado, false);
            }
            catch(Exception ex)
            {

            }
        }

        public ICommand CancelarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("..");
        });

        public ICommand SelecionarMapaCommand => new Command(async () =>
        {
            string[] Pontos = new string[] { AppResource.PontoInicio, AppResource.PontoFim };
            var resultado = await DialogService.ShowActionList(AppResource.PontoAjustar, AppResource.Cancelar, string.Empty, Pontos.ToList());
            if (!string.IsNullOrEmpty(resultado))
            {

                string Nome = $"{nameof(DeslocamentoEdicaoViewModel)}_{MessageKeys.SelecionarPosicao}_{AppResource.PontoInicio}";
                MessagingCenter.Unsubscribe<MapaSelecaoViewModel, PosicaoMapa>(this, Nome);
                Nome = $"{nameof(DeslocamentoEdicaoViewModel)}_{MessageKeys.SelecionarPosicao}_{AppResource.PontoFim}";
                MessagingCenter.Unsubscribe<MapaSelecaoViewModel, PosicaoMapa>(this, Nome);
                PosicaoMapa posicao = new PosicaoMapa()
                {
                    Latitude = resultado == AppResource.PontoInicio ? _latitudeInicio.GetValueOrDefault() : _latitudeFim.GetValueOrDefault(),
                    Longitude = resultado == AppResource.PontoInicio ? _longitudeInicio.GetValueOrDefault() : _longitudeFim.GetValueOrDefault(),
                    NomeMensagem = $"{nameof(DeslocamentoEdicaoViewModel)}_{MessageKeys.SelecionarPosicao}_{resultado}"
                };
                MessagingCenter.Subscribe<MapaSelecaoViewModel, PosicaoMapa>(this, $"{nameof(DeslocamentoEdicaoViewModel)}_{MessageKeys.SelecionarPosicao}_{AppResource.PontoInicio}", (sender, d) =>
                {
                    _latitudeInicio = d.Latitude;
                    _longitudeInicio = d.Longitude;

                });
                MessagingCenter.Subscribe<MapaSelecaoViewModel, PosicaoMapa>(this, $"{nameof(DeslocamentoEdicaoViewModel)}_{MessageKeys.SelecionarPosicao}_{AppResource.PontoFim}", (sender, d) =>
                {
                    _latitudeFim = d.Latitude;
                    _longitudeFim = d.Longitude;

                });
                await NavigationService.TrocarPaginaShell("MapaSelecaoPage", posicao);

            }
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
                    ViagenAereas = new ObservableCollection<GastoViagemAerea>() { new GastoViagemAerea() { IdentificadorViagemAerea = deslocamento.Identificador, DataAtualizacao = DateTime.UtcNow } }
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
                var obj = deslocamento.Gastos.Where(d => d.IdentificadorGasto == g.Identificador).FirstOrDefault();
                if (obj != null)
                {
                    obj.DataExclusao = DateTime.Now.ToUniversalTime();
                    bool Executado = false;
                    if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
                    {
                        Resultado = await _apiService.SalvarGastoViagemAerea(obj);
                        if (Resultado != null)
                        {
                            var itemBase = await _database.RetornarGastoViagemAerea(obj.Identificador);
                            if (itemBase != null)
                                await _database.ExcluirGastoViagemAerea(itemBase);

                            Executado = true;
                        }
                    }
                    if (!Executado)
                    {
                        obj.AtualizadoBanco = false;
                        if (obj.Identificador > 0)
                        {
                            await _database.SalvarGastoViagemAerea(obj);
                        }
                        else
                            await _database.ExcluirGastoViagemAerea(obj);
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



