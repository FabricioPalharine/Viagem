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
      .factory('SignalR', ['$location', '$rootScope', '$http', '$cookies', '$q', '$translate', '$state','$timeout', SignalR]);

    function SignalR($location, $rootScope, $http, $cookies, $q, $translate, $state, $timeout) {
        var $hub = $.connection.Viagem;
        var connection = null;

        

        var signalR = {
            startHub: function () {
                console.log("started");
                connection = $.connection.hub.start();
                $.connection.hub.disconnected(function () {
                    $timeout(function () {
                        connection =  $.connection.hub.start().done(function() {
                            $rootScope.$emit('SignalRConnected');
                        });
                    }, 5000); // Restart connection after 5 seconds.
                });
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
            ViagemAtualizada: function (IdentificadorViagem, TipoAtualizacao, Identificador, Inclusao ) {
                connection.done(function () {
                    $hub.server.viagemAtualizada(IdentificadorViagem, TipoAtualizacao, Identificador, Inclusao);
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