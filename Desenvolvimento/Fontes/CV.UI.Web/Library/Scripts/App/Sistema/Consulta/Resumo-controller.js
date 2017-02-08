(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ResumoCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Viagem', 'Consulta', 'Dominio','$compile', ResumoCtrl]);

	function ResumoCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Viagem, Consulta, Dominio, $compile) {
		var vm = this;
		vm.filtro = { Index: 0, Count: 0 };
		vm.filtroAtualizacao = { Index: 0, Count: 0 };
		vm.loading = false;
		vm.ItemResumo = false;
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
        
        vm.timeFunction = function (timeObj) {

            if (timeObj) {
                var obj = moment.duration(timeObj);
                return Math.floor(obj.asHours()) + moment.utc(obj.asMilliseconds()).format(":mm")
            }
            else
                return null;

        };
    
//
        vm.CarregarDadosWebApi = function () {
            vm.loading = true;
            vm.filtroAtualizacao.Index = 0;
            vm.filtroAtualizacao.Count = null;

            vm.CamposInvalidos = {};
            vm.messages = [];

            Consulta.CarregarResumo({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                vm.ItemResumo = data;

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
  	}
}());
