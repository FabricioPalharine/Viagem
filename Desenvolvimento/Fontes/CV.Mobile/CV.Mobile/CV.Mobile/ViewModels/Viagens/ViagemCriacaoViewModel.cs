using CV.Mobile.Helper;
using CV.Mobile.Models;
using CV.Mobile.Resources;
using CV.Mobile.Services.Api;
using CV.Mobile.Services.Data;
using CV.Mobile.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CV.Mobile.ViewModels.Viagens
{
  

    public class ViagemCriacaoViewModel : BaseViewModel
    {

        private readonly IApiService _apiService;
        private readonly IDatabase _database;


        private Viagem _viagem = new Viagem();
        private ValidatableObject<string> _nome = new ValidatableObject<string>();
        private ValidatableObject<DateTime> _dataInicio = new ValidatableObject<DateTime>();
        private ValidatableObject<DateTime> _dataFim = new ValidatableObject<DateTime>();
        private ValidatableObject<ItemLista> _moeda = new ValidatableObject<ItemLista>();
        private ValidatableObject<int> _QuantidadeParticipantes = new ValidatableObject<int>();
        private bool _gastosPublicos = false;
        private bool _unidadeMetrica = true;
        private bool _viagemPublica = true;

        private ObservableCollection<Usuario> _participantes = new ObservableCollection<Usuario>();
        private ObservableCollection<Usuario> _amigosVerCustos = new ObservableCollection<Usuario>();
        private ObservableCollection<Usuario> _amigos = new ObservableCollection<Usuario>();
        private ValidatableObject<Usuario> _participante = new ValidatableObject<Usuario>();
        private ValidatableObject<Usuario> _amigoVerCustos = new ValidatableObject<Usuario>();
        private ObservableCollection<ItemLista> _moedas = new ObservableCollection<ItemLista>();
        private int _identificadorUsuarioPrincipal;
        private int _tabSelecionada = 0;

        public ViagemCriacaoViewModel(IApiService apiService, IDatabase database)
        {
            _apiService = apiService;
            _database = database;
            _identificadorUsuarioPrincipal = GlobalSetting.Instance.UsuarioLogado.Codigo;
            Moedas = Funcoes.RetornarMoedas();
            AdicionarValidacoes();
            DataInicio.Value = DateTime.Today;
            DataFim.Value = DateTime.Today.AddDays(10);
            QuantidadeParticipantes.Value = 1;
        }

        public int TabSelecionada
        {
            get { return _tabSelecionada; }
            set { SetProperty(ref _tabSelecionada, value); }
        }

        public ObservableCollection<ItemLista> Moedas
        {
            get { return _moedas; }
            set { SetProperty(ref _moedas, value); }
        }

        public ObservableCollection<Usuario> AmigosVerCustos
        {
            get { return _amigosVerCustos; }
            set { SetProperty(ref _amigosVerCustos, value); }
        }
        public ObservableCollection<Usuario> Participantes
        {
            get { return _participantes; }
            set { SetProperty(ref _participantes, value); }
        }

        public ObservableCollection<Usuario> Amigos
        {
            get { return _amigos; }
            set { SetProperty(ref _amigos, value); }
        }

        public ValidatableObject<Usuario> AmigoVerCusto
        {
            get { return _amigoVerCustos; }
            set { SetProperty(ref _amigoVerCustos, value); }
        }

        public ValidatableObject<Usuario> Participante
        {
            get { return _participante; }
            set { SetProperty(ref _participante, value); }
        }

        public bool UnidadeMetrica
        {
            get { return _unidadeMetrica; }
            set { SetProperty(ref _unidadeMetrica, value); }
        }

        public bool GastosPublicos
        {
            get { return _gastosPublicos; }
            set { SetProperty(ref _gastosPublicos, value); }
        }
        public bool ViagemPublica
        {
            get { return _viagemPublica; }
            set { SetProperty(ref _viagemPublica, value); }
        }

        public ValidatableObject<string> Nome
        {
            get { return _nome; }
            set { SetProperty(ref _nome, value); }
        }

        public ValidatableObject<ItemLista> Moeda
        {
            get { return _moeda; }
            set { SetProperty(ref _moeda, value); }
        }

        public ValidatableObject<int> QuantidadeParticipantes
        {
            get { return _QuantidadeParticipantes; }
            set { SetProperty(ref _QuantidadeParticipantes, value); }
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

        public int IdentificadorUsuarioPrincipal
        {
            get { return _identificadorUsuarioPrincipal; }
            set { SetProperty(ref _identificadorUsuarioPrincipal, value); }
        }

        public bool NovaViagem { get; set; } = false;

        public ICommand PageAppearingCommand => new Command(async () => await CarregarPagina());
        public ICommand CancelarCommand => new Command(async () => await NavigationService.TrocarPaginaShell("///HomePage"));
        public ICommand ExcluirParticipanteCommand => new Command<Usuario>(async (d) => await ExcluirParticipante(d));
        public ICommand ExcluirVerCustosCommand => new Command<Usuario>(async (d) => await ExcluirVerCustos(d));

        public ICommand ValidarNomeCommad => new Command(() => ValidarNome());
        public ICommand ValidarDataInicioCommad => new Command(() => ValidarDataInicio());
        public ICommand ValidarDataFimCommad => new Command(() => ValidarDataFim());
        public ICommand ValidarMoedaCommad => new Command(() => ValidarMoeda());
        public ICommand ValidarQuantidadeParticipantesCommand => new Command(() => ValidarQuantidadeParticipantes());
        public ICommand AdicionarParticipanteCommand => new Command(() => AdicionarParticipante());
        public ICommand AdicionarAmigoVerCustoCommand => new Command(() => AdicionarAmigoVerCusto());
        public ICommand SalvarCommand => new Command(async () => await Salvar());

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
            return DataFim.Validate();
        }
        private bool ValidarMoeda()
        {
            return Moeda.Validate();
        }
        private bool ValidarQuantidadeParticipantes()
        {
            return QuantidadeParticipantes.Validate();
        }

        public Viagem Viagem
        {
            get { return _viagem; }
            set { SetProperty(ref _viagem, value); }
        }

        private async Task CarregarPagina()
        {
            IsBusy = true;
            try
            {
                if (Funcoes.AcessoInternet)
                {
                    Amigos = new ObservableCollection<Usuario>(await _apiService.ListarAmigos());
                    
                    Participantes.Insert(0, await _apiService.CarregarUsuario(IdentificadorUsuarioPrincipal));


                }
                else
                {
                    await NavigationService.TrocarPaginaShell("///HomePage");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }


        private void AdicionarValidacoes()
        {
            Nome.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = AppResource.CampoObrigatorio });
            QuantidadeParticipantes.Validations.Add(new MaiorZeroRule<int> { ValidationMessage = AppResource.CampoObrigatorio });
            DataInicio.Validations.Add(new MaiorZeroRule<DateTime> { ValidationMessage = AppResource.CampoObrigatorio });
            DataFim.Validations.Add(new MaiorDataRule<DateTime> { ValidationMessage = AppResource.MaiorDataInicio, DataLimite = DataInicio });
            Moeda.Validations.Add(new IsNotNullOrEmptyRule<ItemLista>() { ValidationMessage = AppResource.CampoObrigatorio });
            Participante.Validations.Add(new IsNotNullOrEmptyRule<Usuario>() { ValidationMessage = AppResource.CampoObrigatorio });
            AmigoVerCusto.Validations.Add(new IsNotNullOrEmptyRule<Usuario>() { ValidationMessage = AppResource.CampoObrigatorio });

        }

        private Task ExcluirParticipante(Usuario d)
        {
            if (d.Identificador != IdentificadorUsuarioPrincipal && d.Identificador != GlobalSetting.Instance.UsuarioLogado.Codigo)
            {
                Participantes.Remove(d);
            }
            return Task.FromResult(true);
        }

        private Task ExcluirVerCustos(Usuario d)
        {

            AmigosVerCustos.Remove(d);

            return Task.FromResult(true);
        }

        private void AdicionarParticipante()
        {
            if (Participante.Validate() && !Participantes.Where(d => d.Identificador == Participante.Value.Identificador).Any())
            {
                Participantes.Add(Participante.Value);

            }
        }

        private void AdicionarAmigoVerCusto()
        {
            if (AmigoVerCusto.Validate() && !AmigosVerCustos.Where(d => d.Identificador == AmigoVerCusto.Value.Identificador).Any())
            {
                AmigosVerCustos.Add(Participante.Value);

            }
        }

        private async Task Salvar()
        {
            if (ValidarNome() && ValidarDataInicio() && ValidarDataFim() && ValidarQuantidadeParticipantes() && ValidarMoeda())
            {
                IsBusy = true;
                try
                {
                    Viagem.IdentificadorUsuario = IdentificadorUsuarioPrincipal;
                    Viagem.Aberto = true;
                    Viagem.DataAlteracao = DateTime.Now;
                    Viagem.DataFim = DataFim.Value;
                    Viagem.DataInicio = DataInicio.Value;
                    Viagem.Moeda = Convert.ToInt32(Moeda.Value.Codigo);
                    Viagem.Nome = Nome.Value;
                    Viagem.PublicaGasto = GastosPublicos;
                    //Viagem.Aberto = ViagemPublica;
                    Viagem.QuantidadeParticipantes = QuantidadeParticipantes.Value;
                    Viagem.UnidadeMetrica = UnidadeMetrica;
                    Viagem.UsuariosGastos = new ObservableCollection<UsuarioGasto>(AmigosVerCustos.Select(d => new UsuarioGasto() { IdentificadorViagem = Viagem.Identificador, IdentificadorUsuario = d.Identificador, NomeUsuario = d.Nome }));
                    foreach (var usuario in Participantes.Where(d => !Viagem.Participantes.Where(f => f.IdentificadorUsuario == d.Identificador).Any()))
                    {
                        Viagem.Participantes.Add(new ParticipanteViagem() { IdentificadorViagem = Viagem.Identificador, IdentificadorUsuario = usuario.Identificador, NomeUsuario = usuario.Nome });
                    }
                    foreach (var usuario in Viagem.Participantes.Where(f => !Participantes.Where(d => f.IdentificadorUsuario == d.Identificador).Any()).ToList())
                    {
                        Viagem.Participantes.Remove(usuario);
                    }


                    var resultado = await _apiService.SalvarViagem(Viagem);
                    if (resultado.Sucesso)
                    {
                        await DialogService.ShowAlertAsync(String.Join(Environment.NewLine, resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            AppResource.Sucesso, AppResource.Ok);
                        if (NovaViagem)
                        {
                            bool TrocarViagem = true;
                            if (GlobalSetting.Instance.UsuarioLogado.IdentificadorViagem.HasValue)
                            {
                                Viagem itemViagem = await _database.GetViagemAtualAsync();
                                if (itemViagem.Aberto)
                                {
                                    TrocarViagem = await DialogService.ShowConfirmAsync(AppResource.TrocarViagemSelecionada, AppResource.Confirmacao, AppResource.Confirmar, AppResource.Cancelar);
                                }
                            }
                            if (TrocarViagem)
                            {

                                Viagem itemViagem = await _apiService.CarregarViagem(resultado.IdentificadorRegistro);
                                var itemViagemBanco = await _database.GetViagemAtualAsync();
                                itemViagem.Id = itemViagemBanco?.Id;

                                MessagingCenter.Send<ViagemCriacaoViewModel, Viagem>(this, MessageKeys.SelecionarViagem, itemViagem);

                            }

                        }
                        await NavigationService.TrocarPaginaShell("///HomePage");
                    }
                    else
                    {
                        var mensagens = AjustarErros(resultado.Mensagens);
                        if (mensagens.Any())
                            await DialogService.ShowAlertAsync(String.Join(Environment.NewLine, mensagens.Select(d => d.Mensagem).ToArray()),
                            AppResource.Problemas, AppResource.Ok);

                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }

        }


    }
}
