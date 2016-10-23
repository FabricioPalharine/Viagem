(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ListaCompraEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Usuario','Viagem','ListaCompra', ListaCompraEditCtrl]);

	function ListaCompraEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Usuario,Viagem,ListaCompra) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemListaCompra = { };
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.itemViagem = null;
		vm.ListaViagem = [];
		vm.itemUsuario = null;
		vm.ListaUsuario = [];
		vm.itemUsuario = null;
		vm.ListaUsuario = [];

		vm.load = function () {
			vm.loading = true;
			var param = $stateParams;
			vm.filtroConsulta = param.filtro;
			Viagem.list({ json: JSON.stringify({}) }, function (data) {
				vm.ListaViagem=data.Lista;
			},
			function (err) {
				Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
			});
			Usuario.list({ json: JSON.stringify({}) }, function (data) {
				vm.ListaUsuario=data.Lista;
			},
			function (err) {
				Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
			});
			Usuario.list({ json: JSON.stringify({}) }, function (data) {
				vm.ListaUsuario=data.Lista;
			},
			function (err) {
				Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
			});

			if (param.id !== undefined && param.id !== '') {
				vm.edicao=true;
				ListaCompra.get({id: param.id }, function (data) {
					vm.itemListaCompra=data;
			if (data.IdentificadorViagem!=null)
				vm.itemViagem= {Identificador:data.IdentificadorViagem};
			if (data.IdentificadorUsuario!=null)
				vm.itemUsuario= {Identificador:data.IdentificadorUsuario};
			if (data.IdentificadorUsuarioPedido!=null)
				vm.itemUsuario= {Identificador:data.IdentificadorUsuarioPedido};

					vm.loading = false;
				}, function (err) {
					vm.loading = false;
					Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
				});
			}
			else
			{
				vm.inclusao=true;
				vm.loading = false
			};
		};
		vm.save = function (form) {
			vm.messages = [];
			vm.submitted = true;
			vm.CamposInvalidos = {};
			if (form.$valid) {
				if(vm.itemViagem!== null)
					vm.itemListaCompra.IdentificadorViagem= vm.itemViagem.Identificador;
				else
					vm.itemListaCompra.IdentificadorViagem= null;
				if(vm.itemUsuario!== null)
					vm.itemListaCompra.IdentificadorUsuario= vm.itemUsuario.Identificador;
				else
					vm.itemListaCompra.IdentificadorUsuario= null;
				if(vm.itemUsuario!== null)
					vm.itemListaCompra.IdentificadorUsuarioPedido= vm.itemUsuario.Identificador;
				else
					vm.itemListaCompra.IdentificadorUsuarioPedido= null;

				ListaCompra.save(vm.itemListaCompra, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('ListaCompra', { filtro: vm.filtroConsulta });
					} else {
						vm.messages = data.Mensagens;
						vm.verificaCampoInvalido();
					}
					}, function (err) {
						vm.loading = false;
						Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
					});
			}
			vm.submitted = false;
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
                vm.checarAcessos[item.Campo] = true;
            });
        };
	}
}());
