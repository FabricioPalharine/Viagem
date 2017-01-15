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
using CV.Mobile.Helpers;
using Newtonsoft.Json.Linq;

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

        public static async void SincronizarParticipanteViagem(Viagem itemViagem)
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

        internal static async Task<bool> ExisteEnvioPendente()
        {
            var item = await CarregarDadosEnvioSincronizar();
            return item.AportesDinheiro.Any() || item.Atracoes.Any() || item.CalendariosPrevistos.Any() || item.CarroDeslocamentos.Any()
                || item.Carros.Any() || item.Comentarios.Any() || item.Compras.Any() || item.CotacoesMoeda.Any() || item.Deslocamentos.Any()
                || item.EventosHotel.Any() || item.Gastos.Any() || item.GastosAtracao.Any() || item.GastosCarro.Any() || item.GastosDeslocamento.Any()
                || item.GastosHotel.Any() || item.GastosRefeicao.Any() || item.Hoteis.Any() || item.ItensComprados.Any() || item.ListaCompra.Any()
                || item.Lojas.Any() || item.Reabastecimento.Any() || item.Refeicoes.Any() || item.Sugestoes.Any();
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
                itemAgenda.itemCalendario.DataProximoAviso = itemAgenda.itemCalendario.DataInicio.GetValueOrDefault(new DateTime(1900, 01, 01));
                await Database.SalvarCalendarioPrevisto(itemAgenda.itemCalendario);
                await Database.SalvarSugestao(itemAgenda.itemSugestao);


                Erros.Add(new MensagemErro() { Mensagem = "Sugestão Agendada com Sucesso" });

            }
            itemResultado.Mensagens = Erros.ToArray();

            return itemResultado;
        }

        internal async static Task<ResultadoOperacao> SalvarCalendarioPrevisto(CalendarioPrevisto itemCalendarioPrevisto)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            if (string.IsNullOrEmpty(itemCalendarioPrevisto.Nome))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo nome é obrigatório" });
            }
            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemCalendarioPrevisto.AtualizadoBanco = false;
                itemCalendarioPrevisto.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemCalendarioPrevisto.DataProximoAviso = itemCalendarioPrevisto.DataInicio.GetValueOrDefault(new DateTime(1900, 01, 01));

                await Database.SalvarCalendarioPrevisto(itemCalendarioPrevisto);
                itemResultado.IdentificadorRegistro = itemCalendarioPrevisto.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Agendamento salvo com sucesso" });

            }
            itemResultado.Mensagens = Erros.ToArray();
            return itemResultado;
        }

        internal static async Task<ResultadoOperacao> SalvarListaCompra(ListaCompra itemListaCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            if (string.IsNullOrEmpty(itemListaCompra.Descricao))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Modelo é obrigatório" });
            }
            if (string.IsNullOrEmpty(itemListaCompra.Marca))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Marca é obrigatório" });
            }
            if (!itemListaCompra.Moeda.HasValue)
                Erros.Add(new MensagemErro() { Mensagem = "O campo Moeda é obrigatório" });
            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemListaCompra.AtualizadoBanco = false;
                itemListaCompra.DataAtualizacao = DateTime.Now.ToUniversalTime();
                await Database.SalvarListaCompra(itemListaCompra);
                itemResultado.IdentificadorRegistro = itemListaCompra.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Lista de Compra salva com sucesso" });

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
                itemResultado.IdentificadorRegistro = itemAporteDinheiro.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Aporte de Dinheiro salvo com sucesso" });

            }
            itemResultado.Mensagens = Erros.ToArray();


            return itemResultado;
        }

        internal static async void SalvarGastoSincronizado(Gasto itemGasto)
        {
            await Database.Excluir_Gasto_Filhos(itemGasto.Identificador.GetValueOrDefault());
            var itemBase = await Database.RetornarGasto(itemGasto.Identificador);
            if (itemBase != null)
                itemGasto.Id = itemBase.Id;
            foreach (var item in itemGasto.Alugueis)
            {
                await Database.SalvarAluguelGasto(item);
            }
            foreach (var item in itemGasto.Compras)
            {
                await Database.SalvarGastoCompra(item);
            }
            foreach (var item in itemGasto.Atracoes)
            {
                await Database.SalvarGastoAtracao(item);
            }
            foreach (var item in itemGasto.Hoteis)
            {
                await Database.SalvarGastoHotel(item);
            }
            foreach (var item in itemGasto.Reabastecimentos)
            {
                await Database.SalvarReabastecimentoGasto(item);
            }
            foreach (var item in itemGasto.Refeicoes)
            {
                await Database.SalvarGastoRefeicao(item);
            }
            foreach (var item in itemGasto.ViagenAereas)
            {
                await Database.SalvarGastoViagemAerea(item);
            }

            foreach (var item in itemGasto.Usuarios)
            {
                await Database.SalvarGastoDividido(item);
            }
        }

        internal async static Task<ResultadoOperacao> SalvarItemCompra(ItemCompra itemItemCompra, int? _IdentificadorListaCompraInicial)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            if (string.IsNullOrEmpty(itemItemCompra.Descricao))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Modelo é obrigatório" });
            }
            if (string.IsNullOrEmpty(itemItemCompra.Marca))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Marca é obrigatório" });
            }
            if (!itemItemCompra.Valor.HasValue)
                Erros.Add(new MensagemErro() { Mensagem = "O campo Valor é obrigatório" });
            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemItemCompra.AtualizadoBanco = false;
                itemItemCompra.DataAtualizacao = DateTime.Now.ToUniversalTime();
                if (itemItemCompra.IdentificadorUsuario.HasValue)
                {
                    var usuario = await Database.CarregarUsuario(itemItemCompra.IdentificadorUsuario.Value);
                    if (usuario != null)
                        itemItemCompra.NomeUsuario = usuario.Nome;
                }
                await Database.SalvarItemCompra(itemItemCompra);
                itemResultado.IdentificadorRegistro = itemItemCompra.Identificador;
                if (itemItemCompra.IdentificadorListaCompra.GetValueOrDefault(0) != _IdentificadorListaCompraInicial.GetValueOrDefault(0))
                {
                    if (itemItemCompra.IdentificadorListaCompra.HasValue)
                    {
                        var itemLC = await Database.RetornarListaCompra(itemItemCompra.IdentificadorListaCompra.Value);
                        if (itemLC != null)
                        {
                            itemLC.Status = (int)enumStatusListaCompra.Comprado;
                            itemLC.DataAtualizacao = DateTime.Now.ToUniversalTime();
                            itemLC.AtualizadoBanco = false;
                            await Database.SalvarListaCompra(itemLC);
                        }
                    }

                    if (_IdentificadorListaCompraInicial.HasValue)
                    {
                        var itemLC = await Database.RetornarListaCompra(_IdentificadorListaCompraInicial);
                        if (itemLC != null)
                        {
                            itemLC.Status = (int)enumStatusListaCompra.Pendente;
                            itemLC.DataAtualizacao = DateTime.Now.ToUniversalTime();
                            itemLC.AtualizadoBanco = false;
                            await Database.SalvarListaCompra(itemLC);
                        }
                    }
                }

                Erros.Add(new MensagemErro() { Mensagem = "Item da Compra salvo com sucesso" });

            }
            itemResultado.Mensagens = Erros.ToArray();
            return itemResultado;
        }

        public static async Task<ResultadoOperacao> SalvarGasto(Gasto itemGasto)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();

            if (!itemGasto.Moeda.HasValue)
                Erros.Add(new MensagemErro() { Mensagem = "O campo Moeda é obrigatório." });
            if (!itemGasto.Valor.HasValue)
                Erros.Add(new MensagemErro() { Mensagem = "O campo Valor é obrigatório." });
            if (string.IsNullOrWhiteSpace(itemGasto.Descricao))
                Erros.Add(new MensagemErro() { Mensagem = "O campo Descrição é obrigatório." });

            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemGasto.AtualizadoBanco = false;
                itemGasto.DataAtualizacao = DateTime.Now.ToUniversalTime();
                await Database.SalvarGasto(itemGasto);
                itemResultado.IdentificadorRegistro = itemGasto.Identificador;
                itemGasto.NomeUsuario = ((await Database.CarregarUsuario(itemGasto.IdentificadorUsuario.GetValueOrDefault())) ?? new Usuario()).Nome;

                foreach (var item in itemGasto.Alugueis.Where(d => !d.DataExclusao.HasValue || d.Identificador > 0))
                {
                    item.AtualizadoBanco = false;
                    item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    item.IdentificadorGasto = itemGasto.Identificador;
                    await Database.SalvarAluguelGasto(item);
                }
                foreach (var item in itemGasto.Compras.Where(d => !d.DataExclusao.HasValue || d.Identificador > 0))
                {
                    item.AtualizadoBanco = false;
                    item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    item.IdentificadorGasto = itemGasto.Identificador;
                    await Database.SalvarGastoCompra(item);
                }
                foreach (var item in itemGasto.Atracoes.Where(d => !d.DataExclusao.HasValue || d.Identificador > 0))
                {
                    item.AtualizadoBanco = false;
                    item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    item.IdentificadorGasto = itemGasto.Identificador;
                    await Database.SalvarGastoAtracao(item);
                }
                foreach (var item in itemGasto.Hoteis.Where(d => !d.DataExclusao.HasValue || d.Identificador > 0))
                {
                    item.AtualizadoBanco = false;
                    item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    item.IdentificadorGasto = itemGasto.Identificador;
                    await Database.SalvarGastoHotel(item);
                }
                foreach (var item in itemGasto.Reabastecimentos.Where(d => !d.DataExclusao.HasValue || d.Identificador > 0))
                {
                    item.AtualizadoBanco = false;
                    item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    item.IdentificadorGasto = itemGasto.Identificador;
                    await Database.SalvarReabastecimentoGasto(item);
                }
                foreach (var item in itemGasto.Refeicoes.Where(d => !d.DataExclusao.HasValue || d.Identificador > 0))
                {
                    item.AtualizadoBanco = false;
                    item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    item.IdentificadorGasto = itemGasto.Identificador;
                    await Database.SalvarGastoRefeicao(item);
                }
                foreach (var item in itemGasto.ViagenAereas.Where(d => !d.DataExclusao.HasValue || d.Identificador > 0))
                {
                    item.AtualizadoBanco = false;
                    item.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    item.IdentificadorGasto = itemGasto.Identificador;
                    await Database.SalvarGastoViagemAerea(item);
                }
                await Database.ExcluirGastoDividido_IdentificadorGasto(itemGasto.Identificador.GetValueOrDefault());
                foreach (var item in itemGasto.Usuarios)
                {
                    item.AtualizadoBanco = false;
                    item.IdentificadorGasto = itemGasto.Identificador;
                    await Database.SalvarGastoDividido(item);
                }

                Erros.Add(new MensagemErro() { Mensagem = "Gasto salvo com sucesso" });
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
                await Database.SalvarComentario(itemComentario);

                itemResultado.IdentificadorRegistro = itemComentario.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Comentário salvo com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();


            return itemResultado;
        }

        internal static async Task<AporteDinheiro> CarregarAporteDinheiro(int? identificador)
        {
            var itemAporte = await Database.RetornarAporteDinheiro(identificador);
            if (itemAporte.IdentificadorGasto.HasValue)
                itemAporte.ItemGasto = await CarregarGasto(itemAporte.IdentificadorGasto);
            return itemAporte;
        }

        public static async Task<Gasto> CarregarGasto(int? identificadorGasto)
        {
            var itemGasto = await Database.RetornarGasto(identificadorGasto);
            itemGasto.Alugueis = new ObservableRangeCollection<AluguelGasto>(await Database.ListarAluguelGasto_IdentificadorGasto(identificadorGasto));
            itemGasto.Atracoes = new ObservableRangeCollection<GastoAtracao>(await Database.ListarGastoAtracao_IdentificadorGasto(identificadorGasto));
            itemGasto.Compras = new ObservableRangeCollection<GastoCompra>(await Database.ListarGastoCompra_IdentificadorGasto(identificadorGasto));
            itemGasto.Hoteis = new ObservableRangeCollection<GastoHotel>(await Database.ListarGastoHotel_IdentificadorGasto(identificadorGasto));
            itemGasto.Reabastecimentos = new ObservableRangeCollection<ReabastecimentoGasto>(await Database.ListarReabastecimentoGasto_IdentificadorGasto(identificadorGasto));
            itemGasto.Refeicoes = new ObservableRangeCollection<GastoRefeicao>(await Database.ListarGastoRefeicao_IdentificadorGasto(identificadorGasto));
            itemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(identificadorGasto));
            itemGasto.ViagenAereas = new ObservableRangeCollection<GastoViagemAerea>(await Database.ListarGastoViagemAerea_IdentificadorGasto(identificadorGasto));

            return itemGasto;
        }

        internal static async Task ExcluirAporteDinheiro(AporteDinheiro item, bool AtualizadoBanco)
        {

            var itemAporte = await CarregarAporteDinheiro(item.Identificador);
            itemAporte.DataExclusao = DateTime.Now.ToUniversalTime();
            itemAporte.AtualizadoBanco = AtualizadoBanco;
            if (itemAporte.Identificador > 0 && !AtualizadoBanco)
                await Database.SalvarAporteDinheiro(itemAporte);
            else
                await Database.ExcluirAporteDinheiro(itemAporte);
            if (itemAporte.ItemGasto != null)
            {
                await ExcluirGasto(itemAporte.ItemGasto, AtualizadoBanco);
            }
        }

        internal static async Task AtualizarBancoRecepcaoAcao(string tipo, int identificador, UsuarioLogado itemUsuario)
        {
            if (tipo == "V")
                await VerificarAtualizacaoViagem(identificador);
            else if (tipo == "LC")
                await VerificarAtualizacaoListaCompra(identificador);
            else if (tipo == "T")
                await VerificarAtualizacaoComentario(identificador, itemUsuario.Codigo);
            else if (tipo == "AP")
                await VerificarAtualizacaoAporteDinheiro(identificador, itemUsuario.Codigo);
            else if (tipo == "CM")
                await VerificarAtualizacaoCotacaoMoeda(identificador);
            else if (tipo == "CP")
                await VerificarAtualizacaoCalendarioPrevisto(identificador);
            else if (tipo == "S")
                await VerificarAtualizacaoSugestao(identificador);
            else if (tipo == "HE")
                await VerificarAtualizacaoHotelEvento(identificador);
            else if (tipo == "IC")
                await VerificarAtualizacaoItemCompra(identificador);
            else if (tipo == "GA")
                await VerificarAtualizacaoGastoAtracao(identificador);
            else if (tipo == "GH")
                await VerificarAtualizacaoGastoHotel(identificador);
            else if (tipo == "GR")
                await VerificarAtualizacaoGastoRefeicao(identificador);
            else if (tipo == "GV")
                await VerificarAtualizacaoGastoViagemAerea(identificador);
            else if (tipo == "GC")
                await VerificarAtualizacaoAluguelGasto(identificador);
            else if (tipo == "GL")
                await VerificarAtualizacaoGastoCompra(identificador);
            else if (tipo == "CD")
                await VerificarAtualizacaoCarroDeslocamento(identificador);
            else if (tipo == "CR")
                await VerificarAtualizacaoReabastecimento(identificador);
            else if (tipo == "G")
                await VerificarAtualizacaoGasto(identificador);
            else if (tipo == "A")
                await VerificarAtualizacaoAtracao(identificador);
            else if (tipo == "H")
                await VerificarAtualizacaoHotel(identificador);
            else if (tipo == "VA")
                await VerificarAtualizacaoViagemAerea(identificador);
            else if (tipo == "C")
                await VerificarAtualizacaoCarro(identificador);
            else if (tipo == "R")
                await VerificarAtualizacaoRefeicao(identificador);

            //var itemControle = await DatabaseService.Database.GetControleSincronizacaoAsync();
            //if (itemControle.SincronizadoEnvio)
            //{
            //    itemControle.UltimaDataRecepcao = DateTime.Now.ToUniversalTime();
            //    await DatabaseService.Database.SalvarControleSincronizacao(itemControle);
            //}
        }

        private static async Task VerificarAtualizacaoRefeicao(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarRefeicao(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    await ExcluirRefeicao(identificador, true);
                }
                else
                {
                    var itemBanco = await Database.RetornarRefeicao(identificador);
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemLista.DataAtualizacao)
                        await SalvarRefeicaoReplicada(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoCarro(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarCarro(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    await ExcluirCarro(identificador, true);
                }
                else
                {
                    var itemBanco = await Database.RetornarCarro(identificador);
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemLista.DataAtualizacao)
                        await SalvarCarroReplicada(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoViagemAerea(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarViagemAerea(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    await ExcluirViagemAerea(identificador, true);
                }
                else
                {
                    var itemBanco = await Database.RetornarViagemAerea(identificador);
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemLista.DataAtualizacao)
                        await SalvarViagemAereaReplicada(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoHotel(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarHotel(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    await ExcluirHotel(identificador, true);
                }
                else
                {
                    var itemBanco = await Database.RetornarHotel(identificador);
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemLista.DataAtualizacao)
                        await SalvarHotelReplicada(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoAtracao(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarAtracao(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    await ExcluirAtracao(identificador, true);
                }
                else
                {
                    var itemBanco = await Database.RetornarAtracao(identificador);
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemLista.DataAtualizacao)
                        await SalvarAtracaoReplicada(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoGasto(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarGasto(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    await ExcluirGasto(itemLista, true);
                }
                else
                {
                    var itemBanco = await Database.RetornarGasto(identificador);
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemLista.DataAtualizacao)
                         SalvarGastoSincronizado(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoCarroDeslocamento(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarCarroDeslocamento(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    await ExcluirCarroDeslocamento(identificador, true);                    
                }
                else
                {
                    var itemBanco = await Database.RetornarCarroDeslocamento(identificador);
                    if (itemBanco == null   || itemBanco.DataAtualizacao < itemLista.DataAtualizacao)                        
                        await SalvarCarroDeslocamentoReplicada(itemLista);
                }
            }
        }
        private static async Task VerificarAtualizacaoReabastecimento(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarReabastecimento(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    await ExcluirReabastecimento(identificador, true);
                }
                else
                {
                    var itemBanco = await Database.RetornarCarroDeslocamento(identificador);
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemLista.DataAtualizacao)
                        await SalvarReabastecimentoReplicada(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoGastoCompra(int identificador)
        {
            using (ApiService srv = new ApiService())
            {

                var itemLista = await srv.CarregarGastoCompra(identificador);
                var itemBanco = await Database.RetornarGastoCompra(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                   await ExcluirGastoCompra(identificador, true);
                    
                }
                else
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemLista.DataAtualizacao)
                        await SalvarGastoCompraReplicada(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoAluguelGasto(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarAluguelGasto(identificador);
                var itemBanco = await Database.RetornarAluguelGasto(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    if (itemBanco != null)
                        await Database.ExcluirAluguelGasto(itemBanco);
                }
                else
                {
                    if (itemBanco != null)
                    {
                        if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                            return;
                        else
                            itemLista.Id = itemBanco.Id;
                    }
                    await Database.SalvarAluguelGasto(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoGastoViagemAerea(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarGastoViagemAerea(identificador);
                var itemBanco = await Database.RetornarGastoViagemAerea(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    if (itemBanco != null)
                        await Database.ExcluirGastoViagemAerea(itemBanco);
                }
                else
                {
                    if (itemBanco != null)
                    {
                        if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                            return;
                        else
                            itemLista.Id = itemBanco.Id;
                    }
                    await Database.SalvarGastoViagemAerea(itemLista);
                }
            }
        }


        private static async Task VerificarAtualizacaoGastoRefeicao(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarGastoRefeicao(identificador);
                var itemBanco = await Database.RetornarGastoRefeicao(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    if (itemBanco != null)
                        await Database.ExcluirGastoRefeicao(itemBanco);
                }
                else
                {
                    if (itemBanco != null)
                    {
                        if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                            return;
                        else
                            itemLista.Id = itemBanco.Id;
                    }
                    await Database.SalvarGastoRefeicao(itemLista);
                }
            }
        }
        private static async Task VerificarAtualizacaoGastoHotel(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarGastoHotel(identificador);
                var itemBanco = await Database.RetornarGastoHotel(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    if (itemBanco != null)
                        await Database.ExcluirGastoHotel(itemBanco);
                }
                else
                {
                    if (itemBanco != null)
                    {
                        if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                            return;
                        else
                            itemLista.Id = itemBanco.Id;
                    }
                    await Database.SalvarGastoHotel(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoGastoAtracao(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarGastoAtracao(identificador);
                var itemBanco = await Database.RetornarGastoAtracao(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    if (itemBanco != null)
                        await Database.ExcluirGastoAtracao(itemBanco);
                }
                else
                {
                    if (itemBanco != null)
                    {
                        if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                            return;
                        else
                            itemLista.Id = itemBanco.Id;
                    }
                    await Database.SalvarGastoAtracao(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoItemCompra(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarItemCompra(identificador);
                var itemBanco = await Database.RetornarItemCompra(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    if (itemBanco != null)
                        await Database.ExcluirItemCompra(itemBanco);
                }
                else
                {
                    if (itemBanco != null)
                    {
                        if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                            return;
                        else
                            itemLista.Id = itemBanco.Id;
                    }
                    await Database.SalvarItemCompra(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoHotelEvento(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarHotelEvento(identificador);
                var itemBanco = await Database.RetornarHotelEvento(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    if (itemBanco != null)
                        await Database.ExcluirHotelEvento(itemBanco);
                }
                else
                {
                    if (itemBanco != null)
                    {
                        if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                            return;
                        else
                            itemLista.Id = itemBanco.Id;
                    }
                    await Database.SalvarHotelEvento(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoSugestao(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarSugestao(identificador);
                var itemBanco = await Database.RetornarSugestao(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    if (itemBanco != null)
                        await Database.ExcluirSugestao(itemBanco);
                }
                else
                {
                    if (itemBanco != null)
                    {
                        if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                            return;
                        else
                            itemLista.Id = itemBanco.Id;
                    }
                    await Database.SalvarSugestao(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoCalendarioPrevisto(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarCalendarioPrevisto(identificador);
                var itemBanco = await Database.CarregarCalendarioPrevisto(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    if (itemBanco != null)
                        await Database.ExcluirCalendarioPrevisto(itemBanco);
                }
                else
                { 
                    itemLista.DataProximoAviso = itemLista.DataInicio.GetValueOrDefault(new DateTime(1900, 01, 01));
                    if (itemBanco != null)
                    {
                        if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                            return;
                        else
                        {
                            itemLista.Id = itemBanco.Id;
                        itemLista.DataProximoAviso = itemBanco.DataProximoAviso;
                        }
                    }
                    await Database.SalvarCalendarioPrevisto(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoCotacaoMoeda(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarCotacaoMoeda(identificador);
                var itemBanco = await Database.RetornarCotacaoMoeda(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    if (itemBanco != null)
                        await Database.ExcluirCotacaoMoeda(itemBanco);
                }
                else
                {
                    if (itemBanco != null)
                    {
                        if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                            return;
                        else
                            itemLista.Id = itemBanco.Id;
                    }
                    await Database.SalvarCotacaoMoeda(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoComentario(int identificador, int codigo)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarComentario(identificador);
                if (itemLista.IdentificadorUsuario == codigo)
                {
                    var itemBanco = await Database.RetornarComentario(identificador);
                    if (itemLista.DataExclusao.HasValue)
                    {
                        if (itemBanco != null)
                            await Database.ExcluirComentario(itemBanco);
                    }
                    else
                    {
                        if (itemBanco != null)
                        {
                            if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                                return;
                            else
                                itemLista.Id = itemBanco.Id;
                        }
                        await Database.SalvarComentario(itemLista);
                    }
                }
            }
        }


        private static async Task VerificarAtualizacaoAporteDinheiro(int identificador, int codigo)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarAporteDinheiro(identificador);
                if (itemLista.IdentificadorUsuario == codigo)
                {
                    var itemBanco = await CarregarAporteDinheiro(identificador);
                    if (itemLista.DataExclusao.HasValue)
                    {
                        await ExcluirAporteDinheiro(itemBanco, true);
                    }
                    else
                    {
                        if (itemBanco != null)
                        {
                            if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                                return;
                            else
                                itemLista.Id = itemBanco.Id;
                        }

                        if (itemLista.ItemGasto != null)
                        {
                            var itemGL = await DatabaseService.Database.RetornarGasto(itemLista.ItemGasto.Identificador);
                            if (itemGL != null)
                                itemLista.ItemGasto.Id = itemGL.Id;
                        }
                        await GravarDadosAporte(itemLista);
                    }
                }
            }
        }

        private static async Task VerificarAtualizacaoListaCompra(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemLista = await srv.CarregarListaCompra(identificador);
                var itemBanco = await Database.RetornarListaCompra(identificador);
                if (itemLista.DataExclusao.HasValue)
                {
                    if (itemBanco != null)
                        await Database.ExcluirListaCompra(itemBanco);
                }
                else
                {
                    if (itemBanco != null)
                    {
                        if (itemBanco.DataAtualizacao > itemLista.DataAtualizacao)
                            return;
                        else
                            itemLista.Id = itemBanco.Id;
                    }
                    await Database.SalvarListaCompra(itemLista);
                }
            }
        }

        private static async Task VerificarAtualizacaoViagem(int identificador)
        {
            using (ApiService srv = new ApiService())
            {
                var itemViagem = await srv.CarregarViagem(identificador);
                SincronizarParticipanteViagem(itemViagem);
                var ItemViagemLocal = await Database.GetViagemAtualAsync();
                itemViagem.Id = ItemViagemLocal.Id;
                itemViagem.ControlaPosicaoGPS = ItemViagemLocal.ControlaPosicaoGPS;
                itemViagem.Edicao = ItemViagemLocal.Edicao;
                itemViagem.VejoGastos = ItemViagemLocal.VejoGastos;
                await Database.SalvarViagemAsync(itemViagem);
            }
        }

        public static async Task ExcluirGasto(Gasto itemGasto, bool AtualizadoBanco)
        {
            if (itemGasto.Identificador > 0 && !AtualizadoBanco)
            {
                itemGasto.AtualizadoBanco = AtualizadoBanco;
                itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                await Database.SalvarGasto(itemGasto);
                foreach (var item in itemGasto.Alugueis)
                {
                    item.AtualizadoBanco = AtualizadoBanco;
                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    await Database.SalvarAluguelGasto(item);
                }
                foreach (var item in itemGasto.Compras)
                {
                    item.AtualizadoBanco = AtualizadoBanco;

                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    await Database.SalvarGastoCompra(item);
                }
                foreach (var item in itemGasto.Atracoes)
                {
                    item.AtualizadoBanco = AtualizadoBanco;

                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    await Database.SalvarGastoAtracao(item);
                }
                foreach (var item in itemGasto.Hoteis)
                {
                    item.AtualizadoBanco = AtualizadoBanco;

                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    await Database.SalvarGastoHotel(item);
                }
                foreach (var item in itemGasto.Reabastecimentos)
                {
                    item.AtualizadoBanco = AtualizadoBanco;

                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    await Database.SalvarReabastecimentoGasto(item);
                }
                foreach (var item in itemGasto.Refeicoes)
                {
                    item.AtualizadoBanco = AtualizadoBanco;

                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    await Database.SalvarGastoRefeicao(item);
                }
                foreach (var item in itemGasto.ViagenAereas)
                {
                    item.AtualizadoBanco = AtualizadoBanco;

                    item.DataExclusao = DateTime.Now.ToUniversalTime();
                    await Database.SalvarGastoViagemAerea(item);
                }
            }
            else
            {
                await Database.ExcluirGasto(itemGasto);
                await Database.Excluir_Gasto_Filhos(itemGasto.Identificador.GetValueOrDefault());
            }

        }

        internal async static Task AjustarDePara(ClasseSincronizacao itemEnvio, List<DeParaIdentificador> resultadoSincronizacao, ControleSincronizacao itemCS)
        {
            await Database.SalvarControleSincronizacao(itemCS);
            await AjustarDadosAporte(itemEnvio, resultadoSincronizacao);
            await AjustarDadosAtracao(itemEnvio.Atracoes, resultadoSincronizacao);
            await AjustarDadosCalendarioPrevisto(itemEnvio.CalendariosPrevistos, resultadoSincronizacao);
            await AjustarDadosCarroDeslocamento(itemEnvio.CarroDeslocamentos, resultadoSincronizacao);
            await AjustarDadosCarro(itemEnvio.Carros, resultadoSincronizacao);
            await AjustarDadosReabastecimento(itemEnvio.Reabastecimento, resultadoSincronizacao);
            await AjustarDadosComentario(itemEnvio.Comentarios, resultadoSincronizacao);
            await AjustarDadosCotacaoMoeda(itemEnvio.CotacoesMoeda, resultadoSincronizacao);
            await AjustarDadosCompra(itemEnvio.Compras, resultadoSincronizacao);
            await AjustarDadosDeslocamentos(itemEnvio.Deslocamentos, resultadoSincronizacao);
            await AjustarDadosEventosHotel(itemEnvio.EventosHotel, resultadoSincronizacao);
            await AjustarDadosGastos(itemEnvio.Gastos, resultadoSincronizacao);
            await AjustarDadosGastosCarro(itemEnvio.GastosCarro, resultadoSincronizacao);
            await AjustarDadosGastosAtracao(itemEnvio.GastosAtracao, resultadoSincronizacao);
            await AjustarDadosGastosHotel( itemEnvio.GastosHotel, resultadoSincronizacao);
            await AjustarDadosGastosRefeicao(itemEnvio.GastosRefeicao, resultadoSincronizacao);
            await AjustarDadosGastosViagemAerea(itemEnvio.GastosDeslocamento, resultadoSincronizacao);
            await AjustarDadosHotel(itemEnvio.Hoteis, resultadoSincronizacao);
            await AjustarDadosItensCompra(itemEnvio.ItensComprados, resultadoSincronizacao);
            await AjustarDadosListaCompra(itemEnvio.ListaCompra, resultadoSincronizacao);
            await AjustarDadosLoja(itemEnvio.Lojas, resultadoSincronizacao);
            await AjustarDadosSugestao(itemEnvio.Sugestoes, resultadoSincronizacao);
            foreach (var item in itemEnvio.Posicoes)
                await Database.ExcluirPosicao(item);
        }

        private static async Task AjustarDadosSugestao(List<Sugestao> sugestoes, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in sugestoes)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.ExcluirSugestao(item);
                }
                else
                {

                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "S").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                        if (itemDePara.ItemRetorno != null)
                        {
                            var Jresultado = (JObject)itemDePara.ItemRetorno;
                            var itemGravado = Jresultado.ToObject<Sugestao>();
                            item.NomeCidade = itemGravado.NomeCidade;
                            item.IdentificadorCidade = itemGravado.IdentificadorCidade;
                        }
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarSugestao(item);
                }
            }
        }



        private async static Task AjustarDadosRefeicao(List<Refeicao> refeicoes, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in refeicoes)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.ExcluirRefeicaoPedido_IdentificadorRefeicao(item.Identificador);
                    await Database.ExcluirRefeicao(item);
                }
                else
                {
                    if (item.IdentificadorAtracao.HasValue)
                    {
                        DeParaIdentificador itemPai = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorAtracao && d.TipoObjeto == "A").FirstOrDefault();
                        if (itemPai != null && item.IdentificadorAtracao != itemPai.IdentificadorDetino)
                            item.IdentificadorAtracao = itemPai.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "R").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                        if (itemDePara.ItemRetorno != null)
                        {
                            var Jresultado = (JObject)itemDePara.ItemRetorno;
                            var itemGravado = Jresultado.ToObject<Refeicao>();
                            item.NomeCidade = itemGravado.NomeCidade;
                            item.IdentificadorCidade = itemGravado.IdentificadorCidade;
                        }
                    }
                    if (itemDePara.IdentificadorOrigem != itemDePara.IdentificadorDetino)
                    {
                        var UploadsArquivo = await Database.ListarUploadFoto_IdentificadorRefeicao(itemDePara.IdentificadorOrigem);
                        foreach (var itemVideo in UploadsArquivo)
                        {
                            itemVideo.IdentificadorRefeicao = itemDePara.IdentificadorDetino;
                            await Database.SalvarUploadFoto(itemVideo);
                        }
                    }
                    await Database.SalvarRefeicao(item);
                    foreach (var itemAvaliacao in item.Pedidos)
                    {
                        itemAvaliacao.AtualizadoBanco = true;
                        itemAvaliacao.IdentificadorRefeicao = item.Identificador;
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemAvaliacao.Identificador && d.TipoObjeto == "RP").FirstOrDefault();
                        if (itemDeParaAvaliacao != null)
                            itemAvaliacao.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                        await Database.SalvarRefeicaoPedido(itemAvaliacao);
                    }
                }

            }
        }


        private async static Task AjustarDadosLoja(List<Loja> atracoes, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in atracoes)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.ExcluirAvaliacaoLoja_IdentificadorLoja(item.Identificador);
                    await Database.ExcluirLoja(item);
                }
                else
                {
                    if (item.IdentificadorAtracao.HasValue)
                    {
                        DeParaIdentificador itemPai = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorAtracao && d.TipoObjeto == "A").FirstOrDefault();
                        if (itemPai != null && item.IdentificadorAtracao != itemPai.IdentificadorDetino)
                            item.IdentificadorAtracao = itemPai.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "L").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                        if (itemDePara.ItemRetorno != null)
                        {
                            var Jresultado = (JObject)itemDePara.ItemRetorno;
                            var itemGravado = Jresultado.ToObject<Loja>();
                            item.NomeCidade = itemGravado.NomeCidade;
                            item.IdentificadorCidade = itemGravado.IdentificadorCidade;
                        }
                    }
                    await Database.SalvarLoja(item);
                    foreach (var itemAvaliacao in item.Avaliacoes)
                    {
                        itemAvaliacao.AtualizadoBanco = true;
                        itemAvaliacao.IdentificadorLoja = item.Identificador;
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemAvaliacao.Identificador && d.TipoObjeto == "AL").FirstOrDefault();
                        if (itemDeParaAvaliacao != null)
                            itemAvaliacao.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                        await Database.SalvarAvaliacaoLoja(itemAvaliacao);
                    }
                }

            }
        }


        private static async Task AjustarDadosListaCompra(List<ListaCompra> listaCompra, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in listaCompra)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.ExcluirListaCompra(item);
                }
                else
                {

                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "LC").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarListaCompra(item);
                }
            }
        }


        private async static Task AjustarDadosItensCompra(List<ItemCompra> itensComprados, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in itensComprados)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.ExcluirItemCompra(item);

                }
                else
                {
                    if (item.IdentificadorListaCompra.HasValue)
                    {
                        DeParaIdentificador itemCusto = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorListaCompra && d.TipoObjeto == "LC").FirstOrDefault();
                        if (itemCusto != null && itemCusto.IdentificadorDetino != itemCusto.IdentificadorOrigem)
                        {
                            item.IdentificadorListaCompra = itemCusto.IdentificadorDetino;
                        }

                    }
                    DeParaIdentificador deParaLoja = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorGastoCompra && d.TipoObjeto == "GC").FirstOrDefault();
                    if (deParaLoja != null && deParaLoja.IdentificadorDetino != deParaLoja.IdentificadorOrigem)
                        item.IdentificadorGastoCompra = deParaLoja.IdentificadorDetino;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "IC").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarItemCompra(item);
                }
            }
        }

        private async static Task AjustarDadosHotel(List<Hotel> hoteis, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in hoteis)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.ExcluirHotelAvaliacao_IdentificadorHotel(item.Identificador);
                    await Database.ExcluirHotel(item);
                }
                else
                {
                   
                    item.AtualizadoBanco = true;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "H").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                        if (itemDePara.ItemRetorno != null)
                        {
                            var Jresultado = (JObject)itemDePara.ItemRetorno;
                            var itemGravado = Jresultado.ToObject<Hotel>();
                            item.NomeCidade = itemGravado.NomeCidade;
                            item.IdentificadorCidade = itemGravado.IdentificadorCidade;
                        }

                        if (itemDePara.IdentificadorOrigem != itemDePara.IdentificadorDetino)
                        {
                            var UploadsArquivo = await Database.ListarUploadFoto_IdentificadorHotel(itemDePara.IdentificadorOrigem);
                            foreach (var itemVideo in UploadsArquivo)
                            {
                                itemVideo.IdentificadorRefeicao = itemDePara.IdentificadorDetino;
                                await Database.SalvarUploadFoto(itemVideo);
                            }
                        }
                    }
                    await Database.SalvarHotel(item);
                    foreach (var itemAvaliacao in item.Avaliacoes)
                    {
                        itemAvaliacao.AtualizadoBanco = true;
                        itemAvaliacao.IdentificadorHotel = item.Identificador;
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemAvaliacao.Identificador && d.TipoObjeto == "AH").FirstOrDefault();
                        if (itemDeParaAvaliacao != null)
                            itemAvaliacao.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                        await Database.SalvarHotelAvaliacao(itemAvaliacao);
                    }
                }

            }
        }
        private async static Task AjustarDadosGastosViagemAerea(List<GastoViagemAerea> gastos, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in gastos)
            {
                if (item.DataExclusao.HasValue)
                {
                    if (item.ItemGasto != null)
                        await Database.ExcluirGasto(item.ItemGasto);
                    await Database.ExcluirGastoViagemAerea(item);

                }
                else
                {
                    if (item.IdentificadorGasto.HasValue)
                    {
                        DeParaIdentificador itemCusto = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                        if (itemCusto != null && itemCusto.IdentificadorDetino != itemCusto.IdentificadorOrigem)
                            item.IdentificadorGasto = itemCusto.IdentificadorDetino;


                    }
                    DeParaIdentificador deParaLoja = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorViagemAerea && d.TipoObjeto == "A").FirstOrDefault();
                    if (deParaLoja != null && deParaLoja.IdentificadorDetino != deParaLoja.IdentificadorOrigem)
                        item.IdentificadorViagemAerea = deParaLoja.IdentificadorDetino;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "GA").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarGastoViagemAerea(item);
                }
            }
        }


        private async static Task AjustarDadosGastosRefeicao(List<GastoRefeicao> gastos, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in gastos)
            {
                if (item.DataExclusao.HasValue)
                {
                    if (item.ItemGasto != null)
                        await Database.ExcluirGasto(item.ItemGasto);
                    await Database.ExcluirGastoRefeicao(item);

                }
                else
                {
                    if (item.IdentificadorGasto.HasValue)
                    {
                        DeParaIdentificador itemCusto = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                        if (itemCusto != null && itemCusto.IdentificadorDetino != itemCusto.IdentificadorOrigem)
                            item.IdentificadorGasto = itemCusto.IdentificadorDetino;


                    }
                    DeParaIdentificador deParaLoja = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorRefeicao && d.TipoObjeto == "A").FirstOrDefault();
                    if (deParaLoja != null && deParaLoja.IdentificadorDetino != deParaLoja.IdentificadorOrigem)
                        item.IdentificadorRefeicao = deParaLoja.IdentificadorDetino;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "GA").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarGastoRefeicao(item);
                }
            }
        }


        private async static Task AjustarDadosGastosHotel(List<GastoHotel> gastos, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in gastos)
            {
                if (item.DataExclusao.HasValue)
                {
                    if (item.ItemGasto != null)
                        await Database.ExcluirGasto(item.ItemGasto);
                    await Database.ExcluirGastoHotel(item);

                }
                else
                {
                    if (item.IdentificadorGasto.HasValue)
                    {
                        DeParaIdentificador itemCusto = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                        if (itemCusto != null && itemCusto.IdentificadorDetino != itemCusto.IdentificadorOrigem)
                            item.IdentificadorGasto = itemCusto.IdentificadorDetino;


                    }
                    DeParaIdentificador deParaLoja = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorHotel && d.TipoObjeto == "A").FirstOrDefault();
                    if (deParaLoja != null && deParaLoja.IdentificadorDetino != deParaLoja.IdentificadorOrigem)
                        item.IdentificadorHotel = deParaLoja.IdentificadorDetino;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "GA").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarGastoHotel(item);
                }
            }
        }


        private async static Task AjustarDadosGastosAtracao(List<GastoAtracao> gastos, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in gastos)
            {
                if (item.DataExclusao.HasValue)
                {
                    if (item.ItemGasto != null)
                        await Database.ExcluirGasto(item.ItemGasto);
                    await Database.ExcluirGastoAtracao(item);

                }
                else
                {
                    if (item.IdentificadorGasto.HasValue)
                    {
                        DeParaIdentificador itemCusto = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                        if (itemCusto != null && itemCusto.IdentificadorDetino != itemCusto.IdentificadorOrigem)
                        {
                            item.IdentificadorGasto = itemCusto.IdentificadorDetino;

                        }

                    }
                    DeParaIdentificador deParaLoja = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorAtracao && d.TipoObjeto == "A").FirstOrDefault();
                    if (deParaLoja != null && deParaLoja.IdentificadorDetino != deParaLoja.IdentificadorOrigem)
                        item.IdentificadorAtracao = deParaLoja.IdentificadorDetino;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "GA").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarGastoAtracao(item);
                }
            }
        }

        private async static Task AjustarDadosGastosCarro(List<AluguelGasto> gastos, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in gastos)
            {
                if (item.DataExclusao.HasValue)
                {
                    if (item.ItemGasto != null)
                        await Database.ExcluirGasto(item.ItemGasto);
                    await Database.ExcluirAluguelGasto(item);

                }
                else
                {
                    if (item.IdentificadorGasto.HasValue)
                    {
                        DeParaIdentificador itemCusto = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                        if (itemCusto != null && itemCusto.IdentificadorDetino != itemCusto.IdentificadorOrigem)
                            item.IdentificadorGasto = itemCusto.IdentificadorDetino;


                    }
                    DeParaIdentificador deParaLoja = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorCarro && d.TipoObjeto == "C").FirstOrDefault();
                    if (deParaLoja != null && deParaLoja.IdentificadorDetino != deParaLoja.IdentificadorOrigem)
                        item.IdentificadorCarro = deParaLoja.IdentificadorDetino;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "GC").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarAluguelGasto(item);
                }
            }
        }

        private static async Task AjustarDadosGastos(List<Gasto> gastos, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in gastos)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.Excluir_Gasto_Filhos(item.Identificador.GetValueOrDefault());
                }
                else
                {

                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "G").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarGasto(item);
                    foreach (var itemUsuario in item.Usuarios)
                    {
                        itemUsuario.IdentificadorGasto = item.Identificador;
                        itemUsuario.AtualizadoBanco = true;
                        await Database.SalvarGastoDividido(itemUsuario);
                    }

                    foreach (var itemRelacao in item.Alugueis.Where(d=>!d.AtualizadoBanco))
                    {
                        if (itemRelacao.DataExclusao.HasValue)
                            await Database.ExcluirAluguelGasto(itemRelacao);
                        else
                        {
                            itemRelacao.IdentificadorGasto = item.Identificador;
                            itemRelacao.AtualizadoBanco = true;
                            DeParaIdentificador deParaPai = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemRelacao.IdentificadorCarro && d.TipoObjeto == "C").FirstOrDefault();
                            if (deParaPai != null && deParaPai.IdentificadorDetino != deParaPai.IdentificadorOrigem)
                                itemRelacao.IdentificadorCarro = deParaPai.IdentificadorDetino;
                            await Database.SalvarAluguelGasto(itemRelacao);
                        }
                    }

                    foreach (var itemRelacao in item.Atracoes.Where(d => !d.AtualizadoBanco))
                    {
                        if (itemRelacao.DataExclusao.HasValue)
                            await Database.ExcluirGastoAtracao(itemRelacao);
                        else
                        {
                            itemRelacao.IdentificadorGasto = item.Identificador;
                            itemRelacao.AtualizadoBanco = true;
                            DeParaIdentificador deParaPai = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemRelacao.IdentificadorAtracao && d.TipoObjeto == "A").FirstOrDefault();
                            if (deParaPai != null && deParaPai.IdentificadorDetino != deParaPai.IdentificadorOrigem)
                                itemRelacao.IdentificadorAtracao = deParaPai.IdentificadorDetino;
                            await Database.SalvarGastoAtracao(itemRelacao);
                        }
                    }
                    foreach (var itemRelacao in item.Hoteis.Where(d => !d.AtualizadoBanco))
                    {
                        if (itemRelacao.DataExclusao.HasValue)
                            await Database.ExcluirGastoHotel(itemRelacao);
                        else
                        {
                            itemRelacao.IdentificadorGasto = item.Identificador;
                            itemRelacao.AtualizadoBanco = true;
                            DeParaIdentificador deParaPai = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemRelacao.IdentificadorHotel && d.TipoObjeto == "H").FirstOrDefault();
                            if (deParaPai != null && deParaPai.IdentificadorDetino != deParaPai.IdentificadorOrigem)
                                itemRelacao.IdentificadorHotel = deParaPai.IdentificadorDetino;
                            await Database.SalvarGastoHotel(itemRelacao);
                        }
                    }

                    foreach (var itemRelacao in item.Refeicoes.Where(d => !d.AtualizadoBanco))
                    {
                        if (itemRelacao.DataExclusao.HasValue)
                            await Database.ExcluirGastoRefeicao(itemRelacao);
                        else
                        {
                            itemRelacao.IdentificadorGasto = item.Identificador;
                            itemRelacao.AtualizadoBanco = true;
                            DeParaIdentificador deParaPai = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemRelacao.IdentificadorRefeicao && d.TipoObjeto == "R").FirstOrDefault();
                            if (deParaPai != null && deParaPai.IdentificadorDetino != deParaPai.IdentificadorOrigem)
                                itemRelacao.IdentificadorRefeicao = deParaPai.IdentificadorDetino;
                            await Database.SalvarGastoRefeicao(itemRelacao);
                        }
                    }

                    foreach (var itemRelacao in item.ViagenAereas.Where(d => !d.AtualizadoBanco))
                    {
                        if (itemRelacao.DataExclusao.HasValue)
                            await Database.ExcluirGastoViagemAerea(itemRelacao);
                        else
                        {
                            itemRelacao.IdentificadorGasto = item.Identificador;
                            itemRelacao.AtualizadoBanco = true;
                            DeParaIdentificador deParaPai = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemRelacao.IdentificadorViagemAerea && d.TipoObjeto == "VA").FirstOrDefault();
                            if (deParaPai != null && deParaPai.IdentificadorDetino != deParaPai.IdentificadorOrigem)
                                itemRelacao.IdentificadorViagemAerea = deParaPai.IdentificadorDetino;
                            await Database.SalvarGastoViagemAerea(itemRelacao);
                        }
                    }
                }
            }
        }

        private static async Task AjustarDadosEventosHotel(List<HotelEvento> eventosHotel, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in eventosHotel)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.ExcluirHotelEvento(item);
                }
                else
                {

                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "HE").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarHotelEvento(item);
                }
            }
        }

        private static async Task AjustarDadosDeslocamentos(List<ViagemAerea> deslocamentos, List<DeParaIdentificador> resultadoSincronizacao)
        {

            foreach (var item in deslocamentos)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.ExcluirAvaliacaoAerea_IdentificadorViagemAerea(item.Identificador);
                    await Database.ExcluirViagemAereaAeroporto_IdentificadorViagemAerea(item.Identificador);
                    await Database.ExcluirViagemAerea(item);
                }
                else
                {

                    item.AtualizadoBanco = true;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "VA").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;

                    }
                    await Database.SalvarViagemAerea(item);
                    foreach (var itemAvaliacao in item.Aeroportos)
                    {

                        itemAvaliacao.AtualizadoBanco = true;
                        itemAvaliacao.IdentificadorViagemAerea = item.Identificador;
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemAvaliacao.Identificador && d.TipoObjeto == "VAA").FirstOrDefault();

                        if (itemDeParaAvaliacao != null)
                        {
                            itemAvaliacao.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                            if (itemDePara.ItemRetorno != null)
                            {
                                var Jresultado = (JObject)itemDePara.ItemRetorno;
                                var itemGravado = Jresultado.ToObject<ViagemAereaAeroporto>();
                                itemAvaliacao.IdentificadorCidade = itemGravado.IdentificadorCidade;
                            }
                        }
                        await Database.SalvarViagemAereaAeroporto(itemAvaliacao);
                    }
                    foreach (var itemAvaliacao in item.Avaliacoes)
                    {

                        itemAvaliacao.AtualizadoBanco = true;
                        itemAvaliacao.IdentificadorViagemAerea = item.Identificador;
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemAvaliacao.Identificador && d.TipoObjeto == "AVA").FirstOrDefault();
                        if (itemDeParaAvaliacao != null)
                            itemAvaliacao.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                        await Database.SalvarAvaliacaoAerea(itemAvaliacao);
                    }
                }


            }
        }

        private static async Task AjustarDadosCompra(List<GastoCompra> compras, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in compras)
            {
                if (item.DataExclusao.HasValue)
                {
                    if (item.ItemGasto != null)
                        await Database.ExcluirGasto(item.ItemGasto);
                    await Database.ExcluirGastoCompra(item);

                }
                else
                {
                    if (item.IdentificadorGasto.HasValue)
                    {
                        DeParaIdentificador itemCusto = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                        if (itemCusto != null && itemCusto.IdentificadorDetino != itemCusto.IdentificadorOrigem)
                        {
                            item.ItemGasto.AtualizadoBanco = true;
                            item.ItemGasto.Identificador = itemCusto.IdentificadorDetino;
                            await Database.SalvarGasto(item.ItemGasto);
                        }
                        item.IdentificadorGasto = item.ItemGasto.Identificador;

                    }
                    DeParaIdentificador deParaLoja = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorLoja && d.TipoObjeto == "L").FirstOrDefault();
                    if (deParaLoja != null && deParaLoja.IdentificadorDetino != deParaLoja.IdentificadorOrigem)
                        item.IdentificadorLoja = deParaLoja.IdentificadorDetino;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "GL").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarGastoCompra(item);
                }
            }
        }

        private static async Task AjustarDadosCotacaoMoeda(List<CotacaoMoeda> CotacaoMoedas, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in CotacaoMoedas)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.ExcluirCotacaoMoeda(item);
                }
                else
                {

                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "CM").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarCotacaoMoeda(item);
                }
            }
        }

        private static async Task AjustarDadosComentario(List<Comentario> comentarios, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in comentarios)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.ExcluirComentario(item);
                }
                else
                {

                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "T").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                        if (itemDePara.ItemRetorno != null)
                        {
                            var Jresultado = (JObject)itemDePara.ItemRetorno;
                            var itemGravado = Jresultado.ToObject<Comentario>();
                            item.NomeCidade = itemGravado.NomeCidade;
                            item.IdentificadorCidade = itemGravado.IdentificadorCidade;
                        }
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarComentario(item);
                }
            }
        }


        private async static Task AjustarDadosReabastecimento(List<Reabastecimento> reabastecimento, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in reabastecimento)
            {
                if (item.DataExclusao.HasValue)
                {
                    foreach (var itemGR in item.Gastos)
                    {
                        if (itemGR.ItemGasto != null)
                            await Database.ExcluirGasto(itemGR.ItemGasto);
                        await Database.ExcluirReabastecimentoGasto(itemGR);
                    }
                    await Database.ExcluirReabastecimento(item);

                }
                else
                {
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "CR").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    foreach (var itemGR in item.Gastos)
                    {
                        DeParaIdentificador itemCusto = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemGR.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                        if (itemCusto != null && itemCusto.IdentificadorDetino != itemCusto.IdentificadorOrigem)
                        {
                            itemGR.ItemGasto.AtualizadoBanco = true;
                            itemGR.ItemGasto.Identificador = itemCusto.IdentificadorDetino;
                            await Database.SalvarGasto(itemGR.ItemGasto);
                            itemGR.IdentificadorGasto = itemCusto.IdentificadorDetino;
                        }

                        itemGR.IdentificadorReabastecimento = item.Identificador;
                        itemGR.AtualizadoBanco = true;
                        await Database.SalvarReabastecimentoGasto(itemGR);
                    }
                 
                    item.AtualizadoBanco = true;
                    await Database.SalvarReabastecimento(item);
                }
            }
        }

        private static async Task AjustarDadosCarro(List<Carro> carros, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in carros)
            {
                if (item.DataExclusao.HasValue)
                {
                    if (item.ItemCarroEventoDevolucao != null)
                        await Database.ExcluirCarroEvento(item.ItemCarroEventoDevolucao);
                    if (item.ItemCarroEventoRetirada != null)
                        await Database.ExcluirCarroEvento(item.ItemCarroEventoRetirada);
                    await Database.ExcluirCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(item.Identificador);
                    await Database.ExcluirCarro(item);
                }
                else
                {

                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "C").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    if (item.IdentificadorCarroEventoDevolucao.HasValue)
                    {
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorCarroEventoDevolucao && d.TipoObjeto == "CE").FirstOrDefault();
                        if (itemDeParaAvaliacao != null && itemDeParaAvaliacao.IdentificadorOrigem != itemDeParaAvaliacao.IdentificadorDetino)
                        {
                            item.IdentificadorCarroEventoDevolucao = itemDeParaAvaliacao.IdentificadorDetino;
                            item.ItemCarroEventoDevolucao.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                        }
                        item.ItemCarroEventoDevolucao.AtualizadoBanco = true;
                        await Database.SalvarCarroEvento(item.ItemCarroEventoDevolucao);
                    }

                    if (item.IdentificadorCarroEventoRetirada.HasValue)
                    {
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorCarroEventoRetirada && d.TipoObjeto == "CE").FirstOrDefault();
                        if (itemDeParaAvaliacao != null && itemDeParaAvaliacao.IdentificadorOrigem != itemDeParaAvaliacao.IdentificadorDetino)
                        {
                            item.IdentificadorCarroEventoRetirada = itemDeParaAvaliacao.IdentificadorDetino;
                            item.ItemCarroEventoRetirada.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                        }
                        item.ItemCarroEventoRetirada.AtualizadoBanco = true;
                        await Database.SalvarCarroEvento(item.ItemCarroEventoRetirada);
                    }
                    foreach (var itemAvaliacao in item.Avaliacoes)
                    {
                        itemAvaliacao.AtualizadoBanco = true;
                        itemAvaliacao.IdentificadorCarro = item.Identificador;
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemAvaliacao.Identificador && d.TipoObjeto == "AC").FirstOrDefault();
                        if (itemDeParaAvaliacao != null)
                            itemAvaliacao.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                        await Database.SalvarAvaliacaoAluguel(itemAvaliacao);

                    }

                    item.AtualizadoBanco = true;
                    await Database.SalvarCarro(item);
                }
            }
        }

        private static async Task AjustarDadosCarroDeslocamento(List<CarroDeslocamento> carroDeslocamentos, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in carroDeslocamentos)
            {
                if (item.DataExclusao.HasValue)
                {
                    if (item.ItemCarroEventoChegada != null)
                        await Database.ExcluirCarroEvento(item.ItemCarroEventoChegada);
                    if (item.ItemCarroEventoPartida != null)
                        await Database.ExcluirCarroEvento(item.ItemCarroEventoPartida);
                    await Database.ExcluirCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(item.Identificador);
                    await Database.ExcluirCarroDeslocamento(item);
                }
                else
                {

                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "CD").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorCarro && d.TipoObjeto == "C").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.IdentificadorCarro = itemDePara.IdentificadorDetino;
                    }
                    if (item.IdentificadorCarroEventoChegada.HasValue)
                    {
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorCarroEventoChegada && d.TipoObjeto == "CE").FirstOrDefault();
                        if (itemDeParaAvaliacao != null && itemDeParaAvaliacao.IdentificadorOrigem != itemDeParaAvaliacao.IdentificadorDetino)
                        {
                            item.IdentificadorCarroEventoChegada = itemDeParaAvaliacao.IdentificadorDetino;
                            item.ItemCarroEventoChegada.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                        }
                        item.ItemCarroEventoChegada.AtualizadoBanco = true;
                        await Database.SalvarCarroEvento(item.ItemCarroEventoChegada);
                    }

                    if (item.IdentificadorCarroEventoPartida.HasValue)
                    {
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorCarroEventoPartida && d.TipoObjeto == "CE").FirstOrDefault();
                        if (itemDeParaAvaliacao != null && itemDeParaAvaliacao.IdentificadorOrigem != itemDeParaAvaliacao.IdentificadorDetino)
                        {
                            item.IdentificadorCarroEventoPartida = itemDeParaAvaliacao.IdentificadorDetino;
                            item.ItemCarroEventoPartida.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                        }
                        item.ItemCarroEventoPartida.AtualizadoBanco = true;
                        await Database.SalvarCarroEvento(item.ItemCarroEventoPartida);
                    }
                    foreach (var itemUsuario in item.Usuarios)
                    {
                        itemUsuario.AtualizadoBanco = true;
                        itemUsuario.IdentificadorCarroDeslocamento = item.Identificador;
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemUsuario.Identificador && d.TipoObjeto == "CDU").FirstOrDefault();
                        if (itemDeParaAvaliacao != null)
                            itemUsuario.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                        await Database.SalvarCarroDeslocamentoUsuario(itemUsuario);

                    }

                    item.AtualizadoBanco = true;
                    await Database.SalvarCarroDeslocamento(item);
                }
            }
        }

        private static async Task AjustarDadosCalendarioPrevisto(List<CalendarioPrevisto> calendariosPrevistos, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in calendariosPrevistos)
            {
                if (item.DataExclusao.HasValue)
                {                    
                    await Database.ExcluirCalendarioPrevisto(item);
                }
                else
                {
                   
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "CP").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarCalendarioPrevisto(item);
                }
            }
        }

        private async static Task AjustarDadosAtracao(List<Atracao> atracoes, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in atracoes)
            {
                if (item.DataExclusao.HasValue)
                {
                    await Database.ExcluirAvaliacaoAtracao_IdentificadorAtracao(item.Identificador);
                    await Database.ExcluirAtracao(item);
                }
                else
                {
                    if (item.IdentificadorAtracaoPai.HasValue)
                    {
                        DeParaIdentificador itemPai = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorAtracaoPai && d.TipoObjeto == "A").FirstOrDefault();
                        if (itemPai != null && item.IdentificadorAtracaoPai != itemPai.IdentificadorDetino)
                            item.IdentificadorAtracaoPai = itemPai.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "A").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                        if (itemDePara.ItemRetorno != null)
                        {
                            var Jresultado = (JObject)itemDePara.ItemRetorno;
                            var itemGravado = Jresultado.ToObject<Atracao>();
                            item.NomeCidade = itemGravado.NomeCidade;
                            item.IdentificadorCidade = itemGravado.IdentificadorCidade;
                        }
                        if (itemDePara.IdentificadorOrigem != itemDePara.IdentificadorDetino)
                        {
                            var UploadsArquivo = await Database.ListarUploadFoto_IdentificadorAtracao(itemDePara.IdentificadorOrigem);
                            foreach (var itemVideo in UploadsArquivo)
                            {
                                itemVideo.IdentificadorRefeicao = itemDePara.IdentificadorDetino;
                                await Database.SalvarUploadFoto(itemVideo);
                            }
                        }
                    }
                   
                    await Database.SalvarAtracao(item);
                    foreach (var itemAvaliacao in item.Avaliacoes)
                    {
                        itemAvaliacao.AtualizadoBanco = true;
                        itemAvaliacao.IdentificadorAtracao = item.Identificador;
                        DeParaIdentificador itemDeParaAvaliacao = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == itemAvaliacao.Identificador && d.TipoObjeto == "AA").FirstOrDefault();
                        if (itemDeParaAvaliacao != null)
                            itemAvaliacao.Identificador = itemDeParaAvaliacao.IdentificadorDetino;
                        await Database.SalvarAvaliacaoAtracao(itemAvaliacao);
                    }
                }

            }
        }

        private static async Task AjustarDadosAporte(ClasseSincronizacao itemEnvio, List<DeParaIdentificador> resultadoSincronizacao)
        {
            foreach (var item in itemEnvio.AportesDinheiro)
            {
                if (item.DataExclusao.HasValue)
                {
                    if (item.ItemGasto != null)
                        await Database.ExcluirGasto(item.ItemGasto);
                    await Database.ExcluirAporteDinheiro(item);

                }
                else
                {
                    if (item.IdentificadorGasto.HasValue)
                    {
                        DeParaIdentificador itemCusto = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.IdentificadorGasto && d.TipoObjeto == "G").FirstOrDefault();
                        if (itemCusto != null && itemCusto.IdentificadorDetino != itemCusto.IdentificadorOrigem)
                        {
                            item.ItemGasto.AtualizadoBanco = true;
                            item.ItemGasto.Identificador = itemCusto.IdentificadorDetino;
                            await Database.SalvarGasto(item.ItemGasto);
                            item.IdentificadorGasto = itemCusto.IdentificadorDetino;
                        }
                    }
                    DeParaIdentificador itemDePara = resultadoSincronizacao.Where(d => d.IdentificadorOrigem == item.Identificador && d.TipoObjeto == "AD").FirstOrDefault();
                    if (itemDePara != null)
                    {
                        item.Identificador = itemDePara.IdentificadorDetino;
                    }
                    item.AtualizadoBanco = true;
                    await Database.SalvarAporteDinheiro(item);
                }
            }
        }

        internal async static  Task SincronizarDadosServidorLocal(ControleSincronizacao itemCS, ClasseSincronizacao dadosSincronizar, UsuarioLogado itemUsuario,DateTime DataSincronizacao)
        {
            await SalvarAmigos(dadosSincronizar.Amigos);
            SincronizarParticipanteViagem(dadosSincronizar.ItemViagem);
            var ItemViagemLocal = await Database.GetViagemAtualAsync();
            dadosSincronizar.ItemViagem.Id = ItemViagemLocal.Id;
            dadosSincronizar.ItemViagem.ControlaPosicaoGPS = ItemViagemLocal.ControlaPosicaoGPS;
            dadosSincronizar.ItemViagem.Edicao = ItemViagemLocal.Edicao;
            dadosSincronizar.ItemViagem.VejoGastos = ItemViagemLocal.VejoGastos;
            await Database.SalvarViagemAsync(dadosSincronizar.ItemViagem);
            await IncluirCidades(dadosSincronizar);
            SincronizarCotacoes(dadosSincronizar);
            SincronizarComentarios(dadosSincronizar.Comentarios);
            SincronizarCalendariosPrevistos(dadosSincronizar.CalendariosPrevistos);
            SincronizarSugestoes(dadosSincronizar.Sugestoes);
            SincronizarListaCompra(dadosSincronizar.ListaCompra);
            await SincronizarAporteDinheiro(dadosSincronizar.AportesDinheiro);
            await SincronizarAtracao(dadosSincronizar.Atracoes, itemUsuario);
            await SincronizarRefeicao(dadosSincronizar.Refeicoes, itemUsuario);
            await SincronizarHotel(dadosSincronizar.Hoteis, itemUsuario);
            await SincronizarEventosHotel(dadosSincronizar.EventosHotel);
            await SincronizarLoja(dadosSincronizar.Lojas, itemUsuario);
            await SincronizarViagemAerea(dadosSincronizar.Deslocamentos, itemUsuario);
            await SincronizarCarro(dadosSincronizar.Carros);
            await SincronizarCarroDeslocamento(dadosSincronizar.CarroDeslocamentos);
            await SincronizarGastos(dadosSincronizar.Gastos, itemCS.UltimaDataRecepcao.GetValueOrDefault());
            await SincronizarReabastecimentos(dadosSincronizar.Reabastecimento);
            await SincronizarCompras(dadosSincronizar.Compras);
            await SincronizarItemCompra(dadosSincronizar.ItensComprados);
            await SincronizarGastoAtracao(dadosSincronizar.GastosAtracao);
            await SincronizarGastoRefeicao(dadosSincronizar.GastosRefeicao);
            await SincronizarGastoHotel(dadosSincronizar.GastosHotel);
            await SincronizarGastoViagemAerea(dadosSincronizar.GastosDeslocamento);
            await SincronizarGastoAluguel(dadosSincronizar.GastosCarro);
            itemCS.UltimaDataRecepcao = DataSincronizacao;
            itemCS.SincronizadoEnvio = true;
            await Database.SalvarControleSincronizacao(itemCS);

        }

        private static async Task SincronizarGastoAluguel(List<AluguelGasto> gastosAluguel)
        {
            foreach (var itemGasto in gastosAluguel)
            {

                var itemBanco = await Database.RetornarAluguelGasto(itemGasto.Identificador.GetValueOrDefault());
                if (!itemGasto.DataExclusao.HasValue)
                {

                    if (itemBanco == null || itemBanco.DataAtualizacao < itemGasto.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemGasto.Id = itemBanco.Id;
                        await Database.SalvarAluguelGasto(itemGasto);
                    }
                }
                else if (itemBanco != null)
                {

                    await Database.ExcluirAluguelGasto(itemBanco);
                }
            }
        }

        private static async Task SincronizarGastoViagemAerea(List<GastoViagemAerea> gastosViagemAerea)
        {
            foreach (var itemGasto in gastosViagemAerea)
            {

                var itemBanco = await Database.RetornarGastoViagemAerea(itemGasto.Identificador.GetValueOrDefault());
                if (!itemGasto.DataExclusao.HasValue)
                {

                    if (itemBanco == null || itemBanco.DataAtualizacao < itemGasto.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemGasto.Id = itemBanco.Id;
                        await Database.SalvarGastoViagemAerea(itemGasto);
                    }
                }
                else if (itemBanco != null)
                {

                    await Database.ExcluirGastoViagemAerea(itemBanco);
                }
            }
        }

        private static async Task SincronizarGastoHotel(List<GastoHotel> gastosHotel)
        {
            foreach (var itemGasto in gastosHotel)
            {

                var itemBanco = await Database.RetornarGastoHotel(itemGasto.Identificador.GetValueOrDefault());
                if (!itemGasto.DataExclusao.HasValue)
                {

                    if (itemBanco == null || itemBanco.DataAtualizacao < itemGasto.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemGasto.Id = itemBanco.Id;
                        await Database.SalvarGastoHotel(itemGasto);
                    }
                }
                else if (itemBanco != null)
                {

                    await Database.ExcluirGastoHotel(itemBanco);
                }
            }
        }

        private static async Task SincronizarGastoRefeicao(List<GastoRefeicao> gastosRefeicao)
        {
            foreach (var itemGasto in gastosRefeicao)
            {

                var itemBanco = await Database.RetornarGastoRefeicao(itemGasto.Identificador.GetValueOrDefault());
                if (!itemGasto.DataExclusao.HasValue)
                {

                    if (itemBanco == null || itemBanco.DataAtualizacao < itemGasto.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemGasto.Id = itemBanco.Id;
                        await Database.SalvarGastoRefeicao(itemGasto);
                    }
                }
                else if (itemBanco != null)
                {

                    await Database.ExcluirGastoRefeicao(itemBanco);
                }
            }
        }

        private static async Task SincronizarGastoAtracao(List<GastoAtracao> gastosAtracao)
        {
            foreach (var itemGasto in gastosAtracao)
            {

                var itemBanco = await Database.RetornarGastoAtracao(itemGasto.Identificador.GetValueOrDefault());
                if (!itemGasto.DataExclusao.HasValue)
                {

                    if (itemBanco == null || itemBanco.DataAtualizacao < itemGasto.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemGasto.Id = itemBanco.Id;
                        await Database.SalvarGastoAtracao(itemGasto);
                    }
                }
                else if (itemBanco != null)
                {

                    await Database.ExcluirGastoAtracao(itemBanco);
                }
            }
        }

        private static async Task SincronizarItemCompra(List<ItemCompra> itensComprados)
        {
            foreach (var itemComprado in itensComprados)
            {

                var itemBanco = await Database.RetornarItemCompra(itemComprado.Identificador.GetValueOrDefault());
                if (!itemComprado.DataExclusao.HasValue)
                {

                    if (itemBanco == null || itemBanco.DataAtualizacao < itemComprado.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemComprado.Id = itemBanco.Id;
                        await Database.SalvarItemCompra(itemComprado);
                    }
                }
                else if (itemBanco != null)
                {

                    await Database.ExcluirItemCompra(itemBanco);
                }
            }
        }

        private static async Task SincronizarCompras(List<GastoCompra> compras)
        {
            foreach (var itemCompra in compras)
            {

                var itemBanco = await Database.RetornarGastoCompra(itemCompra.Identificador.GetValueOrDefault());
                if (!itemCompra.DataExclusao.HasValue)
                {

                    if (itemBanco == null || itemBanco.DataAtualizacao < itemCompra.DataAtualizacao)
                    {
                        await Database.ExcluirGastoDividido_IdentificadorGasto(itemCompra.IdentificadorGasto.GetValueOrDefault());

                        var itemGastoBase = await Database.RetornarGasto(itemCompra.IdentificadorGasto);
                        if (itemGastoBase != null)
                            itemCompra.ItemGasto.Id = itemGastoBase.Id;
                        await Database.SalvarGasto(itemCompra.ItemGasto);
                        if (itemBanco != null)
                            itemCompra.Id = itemBanco.Id;
                        await Database.SalvarGastoCompra(itemCompra);
                    }
                }
                else if (itemBanco != null)
                {
                    await Database.ExcluirGastoDividido_IdentificadorGasto(itemCompra.IdentificadorGasto.GetValueOrDefault());

                    var itemGasto = await Database.RetornarGasto(itemCompra.IdentificadorGasto);
                    if (itemGasto != null)
                        await Database.ExcluirGasto(itemGasto);
                    await Database.ExcluirGastoCompra(itemBanco);
                }
            }
        }

        private async static Task SincronizarReabastecimentos(List<Reabastecimento> reabastecimento)
        {
            foreach (var itemReabastecimento in reabastecimento)
            {

                var itemBanco = await Database.RetornarReabastecimento(itemReabastecimento.Identificador.GetValueOrDefault());
                if (!itemReabastecimento.DataExclusao.HasValue)
                {

                    if (itemBanco == null || itemBanco.DataAtualizacao < itemReabastecimento.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemReabastecimento.Id = itemBanco.Id;
                        var itemGastoReabastecimento = itemReabastecimento.Gastos.FirstOrDefault();
                        if (itemGastoReabastecimento != null)
                        {
                            if (itemBanco != null)
                            {
                                var ListaGastos = await Database.ListarReabastecimentoGasto_IdentificadorReabastecimento(itemBanco.Identificador);
                                if (ListaGastos.Any())
                                {
                                    var itemGRBase = ListaGastos.FirstOrDefault();
                                    var itemGastoBase = await Database.RetornarGasto(itemGRBase.IdentificadorGasto);
                                    if (itemGastoBase != null)
                                        itemGastoReabastecimento.ItemGasto.Id = itemGastoBase.Id;
                                    itemGastoReabastecimento.Id = itemGRBase.Id;
                                }
                            }
                            await Database.SalvarGasto(itemGastoReabastecimento.ItemGasto);
                            await Database.SalvarReabastecimentoGasto(itemGastoReabastecimento);


                        }
                        await Database.SalvarReabastecimento(itemReabastecimento);

                    }
                }
                else if (itemBanco != null)
                {
                    foreach (var itemGastoReabastecimento in await Database.ListarReabastecimentoGasto_IdentificadorReabastecimento(itemBanco.Identificador))
                    {
                        var itemGasto = await Database.RetornarGasto(itemGastoReabastecimento.IdentificadorGasto);
                        if (itemGasto != null)
                            await Database.ExcluirGasto(itemGasto);
                        await Database.ExcluirReabastecimentoGasto(itemGastoReabastecimento);
                    }

                    await Database.ExcluirReabastecimento(itemBanco);
                }
            }
        }

        private async static Task SincronizarCarroDeslocamento(List<CarroDeslocamento> carroDeslocamentos)
        {
            foreach (var itemCarro in carroDeslocamentos)
            {

                var itemBanco = await Database.RetornarCarroDeslocamento(itemCarro.Identificador.GetValueOrDefault());
                if (!itemCarro.DataExclusao.HasValue)
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemCarro.DataAtualizacao)
                    {
                        await Database.ExcluirCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(itemCarro.Identificador);

                        if (itemBanco != null)
                        {
                            if (itemBanco.IdentificadorCarroEventoPartida.HasValue)
                                await Database.ExcluirCarroEvento(await Database.RetornarCarroEvento(itemBanco.IdentificadorCarroEventoPartida));
                            if (itemBanco.IdentificadorCarroEventoChegada.HasValue)
                                await Database.ExcluirCarroEvento(await Database.RetornarCarroEvento(itemBanco.IdentificadorCarroEventoChegada));

                            itemCarro.Id = itemBanco.Id;
                        }
                        if (itemCarro.ItemCarroEventoChegada != null)
                            await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoChegada);
                        if (itemCarro.ItemCarroEventoPartida != null)
                            await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoPartida);
                        await Database.SalvarCarroDeslocamento(itemCarro);
                        await Database.IncluirCarroDeslocamentoUsuario(itemCarro.Usuarios.ToList());
                    }
                }
                else if (itemBanco != null)
                {
                    await Database.ExcluirCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(itemCarro.Identificador);

                    if (itemBanco.IdentificadorCarroEventoChegada.HasValue)
                        await Database.ExcluirCarroEvento(await Database.RetornarCarroEvento(itemBanco.IdentificadorCarroEventoChegada));
                    if (itemBanco.IdentificadorCarroEventoPartida.HasValue)
                        await Database.ExcluirCarroEvento(await Database.RetornarCarroEvento(itemBanco.IdentificadorCarroEventoPartida));

                    await Database.ExcluirCarroDeslocamento(itemBanco);
                }
            }
        }

        private async static Task SincronizarEventosHotel(List<HotelEvento> eventos)
        {
            foreach (var itemHotel in eventos)
            {

                var itemBanco = await Database.RetornarHotelEvento(itemHotel.Identificador.GetValueOrDefault());
                if (!itemHotel.DataExclusao.HasValue)
                {
                   
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemHotel.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemHotel.Id = itemBanco.Id;
                        await Database.SalvarHotelEvento(itemHotel);
                    }
                }
                else if (itemBanco != null)
                {

                    await Database.ExcluirHotelEvento(itemBanco);
                }
            }
        }

        private async static Task SincronizarGastos(List<Gasto> gastos, DateTime DataBaseAtualizacao)
        {
            foreach (var itemGasto in gastos)
            {

                var itemBanco = await Database.RetornarGasto(itemGasto.Identificador.GetValueOrDefault());
                if (!itemGasto.DataExclusao.HasValue)
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemGasto.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemGasto.Id = itemBanco.Id;
                        await Database.SalvarGasto(itemGasto);
                        await Database.ExcluirGastoDividido_IdentificadorGasto(itemGasto.Identificador.GetValueOrDefault());
                        await Database.IncluirGastoDividido(itemGasto.Usuarios.ToList());
                        foreach (var itemAtracao in itemGasto.Atracoes.Where(d=>d.DataAtualizacao > DataBaseAtualizacao || d.DataExclusao > DataBaseAtualizacao))
                        {
                            var itemGastoBanco = await Database.RetornarGastoAtracao(itemAtracao.Identificador);
                            if (!itemAtracao.DataExclusao.HasValue)
                            {
                                if (itemGastoBanco == null || itemGastoBanco.DataAtualizacao < itemGasto.DataAtualizacao)
                                {
                                    if (itemGastoBanco != null)
                                        itemAtracao.Id = itemGastoBanco.Id;
                                    await Database.SalvarGastoAtracao(itemAtracao);
                                }
                            }
                            else if (itemGastoBanco != null)
                            {
                                await Database.ExcluirGastoAtracao(itemGastoBanco);
                            }
                        }
                        foreach (var itemRefeicao in itemGasto.Refeicoes.Where(d => d.DataAtualizacao > DataBaseAtualizacao || d.DataExclusao > DataBaseAtualizacao))
                        {
                            var itemGastoBanco = await Database.RetornarGastoRefeicao(itemRefeicao.Identificador);
                            if (!itemRefeicao.DataExclusao.HasValue)
                            {
                                if (itemGastoBanco == null || itemGastoBanco.DataAtualizacao < itemGasto.DataAtualizacao)
                                {
                                    if (itemGastoBanco != null)
                                        itemRefeicao.Id = itemGastoBanco.Id;
                                    await Database.SalvarGastoRefeicao(itemRefeicao);
                                }
                            }
                            else if (itemGastoBanco != null)
                            {
                                await Database.ExcluirGastoRefeicao(itemGastoBanco);
                            }
                        }
                        foreach (var itemHotel in itemGasto.Hoteis.Where(d => d.DataAtualizacao > DataBaseAtualizacao || d.DataExclusao > DataBaseAtualizacao))
                        {
                            var itemGastoBanco = await Database.RetornarGastoHotel(itemHotel.Identificador);
                            if (!itemHotel.DataExclusao.HasValue)
                            {
                                if (itemGastoBanco == null || itemGastoBanco.DataAtualizacao < itemGasto.DataAtualizacao)
                                {
                                    if (itemGastoBanco != null)
                                        itemHotel.Id = itemGastoBanco.Id;
                                    await Database.SalvarGastoHotel(itemHotel);
                                }
                            }
                            else if (itemGastoBanco != null)
                            {
                                await Database.ExcluirGastoHotel(itemGastoBanco);
                            }
                        }

                        foreach (var itemCarro in itemGasto.Alugueis.Where(d => d.DataAtualizacao > DataBaseAtualizacao || d.DataExclusao > DataBaseAtualizacao))
                        {
                            var itemGastoBanco = await Database.RetornarAluguelGasto(itemCarro.Identificador);
                            if (!itemCarro.DataExclusao.HasValue)
                            {
                                if (itemGastoBanco == null || itemGastoBanco.DataAtualizacao < itemGasto.DataAtualizacao)
                                {
                                    if (itemGastoBanco != null)
                                        itemCarro.Id = itemGastoBanco.Id;
                                    await Database.SalvarAluguelGasto(itemCarro);
                                }
                            }
                            else if (itemGastoBanco != null)
                            {
                                await Database.ExcluirAluguelGasto(itemGastoBanco);
                            }
                        }

                        foreach (var itemViagemAerea in itemGasto.ViagenAereas.Where(d => d.DataAtualizacao > DataBaseAtualizacao || d.DataExclusao > DataBaseAtualizacao))
                        {
                            var itemGastoBanco = await Database.RetornarGastoViagemAerea(itemViagemAerea.Identificador);
                            if (!itemViagemAerea.DataExclusao.HasValue)
                            {
                                if (itemGastoBanco == null || itemGastoBanco.DataAtualizacao < itemGasto.DataAtualizacao)
                                {
                                    if (itemGastoBanco != null)
                                        itemViagemAerea.Id = itemGastoBanco.Id;
                                    await Database.SalvarGastoViagemAerea(itemViagemAerea);
                                }
                            }
                            else if (itemGastoBanco != null)
                            {
                                await Database.ExcluirGastoViagemAerea(itemGastoBanco);
                            }
                        }
                    }
                }
                else if (itemBanco != null)
                {
                    await Database.Excluir_Gasto_Filhos(itemBanco.Identificador.GetValueOrDefault());
                    await Database.ExcluirGasto(itemBanco);
                }
            }
        }

        private async static Task SincronizarCarro(List<Carro> carros)
        {
            foreach (var itemCarro in carros)
            {

                var itemBanco = await Database.RetornarCarro(itemCarro.Identificador.GetValueOrDefault());
                if (!itemCarro.DataExclusao.HasValue)
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemCarro.DataAtualizacao)
                    {
                        await Database.ExcluirAvaliacaoAluguel_IdentificadorCarro(itemCarro.Identificador);

                        if (itemBanco != null)
                        {
                            if (itemBanco.IdentificadorCarroEventoRetirada.HasValue)
                                await Database.ExcluirCarroEvento(await Database.RetornarCarroEvento(itemBanco.IdentificadorCarroEventoRetirada));
                            if (itemBanco.IdentificadorCarroEventoDevolucao.HasValue)
                                await Database.ExcluirCarroEvento(await Database.RetornarCarroEvento(itemBanco.IdentificadorCarroEventoDevolucao));

                            itemCarro.Id = itemBanco.Id;
                        }
                        if (itemCarro.ItemCarroEventoDevolucao != null)
                            await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoDevolucao);
                        if (itemCarro.ItemCarroEventoRetirada != null)
                            await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoRetirada);
                        await Database.SalvarCarro(itemCarro);
                        await Database.IncluirAvaliacaoAluguel(itemCarro.Avaliacoes.ToList());
                    }
                }
                else if (itemBanco != null)
                {
                    await Database.ExcluirAvaliacaoAluguel_IdentificadorCarro(itemCarro.Identificador);

                    if (itemBanco.IdentificadorCarroEventoRetirada.HasValue)
                        await Database.ExcluirCarroEvento(await Database.RetornarCarroEvento(itemBanco.IdentificadorCarroEventoRetirada));
                    if (itemBanco.IdentificadorCarroEventoDevolucao.HasValue)
                        await Database.ExcluirCarroEvento(await Database.RetornarCarroEvento(itemBanco.IdentificadorCarroEventoDevolucao));

                    await Database.ExcluirCarro(itemBanco);
                }
            }
        }

        private async static Task SincronizarViagemAerea(List<ViagemAerea> deslocamentos, UsuarioLogado itemUsuario)
        {
            foreach (var itemViagemAerea in deslocamentos)
            {
                var itemBanco = await Database.RetornarViagemAerea(itemViagemAerea.Identificador.GetValueOrDefault());
                if (!itemViagemAerea.DataExclusao.HasValue)
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemViagemAerea.DataAtualizacao)
                    {
                        await Database.ExcluirAvaliacaoAerea_IdentificadorViagemAerea(itemViagemAerea.Identificador);
                        await Database.ExcluirViagemAereaAeroporto_IdentificadorViagemAerea(itemViagemAerea.Identificador);

                        if (itemBanco != null)
                            itemViagemAerea.Id = itemBanco.Id;
                        await Database.SalvarViagemAerea(itemViagemAerea);
                        await Database.IncluirAvaliacaoAerea(itemViagemAerea.Avaliacoes.ToList());
                        await Database.IncluirViagemAereaAeroporto(itemViagemAerea.Aeroportos.ToList());
                    }
                }
                else if (itemBanco != null)
                {
                    await Database.ExcluirAvaliacaoAerea_IdentificadorViagemAerea(itemViagemAerea.Identificador);
                    await Database.ExcluirViagemAereaAeroporto_IdentificadorViagemAerea(itemViagemAerea.Identificador);

                    await Database.ExcluirViagemAerea(itemBanco);
                }
            }
        }

        private static async Task SincronizarLoja(List<Loja> lojas, UsuarioLogado itemUsuario)
        {

            foreach (var itemLoja in lojas)
            {

                var itemBanco = await Database.RetornarLoja(itemLoja.Identificador.GetValueOrDefault());
                if (!itemLoja.DataExclusao.HasValue)
                {
                    await Database.ExcluirAvaliacaoLoja_IdentificadorLoja(itemLoja.Identificador);

                    if (itemBanco == null || itemBanco.DataAtualizacao < itemLoja.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemLoja.Id = itemBanco.Id;
                        await Database.SalvarLoja(itemLoja);
                        await Database.IncluirAvaliacaoLoja(itemLoja.Avaliacoes.ToList());
                    }
                }
                else if (itemBanco != null)
                {
                    await Database.ExcluirAvaliacaoLoja_IdentificadorLoja(itemLoja.Identificador);

                    await Database.ExcluirLoja(itemBanco);
                }
            }
        }

        private static async Task SincronizarHotel(List<Hotel> hoteis, UsuarioLogado itemUsuario)
        {

            foreach (var itemHotel in hoteis)
            {

                var itemBanco = await Database.RetornarHotel(itemHotel.Identificador.GetValueOrDefault());
                if (!itemHotel.DataExclusao.HasValue)
                {
                    await Database.ExcluirHotelAvaliacao_IdentificadorHotel(itemHotel.Identificador);

                    if (itemBanco == null || itemBanco.DataAtualizacao < itemHotel.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemHotel.Id = itemBanco.Id;
                        await Database.SalvarHotel(itemHotel);
                        await Database.IncluirHotelAvaliacao(itemHotel.Avaliacoes.ToList());
                    }
                }
                else if (itemBanco != null)
                {
                    await Database.ExcluirHotelAvaliacao_IdentificadorHotel(itemHotel.Identificador);

                    await Database.ExcluirHotel(itemBanco);
                }
            }
        }


        private static async Task SincronizarRefeicao(List<Refeicao> refeicoes, UsuarioLogado itemUsuario)
        {

            foreach (var itemRefeicao in refeicoes)
            {

                var itemBanco = await Database.RetornarRefeicao(itemRefeicao.Identificador.GetValueOrDefault());
                if (!itemRefeicao.DataExclusao.HasValue)
                {
                    await Database.ExcluirRefeicaoPedido_IdentificadorRefeicao(itemRefeicao.Identificador);

                    if (itemBanco == null || itemBanco.DataAtualizacao < itemRefeicao.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemRefeicao.Id = itemBanco.Id;
                        await Database.SalvarRefeicao(itemRefeicao);
                        await Database.IncluirRefeicaoPedido(itemRefeicao.Pedidos.ToList());
                    }
                }
                else if (itemBanco != null)
                {
                    await Database.ExcluirRefeicaoPedido_IdentificadorRefeicao(itemRefeicao.Identificador);

                    await Database.ExcluirRefeicao(itemBanco);
                }
            }
        }


        private static async Task SincronizarAtracao(List<Atracao> atracoes, UsuarioLogado itemUsuario)
        {

            foreach (var itemAtracao in atracoes)
            {

                var itemBanco = await Database.RetornarAtracao(itemAtracao.Identificador.GetValueOrDefault());
                if (!itemAtracao.DataExclusao.HasValue)
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemAtracao.DataAtualizacao)
                    {
                        await Database.ExcluirAvaliacaoAtracao_IdentificadorAtracao(itemAtracao.Identificador);

                        if (itemBanco != null)
                            itemAtracao.Id = itemBanco.Id;
                        await Database.SalvarAtracao(itemAtracao);
                        await Database.IncluirAvaliacaoAtracao(itemAtracao.Avaliacoes.ToList());
                    }
                }
                else if (itemBanco != null)
                {
                    await Database.ExcluirAvaliacaoAtracao_IdentificadorAtracao(itemAtracao.Identificador);

                    await Database.ExcluirAtracao(itemBanco);
                }
            }
        }

        private static async Task SincronizarAporteDinheiro(List<AporteDinheiro> aportesDinheiro)
        {
            foreach (var itemAporte in aportesDinheiro)
            {
                var itemBanco = await Database.RetornarAporteDinheiro(itemAporte.Identificador.GetValueOrDefault());
                if (!itemAporte.DataExclusao.HasValue)
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemAporte.DataAtualizacao)
                    {
                        Gasto itemGasto = null;
                        if (itemBanco != null)
                        {
                            itemAporte.Id = itemBanco.Id;

                            if (itemBanco.IdentificadorGasto.HasValue)
                                itemGasto = await Database.RetornarGasto(itemBanco.IdentificadorGasto);
                        }
                        if (itemAporte.IdentificadorGasto.HasValue && !itemAporte.ItemGasto.DataExclusao.HasValue)
                        {
                            if (itemGasto != null)
                                itemAporte.ItemGasto.Id = itemGasto.Id;
                            await Database.SalvarGasto(itemAporte.ItemGasto);
                        }
                        else
                        {
                            itemAporte.IdentificadorGasto = null;
                            if (itemGasto != null)
                                await Database.ExcluirGasto(itemGasto);
                        }
                        await Database.SalvarAporteDinheiro(itemAporte);
                    }
                }
                else if (itemBanco != null)
                {
                    if (itemBanco.IdentificadorGasto.HasValue)
                        await Database.ExcluirGasto(await Database.RetornarGasto(itemBanco.IdentificadorGasto));
                    await Database.ExcluirAporteDinheiro(itemBanco);
                }
            }
        }

        private static async void SincronizarListaCompra(List<ListaCompra> listaCompra)
        {
            foreach (var itemListaCompra in listaCompra)
            {
                var itemBanco = await Database.RetornarListaCompra(itemListaCompra.Identificador.GetValueOrDefault());
                if (!itemListaCompra.DataExclusao.HasValue)
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemListaCompra.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemListaCompra.Id = itemBanco.Id;
                        await Database.SalvarListaCompra(itemListaCompra);
                    }
                }
                else if (itemBanco != null)
                    await Database.ExcluirListaCompra(itemBanco);
            }
        }

        private static async void SincronizarSugestoes(List<Sugestao> sugestoes)
        {
            foreach (var itemSugestao in sugestoes)
            {
                var itemBanco = await Database.RetornarSugestao(itemSugestao.Identificador.GetValueOrDefault());
                if (!itemSugestao.DataExclusao.HasValue)
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemSugestao.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemSugestao.Id = itemBanco.Id;
                        await Database.SalvarSugestao(itemSugestao);
                    }
                }
                else if (itemBanco != null)
                    await Database.ExcluirSugestao(itemBanco);
            }
        }

        private static async void SincronizarCalendariosPrevistos(List<CalendarioPrevisto> calendariosPrevistos)
        {
            foreach (var itemCalendario in calendariosPrevistos)
            {
                var itemBanco = await Database.CarregarCalendarioPrevisto(itemCalendario.Identificador.GetValueOrDefault());
                if (!itemCalendario.DataExclusao.HasValue)
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemCalendario.DataAtualizacao)
                    {
                        if (itemBanco != null)
                        {
                            itemCalendario.Id = itemBanco.Id;
                            itemCalendario.DataProximoAviso = itemBanco.DataProximoAviso;
                        }
                        else
                        {
                            itemCalendario.DataProximoAviso = itemCalendario.DataInicio.GetValueOrDefault(new DateTime(1900, 01, 01));
                        }
                        await Database.SalvarCalendarioPrevisto(itemCalendario);
                    }
                }
                else if (itemBanco != null)
                    await Database.ExcluirCalendarioPrevisto(itemBanco);
            }
        }

        private static async void SincronizarComentarios(List<Comentario> comentarios)
        {
            foreach (var itemComentario in comentarios)
            {
                var itemBanco = await Database.RetornarComentario(itemComentario.Identificador);
                if (!itemComentario.DataExclusao.HasValue)
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemComentario.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemComentario.Id = itemBanco.Id;
                        await Database.SalvarComentario(itemComentario);
                    }
                }
                else if (itemBanco != null)
                    await Database.ExcluirComentario(itemBanco);
            }
        }

        private static async void SincronizarCotacoes(ClasseSincronizacao dadosSincronizar)
        {
            foreach (var itemCotacao in dadosSincronizar.CotacoesMoeda)
            {
                var itemBanco = await Database.RetornarCotacaoMoeda(itemCotacao.Identificador);
                if (!itemCotacao.DataExclusao.HasValue)
                {
                    if (itemBanco == null || itemBanco.DataAtualizacao < itemCotacao.DataAtualizacao)
                    {
                        if (itemBanco != null)
                            itemCotacao.Id = itemBanco.Id;
                        await Database.SalvarCotacaoMoeda(itemCotacao);
                    }
                }
                else if (itemBanco != null)
                    await  Database.ExcluirCotacaoMoeda(itemBanco);
            }
        }

        private static async Task IncluirCidades(ClasseSincronizacao dadosSincronizar)
        {
            await Database.ExcluirCidades();
            foreach (var itemCidade in dadosSincronizar.CidadesAtracao)
                itemCidade.Tipo = "A";
            foreach (var itemCidade in dadosSincronizar.CidadesComentario)
                itemCidade.Tipo = "CO";
            foreach (var itemCidade in dadosSincronizar.CidadesHotel)
                itemCidade.Tipo = "H";
            foreach (var itemCidade in dadosSincronizar.CidadesLoja)
                itemCidade.Tipo = "L";
            foreach (var itemCidade in dadosSincronizar.CidadesRefeicao)
                itemCidade.Tipo = "R";
            foreach (var itemCidade in dadosSincronizar.CidadesSugestao)
                itemCidade.Tipo = "S";
            foreach (var itemCidade in dadosSincronizar.CidadesViagemAerea)
                itemCidade.Tipo = "V";
            var ListaCompleta = dadosSincronizar.CidadesAtracao.Concat(dadosSincronizar.CidadesComentario).Concat(dadosSincronizar.CidadesHotel).Concat(dadosSincronizar.CidadesLoja).Concat(dadosSincronizar.CidadesRefeicao).Concat(dadosSincronizar.CidadesSugestao).Concat(dadosSincronizar.CidadesViagemAerea).ToList();
            await Database.IncluirCidades(ListaCompleta);
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
                itemResultado.IdentificadorRegistro = itemCotacaoMoeda.Identificador;

                Erros.Add(new MensagemErro() { Mensagem = "Cotação de Moeda salva com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();


            return itemResultado;
        }

        internal static async Task<Atracao> CarregarAtracao(int? Identificador)
        {
            Atracao item = await Database.RetornarAtracao(Identificador);
            item.Avaliacoes = new System.Collections.ObjectModel.ObservableCollection<AvaliacaoAtracao>(await Database.ListarAvaliacaoAtracao_IdentificadorAtracao(Identificador));
            item.Gastos = new System.Collections.ObjectModel.ObservableCollection<GastoAtracao>(await Database.ListarGastoAtracao_IdentificadorAtracao(Identificador));
            foreach (var itemGastoAtracao in item.Gastos)
            {
                itemGastoAtracao.ItemGasto = await Database.RetornarGasto(itemGastoAtracao.IdentificadorGasto);
                itemGastoAtracao.ItemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(itemGastoAtracao.IdentificadorGasto));

            }
            return item;
        }

        internal static async Task ExcluirAtracao(int? Identificador, bool Sincronizado)
        {
            var itemBanco = await CarregarAtracao(Identificador);
            if (itemBanco != null)
            {
                if (itemBanco.Identificador > 0 && !Sincronizado)
                {
                    itemBanco.DataExclusao = DateTime.Now.ToUniversalTime();
                    itemBanco.AtualizadoBanco = false;
                    await Database.SalvarAtracao(itemBanco);
                    foreach (var itemAvaliacao in itemBanco.Avaliacoes)
                    {
                        if (itemAvaliacao.Identificador > 0)
                        {
                            itemAvaliacao.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemAvaliacao.AtualizadoBanco = false;
                            await Database.SalvarAvaliacaoAtracao(itemAvaliacao);
                        }
                        else
                            await Database.ExcluirAvaliacaoAtracao(itemAvaliacao);
                    }
                    foreach (var itemGasto in itemBanco.Gastos)
                    {
                        if (itemGasto.Identificador > 0)
                        {
                            itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemGasto.AtualizadoBanco = false;
                            await Database.SalvarGastoAtracao(itemGasto);
                        }
                        else
                            await Database.ExcluirGastoAtracao(itemGasto);
                    }
                }

                else
                {
                    foreach (var itemAvaliacao in itemBanco.Avaliacoes)
                        await Database.ExcluirAvaliacaoAtracao(itemAvaliacao);
                    foreach (var itemGasto in itemBanco.Gastos)
                        await Database.ExcluirGastoAtracao(itemGasto);
                    await Database.ExcluirAtracao(itemBanco);
                }
            }
        }

        internal static async Task<ResultadoOperacao> SalvarAtracao(Atracao itemAtracao)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            if (string.IsNullOrEmpty(itemAtracao.Nome))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Nome é obrigatório" });
            }


            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemAtracao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemAtracao.AtualizadoBanco = false;
                await Database.SalvarAtracao(itemAtracao);
                foreach (var itemAvaliacao in itemAtracao.Avaliacoes)
                {
                    itemAvaliacao.IdentificadorAtracao = itemAtracao.Identificador;
                    itemAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    itemAvaliacao.AtualizadoBanco = false;
                    await Database.SalvarAvaliacaoAtracao(itemAvaliacao);
                }
                itemResultado.IdentificadorRegistro = itemAtracao.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Atração salva com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();

            return itemResultado;
        }

        internal static async Task SalvarAtracaoReplicada(Atracao itemAtracao)
        {
            var itemAtracaoBase = await Database.RetornarAtracao(itemAtracao.Identificador);
            if (itemAtracaoBase != null)
                itemAtracao.Id = itemAtracaoBase.Id;
            await DatabaseService.SalvarAtracao(itemAtracao);
            foreach (var itemAvaliacao in itemAtracao.Avaliacoes)
            {
                var itemAvaliacaoBase = await Database.RetornarAvaliacaoAtracao(itemAvaliacao.Identificador);
                if (itemAvaliacao.DataExclusao.HasValue)
                {
                    if (itemAvaliacaoBase != null)
                        await Database.ExcluirAvaliacaoAtracao(itemAvaliacaoBase);
                }
                else
                {
                    if (itemAvaliacaoBase != null)
                        itemAvaliacao.Id = itemAvaliacaoBase.Id;
                    await Database.SalvarAvaliacaoAtracao(itemAvaliacao);
                }
            }

        }


        internal static async Task<Refeicao> CarregarRefeicao(int? Identificador)
        {
            Refeicao item = await Database.RetornarRefeicao(Identificador);
            item.Pedidos = new ObservableRangeCollection<RefeicaoPedido>(await Database.ListarRefeicaoPedido_IdentificadorRefeicao(Identificador));
            item.Gastos = new ObservableRangeCollection<GastoRefeicao>(await Database.ListarGastoRefeicao_IdentificadorRefeicao(Identificador));
            foreach (var itemGastoRefeicao in item.Gastos)
            {
                itemGastoRefeicao.ItemGasto = await Database.RetornarGasto(itemGastoRefeicao.IdentificadorGasto);
                itemGastoRefeicao.ItemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(itemGastoRefeicao.IdentificadorGasto));

            }
            return item;
        }

        internal static async Task ExcluirRefeicao(int? Identificador, bool Sincronizado)
        {
            var itemBanco = await CarregarRefeicao(Identificador);
            if (itemBanco != null)
            {
                if (itemBanco.Identificador > 0 && !Sincronizado)
                {
                    itemBanco.DataExclusao = DateTime.Now.ToUniversalTime();
                    itemBanco.AtualizadoBanco = false;
                    await Database.SalvarRefeicao(itemBanco);
                    foreach (var itemAvaliacao in itemBanco.Pedidos)
                    {
                        if (itemAvaliacao.Identificador > 0)
                        {
                            itemAvaliacao.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemAvaliacao.AtualizadoBanco = false;
                            await Database.SalvarRefeicaoPedido(itemAvaliacao);
                        }
                        else
                            await Database.ExcluirRefeicaoPedido(itemAvaliacao);
                    }
                    foreach (var itemGasto in itemBanco.Gastos)
                    {
                        if (itemGasto.Identificador > 0)
                        {
                            itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemGasto.AtualizadoBanco = false;
                            await Database.SalvarGastoRefeicao(itemGasto);
                        }
                        else
                            await Database.ExcluirGastoRefeicao(itemGasto);
                    }

                }
                else
                {
                    foreach (var itemAvaliacao in itemBanco.Pedidos)
                        await Database.ExcluirRefeicaoPedido(itemAvaliacao);
                    foreach (var itemGasto in itemBanco.Gastos)
                        await Database.ExcluirGastoRefeicao(itemGasto);
                    await Database.ExcluirRefeicao(itemBanco);
                }
            }
        }

        internal static async Task<ResultadoOperacao> SalvarRefeicao(Refeicao itemRefeicao)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            if (string.IsNullOrEmpty(itemRefeicao.Nome))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Nome é obrigatório" });
            }


            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemRefeicao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemRefeicao.AtualizadoBanco = false;
                await Database.SalvarRefeicao(itemRefeicao);
                foreach (var itemAvaliacao in itemRefeicao.Pedidos)
                {
                    itemAvaliacao.IdentificadorRefeicao = itemRefeicao.Identificador;
                    itemAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    itemAvaliacao.AtualizadoBanco = false;
                    await Database.SalvarRefeicaoPedido(itemAvaliacao);
                }
                itemResultado.IdentificadorRegistro = itemRefeicao.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Refeição salva com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();

            return itemResultado;
        }

        internal static async Task SalvarRefeicaoReplicada(Refeicao itemRefeicao)
        {
            var itemRefeicaoBase = await Database.RetornarRefeicao(itemRefeicao.Identificador);
            if (itemRefeicaoBase != null)
                itemRefeicao.Id = itemRefeicaoBase.Id;
            await DatabaseService.SalvarRefeicao(itemRefeicao);
            foreach (var itemAvaliacao in itemRefeicao.Pedidos)
            {
                var itemAvaliacaoBase = await Database.RetornarRefeicaoPedido(itemAvaliacao.Identificador);
                if (itemAvaliacao.DataExclusao.HasValue)
                {
                    if (itemAvaliacaoBase != null)
                        await Database.ExcluirRefeicaoPedido(itemAvaliacaoBase);
                }
                else
                {
                    if (itemAvaliacaoBase != null)
                        itemAvaliacao.Id = itemAvaliacaoBase.Id;
                    await Database.SalvarRefeicaoPedido(itemAvaliacao);
                }
            }

        }

        internal static async Task<Hotel> CarregarHotel(int? Identificador)
        {
            Hotel item = await Database.RetornarHotel(Identificador);
            if (item != null)
            {
                item.Avaliacoes = new ObservableRangeCollection<HotelAvaliacao>(await Database.ListarHotelAvaliacao_IdentificadorHotel(Identificador));
                item.Gastos = new ObservableRangeCollection<GastoHotel>(await Database.ListarGastoHotel_IdentificadorHotel(Identificador));
                item.Eventos = new ObservableRangeCollection<HotelEvento>(await Database.ListarHotelEvento_IdentificadorHotel(Identificador));
                foreach (var itemGastoHotel in item.Gastos)
                {
                    itemGastoHotel.ItemGasto = await Database.RetornarGasto(itemGastoHotel.IdentificadorGasto);
                    itemGastoHotel.ItemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(itemGastoHotel.IdentificadorGasto));

                }
            }
            return item;
        }

        internal static async Task ExcluirHotel(int? Identificador, bool Sincronizado)
        {
            var itemBanco = await CarregarHotel(Identificador);
            if (itemBanco != null)
            {
                if (itemBanco.Identificador > 0 && !Sincronizado)
                {
                    itemBanco.DataExclusao = DateTime.Now.ToUniversalTime();
                    itemBanco.AtualizadoBanco = false;
                    await Database.SalvarHotel(itemBanco);
                    foreach (var itemAvaliacao in itemBanco.Avaliacoes)
                    {
                        if (itemAvaliacao.Identificador > 0)
                        {
                            itemAvaliacao.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemAvaliacao.AtualizadoBanco = false;
                            await Database.SalvarHotelAvaliacao(itemAvaliacao);
                        }
                        else
                            await Database.ExcluirHotelAvaliacao(itemAvaliacao);
                    }

                    foreach (var itemEvento in itemBanco.Eventos)
                    {
                        if (itemEvento.Identificador > 0)
                        {
                            itemEvento.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemEvento.AtualizadoBanco = false;
                            await Database.SalvarHotelEvento(itemEvento);
                        }
                        else
                            await Database.ExcluirHotelEvento(itemEvento);
                    }
                    foreach (var itemGasto in itemBanco.Gastos)
                    {
                        if (itemGasto.Identificador > 0)
                        {
                            itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemGasto.AtualizadoBanco = false;
                            await Database.SalvarGastoHotel(itemGasto);
                        }
                        else
                            await Database.ExcluirGastoHotel(itemGasto);
                    }
                }

                else
                {
                    foreach (var itemAvaliacao in itemBanco.Avaliacoes)
                        await Database.ExcluirHotelAvaliacao(itemAvaliacao);
                    foreach (var itemGasto in itemBanco.Gastos)
                        await Database.ExcluirGastoHotel(itemGasto);
                    foreach (var itemGasto in itemBanco.Eventos)
                        await Database.ExcluirHotelEvento(itemGasto);
                    await Database.ExcluirHotel(itemBanco);
                }

            }
        }

        internal static async Task<ResultadoOperacao> SalvarHotel(Hotel itemHotel)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            if (string.IsNullOrEmpty(itemHotel.Nome))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Nome é obrigatório" });
            }


            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemHotel.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemHotel.AtualizadoBanco = false;
                await Database.SalvarHotel(itemHotel);
                foreach (var itemAvaliacao in itemHotel.Avaliacoes)
                {
                    itemAvaliacao.IdentificadorHotel = itemHotel.Identificador;
                    itemAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    itemAvaliacao.AtualizadoBanco = false;
                    await Database.SalvarHotelAvaliacao(itemAvaliacao);
                }
                foreach (var itemAvaliacao in itemHotel.Eventos)
                {
                    itemAvaliacao.IdentificadorHotel = itemHotel.Identificador;
                    itemAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    itemAvaliacao.AtualizadoBanco = false;
                    await Database.SalvarHotelEvento(itemAvaliacao);
                }
                itemResultado.IdentificadorRegistro = itemHotel.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Hotel salvo com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();

            return itemResultado;
        }

        internal static async Task SalvarHotelReplicada(Hotel itemHotel)
        {
            var itemHotelBase = await Database.RetornarHotel(itemHotel.Identificador);
            if (itemHotelBase != null)
                itemHotel.Id = itemHotelBase.Id;
            await DatabaseService.SalvarHotel(itemHotel);
            foreach (var itemAvaliacao in itemHotel.Avaliacoes)
            {
                var itemAvaliacaoBase = await Database.RetornarHotelAvaliacao(itemAvaliacao.Identificador);
                if (itemAvaliacao.DataExclusao.HasValue)
                {
                    if (itemAvaliacaoBase != null)
                        await Database.ExcluirHotelAvaliacao(itemAvaliacaoBase);
                }
                else
                {
                    if (itemAvaliacaoBase != null)
                        itemAvaliacao.Id = itemAvaliacaoBase.Id;
                    await Database.SalvarHotelAvaliacao(itemAvaliacao);
                }
            }

            foreach (var itemAvaliacao in itemHotel.Eventos)
            {
                var itemAvaliacaoBase = await Database.RetornarHotelEvento(itemAvaliacao.Identificador);
                if (itemAvaliacao.DataExclusao.HasValue)
                {
                    if (itemAvaliacaoBase != null)
                        await Database.ExcluirHotelEvento(itemAvaliacaoBase);
                }
                else
                {
                    if (itemAvaliacaoBase != null)
                        itemAvaliacao.Id = itemAvaliacaoBase.Id;
                    await Database.SalvarHotelEvento(itemAvaliacao);
                }
            }

        }


        internal static async Task<Loja> CarregarLoja(int? Identificador)
        {
            Loja item = await Database.RetornarLoja(Identificador);
            item.Avaliacoes = new ObservableRangeCollection<AvaliacaoLoja>(await Database.ListarAvaliacaoLoja_IdentificadorLoja(Identificador));
            item.Compras = new ObservableRangeCollection<GastoCompra>(await Database.ListarGastoCompra_IdentificadorLoja(Identificador));
            foreach (var itemGastoLoja in item.Compras)
            {
                itemGastoLoja.ItemGasto = await Database.RetornarGasto(itemGastoLoja.IdentificadorGasto);
                itemGastoLoja.ItemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(itemGastoLoja.IdentificadorGasto));

                itemGastoLoja.ItensComprados = new ObservableRangeCollection<ItemCompra>(await Database.ListarItemCompra_IdentificadorGastoCompra(itemGastoLoja.Identificador));
            }
            return item;
        }

        internal static async Task ExcluirLoja(int? Identificador, bool Sincronizado)
        {
            var itemBanco = await CarregarLoja(Identificador);
            if (itemBanco != null)
            {
                if (itemBanco.Identificador > 0 && !Sincronizado)
                {
                    itemBanco.DataExclusao = DateTime.Now.ToUniversalTime();
                    itemBanco.AtualizadoBanco = false;
                    await Database.SalvarLoja(itemBanco);
                    foreach (var itemAvaliacao in itemBanco.Avaliacoes)
                    {
                        if (itemAvaliacao.Identificador > 0)
                        {
                            itemAvaliacao.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemAvaliacao.AtualizadoBanco = false;
                            await Database.SalvarAvaliacaoLoja(itemAvaliacao);
                        }
                        else
                            await Database.ExcluirAvaliacaoLoja(itemAvaliacao);
                    }
                    foreach (var itemGasto in itemBanco.Compras)
                    {
                        await Database.ExcluirItemCompra_IdentificadorGastoCompra(itemGasto.Identificador);
                        if (itemGasto.Identificador > 0)
                        {
                            itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemGasto.AtualizadoBanco = false;
                            await Database.SalvarGastoCompra(itemGasto);
                            itemGasto.ItemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemGasto.ItemGasto.AtualizadoBanco = false;
                            await Database.SalvarGasto(itemGasto.ItemGasto);
                        }
                        else
                        {
                            await Database.ExcluirGasto(itemGasto.ItemGasto);
                            await Database.ExcluirGastoCompra(itemGasto);
                        }
                    }

                }
                else
                {
                    foreach (var itemAvaliacao in itemBanco.Avaliacoes)
                        await Database.ExcluirAvaliacaoLoja(itemAvaliacao);
                    foreach (var itemGasto in itemBanco.Compras)
                    {
                        await Database.ExcluirItemCompra_IdentificadorGastoCompra(itemGasto.Identificador);
                        await Database.ExcluirGasto(itemGasto.ItemGasto);
                        await Database.ExcluirGastoCompra(itemGasto);
                    }
                    await Database.ExcluirLoja(itemBanco);
                }
            }
        }

        internal static async Task ExcluirGastoCompra(int? IdentificadorCompra, bool Sincronizado)
        {
            var itemCompra = await Database.RetornarGastoCompra(IdentificadorCompra);
            if (itemCompra != null)
            {
                await Database.ExcluirItemCompra_IdentificadorGastoCompra(itemCompra.Identificador);

                if (itemCompra.Identificador > 0 && !Sincronizado)
                {
                    var itemGasto = await Database.RetornarGasto(itemCompra.IdentificadorGasto);
                    itemGasto.AtualizadoBanco = false;
                    itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                    await Database.SalvarGasto(itemGasto);
                    itemCompra.DataExclusao = DateTime.Now.ToUniversalTime();
                    itemCompra.AtualizadoBanco = false;
                    await Database.SalvarGastoCompra(itemCompra);
                }
                else
                {
                    await Database.ExcluirGasto(await Database.RetornarGasto(itemCompra.IdentificadorGasto));
                    await Database.ExcluirGastoCompra(itemCompra);
                }
            }
        }

        internal static async Task<ResultadoOperacao> SalvarLoja(Loja itemLoja)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            if (string.IsNullOrEmpty(itemLoja.Nome))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Nome é obrigatório" });
            }


            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemLoja.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemLoja.AtualizadoBanco = false;
                await Database.SalvarLoja(itemLoja);
                foreach (var itemAvaliacao in itemLoja.Avaliacoes)
                {
                    itemAvaliacao.IdentificadorLoja = itemLoja.Identificador;
                    itemAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    itemAvaliacao.AtualizadoBanco = false;
                    await Database.SalvarAvaliacaoLoja(itemAvaliacao);
                }
                itemResultado.IdentificadorRegistro = itemLoja.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Loja salva com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();

            return itemResultado;
        }

        internal static async Task SalvarLojaReplicada(Loja itemLoja)
        {
            var itemLojaBase = await Database.RetornarLoja(itemLoja.Identificador);
            if (itemLojaBase != null)
                itemLoja.Id = itemLojaBase.Id;
            await DatabaseService.SalvarLoja(itemLoja);
            foreach (var itemAvaliacao in itemLoja.Avaliacoes)
            {
                var itemAvaliacaoBase = await Database.RetornarAvaliacaoLoja(itemAvaliacao.Identificador);
                if (itemAvaliacao.DataExclusao.HasValue)
                {
                    if (itemAvaliacaoBase != null)
                        await Database.ExcluirAvaliacaoLoja(itemAvaliacaoBase);
                }
                else
                {
                    if (itemAvaliacaoBase != null)
                        itemAvaliacao.Id = itemAvaliacaoBase.Id;
                    await Database.SalvarAvaliacaoLoja(itemAvaliacao);
                }
            }

        }


        internal static async Task<GastoCompra> CarregarGastoCompra(int? Identificador)
        {
            GastoCompra item = await Database.RetornarGastoCompra(Identificador);
            item.ItemGasto = await Database.RetornarGasto(item.IdentificadorGasto);
            item.ItensComprados = new ObservableRangeCollection<ItemCompra>(await Database.ListarItemCompra_IdentificadorGastoCompra(item.Identificador));
            item.ItemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(item.ItemGasto.Identificador));
            return item;
        }

        internal static async Task SalvarGastoCompraReplicada(GastoCompra itemGasto)
        {
            var itemBase = await Database.RetornarLoja(itemGasto.Identificador);
            if (itemBase != null)
                itemGasto.Id = itemBase.Id;
            var itemBaseCompra = await Database.RetornarGasto(itemGasto.IdentificadorGasto);
            if (itemBaseCompra != null)
            {

                itemGasto.ItemGasto.Id = itemBaseCompra.Id;
            }
            await Database.Excluir_Gasto_Filhos(itemGasto.IdentificadorGasto.GetValueOrDefault());
            await Database.SalvarGasto(itemGasto.ItemGasto);
            foreach (var item in itemGasto.ItemGasto.Usuarios)
            {
                await Database.SalvarGastoDividido(item);
            }
            await Database.SalvarGastoCompra(itemGasto);


        }

        public static async Task<ResultadoOperacao> SalvarGastoCompra(GastoCompra itemGastoCompra)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            var itemGasto = itemGastoCompra.ItemGasto;
            if (!itemGasto.Moeda.HasValue)
                Erros.Add(new MensagemErro() { Mensagem = "O campo Moeda é obrigatório." });
            if (!itemGasto.Valor.HasValue)
                Erros.Add(new MensagemErro() { Mensagem = "O campo Valor é obrigatório." });
            if (string.IsNullOrWhiteSpace(itemGasto.Descricao))
                Erros.Add(new MensagemErro() { Mensagem = "O campo Descrição é obrigatório." });

            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemGasto.AtualizadoBanco = false;
                itemGasto.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemGastoCompra.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemGastoCompra.AtualizadoBanco = false;
                await Database.SalvarGasto(itemGasto);
                itemResultado.IdentificadorRegistro = itemGasto.Identificador;
                itemGastoCompra.IdentificadorGasto = itemGasto.Identificador;
                await Database.SalvarGastoCompra(itemGastoCompra);
                await Database.ExcluirGastoDividido_IdentificadorGasto(itemGasto.Identificador.GetValueOrDefault());

                foreach (var item in itemGasto.Usuarios)
                {
                    item.AtualizadoBanco = false;
                    item.IdentificadorGasto = itemGasto.Identificador;
                    await Database.SalvarGastoDividido(item);
                }

                Erros.Add(new MensagemErro() { Mensagem = "Compra salva com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();


            return itemResultado;
        }

        internal static async Task<Carro> CarregarCarro(int? Identificador)
        {
            Carro item = await Database.RetornarCarro(Identificador);
            if (item.IdentificadorCarroEventoDevolucao.HasValue)
                item.ItemCarroEventoDevolucao = await Database.RetornarCarroEvento(item.IdentificadorCarroEventoDevolucao);
            if (item.IdentificadorCarroEventoRetirada.HasValue)
                item.ItemCarroEventoRetirada = await Database.RetornarCarroEvento(item.IdentificadorCarroEventoRetirada);
            item.Avaliacoes = new ObservableRangeCollection<AvaliacaoAluguel>(await Database.ListarAvaliacaoAluguel_IdentificadorCarro(Identificador));
            item.Gastos = new ObservableRangeCollection<AluguelGasto>(await Database.ListarAluguelGasto_IdentificadorCarro(Identificador));
            item.Deslocamentos = new ObservableRangeCollection<CarroDeslocamento>(await Database.ListarCarroDeslocamento_IdentificadorCarro(Identificador));
            item.Reabastecimentos = new ObservableRangeCollection<Reabastecimento>(await Database.ListarReabastecimento_IdentificadorCarro(Identificador));
            foreach (var itemGastoCarro in item.Gastos)
            {
                itemGastoCarro.ItemGasto = await Database.RetornarGasto(itemGastoCarro.IdentificadorGasto);
                itemGastoCarro.ItemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(itemGastoCarro.IdentificadorGasto));
            }
            foreach (var itemDeslocamento in item.Deslocamentos)
            {
                if (itemDeslocamento.IdentificadorCarroEventoPartida.HasValue)
                    itemDeslocamento.ItemCarroEventoPartida = await Database.RetornarCarroEvento(itemDeslocamento.IdentificadorCarroEventoPartida);
                if (itemDeslocamento.IdentificadorCarroEventoChegada.HasValue)
                    itemDeslocamento.ItemCarroEventoChegada = await Database.RetornarCarroEvento(itemDeslocamento.IdentificadorCarroEventoChegada);
                itemDeslocamento.Usuarios = new ObservableRangeCollection<CarroDeslocamentoUsuario>(await Database.ListarCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(itemDeslocamento.Identificador));
            }
            foreach (var itemReabastecimento in item.Reabastecimentos)
            {
                itemReabastecimento.Gastos = new ObservableRangeCollection<ReabastecimentoGasto>(await Database.ListarReabastecimentoGasto_IdentificadorReabastecimento(itemReabastecimento.Identificador));
                foreach (var itemGastoCarro in itemReabastecimento.Gastos)
                {
                    itemGastoCarro.ItemGasto = await Database.RetornarGasto(itemGastoCarro.IdentificadorGasto);
                    itemGastoCarro.ItemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(itemGastoCarro.IdentificadorGasto));

                }
            }


            return item;
        }

        internal static async Task ExcluirCarro(int? Identificador, bool Sincronizado)
        {
            var itemBanco = await CarregarCarro(Identificador);
            if (itemBanco != null)
            {
                if (itemBanco.Identificador > 0 && !Sincronizado)
                {
                    itemBanco.DataExclusao = DateTime.Now.ToUniversalTime();
                    itemBanco.AtualizadoBanco = false;
                    await Database.SalvarCarro(itemBanco);
                    foreach (var itemAvaliacao in itemBanco.Avaliacoes)
                    {
                        if (itemAvaliacao.Identificador > 0)
                        {
                            itemAvaliacao.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemAvaliacao.AtualizadoBanco = false;
                            await Database.SalvarAvaliacaoAluguel(itemAvaliacao);
                        }
                        else
                            await Database.ExcluirAvaliacaoAluguel(itemAvaliacao);
                    }
                    foreach (var itemGasto in itemBanco.Gastos)
                    {
                        if (itemGasto.Identificador > 0)
                        {
                            itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemGasto.AtualizadoBanco = false;
                            await Database.SalvarAluguelGasto(itemGasto);
                        }
                        else
                            await Database.ExcluirAluguelGasto(itemGasto);
                    }
                    foreach (var itemDeslocamento in itemBanco.Deslocamentos)
                    {
                        foreach (var itemUsuario in itemDeslocamento.Usuarios)
                            await Database.ExcluirCarroDeslocamentoUsuario(itemUsuario);
                        if (itemDeslocamento.Identificador > 0)
                        {
                            itemDeslocamento.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemDeslocamento.AtualizadoBanco = false;
                            await Database.SalvarCarroDeslocamento(itemDeslocamento);
                        }
                        else
                            await Database.ExcluirCarroDeslocamento(itemDeslocamento);
                    }

                    foreach (var itemReabastecimento in itemBanco.Reabastecimentos)
                    {
                        foreach (var itemGasto in itemReabastecimento.Gastos)
                        {
                            await Database.ExcluirGastoDividido_IdentificadorGasto(itemGasto.IdentificadorGasto.GetValueOrDefault());
                            await Database.ExcluirGasto(itemGasto.ItemGasto);
                            await Database.ExcluirReabastecimentoGasto(itemGasto);
                        }
                        if (itemReabastecimento.Identificador > 0)
                        {
                            itemReabastecimento.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemReabastecimento.AtualizadoBanco = false;
                            await Database.SalvarReabastecimento(itemReabastecimento);
                        }
                        else
                            await Database.ExcluirReabastecimento(itemReabastecimento);
                    }

                }

                else
                {
                    foreach (var itemAvaliacao in itemBanco.Avaliacoes)
                        await Database.ExcluirAvaliacaoAluguel(itemAvaliacao);
                    foreach (var itemGasto in itemBanco.Gastos)
                        await Database.ExcluirAluguelGasto(itemGasto);
                    foreach (var itemDeslocamento in itemBanco.Deslocamentos)
                    {
                        foreach (var itemUsuario in itemDeslocamento.Usuarios)
                            await Database.ExcluirCarroDeslocamentoUsuario(itemUsuario);
                        await Database.ExcluirCarroDeslocamento(itemDeslocamento);
                    }
                    foreach (var itemReabastecimento in itemBanco.Reabastecimentos)
                    {
                        foreach (var itemGasto in itemReabastecimento.Gastos)
                        {
                            await Database.ExcluirGastoDividido_IdentificadorGasto(itemGasto.IdentificadorGasto.GetValueOrDefault());
                            await Database.ExcluirGasto(itemGasto.ItemGasto);
                            await Database.ExcluirReabastecimentoGasto(itemGasto);
                        }
                        await Database.ExcluirReabastecimento(itemReabastecimento);
                    }
                    await Database.ExcluirCarro(itemBanco);
                }
            }
        }

        internal static async Task<ResultadoOperacao> SalvarCarro(Carro itemCarro)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            if (string.IsNullOrEmpty(itemCarro.Modelo))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Modelo é obrigatório" });
            }
            if (string.IsNullOrEmpty(itemCarro.Descricao))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Descrição é obrigatório" });
            }
            if (itemCarro.Alugado && string.IsNullOrEmpty(itemCarro.Locadora))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Locadora é obrigatório" });
            }


            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemCarro.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemCarro.AtualizadoBanco = false;
                if (itemCarro.ItemCarroEventoRetirada != null)
                {
                    await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoRetirada);
                    itemCarro.IdentificadorCarroEventoRetirada = itemCarro.ItemCarroEventoRetirada.Identificador;
                }
                else
                    itemCarro.IdentificadorCarroEventoRetirada = null;

                if (itemCarro.ItemCarroEventoDevolucao != null)
                {
                    await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoDevolucao);
                    itemCarro.IdentificadorCarroEventoDevolucao = itemCarro.ItemCarroEventoDevolucao.Identificador;
                }
                else
                    itemCarro.IdentificadorCarroEventoDevolucao = null;
                await Database.SalvarCarro(itemCarro);
                foreach (var itemAvaliacao in itemCarro.Avaliacoes)
                {
                    itemAvaliacao.IdentificadorCarro = itemCarro.Identificador;
                    itemAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    itemAvaliacao.AtualizadoBanco = false;
                    await Database.SalvarAvaliacaoAluguel(itemAvaliacao);
                }
                itemResultado.IdentificadorRegistro = itemCarro.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Carro salvo com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();

            return itemResultado;
        }

        internal static async Task SalvarCarroReplicada(Carro itemCarro)
        {
            var itemCarroBase = await Database.RetornarCarro(itemCarro.Identificador);
            if (itemCarroBase != null)
                itemCarro.Id = itemCarroBase.Id;
            await DatabaseService.SalvarCarro(itemCarro);
            if (itemCarro.ItemCarroEventoRetirada != null)
            {
                var itemCarroEvento = await Database.RetornarCarroEvento(itemCarro.IdentificadorCarroEventoRetirada);
                if (itemCarroEvento != null)
                    itemCarro.ItemCarroEventoRetirada.Id = itemCarroEvento.Id;
                await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoRetirada);
            }
            if (itemCarro.ItemCarroEventoDevolucao != null)
            {
                var itemCarroEvento = await Database.RetornarCarroEvento(itemCarro.IdentificadorCarroEventoDevolucao);
                if (itemCarroEvento != null)
                    itemCarro.ItemCarroEventoDevolucao.Id = itemCarroEvento.Id;
                await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoDevolucao);
            }

            foreach (var itemAvaliacao in itemCarro.Avaliacoes)
            {
                var itemAvaliacaoBase = await Database.RetornarAvaliacaoAluguel(itemAvaliacao.Identificador);
                if (itemAvaliacao.DataExclusao.HasValue)
                {
                    if (itemAvaliacaoBase != null)
                        await Database.ExcluirAvaliacaoAluguel(itemAvaliacaoBase);
                }
                else
                {
                    if (itemAvaliacaoBase != null)
                        itemAvaliacao.Id = itemAvaliacaoBase.Id;
                    await Database.SalvarAvaliacaoAluguel(itemAvaliacao);
                }
            }

        }


        internal static async Task ExcluirCarroDeslocamento(int? Identificador, bool Sincronizado)
        {
            var itemBanco = await Database.RetornarCarroDeslocamento(Identificador);
            if (itemBanco != null)
            {
                if (itemBanco.Identificador > 0 && !Sincronizado)
                {
                    itemBanco.DataExclusao = DateTime.Now.ToUniversalTime();
                    itemBanco.AtualizadoBanco = false;
                    await Database.SalvarCarroDeslocamento(itemBanco);
                    foreach (var itemUsuario in await Database.ListarCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(itemBanco.Identificador))
                        await Database.ExcluirCarroDeslocamentoUsuario(itemUsuario);

                }

                else
                {
                    foreach (var itemUsuario in await Database.ListarCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(itemBanco.Identificador))
                        await Database.ExcluirCarroDeslocamentoUsuario(itemUsuario);

                    await Database.ExcluirCarroDeslocamento(itemBanco);

                }
            }
        }

        internal static async Task SalvarCarroDeslocamentoReplicada(CarroDeslocamento itemCarro)
        {
            var itemCarroBase = await Database.RetornarCarroDeslocamento(itemCarro.Identificador);
            if (itemCarroBase != null)
            {
                itemCarro.Id = itemCarroBase.Id;
                foreach (var itemUsuario in await Database.ListarCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(itemCarroBase.Identificador))
                    await Database.ExcluirCarroDeslocamentoUsuario(itemUsuario);

            }

            await DatabaseService.Database.SalvarCarroDeslocamento(itemCarro);
            if (itemCarro.ItemCarroEventoPartida != null)
            {
                var itemCarroEvento = await Database.RetornarCarroEvento(itemCarro.IdentificadorCarroEventoPartida);
                if (itemCarroEvento != null)
                    itemCarro.ItemCarroEventoPartida.Id = itemCarroEvento.Id;
                await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoPartida);
            }
            if (itemCarro.ItemCarroEventoChegada != null)
            {
                var itemCarroEvento = await Database.RetornarCarroEvento(itemCarro.IdentificadorCarroEventoChegada);
                if (itemCarroEvento != null)
                    itemCarro.ItemCarroEventoChegada.Id = itemCarroEvento.Id;
                await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoChegada);
            }

            foreach (var itemUsuario in itemCarro.Usuarios)
            {
                await Database.SalvarCarroDeslocamentoUsuario(itemUsuario);
            }

        }
        internal static async Task<ResultadoOperacao> SalvarCarroDeslocamento(CarroDeslocamento itemCarro)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();


            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemCarro.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemCarro.AtualizadoBanco = false;
                if (itemCarro.ItemCarroEventoPartida != null)
                {
                    await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoPartida);
                    itemCarro.IdentificadorCarroEventoPartida = itemCarro.ItemCarroEventoPartida.Identificador;
                }
                else
                    itemCarro.IdentificadorCarroEventoPartida = null;

                if (itemCarro.ItemCarroEventoChegada != null)
                {
                    await Database.SalvarCarroEvento(itemCarro.ItemCarroEventoChegada);
                    itemCarro.IdentificadorCarroEventoChegada = itemCarro.ItemCarroEventoChegada.Identificador;
                }
                else
                    itemCarro.IdentificadorCarroEventoChegada = null;
                await Database.SalvarCarroDeslocamento(itemCarro);
                await Database.ExcluirCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(itemCarro.Identificador.GetValueOrDefault());
                foreach (var itemUsuario in itemCarro.Usuarios)
                {
                    itemUsuario.IdentificadorCarroDeslocamento = itemCarro.Identificador;
                    itemUsuario.AtualizadoBanco = false;
                    await Database.SalvarCarroDeslocamentoUsuario(itemUsuario);
                }
                itemResultado.IdentificadorRegistro = itemCarro.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Deslocamento salvo com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();

            return itemResultado;
        }
        internal static async Task<CarroDeslocamento> CarregarCarroDeslocamento(int? Identificador)
        {
            CarroDeslocamento itemDeslocamento = await DatabaseService.Database.RetornarCarroDeslocamento(Identificador);
          
                if (itemDeslocamento.IdentificadorCarroEventoPartida.HasValue)
                    itemDeslocamento.ItemCarroEventoPartida = await Database.RetornarCarroEvento(itemDeslocamento.IdentificadorCarroEventoPartida);
                if (itemDeslocamento.IdentificadorCarroEventoChegada.HasValue)
                    itemDeslocamento.ItemCarroEventoChegada = await Database.RetornarCarroEvento(itemDeslocamento.IdentificadorCarroEventoChegada);
                itemDeslocamento.Usuarios = new ObservableRangeCollection<CarroDeslocamentoUsuario>(await Database.ListarCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(itemDeslocamento.Identificador));
           

            return itemDeslocamento;
        }


        internal static async Task<Reabastecimento> CarregarReabastecimento(int? Identificador)
        {
            Reabastecimento itemReabastecimento = await DatabaseService.Database.RetornarReabastecimento    (Identificador);
            itemReabastecimento.Gastos = new ObservableRangeCollection<ReabastecimentoGasto>(await Database.ListarReabastecimentoGasto_IdentificadorReabastecimento(itemReabastecimento.Identificador));
            foreach (var itemGastoCarro in itemReabastecimento.Gastos)
            {
                itemGastoCarro.ItemGasto = await Database.RetornarGasto(itemGastoCarro.IdentificadorGasto);
                itemGastoCarro.ItemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(itemGastoCarro.IdentificadorGasto));

            }
  

            return itemReabastecimento;
        }

        internal static async Task ExcluirReabastecimento(int? Identificador, bool Sincronizado)
        {
            var itemBanco = await CarregarReabastecimento(Identificador);
            if (itemBanco != null)
            {
                if (itemBanco.Identificador > 0 && !Sincronizado)
                {
                    itemBanco.DataExclusao = DateTime.Now.ToUniversalTime();
                    itemBanco.AtualizadoBanco = false;
                    await Database.SalvarReabastecimento(itemBanco);
                    foreach (var itemGasto in itemBanco.Gastos)
                    {

                        await Database.ExcluirGastoDividido_IdentificadorGasto(itemGasto.IdentificadorGasto.GetValueOrDefault());
                        itemGasto.ItemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                        itemGasto.ItemGasto.AtualizadoBanco = false;
                        await Database.SalvarGasto(itemGasto.ItemGasto);
                        itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                        itemGasto.AtualizadoBanco = false;
                        await Database.SalvarReabastecimentoGasto(itemGasto);
                    }

                }

                else
                {
                    foreach (var itemGasto in itemBanco.Gastos)
                    {

                        await Database.ExcluirGastoDividido_IdentificadorGasto(itemGasto.IdentificadorGasto.GetValueOrDefault());
                        await Database.ExcluirGasto(itemGasto.ItemGasto);
                        await Database.ExcluirReabastecimentoGasto(itemGasto);
                    }
                }

                
            }
        }


        internal static async Task SalvarReabastecimentoReplicada(Reabastecimento itemCarro)
        {
            var itemCarroBase = await Database.RetornarReabastecimento(itemCarro.Identificador);
            if (itemCarroBase != null)
            {
                itemCarro.Id = itemCarroBase.Id;            

            }
            await DatabaseService.Database.SalvarReabastecimento(itemCarro);
            foreach (var itemGasto in itemCarro.Gastos)
            {
                var itemBaseGastoReabastecimento = await Database.RetornarReabastecimentoGasto(itemGasto.Identificador);
                if (itemBaseGastoReabastecimento != null)
                    itemGasto.Id = itemBaseGastoReabastecimento.Id;
                await Database.SalvarReabastecimentoGasto(itemGasto);
                await Database.ExcluirGastoDividido_IdentificadorGasto(itemGasto.IdentificadorGasto.GetValueOrDefault());
                var itemBaseGasto = await Database.RetornarGasto(itemGasto.IdentificadorGasto);
                if (itemBaseGasto != null)
                    itemGasto.ItemGasto.Id = itemBaseGasto.Id;
                await Database.SalvarGasto(itemGasto.ItemGasto);
                foreach (var itemUsuario in itemGasto.ItemGasto.Usuarios)
                    await Database.SalvarGastoDividido(itemUsuario);
            }

           

        }
        internal static async Task<ResultadoOperacao> SalvarReabastecimento(Reabastecimento itemReabastecimento)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            foreach (var itemGastoReabastecimento in itemReabastecimento.Gastos)
            {
                var itemGasto = itemGastoReabastecimento.ItemGasto;
                if (!itemGasto.Moeda.HasValue)
                    Erros.Add(new MensagemErro() { Mensagem = "O campo Moeda é obrigatório." });
                if (!itemGasto.Valor.HasValue)
                    Erros.Add(new MensagemErro() { Mensagem = "O campo Valor é obrigatório." });
                if (string.IsNullOrWhiteSpace(itemGasto.Descricao))
                    Erros.Add(new MensagemErro() { Mensagem = "O campo Descrição é obrigatório." });

            }

            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemReabastecimento.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemReabastecimento.AtualizadoBanco = false;
                await Database.SalvarReabastecimento(itemReabastecimento);
                foreach (var itemGastoReabastecimento in itemReabastecimento.Gastos)
                {
                    var itemGasto = itemGastoReabastecimento.ItemGasto;
                    itemGasto.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    itemGasto.AtualizadoBanco = false;
                    await Database.SalvarGasto(itemGasto);
                    await Database.ExcluirGastoDividido_IdentificadorGasto(itemGasto.Identificador.GetValueOrDefault());
                    foreach (var itemUsuario in itemGasto.Usuarios)
                    {
                        itemUsuario.IdentificadorGasto = itemGasto.Identificador;
                        await Database.SalvarGastoDividido(itemUsuario);
                    }
                    itemGastoReabastecimento.IdentificadorGasto = itemGasto.Identificador;
                    itemGastoReabastecimento.IdentificadorReabastecimento = itemReabastecimento.Identificador;
                    itemGastoReabastecimento.AtualizadoBanco = false;
                    itemGastoReabastecimento.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    await Database.SalvarReabastecimentoGasto(itemGastoReabastecimento);
                }                  
                itemResultado.IdentificadorRegistro = itemReabastecimento.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Reabastecimento salvo com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();

            return itemResultado;
        }


        internal static async Task<ViagemAerea> CarregarViagemAerea(int? Identificador)
        {
            ViagemAerea item = await Database.RetornarViagemAerea(Identificador);
            item.Avaliacoes = new ObservableRangeCollection<AvaliacaoAerea>(await Database.ListarAvaliacaoAerea_IdentificadorViagemAerea(Identificador));
            item.Gastos = new ObservableRangeCollection<GastoViagemAerea>(await Database.ListarGastoViagemAerea_IdentificadorViagemAerea(Identificador));
            foreach (var itemGastoAtracao in item.Gastos)
            {
                itemGastoAtracao.ItemGasto = await Database.RetornarGasto(itemGastoAtracao.IdentificadorGasto);
                //itemGastoAtracao.ItemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(itemGastoAtracao.IdentificadorGasto));

            }
            item.Aeroportos = new ObservableRangeCollection<ViagemAereaAeroporto>(await Database.ListarViagemAereaAeroporto_IdentificadorViagemAerea(Identificador));
            return item;
        }


        internal static async Task ExcluirViagemAerea(int? Identificador, bool Sincronizado)
        {
            var itemBanco = await CarregarViagemAerea(Identificador);
            if (itemBanco != null)
            {
                if (itemBanco.Identificador > 0 && !Sincronizado)
                {
                    itemBanco.DataExclusao = DateTime.Now.ToUniversalTime();
                    itemBanco.AtualizadoBanco = false;
                    await Database.SalvarViagemAerea(itemBanco);
                    foreach (var itemAvaliacao in itemBanco.Avaliacoes)
                    {
                        if (itemAvaliacao.Identificador > 0)
                        {
                            itemAvaliacao.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemAvaliacao.AtualizadoBanco = false;
                            await Database.SalvarAvaliacaoAerea(itemAvaliacao);
                        }
                        else
                            await Database.ExcluirAvaliacaoAerea(itemAvaliacao);
                    }
                    foreach (var itemGasto in itemBanco.Gastos)
                    {
                        if (itemGasto.Identificador > 0)
                        {
                            itemGasto.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemGasto.AtualizadoBanco = false;
                            await Database.SalvarGastoViagemAerea(itemGasto);
                        }
                        else
                            await Database.ExcluirGastoViagemAerea(itemGasto);
                    }

                    foreach (var itemAeroporto in itemBanco.Aeroportos)
                    {
                        if (itemAeroporto.Identificador > 0)
                        {
                            itemAeroporto.DataExclusao = DateTime.Now.ToUniversalTime();
                            itemAeroporto.AtualizadoBanco = false;
                            await Database.SalvarViagemAereaAeroporto(itemAeroporto);
                        }
                        else
                            await Database.ExcluirViagemAereaAeroporto(itemAeroporto);
                    }

                }
                else
                {
                    foreach (var itemAvaliacao in itemBanco.Avaliacoes)
                        await Database.ExcluirAvaliacaoAerea(itemAvaliacao);
                    foreach (var itemGasto in itemBanco.Gastos)
                        await Database.ExcluirGastoViagemAerea(itemGasto);
                    foreach (var itemAeroporto in itemBanco.Aeroportos)
                        await Database.ExcluirViagemAereaAeroporto(itemAeroporto);
                    await Database.ExcluirViagemAerea(itemBanco);
                }
            }
        }

        internal static async Task<ResultadoOperacao> SalvarViagemAerea(ViagemAerea itemViagemAerea)
        {
            ResultadoOperacao itemResultado = new ResultadoOperacao();
            List<MensagemErro> Erros = new List<MensagemErro>();
            if (string.IsNullOrEmpty(itemViagemAerea.Descricao))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Descrição é obrigatório" });
            }
            if (string.IsNullOrEmpty(itemViagemAerea.CompanhiaAerea))
            {
                Erros.Add(new MensagemErro() { Mensagem = "O campo Companhia é obrigatório" });
            }
            foreach (var itemAeroporto in itemViagemAerea.Aeroportos)
            {
                if (string.IsNullOrEmpty(itemAeroporto.Aeroporto))
                    Erros.Add(new MensagemErro() { Mensagem = "O campo Aeroporto/Porto/Estação é obrigatório" });

            }

            itemResultado.Sucesso = !Erros.Any();
            if (itemResultado.Sucesso)
            {
                itemViagemAerea.DataAtualizacao = DateTime.Now.ToUniversalTime();
                itemViagemAerea.AtualizadoBanco = false;
                await Database.SalvarViagemAerea(itemViagemAerea);
                foreach (var itemAvaliacao in itemViagemAerea.Avaliacoes)
                {
                    itemAvaliacao.IdentificadorViagemAerea = itemViagemAerea.Identificador;
                    itemAvaliacao.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    itemAvaliacao.AtualizadoBanco = false;
                    await Database.SalvarAvaliacaoAerea(itemAvaliacao);
                }
                foreach (var itemAeroporto in itemViagemAerea.Aeroportos)
                {
                    itemAeroporto.IdentificadorViagemAerea = itemViagemAerea.Identificador;
                    itemAeroporto.DataAtualizacao = DateTime.Now.ToUniversalTime();
                    itemAeroporto.AtualizadoBanco = false;
                    await Database.SalvarViagemAereaAeroporto(itemAeroporto);
                }
                itemResultado.IdentificadorRegistro = itemViagemAerea.Identificador;
                Erros.Add(new MensagemErro() { Mensagem = "Deslocamento salvo com sucesso" });
            }
            itemResultado.Mensagens = Erros.ToArray();

            return itemResultado;
        }

        internal static async Task SalvarViagemAereaReplicada(ViagemAerea itemViagemAerea)
        {
            var itemViagemAereaBase = await Database.RetornarViagemAerea(itemViagemAerea.Identificador);
            if (itemViagemAereaBase != null)
                itemViagemAerea.Id = itemViagemAereaBase.Id;
            await DatabaseService.SalvarViagemAerea(itemViagemAerea);
            foreach (var itemAvaliacao in itemViagemAerea.Avaliacoes)
            {
                var itemAvaliacaoBase = await Database.RetornarAvaliacaoAerea(itemAvaliacao.Identificador);
                if (itemAvaliacao.DataExclusao.HasValue)
                {
                    if (itemAvaliacaoBase != null)
                        await Database.ExcluirAvaliacaoAerea(itemAvaliacaoBase);
                }
                else
                {
                    if (itemAvaliacaoBase != null)
                        itemAvaliacao.Id = itemAvaliacaoBase.Id;
                    await Database.SalvarAvaliacaoAerea(itemAvaliacao);
                }
            }
            foreach (var itemAeroporto in itemViagemAerea.Aeroportos)
            {
                var itemAeroportoBase = await Database.RetornarViagemAereaAeroporto(itemAeroporto.Identificador);
                if (itemAeroporto.DataExclusao.HasValue)
                {
                    if (itemAeroportoBase != null)
                        await Database.ExcluirViagemAereaAeroporto(itemAeroportoBase);
                }
                else
                {
                    if (itemAeroportoBase != null)
                        itemAeroporto.Id = itemAeroportoBase.Id;
                    await Database.SalvarViagemAereaAeroporto(itemAeroporto);
                }
            }
        }

        internal static async Task<ClasseSincronizacao> CarregarDadosEnvioSincronizar()
        {
            ClasseSincronizacao itemControle = new ClasseSincronizacao();
            itemControle.AportesDinheiro = await Database.ListarAporteDinheiro_Pendente();
            List<int?> ListaGastosCarregados = new List<int?>();
            foreach (var itemAporte in itemControle.AportesDinheiro.Where(d => d.IdentificadorGasto.HasValue))
            {
                itemAporte.ItemGasto = await Database.RetornarGasto(itemAporte.IdentificadorGasto);
                ListaGastosCarregados.Add(itemAporte.IdentificadorGasto);
            }
            itemControle.CotacoesMoeda = await Database.ListarCotacaoMoeda_Pendente();
            itemControle.Atracoes = await Database.ListarAtracao_Pendente();
            foreach (var itemAtracao in itemControle.Atracoes)
                itemAtracao.Avaliacoes = new System.Collections.ObjectModel.ObservableCollection<AvaliacaoAtracao>( await Database.ListarAvaliacaoAtracao_IdentificadorAtracao(itemAtracao.Identificador));
            itemControle.CalendariosPrevistos = await Database.ListarCalendarioPrevisto_Pendente();
            itemControle.Carros = await Database.ListarCarro_Pendente();
            foreach (var itemCarro in itemControle.Carros)
            {
                itemCarro.Avaliacoes = new ObservableRangeCollection<AvaliacaoAluguel>(await Database.ListarAvaliacaoAluguel_IdentificadorCarro(itemCarro.Identificador));
                if (itemCarro.IdentificadorCarroEventoDevolucao.HasValue)
                    itemCarro.ItemCarroEventoDevolucao = await Database.RetornarCarroEvento(itemCarro.IdentificadorCarroEventoDevolucao);
                if (itemCarro.IdentificadorCarroEventoRetirada.HasValue)
                    itemCarro.ItemCarroEventoRetirada = await Database.RetornarCarroEvento(itemCarro.IdentificadorCarroEventoRetirada);
            }
            itemControle.CarroDeslocamentos = await Database.ListarCarroDeslocamento_Pendente();
            foreach (var itemCarro in itemControle.CarroDeslocamentos)
            {
                itemCarro.Usuarios = new ObservableRangeCollection<CarroDeslocamentoUsuario>(await Database.ListarCarroDeslocamentoUsuario_IdentificadorCarroDeslocamento(itemCarro.Identificador));
                if (itemCarro.IdentificadorCarroEventoChegada.HasValue)
                    itemCarro.ItemCarroEventoChegada = await Database.RetornarCarroEvento(itemCarro.IdentificadorCarroEventoChegada);
                if (itemCarro.IdentificadorCarroEventoPartida.HasValue)
                    itemCarro.ItemCarroEventoPartida= await Database.RetornarCarroEvento(itemCarro.IdentificadorCarroEventoPartida);
            }

            itemControle.Reabastecimento = await Database.ListarReabastecimento_Pendente();
            foreach (var itemCarro in itemControle.Reabastecimento)
            {
                itemCarro.Gastos = new ObservableRangeCollection<ReabastecimentoGasto>(await Database.ListarReabastecimentoGasto_IdentificadorReabastecimento(itemCarro.Identificador));
                foreach (var itemGasto in itemCarro.Gastos)
                {
                    itemGasto.ItemGasto = await Database.RetornarGasto(itemGasto.IdentificadorGasto);
                    ListaGastosCarregados.Add(itemGasto.IdentificadorGasto);
                }
            }
            itemControle.Comentarios = await Database.ListarComentario_Pendente();
            itemControle.Compras = await Database.ListarGastoCompra_Pendente();
            foreach (var itemGasto in itemControle.Compras)
            {
                ListaGastosCarregados.Add(itemGasto.IdentificadorGasto);
                itemGasto.ItemGasto = await Database.RetornarGasto(itemGasto.IdentificadorGasto);
            }
            itemControle.ItensComprados = await Database.ListarItemCompra_Pendente();
            itemControle.Deslocamentos = await Database.ListarViagemAerea_Pendente();
            foreach (var itemVA in itemControle.Deslocamentos)
            {
                itemVA.Avaliacoes = new ObservableRangeCollection<AvaliacaoAerea>(await Database.ListarAvaliacaoAerea_IdentificadorViagemAerea(itemVA.Identificador));
                itemVA.Aeroportos = new ObservableRangeCollection<ViagemAereaAeroporto>(await database.ListarViagemAereaAeroporto_IdentificadorViagemAerea(itemVA.Identificador));
            }
            itemControle.Gastos = await Database.ListarGasto_Pendente(ListaGastosCarregados);
            foreach (var itemGasto in itemControle.Gastos)
            {
                itemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(itemGasto.Identificador));
                itemGasto.Atracoes = new ObservableRangeCollection<GastoAtracao>(await Database.ListarGastoAtracao_IdentificadorGasto(itemGasto.Identificador));
                itemGasto.Hoteis = new ObservableRangeCollection<GastoHotel>(await Database.ListarGastoHotel_IdentificadorGasto(itemGasto.Identificador));
                itemGasto.Alugueis = new ObservableRangeCollection<AluguelGasto>(await Database.ListarAluguelGasto_IdentificadorGasto(itemGasto.Identificador));
                itemGasto.Refeicoes = new ObservableRangeCollection<GastoRefeicao>(await Database.ListarGastoRefeicao_IdentificadorGasto(itemGasto.Identificador));
                itemGasto.ViagenAereas = new ObservableRangeCollection<GastoViagemAerea>(await Database.ListarGastoViagemAerea_IdentificadorGasto(itemGasto.Identificador));

            }
            itemControle.GastosAtracao = (await Database.ListarGastoAtracao_Pendente()).Where(d => !itemControle.Gastos.Where(f => f.Identificador == d.IdentificadorGasto).Any()).ToList();
            itemControle.GastosCarro = (await Database.ListarAluguelGasto_Pendente()).Where(d => !itemControle.Gastos.Where(f => f.Identificador == d.IdentificadorGasto).Any()).ToList();
            itemControle.GastosDeslocamento = (await Database.ListarGastoViagemAerea_Pendente()).Where(d => !itemControle.Gastos.Where(f => f.Identificador == d.IdentificadorGasto).Any()).ToList();
            itemControle.GastosHotel = (await Database.ListarGastoHotel_Pendente()).Where(d => !itemControle.Gastos.Where(f => f.Identificador == d.IdentificadorGasto).Any()).ToList();
            itemControle.GastosRefeicao= (await Database.ListarGastoRefeicao_Pendente()).Where(d => !itemControle.Gastos.Where(f => f.Identificador == d.IdentificadorGasto).Any()).ToList();

            itemControle.Hoteis = await Database.ListarHotel_Pendente();
            itemControle.EventosHotel = await Database.ListarHotelEvento_Pendente();
            foreach (var itemHotel in itemControle.Hoteis)
                itemHotel.Avaliacoes = new ObservableRangeCollection<HotelAvaliacao>(await Database.ListarHotelAvaliacao_IdentificadorHotel(itemHotel.Identificador));

            itemControle.ListaCompra = await Database.ListarListaCompra_Pendente();
            itemControle.Lojas = await Database.ListarLoja_Pendente();

            foreach (var itemLoja in itemControle.Lojas)
                itemLoja.Avaliacoes = new ObservableRangeCollection<AvaliacaoLoja>(await Database.ListarAvaliacaoLoja_IdentificadorLoja(itemLoja.Identificador));

            itemControle.Refeicoes = await Database.ListarRefeicao_Pendente();
            foreach (var itemRefeicao in itemControle.Refeicoes)
                itemRefeicao.Pedidos = new ObservableRangeCollection<RefeicaoPedido>(await Database.ListarRefeicaoPedido_IdentificadorRefeicao(itemRefeicao.Identificador));
            itemControle.Posicoes = await Database.ListarPosicao_Pendente();
            itemControle.Sugestoes = await Database.ListarSugestao_Pendente();
            return itemControle;
        }
    }
}
