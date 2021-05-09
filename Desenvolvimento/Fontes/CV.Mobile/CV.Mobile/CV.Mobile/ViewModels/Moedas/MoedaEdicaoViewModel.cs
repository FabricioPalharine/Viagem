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
    public class MoedaEdicaoViewModel: BaseViewModel
    {
        private readonly IApiService _apiService;
        private CriterioBusca criterioBusca = new CriterioBusca();
        private readonly IDatabase _database;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private ValidatableObject<DateTime> _dataAporte = new ValidatableObject<DateTime>();
        private ValidatableObject<ItemLista> _moeda = new ValidatableObject<ItemLista>();
        private ValidatableObject<decimal> _valor = new ValidatableObject<decimal>();
        private ValidatableObject<decimal> _cotacao = new ValidatableObject<decimal>();
        private bool _baixarMoeda = true;
        private ValidatableObject<decimal> _valorBaixa = new ValidatableObject<decimal>();
        private ValidatableObject<ItemLista> _moedaBaixa = new ValidatableObject<ItemLista>();
        private int? IdGasto = null;
        private ObservableCollection<ItemLista> _moedas = new ObservableCollection<ItemLista>();
        private AporteDinheiro aporte = new AporteDinheiro() { IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador, IdentificadorUsuario= GlobalSetting.Instance.UsuarioLogado.Codigo};
        public MoedaEdicaoViewModel(IApiService apiService, IDataService dataService, IDatabase database, ISettingsService settingsService)
        {
            _apiService = apiService;
            _database = database;
            _dataService = dataService;
            _settingsService = settingsService;
            AdicionarValidacoes();
            DataAporte.Value = DateTime.Today;
            Moedas = Funcoes.RetornarMoedas();
            Moeda.Value = Moedas.Where(d => d.Codigo == GlobalSetting.Instance.ViagemSelecionado.Moeda.GetValueOrDefault().ToString()).FirstOrDefault();


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

        public ValidatableObject<ItemLista> MoedaBaixa
        {
            get { return _moedaBaixa; }
            set { SetProperty(ref _moedaBaixa, value); }
        }
        public ValidatableObject<DateTime> DataAporte
        {
            get { return _dataAporte; }
            set { SetProperty(ref _dataAporte, value); }
        }
        public ValidatableObject<decimal> Valor
        {
            get { return _valor; }
            set { SetProperty(ref _valor, value); }
        }
        public ValidatableObject<decimal> ValorBaixa
        {
            get { return _valorBaixa; }
            set { SetProperty(ref _valorBaixa, value); }
        }

        public ValidatableObject<decimal> Cotacao
        {
            get { return _cotacao; }
            set { SetProperty(ref _cotacao, value); }
        }

        public bool BaixarMoeda
        {
            get { return _baixarMoeda; }
            set { SetProperty(ref _baixarMoeda, value); }
        }

        public ICommand ValidarMoedaCommad => new Command(() => ValidarMoeda());
        public ICommand ValidarMoedaBaixaCommad => new Command(() => ValidarMoedaBaixa());

        public ICommand ValidarDataAporteCommad => new Command(() => ValidarDataAporte());
        public ICommand ValidarValorCommad => new Command(() => { ValidarValor(); CalcularValorBaixa(); });
        public ICommand ValidarCotacaoCommad => new Command(() => CalcularValorBaixa());

        public ICommand ValidarValorBaixaCommad => new Command(() => ValidarValorBaixa());
        public ICommand ValidarBaixarMoedaCommad => new Command(() => ValidarBaixarMoedaCotacao());

        public ICommand SalvarCommand => new Command(async () => await Salvar());

        public ICommand CancelarCommand => new Command(async () =>
        {
            await NavigationService.TrocarPaginaShell("..");
        });

        private bool ValidarMoeda()
        {
            return Moeda.Validate();
        }

        private bool ValidarMoedaBaixa()
        {
            return MoedaBaixa.Validate();
        }

        private bool ValidarDataAporte()
        {
            return DataAporte.Validate();
        }
        private bool ValidarValor()
        {
            return Valor.Validate();
        }
        private bool ValidarValorBaixa()
        {
            return ValorBaixa.Validate();
        }
        private void ValidarBaixarMoedaCotacao()
        {
            if (BaixarMoeda)
            {
                ValidarMoedaBaixa();
                ValidarValorBaixa();

            }

        }

        private void CalcularValorBaixa()
        {
            if (ValidarValor() && Cotacao.Value > 0)
                ValorBaixa.Value = Valor.Value * Cotacao.Value;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData != null && navigationData is AporteDinheiro item)
            {
                IsBusy = true;
                try
                {

                    aporte = await _dataService.CarregarAporteDinheiro(item.Identificador);
                    if (aporte.ItemGasto != null)
                        IdGasto = aporte.ItemGasto.Id;
                    else
                        IdGasto = null;
                    DataAporte.Value = aporte.DataAporte.GetValueOrDefault();
                    if (aporte.Cotacao.HasValue)
                        Cotacao.Value = aporte.Cotacao.Value;
                    Valor.Value = aporte.Valor.GetValueOrDefault();
                    BaixarMoeda = aporte.ItemGasto != null;
                    Moeda.Value = Moedas.Where(d => d.Codigo == item.Moeda.ToString()).FirstOrDefault();

                    if (BaixarMoeda)
                    {
                        ValorBaixa.Value = aporte.ItemGasto.Valor.GetValueOrDefault();
                        MoedaBaixa.Value = Moedas.Where(d => d.Codigo == aporte.ItemGasto.Moeda.ToString()).FirstOrDefault();

                    }
                }
                finally
                {
                    IsBusy = false;
                }

            }
        }

        private void AdicionarValidacoes()
        {
            Valor.Validations.Add(new MaiorZeroRule<decimal> { ValidationMessage = AppResource.CampoObrigatorio });
            DataAporte.Validations.Add(new MaiorZeroRule<DateTime> { ValidationMessage = AppResource.CampoObrigatorio });
            Moeda.Validations.Add(new IsNotNullOrEmptyRule<ItemLista>() { ValidationMessage = AppResource.CampoObrigatorio });
            ValorBaixa.Validations.Add(new MaiorZeroRule<decimal> { ValidationMessage = AppResource.CampoObrigatorio });
            MoedaBaixa.Validations.Add(new IsNotNullOrEmptyRule<ItemLista>() { ValidationMessage = AppResource.CampoObrigatorio });


        }

        private async Task Salvar()
        {
            bool valido = ValidarMoeda() & ValidarDataAporte() & ValidarValor();
            if (BaixarMoeda)
                valido &= ValidarMoedaBaixa() & ValidarValorBaixa();
            if (valido)
            {
                IsBusy = true;
                try
                {
                    bool Executado = false;
                    if (Cotacao.Value > 0)
                        aporte.Cotacao = Cotacao.Value;
                    else
                        aporte.Cotacao = null;
                    aporte.DataAporte = DataAporte.Value;
                    aporte.Moeda = Convert.ToInt32(Moeda.Value.Codigo);
                    aporte.Valor = Valor.Value;
                    if (BaixarMoeda)
                    {
                        if (aporte.ItemGasto == null)
                        {
                            aporte.ItemGasto = new Gasto() {IdentificadorViagem = GlobalSetting.Instance.ViagemSelecionado.Identificador, IdentificadorUsuario = GlobalSetting.Instance.UsuarioLogado.Codigo, ApenasBaixa = true, Dividido = false, Descricao = "Compra Moeda", Especie = true };
                        }
                        aporte.ItemGasto.Valor = ValorBaixa.Value;
                        aporte.ItemGasto.Moeda = Convert.ToInt32(MoedaBaixa.Value.Codigo);
                        aporte.ItemGasto.Data = DataAporte.Value;
                    }
                    else
                        aporte.ItemGasto = null;

                    ResultadoOperacao Resultado = new ResultadoOperacao();
                    if (Funcoes.VerificarConsultaInternet(_settingsService.AcompanhamentoOnline))
                    {
                        Resultado = await _apiService.SalvarAporteDinheiro(aporte);
                        if (Resultado != null)
                        {
                            var id = aporte.Id;
                            Executado = true;
                            var itemAporte = await _apiService.CarregarAporteDinheiro(aporte.Identificador.GetValueOrDefault(Resultado.IdentificadorRegistro.GetValueOrDefault()));
                            var itemLocal = await _database.RetornarAporteDinheiro(itemAporte.Identificador);
                            itemAporte.Id = id;
                            if (itemAporte.ItemGasto != null)
                            {

                                itemAporte.ItemGasto.Id = IdGasto;
                            }
                            await _dataService.GravarDadosAporte(itemAporte);
                        }
                    }
                    if (!Executado)
                    {
                        aporte.AtualizadoBanco = false;
                        aporte.DataAtualizacao = DateTime.UtcNow;
                        if (aporte.ItemGasto != null)
                        {
                            aporte.ItemGasto.AtualizadoBanco = false;
                            aporte.ItemGasto.DataAtualizacao = DateTime.UtcNow;
                        }
                        Resultado = await _dataService.SalvarAporteDinheiro(aporte);
                    }
                    if (Resultado.Sucesso)
                        MessagingCenter.Send<MoedaEdicaoViewModel, AporteDinheiro>(this, MessageKeys.SalvarMoeda, aporte);
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
