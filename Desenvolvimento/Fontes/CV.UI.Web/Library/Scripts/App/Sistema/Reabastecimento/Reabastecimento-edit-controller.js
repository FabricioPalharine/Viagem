(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ReabastecimentoEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Carro','Cidade','Reabastecimento', ReabastecimentoEditCtrl]);

	function ReabastecimentoEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Carro,Cidade,Reabastecimento) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemReabastecimento = { };
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.itemCarro = null;
		vm.ListaCarro = [];
		vm.itemCidade = null;
		vm.ListaCidade = [];

		vm.load = function () {
			vm.loading = true;
			var param = $stateParams;
			vm.filtroConsulta = param.filtro;
			Carro.list({ json: JSON.stringify({}) }, function (data) {
				vm.ListaCarro=data.Lista;
			},
			function (err) {
				Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
			});
			Cidade.list({ json: JSON.stringify({}) }, function (data) {
				vm.ListaCidade=data.Lista;
			},
			function (err) {
				Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
			});

			if (param.id !== undefined && param.id !== '') {
				vm.edicao=true;
				Reabastecimento.get({id: param.id }, function (data) {
					vm.itemReabastecimento=data;
			if (data.IdentificadorCarro!=null)
				vm.itemCarro= {Identificador:data.IdentificadorCarro};
			if (data.IdentificadorCidade!=null)
				vm.itemCidade= {Identificador:data.IdentificadorCidade};

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
				if(vm.itemCarro!== null)
					vm.itemReabastecimento.IdentificadorCarro= vm.itemCarro.Identificador;
				else
					vm.itemReabastecimento.IdentificadorCarro= null;
				if(vm.itemCidade!== null)
					vm.itemReabastecimento.IdentificadorCidade= vm.itemCidade.Identificador;
				else
					vm.itemReabastecimento.IdentificadorCidade= null;

				Reabastecimento.save(vm.itemReabastecimento, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('Reabastecimento', { filtro: vm.filtroConsulta });
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
