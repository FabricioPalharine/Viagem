(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('CidadeGrupoCtrl',['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService','Cidade','Viagem','CidadeGrupo', CidadeGrupoCtrl]);

	function CidadeGrupoCtrl($uibModal,  Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService,Cidade,Viagem,CidadeGrupo) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0 };
		vm.filtroAtualizacao = {  Index: 0, Count: 0 };
		vm.loading = false;		
		vm.ListaDados = [];
		vm.gridApi = null;

		vm.load = function () {
			vm.loading = true;			
			vm.CarregarDadosWebApi();
		};

		vm.delete = function (itemForDelete, indexForDelete, callback) {
			vm.loading = true;
			CidadeGrupo.delete({ id: itemForDelete.Identificador }, function (data) {
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

	

        vm.AjustarDadosPagina = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
            vm.ListaDados = data;
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
            var ItemSugestao = { Edicao: false, CidadesFilhas: [] };
            vm.AbrirEditar(ItemSugestao);

        };

        vm.Editar = function (ItemSugestao) {
            CidadeGrupo.get({ id: ItemSugestao.Identificador }, function (data) {
                vm.AbrirEditar(data);
            });
        };

        vm.AbrirEditar = function (ItemCidadeGrupo) {
            $uibModal.open({
                templateUrl: 'Sistema/CidadeGrupoEdicao',
                controller: 'CidadeGrupoEditCtrl',
                controllerAs: 'itemCidadeGrupoEdit',
                resolve: {
                    EscopoAtualizacao: vm,
                    ItemCidadeGrupo: function () { return ItemCidadeGrupo }
                }
            });
        };

        vm.AtualizarItemSalvo = function (itemCidade, itemOriginal) {
            if (!itemOriginal.IdentificadorCidade) {
                vm.ListaDados.push(itemCidade);
            }
        };
//
        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            vm.filtroAtualizacao.Index = 0;
            vm.filtroAtualizacao.Count = null;


            vm.CamposInvalidos = {};
            vm.messages = [];

            Cidade.ListarCidadePai(function (data) {
                vm.loading = false;
                vm.AjustarDadosPagina(data);
             

               
                vm.loading = false;
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
                
                vm.loading = false;
            });
        };
//
        vm.gridOptions = {
            data: 'itemCidadeGrupo.ListaDados',           
            			columnDefs: [
				{field:'Identificador',  displayName: '', cellTemplate: "BotoesGridTemplate.html",  width: 60,},
				{field:'Nome', displayName: $translate.instant('Cidade_Nome'),},
				{ field: 'Estado', displayName: $translate.instant('Cidade_Estado'), },
				{ field: 'ItemPais.Nome', displayName: $translate.instant('Cidade_IdentificadorPais'), },
			],

            enablePagination: false,
            showGridFooter: false,
            enableRowSelection: false,
            multiSelect: false,
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
