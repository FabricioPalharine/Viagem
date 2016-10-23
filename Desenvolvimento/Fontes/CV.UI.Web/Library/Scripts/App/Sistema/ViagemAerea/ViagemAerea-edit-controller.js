(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ViagemAereaEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Viagem','ViagemAerea', ViagemAereaEditCtrl]);

	function ViagemAereaEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Viagem,ViagemAerea) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemViagemAerea = { };
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
				ViagemAerea.get({id: param.id }, function (data) {
					vm.itemViagemAerea=data;
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
					vm.itemViagemAerea.IdentificadorViagem= vm.itemViagem.Identificador;
				else
					vm.itemViagemAerea.IdentificadorViagem= null;
				if(vm.itemViagemAerea.DataPrevista){
					if (typeof vm.itemViagemAerea.DataPrevista == "string"){
						 var date = Date.parse(vm.itemViagemAerea.DataPrevista);
						if (!isNaN(date))
							vm.itemViagemAerea.DataPrevista=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemViagemAerea.DataPrevista=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemViagemAerea.DataPrevista);
				}
				if(vm.itemViagemAerea.DataInicio){
					if (typeof vm.itemViagemAerea.DataInicio == "string"){
						 var date = Date.parse(vm.itemViagemAerea.DataInicio);
						if (!isNaN(date))
							vm.itemViagemAerea.DataInicio=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemViagemAerea.DataInicio=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemViagemAerea.DataInicio);
				}
				if(vm.itemViagemAerea.DataFim){
					if (typeof vm.itemViagemAerea.DataFim == "string"){
						 var date = Date.parse(vm.itemViagemAerea.DataFim);
						if (!isNaN(date))
							vm.itemViagemAerea.DataFim=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemViagemAerea.DataFim=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemViagemAerea.DataFim);
				}

				ViagemAerea.save(vm.itemViagemAerea, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('ViagemAerea', { filtro: vm.filtroConsulta });
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
