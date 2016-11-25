(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('ItemCompraEditCtrl', ['$uibModalInstance', 'Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Loja', 'EscopoAtualizacao', 'ItemItemCompra','ItemGastoCompra', 'Viagem', '$uibModal', 'Usuario','ListaCompra', ItemCompraEditCtrl]);

    function ItemCompraEditCtrl($uibModalInstance, Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Loja, EscopoAtualizacao, ItemItemCompra,ItemGastoCompra, Viagem, $uibModal, Usuario, ListaCompra) {
        var vm = this;
        vm.itemItemCompra = jQuery.extend({}, ItemItemCompra);
        vm.itemOriginal = ItemItemCompra;
        vm.ItemGastoCompra = ItemGastoCompra;
        vm.EscopoAtualizacao = EscopoAtualizacao;
        vm.messages = [];
        vm.loggedUser = Auth.currentUser;
        vm.CamposInvalidos = {};
        vm.ListaItensCompra = [];
        vm.itemListaCompra = {};
        vm.itemUsuario = {};
        vm.ListaUsuario = [];

        vm.CadastradoAmigo = false;
        vm.loading = false;
        vm.load = function () {
            vm.loading = true;
            vm.CadastradoAmigo = vm.itemItemCompra.IdentificadorUsuario != null;
            if (vm.itemItemCompra.IdentificadorUsuario)
            {
                vm.itemUsuario = { Identificador: vm.itemItemCompra.IdentificadorUsuario };
            }
            if (vm.itemItemCompra.IdentificadorListaCompra)
                vm.itemListaCompra = { Identificador: vm.itemItemCompra.IdentificadorListaCompra };

            ListaCompra.CarregarListaPedidos({ json: JSON.stringify({ IdentificadorParticipante: vm.ItemGastoCompra.ItemGasto.IdentificadorUsuario }) }, function (data) {
                vm.ListaItensCompra = data;
            }, function (err) {
              
            });

            Usuario.listaAmigos(function (data) {
                vm.ListaUsuario = data;
                vm.loading = false;
            },
           function (err) {
               vm.loading = false;
           });

        };

        vm.TrocarItemCompra = function () {
            if (vm.itemListaCompra.Identificador)
            {
                vm.itemItemCompra.Marca = vm.itemListaCompra.Marca;
                vm.itemItemCompra.Descricao = vm.itemListaCompra.Descricao;
                vm.itemItemCompra.Destinatario = vm.itemListaCompra.Destinatario;
                if (vm.itemListaCompra.IdentificadorUsuario != vm.ItemGastoCompra.ItemGasto.IdentificadorUsuario)
                    vm.itemItemCompra.IdentificadorUsuario = vm.itemListaCompra.IdentificadorUsuario;
                else if (vm.itemListaCompra.IdentificadorUsuarioPedido)
                    vm.itemItemCompra.IdentificadorUsuario = vm.itemListaCompra.IdentificadorUsuarioPedido;
                else
                    vm.itemItemCompra.IdentificadorUsuario = null;
                if (vm.itemItemCompra.IdentificadorUsuario)
                    vm.itemUsuario = { Identificador: vm.itemItemCompra.IdentificadorUsuario };
                else
                    vm.itemUsuario = {};
                vm.itemItemCompra.Reembolsavel = vm.itemListaCompra.Reembolsavel;
            }
        };

        vm.salvar = function () {
            vm.messages = [];
            vm.loading = true;
            vm.CamposInvalidos = {};            
                             
            if (vm.CadastradoAmigo) {
                if (vm.itemUsuario && vm.itemUsuario.Identificador)
                    vm.itemItemCompra.IdentificadorUsuario = vm.itemUsuario.Identificador;
                else
                    vm.itemItemCompra.IdentificadorUsuario = null;
                vm.itemItemCompra.Destinatario = null;
            }
            else
                vm.itemItemCompra.IdentificadorUsuario = null;

            if (vm.itemListaCompra && vm.itemListaCompra.Identificador)
                vm.itemItemCompra.IdentificadorListaCompra = vm.itemListaCompra.Identificador;
            else
                vm.itemItemCompra.IdentificadorListaCompra = null;

            Loja.SalvarItemCompra(vm.itemItemCompra, function (data) {
                vm.loading = false;
                if (data.Sucesso) {
                    vm.itemItemCompra.Identificador = data.IdentificadorRegistro;
                    vm.EscopoAtualizacao.AtualizarItemCompra(vm.itemOriginal, vm.itemItemCompra);
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
