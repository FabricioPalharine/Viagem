(function () {
  'use strict';

  angular
    .module('home')
      .config(['$stateProvider', config]);

  function config($stateProvider) {
      $stateProvider
            
          .state('alterasenha', {
              url: '/alterasenha',
              templateUrl: 'Home/AlterarSenha',
              controller: 'AlteraSenhaCtrl',
              controllerAs: 'alteraSenha',
              resolve: {
                  deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                      return $ocLazyLoad.load([{
                          name: 'AlteraSenhaCtrl',
                          files: [
                              'library/scripts/app/home/alteraSenha-controller.js'
                          ]
                      }]);
                  }]
              }
          })
        .state('home', {
            url: '/home',
            templateUrl: 'home/dashboard',
            controller: 'HomeCtrl',
            controllerAs: 'home',
            authenticate: false,
            params: {
                filtro: null
            },
            resolve: {
                deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load([
                    {
                        name: 'HomeCtrl',
                        files: [
                            'library/scripts/app/home/home-controller.js'
                        ]
                    }]);
                }]
            }


        });
      
  }
}());