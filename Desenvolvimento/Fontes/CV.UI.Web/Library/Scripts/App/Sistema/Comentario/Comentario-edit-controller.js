(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ComentarioEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade','Viagem','Comentario','$uibModal', ComentarioEditCtrl]);

	function ComentarioEditCtrl(Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Cidade, Viagem, Comentario, $uibModal) {
		var vm = this;
		vm.itemComentario = {};
		vm.loading = false;
		vm.messages = [];
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.ListaParticipante = [];
		vm.ItemAvaliacao = {};
		vm.itemOriginal = {};

		vm.ajustaInicio = function (item) {
		    vm.itemOriginal = item;
		    vm.itemComentario = jQuery.extend({}, item);		   
		};
	
		vm.save = function () {
		    vm.messages = [];
		    vm.submitted = true;
		    vm.loading = true;
		    vm.CamposInvalidos = {};
		    

		       
		        if (vm.itemComentario.Data) {
		            if (typeof vm.itemComentario.Data == "string") {

		                vm.itemComentario.Data = moment(vm.itemComentario.Data).format("YYYY-MM-DDT");
		            }
		            else
		                vm.itemComentario.Data = moment(vm.itemComentario.Data).format("YYYY-MM-DDT");
		            vm.itemComentario.Data += (vm.itemComentario.strHora) ? vm.itemComentario.strHora : "00:00:00";

		        }


		        Comentario.save(vm.itemComentario, function (data) {
		            vm.loading = false;
		            if (data.Sucesso) {
		                Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
		                vm.itemComentario.Identificador = data.IdentificadorRegistro;
		                $scope.$parent.itemComentario.AjustarComentarioSalvo(vm.itemOriginal, vm.itemComentario);
		            } else {
		                vm.messages = data.Mensagens;
		                vm.verificaCampoInvalido();
		            }
		        }, function (err) {
		            vm.loading = false;
		            Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
		        });
		    
		};

		vm.Idioma = function () {
		    if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
		        return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
		    else
		        return "pt";
		};

		vm.verificaCampoInvalido = function () {
		    vm.CamposInvalidos = {

		    };
		    //  var _retorno = false;
		    $(vm.messages).each(function (i, item) {
		        vm.CamposInvalidos[item.Campo] = true;
		    });
		};


		vm.VerificarExclusao = function () {
		    $scope.$parent.itemComentario.modalPopupTrigger(vm.itemComentario, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
		        $scope.$parent.itemComentario.Excluir(vm.itemComentario)
		    });
		};




		vm.SelecionarPosicao = function () {
		    $uibModal.open({
		        templateUrl: 'modalMapa.html',
		        controller: ['$uibModalInstance', 'NgMap', '$timeout', '$scope', 'item', vm.MapModalCtrl],
		        controllerAs: 'vmMapa',
		        resolve: {
		            item: function () { return vm.itemComentario; },
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

		    vmMapa.AjustarPosicao = function (position) {
		        vmMapa.lat = position.coords.latitude;
		        vmMapa.lng = position.coords.longitude;
		    };

		    if (navigator.geolocation) {
		        navigator.geolocation.getCurrentPosition(vmMapa.AjustarPosicao);
		    }

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
