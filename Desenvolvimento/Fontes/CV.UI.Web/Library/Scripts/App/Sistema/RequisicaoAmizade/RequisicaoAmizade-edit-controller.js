(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('RequisicaoAmizadeEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Usuario','RequisicaoAmizade', RequisicaoAmizadeEditCtrl]);

	function RequisicaoAmizadeEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Usuario,RequisicaoAmizade) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemRequisicaoAmizade = { };
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.itemUsuario = null;
		vm.ListaUsuario = [];
		vm.itemUsuario = null;
		vm.ListaUsuario = [];

		vm.load = function () {
			vm.loading = true;
			var param = $stateParams;
			vm.filtroConsulta = param.filtro;
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
				RequisicaoAmizade.get({id: param.id }, function (data) {
					vm.itemRequisicaoAmizade=data;
			if (data.IdentificadorUsuario!=null)
				vm.itemUsuario= {Identificador:data.IdentificadorUsuario};
			if (data.IdentificadorUsuarioRequisitado!=null)
				vm.itemUsuario= {Identificador:data.IdentificadorUsuarioRequisitado};

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
				if(vm.itemUsuario!== null)
					vm.itemRequisicaoAmizade.IdentificadorUsuario= vm.itemUsuario.Identificador;
				else
					vm.itemRequisicaoAmizade.IdentificadorUsuario= null;
				if(vm.itemUsuario!== null)
					vm.itemRequisicaoAmizade.IdentificadorUsuarioRequisitado= vm.itemUsuario.Identificador;
				else
					vm.itemRequisicaoAmizade.IdentificadorUsuarioRequisitado= null;

				RequisicaoAmizade.save(vm.itemRequisicaoAmizade, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('RequisicaoAmizade', { filtro: vm.filtroConsulta });
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
