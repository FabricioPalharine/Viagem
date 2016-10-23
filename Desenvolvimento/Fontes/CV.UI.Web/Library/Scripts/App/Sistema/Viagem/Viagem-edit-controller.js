(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ViagemEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Usuario','Viagem', ViagemEditCtrl]);

	function ViagemEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Usuario,Viagem) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemViagem = { };
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
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

			if (param.id !== undefined && param.id !== '') {
				vm.edicao=true;
				Viagem.get({id: param.id }, function (data) {
					vm.itemViagem=data;
			if (data.IdentificadorUsuario!=null)
				vm.itemUsuario= {Identificador:data.IdentificadorUsuario};

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
					vm.itemViagem.IdentificadorUsuario= vm.itemUsuario.Identificador;
				else
					vm.itemViagem.IdentificadorUsuario= null;
				if(vm.itemViagem.DataInicio){
					if (typeof vm.itemViagem.DataInicio == "string"){
						 var date = Date.parse(vm.itemViagem.DataInicio);
						if (!isNaN(date))
							vm.itemViagem.DataInicio=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemViagem.DataInicio=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemViagem.DataInicio);
				}
				if(vm.itemViagem.DataFim){
					if (typeof vm.itemViagem.DataFim == "string"){
						 var date = Date.parse(vm.itemViagem.DataFim);
						if (!isNaN(date))
							vm.itemViagem.DataFim=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemViagem.DataFim=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemViagem.DataFim);
				}

				Viagem.save(vm.itemViagem, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('Viagem', { filtro: vm.filtroConsulta });
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
