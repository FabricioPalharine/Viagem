(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('PontosMapaCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Viagem', 'Consulta', 'Dominio','NgMap', PontosMapaCtrl]);

	function PontosMapaCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Viagem, Consulta, Dominio, NgMap) {
		var vm = this;
		vm.filtro = { Index: 0, Count: 0 };
		vm.filtroAtualizacao = { Index: 0, Count: 0 };
		vm.loading = false;
		vm.ListaDados = [];
		vm.gridApi = null;
		vm.itemUsuario = {};
		vm.ListaUsuarios = [];
		vm.load = function () {
		    vm.loading = true;
		    MarkerClusterer.prototype.MARKER_CLUSTER_IMAGE_PATH_  = 'library/res/images/m'
		    Viagem.CarregarParticipantesAmigo(function (lista) {

		        vm.ListaUsuarios = lista;
		        var listaEu = lista.filter(function (v) { return v.Identificador == Auth.currentUser.Codigo; });
		        if (listaEu.length > 0)
		            vm.itemUsuario = listaEu[0];
		        else
		            vm.itemUsuario = lista[0];
		        vm.filtroAtualizacao.IdentificadorParticipante = vm.itemUsuario.Identificador;
		        vm.CarregarDadosWebApi();

		    });
		    
		};


        vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);
                     
            if (vm.itemUsuario && vm.itemUsuario.Identificador)
                vm.filtroAtualizacao.IdentificadorParticipante = vm.itemUsuario.Identificador;
            else
                vm.filtroAtualizacao.IdentificadorParticipante = null;
    
            vm.CarregarDadosWebApi();
        };


        vm.AjustarDadosPagina = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
            vm.ListaDados = data;

           if (!$scope.$$phase) {
                $scope.$apply();
           }
           vm.dynMarkers = [];
           $timeout(function () {
               NgMap.getMap().then(function (map) {
                   var bounds = new google.maps.LatLngBounds();
                   for (var k in map.markers) {
                       var cm = map.markers[k];
                       vm.dynMarkers.push(cm);
                       bounds.extend(cm.getPosition());
                   };

                   vm.markerClusterer = new MarkerClusterer(map, vm.dynMarkers, {});
                   map.setCenter(bounds.getCenter());
                   map.fitBounds(bounds);
               })
           }, 1000);
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

            Consulta.ListarPontosViagem({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
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

        vm.dynMarkers = []
       

//
  	}
}());
