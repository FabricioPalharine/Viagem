(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('CarroEditCtrl', ['Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Viagem', '$uibModal', '$timeout', 'Gasto', 'SignalR', 'Carro', 'Reabastecimento', CarroEditCtrl]);

	function CarroEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Viagem, $uibModal, $timeout, Gasto, SignalR,Carro,Reabastecimento) {
		var vm = this;
		vm.itemCarro = {};
		vm.loading = false;
		vm.messages = [];
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.ListaParticipante = [];
		vm.ItemAvaliacao = {};
		vm.itemOriginal = {};


		vm.ajustaInicio = function (item) {
		    vm.itemOriginal = vm.itemCarro = item;
		    if (!item.Identificador) {
		        angular.forEach($scope.$parent.itemCarro.ListaParticipantes, function (c) {
		            var item2 = jQuery.extend({}, c);
		            item2.Selecionado = true;
		            vm.ListaParticipante.push(item2);
		        });
		      
		    }
		};


		vm.load = function (itemBase) {
		    if (itemBase.Identificador && !vm.itemCarro.Avaliacoes && !vm.loading) {
		        vm.loading = true;
		        Carro.get({ id: itemBase.Identificador }, function (data) {
		            vm.itemCarro = data;
		          
		            angular.forEach($scope.$parent.itemCarro.ListaParticipantes, function (c) {
		                var item = jQuery.extend({}, c);
		                item.Selecionado = vm.itemCarro.Avaliacoes && $.grep(vm.itemCarro.Avaliacoes, function (e) {
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

		vm.save = function () {
		    vm.messages = [];
		    vm.submitted = true;
		    vm.loading = true;
		    vm.CamposInvalidos = {};
		    {
		        if (vm.itemCarro.DataRetirada) {
		            if (typeof vm.itemCarro.DataRetirada == "string") {

		                vm.itemCarro.DataRetirada = moment(vm.itemCarro.DataRetirada).format("YYYY-MM-DDTHH:mm:ss");
		            }
		            else
		                vm.itemCarro.DataRetirada = moment(vm.itemCarro.DataRetirada).format("YYYY-MM-DDTHH:mm:ss");

		        }

		        if (vm.itemCarro.DataDevolucao) {
		            if (typeof vm.itemCarro.DataDevolucao == "string") {

		                vm.itemCarro.DataDevolucao = moment(vm.itemCarro.DataDevolucao).format("YYYY-MM-DDTHH:mm:ss");
		            }
		            else
		                vm.itemCarro.DataDevolucao = moment(vm.itemCarro.DataDevolucao).format("YYYY-MM-DDTHH:mm:ss");

		        }

		        if (vm.itemCarro.ItemCarroEventoRetirada.Data)
		        {		           
		            vm.itemCarro.ItemCarroEventoRetirada.Data = moment(vm.itemCarro.ItemCarroEventoRetirada.Data).format("YYYY-MM-DDT");
		            vm.itemCarro.ItemCarroEventoRetirada.Data += (vm.itemCarro.ItemCarroEventoRetirada.strHora) ? vm.itemCarro.ItemCarroEventoRetirada.strHora : "00:00:00";

		        }

		        if (vm.itemCarro.ItemCarroEventoDevolucao.Data) {		          
		            vm.itemCarro.ItemCarroEventoDevolucao.Data = moment(vm.itemCarro.ItemCarroEventoDevolucao.Data).format("YYYY-MM-DDT");
		            vm.itemCarro.ItemCarroEventoDevolucao.Data += (vm.itemCarro.ItemCarroEventoDevolucao.strHora) ? vm.itemCarro.ItemCarroEventoDevolucao.strHora : "00:00:00";
		        }


		        angular.forEach(vm.ListaParticipante, function (item) {
		            var itens =
                         $.grep(vm.itemCarro.Avaliacoes, function (e) { return e.IdentificadorUsuario == item.Identificador && !e.DataExclusao });
		            if (item.Selecionado && itens.length == 0) {
		                var NovoItem = { IdentificadorUsuario: item.Identificador, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") }
		                vm.itemCarro.Avaliacoes.push(NovoItem);
		            }
		            else if (!item.Selecionado && itens.length > 0) {

		                item.DataExclusao.utc = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		            }

		        });

		        angular.forEach(vm.itemCarro.Gastos, function (item) {
		            if (item.ItemGasto != null)
		                item.ItemGasto.Alugueis = null;
		        });

		        angular.forEach(vm.itemCarro.Eventos, function (item) {
		            if (item.itemCarro != null)
		                item.itemCarro = null;
		        });

		        angular.forEach(vm.itemCarro.Reabastecimentos, function (item) {
		            if (item.itemCarro != null)
		                item.itemCarro = null;
		            angular.forEach(item.Gastos, function (itemGasto) {
		                if (itemGasto.ItemGasto != null) {
		                    itemGasto.ItemGasto.Alugueis = null;
		                    itemGasto.ItemGasto.Reabastecimentos = null;
		                }
		            });
		        });

		       
		        var MinhaAvaliacao =
                         $.grep(vm.itemCarro.Avaliacoes, function (e) { return e.IdentificadorUsuario == Auth.currentUser.Codigo && !e.DataExclusao });
		        if (MinhaAvaliacao.length > 0) {
		            MinhaAvaliacao[0].DataAtualizacao = moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		            MinhaAvaliacao[0].Comentario = vm.ItemAvaliacao.Comentario;
		            MinhaAvaliacao[0].Nota = vm.ItemAvaliacao.Nota;
		        }

		       
		        Carro.save(vm.itemCarro, function (data) {
		            vm.loading = false;
		            if (data.Sucesso) {
		                Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
		                $scope.$parent.itemCarro.AjustarCarroSalvo(vm.itemOriginal, data.ItemRegistro);
		            } else {
		                vm.messages = data.Mensagens;
		                vm.verificaCampoInvalido();
		            }
		        }, function (err) {
		            vm.loading = false;
		            Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
		        });
		        
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



		vm.VerificarParticipoCarro = function () {
		    return $.grep(vm.ListaParticipante, function (e) {

		        return e.Identificador == Auth.currentUser.Codigo && e.Selecionado;
		    }).length > 0;
		};

		vm.RemoverCusto = function (itemCusto) {
		    $scope.$parent.itemCarro.modalPopupTrigger(itemCusto, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
		        itemCusto.DataExclusao = moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		        Gasto.SalvarCustoCarro(itemCusto);
		        SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'GC', itemCusto.Identificador, false);

		    });

		};



		vm.AtualizarGasto = function (itemGasto, itemOriginal) {
		    var itemPush = itemGasto.Alugueis[0];
		    itemPush.ItemGasto = itemGasto;
		    vm.itemCarro.Gastos.push(itemPush);
		    SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'G', itemGasto.Identificador, true);
		};


		vm.AtualizarReabastecimento = function (itemReabastecimento) {
		    vm.itemCarro.Reabastecimentos.push(itemReabastecimento);
		};

		vm.AbrirGasto = function () {
		    $scope.$parent.itemCarro.modalPopupTrigger(vm.itemCarro, $translate.instant('MensagemGastoAdicionar'), $translate.instant('Novo'), $translate.instant('Existente'), function () {
		        var Referencias = { IdentificadorCarro: vm.itemCarro.Identificador };
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
		    $scope.$parent.itemCarro.modalPopupTrigger(vm.itemCarro, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
		        $scope.$parent.itemCarro.Excluir(vm.itemOriginal)
		    });
		};

		vm.Cancelar = function () {
		    $scope.$parent.itemCarro.Cancelar(vm.itemOriginal);
		};
		vm.RemoverReabastecimento = function (itemCusto) {
		    $scope.$parent.itemCarro.modalPopupTrigger(itemCusto, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
		        itemCusto.DataExclusao = moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		        Reabastecimento.delete(itemCusto.Identificador);
		        SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'CR', itemCusto.Identificador, false);

		    });
		};

		vm.RemoverDeslocamento = function (itemCusto) {
		    $scope.$parent.itemCarro.modalPopupTrigger(itemCusto, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
		        itemCusto.DataExclusao = moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss");
		        Carro.SalvarCarroDeslocamento(itemCusto);
		        SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'CD', itemCusto.Identificador, false);

		    });

		};

		vm.AtualizarDeslocamentoSalvo = function (itemDeslocamento, ItemRegistro) {
		    var Posicao = vm.itemCarro.Deslocamentos.indexOf(itemDeslocamento);
		    vm.itemCarro.Deslocamentos.splice(Posicao, 1, ItemRegistro);
		    SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'CD', ItemRegistro.Identificador, itemDeslocamento.Identificador == null);

		};

		vm.EditarDeslocamento = function (itemCusto) {
		    $uibModal.open({
		        templateUrl: 'Sistema/CarroDeslocamentoEdicao',
		        controller: 'CarroDeslocamentoEditCtrl',
		        controllerAs: 'itemCarroDeslocamentoEdit',
		        resolve: {
		            EscopoAtualizacao: vm,
		            itemCarroDeslocamento: itemCusto
		        }
		    });
		};

		vm.AdicionarDeslocamento = function () {
		    var UltimoOdometro = null;
		    var Deslocamentos = $.grep(vm.itemCarro.Deslocamentos, function (e) { return  !e.DataExclusao });

		    if (Deslocamentos.length > 0)
		    {
		        var ultimoDesloc = Deslocamentos[Deslocamentos.length - 1];
		        UltimoOdometro = ultimoDesloc.ItemCarroEventoChegada.Odometro;
		    }
		    else 
		    {   
		        UltimoOdometro = vm.itemCarro.ItemCarroEventoRetirada.Odometro;
		    }
		    var Usuarios = [];
		    angular.forEach(vm.ListaParticipante, function (c) {
		        if (c.Selecionado) {
		            var item2 = { IdentificadorUsuario: c.Identificador }
		            Usuarios.push(item2);
		        }
		    });

		    var itemDeslocamento = { IdentificadorCarro: vm.itemCarro.Identificador, ItemCarroEventoPartida: { Inicio: true, Data: moment(new Date()).format("YYYY-MM-DD"), strHora: moment(new Date()).format("HH:mm:ss"), Odometro: UltimoOdometro }, ItemCarroEventoChegada: { Inicio: false }, Usuarios: Usuarios };
		    $uibModal.open({
		        templateUrl: 'Sistema/CarroDeslocamentoEdicao',
		        controller: 'CarroDeslocamentoEditCtrl',
		        controllerAs: 'itemCarroDeslocamentoEdit',
		        resolve: {
		            EscopoAtualizacao: vm,
		            itemCarroDeslocamento: itemDeslocamento
		        }
		    });
		};

		vm.AdicionarReabastecimento = function () {
		    $uibModal.open({
		        templateUrl: 'Sistema/ReabastecimentoEdicao',
		        controller: 'ReabastecimentoEditCtrl',
		        controllerAs: 'itemReabastecimentoEdit',
		        resolve: {
		            EscopoAtualizacao: vm
		        }
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
		        var itemGravar = { IdentificadorCarro: vm.itemCarro.Identificador, IdentificadorGasto: itemCusto.Identificador, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") };
		        Gasto.SalvarCustoCarro(itemGravar, function (data) {
		            if (data.Sucesso) {
		                var itemPush = { Identificador: data.IdentificadorRegistro, ItemGasto: itemCusto };
		                vm.itemCarro.Gastos.push(itemPush);
		                SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'GC', data.IdentificadorRegistro, true);

		            }
		            $uibModalInstance.close();
		        });
		    };
		}

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
