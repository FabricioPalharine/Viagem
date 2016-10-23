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
      .controller('AccountCtrl',['Auth', '$state', '$rootScope', AccountCtrl]);

    function AccountCtrl(Auth, $state, $rootScope) {
        var vm = this;
        // console.log(vm.permissoes);


        $rootScope.$on('loggin', function (event) {
            //console.log(' pegou evento');
            vm.user = Auth.currentUser;
            vm.permissoes = {
               
            };
            vm.verificarPermissoes();
        });

        vm.load = function () {
            vm.user = Auth.currentUser;

            //console.log(vm.permissoes);
        };
        vm.logout = function () {
            Auth.logout();
           
        };

        vm.alterarSenha = function () {
            $state.go('alterasenha');
        };

        vm.verificarPermissoes = function () {
          
            $(Auth.currentUser.access).each(function (i, item) {
                //  console.log('foreach');
                //  console.log(item);




            });
            //angular.forEach(Auth.currentUser.access, function (value, key) {

            //});
        };
    }
}());