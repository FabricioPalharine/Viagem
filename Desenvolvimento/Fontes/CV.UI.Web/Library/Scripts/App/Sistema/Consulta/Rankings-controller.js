(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('RankingsCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Viagem', 'Consulta', 'Dominio','$compile','Usuario', RankingsCtrl]);

	function RankingsCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Viagem, Consulta, Dominio, $compile,Usuario) {
		var vm = this;
		vm.filtro = { Index: 0, Count: 40, Aberto: false, TipoInteiro: 1 };
		vm.filtroAtualizacao = { Index: 0, Count: 40, Aberto: false, TipoInteiro: 1 };
		vm.loading = false;
		vm.ListaDados = [];

		vm.itemUsuario = {};
		vm.ListaUsuarios = [];
		vm.Total = 0;
		vm.load = function () {
		    vm.loading = true;

		    Usuario.listaAmigosComigo(function (data) {
		        vm.ListaUsuarios = data;
		    })
		    vm.CarregarDadosWebApi();

		};


        vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);
                     
            if (vm.itemUsuario && vm.itemUsuario.Identificador && vm.filtro.TipoInteiro == 3)
                vm.filtroAtualizacao.IdentificadorParticipante = vm.itemUsuario.Identificador;
            else
                vm.filtroAtualizacao.IdentificadorParticipante = null;
    
            vm.CarregarDadosWebApi();
        };



        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };

    
//
        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            vm.filtroAtualizacao.Index = 0;
          

            Consulta.ListarRankings({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                vm.ListaDados = data;

                           
                vm.loading = false;
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
                
                vm.loading = false;
            });
        };

        vm.loadAvaliacoes = function (item) {
            if (!item.Avaliacoes || item.Avaliacoes.length == 0) {
                var filtroLocal = jQuery.extend({}, vm.filtroAtualizacao);
                filtroLocal.Tipo = item.Tipo;
                filtroLocal.Nome = item.Nome;
                filtroLocal.Comentario = item.CodigoGoogle;
                Consulta.ListarAvaliacoesRankings({ json: JSON.stringify(filtroLocal) }, function (data) {
                    vm.loading = false;

                        item.Avaliacoes = data;
                    
                    vm.loading = false;
                }, function (err) {
                    vm.loading = false;
                    Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);

                    vm.loading = false;
                });
            }

        };
     
//
  	}
}());
