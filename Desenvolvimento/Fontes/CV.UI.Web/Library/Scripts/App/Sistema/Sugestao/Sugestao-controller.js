(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('SugestaoCtrl',['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService','Cidade','Usuario','Viagem','Sugestao','SignalR', SugestaoCtrl]);

	function SugestaoCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Cidade, Usuario, Viagem, Sugestao, SignalR) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0 };
		vm.filtroAtualizacao = {  Index: 0, Count: 0 };
		vm.loading = false;		
		vm.ListaDados = [];
		vm.gridApi = null;
		vm.ListaCidades = [];
		vm.itemCidade = {};

		vm.load = function () {
			vm.loading = true;
			Cidade.CarregarSugestao(function (lista) {
			    vm.ListaCidades = lista;

			});
			vm.CarregarDadosWebApi();
		};
		vm.delete = function (itemForDelete, indexForDelete, callback) {
			vm.loading = true;
			Sugestao.delete({ id: itemForDelete.Identificador }, function (data) {
				
				if (data.Sucesso) {
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
            if (vm.itemCidade && vm.itemCidade.Identificador)
                vm.filtroAtualizacao.IdentificadorCidade = vm.itemCidade.Identificador;
                  

         

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
//
        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            vm.filtroAtualizacao.Index = 0;
            vm.filtroAtualizacao.Count = null;

                     vm.CamposInvalidos = {};
            vm.messages = [];

            Sugestao.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                vm.AjustarDadosPagina(data);
                if (!data.Sucesso) {
                    vm.messages = data.Mensagens;
                    vm.verificaCampoInvalido();
                }

               
                vm.loading = false;
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
                
                vm.loading = false;
            });
        };

        vm.Incluir = function () {
                var ItemSugestao = {  Status: 0};
                vm.Editar(ItemSugestao);
          
        };

        vm.Editar = function (ItemSugestao) {
            $uibModal.open({
                templateUrl: 'Sistema/SugestaoEdicao',
                controller: 'SugestaoEditCtrl',
                controllerAs: 'itemSugestaoEdit',
                resolve: {
                    EscopoAtualizacao: vm,
                    ItemSugestao: function () { return ItemSugestao }
                }
            });
        };

        vm.AtualizarItemSalvo = function (itemAporte, itemOriginal) {
            if (itemOriginal.Identificador) {
                var Posicao = vm.ListaDados.indexOf(itemOriginal);
                vm.ListaDados.splice(Posicao, 1, itemAporte);
                var dados = vm.ListaDados;
                vm.ListaDados = [];

                $timeout(function () {
                    vm.ListaDados = dados;
                }, 5);
            }
            else
                vm.ListaDados.push(itemAporte);
        };
//
        vm.gridOptions = {
            data: 'itemSugestao.ListaDados',           
            			columnDefs: [
				{field:'Identificador',  displayName: '', cellTemplate: "BotoesGridTemplate.html",  width: 60,},
				{ field: 'Local', displayName: $translate.instant('Sugestao_Local'), },
                				{ field: 'Tipo', displayName: $translate.instant('Sugestao_Tipo'), },

				{field:'Comentario', displayName: $translate.instant('Sugestao_Comentario'),},
			],

            enablePagination: false,
            showGridFooter: false,
            enableRowSelection: false,
            multiSelect: false,
            paginationPageSizes: [20],
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 1,
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
