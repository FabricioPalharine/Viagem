(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('PosicaoEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Usuario','Viagem','Posicao', PosicaoEditCtrl]);

	function PosicaoEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Usuario,Viagem,Posicao) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemPosicao = { };
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

			if (param.id !== undefined && param.id !== '') {
				vm.edicao=true;
				Posicao.get({id: param.id }, function (data) {
					vm.itemPosicao=data;
			if (data.IdentificadorViagem!=null)
				vm.itemViagem= {Identificador:data.IdentificadorViagem};
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
				if(vm.itemViagem!== null)
					vm.itemPosicao.IdentificadorViagem= vm.itemViagem.Identificador;
				else
					vm.itemPosicao.IdentificadorViagem= null;
				if(vm.itemUsuario!== null)
					vm.itemPosicao.IdentificadorUsuario= vm.itemUsuario.Identificador;
				else
					vm.itemPosicao.IdentificadorUsuario= null;
				if(vm.itemPosicao.DataGMT){
					if (typeof vm.itemPosicao.DataGMT == "string"){
						 var date = Date.parse(vm.itemPosicao.DataGMT);
						if (!isNaN(date))
							vm.itemPosicao.DataGMT=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemPosicao.DataGMT=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemPosicao.DataGMT);
				}
				if(vm.itemPosicao.DataLocal){
					if (typeof vm.itemPosicao.DataLocal == "string"){
						 var date = Date.parse(vm.itemPosicao.DataLocal);
						if (!isNaN(date))
							vm.itemPosicao.DataLocal=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemPosicao.DataLocal=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemPosicao.DataLocal);
				}

				Posicao.save(vm.itemPosicao, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('Posicao', { filtro: vm.filtroConsulta });
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
