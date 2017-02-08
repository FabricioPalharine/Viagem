(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('GastoCtrl',['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService','Usuario','Viagem','Gasto','SignalR', GastoCtrl]);

	function GastoCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Usuario, Viagem, Gasto, SignalR) {
		var vm = this;
		vm.filtro = { Index: null, Count: null , IdentificadorParticipante: Auth.currentUser.Codigo};
		vm.filtroAtualizacao = { Index: null, Count: null,   IdentificadorParticipante: Auth.currentUser.Codigo };
		vm.loading = false;
		vm.showModal = false;
		vm.modalAcao = function () {;
			vm.showModal = true;
		}
		vm.modalDelete = {};
		vm.ListaDados = [];
		vm.itemUsuario = {};
		vm.ListaParticipantes = [];

		vm.load = function () {
			vm.loading = true;
			Viagem.CarregarParticipantes(function (lista) {
			    vm.ListaParticipantes = lista;
			});
			vm.CarregarDadosWebApi();
			
		};

		vm.CriarNovoGasto = function () {
		    var itemGasto = {
		        Dividido: false, Data: moment(new Date()).format("YYYY-MM-DD"), Especie: true, IdentificadorUsuario: Auth.currentUser.Codigo, ApenasBaixa: false, Atracoes: [], Hoteis: [],
		        Compras: [], Alugueis: [], Refeicoes: [], ViagenAereas: [], Usuarios: [], Reabastecimentos: [] 
		    };
		    vm.ListaDados.unshift(itemGasto);
		};

		vm.Cancelar = function (itemForDelete) {
		    var posicao = vm.ListaDados.indexOf(itemForDelete);
		    vm.ListaDados.splice(posicao, 1);
		    vm.ItemAtual--;
		};


		vm.Excluir = function (itemForDelete) {
		    vm.loading = true;
		    Gasto.delete({ id: itemForDelete.Identificador }, function (data) {
		        if (data.Sucesso) {
		            var posicao = vm.ListaDados.indexOf(itemForDelete);
		            vm.ListaDados.splice(posicao, 1);
		            Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
		            SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'G', itemForDelete.Identificador, false);

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

            //if (vm.itemUsuario && vm.itemUsuario.Identificador)
            //    vm.filtroAtualizacao.IdentificadorParticipante = vm.itemUsuario.Identificador;
            //else
            //    vm.filtroAtualizacao.IdentificadorParticipante = null;

            if (vm.filtroAtualizacao.DataInicioDe) {
                if (typeof vm.filtroAtualizacao.DataInicioDe == "string") {

                }
                else
                    vm.filtroAtualizacao.DataInicioDe = $.datepicker.formatDate("yy-mm-ddT00:00:00", vm.filtroAtualizacao.DataInicioDe);
            }

            if (vm.filtroAtualizacao.DataInicioAte) {
                if (typeof vm.filtroAtualizacao.DataInicioAte == "string") {

                }
                else
                    vm.filtroAtualizacao.DataInicioAte = $.datepicker.formatDate("yy-mm-ddT00:00:00", vm.filtroAtualizacao.DataInicioAte);
            }

     
            vm.CarregarDadosWebApi();
        };

        vm.AtualizarGasto = function (itemGasto, ItemRegistro) {
            var Posicao = vm.ListaDados.indexOf(ItemRegistro);
            vm.ListaDados.splice(Posicao, 1, itemGasto);
            SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'G', ItemRegistro.Identificador, ItemRegistro.Identificador == null);

        };

        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };

        vm.CarregarDadosWebApi = function () {
            vm.loading = true;


            vm.CamposInvalidos = {};
            vm.messages = [];

            Gasto.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                vm.ListaDados = data.Lista;
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

     
	}
}());
