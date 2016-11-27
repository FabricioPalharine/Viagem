(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('SugestaoEditCtrl',[ '$uibModalInstance','$uibModal','Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade','Usuario','Viagem','Sugestao','ItemSugestao','EscopoAtualizacao', SugestaoEditCtrl]);

	function SugestaoEditCtrl($uibModalInstance,$uibModal, Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Cidade, Usuario, Viagem, Sugestao, ItemSugestao, EscopoAtualizacao) {
		var vm = this;
		vm.itemOriginal = ItemSugestao;
		vm.itemSugestao = jQuery.extend({}, ItemSugestao);
		vm.messages = [];
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.EscopoAtualizacao = EscopoAtualizacao;
		vm.load = function () {
			
		};

		vm.position = null;


		vm.AjustarPosicao = function (position) {
		    vm.position = {};
		    vm.position.lat = position.coords.latitude;
		    vm.position.lng = position.coords.longitude;
		};

		if (navigator.geolocation) {
		    navigator.geolocation.getCurrentPosition(vm.AjustarPosicao);
		}

		vm.save = function () {
			vm.messages = [];
			vm.submitted = true;
			vm.CamposInvalidos = {};

				

				Sugestao.save(vm.itemSugestao, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
					    Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
					    vm.itemSugestao.Identificador = data.IdentificadorRegistro;
					    vm.EscopoAtualizacao.AtualizarItemSalvo(vm.itemSugestao, vm.itemOriginal);
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

        vm.CarregarDadosGoogleApiPromise = function (valor) {
            var d = $.Deferred();
            vm.PesquisarDadosGoogleApi(valor, function (res) { d.resolve(res); }, function (err) { d.reject(err); });
            return d.promise();
        }

        vm.CarregarDadosGoogleApi = function (valor) {
            return vm.CarregarDadosGoogleApiPromise(valor)
            .then(function (response) {
                return response;
            });
        };

        vm.SelecionarPlacesGoogle = function ($item, $model, $label) {
            vm.itemSugestao.CodigoPlace = $item.place_id;
            vm.itemSugestao.Latitude = $item.geometry.location.lat();
            vm.itemSugestao.Longitude = $item.geometry.location.lng();
        };

        vm.PesquisarDadosGoogleApi = function (valor, callback, error) {
            if (vm.itemSugestao.Latitude && vm.itemSugestao.Longitude || vm.position.lat) {
                var posicao;
                if (vm.itemSugestao.Latitude && vm.itemSugestao.Longitude)
                    posicao = new google.maps.LatLng(vm.itemSugestao.Latitude, vm.itemSugestao.Longitude);
                else
                    posicao = new google.maps.LatLng(vm.position.lat, vm.position.lng);

                var request = {
                    location: posicao,
                    radius: '2500',
                    name: valor
                };
                var service = new google.maps.places.PlacesService(document.createElement('div'));
                service.nearbySearch(request, function (results, status) {
                    if (status == google.maps.places.PlacesServiceStatus.OK) {

                        callback(results);
                    }
                });
            }




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


            vmMapa.AjustarPosicao = function (position) {
                vmMapa.lat = position.coords.latitude;
                vmMapa.lng = position.coords.longitude;
            };

            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(vmMapa.AjustarPosicao);
            }


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


            vmMapa.selecionarEndereco = function () {
                var place = this.getPlace();
                vmMapa.itemEndereco = place.formatted_address;

                vmMapa.lat = place.geometry.location.lat();
                vmMapa.lng = place.geometry.location.lng();


                vmMapa.map.setCenter(place.geometry.location);

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
