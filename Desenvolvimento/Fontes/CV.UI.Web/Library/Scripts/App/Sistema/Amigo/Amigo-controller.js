(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('AmigoCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'uiGridConstants', 'Usuario', 'Amigo','RequisicaoAmizade','SignalR', AmigoCtrl]);

	function AmigoCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, uiGridConstants, Usuario, Amigo, RequisicaoAmizade,SignalR) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0 };
		vm.filtroAtualizacao = { Index: 0, Count: 0 };
		vm.filtroAtualizacaoAprovacao = { Index: 0, Count: 0 };
		vm.loadingAprovacao = false;
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
		vm.gridApiAprovacao = null;
		vm.ListaRequisicao = [];
		vm.AbrirAprovacao = false;
		vm.load = function () {
		    vm.loading = true;
		    vm.loadingAprovacao = true;
			vm.verificarPermissoes();

			var param = $stateParams;
			if (param.filtro != null) {
				vm.filtro = vm.filtroAtualizacao = param.filtro;
				 vm.pagingOptions.fields = vm.filtroAtualizacao.SortField;
				 vm.pagingOptions.directions = vm.filtroAtualizacao.SortOrder;
				vm.pagingOptions.currentPage = (vm.filtroAtualizacao.Index / vm.pagingOptions.pageSize) + 1;

			}
			if (param.AbrirAprovacao)
			    vm.AbrirAprovacao = true;
			vm.CarregarDadosWebApi(vm.pagingOptions.pageSize, vm.pagingOptions.currentPage);
			vm.CarregarDadosWebApiAprovacao(vm.pagingOptionsAprovacao.pageSize, vm.pagingOptionsAprovacao.currentPage);
		};

        vm.modalPopupTrigger = function (itemForDelete, indexForDelete, Mensagem, callback) {
            vm.askDelete(itemForDelete, indexForDelete, Mensagem)
          .then(function (data) {
              callback(data);
          })
          .then(null, function (reason) {
              vm.CarregarDadosWebApi(vm.pagingOptions.pageSize, vm.pagingOptions.currentPage);
          });
        };

        vm.AprovarRequisicao = function (itemRequisicao) {
            itemRequisicao.Status = 2;
            RequisicaoAmizade.save(itemRequisicao, function (data) {
                var Mensagens = new Array();
                $(data.Mensagens).each(function (j, jitem) {
                    Mensagens.push(jitem.Mensagem);
                });
                Error.showError('success', $translate.instant("Alerta"), Mensagens.join("<br/>"), true);
                vm.CarregarDadosWebApi(vm.pagingOptions.pageSize, vm.pagingOptions.currentPage);
                vm.CarregarDadosWebApiAprovacao(vm.pagingOptionsAprovacao.pageSize, vm.pagingOptionsAprovacao.currentPage);

            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
            });
        };

        vm.ReprovarRequisicao = function (itemRequisicao) {
            itemRequisicao.Status = 3;
            RequisicaoAmizade.save(itemRequisicao, function (data) {
                var Mensagens = new Array();
                $(data.Mensagens).each(function (j, jitem) {
                    Mensagens.push(jitem.Mensagem);
                });
                Error.showError('success', $translate.instant("Alerta"), Mensagens.join("<br/>"), true);
                vm.CarregarDadosWebApiAprovacao(vm.pagingOptionsAprovacao.pageSize, vm.pagingOptionsAprovacao.currentPage);
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
            });
        };

        vm.modalPopupTriggerAprovacao = function (itemForDelete, indexForDelete, Mensagem, callback) {
            vm.askDelete(itemForDelete, indexForDelete, Mensagem)
          .then(function (data) {
              callback(data);
          })
          .then(null, function (reason) {
              vm.CarregarDadosWebApiAprovacao(vm.pagingOptions.pageSize, vm.pagingOptions.currentPage);
          });
        };

        vm.askDelete = function (itemForDelete, indexForDelete,Mensagem) {
            // $uibModalInstance.close();
            var modal = $uibModal.open({
                templateUrl: 'modalDelete.html',
                controller: ['$uibModalInstance', 'item', 'index','MensagemConfirmacao', vm.DeleteModalCtrl],
                controllerAs: 'vmDelete',
                resolve: {
                    item: function () { return itemForDelete; },
                    index: function () { return indexForDelete; },
                    MensagemConfirmacao: function () { return Mensagem; },
                }
            });

            return modal.result;
        };

        vm.DeleteModalCtrl = function ($uibModalInstance, itemForDelete, indexForDelete, MensagemConfirmacao) {
            var vmDelete = this;
            vmDelete.MensagemConfirmacao = MensagemConfirmacao;
            vmDelete.itemForDelete = itemForDelete;

            vmDelete.close = function () {
                $uibModalInstance.dismiss();
            };

            vmDelete.back = function () {
                $uibModalInstance.dismiss();
                vm.actionModal();
            };

            vmDelete.delete = function () {

                $uibModalInstance.close(vmDelete.itemForDelete);
            };
        };

        $rootScope.$on('loggin', function (event) {
            vm.verificarPermissoes();
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


        vm.totalServerItemsAprovacao = 0;
        vm.pagingOptionsAprovacao = {
            pageSize: 20,
            currentPage: 1,
            fields: [],
            directions: []
        };

        vm.AjustarDadosPagina = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
            vm.ListaDados = [];
 
            $timeout(function () {
                vm.ListaDados = data.Lista;
            },5);
            vm.gridOptions.totalItems = data.TotalRegistros;
            vm.gridApi.grid.refresh();
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        };

        vm.AjustarDadosPaginaAprovacao = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
            vm.ListaRequisicao = data.Lista;

            vm.gridOptionsAprovar.totalItems = data.TotalRegistros;
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
        vm.CarregarDadosWebApiAprovacao = function (pageSize, page) {
            vm.loadingAprovacao = true;
            vm.filtroAtualizacaoAprovacao.Index = 0;
            vm.filtroAtualizacaoAprovacao.Count = null;
         

            RequisicaoAmizade.list({ json: JSON.stringify(vm.filtroAtualizacaoAprovacao) }, function (data) {
                vm.AjustarDadosPaginaAprovacao(data);               
               
                vm.loadingAprovacao = false;
            }, function (err) {

                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
                
                vm.loadingAprovacao = false;
            });
        };

        vm.CarregarDadosWebApi = function (pageSize, page) {
            vm.loading = true;
            vm.filtroAtualizacao.Index = 0;
            vm.filtroAtualizacao.Count = null;

            vm.CamposInvalidos = {};
            vm.messages = [];

            Amigo.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
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

        vm.gravarAlteracaoAmigo = function (itemAmigo) {
            vm.loading = true;
            Amigo.RequisicaoAmizade(itemAmigo, function (data) {
                var Mensagens = new Array();
                $(data.Mensagens).each(function (j, jitem) {
                    Mensagens.push(jitem.Mensagem);
                });
                Error.showError('success', $translate.instant("Alerta"), Mensagens.join("<br/>"), true);
                vm.CarregarDadosWebApi(vm.pagingOptions.pageSize, vm.pagingOptions.currentPage);
                if (itemAmigo.Acao == 3)
                {
                    SignalR.RequisitarAmizade(itemAmigo.IdentificadorUsuario, data.IdentificadorRegistro);
                }
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
            });
        };

        vm.AlterarSeguidor = function (amigo, indice) {
            var funcRetorno = null;
            var Mensagem = "";
            if (amigo.Seguidor) {
                Mensagem = $translate.instant('Amigo_MensagemRemoverSeguidor');
                funcRetorno = function (itemAmigo) {
                    itemAmigo.Acao = 2;
                    vm.gravarAlteracaoAmigo(itemAmigo);
                };
            }
            else {
                Mensagem = $translate.instant('Amigo_MensagemAdicionarSeguidor');
                funcRetorno = function (itemAmigo) {
                    itemAmigo.Acao = 1;
                    vm.gravarAlteracaoAmigo(itemAmigo);
                };
            }
            vm.modalPopupTrigger(amigo, indice, Mensagem, funcRetorno);
        };

        vm.AlterarSeguido = function (amigo, indice) {
            var funcRetorno = null;
            var Mensagem = "";
            if (amigo.Seguido) {
                Mensagem = $translate.instant('Amigo_MensagemRemoverSeguido');
                funcRetorno = function (itemAmigo) {
                    itemAmigo.Acao = 4;
                    vm.gravarAlteracaoAmigo(itemAmigo);
                };
            }
            else {
                Mensagem = $translate.instant('Amigo_MensagemAdicionarSeguido');
                funcRetorno = function (itemAmigo) {
                    itemAmigo.Acao = 3;
                    vm.gravarAlteracaoAmigo(itemAmigo);
                };
            }
            vm.modalPopupTrigger(amigo, indice, Mensagem, funcRetorno);
        };

        vm.gridOptions = {
            data: 'itemAmigo.ListaDados',           
            			columnDefs: [
				{field:'Nome', displayName: $translate.instant('Amigo_Nome'),},
				{
				    field: 'EMail', displayName: $translate.instant('Amigo_EMail'),
				},
                { field: 'Seguidor', displayName: $translate.instant('Amigo_Seguidor'), cellTemplate: "CheckSeguidorTemplate.html", width: 120, },
                { field: 'Seguido', displayName: $translate.instant('Amigo_Seguido'), cellTemplate: "CheckSeguidoTemplate.html", width: 120, },
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
            pagination: vm.pagingOptions,
            appScopeProvider: vm,
        };

        vm.AtualizarGrid = function () {
            $timeout(function () {
                vm.gridApi.core.handleWindowResize();
                vm.gridApiAprovacao.core.handleWindowResize();
            }, 200);
        };

        vm.gridOptionsAprovar = {
            data: 'itemAmigo.ListaRequisicao',
            columnDefs: [
                { field: 'Identificador', displayName: '', cellTemplate: "BotoesGridTemplate.html", width: 60, },
                { field: 'ItemUsuario.Nome', displayName: $translate.instant('Amigo_Nome'), },
                {
                    field: 'ItemUsuario.EMail', displayName: $translate.instant('Amigo_EMail'),
                },
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
                vm.gridApiAprovacao = grid;
               
            },
            useExternalPagination: false,
            useExternalSorting: false,
            pagination: vm.pagingOptionsAprovacao,
            appScopeProvider: vm,
            totalItems: vm.totalServerItemsAprovacao,
        };
	}
}());
