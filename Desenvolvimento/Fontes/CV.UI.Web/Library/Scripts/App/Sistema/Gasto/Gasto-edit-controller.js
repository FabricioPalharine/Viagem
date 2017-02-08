(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('GastoEditCtrl',[ '$uibModalInstance', 'Error', '$uibModal', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams','Viagem','Gasto', 'Dominio','Referencias','EscopoAtualizacao','SignalR', GastoEditCtrl]);

	function GastoEditCtrl($uibModalInstance, Error,$uibModal, $state, $translate, $scope, Auth, $rootScope, $stateParams, Viagem, Gasto, Dominio, Referencias, EscopoAtualizacao,SignalR) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemGasto = {
		    Dividido: false, Data: moment(new Date()).format("YYYY-MM-DD"), Especie: true, IdentificadorUsuario: Auth.currentUser.Codigo, ApenasBaixa: false, Atracoes: [], Hoteis: [],
		    Compras: [], Alugueis: [], Refeicoes: [], ViagenAereas: [], Usuarios: [], Reabastecimentos: [] 
		};
		vm.itemGastoOriginal = {};
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.ListaUsuario = [];
		vm.ListaMoeda = [];
		vm.itemMoeda = {};
		vm.EscopoAtualizacao = EscopoAtualizacao;
		vm.ExibeHora = false;
		vm.load = function () {
			vm.loading = true;		
			
		

			if (vm.itemGasto.Identificador)
			{
			    Gasto.get({id:  vm.itemGasto.Identificador }, function (data) {
			        vm.itemGasto = data;
			        vm.PreencherLoadItem();
			    });
			}
			else
			    vm.PreencherLoadItem();

			EscopoAtualizacao.SalvarCusto = function () {
			  
			    if (vm.itemMoeda && vm.itemMoeda.Codigo)
			        vm.itemGasto.Moeda = vm.itemMoeda.Codigo;
			    else
			        vm.itemGasto.Moeda = null;


			    if (vm.itemGasto.Data) {
			        vm.itemGasto.Data = moment(vm.itemGasto.Data).format("YYYY-MM-DDT");
			        vm.itemGasto.Data += (vm.itemGasto.strHora) ? vm.itemGasto.strHora : "00:00:00";

			    }

			    return vm.itemGasto;
			};
			
		};

		vm.ajustaItem = function (item) {
		    vm.itemGastoOriginal = item;
		    vm.itemGasto = jQuery.extend({}, item);
		    vm.EscopoAtualizacao = $scope.$parent.itemGasto;


		    Viagem.get({ id: Auth.currentUser.IdentificadorViagem }, function (data) {
		        vm.itemMoeda.Codigo = data.Moeda;
		    });

		    Dominio.CarregaMoedas(function (data) {
		        vm.ListaMoeda = data;
		    });

		    if (item.ExibeHora)
		        vm.ExibeHora = true;
		};

		vm.PreencherLoadItem = function()
		{

		    if (vm.itemGasto.Moeda)
		        vm.itemMoeda = { Codigo: vm.itemGasto.Moeda };

		    Viagem.CarregarParticipantes(function (lista) {
		        vm.ListaUsuario = lista;
		        angular.forEach(vm.ListaUsuario, function (c) {
		            vm.Selecionado = ($.grep(vm.itemGasto.Usuarios, function (e) { return e.IdentificadorUsuario == c.Identificador; }).length > 0);

		        });
		        vm.loading = false;
		    });

		    if (Referencias) {
		        if (Referencias.IdentificadorAtracao) {
		            var item = { IdentificadorAtracao: Referencias.IdentificadorAtracao, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") }
		            vm.itemGasto.Atracoes.push(item);
		        }
		        if (Referencias.IdentificadorHotel) {
		            var item = { IdentificadorHotel: Referencias.IdentificadorHotel, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") }
		            vm.itemGasto.Hoteis.push(item);
		        }
		        if (Referencias.IdentificadorRefeicao) {
		            var item = { IdentificadorRefeicao: Referencias.IdentificadorRefeicao, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") }
		            vm.itemGasto.Refeicoes.push(item);
		        }
		        if (Referencias.IdentificadorCarro) {
		            var item = { IdentificadorCarro: Referencias.IdentificadorCarro, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") }
		            vm.itemGasto.Alugueis.push(item);
		        }
		        if (Referencias.IdentificadorViagemAerea) {
		            var item = { IdentificadorViagemAerea: Referencias.IdentificadorViagemAerea, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") }
		            vm.itemGasto.ViagenAereas.push(item);
		        }
		        if (Referencias.IdentificadorReabastecimento) {
		            var item = { IdentificadorReabastecimento: Referencias.IdentificadorReabastecimento, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") }
		            vm.itemGasto.Reabastecimentos.push(item);
		        }
		    }

		}

		vm.salvar = function () {
			vm.messages = [];
			vm.submitted = true;
			vm.CamposInvalidos = {};
			vm.loading = true;
				
			

				if (vm.itemMoeda && vm.itemMoeda.Codigo)
				    vm.itemGasto.Moeda = vm.itemMoeda.Codigo;
				else
				    vm.itemGasto.Moeda = null;


				if (vm.itemGasto.Data) {
				    if (vm.itemGasto.Data) {
				        if (typeof vm.itemGasto.Data == "string") {
				           
				        }
				        else
				            vm.itemGasto.Data =  moment( vm.itemGasto.Data).format("YYYY-MM-DDTHH:mm:ss")  ;
				    }
				}

				if (vm.itemGasto.DataPagamento && !vm.itemGasto.Especie && vm.itemGasto.Moeda != 790) {

				    if (typeof vm.itemGasto.DataPagamento == "string") {
				      
				    }
				    else
				        vm.itemGasto.DataPagamento = moment(vm.itemGasto.DataPagamento).format("YYYY-MM-DDTHH:mm:ss");


				}
				else
				    vm.itemGasto.DataPagamento = null;

				if (vm.itemGasto.Dividido)
				{
				    angular.forEach(vm.ListaUsuario, function (c) {
				        var item = $.grep(vm.itemGasto.Usuarios, function (e) { return e.IdentificadorUsuario == c.Identificador; });
				        if (c.Selecionado && c.Identificador != vm.itemGasto.IdentificadorUsuario) {
				            if (item.length == 0)
				                vm.itemGasto.Usuarios.push({ IdentificadorUsuario: c.Identificador, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") })
				        }
				        else if (item.length > 0) {
				            var posicao = vm.itemGasto.Usuarios.indexOf(item[0]);
				            vm.itemGasto.Usuarios.splice(posicao, 1);
				        };
				    });
				}
				else
				{
				    vm.itemGasto.Usuarios = [];
				    
				}

				Gasto.save(vm.itemGasto, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
					    SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'G', data.IdentificadorRegistro, vm.itemGastoOriginal.Identificador==null);

					    vm.EscopoAtualizacao.AtualizarGasto(data.ItemRegistro, vm.itemGastoOriginal);
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
            $(vm.messages).each(function (i, item) {
                vm.CamposInvalidos[item.Campo] = true;
            });
        };

        vm.VerificarExclusao = function () {
            vm.EscopoAtualizacao.modalPopupTrigger(vm.itemGastoOriginal, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
                vm.EscopoAtualizacao.Excluir(vm.itemGastoOriginal)
            });
        };

        vm.Cancelar = function () {
            vm.EscopoAtualizacao.Cancelar(vm.itemGastoOriginal);
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
                    item: function () { return vm.itemGasto; },
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
