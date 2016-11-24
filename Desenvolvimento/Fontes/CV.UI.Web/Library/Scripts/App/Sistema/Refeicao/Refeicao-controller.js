(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('RefeicaoCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Cidade', 'Viagem', 'Atracao', 'SignalR', 'Refeicao', RefeicaoCtrl]);
    
	function RefeicaoCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Cidade, Viagem, Atracao, SignalR, Refeicao) {
	    var vm = this;
	    vm.filtro = { Index: 0, Count: 0};
	    vm.filtroAtualizacao = { Index: 0, Count: 0};
	    vm.loading = false;
	    vm.showModal = false;
	    vm.modalAcao = function () {;
	        vm.showModal = true;
	    }
	    vm.modalDelete = {};
	    vm.ListaAtracao = [];
	    vm.ListaCidades = [];
	    vm.itemCidade = null;
	    vm.ListaParticipantes = [];
	    vm.ListaDados = [];
	    vm.ScrollEnabled = false;

	    vm.load = function () {
	        vm.loading = true;
	        vm.enableScroll = true;
	        Cidade.CarregarRefeicao(function (lista) {
	            vm.ListaCidades = lista;

	        });
	        Atracao.CarregarFoto(function (lista) {
	            vm.ListaAtracao = lista;
	        });
	        Viagem.CarregarParticipantes(function (lista) {
	            vm.ListaParticipantes = lista;
	        });
	        vm.CarregarDadosWebApi( vm.AjustarDadosPagina);

	        SignalR.AvisarAlertaAtualizacao = function (TipoAtualizacao, Identificador, Inclusao) {
	            if (TipoAtualizacao == "R") {
	                var itemPesquisa = { Index: 0, Count: 1, Identificador: Identificador };

	                var itens = $.grep(vm.ListaDados, function (e) { return e.Identificador == Identificador; });
	                if (itens.length == 0 && Inclusao) {
	                    Refeicao.list({ json: JSON.stringify(itemPesquisa) }, function (data) {
	                        vm.ListaDados.unshift(data.Lista[0]);
	                    }, function (err) {
	                        Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
	                    });
	                }
	                else if (itens.length > 0) {
	                    var Posicao = vm.ListaDados.indexOf(itens[0]);
	                    Refeicao.list({ json: JSON.stringify(itemPesquisa) }, function (data) {
	                        vm.ListaDados.splice(Posicao, 1, data.Lista[0]);
	                    }, function (err) {
	                        Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
	                    });
	                }
	            }
	        };
	    };

	    vm.Excluir = function (itemForDelete) {
	        vm.loading = true;
	        Refeicao.delete({ id: itemForDelete.Identificador }, function (data) {
	            if (data.Sucesso) {
	                var posicao = vm.ListaDados.indexOf(itemForDelete);
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

	     

	        vm.CarregarDadosWebApi( vm.AjustarDadosPagina);
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


	    vm.CriarNovaRefeicao = function () {
	        var itemRefeicao = { Pedidos: [], Fotos: [], Gastos: [], Data: moment(new Date()).format("YYYY-MM-DD"), strHora:  moment(new Date()).format("HH:mm:ss") };
	        Atracao.VerificarAtracaoAberto(function (itemAberto) {
	            if (itemAberto.Identificador)
	                vm.modalPopupTrigger(itemAberto, $translate.instant('Refeicao_AssociaPai').format(itemAberto.Nome), $translate.instant('Sim'), $translate.instant('Nao'), function () {
	                    itemRefeicao.IdentificadorAtracao = itemAberto.Identificador;
	                    itemRefeicao.ItemAtracao = itemAberto;
	                    itemRefeicao.Latitude = itemAberto.Latitude;
	                    itemRefeicao.Longitude = itemAberto.Longitude;
	                });
	        });
	        vm.ListaDados.unshift(itemRefeicao);
	    };


	    vm.AjustarRefeicaoSalva = function (itemRefeicao, ItemRegistro) {
	        var Posicao = vm.ListaDados.indexOf(itemRefeicao);
	        ItemRegistro.Pedidos = null;
	        vm.ListaDados.splice(Posicao, 1, ItemRegistro);
	        SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'R', ItemRegistro.Identificador, itemRefeicao.Identificador == null);

	        vm.ItemAtual++;
	    };


	    vm.AjustarDadosPagina = function (data) {
	        // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
	        vm.ListaDados = data.Lista;
	        vm.ScrollEnabled = true;
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


	        Refeicao.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
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
