(function () {
    'use strict';


    angular
      .module('home')
      .controller('HomeCtrl', ['Error','$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$compile', 'uiCalendarConfig' , HomeCtrl]);

    function HomeCtrl(Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $compile, uiCalendarConfig) {
        var vm = this;
        vm.loading = false;      
        vm.Iniciado = false;
        
        vm.load = function () {
            
            if (Auth.currentUser && Auth.currentUser.IdentificadorUsuario)
                vm.CarregarInformacoes();
        };

        vm.CarregarInformacoes = function () {
          
        };

     

        $rootScope.$on('loggin', function (event) {
            vm.CarregarInformacoes();
        });
    }
}());