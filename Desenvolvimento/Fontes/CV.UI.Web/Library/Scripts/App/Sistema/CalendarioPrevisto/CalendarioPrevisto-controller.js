(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('CalendarioPrevistoCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Viagem', 'CalendarioPrevisto', 'uiCalendarConfig','SignalR', CalendarioPrevistoCtrl]);

	function CalendarioPrevistoCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Viagem, CalendarioPrevisto, uiCalendarConfig, SignalR) {
		var vm = this;
		vm.loading = false;
		vm.filtroAtualizacao = {};
		
		vm.gridApi = null;
		vm.ListaDados = [];
		vm.ListaEventos = [vm.ListaDados]

		vm.load = function () {
			vm.loading = true;
			
			vm.CarregarDadosWebApi();

			SignalR.AvisarAlertaAtualizacao = function (TipoAtualizacao, Identificador, Inclusao) {
			    if (TipoAtualizacao == "CP") {
			        CalendarioPrevisto.get({ id: Identificador }, function (data) {
			            var itens = $.grep(vm.ListaDados, function (e) { return e.id == Identificador; });
			            if (data.DataExclusao)
			            {
			                if (itens.length > 0) {
			                    var Posicao = vm.ListaDados.indexOf(itens[0]);
			                    vm.ListaDados.splice(Posicao, 1);
			                }
			            }
			            else
			            {
			                if (itens.length == 0 && Inclusao) {
			                    vm.ListaDados.push(vm.TransformarCalendario(data));
			                }
			                else if (itens.length > 0) {
			                    var Posicao = vm.ListaDados.indexOf(itens[0]);
			                    vm.ListaDados.splice(Posicao, 1, vm.TransformarCalendario(data));
			                }
			            }
			        });

			    }
			};
		};

		vm.delete = function (itemForDelete, indexForDelete, callback) {
			vm.loading = true;
			CalendarioPrevisto.delete({ id: itemForDelete.Identificador }, function (data) {
				
			    if (data.Sucesso) {
			        callback(data);
			        var ItemExcluir = $.grep(vm.ListaDados, function (e) { return e.Identificador == itemForDelete.Identificador })[0];
			        var Posicao = vm.ListaDados.indexOf(ItemExcluir);
			        vm.ListaDados.splice(Posicao, 1);
			        SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'CP', itemForDelete.Identificador, false);
					Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
				}
				else {
					var Mensagens = new Array();
					$(data.Mensagens).each(function (j, jitem) {
						Mensagens.push(jitem.Mensagem);
					});
				Error.showError('warning', $translate.instant("Alerta"), Mensagens.join("<br/>"), true);
				}
				vm.loading = false;
			},
			function (err) {
				$uibModalInstance.close();
				Error.showError('error', 'Ops!', $translate.instant("ErroExcluir"), true);
				vm.loading = false;
			})
		};
             
        vm.askDelete = function (itemForDelete, callback) {
            // $uibModalInstance.close();
            $uibModal.open({
                templateUrl: 'modalDelete.html',
                controller: ['$uibModalInstance', 'item', 'callback', vm.DeleteModalCtrl],
                controllerAs: 'vmDelete',
                resolve: {
                    item: function () { return itemForDelete; },
                    callback: function () { return callback; }
                }
            });
        };

        vm.DeleteModalCtrl = function ($uibModalInstance, itemForDelete, callback) {
            var vmDelete = this;
            vmDelete.itemForDelete = itemForDelete;

            vmDelete.close = function () {
                $uibModalInstance.close();
            };

            vmDelete.back = function () {
                $uibModalInstance.close();
                vm.actionModal();
            };

            vmDelete.delete = function () {
                vm.delete(vmDelete.itemForDelete, function () {
                    $uibModalInstance.close();
                    callback();
                });
            };
        };

       


        vm.AjustarDadosPagina = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
           // vm.ListaDados = [];
            angular.forEach(data.Lista, function (item) {
                vm.ListaDados.push(vm.TransformarCalendario(item));
            });
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        };

        vm.TransformarCalendario = function(item)
        {
            var itemCalendario = {
                id: item.Identificador, title: item.Nome, start: moment(  item.DataInicio), end: moment( item.DataFim), editable: Auth.currentUser.PermiteEdicao,
                //className: item.Prioridade==1?"bg-blue":item.Prioridade==2?"bg-yellow":"bg-red"
            };
            return itemCalendario;

        }

        vm.Incluir = function () {
            var ItemSugestao = { Prioridade: 2, AvisarHorario: false };
            vm.AbrirEdicao(ItemSugestao);
        };

        vm.Editar = function (ItemSugestao) {
            CalendarioPrevisto.get({ id: ItemSugestao.id }, function (data) {
                vm.AbrirEdicao(data);
            });
        };

        vm.AtualizarItemSalvo = function (itemNovo, itemOriginal) {
            if (!itemOriginal.Identificador)
            {
                vm.ListaDados.push(vm.TransformarCalendario(itemNovo));
            }
            else
            {
                var ItemExcluir = $.grep(vm.ListaDados, function (e) { return e.Identificador == itemOriginal.Identificador })[0];
                var Posicao = vm.ListaDados.indexOf(ItemExcluir);
                vm.ListaDados.splice(Posicao, 1, vm.TransformarCalendario(itemNovo));
            }
            SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'CP', itemNovo.Identificador, !itemOriginal.Identificador );
        };


        vm.AbrirEdicao = function (ItemSugestao) {
            $uibModal.open({
                templateUrl: 'Sistema/CalendarioPrevistoEdicao',
                controller: 'CalendarioPrevistoEditCtrl',
                controllerAs: 'itemCalendarioPrevistoEdit',
                resolve: {
                    EscopoAtualizacao: vm,
                    ItemCalendarioPrevisto: function () { return ItemSugestao }
                }
            });
        };

        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };
//
        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            vm.filtroAtualizacao.Index = 0;
            vm.filtroAtualizacao.Count = null;

      

            CalendarioPrevisto.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                vm.AjustarDadosPagina(data);
                if (!data.Sucesso) {
                }

               
                vm.loading = false;
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
                
                vm.loading = false;
            });


            vm.alertOnEventClick = function (date, jsEvent, view) {
                $scope.alertMessage = (date.title + ' was clicked ');
            };
            /* alert on Drop */
            vm.alertOnDrop = function (event, delta, revertFunc, jsEvent, ui, view) {
                $scope.alertMessage = ('Event Dropped to make dayDelta ' + delta);
            };
            /* alert on Resize */
            vm.alertOnResize = function (event, delta, revertFunc, jsEvent, ui, view) {
                $scope.alertMessage = ('Event Resized to make dayDelta ' + delta);
            };
          
            vm.uiConfig = {
                calendar: {
                    height: 450,
                    editable: true,
                    header: {
                        left: 'prev,next today',
                        center: 'title',
                        right: 'month,agendaWeek,agendaDay,listMonth'
                    },
                    defaultView: 'agendaWeek',
                    locale: vm.Idioma(),
                    lazyFetching: false,
                    allDaySlot: false,
                    eventClick: vm.alertOnEventClick,
                    eventDrop: vm.alertOnDrop,
                    eventResize: vm.alertOnResize,
                    //eventRender: $scope.eventRender
                }
            };
        };
	    //

	}
}());
