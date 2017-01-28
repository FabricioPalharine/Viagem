(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('CalendarioRealizadoCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Viagem', 'Consulta', 'Dominio','$compile', CalendarioRealizadoCtrl]);

	function CalendarioRealizadoCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Viagem, Consulta, Dominio, $compile) {
		var vm = this;
		vm.filtro = { Index: 0, Count: 0 };
		vm.filtroAtualizacao = { Index: 0, Count: 0 };
		vm.loading = false;
		vm.ListaDados = [];
		vm.ListaEventos = [vm.ListaDados]

		vm.itemUsuario = {};
		vm.ListaUsuarios = [];
		vm.Total = 0;
		vm.load = function () {
		    vm.loading = true;

		    Viagem.CarregarParticipantesAmigo(function (lista) {

		        vm.ListaUsuarios = lista;
		        var listaEu = lista.filter(function (v) { return v.Identificador == Auth.currentUser.Codigo; });
		        if (listaEu.length > 0)
		            vm.itemUsuario = listaEu[0];
		        else
		            vm.itemUsuario = lista[0];
		        vm.filtroAtualizacao.IdentificadorParticipante = vm.itemUsuario.Identificador;
		        vm.CarregarDadosWebApi();

		    });
		    
		};


        vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);
                     
            if (vm.itemUsuario && vm.itemUsuario.Identificador)
                vm.filtroAtualizacao.IdentificadorParticipante = vm.itemUsuario.Identificador;
            else
                vm.filtroAtualizacao.IdentificadorParticipante = Auth.currentUser.Codigo;
    
            vm.CarregarDadosWebApi();
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

            Consulta.ListarCalendarioRealizado({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                while (vm.ListaDados.length > 0) {
                    vm.ListaDados.pop();
                }
                angular.forEach(data.map(function (v) {
                    return {
                        id: v.Id + v.Tipo,
                        title: v.Titulo,
                        start: moment(v.DataInicio),
                        end: moment(v.DataFim),
                        editable: false
                    };
                }), function (item) {
                    vm.ListaDados.push(item);
                });

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

        vm.eventRender = function (event, element, view) {
            element.attr({
                'uib-tooltip': event.title,
                'tooltip-append-to-body': true
            });
            $compile(element)($scope);
        };

        vm.uiConfig = {
            calendar: {
                height: 450,
                editable: false,
                header: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'month,agendaWeek,agendaDay,listMonth'
                },
                defaultView: 'agendaWeek',
                locale: vm.Idioma(),
                lazyFetching: false,
                allDaySlot: false,
                eventRender: vm.eventRender
            }
        };
//
  	}
}());
