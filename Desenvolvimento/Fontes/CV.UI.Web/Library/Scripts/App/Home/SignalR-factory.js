(function () {
    'use strict';

    /**
     * @ngdoc service
     * @name account.factory:Auth
     *
     * @description
     *
     */
    angular
      .module('home')
      .factory('SignalR', ['$location', '$rootScope', '$http', '$cookies', '$q', '$translate', '$state', SignalR]);

    function SignalR($location, $rootScope, $http, $cookies, $q, $translate, $state) {
        var $hub = $.connection.Viagem;
        var connection = null;
        var signalR = {
            startHub: function () {
                console.log("started");
                connection = $.connection.hub.start();
            },
            ConectarUsuario: function (IdentificadorUsuario) {
                connection.done(function () {
                    $hub.server.conectarUsuario(IdentificadorUsuario);
                });
            },
            DesconectarUsuario: function (IdentificadorUsuario) {
                connection.done(function () {
                    $hub.server.desconectarUsuario(IdentificadorUsuario);
                });
            },
            RequisitarAmizade: function (IdentificadorUsuario, IdentificadorRequisicao) {
                connection.done(function () {
                    $hub.server.requisitarAmizade(IdentificadorUsuario, IdentificadorRequisicao);
                });
            },
            ConectarViagem: function (IdentificadorUsuario, Edicao) {
                connection.done(function () {
                    $hub.server.conectarViagem(IdentificadorUsuario, Edicao);
                });
            },
            SugerirVisitaViagem: function (itemSugestao) {
                connection.done(function () {
                    $hub.server.sugerirVisitaViagem(itemSugestao);
                });
            },
            ViagemAtualizada: function (IdentificadorViagem, TipoAtualizacao) {
                connection.done(function () {
                    $hub.server.viagemAtualizada(IdentificadorViagem, TipoAtualizacao);
                });
            },

            EnviarAlertaRequisicao: function (callback) {
                $hub.client.enviarAlertaRequisicao = callback;
            },
            EnviarAlertaSugestao: function (callback) {
                $hub.client.enviarAlertaSugestao = callback;
            },
            AvisarAlertaAtualizacao: function (callback) {
                $hub.client.avisarAlertaAtualizacao = callback;
            },
           
        };

        return signalR;
    }
}());