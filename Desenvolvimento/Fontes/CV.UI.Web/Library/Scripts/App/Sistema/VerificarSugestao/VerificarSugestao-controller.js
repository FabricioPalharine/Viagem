(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('VerificarSugestaoCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Cidade', 'Usuario', 'Viagem', 'Sugestao', 'SignalR', VerificarSugestaoCtrl]);

	function VerificarSugestaoCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Cidade, Usuario, Viagem, Sugestao, SignalR) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0, Situacao:1 };
		vm.filtroAtualizacao = {  Index: 0, Count: 0, Situacao:1 };
		vm.loading = false;		
		vm.ListaDados = [];
		vm.gridApi = null;
		vm.ListaCidades = [];
		vm.itemCidade = {};
		vm.ListaAmigo = [];
		vm.itemAmigo = {};
		vm.load = function () {
			vm.loading = true;
			Cidade.CarregarSugestao(function (lista) {
			    vm.ListaCidades = lista;

			});

			Usuario.listaAmigos(function (data) {
			    vm.ListaAmigo = data;
			});

			vm.CarregarDadosWebApi();
		};

		vm.delete = function (itemForDelete, callback) {
		    vm.loading = true;
		    itemForDelete.Status = 3;
		    Sugestao.save(itemForDelete, function (data) {
		        if (data.Sucesso) {
		            SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'S', itemForDelete.Identificador, false);

		            callback(data);
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



        vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);
            if (vm.itemCidade && vm.itemCidade.Identificador)
                vm.filtroAtualizacao.IdentificadorCidade = vm.itemCidade.Identificador;
            else
                vm.filtroAtualizacao.IdentificadorCidade = null;

            if (vm.itemAmigo && vm.itemAmigo.Identificador)
                vm.filtroAtualizacao.IdentificadorParticipante = vm.itemAmigo.Identificador;
            else
                vm.filtroAtualizacao.IdentificadorParticipante = null;

            vm.CarregarDadosWebApi();
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
//
        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            vm.filtroAtualizacao.Index = 0;
            vm.filtroAtualizacao.Count = null;

                     vm.CamposInvalidos = {};
            vm.messages = [];

            Sugestao.listarConsulta({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                vm.AjustarDadosPagina(data);
              

               
                vm.loading = false;
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
                
                vm.loading = false;
            });
        };


        vm.Editar = function (ItemSugestao) {
            $uibModal.open({
                templateUrl: 'Sistema/VerificarSugestaoEdicao',
                controller: 'VerificarSugestaoEditCtrl',
                controllerAs: 'itemVerificarSugestaoEdit',
                resolve: {
                    EscopoAtualizacao: vm,
                    ItemSugestao: function () { return ItemSugestao }
                }
            });
        };
        
//
        vm.gridOptions = {
            data: 'itemVerificarSugestao.ListaDados',           
            			columnDefs: [
				{ field: 'Identificador', displayName: '', cellTemplate: "BotoesGridTemplate.html", width: 60, },
                { field: 'ItemUsuario.Nome', displayName: $translate.instant('Sugestao_Amigo'), },
				{ field: 'Local', displayName: $translate.instant('Sugestao_Local'), },
                { field: 'Tipo', displayName: $translate.instant('Sugestao_Tipo'), },
				{ field: 'Comentario', displayName: $translate.instant('Sugestao_Comentario'), },
                { field: 'ItemCidade.Nome', displayName: $translate.instant('Sugestao_IdentificadorCidade'), },
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
