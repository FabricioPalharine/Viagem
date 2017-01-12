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
            item.Avaliacoes = new ObservableRangeCollection<HotelAvaliacao>(await Database.ListarHotelAvaliacao_IdentificadorHotel(Identificador));
            item.Gastos = new ObservableRangeCollection<GastoHotel>(await Database.ListarGastoHotel_IdentificadorHotel(Identificador));
            item.Eventos = new ObservableRangeCollection<HotelEvento>(await Database.ListarHotelEvento_IdentificadorHotel(Identificador));
            foreach (var itemGastoHotel in item.Gastos)
            {
                itemGastoHotel.ItemGasto = await Database.RetornarGasto(itemGastoHotel.IdentificadorGasto);
                itemGastoHotel.ItemGasto.Usuarios = new ObservableRangeCollection<GastoDividido>(await Database.ListarGastoDividido_IdentificadorGasto(itemGastoHotel.IdentificadorGasto));

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
            if (itemCarro.Alugado && !string.IsNullOrEmpty(itemCarro.Locadora))
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
                    itemUsuario.IdentificadorUsuario = itemCarro.Identificador;
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

    }
}
