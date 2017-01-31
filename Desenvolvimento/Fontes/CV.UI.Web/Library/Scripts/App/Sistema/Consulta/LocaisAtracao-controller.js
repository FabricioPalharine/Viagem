(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('LocaisAtracaoCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Viagem', 'Consulta', 'Dominio', LocaisAtracaoCtrl]);

    function LocaisAtracaoCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Viagem, Consulta, Dominio, SignalR) {
        var vm = this;
        vm.filtroAtualizacao = { Index: 0, Count: 0 };
        vm.loading = false;
        vm.ListaDados = [];
        vm.ListaDetalhes = [];
        vm.ListaGastos = [];
        vm.ListaFotos = [];
        vm.Item = null;
        vm.ItemPai = null;
        vm.load = function () {
            vm.loading = true;
            var param = $stateParams;
            if (param.filtro != null) {
               vm.filtroAtualizacao = param.filtro;
            }
            if (param.Item != null) {
                vm.Item = param.Item;
            }
            if (param.ItemPai != null)
                vm.ItemPai = param.ItemPai;
            vm.CarregarDadosWebApi();
            vm.CarregarEndereco();

        };

        vm.CarregarEndereco = function () {
            var geocoder = new google.maps.Geocoder;
            var latlng = { lat: vm.Item.Latitude, lng: vm.Item.Longitude };

            geocoder.geocode({ 'location': latlng }, function (results, status) {
                if (status === google.maps.GeocoderStatus.OK) {
                    if (results[0]) {
                        vm.Item.Endereco = results[0].formatted_address;
                    } 
                } 
            });

        };

        vm.Voltar = function()
        {
            if (vm.ItemPai == null)
                $state.go('ConsultarLocaisVisitados', { filtro: vm.filtroConsulta });
            else
                $state.go('ConsultarLocaisAtracao', { filtro: vm.filtroConsulta, Item: vm.ItemPai.Item, ItemPai: vm.ItemPai.ItemPai });

        }

  
        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };

        vm.AbrirDetalhes = function (item) {
            
                if (item.Tipo == "A") {
                    $state.go('ConsultarLocaisAtracao', { filtro: vm.filtroConsulta, Item: item, ItemPai:vm });
                }
              
                else if (item.Tipo == "R") {
                    $state.go('ConsultarLocaisRestaurante', { filtro: vm.filtroConsulta, Item: item, ItemPai: vm });
                }
                else if (item.Tipo == "L") {
                    $state.go('ConsultarLocaisLoja', { filtro: vm.filtroConsulta, Item: item, ItemPai: vm });
                }
            
        };

        vm.RetornarURLMap = function (item) {
            var url = "https://maps.googleapis.com/maps/api/staticmap?center=" + item.Latitude + ',' + item.Longitude +
                '&zoom=16&size=200x200&maptype=roadmap&markers=color:blue%7C' + item.Latitude + ',' + item.Longitude +
                '&key=AIzaSyAlUpOpwZWS_ZGlMAtB6lY76oy1QBWk97g';
            return url;
        };

        //
        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            
            vm.CamposInvalidos = {};
            vm.messages = [];

            var FiltroConsulta = jQuery.extend({}, vm.filtroAtualizacao);
            FiltroConsulta.Nome = vm.Item.Nome;
            FiltroConsulta.Comentario = vm.Item.CodigoCoogle;
            Consulta.CarregarDetalhesAtracao({ json: JSON.stringify(FiltroConsulta) }, function (data) {
                vm.loading = false;
                vm.ListaDados = data.LocaisFilho;
                vm.ListaDetalhes = data.Detalhes;
                vm.ListaGastos = data.Gastos;
                vm.ListaFotos = data.Fotos.map(function (v) { return {
                    title:  moment(v.Data).format("DD/MM/YYYY HH:mm") + (v.Comentario?" - " + v.Comentario:""),
                    thumbUrl: v.LinkThumbnail,
                    url: v.LinkFoto,
                    video: v.Video}; });
                vm.Item.Media = data.Media;
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
