(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ViagemCtrl',['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService','Usuario','Viagem', ViagemCtrl]);

	function ViagemCtrl($uibModal,  Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService,Usuario,Viagem) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0, Situacao:"true" };
		vm.filtroAtualizacao = {  Index: 0, Count: 0, Aberto:true };
		vm.loading = false;
		vm.showModal = false;
		vm.modalAcao = function () {;
			vm.showModal = true;
		}
		vm.ListaDados = [];
		vm.gridApi = null;
		vm.itemParticipante = null;
		vm.ListaUsuario = []
		vm.load = function () {
			vm.loading = true;

			var param = $stateParams;
			if (param.filtro != null) {
				vm.filtro = vm.filtroAtualizacao = param.filtro;
				 vm.pagingOptions.fields = vm.filtroAtualizacao.SortField;
				 vm.pagingOptions.directions = vm.filtroAtualizacao.SortOrder;
				vm.pagingOptions.currentPage = (vm.filtroAtualizacao.Index / vm.pagingOptions.pageSize) + 1;

			}
			Usuario.listaAmigosComigo(function (data)
			{
			    vm.ListaUsuario = data;
			})
			vm.CarregarDadosWebApi(vm.pagingOptions.pageSize, vm.pagingOptions.currentPage);
		};

	
        $rootScope.$on('loggin', function (event) {
            
        });

        vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);
            
            if (vm.filtroAtualizacao.Situacao == "")
                vm.filtroAtualizacao.Aberto = null;
            else
                vm.filtroAtualizacao.Aberto = vm.filtroAtualizacao.Situacao == "true"

            if (vm.itemParticipante && vm.itemParticipante.Identificador)
                vm.filtroAtualizacao.IdentificadorParticipante = vm.itemParticipante.Identificador;
            else
                vm.filtroAtualizacao.IdentificadorParticipante = null;

            if (vm.filtroAtualizacao.DataInicioDe) {
                if (typeof vm.filtroAtualizacao.DataInicioDe == "string") {
                    var date = Date.parse(vm.filtroAtualizacao.DataInicioDe);
                    if (!isNaN(date))
                        vm.filtroAtualizacao.DataInicioDe = $.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
                }
                else
                    vm.filtroAtualizacao.DataInicioDe = $.datepicker.formatDate("yy-mm-ddT00:00:00", vm.filtroAtualizacao.DataInicioDe);
            }

            if (vm.filtroAtualizacao.DataInicioAte) {
                if (typeof vm.filtroAtualizacao.DataInicioAte == "string") {
                    var date = Date.parse(vm.filtroAtualizacao.DataInicioAte);
                    if (!isNaN(date))
                        vm.filtroAtualizacao.DataInicioAte = $.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
                }
                else
                    vm.filtroAtualizacao.DataInicioAte = $.datepicker.formatDate("yy-mm-ddT00:00:00", vm.filtroAtualizacao.DataInicioAte);
            }

            if (vm.filtroAtualizacao.DataFimDe) {
                if (typeof vm.filtroAtualizacao.DataFimDe == "string") {
                    var date = Date.parse(vm.filtroAtualizacao.DataFimDe);
                    if (!isNaN(date))
                        vm.filtroAtualizacao.DataFimDe = $.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
                }
                else
                    vm.filtroAtualizacao.DataFimDe = $.datepicker.formatDate("yy-mm-ddT00:00:00", vm.filtroAtualizacao.DataFimDe);
            }

            if (vm.filtroAtualizacao.DataFimAte) {
                if (typeof vm.filtroAtualizacao.DataFimAte == "string") {
                    var date = Date.parse(vm.filtroAtualizacao.DataDataFimAteFim);
                    if (!isNaN(date))
                        vm.filtroAtualizacao.DataFimAte = $.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
                }
                else
                    vm.filtroAtualizacao.DataFimAte = $.datepicker.formatDate("yy-mm-ddT00:00:00", vm.filtroAtualizacao.DataFimAte);
            }

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

            Viagem.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
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
            data: 'itemViagem.ListaDados',           
            columnDefs: [
				{field:'Identificador',  displayName: '', cellTemplate: "BotoesGridTemplate.html",  width: 60,},
				{field:'Nome', displayName: $translate.instant('Viagem_Nome'),},
				{field:'DataInicio', displayName: $translate.instant('Viagem_DataInicio'),cellFilter: 'date:\'dd/MM/yyyy\'' },
				{field:'DataFim', displayName: $translate.instant('Viagem_DataFim'),cellFilter: 'date:\'dd/MM/yyyy\'' },
				{ field: 'AbertoTexto', displayName: $translate.instant('Viagem_Aberto'), },
                { field: 'Participantes', displayName: $translate.instant('Viagem_Participantes'), cellTemplate: "ListaParticipantes.html", },

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
            totalItems: vm.totalServerItems,
        };

        vm.SelecionarViagem = function (viagem) {
            Auth.SelecionarViagem(viagem.Identificador);
        };
	}
}());
