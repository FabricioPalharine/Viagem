(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('CalendarioPrevistoEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Viagem','CalendarioPrevisto', CalendarioPrevistoEditCtrl]);

	function CalendarioPrevistoEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Viagem,CalendarioPrevisto) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemCalendarioPrevisto = { };
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.itemViagem = null;
		vm.ListaViagem = [];

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

			if (param.id !== undefined && param.id !== '') {
				vm.edicao=true;
				CalendarioPrevisto.get({id: param.id }, function (data) {
					vm.itemCalendarioPrevisto=data;
			if (data.IdentificadorViagem!=null)
				vm.itemViagem= {Identificador:data.IdentificadorViagem};

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
					vm.itemCalendarioPrevisto.IdentificadorViagem= vm.itemViagem.Identificador;
				else
					vm.itemCalendarioPrevisto.IdentificadorViagem= null;
				if(vm.itemCalendarioPrevisto.Data){
					if (typeof vm.itemCalendarioPrevisto.Data == "string"){
						 var date = Date.parse(vm.itemCalendarioPrevisto.Data);
						if (!isNaN(date))
							vm.itemCalendarioPrevisto.Data=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemCalendarioPrevisto.Data=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemCalendarioPrevisto.Data);
				}

				CalendarioPrevisto.save(vm.itemCalendarioPrevisto, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('CalendarioPrevisto', { filtro: vm.filtroConsulta });
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
