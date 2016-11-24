(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('AtracaoCtrl',['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService','Cidade','Viagem','Atracao','SignalR', AtracaoCtrl]);

	function AtracaoCtrl($uibModal,  Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService,Cidade,Viagem,Atracao,SignalR) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0 , Situacao:1};
		vm.filtroAtualizacao = { Index: 0, Count: 0, Situacao: 1 };
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
		vm.TamanhoPagina = 2;
		vm.ItemAtual = 0;
		vm.ScrollEnabled = false;

		vm.load = function () {
		    vm.loading = true;
		    vm.enableScroll = true;
			Cidade.CarregarAtracao(function (lista) {
			    vm.ListaCidades = lista;

			});
			Atracao.CarregarFoto(function (lista) {
			    vm.ListaAtracao = lista;
			});
			Viagem.CarregarParticipantes(function (lista) {
			    vm.ListaParticipantes = lista;
			});
			vm.CarregarDadosWebApi(5, 0, vm.AjustarDadosPagina);

			SignalR.AvisarAlertaAtualizacao = function (TipoAtualizacao, Identificador, Inclusao) {
			    if (TipoAtualizacao == "A") {
			        var itemPesquisa = { Index: 0, Count: 1, Identificador: Identificador };

			        var itens = $.grep(vm.ListaDados, function (e) { return e.Identificador == Identificador; });
			        if (itens.length == 0 && Inclusao ) {
			            Atracao.list({ json: JSON.stringify(itemPesquisa) }, function (data) {
			                vm.ListaDados.unshift(data.Lista[0]);
			            }, function (err) {
			                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
			            });
			        }
			        else if (itens.length > 0) {
			            var Posicao = vm.ListaDados.indexOf(itens[0]);
			            Atracao.list({ json: JSON.stringify(itemPesquisa) }, function (data) {
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
			Atracao.delete({ id: itemForDelete.Identificador }, function (data) {
				if (data.Sucesso) {
				    var posicao = vm.ListaDados.indexOf(itemForDelete);
				    vm.ListaDados.splice(posicao, 1);
				    vm.ItemAtual--;
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

            vm.ItemAtual = 0;

            vm.CarregarDadosWebApi(5, vm.ItemAtual, vm.AjustarDadosPagina);
        };

        vm.modalPopupTrigger = function (itemForDelete, Mensagem,TextoBotaoOK, TextoBotaoCancel, callbackOk, callbackCancel) {
            vm.askDelete(itemForDelete, Mensagem,TextoBotaoOK, TextoBotaoCancel)
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
                controller: ['$uibModalInstance', 'item',  'MensagemConfirmacao','TextoBotaoOK', 'TextoBotaoCancel', vm.DeleteModalCtrl],
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

        vm.DeleteModalCtrl = function ($uibModalInstance, itemForDelete, MensagemConfirmacao,TextoBotaoOK, TextoBotaoCancel) {
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


        vm.CriarNovaAtracao = function () {
            var itemAtracao = { Avaliacoes: [], Fotos :[], Custos:[]};
            Atracao.VerificarAtracaoAberto(function (itemAberto) {
                if (itemAberto.Identificador)
                    vm.modalPopupTrigger(itemAberto, $translate.instant('Atracao_AssociaPai').format(itemAberto.Nome), $translate.instant('Sim'), $translate.instant('Nao'), function () {
                        itemAtracao.IdentificadorAtracaoPai = itemAberto.Identificador;
                        itemAtracao.ItemAtracaoPai = itemAberto;
                        itemAtracao.Latitude = itemAberto.Latitude;
                        itemAtracao.Longitude = itemAberto.Longitude;
                    });
            });
            vm.ListaDados.unshift(itemAtracao);
        };


        vm.AjustarAtracaoSalva = function (itemAtracao, ItemRegistro) {
            var Posicao = vm.ListaDados.indexOf(itemAtracao);
            ItemRegistro.Avaliacoes = null;
            vm.ListaDados.splice(Posicao, 1, ItemRegistro);
            SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'A', ItemRegistro.Identificador, itemAtracao.Identificador == null);

            vm.ItemAtual++;
        };
       
       vm.TrocarPagina = function() { 
            vm.CarregarDadosWebApi(vm.TamanhoPagina, vm.ItemAtual, function (data) {
                vm.ItemAtual += data.Lista.length;
                angular.forEach(data.Lista, function (c) {
                    vm.ListaDados.push(c);
                });
                vm.ScrollEnabled = true;
                if (!$scope.$$phase) {
                    $scope.$apply();
                }
            });
        };

        vm.AjustarDadosPagina = function (data) {
            // var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
            vm.ListaDados = data.Lista;
            vm.ItemAtual = data.Lista.length;
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
        vm.CarregarDadosWebApi = function (pageSize, page,callback) {
            vm.loading = true;
            vm.filtroAtualizacao.Index = page;
            vm.filtroAtualizacao.Count = pageSize;
            vm.ScrollEnabled = false;
           

            Atracao.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
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
