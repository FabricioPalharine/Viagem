(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('SugestaoEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade','Usuario','Viagem','Sugestao', SugestaoEditCtrl]);

	function SugestaoEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Cidade,Usuario,Viagem,Sugestao) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemSugestao = { };
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
		vm.itemCidade = null;
		vm.ListaCidade = [];

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
			Cidade.list({ json: JSON.stringify({}) }, function (data) {
				vm.ListaCidade=data.Lista;
			},
			function (err) {
				Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
			});

			if (param.id !== undefined && param.id !== '') {
				vm.edicao=true;
				Sugestao.get({id: param.id }, function (data) {
					vm.itemSugestao=data;
			if (data.IdentificadorViagem!=null)
				vm.itemViagem= {Identificador:data.IdentificadorViagem};
			if (data.IdentificadorUsuario!=null)
				vm.itemUsuario= {Identificador:data.IdentificadorUsuario};
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
				if(vm.itemViagem!== null)
					vm.itemSugestao.IdentificadorViagem= vm.itemViagem.Identificador;
				else
					vm.itemSugestao.IdentificadorViagem= null;
				if(vm.itemUsuario!== null)
					vm.itemSugestao.IdentificadorUsuario= vm.itemUsuario.Identificador;
				else
					vm.itemSugestao.IdentificadorUsuario= null;
				if(vm.itemCidade!== null)
					vm.itemSugestao.IdentificadorCidade= vm.itemCidade.Identificador;
				else
					vm.itemSugestao.IdentificadorCidade= null;

				Sugestao.save(vm.itemSugestao, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('Sugestao', { filtro: vm.filtroConsulta });
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
