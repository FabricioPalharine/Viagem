(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('VerificarSugestaoEditCtrl', ['$uibModalInstance', '$uibModal', 'Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade', 'Usuario', 'Viagem', 'Sugestao', 'ItemSugestao', 'EscopoAtualizacao','SignalR', VerificarSugestaoEditCtrl]);

    function VerificarSugestaoEditCtrl($uibModalInstance, $uibModal, Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Cidade, Usuario, Viagem, Sugestao, ItemSugestao, EscopoAtualizacao, SignalR) {
        var vm = this;
        vm.itemOriginal = ItemSugestao;
        vm.itemSugestao = jQuery.extend({}, ItemSugestao);
        vm.itemAgendamento = { Prioridade: 2, AvisarHorario : false}
        vm.messages = [];
        vm.loggedUser = Auth.currentUser;
        vm.CamposInvalidos = {};
        vm.EscopoAtualizacao = EscopoAtualizacao;
        vm.load = function () {
        };


        vm.save = function () {
            vm.messages = [];
            vm.submitted = true;
            vm.CamposInvalidos = {};

            if (vm.itemCalendario.DataInicio) {
                if (typeof vm.itemCalendario.DataInicio == "string") {

                    vm.itemCalendario.DataInicio = moment(vm.itemCalendario.DataInicio).format("YYYY-MM-DDT");
                }
                else
                    vm.itemCalendario.DataInicio = moment(vm.itemCalendario.DataInicio).format("YYYY-MM-DDT");
                vm.itemCalendario.DataInicio += (vm.itemCalendario.strHoraDataInicio) ? vm.itemCalendario.strHoraDataInicio : "00:00:00";

            }

            if (vm.itemCalendario.DataFim) {
                if (typeof vm.itemCalendario.DataFim == "string") {
                    vm.itemCalendario.DataFim = moment(vm.itemCalendario.DataFim).format("YYYY-MM-DDT");
                }
                else
                    vm.itemCalendario.DataFim = moment(vm.itemCalendario.DataFim).format("YYYY-MM-DDT");
                vm.itemCalendario.DataFim += (vm.itemCalendario.strHoraDataFim) ? vm.itemCalendario.strHoraDataFim : "00:00:00";

            }

            Sugestao.AgendarSugestao({ itemSugestao: vm.itemSugestao, itemCalendario: vm.itemAgendamento}, function (data) {
                vm.loading = false;
                if (data.Sucesso) {
                    Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
                    vm.itemSugestao.Identificador = data.IdentificadorRegistro;
                    SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'CP', data.IdentificadorRegistro, true);
                    SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'S', vm.itemSugestao.Identificador, false);

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


        vm.close = function () {

            if ($uibModalInstance)
                $uibModalInstance.close();

        };

        vm.confirmarIgnore = function () {
            vm.EscopoAtualizacao.askDelete(vm.itemSugestao, function () { vm.close(); })
        };

        vm.abrirMapa = function () {
            $uibModal.open({
                templateUrl: 'modalMapa.html',
                controller: ['$uibModalInstance', 'NgMap', '$timeout', '$scope', 'item', vm.MapModalCtrl],
                controllerAs: 'vmMapa',
                resolve: {
                    item: function () { return vm.itemSugestao; },
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


        };

    }
}());
