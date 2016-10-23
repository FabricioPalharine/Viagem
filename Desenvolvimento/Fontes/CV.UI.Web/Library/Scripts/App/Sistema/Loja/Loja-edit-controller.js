(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('LojaEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Viagem','Atracao','Loja', LojaEditCtrl]);

	function LojaEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Viagem,Atracao,Loja) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemLoja = { };
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.itemViagem = null;
		vm.ListaViagem = [];
		vm.itemAtracao = null;
		vm.ListaAtracao = [];

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
			Atracao.list({ json: JSON.stringify({}) }, function (data) {
				vm.ListaAtracao=data.Lista;
			},
			function (err) {
				Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
			});

			if (param.id !== undefined && param.id !== '') {
				vm.edicao=true;
				Loja.get({id: param.id }, function (data) {
					vm.itemLoja=data;
			if (data.IdentificadorViagem!=null)
				vm.itemViagem= {Identificador:data.IdentificadorViagem};
			if (data.IdentificadorAtracao!=null)
				vm.itemAtracao= {Identificador:data.IdentificadorAtracao};

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
					vm.itemLoja.IdentificadorViagem= vm.itemViagem.Identificador;
				else
					vm.itemLoja.IdentificadorViagem= null;
				if(vm.itemLoja.Data){
					if (typeof vm.itemLoja.Data == "string"){
						 var date = Date.parse(vm.itemLoja.Data);
						if (!isNaN(date))
							vm.itemLoja.Data=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemLoja.Data=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemLoja.Data);
				}
				if(vm.itemAtracao!== null)
					vm.itemLoja.IdentificadorAtracao= vm.itemAtracao.Identificador;
				else
					vm.itemLoja.IdentificadorAtracao= null;

				Loja.save(vm.itemLoja, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('Loja', { filtro: vm.filtroConsulta });
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
