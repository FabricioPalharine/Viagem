(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('CalendarioPrevistoEditCtrl',['$uibModalInstance','$uibModal', 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Viagem','CalendarioPrevisto','ItemCalendarioPrevisto','EscopoAtualizacao', CalendarioPrevistoEditCtrl]);

	function CalendarioPrevistoEditCtrl($uibModalInstance, $uibModal, Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Viagem, CalendarioPrevisto, ItemCalendarioPrevisto, EscopoAtualizacao) {
		var vm = this;
		vm.itemOriginal = ItemCalendarioPrevisto;
		vm.itemCalendarioPrevisto = jQuery.extend({}, ItemCalendarioPrevisto);
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

		    if (vm.itemCalendarioPrevisto.DataInicio) {
		        if (typeof vm.itemCalendarioPrevisto.DataInicio == "string") {

		            vm.itemCalendarioPrevisto.DataInicio = moment(vm.itemCalendarioPrevisto.DataInicio).format("YYYY-MM-DDT");
		        }
		        else
		            vm.itemCalendarioPrevisto.DataInicio = moment(vm.itemCalendarioPrevisto.DataInicio).format("YYYY-MM-DDT");
		        vm.itemCalendarioPrevisto.DataInicio += (vm.itemCalendarioPrevisto.strHoraDataInicio) ? vm.itemCalendarioPrevisto.strHoraDataInicio : "00:00:00";

		    }

		    if (vm.itemCalendarioPrevisto.DataFim) {
		        if (typeof vm.itemCalendarioPrevisto.DataFim == "string") {
		            vm.itemCalendarioPrevisto.DataFim = moment(vm.itemCalendarioPrevisto.DataFim).format("YYYY-MM-DDT");
		        }
		        else
		            vm.itemCalendarioPrevisto.DataFim = moment(vm.itemCalendarioPrevisto.DataFim).format("YYYY-MM-DDT");
		        vm.itemCalendarioPrevisto.DataFim += (vm.itemCalendarioPrevisto.strHoraDataFim) ? vm.itemCalendarioPrevisto.strHoraDataFim : "00:00:00";

		    }


		    CalendarioPrevisto.save(vm.itemCalendarioPrevisto, function (data) {
		        vm.loading = false;
		        if (data.Sucesso) {
		            Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
		            vm.itemCalendarioPrevisto.Identificador = data.IdentificadorRegistro;
		            vm.EscopoAtualizacao.AtualizarItemSalvo(vm.itemCalendarioPrevisto, vm.itemOriginal);
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
		    vm.itemCalendarioPrevisto.CodigoPlace = $item.place_id;
		    vm.itemCalendarioPrevisto.Latitude = $item.geometry.location.lat();
		    vm.itemCalendarioPrevisto.Longitude = $item.geometry.location.lng();
		};

		vm.PesquisarDadosGoogleApi = function (valor, callback, error) {
		    if (vm.itemCalendarioPrevisto.Latitude && vm.itemCalendarioPrevisto.Longitude || vm.position.lat) {
		        var posicao;
		        if (vm.itemCalendarioPrevisto.Latitude && vm.itemCalendarioPrevisto.Longitude)
		            posicao = new google.maps.LatLng(vm.itemCalendarioPrevisto.Latitude, vm.itemCalendarioPrevisto.Longitude);
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

		vm.excluir = function () {
		    vm.EscopoAtualizacao.askDelete(vm.itemCalendarioPrevisto, function () { vm.close(); })
		}

		


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
		            item: function () { return vm.itemCalendarioPrevisto; },
		        }
		    });
		};

		vm.MapModalCtrl = function ($uibModalInstance, NgMap, $timeout, $scope, item) {
		    var vmMapa = this;
		    vmMapa.lat = 0;
		    vmMapa.lng = 0;
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
