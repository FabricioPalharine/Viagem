(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('LocaisVisitadosCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Viagem', 'Consulta', 'Dominio', LocaisVisitadosCtrl]);

    function LocaisVisitadosCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Viagem, Consulta, Dominio, SignalR) {
        var vm = this;
        vm.filtro = { Index: 0, Count: 0 };
        vm.filtroAtualizacao = { Index: 0, Count: 0 };
        vm.loading = false;
        vm.ListaDados = [];
        vm.gridApi = null;
        vm.itemUsuario = {};
        vm.Total = 0;
        vm.load = function () {
            vm.loading = true;
            var param = $stateParams;
            if (param.filtro != null) {
                vm.filtro = vm.filtroAtualizacao = param.filtro;
            }
            vm.CarregarDadosWebApi();

        };


        vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);

  
            vm.CarregarDadosWebApi();
        };


        vm.AjustarDadosPagina = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
            vm.ListaDados = data;
            vm.Total = 0;
            angular.forEach(vm.ListaDados, function (c) {
                vm.Total += c.ValorReal;
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

        vm.AbrirDetalhes = function (item) {
            if (item.Tipo == "A")
            {
                $state.go('ConsultarLocaisAtracao', { filtro: vm.filtroConsulta, Item: item });
            }
            else if (item.Tipo == "H") {
                $state.go('ConsultarLocaisHotel', { filtro: vm.filtroConsulta, Item: item });
            }
            else if (item.Tipo == "R") {
                $state.go('ConsultarLocaisRestaurante', { filtro: vm.filtroConsulta, Item: item });
            }
            else if (item.Tipo == "L") {
                $state.go('ConsultarLocaisLoja', { filtro: vm.filtroConsulta, Item: item });
            }
        };

        vm.RetornarURLMap = function (item) {
            var url = "https://maps.googleapis.com/maps/api/staticmap?center=" + item.Latitude + ',' + item.Longitude +
                '&zoom=16&size=200x200&maptype=roadmap&markers=color:blue%7C' + item.Latitude + ',' + item.Longitude +
                '&key=' + Auth.apiKey;
            return url;
        };

        //
        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            vm.filtroAtualizacao.Index = 0;
            vm.filtroAtualizacao.Count = null;

            vm.CamposInvalidos = {};
            vm.messages = [];

            Consulta.ListarLocaisVisitados({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
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

        vm.abrirMapa = function (Item) {
            $uibModal.open({
                templateUrl: 'modalMapa.html',
                controller: ['$uibModalInstance', 'NgMap', '$timeout', '$scope', 'item', vm.MapModalCtrl],
                controllerAs: 'vmMapa',
                resolve: {
                    item: function () { return Item; },
                }
            });

        };

        vm.MapModalCtrl = function ($uibModalInstance, NgMap, $timeout, $scope, item) {
            var vmMapa = this;
            vmMapa.lat = 0;
            vmMapa.lng = 0;
            vmMapa.itemFoto = item;
            vmMapa.itemMarcador = {};
            vmMapa.map = null;
                                  

            NgMap.getMap().then(function (evtMap) {
                vmMapa.map = evtMap;
                $timeout(function () {
                    google.maps.event.trigger(vmMapa.map, "resize");

                    if (vmMapa.itemFoto.Latitude && vmMapa.itemFoto.Longitude) {
                        vmMapa.lat = vmMapa.itemFoto.Latitude;
                        vmMapa.lng = vmMapa.itemFoto.Longitude;
                        vmMapa.itemMarcador = { Latitude: vmMapa.itemFoto.Latitude, Longitude: vmMapa.itemFoto.Longitude };
                    }
                    $scope.$apply();
                }, 500);
            });

            vmMapa.close = function () {
                $uibModalInstance.close();
            };                    

          
        };
    }
}());
