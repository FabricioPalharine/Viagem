(function () {
    'use strict';

    /**
     * @ngdoc object
     * @name account.controller:AccountCtrl
     *
     * @description
     *
     */
    angular
      .module('home')
      .controller('AccountCtrl',['Auth', '$state', '$rootScope','SignalR', AccountCtrl]);

    function AccountCtrl(Auth, $state, $rootScope, SignalR) {
        var vm = this;
        vm.alertas = [];
        vm.totalAlertas = 0;
        // console.log(vm.permissoes);
        $rootScope.$on('loggin', function (event) {
            //console.log(' pegou evento');
            vm.user = Auth.currentUser;
            Auth.CarregarAlertas(function (data) {
                vm.alertas = [];
                for (var i=0;i<data.length;i++)
                {
                    vm.alertas.push(data[i]);
                }
                vm.totalAlertas = vm.alertas.length;
            });
        });

        vm.load = function () {
            vm.user = Auth.currentUser;
            SignalR.EnviarAlertaRequisicao(function (user) {
                vm.alertas.push(user);
                vm.totalAlertas++;
            });
            //console.log(vm.permissoes);
        };
        vm.logout = function () {
            Auth.logout();
           
        };

        vm.SelecionarViagem = function (IdentificadorViagem) {
            Auth.SelecionarViagem(IdentificadorViagem, function () {
                Auth.CarregarAlertas(function (data) {
                    vm.alertas = [];
                    for (var i = 0; i < data.length; i++) {
                        vm.alertas.push(data[i]);
                    }
                    vm.totalAlertas = vm.alertas.length;
                })
            });
            $state.go('home');
        };

        $rootScope.$on('ViagemSelecionada', function (event) {
            Auth.CarregarAlertas(function (data) {
                vm.alertas = [];
                for (var i = 0; i < data.length; i++) {
                    vm.alertas.push(data[i]);
                }
                vm.totalAlertas = vm.alertas.length;
            })
        });

        vm.AbrirAlerta = function (IdentificadorAlerta, TipoAlerta) {
            if (TipoAlerta == 1) {
                $state.go('Amigo', { AbrirAprovacao: true });

            }
            if (TipoAlerta == 2) {
                $state.go('VerificarSugestao');

            }
        };

        vm.ViagemSelecionada = function () {
            return Auth.currentUser.IdentificadorViagem != null;
        };

        vm.PermiteEdicao = function () {
            return Auth.currentUser.PermiteEdicao;
        };

        vm.VerCustos = function () {
            return Auth.currentUser.VerCustos;
        };

        vm.Aberto = function () {
            return Auth.currentUser.Aberto;
        };

        vm.TrocarSituacaoViagem = function()
        {
            Auth.TrocarSituacaoViagem();
        };
    }
}());