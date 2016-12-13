using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CV.Mobile.Helpers;
using CV.Mobile.Models;
using System.Collections.ObjectModel;
using CV.Mobile.Services;
using FormsToolkit;

namespace CV.Mobile.ViewModels
{
    public class EdicaoAgrupamentoCidadeViewModel : BaseNavigationViewModel
    {
        private ManutencaoCidadeGrupo _ItemCidadeGrupo;
        private Cidade _ItemCidadeSelecionada;

        public EdicaoAgrupamentoCidadeViewModel(ManutencaoCidadeGrupo pItemCidadeGrupo, List<Cidade> pCidadesPai, List<Cidade> pCidadesFilhas)
        {
            _ItemCidadeGrupo = pItemCidadeGrupo;
            CidadesPai = new ObservableCollection<Cidade>(pCidadesPai);
            CidadesFilhas = new ObservableCollection<Cidade>(pCidadesFilhas);
            foreach (var itemCidade in CidadesFilhas)
            {
                if (pItemCidadeGrupo.CidadesFilhas.Contains(itemCidade.Identificador))
                    itemCidade.Selecionada = true;
            }
            SalvarCommand = new Command(
                                            async () => await Salvar(),
                                            () => true);
            ClicarCidadeCommand = new Command<Cidade>( (item) =>  AlterarSelecionadoItem(item));

        }

     
        public Command SalvarCommand { get; set; }
        public Command ClicarCidadeCommand { get; set; }

        public ObservableCollection<Cidade> CidadesPai { get; set; }
        public ObservableCollection<Cidade> CidadesFilhas { get; set; }
        public ManutencaoCidadeGrupo ItemCidadeGrupo
        {
            get
            {
                return _ItemCidadeGrupo;
            }

            set
            {
                SetProperty(ref _ItemCidadeGrupo, value);
            }
        }

        public Cidade ItemCidadeSelecionada
        {
            get
            {
                return _ItemCidadeSelecionada;
            }

            set
            {
                SetProperty(ref _ItemCidadeSelecionada, value);
                foreach (var itemCidade in CidadesFilhas)
                {
                    if (value.Identificador == itemCidade.Identificador)
                        itemCidade.Visivel = false;
                    else
                        itemCidade.Visivel = true;
                }

            }
        }

        private void AlterarSelecionadoItem(Cidade item)
        {
            item.Selecionada = !item.Selecionada;
        }


        private async Task Salvar()
        {
            IsBusy = true;
            SalvarCommand.ChangeCanExecute();
            try
            {
                if (ItemCidadeSelecionada != null)
                    ItemCidadeGrupo.IdentificadorCidade = ItemCidadeSelecionada.Identificador;
                else
                    ItemCidadeGrupo.IdentificadorCidade = null;
                ItemCidadeGrupo.CidadesFilhas = new ObservableCollection<int?>(CidadesFilhas.Where(d=>d.Selecionada && d.Visivel).Select(d=>d.Identificador));
                using (ApiService srv = new ApiService())
                {
                    var Resultado = await srv.SalvarCidadeGrupo(ItemCidadeGrupo);
                    if (Resultado.Sucesso)
                    {

                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Sucesso",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });
                        // ItemCotacao.Identificador = Resultado.IdentificadorRegistro;
                        var itemCidade = new Cidade();
                        MessagingService.Current.SendMessage<Cidade>(MessageKeys.ManutencaoAgrupamentoCidade, itemCidade);
                        await PopAsync();
                    }
                    else if (Resultado.Mensagens != null && Resultado.Mensagens.Any())
                    {
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                        {
                            Title = "Problemas Validação",
                            Message = String.Join(Environment.NewLine, Resultado.Mensagens.Select(d => d.Mensagem).ToArray()),
                            Cancel = "OK"
                        });

                    }
                }
            }
            finally
            {
                SalvarCommand.ChangeCanExecute();
                IsBusy = false;
            }
        }

    }
}
