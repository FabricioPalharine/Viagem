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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;


namespace CV.Mobile.ViewModels.Comentarios
{
    public class ComentarioEdicaoViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private ValidatableObject<DateTime> _data = new ValidatableObject<DateTime>();
        private ValidatableObject<TimeSpan> _hora = new ValidatableObject<TimeSpan>();
        private ValidatableObject<string> _texto = new ValidatableObject<string>();
        private double? _latitude = null;
        private double? _longitude = null;
        private readonly IGPSService _gps;

        private Comentario comentario = new Comentario() { IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador, IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo };

        public ComentarioEdicaoViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService, IGPSService gPSService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
            _gps = gPSService;
            AdicionarValidacoes();
            Data.Value = DateTime.Today;
            Hora.Value = DateTime.Now.TimeOfDay;

        }
        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData != null && navigationData is Comentario item)
            {
                comentario = item;
                Texto.Value = item.Texto;
                Data.Value = item.Data.GetValueOrDefault();
                Hora.Value = item.Hora.GetValueOrDefault();
                _latitude = item.Latitude;
                _longitude = item.Longitude;

            }
            else
            {
                var posicao = await _gps.RetornarPosicao();
                if (posicao != null)
                {
                    _latitude = posicao.Latitude;
                    _longitude = posicao.Longitude;
                }

            }
        }


        public ValidatableObject<string> Texto
        {
            get { return _texto; }
            set { SetProperty(ref _texto, value); }
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

        public ICommand ValidarTextoCommad => new Command(() => ValidarTexto());
        public ICommand ValidarDataCommad => new Command(() => ValidarData());
        public ICommand ValidarHoraCommad => new Command(() => ValidarHora());

        public ICommand SalvarCommand => new Command(async () => await Salvar());

        public ICommand CancelarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("..");
        });

        public ICommand SelecionarMapaCommand => new Command(() =>
       {
           string Nome = $"{nameof(ComentarioEdicaoViewModel)}_{MessageKeys.SelecionarPosicao}";
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

        private bool ValidarTexto()
        {
            return Texto.Validate();
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
            Texto.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = AppResource.CampoObrigatorio });

        }

        private async Task Salvar()
        {
            if (ValidarTexto() && ValidarData() && ValidarHora())
            {
                IsBusy = true;
                try
                {
                    bool Executado = false;
                    comentario.Texto = Texto.Value;
                    comentario.Data = DateTime.SpecifyKind(Data.Value.Date.Add(Hora.Value), DateTimeKind.Unspecified);
                    comentario.Hora = Hora.Value;
                    comentario.Latitude = _latitude;
                    comentario.Longitude = _longitude;
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
                    {
                        Resultado = await _apiService.SalvarComentario(comentario);
                        if (Resultado != null)
                        {
                            var id = comentario.Id;
                            Executado = true;
                            comentario = await _apiService.CarregarComentario(comentario.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()));
                            comentario.Id = id;
                            comentario.DataAtualizacao = DateTime.Now.ToUniversalTime();
                            await _database.SalvarComentario(comentario);
                        }
                    }
                    if (!Executado)
                    {
                        comentario.DataAtualizacao = DateTime.Now.ToUniversalTime();
                        comentario.AtualizadoBanco = false;
                        Resultado = await _dataService.SalvarComentario(comentario);
                    }
                    if (Resultado.Sucesso)
                        MessagingCenter.Send<ComentarioEdicaoViewModel, Comentario>(this, MessageKeys.SalvarComentario, comentario);
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
