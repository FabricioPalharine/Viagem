(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('FotoCtrl',['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService','Cidade','Usuario','Viagem','Foto', FotoCtrl]);

	function FotoCtrl($uibModal,  Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService,Cidade,Usuario,Viagem,Foto) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0 };
		vm.filtroAtualizacao = {  Index: 0, Count: 0 };
		vm.loading = false;
		vm.showModal = false;
		vm.modalAcao = function () {;
			vm.showModal = true;
		}
		vm.modalDelete = {};
		vm.PermiteInclusao = true;
		vm.PermiteAlteracao = true;
		vm.PermiteExclusao = true;
		vm.ListaDados = [];
		vm.gridApi = null;

		vm.load = function () {
			vm.loading = true;
			vm.verificarPermissoes();

			var param = $stateParams;
			if (param.filtro != null) {
				vm.filtro = vm.filtroAtualizacao = param.filtro;
				 vm.pagingOptions.fields = vm.filtroAtualizacao.SortField;
				 vm.pagingOptions.directions = vm.filtroAtualizacao.SortOrder;
				vm.pagingOptions.currentPage = (vm.filtroAtualizacao.Index / vm.pagingOptions.pageSize) + 1;

			}
			vm.CarregarDadosWebApi(vm.pagingOptions.pageSize, vm.pagingOptions.currentPage);
		};
		vm.delete = function (itemForDelete, indexForDelete, callback) {
			vm.loading = true;
			Foto.delete({ id: itemForDelete.Identificador }, function (data) {
				callback(data);
				if (data.Sucesso) {
					vm.CarregarDadosWebApi(vm.pagingOptions.pageSize, vm.pagingOptions.currentPage);
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

        vm.actionModal = function (item, indexForDelete) {
            $uibModal.open({
                templateUrl: 'modal.html',
                controller: ['$uibModalInstance', 'item', 'index', vm.ActionModalCtrl],
                controllerAs: 'vmAction',
                resolve: {
                    item: function () { return item; },
                    index: function () { return indexForDelete; }
                }
            });
        };
        vm.ActionModalCtrl = function ($uibModalInstance, item, index) {
            var vmAction = this;
            vmAction.item = item;
            vmAction.indexForDelete = index;
            // console.log(itens);
            vmAction.close = function () {
                $uibModalInstance.close();
            }
            vmAction.edit = function (idToEdit) {
                $uibModalInstance.close();
                $state.go('FotoEdicao', { id: idToEdit, filtro: vm.filtroAtualizacao });
            };

            vmAction.askDelete = function (itemForDelete, indexForDelete) {
                vm.askDelete(itemForDelete, indexForDelete);
                $uibModalInstance.close();
            };

        }

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

        $rootScope.$on('loggin', function (event) {
            vm.verificarPermissoes();
        });

        angular.element($window).bind('resize', function () {
            var screenSizes = $.AdminLTE.options.screenSizes;
            vm.gridOptions.columnDefs[0].visible = $(window).width() > (screenSizes.sm - 1);
            vm.gridApi.grid.refresh();

           
        });


		vm.verificarPermissoes = function () {
			$(Auth.currentUser.access).each(function (i, item) {
			});
		};

        vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);

                  

            vm.pagingOptions.currentPage = 1;
            vm.gridApi.grid.options.paginationCurrentPage = 1;
            vm.pagingOptions.fields = [];
            vm.pagingOptions.directions = [];
            angular.forEach(vm.gridApi.grid.columns, function (c) {
                c.sort = {};
            });

            vm.CarregarDadosWebApi(vm.pagingOptions.pageSize, vm.pagingOptions.currentPage);
        };

        vm.clean = function () {
            vm.filtro = { Nome: '', Index: 0, Count: 0 };
            vm.filtraDado();
        };

        vm.totalServerItems = 0;
        vm.pagingOptions = {
            pageSize: 20,
            currentPage: 1,
            fields: [],
            directions: []
        };

        vm.AjustarDadosPagina = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
            vm.ListaDados = data.Lista;
            vm.gridOptions.totalItems = data.TotalRegistros;
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
        vm.CarregarDadosWebApi = function (pageSize, page) {
            vm.loading = true;
            vm.filtroAtualizacao.Index = (page - 1) * pageSize;
            vm.filtroAtualizacao.Count = pageSize;

            vm.filtroAtualizacao.SortField =vm.pagingOptions.fields;
            vm.filtroAtualizacao.SortOrder =vm.pagingOptions.directions;

            vm.CamposInvalidos = {};
            vm.messages = [];

            Foto.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
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
//
        vm.gridOptions = {
            data: 'itemFoto.ListaDados',           
            			columnDefs: [
				{field:'Identificador',  displayName: '', cellTemplate: "BotoesGridTemplate.html",  width: 60,},
				{field:'IdentificadorViagem', displayName: $translate.instant('Foto_IdentificadorViagem'),},
				{field:'IdentificadorUsuario', displayName: $translate.instant('Foto_IdentificadorUsuario'),},
				{field:'IdentificadorCidade', displayName: $translate.instant('Foto_IdentificadorCidade'),},
				{field:'Comentario', displayName: $translate.instant('Foto_Comentario'),},
				{field:'Latitude', displayName: $translate.instant('Foto_Latitude'),cellFilter: 'number:\'8\'' },
				{field:'Longitude', displayName: $translate.instant('Foto_Longitude'),cellFilter: 'number:\'8\'' },
				{field:'Data', displayName: $translate.instant('Foto_Data'),cellFilter: 'date:\'dd/MM/yyyy\'' },
				{field:'LinkThumbnail', displayName: $translate.instant('Foto_LinkThumbnail'),},
				{field:'LinkFoto', displayName: $translate.instant('Foto_LinkFoto'),},
				{field:'CodigoFoto', displayName: $translate.instant('Foto_CodigoFoto'),},
				{field:'Video', displayName: $translate.instant('Foto_Video'),},
				{field:'TipoArquivo', displayName: $translate.instant('Foto_TipoArquivo'),},
			],

            enablePagination: true,
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
                var screenSizes = $.AdminLTE.options.screenSizes;
                vm.gridOptions.columnDefs[0].visible = $(window).width() > (screenSizes.sm - 1);
                grid.core.on.sortChanged($scope, function (grid, sortColumns) {
                    vm.pagingOptions.fields = [];
                    vm.pagingOptions.directions = [];
                    angular.forEach(sortColumns, function (c) {
                        vm.pagingOptions.fields.push(c.field);
                        vm.pagingOptions.directions.push(c.sort.direction);
                    });
                    vm.CarregarDadosWebApi(vm.pagingOptions.pageSize, vm.pagingOptions.currentPage);
                });
                grid.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                    vm.pagingOptions.currentPage = newPage;
                    vm.CarregarDadosWebApi(pageSize, newPage);
                });
            },
            useExternalPagination: true,
            useExternalSorting: true,
            pagination: vm.pagingOptions,
            paginationTemplate: "NewFooterTemplate.html",
            appScopeProvider: vm,
            totalItems: vm.totalServerItems,
            rowTemplate: "<div on-long-press=\"grid.appScope.actionModal(row.entity, $index)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.uid\" ui-grid-one-bind-id-grid=\"rowRenderIndex + '-' + col.uid + '-cell'\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" role=\"{{col.isRowHeader ? 'rowheader' : 'gridcell'}}\" ui-grid-cell></div>" 
        };
	}
}());
