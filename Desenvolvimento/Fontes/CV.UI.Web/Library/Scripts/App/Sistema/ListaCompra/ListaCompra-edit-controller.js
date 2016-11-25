(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ListaCompraEditCtrl',['$uibModalInstance', 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Usuario','Viagem','ListaCompra','Dominio','EscopoAtualizacao', 'ItemListaCompra', ListaCompraEditCtrl]);

	function ListaCompraEditCtrl($uibModalInstance, Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Usuario, Viagem, ListaCompra, Dominio, EscopoAtualizacao, ItemListaCompra) {
	    var vm = this;
	    vm.itemOriginal = jQuery.extend({}, ItemListaCompra);
	    vm.itemListaCompra = ItemListaCompra;
	    vm.EscopoAtualizacao = EscopoAtualizacao;
	    vm.messages = [];
	    vm.loggedUser = Auth.currentUser;
	    vm.CamposInvalidos = {};
	    vm.ListaItensCompra = [];

	    vm.itemUsuario = {};
	    vm.ListaUsuario = [];
	    vm.itemMoeda = {};
	    vm.ListaMoeda = [];

	    vm.CadastradoAmigo = false;
	    vm.loading = false;
	    vm.load = function () {
	        vm.loading = true;
	        vm.CadastradoAmigo = vm.itemListaCompra.IdentificadorUsuarioPedido != null;
	        if (vm.itemListaCompra.IdentificadorUsuarioPedido) {
	            vm.itemUsuario = { Identificador: vm.itemListaCompra.IdentificadorUsuarioPedido };
	        }
	        if (vm.itemListaCompra.Moeda)
	            vm.itemMoeda = { Codigo: vm.itemListaCompra.Moeda };

	        Dominio.CarregaMoedas(function (data) {
	            vm.ListaMoeda = data;
	        });

	        Usuario.listaAmigos(function (data) {
	            vm.ListaUsuario = data;
	            vm.loading = false;
	        },
           function (err) {
               vm.loading = false;
           });

	    };

	    
	    vm.salvar = function () {
	        vm.messages = [];
	        vm.loading = true;
	        vm.CamposInvalidos = {};
	        if (vm.itemMoeda && vm.itemMoeda.Codigo)
	            vm.itemListaCompra.Moeda = vm.itemMoeda.Codigo;
	        else
	            vm.itemListaCompra.Moeda = null;

	        if (vm.CadastradoAmigo) {
	            if (vm.itemUsuario && vm.itemUsuario.Identificador)
	                vm.itemListaCompra.IdentificadorUsuarioPedido = vm.itemUsuario.Identificador;
	            else
	                vm.itemListaCompra.IdentificadorUsuarioPedido = null;
	            vm.itemListaCompra.Destinatario = null;
	        }
	        else
	            vm.itemListaCompra.IdentificadorUsuarioPedido = null;

	        ListaCompra.save(vm.itemListaCompra, function (data) {
	            vm.loading = false;
	            if (data.Sucesso) {
	                vm.itemListaCompra.Identificador = data.IdentificadorRegistro;
	                vm.EscopoAtualizacao.AtualizarItemCompra(vm.itemOriginal, vm.itemListaCompra);
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
