(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('AmigoEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Usuario','Amigo','SignalR', AmigoEditCtrl]);

	function AmigoEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Usuario,Amigo,SignalR) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemAmigo = {Seguido:false, Seguidor:false };
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.itemUsuario = null;
		vm.ListaUsuario = [];
		vm.itemUsuario = null;
		vm.ListaUsuario = [];
		vm.checarAcessos = {};
		vm.load = function () {
			vm.loading = true;
			var param = $stateParams;
			vm.filtroConsulta = param.filtro;		
			
			vm.inclusao=true;
			vm.loading = false			
		};

		vm.save = function (form) {
			vm.messages = [];
			vm.submitted = true;
			vm.CamposInvalidos = {};
			if (form.$valid) {
				

				Amigo.save(vm.itemAmigo, function (data) {
					vm.loading = false;
					if (data.Sucesso) {

					    var Mensagens = new Array();
					    $(data.Mensagens).each(function (j, jitem) {
					        Mensagens.push(jitem.Mensagem);
					    });

					    Error.showError('success', $translate.instant("Sucesso"), Mensagens, true);

                        if (vm.itemAmigo.Seguido)
					        SignalR.RequisitarAmizade(vm.itemAmigo.IdentificadorUsuario, data.IdentificadorRegistro);

						$state.go('Amigo', { filtro: vm.filtroConsulta });
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

		vm.CarregarUsuario = function () {
		    if (vm.itemAmigo.EMail && vm.itemAmigo.EMail.length > 0) {
		        Usuario.list({ json: JSON.stringify({ EMail: vm.itemAmigo.EMail }) }, function (data) {
		            if (data.Lista.length > 0) {
		                vm.itemAmigo.IdentificadorUsuario = data.Lista[0].Identificador;
		                vm.itemAmigo.Nome = data.Lista[0].Nome;
		            }
		            else {
		                vm.itemAmigo.IdentificadorUsuario = null;
		                vm.itemAmigo.Nome = null;
		            }
		        },
                function (err) {
                    Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
                });
		    }
		    else
		    {
		        vm.itemAmigo.IdentificadorUsuario = null;
		        vm.itemAmigo.Nome = null;
		    }
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
