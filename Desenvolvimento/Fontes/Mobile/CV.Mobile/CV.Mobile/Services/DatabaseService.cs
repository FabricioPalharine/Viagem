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


                foreach (var item in itemGasto.Alugueis.Where(d=>!d.DataExclusao.HasValue || d.Identificador > 0))
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
                var itemAvaliacaoBase = await Database.RetornarGastoAtracao(itemAvaliacao.Identificador);
                if (itemAvaliacao.DataExclusao.HasValue)
                {
                    if (itemAvaliacaoBase != null)
                        await Database.ExcluirGastoAtracao(itemAvaliacaoBase);
                }
                else
                {
                    if (itemAvaliacaoBase != null)
                        itemAvaliacao.Id = itemAvaliacaoBase.Id;
                    await Database.SalvarAvaliacaoAtracao(itemAvaliacao);
                }
            }

        }
    }
}
