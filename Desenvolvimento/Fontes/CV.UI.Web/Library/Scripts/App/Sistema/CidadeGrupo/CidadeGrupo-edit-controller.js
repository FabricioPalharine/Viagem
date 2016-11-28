(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('CidadeGrupoEditCtrl', ['$uibModalInstance', 'Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade', 'Viagem', 'CidadeGrupo', 'EscopoAtualizacao', 'ItemCidadeGrupo', CidadeGrupoEditCtrl]);

    function CidadeGrupoEditCtrl($uibModalInstance, Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Cidade, Viagem, CidadeGrupo, EscopoAtualizacao, ItemCidadeGrupo) {
        var vm = this;
        vm.itemOriginal = ItemCidadeGrupo;
        vm.itemCidadeGrupo = jQuery.extend({}, ItemCidadeGrupo);
        vm.messages = [];
        vm.loggedUser = Auth.currentUser;
        vm.CamposInvalidos = {};
        vm.EscopoAtualizacao = EscopoAtualizacao;

        vm.itemCidade = {};
        vm.ListaCidade = [];
        vm.ListaCidadeFilhas = [];

        vm.load = function () {
            vm.loading = true;
            if (vm.itemCidadeGrupo.IdentificadorCidade) {
                vm.itemCidade.Identificador = vm.itemCidadeGrupo.IdentificadorCidade;
            }
            Cidade.ListarCidadeNaoAssociadasFilho(function (cidadesPai) {
                vm.ListaCidade = cidadesPai;
                Cidade.ListarCidadeNaoAssociadasPai({ id: (vm.itemCidade.Identificador)?vm.itemCidade.Identificador:-1 }, function (cidadesFilhas) {
                    vm.ListaCidadeFilhas = cidadesFilhas;
                    angular.forEach(vm.ListaCidadeFilhas, function (item) {
                        item.Selecionado = vm.itemCidadeGrupo.CidadesFilhas && vm.itemCidadeGrupo.CidadesFilhas.indexOf(item.Identificador) >= 0;
                    });
                    vm.loading = false;
                });
            });
        };

        vm.save = function (form) {
            vm.messages = [];
            vm.submitted = true;
            vm.CamposInvalidos = {};

            if (vm.itemCidade && vm.itemCidade.Identificador)
                vm.itemCidadeGrupo.IdentificadorCidade = vm.itemCidade.Identificador;
            else
                vm.itemCidadeGrupo.IdentificadorCidade = null;
            vm.itemCidadeGrupo.CidadesFilhas = [];

            angular.forEach(vm.ListaCidadeFilhas, function (item) {
                if (item.Selecionado && item.Identificador != vm.itemCidadeGrupo.IdentificadorCidade)
                    vm.itemCidadeGrupo.CidadesFilhas.push(item.Identificador);
            });

            CidadeGrupo.save(vm.itemCidadeGrupo, function (data) {
                vm.loading = false;
                if (data.Sucesso) {
                    Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
                    vm.EscopoAtualizacao.AtualizarItemSalvo(data.ItemRegistro, vm.itemOriginal);
                    vm.close();
                } else {
                    vm.messages = data.Mensagens;
                    vm.verificaCampoInvalido();
                }
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
            });
        };
        //
        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };
        //
        vm.verificaCampoInvalido = function () {
            vm.CamposInvalidos = {

            };
            //  var _retorno = false;
            $(vm.messages).each(function (i, item) {
                vm.CamposInvalidos[item.Campo] = true;
            });
        };


        vm.close = function () {

            if ($uibModalInstance)
                $uibModalInstance.close();

        };
    }
}());
