(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('CotacaoMoedaCtrl',['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService','Viagem','CotacaoMoeda','Dominio', CotacaoMoedaCtrl]);

	function CotacaoMoedaCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Viagem, CotacaoMoeda, Dominio) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0 };
		vm.filtroAtualizacao = {  Index: 0, Count: 0 };
		vm.loading = false;
		vm.showModal = false;		
		vm.modalDelete = {};
		vm.ListaDados = [];
		vm.gridApi = null;
		vm.messages = [];

		vm.load = function () {
		    vm.loading = true;

		    Dominio.CarregaMoedas(function (data) {
		        vm.ListaMoeda = data;
		    });

			vm.CarregarDadosWebApi();
		};

		vm.delete = function (itemForDelete, callback) {
			vm.loading = true;
			CotacaoMoeda.delete({ id: itemForDelete.Identificador }, function (data) {
				callback(data);
				if (data.Sucesso) {
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

		vm.RemoverMoeda = function (itemForDelete) {
            // $uibModalInstance.close();
            $uibModal.open({
                templateUrl: 'modalDelete.html',
                controller: ['$uibModalInstance', 'item',  vm.DeleteModalCtrl],
                controllerAs: 'vmDelete',
                resolve: {
                    item: function () { return itemForDelete; },
                }
            });
        };

        vm.DeleteModalCtrl = function ($uibModalInstance, itemForDelete) {
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
                });
            };
        }; 

        vm.AjustarDadosPagina = function (data) {
            vm.ListaDados = [];
            angular.forEach(            data.Lista, function (itemLista) {
                itemLista.ItemMoeda = { Codigo: itemLista.Moeda };
                vm.ListaDados.push(itemLista);
            });
           
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

            vm.messages = [];

            CotacaoMoeda.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
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

        vm.AdicionarNovo = function () {
            var itemCotacao = { Edicao: true, Original: null };
            Viagem.get({ id: Auth.currentUser.IdentificadorViagem }, function (data) {
                itemCotacao.ItemMoeda = { Codigo: data.Moeda };
            });
            vm.ListaDados.push(itemCotacao);
        };

        vm.EditarMoeda = function (itemCotacao) {
            var itemOriginal = jQuery.extend({}, itemCotacao);
            itemCotacao.Edicao = true;
            itemCotacao.Original = itemOriginal;
        };


        vm.CancelarMoeda = function (itemEvento) {
            if (!itemEvento.Identificador) {
                var Posicao = vm.ListaDados.indexOf(itemEvento);
                vm.ListaDados.splice(Posicao, 1);
            }
            else {
                itemEvento.Edicao = false;
                itemEvento.ItemMoeda = itemEvento.Original.ItemMoeda;
                itemEvento.DataCotacao = itemEvento.Original.DataCotacao;
                itemEvento.ValorCotacao = itemEvento.Original.ValorCotacao;
                itemEvento.Moeda = itemEvento.Original.Moeda;
                itemEvento.Original = null;
            }
        };

        vm.SalvarMoeda = function (itemCotacao) {
            if (itemCotacao.ItemMoeda && itemCotacao.ItemMoeda.Codigo)
                itemCotacao.Moeda = itemCotacao.ItemMoeda.Codigo;
            else
                itemCotacao.Moeda = null;
            vm.messages = [];
            CotacaoMoeda.save(itemCotacao, function (data) {
                vm.loading = false;
                if (data.Sucesso) {
                    var itemNovo = data.ItemRegistro;
                    itemNovo.ItemMoeda = { Codigo: itemNovo.Moeda };
                       var Posicao = vm.ListaDados.indexOf(itemCotacao);
                        vm.ListaDados.splice(Posicao, 1, itemNovo);
                            


                } else {
                    vm.messages = data.Mensagens;
                }
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
            });
        };
	}
}());
