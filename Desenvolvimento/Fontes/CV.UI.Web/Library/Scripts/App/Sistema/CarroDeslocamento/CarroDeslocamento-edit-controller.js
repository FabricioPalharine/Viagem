(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('CarroDeslocamentoEditCtrl', ['$uibModalInstance', 'Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Viagem', 'Carro', 'itemCarroDeslocamento', 'EscopoAtualizacao', CarroDeslocamentoEditCtrl]);

    function CarroDeslocamentoEditCtrl($uibModalInstance, Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Viagem, Carro, itemCarroDeslocamento, EscopoAtualizacao) {
        var vm = this;
        vm.itemOriginal = itemCarroDeslocamento;
        vm.itemCarroDeslocamento = jQuery.extend({}, itemCarroDeslocamento);
        vm.EscopoAtualizacao = EscopoAtualizacao;
        vm.ListaParticipantes = [];
        vm.messages = [];
        vm.CamposInvalidos = {};
        vm.FimDeslocamento = false;


        vm.load = function () {
            vm.loading = true;
            vm.enableScroll = true;

            vm.FimDeslocamento = vm.itemCarroDeslocamento.ItemCarroEventoChegada.Data != null;
            Viagem.CarregarParticipantes(function (lista) {
                vm.ListaParticipantes = lista;
                angular.forEach(vm.ListaParticipantes, function (item) {
                    item.Selecionado = vm.itemCarroDeslocamento.Usuarios && $.grep(vm.itemCarroDeslocamento.Usuarios, function (e) {
                        return e.IdentificadorUsuario == item.Identificador;
                    }).length > 0;
                });
            });
           

        };

        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };

        vm.verificaCampoInvalido = function () {
            vm.CamposInvalidos = {

            };
            //  var _retorno = false;
            $(vm.messages).each(function (i, item) {
                vm.CamposInvalidos[item.Campo] = true;
            });
        };

        vm.AjustarHoraChegada = function () {
            if (vm.FimDeslocamento)
            {
                vm.itemCarroDeslocamento.ItemCarroEventoChegada.Data = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                vm.itemCarroDeslocamento.ItemCarroEventoChegada.strHora = moment(new Date()).format("HH:mm:ss");
            }
            else
            {
                vm.itemCarroDeslocamento.ItemCarroEventoChegada.Data = vm.itemCarroDeslocamento.ItemCarroEventoChegada.strHora = null;
            }
        };

        vm.close = function () {
            if ($uibModalInstance)
                $uibModalInstance.close();
        };

        vm.SelecionarPosicao = function (item) {
            vm.EscopoAtualizacao.SelecionarPosicao(item);
        };
    }
}());