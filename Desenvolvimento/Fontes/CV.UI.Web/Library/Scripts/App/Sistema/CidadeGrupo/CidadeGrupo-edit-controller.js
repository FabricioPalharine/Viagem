(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('CidadeGrupoEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade','Viagem','CidadeGrupo', CidadeGrupoEditCtrl]);

	function CidadeGrupoEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Cidade,Viagem,CidadeGrupo) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemCidadeGrupo = { };
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.itemViagem = null;
		vm.ListaViagem = [];
		vm.itemCidade = null;
		vm.ListaCidade = [];
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
			Cidade.list({ json: JSON.stringify({}) }, function (data) {
				vm.ListaCidade=data.Lista;
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
				CidadeGrupo.get({id: param.id }, function (data) {
					vm.itemCidadeGrupo=data;
			if (data.IdentificadorViagem!=null)
				vm.itemViagem= {Identificador:data.IdentificadorViagem};
			if (data.IdentificadorCidadeFilha!=null)
				vm.itemCidade= {Identificador:data.IdentificadorCidadeFilha};
			if (data.IdentificadorCidadePai!=null)
				vm.itemCidade= {Identificador:data.IdentificadorCidadePai};

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
					vm.itemCidadeGrupo.IdentificadorViagem= vm.itemViagem.Identificador;
				else
					vm.itemCidadeGrupo.IdentificadorViagem= null;
				if(vm.itemCidade!== null)
					vm.itemCidadeGrupo.IdentificadorCidadeFilha= vm.itemCidade.Identificador;
				else
					vm.itemCidadeGrupo.IdentificadorCidadeFilha= null;
				if(vm.itemCidade!== null)
					vm.itemCidadeGrupo.IdentificadorCidadePai= vm.itemCidade.Identificador;
				else
					vm.itemCidadeGrupo.IdentificadorCidadePai= null;

				CidadeGrupo.save(vm.itemCidadeGrupo, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('CidadeGrupo', { filtro: vm.filtroConsulta });
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
