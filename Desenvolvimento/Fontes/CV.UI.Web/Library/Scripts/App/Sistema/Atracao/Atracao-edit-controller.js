(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('AtracaoEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade','Viagem','Atracao', AtracaoEditCtrl]);

	function AtracaoEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Cidade,Viagem,Atracao) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemAtracao = { };
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

			if (param.id !== undefined && param.id !== '') {
				vm.edicao=true;
				Atracao.get({id: param.id }, function (data) {
					vm.itemAtracao=data;
			if (data.IdentificadorViagem!=null)
				vm.itemViagem= {Identificador:data.IdentificadorViagem};
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
					vm.itemAtracao.IdentificadorViagem= vm.itemViagem.Identificador;
				else
					vm.itemAtracao.IdentificadorViagem= null;
				if(vm.itemCidade!== null)
					vm.itemAtracao.IdentificadorCidade= vm.itemCidade.Identificador;
				else
					vm.itemAtracao.IdentificadorCidade= null;
				if(vm.itemAtracao.Chegada){
					if (typeof vm.itemAtracao.Chegada == "string"){
						 var date = Date.parse(vm.itemAtracao.Chegada);
						if (!isNaN(date))
							vm.itemAtracao.Chegada=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemAtracao.Chegada=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemAtracao.Chegada);
				}
				if(vm.itemAtracao.Partida){
					if (typeof vm.itemAtracao.Partida == "string"){
						 var date = Date.parse(vm.itemAtracao.Partida);
						if (!isNaN(date))
							vm.itemAtracao.Partida=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemAtracao.Partida=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemAtracao.Partida);
				}

				Atracao.save(vm.itemAtracao, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('Atracao', { filtro: vm.filtroConsulta });
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
