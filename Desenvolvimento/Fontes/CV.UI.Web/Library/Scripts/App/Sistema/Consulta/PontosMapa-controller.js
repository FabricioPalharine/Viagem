(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('PontosMapaCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Viagem', 'Consulta', 'Dominio', 'NgMap', PontosMapaCtrl]);

    function PontosMapaCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Viagem, Consulta, Dominio, NgMap) {
        var vm = this;
        vm.filtro = { Index: 0, Count: 0 };
        vm.filtroAtualizacao = { Index: 0, Count: 0 };
        vm.loading = false;
        vm.ListaDados = [];
        vm.gridApi = null;
        vm.itemUsuario = {};
        vm.ListaUsuarios = [];
        vm.ListaLinhas = [];
        vm.itemSelecionado = {};
        vm.map = null;
        vm.load = function () {
            vm.loading = true;
            MarkerClusterer.prototype.MARKER_CLUSTER_IMAGE_PATH_ = 'library/res/images/m'
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

        vm.RetornarIcone = function (item) {
            var url = item.Tipo == "A" ? "entertainment" :
                    item.Tipo == "CR" || item.Tipo == "CD" ? "automotive" :
                    item.Tipo == "L" ? "shopping" :
                    item.Tipo == "H" ? "hotels" :
                    item.Tipo == "RC" ? "tires-accessories" :
                    item.Tipo == "R" ? "restaurants" :
                    item.Tipo == "P" ? "transport" :
                    item.Tipo == "T" ? "cookbooks" :
                    item.Tipo == "V" || item.Tipo == "F" ? "photography" : "pin";
            var image = {
                url: 'library/res/images/' + url + '.png',

                scaledSize: [22, 29],
                // The origin for this image is (0, 0).
                origin: [0, 0]
            };

            return image;
        };

        vm.AjustarDadosPagina = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
            if (vm.markerClusterer)
                vm.markerClusterer.removeMarkers(vm.dynMarkers);

            vm.ListaDados = [];
            NgMap.getMap().then(function (map) {


                for (var k in map.markers) {
                    var cm = map.markers[k];
                    cm.setMap(null);
                };
                map.markers = [];



                vm.ListaDados = data;

                if (!$scope.$$phase) {
                    $scope.$apply();
                }
                angular.forEach(vm.ListaDados, function (item) {
                    item.Icone = item.Tipo == "A" ? "entertainment" :
                         item.Tipo == "CR" || item.Tipo == "CD" ? "automotive" :
                         item.Tipo == "L" ? "shopping" :
                         item.Tipo == "H" ? "hotels" :
                         item.Tipo == "RC" ? "tires-accessories" :
                         item.Tipo == "R" ? "restaurants" :
                         item.Tipo == "P" ? "transport" :
                         item.Tipo == "T" ? "cookbooks" :
                         item.Tipo == "U" ? "professional" :
                         item.Tipo == "V" || item.Tipo == "F" ? "photography" : "pin";
                });


                vm.dynMarkers = [];
                $timeout(function () {
                    NgMap.getMap().then(function (map) {

                        var bounds = new google.maps.LatLngBounds();
                        for (var k in map.markers) {
                            var cm = map.markers[k];
                            vm.dynMarkers.push(cm);
                            bounds.extend(cm.getPosition());
                        };
                        vm.map = map;
                        vm.markerClusterer = new MarkerClusterer(map, vm.dynMarkers, {});
                        map.setCenter(bounds.getCenter());
                        map.fitBounds(bounds);
                        //google.maps.event.trigger(vm.map, "resize");

                    })
                }, 1000);
            });
        };

        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };

        vm.AjustarDadosLinha = function (data) {
            vm.ListaLinhas = [];
            angular.forEach(data, function (item) {
                item.Points = [];
                item.Color = item.Tipo == "C" ? "blue" : item.Tipo == "D" ? "green" : "red";
                angular.forEach(item.Pontos, function (ponto) {
                    item.Points.push([ponto.Latitude, ponto.Longitude]);
                });
                vm.ListaLinhas.push(item);
            });
        }

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

            Consulta.ListarLinhasViagem({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                vm.AjustarDadosLinha(data);

                vm.loading = false;
            }, function (err) {

            });
        };

        vm.AbrirInfo = function (e, ponto) {
            vm.itemSelecionado = ponto;
            var id = 'dado-window';
            if (ponto.Tipo != "F" && ponto.Tipo != "V")
                id = 'dado-window';
            else
                id = 'url-window';
            $timeout(function () {
                vm.map.infoWindows[id].setPosition(new google.maps.LatLng({ lat: ponto.Latitude, lng: ponto.Longitude })), 200
            });
            vm.map.showInfoWindow(id, ponto.$$hashKey);
        };
        vm.dynMarkers = []


        //
    }
}());
