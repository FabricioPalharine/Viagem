(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('LojaCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Cidade', 'Viagem', 'Atracao', 'SignalR', 'Loja', LojaCtrl]);

	function LojaCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Cidade, Viagem, Atracao, SignalR, Loja) {
		var vm = this;

		vm.filtro = { Index: 0, Count: 0 };
		vm.filtroAtualizacao = { Index: 0, Count: 0 };
		vm.loading = false;
		
	
		vm.ListaAtracao = [];
		vm.ListaCidades = [];
		vm.itemCidade = null;
		vm.ListaParticipantes = [];
		vm.ListaDados = [];
		

		vm.load = function () {
		    vm.loading = true;
		    Cidade.CarregarLoja(function (lista) {
		        vm.ListaCidades = lista;

		    });
		    Atracao.CarregarFoto(function (lista) {
		        vm.ListaAtracao = lista;
		    });
		    Viagem.CarregarParticipantes(function (lista) {
		        vm.ListaParticipantes = lista;
		    });
		    vm.CarregarDadosWebApi(vm.AjustarDadosPagina);

		};
		vm.Cancelar = function (itemForDelete) {
		    var posicao = vm.ListaDados.indexOf(itemForDelete);
		    vm.ListaDados.splice(posicao, 1);
		    vm.ItemAtual--;
		};

		vm.Excluir = function (itemForDelete) {
		    vm.loading = true;
		    Loja.delete({ id: itemForDelete.Identificador }, function (data) {
		        if (data.Sucesso) {
		            var posicao = vm.ListaDados.indexOf(itemForDelete);
		            vm.ListaDados.splice(posicao, 1);
		            Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
		            SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'L', itemForDelete.Identificador, false);

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

		    if (vm.itemCidade && vm.itemCidade.Identificador)
		        vm.filtroAtualizacao.IdentificadorCidade = vm.itemCidade.Identificador;
		    else
		        vm.filtroAtualizacao.IdentificadorCidade = null;

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


		vm.CriarNovaLoja = function () {
		    var itemLoja = { Avaliacoes: [], Compras: [], Data: moment(new Date()).format("YYYY-MM-DD"), strHora: moment(new Date()).format("HH:mm:ss") };
		    Atracao.VerificarAtracaoAberto(function (itemAberto) {
		        if (itemAberto.Identificador)
		            vm.modalPopupTrigger(itemAberto, $translate.instant('Loja_AssociaPai').format(itemAberto.Nome), $translate.instant('Sim'), $translate.instant('Nao'), function () {
		                itemLoja.IdentificadorAtracao = itemAberto.Identificador;
		                itemLoja.ItemAtracao = itemAberto;
		                itemLoja.Latitude = itemAberto.Latitude;
		                itemLoja.Longitude = itemAberto.Longitude;
		            });
		    });
		    vm.ListaDados.unshift(itemLoja);
		};


		vm.AjustarLojaSalva = function (itemLoja, ItemRegistro) {
		    var Posicao = vm.ListaDados.indexOf(itemLoja);
		    ItemRegistro.Avaliacoes = null;
		    vm.ListaDados.splice(Posicao, 1, ItemRegistro);
		    SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'L', ItemRegistro.Identificador, itemLoja.Identificador == null);

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


		    Loja.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
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
