using CV.Mobile.Enums;
using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.GPS;
using CV.Mobile.Services.Settings;
using CV.Mobile.Validations;
using CV.Mobile.ViewModels.Mapas;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace CV.Mobile.ViewModels.Gastos
{
    public class GastoEdicaoViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private ValidatableObject<DateTime> _data = new ValidatableObject<DateTime>();
        private ValidatableObject<TimeSpan> _hora = new ValidatableObject<TimeSpan>();
        private ValidatableObject<string> _descricao = new ValidatableObject<string>();
        private ValidatableObject<ItemLista> _moeda = new ValidatableObject<ItemLista>();
        private ValidatableObject<decimal> _valor = new ValidatableObject<decimal>();
        private bool _dividido = false;
        private bool _especie = false;
        private ObservableCollection<ItemLista> _moedas = new ObservableCollection<ItemLista>();
        private ObservableCollection<Usuario> _amigos = new ObservableCollection<Usuario>();
        private double? _latitude = null;
        private double? _longitude = null;
        private int _indiceTab = 0;
        private readonly IGPSService _gps;

        private Gasto gasto = new Gasto() { IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador, IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo };

        public GastoEdicaoViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService, IGPSService gPSService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
            _gps = gPSService;
            AdicionarValidacoes();
            Data.Value = DateTime.Today;
            Hora.Value = DateTime.Now.TimeOfDay;
            Moedas = Funcoes.RetornarMoedas();
            Moeda.Value = Moedas.Where(d => d.Codigo == GlobalSetting.Instance.ViagemSelecionado.Moeda.GetValueOrDefault().ToString()).FirstOrDefault();

        }
        public override async Task InitializeAsync(object navigationData)
        {
            Moeda.Value = Moedas.Where(d => d.Codigo == GlobalSetting.Instance.ViagemSelecionado.Moeda.GetValueOrDefault().ToString()).FirstOrDefault();

            if (navigationData != null && navigationData is Gasto item)
            {
                if (item.Identificador.HasValue)
                {

                    item = await _dataService.CarregarGasto(item.Identificador);


                    Descricao.Value = item.Descricao;
                    Data.Value = item.Data.GetValueOrDefault();
                    Hora.Value = item.Hora.GetValueOrDefault();
                    Moeda.Value = Moedas.Where(d => d.Codigo == item.Moeda.GetValueOrDefault().ToString()).FirstOrDefault();
                    Valor.Value = item.Valor.GetValueOrDefault();
                    Dividido = item.Dividido;
                    Especie = item.Especie;
                    _latitude = item.Latitude;
                    _longitude = item.Longitude;
                }
                gasto = item;
            }
            else
            {
                if (GlobalSetting.Instance.ViagemSelecionado.DataInicio >= DateTime.Today &&
                    GlobalSetting.Instance.ViagemSelecionado.DataFim >= DateTime.Today &&
                    GlobalSetting.Instance.UsuarioLogado.Aberto)
                {
                    var posicao = await _gps.RetornarPosicao();
                    if (posicao != null)
                    {
                        _latitude = posicao.Latitude;
                        _longitude = posicao.Longitude;
                    }
                }

            }
            List<Usuario> usuarios = new List<Usuario>();
            if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
            {
                usuarios = await _apiService.ListarParticipantesViagem();
            }
            else
            {
                usuarios = await _database.ListarParticipanteViagem();
            }
            usuarios.Where(d => gasto.Usuarios.Where(e => e.IdentificadorUsuario == d.Identificador).Any()).ForEach(d =>
                {
                    d.Selecionado = true;
                });
            Amigos = new ObservableCollection<Usuario>(usuarios.Where(d => d.Identificador != GlobalSetting.Instance.UsuarioLogado.Codigo).ToList());

        }


        public ValidatableObject<string> Descricao
        {
            get { return _descricao; }
            set { SetProperty(ref _descricao, value); }
        }
        public ValidatableObject<DateTime> Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value); }
        }
        public ValidatableObject<TimeSpan> Hora
        {
            get { return _hora; }
            set { SetProperty(ref _hora, value); }
        }

        public ValidatableObject<ItemLista> Moeda
        {
            get { return _moeda; }
            set { SetProperty(ref _moeda, value); }
        }

        public ObservableCollection<ItemLista> Moedas
        {
            get { return _moedas; }
            set { SetProperty(ref _moedas, value); }
        }
        public ValidatableObject<decimal> Valor
        {
            get { return _valor; }
            set { SetProperty(ref _valor, value); }
        }

        public int TabSelecionada
        {
            get { return _indiceTab; }
            set { SetProperty(ref _indiceTab, value); }
        }

        public ObservableCollection<Usuario> Amigos
        {
            get { return _amigos; }
            set { SetProperty(ref _amigos, value); }
        }
        public bool Dividido
        {
            get { return _dividido; }
            set { SetProperty(ref _dividido, value); }
        }
        public bool Especie
        {
            get { return _especie; }
            set { SetProperty(ref _especie, value); }
        }

        public ICommand ValidarDescricaoCommad => new Command(() => ValidarDescricao());
        public ICommand ValidarDataCommad => new Command(() => ValidarData());
        public ICommand ValidarHoraCommad => new Command(() => ValidarHora());
        public ICommand ValidarValorCommad => new Command(() => ValidarValor());
        public ICommand ValidarMoedaCommand => new Command(() => ValidarMoeda());
        public ICommand SalvarCommand => new Command(async () => await Salvar());

        public ICommand CancelarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("..");
        });


        private bool ValidarDescricao()
        {
            return Descricao.Validate();
        }
        private bool ValidarMoeda()
        {
            return Moeda.Validate();
        }

        private bool ValidarValor()
        {
            return Valor.Validate();
        }

        private bool ValidarData()
        {
            return Data.Validate();
        }

        private bool ValidarHora()
        {
            return Hora.Validate();
        }
        private void AdicionarValidacoes()
        {
            //Hora.Validations.Add(new IsNotNullOrEmptyRule<TimeSpan> { ValidationMessage = AppResource.CampoObrigatorio });
            Data.Validations.Add(new MaiorZeroRule<DateTime> { ValidationMessage = AppResource.CampoObrigatorio });
            Descricao.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = AppResource.CampoObrigatorio });
            Valor.Validations.Add(new MaiorZeroRule<decimal> { ValidationMessage = AppResource.CampoObrigatorio });
            Moeda.Validations.Add(new IsNotNullOrEmptyRule<ItemLista>() { ValidationMessage = AppResource.CampoObrigatorio });

        }

        private async Task Salvar()
        {
            if (ValidarDescricao() && ValidarData() && ValidarHora() && ValidarValor() && ValidarMoeda())
            {
                IsBusy = true;
                try
                {
                    bool Executado = false;
                    gasto.Descricao = Descricao.Value;
                    gasto.Data = DateTime.SpecifyKind(Data.Value.Date.Add(Hora.Value), DateTimeKind.Unspecified);
                    gasto.Hora = Hora.Value;
                    gasto.Latitude = _latitude;
                    gasto.Longitude = _longitude;
                    gasto.Valor = Valor.Value;
                    gasto.Dividido = Dividido;
                    gasto.Moeda = Convert.ToInt32(Moeda.Value.Codigo);
                    gasto.Especie = Especie;
                    if (gasto.Especie)
                        gasto.DataPagamento = null;
                    else
                        gasto.DataPagamento = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
                    if (gasto.Dividido)
                    {
                        foreach (var participanteRemover in gasto.Usuarios.Where(d => !Amigos.Where(f => f.Selecionado).Where(f => d.IdentificadorUsuario == f.Identificador).Any()).ToList())
                            gasto.Usuarios.Remove(participanteRemover);
                        foreach (var participanteAdicionar in Amigos.Where(f => f.Selecionado).Where(f => !gasto.Usuarios.Where(d => d.IdentificadorUsuario == f.Identificador).Any()).ToList())
                        {
                            gasto.Usuarios.Add(new GastoDividido()
                            {
                                IdentificadorUsuario = participanteAdicionar.Identificador
                            });
                        }

                    }
                    else
                        gasto.Usuarios = new ObservableCollection<GastoDividido>();
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
                    {
                        Resultado = await _apiService.SalvarGasto(gasto);
                        if (Resultado != null)
                        {
                            var id = gasto.Id;
                            Executado = true;
                            var Jresultado = (JObject)Resultado.ItemRegistro;
                            gasto = Jresultado.ToObject<Gasto>();
                            gasto.Id = id;
                            _dataService.SalvarGastoSincronizado(gasto);
                        }
                    }
                    if (!Executado)
                    {

                        Resultado = await _dataService.SalvarGasto(gasto);
                    }
                    if (Resultado.Sucesso)
                        MessagingCenter.Send<GastoEdicaoViewModel, Gasto>(this, MessageKeys.SalvarCusto, gasto);
                    await base.ExibirResultado(Resultado, true);

                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}

