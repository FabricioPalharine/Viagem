using CV.Mobile.Enums;
using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Services.Settings;
using CV.Mobile.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;


namespace CV.Mobile.ViewModels.Moedas
{
    public class MoedaCotacaoEdicaoViewModel: BaseViewModel
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private ValidatableObject<DateTime> _dataCotacao = new ValidatableObject<DateTime>();
        private ValidatableObject<ItemLista> _moeda = new ValidatableObject<ItemLista>();
        private ValidatableObject<decimal> _valorCotacao = new ValidatableObject<decimal>();
        private ObservableCollection<ItemLista> _moedas = new ObservableCollection<ItemLista>();
        private CotacaoMoeda cotacao = new CotacaoMoeda() { IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador };

        public MoedaCotacaoEdicaoViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
            AdicionarValidacoes();
            DataCotacao.Value = DateTime.Today;
            Moedas = Funcoes.RetornarMoedas();
            Moeda.Value = Moedas.Where(d => d.Codigo == GlobalSetting.Instance.ViagemSelecionado.Moeda.GetValueOrDefault().ToString()).FirstOrDefault();


        }
        public override Task InitializeAsync(object navigationData)
        {
            if (navigationData != null && navigationData is CotacaoMoeda item)
            {
                cotacao = item;
                Moeda.Value = Moedas.Where(d => d.Codigo == item.Moeda.ToString()).FirstOrDefault();
                ValorCotacao.Value = item.ValorCotacao.GetValueOrDefault();
                DataCotacao.Value = item.DataCotacao.GetValueOrDefault();


            }
            return base.InitializeAsync(navigationData);
        }

        public ObservableCollection<ItemLista> Moedas
        {
            get { return _moedas; }
            set { SetProperty(ref _moedas, value); }
        }
        public ValidatableObject<ItemLista> Moeda
        {
            get { return _moeda; }
            set { SetProperty(ref _moeda, value); }
        }
        public ValidatableObject<DateTime> DataCotacao
        {
            get { return _dataCotacao; }
            set { SetProperty(ref _dataCotacao, value); }
        }
        public ValidatableObject<decimal> ValorCotacao
        {
            get { return _valorCotacao; }
            set { SetProperty(ref _valorCotacao, value); }
        }

        public ICommand ValidarMoedaCommad => new Command(() => ValidarMoeda());
        public ICommand ValidarDataCotacaoCommad => new Command(() => ValidarDataCotacao());
        public ICommand ValidarValorCotacaoCommad => new Command(() => ValidarValorCotacao());

        public ICommand SalvarCommand => new Command(async () => await Salvar());

        public ICommand CancelarCommand => new Command(async () =>
        {           
                await NavigationService.TrocarPaginaShell("..");
        });

        private bool ValidarMoeda()
        {
            return Moeda.Validate();
        }

        private bool ValidarDataCotacao()
        {
            return DataCotacao.Validate();
        }

        private bool ValidarValorCotacao()
        {
            return ValorCotacao.Validate();
        }
        private void AdicionarValidacoes()
        {
            ValorCotacao.Validations.Add(new MaiorZeroRule<decimal> { ValidationMessage = AppResource.CampoObrigatorio });
            DataCotacao.Validations.Add(new MaiorZeroRule<DateTime> { ValidationMessage = AppResource.CampoObrigatorio });
            Moeda.Validations.Add(new IsNotNullOrEmptyRule<ItemLista>() { ValidationMessage = AppResource.CampoObrigatorio });

        }

        private async Task Salvar()
        {
            if (ValidarMoeda() && ValidarDataCotacao() && ValidarValorCotacao())
            {
                IsBusy = true;
                try
                {
                    bool Executado = false;
                    cotacao.Moeda = Convert.ToInt32(Moeda.Value.Codigo);
                    cotacao.DataCotacao = DataCotacao.Value;
                    cotacao.ValorCotacao = ValorCotacao.Value;
                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
                    {
                        Resultado = await _apiService.SalvarCotacaoMoeda(cotacao);
                        if (Resultado!=null)
                        {
                            Executado = true;
                            cotacao  = await _apiService.CarregarCotacaoMoeda(cotacao.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()));

                            var itemAjustar = await _database.RetornarCotacaoMoeda(cotacao.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()));
                            if (itemAjustar != null)
                                cotacao.Id = itemAjustar.Id;
                            itemAjustar.DataAtualizacao = DateTime.Now.ToUniversalTime();
                            await _database.SalvarCotacaoMoeda(cotacao);
                        }
                    }
                    if (!Executado)
                    {
                        cotacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                        cotacao.AtualizadoBanco = false;
                        Resultado = await _dataService.SalvarCotacaoMoeda(cotacao);
                    }
                    if (Resultado.Sucesso)
                        MessagingCenter.Send<MoedaCotacaoEdicaoViewModel, CotacaoMoeda>(this, MessageKeys.SalvarCotacao, cotacao);
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
