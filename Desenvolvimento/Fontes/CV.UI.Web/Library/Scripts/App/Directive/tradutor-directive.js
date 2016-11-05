(function () {
    'use strict';

    angular
    .module('CV')
    .config(['$translateProvider', function ($translateProvider) {
        $translateProvider
        .translations('pt-br', {
            ErroRequisicao: 'Ocorreu um erro ao recuperar os itens.',
            ErroSalvar: 'Ocorreu um erro ao enviar o item ao servidor.',
            ErroExcluir: 'Ocorreu um erro ao excluir o item.',
            Sucesso: "Sucesso",
            EdicaoAberta: "Existe outro registro em modo de edi��o",
            Usuario_EMail: 'E-mail',
            Usuario_Nome: 'Nome',
            Usuario_Token: 'Token',
            Usuario_RefreshToken: 'RefreshToken',
            Usuario_DataToken: 'Data',
            Usuario_Lifetime: 'Lifetime',
            Usuario_Codigo: 'Codigo',
            Reabastecimento_IdentificadorCarro: 'Carro',
            Reabastecimento_IdentificadorCidade: 'Cidade',
            Reabastecimento_Latitude: 'Latitude',
            Reabastecimento_Longitude: 'Longitude',
            Reabastecimento_Litro: 'Litro',
            Reabastecimento_QuantidadeReabastecida: 'Quantidade Reabastecida',
            Amigo_IdentificadorUsuario: 'Usu�rio',
            Amigo_IdentificadorAmigo: 'Amigo',
            Amigo_EMail: 'E-Mail',
            Amigo_Nome: 'Nome',
            Amigo_Seguidor: 'Seguidor',
            Amigo_Seguido: 'Seguido',
            Amigo_MensagemExclusao: 'Tem certeza que deseja excluir o item selecionado?',
            Amigo_MensagemAdicionarSeguidor: 'Ao marcar essa pessoa como seguidora ela poder� visualizar suas viagens. Deseja marc�-la?',
            Amigo_MensagemRemoverSeguidor: 'Ao desmarcar essa pessoa como seguidora ela deixar� de visualizar suas viagens. Deseja desmarc�-la?',
            Amigo_MensagemAdicionarSeguido: 'Ao marcar essa pessoa ser� enviada uma solicita��o para voc� visualizar suas viagens. Deseja continuar?',
            Amigo_MensagemRemoverSeguido: 'Ao desmarcar essa pessoa voc� deixar� de acompanhar suas viagens. Deseja remover?',
            AporteDinheiro_IdentificadorViagem: 'Viagem',
            AporteDinheiro_IdentificadorUsuario: 'Usu�rio',
            AporteDinheiro_Valor: 'Valor',
            AporteDinheiro_MoedaEstragengeira: 'Moeda Estrangeira',
            AporteDinheiro_Moeda: 'Moeda',
            AporteDinheiro_DataAporte: 'Data',
            AporteDinheiro_Cotacao: 'Cota��o',
            Atracao_IdentificadorAtracaoPai: 'Atra��o Pai',
            Atracao_IdentificadorViagem: 'Viagem',
            Atracao_IdentificadorCidade: 'Cidade',
            Atracao_Nome: 'Nome',
            Atracao_CodigoPlace: 'C�digo Google Place',
            Atracao_Latitude: 'Latitude',
            Atracao_Longitude: 'Longitude',
            Atracao_Chegada: 'Chegada',
            Atracao_Partida: 'Partida',
            Atracao_Tipo: 'Tipo',
            CalendarioPrevisto_IdentificadorViagem: 'Viagem',
            CalendarioPrevisto_Data: 'Data',
            CalendarioPrevisto_Inicio: 'In�cio Previsto',
            CalendarioPrevisto_Fim: 'Fim Previsto',
            CalendarioPrevisto_Nome: 'Nome',
            CalendarioPrevisto_Latitude: 'Latitude',
            CalendarioPrevisto_Longitude: 'Longitude',
            CalendarioPrevisto_CodigoPlace: 'Codigo Google Place',
            CalendarioPrevisto_Tipo: 'Tipo',
            CalendarioPrevisto_Prioridade: 'Prioridade',
            Carro_IdentificadorViagem: 'Viagem',
            Carro_Locadora: 'Locadora',
            Carro_Modelo: 'Modelo',
            Carro_KM: 'KM',
            Carro_Alugado: 'Alugado',
            Cidade_IdentificadorPais: 'Pa�s',
            Cidade_Nome: 'Nome',
            Cidade_Estado: 'Estado',
            CidadeGrupo_IdentificadorViagem: 'Viagem',
            CidadeGrupo_IdentificadorCidadeFilha: 'Cidade Filha',
            CidadeGrupo_IdentificadorCidadePai: 'Cidade Pai',
            Comentario_IdentificadorViagem: 'Viagem',
            Comentario_IdentificadorCidade: 'Cidade',
            Comentario_Latitude: 'Latitude',
            Comentario_Longitude: 'Longitude',
            Comentario_Texto: 'Texto',
            CotacaoMoeda_Moeda: 'Moeda',
            CotacaoMoeda_DataCotacao: 'Data Cota��o',
            CotacaoMoeda_ValorCotacao: 'Cota��o',
            CotacaoMoeda_IdentificadorViagem: 'Viagem',
            Foto_IdentificadorViagem: 'Viagem',
            Foto_IdentificadorUsuario: 'Usu�rio',
            Foto_IdentificadorCidade: 'Cidade',
            Foto_Comentario: 'Comentario',
            Foto_Latitude: 'Latitude',
            Foto_Longitude: 'Longitude',
            Foto_Data: 'Data',
            Foto_LinkThumbnail: 'Link Thumbnail',
            Foto_LinkFoto: 'Link Foto',
            Foto_CodigoFoto: 'C�digo',
            Foto_Video: 'Video',
            Foto_TipoArquivo: 'Tipo Arquivo',
            Gasto_IdentificadorViagem: 'Viagem',
            Gasto_IdentificadorUsuario: 'Usu�rio',
            Gasto_Descricao: 'Descri��o',
            Gasto_Data: 'Data',
            Gasto_Valor: 'Valor',
            Gasto_MoedaEstrageira: 'Moeda Estrangeira',
            Gasto_Especie: 'Dinheiro',
            Gasto_Moeda: 'Moeda',
            Gasto_DataPagamento: 'DataPagamento',
            Gasto_Dividido: 'Dividido',
            Gasto_ApenasBaixa: 'Baixa Dinheiro',
            Hotel_IdentificadorViagem: 'Viagem',
            Hotel_IdentificadorCidade: 'Cidade',
            Hotel_Nome: 'Nome',
            Hotel_CodigoPlace: 'C�digo Google Place',
            Hotel_DataEntrada: 'Data de Entrada',
            Hotel_DataSaidia: 'Data Sa�da',
            Hotel_Longitude: 'Longitude',
            Hotel_Latitude: 'Latitude',
            Hotel_EntradaPrevista: 'Entrada Prevista',
            Hotel_SaidaPrevista: 'Sa�da Prevista',
            ListaCompra_IdentificadorViagem: 'Viagem',
            ListaCompra_IdentificadorUsuario: 'Usu�rio',
            ListaCompra_IdentificadorUsuarioPedido: 'Usu�rio Pedido',
            ListaCompra_Descricao: 'Descri��o',
            ListaCompra_Marca: 'Marca',
            ListaCompra_ValorMaximo: 'Valor M�ximo',
            ListaCompra_Moeda: 'Moeda',
            ListaCompra_Reembolsavel: 'Reembolsavel',
            ListaCompra_Comprado: 'Comprado',
            ListaCompra_Destinatario: 'Destinat�rio',
            Pais_Sigla: 'Sigla',
            Pais_Nome: 'Nome',
            Posicao_IdentificadorViagem: 'Viagem',
            Posicao_IdentificadorUsuario: 'Usu�rio',
            Posicao_Latitude: 'Latitude',
            Posicao_Longitude: 'Longitude',
            Posicao_DataGMT: 'DataGMT',
            Posicao_Velocidade: 'Velocidade',
            Posicao_DataLocal: 'Data Local',
            Loja_IdentificadorViagem: 'Viagem',
            Loja_Nome: 'Nome',
            Loja_Latitude: 'Latitude',
            Loja_Longitude: 'Longitude',
            Loja_CodigoPlace: 'C�digo Place',
            Loja_Data: 'Data',
            Loja_IdentificadorAtracao: 'Atra��o',
            Refeicao_IdentificadorViagem: 'Viagem',
            Refeicao_IdentificadorCidade: 'Cidade',
            Refeicao_Nome: 'Restaurante',
            Refeicao_CodigoPlace: 'C�digo Place',
            Refeicao_Data: 'Data',
            Refeicao_Latitude: 'Latitude',
            Refeicao_Longitude: 'Longitude',
            Refeicao_Tipo: 'Tipo',
            Refeicao_IdentificadorAtracao: 'Atra��o',
            RequisicaoAmizade_IdentificadorUsuario: 'Usu�rio',
            RequisicaoAmizade_IdentificadorUsuarioRequisitado: 'Usuario Requisitado',
            RequisicaoAmizade_EMail: 'EMail',
            RequisicaoAmizade_Status: 'Status',
            Sugestao_Local: 'Local',
            Sugestao_Latitude: 'Latitude',
            Sugestao_Longitude: 'Longitude',
            Sugestao_Comentario: 'Coment�rio',
            Sugestao_IdentificadorViagem: 'Viagem',
            Sugestao_IdentificadorUsuario: 'Usu�rio',
            Sugestao_IdentificadorCidade: 'Cidade',
            Sugestao_Lida: 'Lida',
            Sugestao_Tipo: 'Tipo',
            Viagem_IdentificadorUsuario: 'Usu�rio',
            Viagem_Nome: 'Nome',
            Viagem_DataInicio: 'Data In�cio',
            Viagem_DataFim: 'Fim',
            Viagem_Aberto: 'Aberto',
            Viagem_UnidadeMetrica: 'Unidade M�trica',
            Viagem_QuantidadeParticipantes: 'Quantidade Participantes',
            Viagem_PublicaGasto: 'Gastos P�blicos',
            Viagem_PercentualIOF: 'Percentual IOF',
            Viagem_Moeda: 'Moeda',
            ViagemAerea_IdentificadorViagem: 'Viagem',
            ViagemAerea_CompanhiaAerea: 'CompanhiaAerea',
            ViagemAerea_DataPrevista: 'DataPrevista',
            ViagemAerea_DataInicio: 'DataInicio',
            ViagemAerea_DataFim: 'DataFim',

        });
        $translateProvider.preferredLanguage('pt-br');
    }]);



}());