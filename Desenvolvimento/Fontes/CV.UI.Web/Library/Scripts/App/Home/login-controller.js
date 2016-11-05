(function () {
    'use strict';

    /**
     * @ngdoc object
     * @name account.controller:LoginCtrl
     *
     * @description
     *
     */
    angular
      .module('home')
      .controller('LoginCtrl',['Auth', '$state', '$rootScope', '$scope', LoginCtrl]);

    function LoginCtrl(Auth, $state, $rootScope, $scope) {
        var vm = this;
        vm.controle = { sucesso: true, mensagens: [] };
        vm.loading = false;
        vm.user = { };

        vm.init = function () {
            Auth.CheckLoginValido(function (Valido) {
                if (Valido)
                {
                    $rootScope.$emit('loggin');
                    $rootScope.isLogged = true;
                }
            });
        };


        vm.login = function (form) {
            this.submitted = true;

            if (form.$valid) {
                vm.loading = true;
                Auth.GetOfflineAcess(function (data) {
                    vm.loading = false;
                    // console.log(data);
                    vm.controle.sucesso = data.Sucesso;
                    vm.controle.mensagens = data.Mensagens;
                    //// console.log(data);
                    if ($rootScope.isLogged) {
                    //    //    console.log('vai pra home');
                        $state.go('home');
                    }

                });
            }
        };
    }
}());