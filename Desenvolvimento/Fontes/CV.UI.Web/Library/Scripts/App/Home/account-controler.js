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
            Auth.SelecionarViagem(IdentificadorViagem);
            Auth.CarregarAlertas(function (data) {
                vm.alertas = [];
                for (var i = 0; i < data.length; i++) {
                    vm.alertas.push(data[i]);
                }
                vm.totalAlertas = vm.alertas.length;
            });
            $state.go('home');
        };

        vm.AbrirAlerta = function(IdentificadorAlerta, TipoAlerta)
        {
            if (TipoAlerta == 1)
            {
                $state.go('Amigo', { AbrirAprovacao: true });

            }
            if (TipoAlerta == 2) {
                $state.go('VerificarSugestao');

            }
        }
        
    }
}());