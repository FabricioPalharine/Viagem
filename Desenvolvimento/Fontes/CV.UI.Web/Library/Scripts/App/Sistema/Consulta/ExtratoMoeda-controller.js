(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ExtratoMoedaCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService',  'Viagem', 'Consulta', 'Dominio', ExtratoMoedaCtrl]);

	function ExtratoMoedaCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService,  Viagem, Consulta, Dominio, SignalR) {
		var vm = this;
		vm.filtro = { Index: 0, Count: 0, DataInicioDe: moment(new Date()).format("YYYY-MM-DD") };
		vm.filtroAtualizacao = { Index: 0, Count: 0, DataInicioDe: moment(new Date()).format("YYYY-MM-DD") };
		vm.loading = false;
		vm.ListaDados = [];
		vm.gridApi = null;
		vm.ListaMoeda = [];
		vm.itemMoeda = {};

		vm.load = function () {
		    vm.loading = true;

		    Dominio.CarregaMoedas(function (data) {
		        vm.ListaMoeda = data;
		    });
		    Viagem.get({ id: Auth.currentUser.IdentificadorViagem }, function (data) {
		        vm.filtroAtualizacao.Moeda = data.Moeda;
		        vm.itemMoeda = { Codigo: data.Moeda };
		        vm.CarregarDadosWebApi();
		    });
		};


        vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);
            if (vm.itemMoeda && vm.itemMoeda.Codigo)
                vm.filtroAtualizacao.Moeda = vm.itemMoeda.Codigo;
            else
                vm.filtroAtualizacao.Moeda = null;                 

    
            vm.CarregarDadosWebApi();
        };


        vm.AjustarDadosPagina = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
            vm.ListaDados = data;
            if (!$scope.$$phase) {
                $scope.$apply();
            }
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
            vm.filtroAtualizacao.Count = null;

            vm.CamposInvalidos = {};
            vm.messages = [];

            Consulta.ConsultarExtratoMoeda({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                vm.AjustarDadosPagina(data);
                if (!data.Sucesso) {
                    vm.messages = data.Mensagens;
                }               
                vm.loading = false;
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
                
                vm.loading = false;
            });
        };
//
  	}
}());
