(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('LocaisLojaCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Viagem', 'Consulta', 'Dominio', LocaisLojaCtrl]);

    function LocaisLojaCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Viagem, Consulta, Dominio, SignalR) {
        var vm = this;
        vm.filtroAtualizacao = { Index: 0, Count: 0 };
        vm.loading = false;
        vm.ListaGastos = [];
        vm.Total = 0;
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

        vm.RetornarURLMap = function (item) {
            var url = "https://maps.googleapis.com/maps/api/staticmap?center=" + item.Latitude + ',' + item.Longitude +
                '&zoom=16&size=200x200&maptype=roadmap&markers=color:blue%7C' + item.Latitude + ',' + item.Longitude +
                '&key=' + Auth.apiKey;
            return url;
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

   

        //
        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            
            vm.CamposInvalidos = {};
            vm.messages = [];

            var FiltroConsulta = jQuery.extend({}, vm.filtroAtualizacao);
            FiltroConsulta.Nome = vm.Item.Nome;
            FiltroConsulta.Comentario = vm.Item.CodigoCoogle;
            Consulta.CarregarDetalhesLoja({ json: JSON.stringify(FiltroConsulta) }, function (data) {
                vm.loading = false;
                vm.ListaGastos = data.Gastos;
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
