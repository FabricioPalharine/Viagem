(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('CompraEditCtrl', ['$uibModalInstance', 'Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams',  'Loja', 'EscopoAtualizacao','ItemGastoCompra', 'Viagem', '$uibModal','Usuario', CompraEditCtrl]);

    function CompraEditCtrl($uibModalInstance, Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Loja, EscopoAtualizacao, ItemGastoCompra, Viagem, $uibModal, Usuario) {
        var vm = this;
        vm.itemCompra = jQuery.extend({}, ItemGastoCompra);
        vm.itemOriginal = ItemGastoCompra;
        vm.EscopoAtualizacao = EscopoAtualizacao;
        vm.messages = [];
        vm.loggedUser = Auth.currentUser;
        vm.CamposInvalidos = {};

        vm.loading = false;
        vm.load = function () {

        };

        vm.salvar = function () {
            vm.messages = [];
            vm.loading = true;
            vm.CamposInvalidos = {};            
            vm.SalvarCusto();                        
            Loja.saveCompra(vm.itemCompra, function (data) {
                vm.loading = false;
                if (data.Sucesso) {
                    vm.itemCompra = data.ItemRegistro;
                    vm.EscopoAtualizacao.AtualizarCompra(vm.itemOriginal, vm.itemCompra);
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


        vm.RemoverItemCompra = function (itemCompra) {
            vm.EscopoAtualizacao.ChamarExclusao(itemCompra,  function () {
                itemCompra.DataExclusao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                Loja.SalvarItemCompra(itemCompra);
            }, function () { });
        };

        vm.AtualizarItemCompra = function (itemOriginal, itemCompra) {
            if (itemOriginal.Identificador) {
                var Posicao = vm.itemCompra.ItensComprados.indexOf(itemOriginal);
                vm.itemCompra.ItensComprados.splice(Posicao, 1, itemCompra);
            }
            else {
                vm.itemCompra.ItensComprados.push(itemCompra);
            }
        };

        vm.EditarCompra = function (itemCompra) {
            $uibModal.open({
                templateUrl: 'Sistema/ItemCompraEdicao',
                controller: 'ItemCompraEditCtrl',
                controllerAs: 'itemItemCompraEdit',
                resolve: {
                    ItemGastoCompra: function () { return vm.itemCompra; },
                    EscopoAtualizacao: vm,
                    ItemItemCompra: function() {return itemCompra}
                }
            });
        };

        vm.AdicionarItemCompra = function () {
            var itemCompra = { IdentificadorGastoCompra: vm.itemCompra.Identificador, Fotos: [], Reembolsavel: false };
            vm.EditarCompra(itemCompra);
        };

 
    }
}());
