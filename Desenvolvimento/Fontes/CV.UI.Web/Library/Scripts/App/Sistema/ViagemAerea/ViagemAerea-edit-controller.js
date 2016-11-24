(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ViagemAereaEditCtrl', ['Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade', 'Viagem',  '$uibModal', '$timeout', 'Gasto', 'SignalR', 'ViagemAerea', ViagemAereaEditCtrl]);

	function ViagemAereaEditCtrl(Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Cidade, Viagem, $uibModal, $timeout, Gasto, SignalR, ViagemAerea) {
		var vm = this;
		vm.itemViagemAerea = {};
		vm.loading = false;
		vm.messages = [];
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.ListaParticipante = [];
		vm.ItemAvaliacao = {};
		vm.itemOriginal = {};
		vm.itemTipoViagem = null;
		vm.itemPontoOrigem = null;
		vm.itemPontoDestino = null;

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
		    vm.itemOriginal = vm.itemViagemAerea = item;
		    if (!item.Identificador) {
		        angular.forEach($scope.$parent.itemViagemAerea.ListaParticipantes, function (c) {
		            var item2 = jQuery.extend({}, c);
		            item2.Selecionado = true;
		            vm.ListaParticipante.push(item2);
		        });
		        vm.itemPontoOrigem  = $.grep(vm.itemViagemAerea.Aeroportos, function (e) { return e.TipoPonto == 1 && !e.DataExclusao })[0];
		        vm.itemPontoDestino = $.grep(vm.itemViagemAerea.Aeroportos, function (e) { return e.TipoPonto == 2 && !e.DataExclusao })[0];
		        vm.itemPontoOrigem.ChegadaPonto = vm.itemPontoOrigem.DataChegada != null;
		        vm.itemPontoOrigem.PartidaPonto = vm.itemPontoOrigem.DataPartida != null;
		    }
		};

		vm.load = function (itemBase) {
		    if (itemBase.Identificador && !vm.itemViagemAerea.Avaliacoes && !vm.loading) {
		        vm.loading = true;
		        ViagemAerea.get({ id: itemBase.Identificador }, function (data) {
		            vm.itemViagemAerea = data;
		            vm.itemTipoViagem = { Codigo: data.Tipo };
		            angular.forEach(vm.itemViagemAerea.Aeroportos, function (c) {
		                c.ChegadaPonto = c.DataChegada != null;
		                c.PartidaPonto = c.DataPartida != null;
		            });

		            vm.itemPontoOrigem = $.grep(vm.itemViagemAerea.Aeroportos, function (e) { return e.TipoPonto == 1 && !e.DataExclusao })[0];
		            vm.itemPontoDestino = $.grep(vm.itemViagemAerea.Aeroportos, function (e) { return e.TipoPonto == 2 && !e.DataExclusao })[0];


		            angular.forEach($scope.$parent.itemViagemAerea.ListaParticipantes, function (c) {
		                var item = jQuery.extend({}, c);
		                item.Selecionado = vm.itemViagemAerea.Avaliacoes && $.grep(vm.itemViagemAerea.Avaliacoes, function (e) {
		                    if (e.IdentificadorUsuario == item.Identificador) {
		                        if (e.IdentificadorUsuario == Auth.currentUser.Codigo) {
		                            vm.ItemAvaliacao.Nota = e.Nota;
		                            vm.ItemAvaliacao.Comentario = e.Comentario;
		                        }
		                    }
		                    return e.IdentificadorUsuario == item.Identificador;
		                }).length > 0;
		                vm.ListaParticipante.push(item);
		            });

		            vm.loading = false;
		        });

		    }
		};

		vm.AdicionarEscala = function () {
		    var itemPonto = { TipoPonto: 3 };
		    vm.itemViagemAerea.Aeroportos.push(itemPonto);
		};

		vm.ExcluirEscala = function (itemEscala) {
		    $scope.$parent.itemViagemAerea.modalPopupTrigger(itemEscala, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
		        itemEscala.DataExclusao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		    });
		};

		vm.AjustarHoraChegada = function (itemEscala) {
		    if (itemEscala.ChegadaPonto)
		    {
		        itemEscala.DataChegada = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		        itemEscala.strHoraChegada = moment(new Date()).format("HH:mm:ss");
		    }
		    else
		    {
		        itemEscala.DataChegada = itemEscala.strHoraChegada = itemEscala.DataPartida = itemEscala.strHoraPartida = null;
		        itemEscala.PartidaPonto = false;
		    }
		};

		vm.AjustarHoraPartida = function (itemEscala) {
		    if (itemEscala.PartidaPonto) {
		        itemEscala.DataPartida = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		        itemEscala.strHoraPartida = moment(new Date()).format("HH:mm:ss");
		    }
		    else {
		        itemEscala.DataPartida = itemEscala.strHoraPartida = null;

		    }
		};

		vm.save = function () {
		    vm.messages = [];
		    vm.submitted = true;
		    vm.loading = true;
		    vm.CamposInvalidos = {};
		    {
		        if (vm.itemViagemAerea.DataPrevista) {
		            if (typeof vm.itemViagemAerea.DataPrevista == "string") {

		                vm.itemViagemAerea.DataPrevista = moment(vm.itemViagemAerea.DataPrevista).format("YYYY-MM-DDTHH:mm:ss");
		            }
		            else
		                vm.itemViagemAerea.DataPrevista = moment(vm.itemViagemAerea.DataPrevista).format("YYYY-MM-DDTHH:mm:ss");

		        }

		        if (vm.itemTipoViagem && vm.itemTipoViagem.Codigo)
		            vm.itemViagemAerea.Tipo = vm.itemTipoViagem.Codigo;
		        else
		            vm.itemViagemAerea.Tipo = null;
		      
		        angular.forEach(vm.ListaParticipante, function (item) {
		            var itens =
                         $.grep(vm.itemViagemAerea.Avaliacoes, function (e) { return e.IdentificadorUsuario == item.Identificador && !e.DataExclusao });
		            if (item.Selecionado && itens.length == 0) {
		                var NovoItem = { IdentificadorUsuario: item.Identificador, DataAtualizacao: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss") }
		                vm.itemViagemAerea.Avaliacoes.push(NovoItem);
		            }
		            else if (!item.Selecionado && itens.length > 0) {

		                item.DataExclusao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		            }

		        });

		        if (vm.itemViagemAerea.Nome == "")
		            vm.itemViagemAerea.Nome = null;
		      
		        angular.forEach(vm.itemViagemAerea.Gastos, function (item) {
		            if (item.ItemGasto != null)
		                item.ItemGasto.ViagenAereas = null;
		        });

		        var possuiPontoSemNome = false;
		        angular.forEach(vm.itemViagemAerea.Aeroportos, function (item) {
		            if (!item.Aeroporto) {
		                item.CamposInvalidos = { Aeroporto: true }
		                possuiPontoSemNome = true;
		            }
		            else
		                item.CamposInvalidos = {};


		            if (item.DataChegada) {
		                if (typeof item.DataChegada == "string") {

		                    item.DataChegada = moment(item.DataChegada).format("YYYY-MM-DDT");
		                }
		                else
		                    item.DataChegada = moment(item.DataChegada).format("YYYY-MM-DDT");
		                item.DataChegada += (item.strHoraChegada) ? item.strHoraChegada : "00:00:00";

		            }

		            if (item.DataPartida) {
		                if (typeof item.DataPartida == "string") {
		                    item.DataPartida = moment(item.DataPartida).format("YYYY-MM-DDT");
		                }
		                else
		                    item.DataPartida = moment(item.DataPartida).format("YYYY-MM-DDT");
		                item.DataPartida += (item.strHoraPartida) ? item.strHoraPartida : "00:00:00";

		            }

		        });

		       

		        var MinhaAvaliacao =
                         $.grep(vm.itemViagemAerea.Avaliacoes, function (e) { return e.IdentificadorUsuario == Auth.currentUser.Codigo && !e.DataExclusao });
		        if (MinhaAvaliacao.length > 0) {
		            MinhaAvaliacao[0].DataAtualizacao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		            MinhaAvaliacao[0].Comentario = vm.ItemAvaliacao.Comentario;
		            MinhaAvaliacao[0].Nota = vm.ItemAvaliacao.Nota;
		        }
		      
		        if (possuiPontoSemNome) {
		            vm.messages = [];
		            vm.messages.push({ Mensagem: $translate.instant("ViagemAerea_AeroportoObrigatorio") });
		            vm.loading = false;
		        }
		        else {
		            ViagemAerea.save(vm.itemViagemAerea, function (data) {
		                vm.loading = false;
		                if (data.Sucesso) {
		                    Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
		                    $scope.$parent.itemViagemAerea.AjustarViagemAereaSalva(vm.itemOriginal, data.ItemRegistro);
		                } else {
		                    vm.messages = data.Mensagens;
		                    vm.verificaCampoInvalido();
		                }
		            }, function (err) {
		                vm.loading = false;
		                Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
		            });
		        }
		    }
		    vm.submitted = false;
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


		vm.CarregarDadosGoogleApiPromise = function (valor, itemViagemAerea) {
		    var d = $.Deferred();
		    vm.PesquisarDadosGoogleApi(valor,itemViagemAerea, function (res) { d.resolve(res); }, function (err) { d.reject(err); });
		    return d.promise();
		}

		vm.CarregarDadosGoogleApi = function (valor, itemViagemAerea) {
		    return vm.CarregarDadosGoogleApiPromise(valor, itemViagemAerea)
            .then(function (response) {
                return response;
            });
		};

		vm.SelecionarPlacesGoogle = function ($item, $model, $label, itemAeroporto) {
		    itemAeroporto.CodigoPlace = $item.place_id;
		    itemAeroporto.Latitude = $item.geometry.location.lat();
		    itemAeroporto.Longitude = $item.geometry.location.lng();
		};

		vm.PesquisarDadosGoogleApi = function (valor, itemViagemAerea, callback, error) {
		    if (itemViagemAerea.Latitude && itemViagemAerea.Longitude || vm.position.lat) {
		        var posicao;
		        if (itemViagemAerea.Latitude && itemViagemAerea.Longitude)
		            posicao = new google.maps.LatLng(itemViagemAerea.Latitude, itemViagemAerea.Longitude);
		        else
		            posicao = new google.maps.LatLng(vm.position.lat, vm.position.lng);
		        var request = {
		            location: posicao,
		            radius: '2500',
		            name: valor,
		            types: ['airport', 'port', 'bus_station','train_station','taxi_stand']
		        };
		        var service = new google.maps.places.PlacesService(document.createElement('div'));
		        service.nearbySearch(request, function (results, status) {
		            if (status == google.maps.places.PlacesServiceStatus.OK) {

		                callback(results);
		            }
		        });
		    }

		};



		vm.VerificarParticipoViagemAerea = function () {
		    return $.grep(vm.ListaParticipante, function (e) {

		        return e.Identificador == Auth.currentUser.Codigo && e.Selecionado;
		    }).length > 0;
		};

		vm.RemoverCusto = function (itemCusto) {
		    $scope.$parent.itemViagemAerea.modalPopupTrigger(itemCusto, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
		        itemCusto.DataExclusao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		        Gasto.SalvarCustoViagemAerea(itemCusto);
		    });

		};


		vm.AtualizarGasto = function (itemGasto, itemOriginal) {
		    var itemPush = itemGasto.ViagenAereas[0];
		    itemPush.ItemGasto = itemGasto;
		    vm.itemViagemAerea.Gastos.push(itemPush);
		    SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'G', itemGasto.Identificador, true);
		};


		vm.AbrirGasto = function () {
		    $scope.$parent.itemViagemAerea.modalPopupTrigger(vm.itemViagemAerea, $translate.instant('MensagemGastoAdicionar'), $translate.instant('Novo'), $translate.instant('Existente'), function () {
		        var Referencias = { IdentificadorViagemAerea: vm.itemViagemAerea.Identificador };
		        $uibModal.open({
		            templateUrl: 'EditaCusto.html',
		            controller: 'GastoEditCtrl',
		            controllerAs: 'itemGastoEdit',
		            resolve: {
		                Referencias: function () { return Referencias; },
		                EscopoAtualizacao: vm
		            }
		        });
		    }, function () {
		        $uibModal.open({
		            templateUrl: 'Sistema/GastoSelecao',
		            controller: ['$uibModalInstance', vm.SelecionaCustoCtrl],
		            controllerAs: 'vmSelecaoGasto',
		            resolve: {

		            }
		        });
		    });
		};


		vm.VerificarExclusao = function () {
		    $scope.$parent.itemViagemAerea.modalPopupTrigger(vm.itemViagemAerea, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
		        $scope.$parent.itemViagemAerea.Excluir(vm.itemOriginal)
		    });
		};

		vm.SelecionarPosicao = function (itemAeroporto) {
		    $uibModal.open({
		        templateUrl: 'modalMapa.html',
		        controller: ['$uibModalInstance', 'NgMap', '$timeout', '$scope', 'item', vm.MapModalCtrl],
		        controllerAs: 'vmMapa',
		        resolve: {
		            item: function () { return itemAeroporto; },
		        }
		    });
		};


		vm.SelecionaCustoCtrl = function ($uibModalInstance) {
		    var vmSelecao = this;
		    vmSelecao.ListaParticipante = vm.ListaParticipante;
		    vmSelecao.filtro = {};
		    vmSelecao.itemUsuario = null;
		    vmSelecao.ListaGastos = [];

		    vmSelecao.Idioma = function () {
		        if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
		            return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
		        else
		            return "pt";
		    };
		    vmSelecao.filtraDado = function () {
		        if (vmSelecao.itemUsuario && vmSelecao.itemUsuario.Identificador)
		            vmSelecao.filtro.IdentificadorUsuario = vmSelecao.itemUsuario.Identificador;
		        else
		            vmSelecao.filtro.IdentificadorUsuario = null;

		        if (vmSelecao.filtro.DataInicioDe) {
		            if (typeof vmSelecao.filtro.DataInicioDe == "string") {

		            }
		            else
		                vmSelecao.filtro.DataInicioDe = $.datepicker.formatDate("yy-mm-ddT00:00:00", vmSelecao.filtro.DataInicioDe);
		        }

		        if (vmSelecao.filtro.DataInicioAte) {
		            if (typeof vmSelecao.filtro.DataInicioAte == "string") {

		            }
		            else
		                vmSelecao.filtro.DataInicioAte = $.datepicker.formatDate("yy-mm-ddT00:00:00", vmSelecao.filtro.DataInicioAte);
		        }

		        Gasto.list({ json: JSON.stringify(vmSelecao.filtro) }, function (data) {
		            vmSelecao.ListaGastos = data.Lista;
		        });
		    };

		    vmSelecao.SelecionarCusto = function (itemCusto) {
		        var itemGravar = { IdentificadorViagemAerea: vm.itemViagemAerea.Identificador, IdentificadorGasto: itemCusto.Identificador, DataAtualizacao: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss") };
		        Gasto.SalvarCustoViagemAerea(itemGravar, function (data) {
		            if (data.Sucesso) {
		                var itemPush = { Identificador: data.IdentificadorRegistro, ItemGasto: itemCusto };
		                vm.itemViagemAerea.Gastos.push(itemPush);
		            }
		            $uibModalInstance.close();
		        });
		    };
		}

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
