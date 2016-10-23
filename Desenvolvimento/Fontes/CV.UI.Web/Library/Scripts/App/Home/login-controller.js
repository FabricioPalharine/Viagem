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

        vm.login = function (form) {
            this.submitted = true;

            if (form.$valid) {
                vm.loading = true;
                Auth.login({
                    login: vm.user.login,
                    senha: vm.user.senha,
                    codigoEmpresa: vm.user.codigoEmpresa
                }).then(function (data) {
                    vm.loading = false;
                    // console.log(data);
                    vm.controle.sucesso = data.Sucesso;
                    vm.controle.mensagens = data.Mensagens;
                    // console.log(data);
                    if ($rootScope.isLogged) {
                        //    console.log('vai pra home');
                        $state.go('home');
                    }

                }).catch(function (err) {
                    vm.loading = false;
                    vm.controle.sucesso = false;
                    vm.controle.mensagens.push({ Mensagem: 'Ocorreu um erro ao efetuar login. Favor procurar o administrador do sistema.' });
                    //     console.log(err);
                });
            }
        };
    }
}());