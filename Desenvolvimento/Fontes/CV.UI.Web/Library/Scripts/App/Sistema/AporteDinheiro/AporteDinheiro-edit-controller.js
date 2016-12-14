(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('AporteDinheiroEditCtrl', ['Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Usuario', 'Viagem', 'AporteDinheiro', 'Dominio', '$uibModalInstance', 'EscopoAtualizacao', 'ItemAporteDinheiro', AporteDinheiroEditCtrl]);

    function AporteDinheiroEditCtrl(Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Usuario, Viagem, AporteDinheiro, Dominio, $uibModalInstance, EscopoAtualizacao, ItemAporteDinheiro) {
        var vm = this;
        vm.itemAporteDinheiro = jQuery.extend({}, ItemAporteDinheiro);
        vm.itemOriginal = ItemAporteDinheiro;
        vm.loading = false;
        vm.messages = [];
        vm.loggedUser = Auth.currentUser;
        vm.CamposInvalidos = {};
        vm.ListaMoeda = [];
        vm.itemMoeda = {};
        vm.EscopoAtualizacao = EscopoAtualizacao;
        vm.itemGasto = null;
        vm.itemMoedaGasto = {};
        vm.BaixarMoeda = false;



        vm.load = function () {
            vm.loading = true;
            vm.itemMoeda = { Codigo: vm.itemAporteDinheiro.Moeda };
            if (vm.itemAporteDinheiro.ItemGasto)
            {
                vm.itemGasto = vm.itemAporteDinheiro.ItemGasto
                vm.itemMoedaGasto = { Codigo: vm.itemGasto.Moeda };
                vm.BaixarMoeda = true;
            }
            Dominio.CarregaMoedas(function (data) {
                vm.ListaMoeda = data;
                vm.loading = false;
            });
        };


        vm.save = function () {
            vm.messages = [];
            vm.submitted = true;
            vm.CamposInvalidos = {};

            if (vm.itemAporteDinheiro.DataAporte) {
              
                vm.itemAporteDinheiro.DataAporte = moment(vm.itemAporteDinheiro.DataAporte).format("YYYY-MM-DDTHH:mm:ss");
              
            }

            vm.itemAporteDinheiro.ItemGasto = vm.itemGasto;

            if (vm.itemGasto)
            {
                if (vm.itemMoedaGasto && vm.itemMoedaGasto.Codigo)
                    vm.itemAporteDinheiro.ItemGasto.Moeda = vm.itemMoedaGasto.Codigo;
                else
                    vm.itemAporteDinheiro.ItemGasto.Moeda = null;
            }

            if (vm.itemMoeda && vm.itemMoeda.Codigo)
                vm.itemAporteDinheiro.Moeda = vm.itemMoeda.Codigo;
            else
                vm.itemAporteDinheiro.Moeda = null;

            AporteDinheiro.save(vm.itemAporteDinheiro, function (data) {
                vm.loading = false;
                if (data.Sucesso) {
                    Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
                    vm.close();
                    vm.EscopoAtualizacao.AtualizarItemSalvo(data.ItemRegistro, vm.itemOriginal);
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

        vm.trocarBaixa = function () {
            if (vm.BaixarMoeda) {
                vm.itemGasto = { Especie: true, ApenasBaixa : true, Dividido: false, Descricao: 'Baixa Moeda'};
            }
            else
                vm.itemGasto = null;
        };
    }
}());
