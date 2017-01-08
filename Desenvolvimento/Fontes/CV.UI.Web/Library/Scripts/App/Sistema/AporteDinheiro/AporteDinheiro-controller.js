(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('AporteDinheiroCtrl',['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService','Usuario','Viagem','AporteDinheiro','Dominio','SignalR', AporteDinheiroCtrl]);

	function AporteDinheiroCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Usuario, Viagem, AporteDinheiro, Dominio,SignalR) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0 };
		vm.filtroAtualizacao = {  Index: 0, Count: 0 };
		vm.loading = false;
		vm.ListaDados = [];
		vm.gridApi = null;
		vm.ListaMoeda = [];
		vm.itemMoeda = {};

		vm.load = function () {
			vm.loading = true;
			Dominio.CarregaMoedas(function (data) {
			    vm.ListaMoeda = data;
			});

			vm.CarregarDadosWebApi();
		};
		vm.delete = function (itemForDelete, indexForDelete, callback) {
			vm.loading = true;
			AporteDinheiro.delete({ id: itemForDelete.Identificador }, function (data) {
				
			    if (data.Sucesso) {
			        SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'AD', itemForDelete.Identificador, false);

			        callback(data);
			        var Posicao = vm.ListaDados.indexOf(itemForDelete);
				    vm.ListaDados.splice(Posicao, 1);
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

     vm.askDelete = function (itemForDelete, indexForDelete) {
            // $uibModalInstance.close();
            $uibModal.open({
                templateUrl: 'modalDelete.html',
                controller: ['$uibModalInstance', 'item', 'index', vm.DeleteModalCtrl],
                controllerAs: 'vmDelete',
                resolve: {
                    item: function () { return itemForDelete; },
                    index: function () { return indexForDelete; }
                }
            });
        };

        vm.DeleteModalCtrl = function ($uibModalInstance, itemForDelete, indexForDelete) {
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
                vm.delete(vmDelete.itemForDelete, indexForDelete, function () {
                    $uibModalInstance.close();
                });
            };
        };

        vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);
            if (vm.itemMoeda && vm.itemMoeda.Codigo)
                vm.filtroAtualizacao.Moeda = vm.itemMoeda.Codigo;
            else
                vm.filtroAtualizacao.Moeda = null;
                  

    
            vm.CarregarDadosWebApi();
        };


        vm.AjustarDadosPagina = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
            vm.ListaDados = data.Lista;
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        };

        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };

        vm.Incluir = function () {
            Viagem.get({ id: Auth.currentUser.IdentificadorViagem }, function (data) {
                var ItemAporte = { DataAporte: moment(new Date()).format("YYYY-MM-DDT00:00:00"), Moeda: data.Moeda };
                vm.EditarItem(ItemAporte);
            });
        };

        vm.EditarItem = function (itemAporte) {
            $uibModal.open({
                templateUrl: 'Sistema/AporteDinheiroEdicao',
                controller: 'AporteDinheiroEditCtrl',
                controllerAs: 'itemAporteDinheiroEdit',
                resolve: {
                    EscopoAtualizacao: vm,
                    ItemAporteDinheiro: function () { return itemAporte }
                }
            });
        };

        vm.AtualizarItemSalvo = function (itemAporte, itemOriginal) {
            if (itemOriginal.Identificador) {
                var Posicao = vm.ListaDados.indexOf(itemOriginal);
                vm.ListaDados.splice(Posicao, 1, itemAporte);
            }
            else
                vm.ListaDados.unshift(itemAporte);
        };
//
        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            vm.filtroAtualizacao.Index = 0;
            vm.filtroAtualizacao.Count = null;

            vm.CamposInvalidos = {};
            vm.messages = [];

            AporteDinheiro.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                vm.AjustarDadosPagina(data);
                if (!data.Sucesso) {
                    vm.messages = data.Mensagens;
                }               
                vm.loading = false;
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
                
                vm.loading = false;
            });
        };
//
        vm.gridOptions = {
            data: 'itemAporteDinheiro.ListaDados',           
            			columnDefs: [
				{field:'Identificador',  displayName: '', cellTemplate: "BotoesGridTemplate.html",  width: 60,},
				{field:'Valor', displayName: $translate.instant('AporteDinheiro_Valor'),cellFilter: 'number:\'2\'' },
				{field:'MoedaSigla', displayName: $translate.instant('AporteDinheiro_Moeda'),},
				{field:'DataAporte', displayName: $translate.instant('AporteDinheiro_DataAporte'),cellFilter: 'date:\'dd/MM/yyyy\'' },
				{field:'Cotacao', displayName: $translate.instant('AporteDinheiro_Cotacao'),cellFilter: 'number:\'6\'' },
			],

            enablePagination: false,
            showGridFooter: false,
            enableRowSelection: false,
            multiSelect: false,
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 0,
            onRegisterApi: function (grid) {
                if (Auth.currentUser && Auth.currentUser.Cultura) {
                    var cultura = Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
                    i18nService.setCurrentLang(cultura)
                }
                vm.gridApi = grid;
              
            },
            useExternalPagination: false,
            useExternalSorting: false,
            appScopeProvider: vm,
        };
	}
}());
