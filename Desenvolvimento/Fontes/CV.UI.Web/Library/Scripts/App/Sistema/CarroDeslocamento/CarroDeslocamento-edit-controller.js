(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('CarroDeslocamentoEditCtrl', ['$uibModalInstance', 'Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Viagem', 'Carro', 'itemCarroDeslocamento', 'EscopoAtualizacao', CarroDeslocamentoEditCtrl]);

    function CarroDeslocamentoEditCtrl($uibModalInstance, Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Viagem, Carro, itemCarroDeslocamento, EscopoAtualizacao) {
        var vm = this;
        vm.itemOriginal = itemCarroDeslocamento;
        vm.itemCarroDeslocamento = jQuery.extend({}, itemCarroDeslocamento);
        vm.EscopoAtualizacao = EscopoAtualizacao;
        vm.ListaParticipantes = [];
        vm.messages = [];
        vm.CamposInvalidos = {};
        vm.FimDeslocamento = false;
        vm.loading = true

        vm.load = function () {
            vm.loading = true;
            vm.enableScroll = true;

            vm.FimDeslocamento = vm.itemCarroDeslocamento.ItemCarroEventoChegada.Data != null;
            Viagem.CarregarParticipantes(function (lista) {
                vm.ListaParticipantes = lista;
                angular.forEach(vm.ListaParticipantes, function (item) {
                    item.Selecionado = vm.itemCarroDeslocamento.Usuarios && $.grep(vm.itemCarroDeslocamento.Usuarios, function (e) {
                        return e.IdentificadorUsuario == item.Identificador;
                    }).length > 0;
                });
            });
           
            vm.loading = false;
        };

        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };

        vm.verificaCampoInvalido = function () {
            vm.CamposInvalidos = {

            };
            //  var _retorno = false;
            $(vm.messages).each(function (i, item) {
                vm.CamposInvalidos[item.Campo] = true;
            });
        };

        vm.AjustarHoraChegada = function () {
            if (vm.FimDeslocamento)
            {
                vm.itemCarroDeslocamento.ItemCarroEventoChegada.Data = moment(new Date()).format("YYYY-MM-DD");
                vm.itemCarroDeslocamento.ItemCarroEventoChegada.strHora = moment(new Date()).format("HH:mm:ss");
            }
            else
            {
                vm.itemCarroDeslocamento.ItemCarroEventoChegada.Data = vm.itemCarroDeslocamento.ItemCarroEventoChegada.strHora = null;
            }
        };

        vm.close = function () {
            if ($uibModalInstance)
                $uibModalInstance.close();
        };

        vm.SelecionarPosicao = function (item) {
            vm.EscopoAtualizacao.SelecionarPosicao(item);
        };

        vm.salvar = function () {
            vm.messages = [];
            vm.loading = true;
            vm.CamposInvalidos = {};

            if (vm.itemCarroDeslocamento.ItemCarroEventoChegada.Data) {
                vm.itemCarroDeslocamento.ItemCarroEventoChegada.Data = moment(vm.itemCarroDeslocamento.ItemCarroEventoChegada.Data).format("YYYY-MM-DDT");
                vm.itemCarroDeslocamento.ItemCarroEventoChegada.Data += (vm.itemCarroDeslocamento.ItemCarroEventoChegada.strHora) ? vm.itemCarroDeslocamento.ItemCarroEventoChegada.strHora : "00:00:00";

            }

            if (vm.itemCarroDeslocamento.ItemCarroEventoPartida.Data) {
                vm.itemCarroDeslocamento.ItemCarroEventoPartida.Data = moment(vm.itemCarroDeslocamento.ItemCarroEventoPartida.Data).format("YYYY-MM-DDT");
                vm.itemCarroDeslocamento.ItemCarroEventoPartida.Data += (vm.itemCarroDeslocamento.ItemCarroEventoPartida.strHora) ? vm.itemCarroDeslocamento.ItemCarroEventoPartida.strHora : "00:00:00";
            }

            angular.forEach(vm.ListaParticipante, function (item) {
                var itens =
                     $.grep(vm.itemCarroDeslocamento.Usuarios, function (e) { return e.IdentificadorUsuario == item.Identificador  });
                if (item.Selecionado && itens.length == 0) {
                    var NovoItem = { IdentificadorUsuario: item.Identificador, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") }
                    vm.itemCarroDeslocamento.Usuarios.push(NovoItem);
                }
                else if (!item.Selecionado && itens.length > 0) {
                    var Posicao = vm.itemCarroDeslocamento.indexOf(itens[0]);
                    vm.itemCarroDeslocamento.Usuarios.splice(Posicao, 1);
                }

            });

            Carro.SalvarCarroDeslocamento(vm.itemCarroDeslocamento, function (data) {
                vm.loading = false;
                if (data.Sucesso) {
                    vm.EscopoAtualizacao.AtualizarDeslocamentoSalvo(vm.itemOriginal, data.ItemRegistro);
                    vm.close();
                } else {
                    vm.messages = data.Mensagens;
                    vm.verificaCampoInvalido();
                }
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
            });
        };
    }
}());