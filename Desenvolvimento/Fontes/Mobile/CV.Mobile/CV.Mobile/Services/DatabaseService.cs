using CV.Mobile.Data;
using CV.Mobile.Interfaces;
using Microsoft.Practices.ServiceLocation;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV.Mobile.Models;
using MvvmHelpers;

namespace CV.Mobile.Services
{
    public static class DatabaseService
    {

        static CVDatabase database;

        public static CVDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new CVDatabase(ServiceLocator.Current.GetInstance<IFileHelper>().GetLocalFilePath("CVBase.db3"));
                }
                return database;
            }
        }

        public static async void SincronizarParticipanteViagem(Viagem itemViagem, ControleSincronizacao itemCS)
        {
            List<ParticipanteViagem> listaParticipantes = await Database.ListarParticipanteViagemAsync();
            foreach (var itemParticipante in itemViagem.Participantes)
            {
                if (!listaParticipantes.Where(d => d.Identificador == itemViagem.Identificador).Any())
                {
                    await Database.SalvarParticipanteViagemAsync(itemParticipante);
                    var itemUsuario = await Database.CarregarUsuario(itemParticipante.IdentificadorUsuario.GetValueOrDefault());
                    if (itemUsuario == null)
                        await Database.SalvarUsuarioAsync(itemParticipante.ItemUsuario);
                }
            }
            foreach (var itemParticipante in listaParticipantes)
            {
                if (!itemViagem.Participantes.Where(d => d.Identificador == itemViagem.Identificador).Any())
                {
                    await Database.ExcluirParticipanteViagemAsync(itemParticipante);
                }
            }
        }

        public static async Task SalvarAmigos(List<Amigo> Amigos)
        {
            foreach (var itemAmigo in Amigos)
            {
                var itemUsuario = await Database.CarregarUsuario(itemAmigo.IdentificadorAmigo.GetValueOrDefault());
                if (itemUsuario == null)
                    await Database.SalvarUsuarioAsync(itemAmigo.ItemAmigo);

            }
            await Database.LimparAmigos();
            await Database.IncluirListaAmigo(Amigos);
        }

        public static async void AjustarAmigo(ConsultaAmigo itemAmigo)
        {
            if (itemAmigo.Acao == 1)
            {
                 AdicionarAmigoBase(itemAmigo);
            }
            else
            {
                var itemBase = await Database.RetornarAmigoIdentificadorUsuario(itemAmigo.IdentificadorUsuario.GetValueOrDefault());
                if (itemBase != null)
                    await Database.ExcluirAmigo(itemBase);
            }
        }

        internal static async Task<ResultadoOperacao> SalvarAgendamentoSugestao(AgendarSugestao itemAgenda)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();

            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemAgenda.itemCalendario.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemAgenda.itemSugestao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemAgenda.itemCalendario.CodigoPlace = itemAgenda.itemSugestao.CodigoPlace;
                itemAgenda.itemCalendario.Nome = itemAgenda.itemSugestao.Local;
                itemAgenda.itemCalendario.Latitude = itemAgenda.itemSugestao.Latitude;
                itemAgenda.itemCalendario.Longitude = itemAgenda.itemSugestao.Longitude;

                await Database.SalvarCalendarioPrevisto(itemAgenda.itemCalendario);
                await Database.SalvarSugestao(itemAgenda.itemSugestao);

                var itemCV = await Database.GetControleSincronizacaoAsync();
                if (itemCV.SincronizadoEnvio)
                {
                    itemCV.SincronizadoEnvio = false;
                    await Database.SalvarControleSincronizacao(itemCV);
                }

                Erros.Add(new MensagemErro() { Mensagem = "Sugestão Agendada com Sucesso" });

            }
            itemResultado.Mensagens = Erros.ToArray();

            return itemResultado;
        }

        public static async void AdicionarAmigoBase(ConsultaAmigo itemAmigo)
        {
            var itemUsuario = await Database.CarregarUsuario(itemAmigo.IdentificadorUsuario.GetValueOrDefault());
            if (itemUsuario == null)
                await Database.SalvarUsuarioAsync(new Usuario() { Identificador = itemAmigo.IdentificadorUsuario, Nome = itemAmigo.Nome });
            Amigo itemNovo = new Amigo() { IdentificadorAmigo = itemAmigo.IdentificadorUsuario };
            await Database.SalvarAmigo(itemNovo);
        }

        internal static async Task<ResultadoOperacao> SalvarAporteDinheiro(AporteDinheiro itemAporteDinheiro)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            if (!itemAporteDinheiro.Cotacao.HasValue)
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Cotação é obrigatório" });

            }
            if (!itemAporteDinheiro.Valor.HasValue)
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Valor é obrigatório" });

            }
            if (!itemAporteDinheiro.Moeda.HasValue)
                Erros.Add(new MensagemErro() { Mensagem = "O campo Moeda é obrigatório" });
            itemAporteDinheiro.DataAtualizacao = DateTime.Now.ToUniversalTime();
            if (itemAporteDinheiro.ItemGasto != null)
            {
                itemAporteDinheiro.ItemGasto.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemAporteDinheiro.ItemGasto.Data = itemAporteDinheiro.DataAporte;
                if (!itemAporteDinheiro.ItemGasto.Valor.HasValue)
                {
                    Erros.Add(new MensagemErro() { Mensagem = "O campo Valor Baixar é obrigatório" });

                }
                if (!itemAporteDinheiro.ItemGasto.Moeda.HasValue)
                    Erros.Add(new MensagemErro() { Mensagem = "O campo Moeda Baixar é obrigatório" });

            }
            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                await GravarDadosAporte(itemAporteDinheiro);
                Erros.Add(new MensagemErro() { Mensagem = "Aporte de Dinheiro salvo com sucesso" });

            }
            itemResultado.Mensagens = Erros.ToArray();


            return itemResultado;
        }

        public static async Task GravarDadosAporte(AporteDinheiro itemAporteDinheiro)
        {
            if (itemAporteDinheiro.ItemGasto != null)
            {
                await Database.SalvarGasto(itemAporteDinheiro.ItemGasto);
                itemAporteDinheiro.IdentificadorGasto = itemAporteDinheiro.ItemGasto.Identificador;
            }
            else
                itemAporteDinheiro.IdentificadorGasto = null;
            await Database.SalvarAporteDinheiro(itemAporteDinheiro);
        }

        internal static async Task<ResultadoOperacao> SalvarComentario(Comentario itemComentario)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            if (string.IsNullOrEmpty(itemComentario.Texto))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Comentário é obrigatório" });
            }
            if (!itemComentario.Latitude.HasValue || !itemComentario.Longitude.HasValue)
                Erros.Add(new MensagemErro() { Mensagem = "Favor aguardar a sua posição ser detectada" });
            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemComentario.DataAtualizacao = DateTime.Now.ToUniversalTime();
                await Database.SalvarComentario(itemComentario) ;

                var itemCV = await Database.GetControleSincronizacaoAsync();
                if (itemCV.SincronizadoEnvio)
                {
                    itemCV.SincronizadoEnvio = false;
                    await Database.SalvarControleSincronizacao(itemCV);
                }
                Erros.Add(new MensagemErro() { Mensagem = "Comentário salvo com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();


            return itemResultado;
        }

        internal  static async Task<AporteDinheiro> CarregarAporteDinheiro(int? identificador)
        {
            var itemAporte = await Database.RetornarAporteDinheiro(identificador);
            if (itemAporte.IdentificadorGasto.HasValue)
                itemAporte.ItemGasto = await CarregarGasto(itemAporte.IdentificadorGasto);
            return itemAporte;
        }

        private static async Task<Gasto> CarregarGasto(int? identificadorGasto)
        {
            var itemGasto = await Database.RetornarGasto(identificadorGasto);
            itemGasto.Alugueis = new ObservableRangeCollection<AluguelGasto>( await Database.ListarAluguelGasto_IdentificadorGasto(identificadorGasto));
            itemGasto.Atracoes = new ObservableRangeCollection<GastoAtracao>(await Database.ListarGastoAtracao_IdentificadorGasto(identificadorGasto));
            itemGasto.Compras = new ObservableRangeCollection<GastoCompra>(await Database.ListarGastoCompra_IdentificadorGasto(identificadorGasto));
            itemGasto.Hoteis = new ObservableRangeCollection<GastoHotel>(await Database.ListarGastoHotel_IdentificadorGasto(identificadorGasto));
            itemGasto.Reabastecimentos = new ObservableRangeCollection<ReabastecimentoGasto>(await Database.ListarReabastecimentoGasto_IdentificadorGasto(identificadorGasto));
            itemGasto.Refeicoes = new ObservableRangeCollection<GastoRefeicao>(await Database.ListarGastoRefeicao_IdentificadorGasto(identificadorGasto));
            itemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(identificadorGasto));
            itemGasto.ViagenAereas = new ObservableRangeCollection<GastoViagemAerea>(await Database.ListarGastoViagemAerea_IdentificadorGasto(identificadorGasto));

            return itemGasto;
        }

        internal static async Task ExcluirAporteDinheiro(AporteDinheiro item)
        {
           
            var itemAporte = await CarregarAporteDinheiro(item.Identificador);
            itemAporte.DataExclusao = DateTime.Now.ToUniversalTime();
            await Database.SalvarAporteDinheiro(itemAporte);
            if (itemAporte.ItemGasto != null)
            {
                await ExcluirGasto(itemAporte.ItemGasto);
            }
        }

        private static async Task ExcluirGasto(Gasto itemGasto)
        {
            itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
            await Database.SalvarGasto(itemGasto);
            foreach (var item in itemGasto.Alugueis)
            {
                item.DataExclusao = DateTime.Now.ToUniversalTime();
                await Database.SalvarAluguelGasto(item);
            }
            foreach (var item in itemGasto.Compras)
            {
                item.DataExclusao = DateTime.Now.ToUniversalTime();
                await Database.SalvarGastoCompra(item);
            }
            foreach (var item in itemGasto.Atracoes)
            {
                item.DataExclusao = DateTime.Now.ToUniversalTime();
                await Database.SalvarGastoAtracao(item);
            }
            foreach (var item in itemGasto.Hoteis)
            {
                item.DataExclusao = DateTime.Now.ToUniversalTime();
                await Database.SalvarGastoHotel(item);
            }
            foreach (var item in itemGasto.Reabastecimentos)
            {
                item.DataExclusao = DateTime.Now.ToUniversalTime();
                await Database.SalvarReabastecimentoGasto(item);
            }
            foreach (var item in itemGasto.Refeicoes)
            {
                item.DataExclusao = DateTime.Now.ToUniversalTime();
                await Database.SalvarGastoRefeicao(item);
            }
            foreach (var item in itemGasto.ViagenAereas)
            {
                item.DataExclusao = DateTime.Now.ToUniversalTime();
                await Database.SalvarGastoViagemAerea(item);
            }
            
        }

        internal static async Task<ResultadoOperacao> SalvarCotacaoMoeda(CotacaoMoeda itemCotacaoMoeda)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
           
            if (!itemCotacaoMoeda.Moeda.HasValue)
                Erros.Add(new MensagemErro() { Mensagem = "O campo Moeda é obrigatório." });
            if (!itemCotacaoMoeda.ValorCotacao.HasValue)
                Erros.Add(new MensagemErro() { Mensagem = "O campo Valor Cotação é obrigatório." });
            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemCotacaoMoeda.DataAtualizacao = DateTime.Now.ToUniversalTime();
                await Database.SalvarCotacaoMoeda(itemCotacaoMoeda);

                var itemCV = await Database.GetControleSincronizacaoAsync();
                if (itemCV.SincronizadoEnvio)
                {
                    itemCV.SincronizadoEnvio = false;
                    await Database.SalvarControleSincronizacao(itemCV);
                }
                Erros.Add(new MensagemErro() { Mensagem = "Cotação de Moeda salva com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();


            return itemResultado;
        }
    }
}
