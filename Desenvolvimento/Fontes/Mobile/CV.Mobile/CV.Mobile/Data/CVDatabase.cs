﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV.Mobile.Models;

namespace CV.Mobile.Data
{
    public class CVDatabase
    {
        readonly SQLiteAsyncConnection database;

        public CVDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath, true);
            database.CreateTableAsync<ControleSincronizacao>().Wait();
            database.CreateTableAsync<AluguelGasto>().Wait();
            database.CreateTableAsync<AporteDinheiro>().Wait();
            database.CreateTableAsync<Atracao>().Wait();
            database.CreateTableAsync<AvaliacaoAerea>().Wait();
            database.CreateTableAsync<AvaliacaoAluguel>().Wait();
            database.CreateTableAsync<AvaliacaoAtracao>().Wait();
            database.CreateTableAsync<AvaliacaoLoja>().Wait();
            database.CreateTableAsync<CalendarioPrevisto>().Wait();
            database.CreateTableAsync<Carro>().Wait();
            database.CreateTableAsync<CarroDeslocamento>().Wait();
            database.CreateTableAsync<CarroDeslocamentoUsuario>().Wait();
            database.CreateTableAsync<CarroEvento>().Wait();
            database.CreateTableAsync<Cidade>().Wait();
            database.CreateTableAsync<Comentario>().Wait();
            database.CreateTableAsync<CotacaoMoeda>().Wait();
            database.CreateTableAsync<UploadFoto>().Wait();
            database.CreateTableAsync<Gasto>().Wait();
            database.CreateTableAsync<GastoAtracao>().Wait();
            database.CreateTableAsync<GastoCompra>().Wait();
            database.CreateTableAsync<GastoDividido>().Wait();
            database.CreateTableAsync<GastoHotel>().Wait();
            database.CreateTableAsync<GastoRefeicao>().Wait();
            database.CreateTableAsync<GastoViagemAerea>().Wait();
            database.CreateTableAsync<Hotel>().Wait();
            database.CreateTableAsync<HotelAvaliacao>().Wait();
            database.CreateTableAsync<HotelEvento>().Wait();
            database.CreateTableAsync<ItemCompra>().Wait();
            database.CreateTableAsync<ListaCompra>().Wait();
            database.CreateTableAsync<Loja>().Wait();
            database.CreateTableAsync<ParticipanteViagem>().Wait();
            database.CreateTableAsync<Posicao>().Wait();
            database.CreateTableAsync<Reabastecimento>().Wait();
            database.CreateTableAsync<ReabastecimentoGasto>().Wait();
            database.CreateTableAsync<Refeicao>().Wait();
            database.CreateTableAsync<RefeicaoPedido>().Wait();
            database.CreateTableAsync<Sugestao>().Wait();
            database.CreateTableAsync<Usuario>().Wait();
            database.CreateTableAsync<UsuarioGasto>().Wait();
            database.CreateTableAsync<ViagemAerea>().Wait();
            database.CreateTableAsync<ViagemAereaAeroporto>().Wait();
            database.CreateTableAsync<Viagem>().Wait();
            database.CreateTableAsync<Amigo>().Wait();
        }

        //public Task<List<TodoItem>> GetItemsAsync()
        //{
        //    return database.Table<TodoItem>().ToListAsync();
        //}

        //public Task<List<TodoItem>> GetItemsNotDoneAsync()
        //{
        //    return database.QueryAsync<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        //}

        //public Task<TodoItem> GetItemAsync(int id)
        //{
        //    return database.Table<TodoItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        //}

        public async Task<int> SalvarViagemAsync(Viagem item)
        {
            if (item.Id.HasValue)
            {
                return await database.UpdateAsync(item);
            }
            else
            {
                return await database.InsertAsync(item);
            }
        }

        public async Task<int> ExcluirViagemAsync(Viagem item)
        {
            return await database.DeleteAsync(item);
        }

        public async Task<Viagem> GetViagemAtualAsync()
        {
            return await database.Table<Viagem>().FirstOrDefaultAsync();
        }


        public async Task<ControleSincronizacao> GetControleSincronizacaoAsync()
        {
            var itemCS = await database.Table<ControleSincronizacao>().FirstOrDefaultAsync();
            if (itemCS == null)
                itemCS = new ControleSincronizacao() { SincronizadoEnvio = false, UltimaDataEnvio = DateTime.Now.ToUniversalTime(), UltimaDataRecepcao = DateTime.Now.ToUniversalTime() };
            return itemCS;
        }
        public async Task SalvarControleSincronizacao(ControleSincronizacao item)
        {
            await database.InsertOrReplaceAsync(item);
        }
        public async Task LimparBancoViagem()
        {
            await database.ExecuteAsync("Delete from ControleSincronizacao");
            await database.ExecuteAsync("Delete from AluguelGasto");
            await database.ExecuteAsync("Delete from AporteDinheiro");
            await database.ExecuteAsync("Delete from Atracao");
            await database.ExecuteAsync("Delete from AvaliacaoAerea");
            await database.ExecuteAsync("Delete from AvaliacaoAluguel");
            await database.ExecuteAsync("Delete from AvaliacaoAtracao");
            await database.ExecuteAsync("Delete from AvaliacaoLoja");
            await database.ExecuteAsync("Delete from CalendarioPrevisto");
            await database.ExecuteAsync("Delete from Carro");
            await database.ExecuteAsync("Delete from CarroDeslocamento");
            await database.ExecuteAsync("Delete from CarroDeslocamentoUsuario");
            await database.ExecuteAsync("Delete from CarroEvento");
            await database.ExecuteAsync("Delete from Cidade");
            await database.ExecuteAsync("Delete from Comentario");
            await database.ExecuteAsync("Delete from UploadFoto");
            await database.ExecuteAsync("Delete from Gasto");
            await database.ExecuteAsync("Delete from GastoAtracao");
            await database.ExecuteAsync("Delete from GastoCompra");
            await database.ExecuteAsync("Delete from GastoDividido");
            await database.ExecuteAsync("Delete from GastoHotel");
            await database.ExecuteAsync("Delete from GastoRefeicao");
            await database.ExecuteAsync("Delete from GastoViagemAerea");
            await database.ExecuteAsync("Delete from Hotel");
            await database.ExecuteAsync("Delete from HotelAvaliacao");
            await database.ExecuteAsync("Delete from HotelEvento");
            await database.ExecuteAsync("Delete from ItemCompra");
            await database.ExecuteAsync("Delete from ListaCompra");
            await database.ExecuteAsync("Delete from Loja");
            await database.ExecuteAsync("Delete from ParticipanteViagem");
            await database.ExecuteAsync("Delete from Posicao");
            await database.ExecuteAsync("Delete from Reabastecimento");
            await database.ExecuteAsync("Delete from ReabastecimentoGasto");
            await database.ExecuteAsync("Delete from Refeicao");
            await database.ExecuteAsync("Delete from RefeicaoPedido");
            await database.ExecuteAsync("Delete from UsuarioGasto");
            await database.ExecuteAsync("Delete from ViagemAerea");
            await database.ExecuteAsync("Delete from Sugestao");
            await database.ExecuteAsync("Delete from ViagemAereaAeroporto");
            await database.ExecuteAsync("Delete from Viagem");

        }

        public async Task<Usuario> CarregarUsuario(int Id)
        {
            return await database.Table<Usuario>().Where(d => d.Identificador == Id).FirstOrDefaultAsync();
        }

        public Task<List<UsuarioGasto>> ListarUsuarioGastoAsync()
        {
            return database.Table<UsuarioGasto>().ToListAsync();
        }

        public Task<List<ParticipanteViagem>> ListarParticipanteViagemAsync()
        {
            return database.Table<ParticipanteViagem>().ToListAsync();
        }

        public async Task<int> SalvarParticipanteViagemAsync(ParticipanteViagem item)
        {
            if (item.Id.HasValue)
            {
                return await database.UpdateAsync(item);
            }
            else
            {
                return await database.InsertAsync(item);
            }
        }

        public async Task<int> ExcluirParticipanteViagemAsync(ParticipanteViagem item)
        {

            return await database.DeleteAsync(item);

        }

        public async Task<int> SalvarUsuarioGastoAsync(UsuarioGasto item)
        {
            if (item.Id.HasValue)
            {
                return await database.UpdateAsync(item);
            }
            else
            {
                return await database.InsertAsync(item);
            }
        }

        public async Task<int> SalvarUsuarioAsync(Usuario item)
        {
            if (item.Id.HasValue)
            {
                return await database.UpdateAsync(item);
            }
            else
            {
                return await database.InsertAsync(item);
            }
        }

        public async Task LimparAmigos()
        {
            await database.ExecuteAsync("Delete from Atracao");

        }

        public async Task SalvarAmigo(Amigo itemAmigo)
        {
            await database.InsertOrReplaceAsync(itemAmigo);
        }

        public async Task ExcluirAmigo(Amigo itemAmigo)
        {
            await database.DeleteAsync(itemAmigo);
        }

        public async Task<Amigo> RetornarAmigoIdentificadorUsuario(int IdentificadorUsuario)
        {
            return await database.Table<Amigo>().Where(d => d.IdentificadorAmigo == IdentificadorUsuario).FirstOrDefaultAsync();
        }

        public async Task IncluirListaAmigo(List<Amigo> Lista)
        {
            await database.InsertAllAsync(Lista);
        }




        public async Task SalvarCalendarioPrevisto(CalendarioPrevisto itemCalendarioPrevisto)
        {
            await database.InsertOrReplaceAsync(itemCalendarioPrevisto);
            if (!itemCalendarioPrevisto.Identificador.HasValue)
            {
                itemCalendarioPrevisto.Identificador = itemCalendarioPrevisto.Id * -1;
                await database.InsertOrReplaceAsync(itemCalendarioPrevisto);
            }
        }

        public async Task SalvarSugestao(Sugestao itemSugestao)
        {
            await database.InsertOrReplaceAsync(itemSugestao);
            if (!itemSugestao.Identificador.HasValue)
            {
                itemSugestao.Identificador = itemSugestao.Id * -1;
                await database.InsertOrReplaceAsync(itemSugestao);
            }
        }

        public async Task SalvarComentario(Comentario itemComentario)
        {
            await database.InsertOrReplaceAsync(itemComentario);
            if (!itemComentario.Identificador.HasValue)
            {
                itemComentario.Identificador = itemComentario.Id * -1;
                await database.InsertOrReplaceAsync(itemComentario);
            }
        }

        public async Task ExcluirComentario(Comentario itemComentario)
        {
            await database.DeleteAsync(itemComentario);
        }

        public async Task<Comentario> RetornarComentario(int? Identificador)
        {
            return await database.Table<Comentario>().Where(d => d.Identificador == Identificador).FirstOrDefaultAsync();
        }

        public async Task<CotacaoMoeda> RetornarCotacaoMoeda(int? Identificador)
        {
            return await database.Table<CotacaoMoeda>().Where(d => d.Identificador == Identificador).FirstOrDefaultAsync();
        }

        public async Task<List<Comentario>> ListarComentario(CriterioBusca itemBusca)
        {
            var query = database.Table<Comentario>();//.Where(d=>!d.DataExclusao.HasValue);
            if (itemBusca.DataInicioDe.HasValue)
                query = query.Where(d => d.Data >= itemBusca.DataInicioDe);
            if (itemBusca.DataInicioAte.HasValue)
                query = query.Where(d => d.Data <= itemBusca.DataInicioAte);
            if (itemBusca.IdentificadorCidade.HasValue)
                query = query.Where(d => d.IdentificadorCidade == itemBusca.IdentificadorCidade);
            return (await query.ToListAsync()).Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<List<Cidade>> ListarCidade_Tipo(string Tipo)
        {
            return await database.Table<Cidade>().Where(d => d.Tipo == Tipo).ToListAsync();
        }

        public async Task SalvarUploadFoto(UploadFoto itemUploadFoto)
        {
            await database.InsertOrReplaceAsync(itemUploadFoto);
        }

        public async Task ExcluirUploadFoto(UploadFoto itemUploadFoto)
        {
            await database.DeleteAsync(itemUploadFoto);
        }

        public async Task<List<UploadFoto>> ListarUploadFoto_Video(bool Video)
        {
            return await database.Table<UploadFoto>().Where(d => d.Video == Video).ToListAsync();
        }

        public async Task<List<Sugestao>> ListarSugestao(CriterioBusca itemBusca)
        {
            var query = database.Table<Sugestao>();//.Where(d => !d.DataExclusao.HasValue);


            if (itemBusca.IdentificadorParticipante.HasValue)
                query = query.Where(d => d.IdentificadorUsuario == itemBusca.IdentificadorParticipante);
            if (!string.IsNullOrEmpty(itemBusca.Nome))
                query = query.Where(d => d.Local.Contains(itemBusca.Nome));
            if (!string.IsNullOrEmpty(itemBusca.Tipo))
                query = query.Where(d => d.Tipo.Contains(itemBusca.Tipo));
            if (itemBusca.Situacao.HasValue && itemBusca.Situacao > -1)
            {
                if (itemBusca.Situacao == 1)
                    query = query.Where(d => d.Status <= 1);
                else
                    query = query.Where(d => d.Status == itemBusca.Situacao);
            }
            if (itemBusca.IdentificadorCidade.HasValue)
                query = query.Where(d => d.IdentificadorCidade == itemBusca.IdentificadorCidade);
            return (await query.ToListAsync()).Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<List<Usuario>> ListarAmigos()
        {
            return await database.QueryAsync<Usuario>("SELECT a.Identificador, a.Nome From Amigo b INNER JOIN Usuario a on b.IdentificadorAmigo = a.Identificador");
        }

        public async Task<List<Usuario>> ListarAmigosComigo(int IdentificadorUsuario)
        {
            var Lista = await database.QueryAsync<Usuario>("SELECT a.Identificador, a.Nome From Amigo b INNER JOIN Usuario a on b.IdentificadorAmigo = a.Identificador");
            var usuario = await CarregarUsuario(IdentificadorUsuario);
            if (!Lista.Where(d => d.Identificador == usuario.Identificador).Any())
                Lista.Add(usuario);
            return Lista.OrderBy(d => d.Nome).ToList();
        }

        public async Task<List<Usuario>> ListarParticipanteViagem()
        {
            return await database.QueryAsync<Usuario>("SELECT a.Identificador, a.Nome From ParticipanteViagem b INNER JOIN Usuario a on b.IdentificadorUsuario = a.Identificador");
        }

        public async Task SalvarAporteDinheiro(AporteDinheiro itemAporteDinheiro)
        {
            await database.InsertOrReplaceAsync(itemAporteDinheiro);
            if (!itemAporteDinheiro.Identificador.HasValue)
            {
                itemAporteDinheiro.Identificador = itemAporteDinheiro.Id * -1;
                await database.InsertOrReplaceAsync(itemAporteDinheiro);
            }
        }

        public async Task ExcluirAporteDinheiro(AporteDinheiro itemAporteDinheiro)
        {
            await database.DeleteAsync(itemAporteDinheiro);
        }

        public async Task<AporteDinheiro> RetornarAporteDinheiro(int? Identificador)
        {
            return await database.Table<AporteDinheiro>().Where(d => d.Identificador == Identificador).FirstOrDefaultAsync();
        }

        public async Task<List<AporteDinheiro>> ListarAporteDinheiro(CriterioBusca itemBusca)
        {
            var query = database.Table<AporteDinheiro>();//.Where(d=>!d.DataExclusao.HasValue);
            if (itemBusca.DataInicioDe.HasValue)
                query = query.Where(d => d.DataAporte >= itemBusca.DataInicioDe);
            if (itemBusca.DataInicioAte.HasValue)
                query = query.Where(d => d.DataAporte <= itemBusca.DataInicioAte);
            if (itemBusca.Moeda.HasValue)
                query = query.Where(d => d.Moeda == itemBusca.Moeda);
            return (await query.ToListAsync()).Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<Gasto> RetornarGasto(int? Identificador)
        {
            return await database.Table<Gasto>().Where(d => d.Identificador == Identificador).FirstOrDefaultAsync();
        }

        public async Task<List<AluguelGasto>> ListarAluguelGasto_IdentificadorGasto(int? IdentificadorGasto)
        {
            var query = database.Table<AluguelGasto>().Where(d => d.IdentificadorGasto == IdentificadorGasto);
            return (await query.ToListAsync());//.Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<List<GastoAtracao>> ListarGastoAtracao_IdentificadorGasto(int? IdentificadorGasto)
        {
            var query = database.Table<GastoAtracao>().Where(d => d.IdentificadorGasto == IdentificadorGasto);
            return (await query.ToListAsync());//.Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<List<GastoCompra>> ListarGastoCompra_IdentificadorGasto(int? IdentificadorGasto)
        {
            var query = database.Table<GastoCompra>().Where(d => d.IdentificadorGasto == IdentificadorGasto);
            return (await query.ToListAsync());//.Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<List<GastoDividido>> ListarGastoDividido_IdentificadorGasto(int? IdentificadorGasto)
        {
            var query = database.Table<GastoDividido>().Where(d => d.IdentificadorGasto == IdentificadorGasto);
            return (await query.ToListAsync());
        }

        public async Task<List<GastoHotel>> ListarGastoHotel_IdentificadorGasto(int? IdentificadorGasto)
        {
            var query = database.Table<GastoHotel>().Where(d => d.IdentificadorGasto == IdentificadorGasto);
            return (await query.ToListAsync());//.Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<List<GastoRefeicao>> ListarGastoRefeicao_IdentificadorGasto(int? IdentificadorGasto)
        {
            var query = database.Table<GastoRefeicao>().Where(d => d.IdentificadorGasto == IdentificadorGasto);
            return (await query.ToListAsync());//.Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<List<GastoViagemAerea>> ListarGastoViagemAerea_IdentificadorGasto(int? IdentificadorGasto)
        {
            var query = database.Table<GastoViagemAerea>().Where(d => d.IdentificadorGasto == IdentificadorGasto);
            return (await query.ToListAsync());//.Where(d => !d.DataExclusao.HasValue).ToList();
        }
        public async Task<List<ReabastecimentoGasto>> ListarReabastecimentoGasto_IdentificadorGasto(int? IdentificadorGasto)
        {
            var query = database.Table<ReabastecimentoGasto>().Where(d => d.IdentificadorGasto == IdentificadorGasto);
            return (await query.ToListAsync());//.Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task SalvarGasto(Gasto Item)
        {
            await database.InsertOrReplaceAsync(Item);
            if (!Item.Identificador.HasValue)
            {
                Item.Identificador = Item.Id * -1;
                await database.InsertOrReplaceAsync(Item);
            }
        }

        public async Task SalvarAluguelGasto(AluguelGasto Item)
        {
            await database.InsertOrReplaceAsync(Item);
            if (!Item.Identificador.HasValue)
            {
                Item.Identificador = Item.Id * -1;
                await database.InsertOrReplaceAsync(Item);
            }
        }

        public async Task SalvarGastoCompra(GastoCompra Item)
        {
            await database.InsertOrReplaceAsync(Item);
            if (!Item.Identificador.HasValue)
            {
                Item.Identificador = Item.Id * -1;
                await database.InsertOrReplaceAsync(Item);
            }
        }

        public async Task SalvarGastoDividido(GastoDividido Item)
        {
            await database.InsertOrReplaceAsync(Item);
            if (!Item.Identificador.HasValue)
            {
                Item.Identificador = Item.Id * -1;
                await database.InsertOrReplaceAsync(Item);
            }
        }

        public async Task SalvarGastoHotel(GastoHotel Item)
        {
            await database.InsertOrReplaceAsync(Item);
            if (!Item.Identificador.HasValue)
            {
                Item.Identificador = Item.Id * -1;
                await database.InsertOrReplaceAsync(Item);
            }
        }

        public async Task SalvarGastoRefeicao(GastoRefeicao Item)
        {
            await database.InsertOrReplaceAsync(Item);
            if (!Item.Identificador.HasValue)
            {
                Item.Identificador = Item.Id * -1;
                await database.InsertOrReplaceAsync(Item);
            }
        }

        public async Task SalvarGastoViagemAerea(GastoViagemAerea Item)
        {
            await database.InsertOrReplaceAsync(Item);
            if (!Item.Identificador.HasValue)
            {
                Item.Identificador = Item.Id * -1;
                await database.InsertOrReplaceAsync(Item);
            }
        }

        public async Task SalvarReabastecimentoGasto(ReabastecimentoGasto Item)
        {
            await database.InsertOrReplaceAsync(Item);
            if (!Item.Identificador.HasValue)
            {
                Item.Identificador = Item.Id * -1;
                await database.InsertOrReplaceAsync(Item);
            }
        }
        public async Task SalvarGastoAtracao(GastoAtracao Item)
        {
            await database.InsertOrReplaceAsync(Item);
            if (!Item.Identificador.HasValue)
            {
                Item.Identificador = Item.Id * -1;
                await database.InsertOrReplaceAsync(Item);
            }
        }

        public async Task SalvarCotacaoMoeda(CotacaoMoeda Item)
        {
            await database.InsertOrReplaceAsync(Item);
            if (!Item.Identificador.HasValue)
            {
                Item.Identificador = Item.Id * -1;
                await database.InsertOrReplaceAsync(Item);
            }
        }

        public async Task<List<CotacaoMoeda>> ListarCotacaoMoeda(CriterioBusca itemBusca)
        {
            var query = database.Table<CotacaoMoeda>();//.Where(d=>!d.DataExclusao.HasValue);
            if (itemBusca.DataInicioDe.HasValue)
                query = query.Where(d => d.DataCotacao >= itemBusca.DataInicioDe);
            if (itemBusca.DataInicioAte.HasValue)
                query = query.Where(d => d.DataCotacao <= itemBusca.DataInicioAte);
            if (itemBusca.Moeda.HasValue)
                query = query.Where(d => d.Moeda == itemBusca.Moeda);
            return (await query.ToListAsync()).Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<Sugestao> RetornarSugestao(int? Identificador)
        {
            return await database.Table<Sugestao>().Where(d => d.Identificador == Identificador).FirstOrDefaultAsync();
        }

        public async Task<List<ListaCompra>> ListarListaCompra_Requisicao(CriterioBusca itemBusca, int IdentificadorUsuario)
        {
            var query = database.Table<ListaCompra>();//.Where(d=>!d.DataExclusao.HasValue);
            query = query.Where(d => d.IdentificadorUsuarioPedido == IdentificadorUsuario);
            if (itemBusca.IdentificadorParticipante.HasValue)
                query = query.Where(d => d.IdentificadorUsuario == itemBusca.IdentificadorParticipante);
            if (itemBusca.Situacao.GetValueOrDefault(-1) >= 0)
            {
                 if (itemBusca.Situacao == 1)
                        query = query.Where(d => d.Status <= itemBusca.Situacao);
                    else
                        query = query.Where(d => d.Status == itemBusca.Situacao);
                
            }
            if (!string.IsNullOrEmpty(itemBusca.Tipo))
                query = query.Where(d => d.Marca.Contains(itemBusca.Tipo));
            if (!string.IsNullOrEmpty(itemBusca.Comentario))
                query = query.Where(d => d.Descricao.Contains(itemBusca.Comentario));
            return (await query.ToListAsync()).Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<List<ListaCompra>> ListarListaCompra(CriterioBusca itemBusca, int IdentificadorUsuario)
        {
            var query = database.Table<ListaCompra>();//.Where(d=>!d.DataExclusao.HasValue);
            query = query.Where(d => d.IdentificadorUsuario == IdentificadorUsuario);
             if (itemBusca.Situacao.GetValueOrDefault(-1) >= 0)
            {
                if (itemBusca.Situacao == 1)
                    query = query.Where(d => d.Status <= itemBusca.Situacao);
                else
                    query = query.Where(d => d.Status == itemBusca.Situacao);

            }
            if (!string.IsNullOrEmpty(itemBusca.Tipo))
                query = query.Where(d => d.Marca.Contains(itemBusca.Tipo));
            if (!string.IsNullOrEmpty(itemBusca.Nome))
                query = query.Where(d => d.Destinatario.Contains(itemBusca.Nome) || d.NomeUsuarioPedido.Contains(itemBusca.Nome));
            if (!string.IsNullOrEmpty(itemBusca.Comentario))
                query = query.Where(d => d.Descricao.Contains(itemBusca.Comentario));
            return (await query.ToListAsync()).Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<ListaCompra> RetornarListaCompra(int? Identificador)
        {
            return await database.Table<ListaCompra>().Where(d => d.Identificador == Identificador).FirstOrDefaultAsync();
        }

        public async Task SalvarListaCompra(ListaCompra itemListaCompra)
        {
            await database.InsertOrReplaceAsync(itemListaCompra);
            if (!itemListaCompra.Identificador.HasValue)
            {
                itemListaCompra.Identificador = itemListaCompra.Id * -1;
                await database.InsertOrReplaceAsync(itemListaCompra);
            }
        }

        public async Task ExcluirListaCompra(ListaCompra itemExcluir)
        {
            await database.DeleteAsync(itemExcluir);
        }

        public async Task ExcluirCotacaoMoeda(CotacaoMoeda item)
        {
            await database.DeleteAsync(item);
        }

        public async Task ExcluirGasto(Gasto item)
        {
            await database.DeleteAsync(item);
        }

        public async Task ExcluirAluguelGasto(AluguelGasto item)
        {
            await database.DeleteAsync(item);
        }

        public async Task ExcluirGastoAtracao(GastoAtracao item)
        {
            await database.DeleteAsync(item);
        }

        public async Task ExcluirGastoCompra(GastoCompra item)
        {
            await database.DeleteAsync(item);
        }

        public async Task ExcluirGastoHotel(GastoHotel item)
        {
            await database.DeleteAsync(item);
        }

        public async Task ExcluirGastoRefeicao(GastoRefeicao item)
        {
            await database.DeleteAsync(item);
        }

        public async Task ExcluirGastoDividido(GastoDividido item)
        {
            await database.DeleteAsync(item);
        }

        public async Task ExcluirGastoViagemAerea(GastoViagemAerea item)
        {
            await database.DeleteAsync(item);
        }

        public async Task ExcluirReabastecimentoGasto(ReabastecimentoGasto item)
        {
            await database.DeleteAsync(item);
        }

        public async Task<List<CalendarioPrevisto>> ListarCalendarioPrevisto(CriterioBusca itemBuscao)
        {
            var query = database.Table<CalendarioPrevisto>();//.Where(d=>!d.DataExclusao.HasValue);
          
            return (await query.ToListAsync()).Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task ExcluirCalendarioPrevisto(CalendarioPrevisto item)
        {
            await database.DeleteAsync(item);
        }
        public async Task<CalendarioPrevisto> CarregarCalendarioPrevisto(int Id)
        {
            return await database.Table<CalendarioPrevisto>().Where(d => d.Identificador == Id).FirstOrDefaultAsync();
        }


        public async Task<List<Atracao>> ListarAtracao(CriterioBusca itemBusca)
        {
            var query = database.Table<Atracao>();//.Where(d=>!d.DataExclusao.HasValue);
            if (itemBusca.DataInicioDe.HasValue)
                query = query.Where(d => d.Chegada >= itemBusca.DataInicioDe);
            if (itemBusca.DataInicioAte.HasValue)
                query = query.Where(d => d.Chegada <= itemBusca.DataInicioAte);

            if (itemBusca.DataFimDe.HasValue)
                query = query.Where(d => d.Partida >= itemBusca.DataFimDe);
            if (itemBusca.DataFimAte.HasValue)
                query = query.Where(d => d.Partida <= itemBusca.DataFimAte);
            if (!string.IsNullOrEmpty(itemBusca.Tipo))
                query = query.Where(d => d.Tipo.Contains(itemBusca.Tipo));
            if (!string.IsNullOrEmpty(itemBusca.Nome))
                query = query.Where(d => d.Nome.Contains(itemBusca.Nome));
            if (itemBusca.IdentificadorCidade.HasValue)
                query = query.Where(d => d.IdentificadorCidade == itemBusca.IdentificadorCidade);
            var ListaResultado = (await query.ToListAsync()).Where(d => !d.DataExclusao.HasValue).ToList();
            if (itemBusca.Situacao == 1)
                ListaResultado = ListaResultado.Where(d => d.Chegada.HasValue && !d.Partida.HasValue).ToList();
            else if (itemBusca.Situacao == 2)
                ListaResultado = ListaResultado.Where(d => d.Chegada.HasValue && d.Partida.HasValue).ToList();
            else if (itemBusca.Situacao == 3)
                ListaResultado = ListaResultado.Where(d => !d.Chegada.HasValue ).ToList();


            return ListaResultado;
        }

        public async Task<Atracao> RetornarAtracaoAberta()
        {
            var ListAtracao = await database.Table<Atracao>().ToListAsync();
            return ListAtracao.Where(d => !d.DataExclusao.HasValue && d.Chegada.HasValue && !d.Partida.HasValue).OrderByDescending(d => d.Chegada).FirstOrDefault();
        }

        public async Task ExcluirAtracao(Atracao item)
        {
            await database.DeleteAsync(item);
        }
        public async Task<Atracao> RetornarAtracao(int? Id)
        {
            return await database.Table<Atracao>().Where(d => d.Identificador == Id).FirstOrDefaultAsync();
        }


        public async Task<GastoAtracao> RetornarGastoAtracao(int? Id)
        {
            return await database.Table<GastoAtracao>().Where(d => d.Identificador == Id).FirstOrDefaultAsync();
        }

        public async Task SalvarAtracao(Atracao item)
        {
            await database.InsertOrReplaceAsync(item);
            if (!item.Identificador.HasValue)
            {
                item.Identificador = item.Id * -1;
                await database.InsertOrReplaceAsync(item);
            }
        }

        public async Task<List<AvaliacaoAtracao>> ListarAvaliacaoAtracao_IdentificadorAtracao(int? IdentificadorAtracao)
        {
            var query = database.Table<AvaliacaoAtracao>().Where(d => d.IdentificadorAtracao == IdentificadorAtracao);
            return (await query.ToListAsync());//.Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task<List<GastoAtracao>> ListarGastoAtracao_IdentificadorAtracao(int? IdentificadorAtracao)
        {
            var query = database.Table<GastoAtracao>().Where(d => d.IdentificadorAtracao == IdentificadorAtracao);
            return (await query.ToListAsync());//.Where(d => !d.DataExclusao.HasValue).ToList();
        }

        public async Task ExcluirAvaliacaoAtracao(AvaliacaoAtracao item)
        {
            await database.DeleteAsync(item);
        }

        public async Task SalvarAvaliacaoAtracao(AvaliacaoAtracao item)
        {
            await database.InsertOrReplaceAsync(item);
            if (!item.Identificador.HasValue)
            {
                item.Identificador = item.Id * -1;
                await database.InsertOrReplaceAsync(item);
            }
        }

        public async Task<List<Gasto>> ListarGasto(CriterioBusca itemBusca)
        {
            var query = database.Table<Gasto>();//.Where(d=>!d.DataExclusao.HasValue);
            if (itemBusca.DataInicioDe.HasValue)
                query = query.Where(d => d.Data >= itemBusca.DataInicioDe);
            if (itemBusca.DataInicioAte.HasValue)
                query = query.Where(d => d.Data <= itemBusca.DataInicioAte);
            if (itemBusca.IdentificadorParticipante.HasValue)
                query = query.Where(d => d.IdentificadorUsuario == itemBusca.IdentificadorParticipante);
            if (!string.IsNullOrEmpty(itemBusca.Nome))
                query = query.Where(d => d.Descricao.Contains(itemBusca.Nome));


            query = query.Where(d => !d.ApenasBaixa);
            query = query.OrderByDescending(d => d.Data);

            var Lista = (await query.ToListAsync()).Where(d => !d.DataExclusao.HasValue).ToList();
            List<int?> Identificadores = Lista.Select(d => d.Identificador).ToList();
            var ListaCompras = await database.Table<GastoCompra>().Where(d => Identificadores.Contains(d.IdentificadorGasto)).ToListAsync();
            var ListaReabastecimentos = await database.Table<ReabastecimentoGasto>().Where(d => Identificadores.Contains(d.IdentificadorGasto)).ToListAsync();
            Lista = Lista.Where(d => !ListaCompras.Where(e => e.IdentificadorGasto == d.Identificador).Any()).ToList();
            Lista = Lista.Where(d => !ListaReabastecimentos.Where(e => e.IdentificadorGasto == d.Identificador).Any()).ToList();
            return Lista;
        }

        public async Task Excluir_Gasto_Filhos(int IdentificadorGasto)
        {
            await database.ExecuteAsync("Delete from AluguelGasto Where IdentificadorGasto = ?", IdentificadorGasto);          
            await database.ExecuteAsync("Delete from GastoAtracao Where IdentificadorGasto = ?", IdentificadorGasto);
            await database.ExecuteAsync("Delete from GastoCompra Where IdentificadorGasto = ?", IdentificadorGasto);
            await database.ExecuteAsync("Delete from GastoDividido Where IdentificadorGasto = ?", IdentificadorGasto);
            await database.ExecuteAsync("Delete from GastoHotel Where IdentificadorGasto = ?", IdentificadorGasto);
            await database.ExecuteAsync("Delete from GastoRefeicao Where IdentificadorGasto = ?", IdentificadorGasto);
            await database.ExecuteAsync("Delete from GastoViagemAerea Where IdentificadorGasto = ?", IdentificadorGasto);           
            await database.ExecuteAsync("Delete from ReabastecimentoGasto  Where IdentificadorGasto = ?", IdentificadorGasto);


        }

    }

}
