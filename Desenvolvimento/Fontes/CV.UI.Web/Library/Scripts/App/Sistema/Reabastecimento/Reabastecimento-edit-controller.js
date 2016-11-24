(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('ReabastecimentoEditCtrl', ['$uibModalInstance', 'Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Carro',  'Reabastecimento', 'EscopoAtualizacao', 'Viagem', '$uibModal', ReabastecimentoEditCtrl]);

    function ReabastecimentoEditCtrl($uibModalInstance, Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Carro, Reabastecimento, EscopoAtualizacao, Viagem, $uibModal) {
        var vm = this;

        vm.EscopoAtualizacao = EscopoAtualizacao;
        vm.itemReabastecimento = { IdentificadorCarro: vm.EscopoAtualizacao.itemCarro.Identificador, Litro: vm.EscopoAtualizacao.itemCarro.KM, Gastos: [] };
        vm.messages = [];
        vm.loggedUser = Auth.currentUser;
        vm.CamposInvalidos = {};
        vm.itemCusto = { Usuarios: [], Especie: true, IdentificadorViagem: vm.EscopoAtualizacao.itemCarro.IdentificadorViagem, ExibeHora: true, Data: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss"), strHora: moment(new Date()).format("HH:mm:ss") };

        vm.loading = false;
        vm.load = function () {

        };

        vm.salvar = function () {
            vm.messages = [];
            vm.loading = true;
            vm.CamposInvalidos = {};            
            vm.SalvarCusto();            
            vm.itemReabastecimento.Data = vm.itemCusto.Data;
            vm.itemCusto.Latitude = vm.itemReabastecimento.Latitude;
            vm.itemCusto.Longitude = vm.itemReabastecimento.Longitude;
            vm.itemReabastecimento.Gastos = [{ DataAtualizacao: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss"), ItemGasto: vm.itemCusto }];
            
            Reabastecimento.save(vm.itemReabastecimento, function (data) {
                vm.loading = false;
                if (data.Sucesso) {
                    vm.itemReabastecimento.Identificador = data.IdentificadorRegistro;
                    vm.EscopoAtualizacao.AtualizarReabastecimento(vm.itemReabastecimento);
                    vm.close();
                } else {
                    vm.messages = data.Mensagens;
                    vm.verificaCampoInvalido();
                }
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
            });

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
                vm.CamposInvalidos[item.Campo] = true;
            });
        };

      

        vm.SalvarCusto = function () {
        };

        vm.close = function () {
            
                if ($uibModalInstance)
                    $uibModalInstance.close();
            
        };

        vm.abrirMapa = function () {
            $uibModal.open({
                templateUrl: 'modalMapa.html',
                controller: ['$uibModalInstance', 'NgMap', '$timeout', '$scope', 'item', vm.MapModalCtrl],
                controllerAs: 'vmMapa',
                resolve: {
                    item: function () { return vm.itemReabastecimento; },
                }
            });
        };

        vm.MapModalCtrl = function ($uibModalInstance, NgMap, $timeout, $scope, item) {
            var vmMapa = this;
            vmMapa.lat = -23.6040963;
            vmMapa.lng = -46.6178018;
            vmMapa.itemEndereco = "";
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

            vmMapa.centralizarEndereco = function () {
                var geocoder = new google.maps.Geocoder();
                geocoder.geocode({ 'address': vmMapa.itemEndereco }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        vmMapa.lat = results[0].geometry.location.lat();
                        vmMapa.lng = results[0].geometry.location.lng();


                        vmMapa.map.setCenter(results[0].geometry.location);
                    }
                });
            };

            vmMapa.salvar = function () {
                vmMapa.itemFoto.Latitude = vmMapa.itemMarcador.Latitude;
                vmMapa.itemFoto.Longitude = vmMapa.itemMarcador.Longitude;
                $uibModalInstance.close();

            };

            vmMapa.ajustaPosicao = function (event) {
                var ll = event.latLng;
                vmMapa.itemMarcador = { Latitude: ll.lat(), Longitude: ll.lng() };

            };

            vmMapa.limparPosicao = function () {
                vmMapa.itemMarcador = {};
            };
        };

    }
}());
