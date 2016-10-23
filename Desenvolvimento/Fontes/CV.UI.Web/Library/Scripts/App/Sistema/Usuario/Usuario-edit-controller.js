(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('UsuarioEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Usuario', UsuarioEditCtrl]);

	function UsuarioEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Usuario) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemUsuario = { };
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};

		vm.load = function () {
			vm.loading = true;
			var param = $stateParams;
			vm.filtroConsulta = param.filtro;

			if (param.id !== undefined && param.id !== '') {
				vm.edicao=true;
				Usuario.get({id: param.id }, function (data) {
					vm.itemUsuario=data;

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
				if(vm.itemUsuario.DataToken){
					if (typeof vm.itemUsuario.DataToken == "string"){
						 var date = Date.parse(vm.itemUsuario.DataToken);
						if (!isNaN(date))
							vm.itemUsuario.DataToken=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemUsuario.DataToken=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemUsuario.DataToken);
				}

				Usuario.save(vm.itemUsuario, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('Usuario', { filtro: vm.filtroConsulta });
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
