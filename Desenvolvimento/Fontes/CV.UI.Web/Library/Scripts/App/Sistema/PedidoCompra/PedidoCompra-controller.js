(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('PedidoCompraCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Usuario', 'Viagem', 'ListaCompra', PedidoCompraCtrl]);

	function PedidoCompraCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Usuario, Viagem, ListaCompra) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0 };
		vm.filtroAtualizacao = { Index: 0, Count: 0 };
		vm.itemUsuario = {};
		vm.loading = false;

		vm.ListaDados = [];
		vm.ListaDadosPedidos = [];
		vm.ListaUsuario = [];
		vm.gridApi = null;

		vm.AbrirRequisicaoCompra = false;
		vm.load = function () {
		    vm.loading = true;


		    vm.CarregarDadosWebApi();
		    Viagem.CarregarParticipantes(function (data) {
		        vm.ListaUsuario = data;

		    });

		};

		vm.delete = function (itemForDelete, indexForDelete, callback) {
			vm.loading = true;
			ListaCompra.delete({ id: itemForDelete.Identificador }, function (data) {
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

		vm.modalPopupTrigger = function (itemForDelete, Mensagem, TextoBotaoOK, TextoBotaoCancel, callbackOk, callbackCancel) {
		    vm.askDelete(itemForDelete, Mensagem, TextoBotaoOK, TextoBotaoCancel)
          .then(function (data) {
              if (callbackOk)
                  callbackOk();
          })
          .then(null, function (reason) {
              if (callbackCancel)
                  callbackCancel()
          });
		};

		vm.askDelete = function (itemForDelete, Mensagem, TextoBotaoOK, TextoBotaoCancel) {
		    // $uibModalInstance.close();
		    var modal = $uibModal.open({
		        templateUrl: 'modalDelete.html',
		        controller: ['$uibModalInstance', 'item', 'MensagemConfirmacao', 'TextoBotaoOK', 'TextoBotaoCancel', vm.DeleteModalCtrl],
		        controllerAs: 'vmDelete',
		        resolve: {
		            item: function () { return itemForDelete; },
		            MensagemConfirmacao: function () { return Mensagem; },
		            TextoBotaoOK: function () { return TextoBotaoOK; },
		            TextoBotaoCancel: function () { return TextoBotaoCancel; },
		        }
		    });

		    return modal.result;
		};

		vm.DeleteModalCtrl = function ($uibModalInstance, itemForDelete, MensagemConfirmacao, TextoBotaoOK, TextoBotaoCancel) {
		    var vmDelete = this;
		    vmDelete.MensagemConfirmacao = MensagemConfirmacao;
		    vmDelete.itemForDelete = itemForDelete;
		    vmDelete.TextoBotaoOK = TextoBotaoOK;
		    vmDelete.TextoBotaoCancel = TextoBotaoCancel;
		    vmDelete.close = function () {
		        $uibModalInstance.dismiss();
		    };

		    vmDelete.back = function () {
		        $uibModalInstance.dismiss();

		    };

		    vmDelete.confirmar = function () {

		        $uibModalInstance.close(vmDelete.itemForDelete);
		    };
		};
        
		vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);
            if (vm.itemUsuario && vm.itemUsuario.Identificador)
                vm.filtroAtualizacao.IdentificadorParticipante = vm.itemUsuario.Identificador;
            else
                vm.filtroAtualizacao.IdentificadorParticipante = null;
            vm.CarregarDadosWebApi();
		};

	
		vm.AdicionarListaCompra = function () {
		    var itemLista = { IdentificadorUsuario: Auth.currentUser.Codigo, Reembolsavel: true,  Status:0};
		    Viagem.get({ id: Auth.currentUser.IdentificadorViagem }, function (data) {
		        itemLista.Moeda = data.Moeda;
		        vm.EditarListaCompra(itemLista);

		    });
		};

		vm.EditarListaCompra = function (itemLista) {
		    $uibModal.open({
		        templateUrl: 'Sistema/PedidoCompraEdicao',
		        controller: 'PedidoCompraEditCtrl',
		        controllerAs: 'itemListaCompraEdit',
		        resolve: {
		            EscopoAtualizacao: vm,
		            ItemListaCompra: function () { return itemLista }
		        }
		    });
		};

		vm.AtualizarListaCompra = function (itemOriginal, itemLista) {
		    if (itemOriginal.Identificador) {
		        var Posicao = vm.ListaDados.indexOf(itemOriginal);
		        vm.ListaDados.splice(Posicao, 1, itemLista);
		    }
		    else {
		        vm.ListaDados.push(itemLista);
		    }
		};

        vm.VerificarExclusao = function (itemExcluir) {
            vm.modalPopupTrigger(itemExcluir, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
                vm.loading = true;
                ListaCompra.delete({ id: itemExcluir.Identificador }, function (data) {
                    if (data.Sucesso) {
                        var posicao = vm.ListaDados.indexOf(itemExcluir);
                        vm.ListaDados.splice(posicao, 1);
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

                    Error.showError('error', 'Ops!', $translate.instant("ErroExcluir"), true);
                    vm.loading = false;
                })
            });
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


        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            vm.filtroAtualizacao.Index = 0;
            vm.filtroAtualizacao.Count = null;

            vm.CamposInvalidos = {};
            vm.messages = [];

            ListaCompra.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
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
            data: 'itemListaCompra.ListaDados',           
            			columnDefs: [
				{ field: 'Identificador', displayName: '', cellTemplate: "BotoesGridTemplate.html", width: 60, },
                { field: 'Marca', displayName: $translate.instant('ListaCompra_Marca'), },
				{field:'Descricao', displayName: $translate.instant('ListaCompra_Descricao'),},
				{field:'ValorMaximo', displayName: $translate.instant('ListaCompra_ValorMaximo'),cellFilter: 'number:\'2\'' },
				{ field: 'MoedaSigla', displayName: $translate.instant('ListaCompra_Moeda'), },
				{field:'Comprado', displayName: $translate.instant('ListaCompra_Comprado'),},
				{ field: 'ItemUsuarioPedido.Nome', displayName: $translate.instant('ListaCompra_PedidoPara'), },
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