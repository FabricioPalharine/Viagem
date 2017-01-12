(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('ViagemAereaCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Cidade', 'Viagem', 'SignalR','Dominio', 'ViagemAerea', ViagemAereaCtrl]);

	function ViagemAereaCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Cidade, Viagem, SignalR,Dominio,ViagemAerea) {
		var vm = this;
		
		vm.filtro = { Index: 0, Count: 0, Situacao: 1 };
		vm.filtroAtualizacao = { Index: 0, Count: 0, Situacao: 1 };
		vm.loading = false;
		vm.modalDelete = {};
		vm.ListaCidades = [];
		vm.itemCidadeOrigem = null;
		vm.itemCidadeDestino = null;
		vm.itemTipo = null;
		vm.ListaParticipantes = [];
		vm.ListaDados = [];
		vm.ListaTipo = [];

		vm.load = function () {
		    vm.loading = true;
		    vm.enableScroll = true;
		    Cidade.CarregarViagemAerea(function (lista) {
		        vm.ListaCidades = lista;

		    });

		    Dominio.CarregaTipoTransporte(function (lista) {
		        vm.ListaTipo = lista;

		    });

		    Viagem.CarregarParticipantes(function (lista) {
		        vm.ListaParticipantes = lista;
		    });
		    vm.CarregarDadosWebApi(vm.AjustarDadosPagina);

		    
		};

		vm.Excluir = function (itemForDelete) {
		    vm.loading = true;
		    ViagemAerea.delete({ id: itemForDelete.Identificador }, function (data) {
		        if (data.Sucesso) {
		            var posicao = vm.ListaDados.indexOf(itemForDelete);
		            vm.ListaDados.splice(posicao, 1);
		            Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
		            SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'VA', itemForDelete.Identificador, false);

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
		};



		vm.filtraDado = function () {

		    vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);

		    if (vm.itemCidadeOrigem && vm.itemCidadeOrigem.Identificador)
		        vm.filtroAtualizacao.IdentificadorCidade = vm.itemCidadeOrigem.Identificador;
		    else
		        vm.filtroAtualizacao.IdentificadorCidade = null;

		    if (vm.itemCidadeDestino && vm.itemCidadeDestino.Identificador)
		        vm.filtroAtualizacao.IdentificadorCidade2 = vm.itemCidadeDestino.Identificador;
		    else
		        vm.filtroAtualizacao.IdentificadorCidade2 = null;

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

		    if (vm.itemTipo && vm.itemTipo.Codigo)
		        vm.filtroAtualizacao.TipoInteiro = vm.itemTipo.Codigo;
		    else
		        vm.filtroAtualizacao.TipoInteiro = null;


		    vm.CarregarDadosWebApi(vm.AjustarDadosPagina);
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


		vm.CriarNovaViagemAerea = function () {
		    var itemVA = {
		        Avaliacoes: [], Gastos: [], Aeroportos: [
                    { TipoPonto: 1 },
                    {TipoPonto: 2}
		        ]
		    };

		    vm.ListaDados.unshift(itemVA);
		};


		vm.AjustarViagemAereaSalva = function (itemHotel, ItemRegistro) {
		    var Posicao = vm.ListaDados.indexOf(itemHotel);
		    ItemRegistro.Avaliacoes = null;
		    vm.ListaDados.splice(Posicao, 1, ItemRegistro);
		    SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'VA', ItemRegistro.Identificador, itemHotel.Identificador == null);

		    vm.ItemAtual++;
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
	    //
		vm.CarregarDadosWebApi = function (callback) {
		    vm.loading = true;
		    vm.filtroAtualizacao.Index = 0;
		    vm.filtroAtualizacao.Count = null;
		    vm.ScrollEnabled = false;


		    ViagemAerea.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
		        vm.loading = false;
		        callback(data);


		        vm.loading = false;
		    }, function (err) {
		        vm.loading = false;
		        Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);

		        vm.loading = false;
		    });
		};

	}
}());
