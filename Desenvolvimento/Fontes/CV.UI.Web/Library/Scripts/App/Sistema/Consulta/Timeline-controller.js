(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('TimelineCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Viagem', 'Consulta', 'Dominio', TimelineCtrl]);

	function TimelineCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Viagem, Consulta, Dominio, SignalR) {
		var vm = this;
		vm.filtro = { Index: 0, Count: 20 };
		vm.filtroAtualizacao = { Index: 0, Count: 20 };
		vm.loading = false;
		vm.ListaDados = [];
		vm.gridApi = null;
		vm.itemUsuario = {};
		vm.ListaUsuarios = [];
		vm.enableScroll = false;
		vm.continuaPesquisando = true;
		vm.FimPagina = false;
		vm.UltimaData = null;
		vm.load = function () {
		    vm.loading = true;

		    Viagem.CarregarParticipantesAmigo(function (lista) {
		        vm.ListaUsuarios = lista;
		    });
		    vm.CarregarDadosWebApi();
		    
		};


        vm.filtraDado = function () {
            vm.ListaDados = [];

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);
                     
            if (vm.itemUsuario && vm.itemUsuario.Identificador)
                vm.filtroAtualizacao.IdentificadorParticipante = vm.itemUsuario.Identificador;
            else
                vm.filtroAtualizacao.IdentificadorParticipante = null;
    
            vm.CarregarDadosWebApi();
        };


        vm.AjustarDadosPagina = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
            vm.Total = 0;
            angular.forEach(data, function (c) {
                vm.ListaDados.push(c);
                vm.UltimaData = c.Data;
            });
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


        vm.ProximaPagina = function () {
                vm.enableScroll = false;
                vm.loading = true;
                vm.filtroAtualizacao.DataInicioDe = vm.UltimaData;
                vm.CarregarDadosWebApi();
            

        };

        vm.RetornarUsuarios = function (lista) {
            return lista.map(function (v) { return v.Nome; }).join(' ,').replace(/,([^,]*)$/, 'e $1');
        };
    
        vm.RetornarPedidos = function (lista) {
            return lista.filter(function (v) { return v.Pedido; });
        };

        vm.RetornarNotas = function (lista) {
            return lista.filter(function (v) { return v.Nota; });
        };

        vm.RetornarURLMap = function (item) {
            var url = "https://maps.googleapis.com/maps/api/staticmap?center=" + item.Latitude + ',' + item.Longitude +
                '&zoom=16&size=400x260&maptype=roadmap&markers=color:blue%7C' + item.Latitude + ',' + item.Longitude +
                '&key=AIzaSyAlUpOpwZWS_ZGlMAtB6lY76oy1QBWk97g';
            return url;
        };
//
        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            vm.filtroAtualizacao.Index = 0;

            vm.CamposInvalidos = {};
            vm.messages = [];

            Consulta.ListarTimeline({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                vm.AjustarDadosPagina(data);
                if (!data.Sucesso) {
                    vm.messages = data.Mensagens;
                }
                vm.enableScroll = true;
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
