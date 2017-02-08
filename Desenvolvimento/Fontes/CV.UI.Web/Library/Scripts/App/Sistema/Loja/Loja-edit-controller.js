(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('LojaEditCtrl', ['Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade', 'Viagem', 'Atracao',  '$uibModal', '$timeout', 'Gasto', 'SignalR', 'Loja', LojaEditCtrl]);

	function LojaEditCtrl(  Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Cidade, Viagem, Atracao, $uibModal, $timeout, Gasto, SignalR,Loja) {
		var vm = this;
		vm.itemLoja = {};
		vm.loading = false;
		vm.messages = [];
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.ListaParticipante = [];
		vm.ItemAvaliacao = {};
		vm.itemOriginal = {};


		vm.position = null;
		vm.AjustarPosicao = function (position) {
		    vm.position = {};
		    vm.position.lat = position.coords.latitude;
		    vm.position.lng = position.coords.longitude;
		};

		if (navigator.geolocation) {
		    navigator.geolocation.getCurrentPosition(vm.AjustarPosicao);
		}

		vm.ajustaInicio = function (item) {
		    vm.itemOriginal = vm.itemLoja = item;
		    if (!item.Identificador) {
		        angular.forEach($scope.$parent.itemLoja.ListaParticipantes, function (c) {
		            var item2 = jQuery.extend({}, c);
		            item2.Selecionado = true;
		            vm.ListaParticipante.push(item2);
		        });
		    }
		};

		vm.load = function (itemBase) {
		    if (itemBase.Identificador && !vm.itemLoja.Avaliacoes && !vm.loading) {
		        vm.loading = true;
		        Loja.get({ id: itemBase.Identificador }, function (data) {
		            vm.itemLoja = data;
		            angular.forEach($scope.$parent.itemLoja.ListaParticipantes, function (c) {
		                var item = jQuery.extend({}, c);
		                item.Selecionado = vm.itemLoja.Avaliacoes && $.grep(vm.itemLoja.Avaliacoes, function (e) {
		                    if (e.IdentificadorUsuario == item.Identificador) {
		                        if (e.IdentificadorUsuario == Auth.currentUser.Codigo) {
		                            vm.ItemAvaliacao.Nota = e.Nota;
		                            vm.ItemAvaliacao.Comentario = e.Comentario;
		                        }
		                        item.Pedido = e.Pedido;
		                    }
		                    return e.IdentificadorUsuario == item.Identificador;
		                }).length > 0;
		                vm.ListaParticipante.push(item);
		            });
		            vm.loading = false;
		        });

		    }
		};

		vm.save = function () {
		    vm.messages = [];
		    vm.submitted = true;
		    vm.loading = true;
		    vm.CamposInvalidos = {};
		    {

		        if (vm.itemLoja.ItemAtracao && vm.itemLoja.ItemAtracao.Identificador)
		            vm.itemLoja.IdentificadorAtracao = vm.itemLoja.ItemAtracao.Identificador
		        else {
		            vm.itemLoja.IdentificadorAtracao = null;
		            vm.itemLoja.ItemAtracao = null;
		        }

		    


		        angular.forEach(vm.ListaParticipante, function (item) {
		            var itens =
                         $.grep(vm.itemLoja.Avaliacoes, function (e) { return e.IdentificadorUsuario == item.Identificador && !e.DataExclusao });
		            if (item.Selecionado && itens.length == 0) {
		                var NovoItem = { IdentificadorUsuario: item.Identificador, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss"), Pedido: item.Pedido }
		                vm.itemLoja.Avaliacoes.push(NovoItem);
		            }
		            else if (!item.Selecionado && itens.length > 0) {

		                item.DataExclusao = moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		            }

		        });

		        if (vm.itemLoja.Nome == "")
		            vm.itemLoja.Nome = null;

		        var MinhaAvaliacao =
                         $.grep(vm.itemLoja.Avaliacoes, function (e) { return e.IdentificadorUsuario == Auth.currentUser.Codigo && !e.DataExclusao });
		        if (MinhaAvaliacao.length > 0) {
		            MinhaAvaliacao[0].DataAtualizacao = moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		            MinhaAvaliacao[0].Comentario = vm.ItemAvaliacao.Comentario;
		            MinhaAvaliacao[0].Nota = vm.ItemAvaliacao.Nota;
		        }
		       
		        angular.forEach(vm.itemLoja.Compras, function (item) {
		            if (item.ItemGasto != null) {
		                item.ItemGasto.Compras = null;
		            }
		            if (item.ItensComprados)
		            {
		                angular.forEach(item.ItensComprados, function (itemCompra) {
		                    itemCompra.ItemGastoCompra = null;
		                    if (itemCompra.Fotos)
		                    {
		                        angular.forEach(itemCompra.Fotos, function (itemFoto) {
		                            itemFoto.ItemItemCompra = null;
		                            if (itemFoto.ItemFoto != null)
		                                temFoto.ItemFoto.ItensCompra = null;
		                        });
		                    }
		                   
		                });
		            }
		        });
		        Loja.save(vm.itemLoja, function (data) {
		            vm.loading = false;
		            if (data.Sucesso) {
		                Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
		                $scope.$parent.itemLoja.AjustarLojaSalva(vm.itemOriginal, data.ItemRegistro);
		            } else {
		                vm.messages = data.Mensagens;
		                vm.verificaCampoInvalido();
		            }
		        }, function (err) {
		            vm.loading = false;
		            Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
		        });
		    }
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
		    vm.itemLoja.CodigoPlace = $item.place_id;

		    vm.itemLoja.Latitude = $item.geometry.location.lat();
		    vm.itemLoja.Longitude = $item.geometry.location.lng();

		    var request = {
		        placeId: $item.place_id
		    };
		    if (!vm.itemLoja.Tipo) {
		        var service = new google.maps.places.PlacesService(document.createElement('div'));
		        service.getDetails(request, function (place, status) {
		            if (status == google.maps.places.PlacesServiceStatus.OK) {

		            }
		        });
		    }
		};

		vm.PesquisarDadosGoogleApi = function (valor, callback, error) {
		    if (vm.itemLoja.Latitude && vm.itemLoja.Longitude || vm.position.lat) {
		        var posicao;
		        if (vm.itemLoja.Latitude && vm.itemLoja.Longitude)
		            posicao = new google.maps.LatLng(vm.itemLoja.Latitude, vm.itemLoja.Longitude);
		        else
		            posicao = new google.maps.LatLng(vm.position.lat, vm.position.lng);
		        var request = {
		            location: posicao,
		            radius: '2500',
		            name: valor,
		            types: ['art_gallery', 'bicycle_store', 'book_store', 'car_dealer', 'clothing_store', 'liquor_store', 'convenience_store', 'department_store', 'electronics_store', 'florist','furniture_store','grocery_or_supermarket','hardware_store','store','shopping_mall','shoe_store','pharmacy','pet_store','jewelry_store','home_goods_store']
		        };
		        var service = new google.maps.places.PlacesService(document.createElement('div'));
		        service.nearbySearch(request, function (results, status) {
		            if (status == google.maps.places.PlacesServiceStatus.OK) {

		                callback(results);
		            }
		        });
		    }
		};

		vm.ListaAtracao = function () {
		    return $scope.$parent.itemLoja.ListaAtracao;
		};



		vm.VerificarVisitoLoja = function () {
		    return $.grep(vm.ListaParticipante, function (e) {

		        return e.Identificador == Auth.currentUser.Codigo && e.Selecionado;
		    }).length > 0;
		};

		vm.RemoverCusto = function (itemCusto) {
		    $scope.$parent.itemLoja.modalPopupTrigger(itemCusto, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
		        itemCusto.DataExclusao = moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		        Loja.excluirCompra(itemCusto);
		        SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'GL', itemCusto.Identificador, false);

		    });

		};


		vm.AtualizarCompra = function (itemOriginal, itemGasto) {
		    if (itemOriginal.Identificador)
		    {
		        var Posicao = vm.itemLoja.Compras.indexOf(itemOriginal);
		        vm.itemLoja.Compras.splice(Posicao, 1, itemGasto);
		    }
		    else
		    {
		        vm.itemLoja.Compras.push(itemGasto);
		    }
		    //SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'G', itemGasto.Identificador, true);
		};

		vm.AdicionarCompra = function () {
		    var itemCusto = { ItensCompra: [], IdentificadorLoja: vm.itemLoja.Identificador, ItemGasto: { Usuarios: [], Especie: true, IdentificadorViagem: vm.itemLoja.IdentificadorViagem, ExibeHora: true, Data: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss"), strHora: moment(new Date()).format("HH:mm:ss"), ItemUsuario: { Identificador: Auth.currentUser.Codigo }, IdentificadorUsuario: Auth.currentUser.Codigo, Dividido: false,ApenasBaixa: false }, Usuarios: [] };
		    $uibModal.open({
		        templateUrl: 'Sistema/ComprasEdicao',
		        controller: 'CompraEditCtrl',
		        controllerAs: 'itemCompraEdit',
		        resolve: {
		            ItemGastoCompra: function () { return itemCusto; },
		            EscopoAtualizacao: vm
		        }
		    });
		};

		vm.EditarCompra = function (itemCusto) {
		    itemCusto.ItemGasto.ExibeHora = true;
		    $uibModal.open({
		        templateUrl: 'Sistema/ComprasEdicao',
		        controller: 'CompraEditCtrl',
		        controllerAs: 'itemCompraEdit',
		       
		        resolve: {
		            ItemGastoCompra: function () { return itemCusto; },
		            EscopoAtualizacao: vm
		        }
		    });
		};
	
	
		vm.VerificarExclusao = function () {
		    $scope.$parent.itemLoja.modalPopupTrigger(vm.itemLoja, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
		        $scope.$parent.itemLoja.Excluir(vm.itemLoja)
		    });
		};

		vm.Cancelar = function () {
		    $scope.$parent.itemLoja.Cancelar(vm.itemLoja);
		};

		vm.ChamarExclusao = function (item, callback1, callback2) {
		    $scope.$parent.itemLoja.modalPopupTrigger(item, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), callback1, callback2);
		};
	

		vm.SelecionarPosicao = function () {
		    $uibModal.open({
		        templateUrl: 'modalMapa.html',
		        controller: ['$uibModalInstance', 'NgMap', '$timeout', '$scope', 'item', vm.MapModalCtrl],
		        controllerAs: 'vmMapa',
		        resolve: {
		            item: function () { return vm.itemLoja; },
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
