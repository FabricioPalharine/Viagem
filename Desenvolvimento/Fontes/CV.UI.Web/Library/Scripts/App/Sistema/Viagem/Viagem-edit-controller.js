(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('ViagemEditCtrl', ['Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Usuario', 'Viagem', 'Dominio','$uibModal', ViagemEditCtrl]);

    function ViagemEditCtrl(Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Usuario, Viagem, Dominio, $uibModal) {
        var vm = this;
        vm.filtroConsulta = {};
        vm.itemViagem = { Aberto: true, UnidadeMetrica: true, QuantidadeParticipantes: 1, PublicaGasto: false, UsuariosGastos: [], Participantes: [] };
        vm.loading = false;
        vm.inclusao = false;
        vm.submitted = false;
        vm.messages = [];
        vm.edicao = false;
        vm.loggedUser = Auth.currentUser;
        vm.CamposInvalidos = {};
        vm.itemUsuarioCusto = null;
        vm.ListaUsuario = [];
        vm.itemParticipante = null;
        vm.ListaMoeda = [];
        vm.itemMoeda = {Codigo: 790}
        vm.load = function () {
            vm.loading = true;
            var param = $stateParams;
            vm.filtroConsulta = param.filtro;
            Usuario.listaAmigos(function (data) {
                vm.ListaUsuario = data;
            },
			function (err) {
			    Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
			});
            Dominio.CarregaMoedas(function (data) {
                vm.ListaMoeda = data;
            },
			function (err) {
			    Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
			});

            if (param.id !== undefined && param.id !== '') {
                vm.edicao = true;
                Viagem.get({ id: param.id }, function (data) {
                    vm.itemViagem = data;
                    vm.itemMoeda = { Codigo: vm.itemViagem.Moeda };
                    vm.loading = false;
                }, function (err) {
                    vm.loading = false;
                    Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
                });
            }
            else {
                vm.itemViagem.IdentificadorUsuario = Auth.currentUser.Codigo;
                var itemParticipante = { IdentificadorUsuario: Auth.currentUser.Codigo, ItemUsuario: { Identificador: Auth.currentUser.Codigo, Nome: Auth.currentUser.Nome } };
                vm.itemViagem.Participantes.push(itemParticipante);
                vm.inclusao = true;
                vm.loading = false
            };
        };

        $rootScope.$on('loggin', function (event) {
            if (!vm.itemViagem.IdentificadorUsuario)
                vm.itemViagem.IdentificadorUsuario = Auth.currentUser.Codigo;
        });

        vm.save = function (form) {
            vm.messages = [];
            vm.submitted = true;
            vm.CamposInvalidos = {};
            if (form.$valid) {

                if (vm.itemViagem.DataInicio) {
                    if (typeof vm.itemViagem.DataInicio == "string") {
                        var date = Date.parse(vm.itemViagem.DataInicio);
                        if (!isNaN(date))
                            vm.itemViagem.DataInicio = $.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
                    }
                    else
                        vm.itemViagem.DataInicio = $.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemViagem.DataInicio);
                }
                if (vm.itemViagem.DataFim) {
                    if (typeof vm.itemViagem.DataFim == "string") {
                        var date = Date.parse(vm.itemViagem.DataFim);
                        if (!isNaN(date))
                            vm.itemViagem.DataFim = $.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
                    }
                    else
                        vm.itemViagem.DataFim = $.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemViagem.DataFim);
                }
                if (vm.itemMoeda && vm.itemMoeda.Codigo)
                    vm.itemViagem.Moeda = vm.itemMoeda.Codigo;
                else
                    vm.itemViagem.Moeda = null;

                Viagem.save(vm.itemViagem, function (data) {
                    vm.loading = false;
                    if (data.Sucesso) {
                        Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
                        if (vm.inclusao)
                        {
                            Auth.SelecionarViagem(data.IdentificadorRegistro);
                        }
                    } else {
                        vm.messages = data.Mensagens;
                        vm.verificaCampoInvalido();
                    }
                }, function (err) {
                    vm.loading = false;
                    Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
                });
            }
            vm.submitted = false;
        };
        //
        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };
        //
        vm.verificaCampoInvalido = function () {
            vm.CamposInvalidos = {

            };
            //  var _retorno = false;
            $(vm.messages).each(function (i, item) {
                vm.CamposInvalidos[item.Campo] = true;
            });
        };


        vm.modalPopupTriggerParticipante = function (itemForDelete, indexForDelete, Mensagem) {
            vm.askDelete(itemForDelete, indexForDelete, Mensagem)
          .then(function (data) {
              vm.itemViagem.Participantes.splice(indexForDelete, 1);

          })
          .then(null, function (reason) {

          });
        };

        vm.modalPopupTriggerCusto = function (itemForDelete, indexForDelete, Mensagem) {
            vm.askDelete(itemForDelete, indexForDelete, Mensagem)
          .then(function (data) {
              vm.itemViagem.UsuariosGastos.splice(indexForDelete, 1);
          })
          .then(null, function (reason) {

          });
        };

        vm.askDelete = function (itemForDelete, indexForDelete, Mensagem) {
            // $uibModalInstance.close();
            var modal = $uibModal.open({
                templateUrl: 'modalDelete.html',
                controller: ['$uibModalInstance', 'item', 'index', 'MensagemConfirmacao', vm.DeleteModalCtrl],
                controllerAs: 'vmDelete',
                resolve: {
                    item: function () { return itemForDelete; },
                    index: function () { return indexForDelete; },
                    MensagemConfirmacao: function () { return Mensagem; },
                }
            });

            return modal.result;
        };

        vm.DeleteModalCtrl = function ($uibModalInstance, itemForDelete, indexForDelete, MensagemConfirmacao) {
            var vmDelete = this;
            vmDelete.MensagemConfirmacao = MensagemConfirmacao;
            vmDelete.itemForDelete = itemForDelete;

            vmDelete.close = function () {
                $uibModalInstance.dismiss();
            };

            vmDelete.back = function () {
                $uibModalInstance.dismiss();
                vm.actionModal();
            };

            vmDelete.delete = function () {

                $uibModalInstance.close(vmDelete.itemForDelete);
            };
        };

        vm.gridOptions = {
            data: 'itemViagemEdit.itemViagem.Participantes',
            columnDefs: [
                { field: 'Identificador', displayName: '', cellTemplate: "BotoesGridTemplate.html", width: 60, },
                { field: 'ItemUsuario.Nome', displayName: $translate.instant('Amigo_Nome'), },

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

            },
            useExternalPagination: false,
            useExternalSorting: false,
            appScopeProvider: vm
        };

        vm.gridOptionsCusto = {
            data: 'itemViagemEdit.itemViagem.UsuariosGastos',
            columnDefs: [
                { field: 'Identificador', displayName: '', cellTemplate: "BotoesGridTemplateCusto.html", width: 60, },
                { field: 'ItemUsuario.Nome', displayName: $translate.instant('Amigo_Nome'), },

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

            },
            useExternalPagination: false,
            useExternalSorting: false,
            appScopeProvider: vm
        };

        vm.AdicionarParticipante = function () {
            vm.messages = [];
            vm.CamposInvalidos = {

            };
            if (vm.itemParticipante == null || !vm.itemParticipante.Identificador) {
                vm.messages.push({ Mensagem: $translate.instant("Viagem_SelecionarAmigo") });
                vm.CamposInvalidos.IdentificadorParticipante = true;
            }
            else if ($.grep(vm.itemViagem.Participantes, function (e) { return e.IdentificadorUsuario == vm.itemParticipante.Identificador; }).length > 0) {
                vm.messages.push({ Mensagem: $translate.instant("Viagem_AmigoJaAdicionado") });
                vm.CamposInvalidos.IdentificadorParticipante = true;
            }
            else {
                var itemParticipante = { IdentificadorUsuario: vm.itemParticipante.Identificador, ItemUsuario: { Identificador: vm.itemParticipante.Identificador, Nome: vm.itemParticipante.Nome } };
                vm.itemViagem.Participantes.push(itemParticipante);

            }
        };

        vm.AdicionarCusto = function () {
            vm.messages = [];
            vm.CamposInvalidos = {

            };
            if (vm.itemUsuarioCusto == null || !vm.itemUsuarioCusto.Identificador) {
                vm.messages.push({ Mensagem: $translate.instant("Viagem_SelecionarAmigo") });
                vm.CamposInvalidos.IdentificadorParticipante = true;
            }
            else if ($.grep(vm.itemViagem.UsuariosGastos, function (e) { return e.IdentificadorUsuario == vm.itemUsuarioCusto.Identificador; }).length > 0) {
                vm.messages.push({ Mensagem: $translate.instant("Viagem_AmigoJaAdicionado") });
                vm.CamposInvalidos.IdentificadorParticipante = true;
            }
            else {
                var itemParticipante = { IdentificadorUsuario: vm.itemUsuarioCusto.Identificador, ItemUsuario: { Identificador: vm.itemUsuarioCusto.Identificador, Nome: vm.itemUsuarioCusto.Nome } };
                vm.itemViagem.UsuariosGastos.push(itemParticipante);

            }
        };
    }
}());
