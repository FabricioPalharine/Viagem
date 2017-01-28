(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ConsultaFotoCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService',  'Viagem', 'Foto',
            'Atracao', 'Hotel', 'Refeicao',  'SignalR', ConsultaFotoCtrl]);

	function ConsultaFotoCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService,   Viagem, Foto, Atracao, Hotel, Refeicao,  SignalR) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0,Atracoes: [], Hoteis: [], Refeicoes:[] };
		vm.filtroAtualizacao = {  Index: 0, Count: 0 };
		vm.loading = false;
	
		vm.modalDelete = {};
		
		vm.ListaFotos = [];

	
		vm.ListaHotel = [];
		vm.ListaAtracao = [];
		vm.ListaRefeicao = [];

		

		vm.load = function () {
			vm.loading = true;
			
			Atracao.CarregarFoto(function (lista) {
			    vm.ListaAtracao = lista;
			    vm.filtro.Atracoes = lista;
			});
			Hotel.CarregarFoto(function (lista) {
			    vm.ListaHotel = lista;
			    vm.filtro.Hoteis = lista;
			});
			Refeicao.CarregarFoto(function (lista) {
			    vm.ListaRefeicao = lista;
			    vm.filtro.Refeicoes = lista;
			});
			
			vm.CarregarDadosWebApi();

				};



        vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);                  

            vm.filtroAtualizacao.Hoteis = null;
            vm.filtroAtualizacao.Atracoes = null;
            vm.filtroAtualizacao.Refeicoes = null;
           
            vm.filtroAtualizacao.ListaHoteis = [];
            angular.forEach(vm.filtro.Hoteis, function (c) {
                if (c.Selecionado)
                    vm.filtroAtualizacao.ListaHoteis.push(c.Identificador);
            });

            vm.filtroAtualizacao.ListaAtracoes = [];
            angular.forEach(vm.filtro.Atracoes, function (c) {
                if (c.Selecionado)
                    vm.filtroAtualizacao.ListaAtracoes.push(c.Identificador);
            });

            vm.filtroAtualizacao.ListaRefeicoes = [];
            angular.forEach(vm.filtro.Refeicoes, function (c) {
                if (c.Selecionado)
                    vm.filtroAtualizacao.ListaRefeicoes.push(c.Identificador);
            });

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
            vm.filtroAtualizacao.Count = 500000;

           
            Foto.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.ListaFotos = data.Lista.map(function (v) {
                    return {
                        title: moment(v.Data).format("DD/MM/YYYY HH:mm") + (v.Comentario ? " - " + v.Comentario : ""),
                        thumbUrl: v.LinkThumbnail,
                        url: v.LinkFoto,
                        video: v.Video
                    };
                });
                             
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
