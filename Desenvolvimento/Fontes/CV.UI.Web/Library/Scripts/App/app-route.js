(function () {
    'use strict';

    angular
      .module('CV')
      .config(['$urlRouterProvider', '$httpProvider',config])
        .factory('authInterceptor', [ '$rootScope', '$q', '$cookies', '$location', function ($rootScope, $q, $cookies, $location) {
            return {
                // Add authorization token to headers
                request: function (config) {
                    config.headers = config.headers || {};
                    if (sessionStorage.getItem('token')) {

                        config.headers.Authorization = 'Bearer ' + sessionStorage.getItem('token');
                        // console.log(config);
                    }
                    return config;
                },

                // Intercept 401s and redirect you to login
                responseError: function (response) {
                    if (response.status === 401) {
                        $rootScope.isLogged = false;
                        $location.path('/home');
                        $rootScope.$emit('logout');
                        // remove any stale tokens
                        sessionStorage.removeItem('token');
                        return $q.reject(response);
                    }
                    else {
                        return $q.reject(response);
                    }
                }
            };
        }])
      .run(['$rootScope', '$location', '$state', 'Auth', function ($rootScope, $location, $state, Auth) {
          // Redirect to login if route requires auth and you're not logged in
          $rootScope.$on('$stateChangeStart', function (event, next) {
              if (next.authenticate == true ) {
                  Auth.isLoggedInAsync(function (loggedIn) {
                      // console.log('to aqui');
                      //   console.log(next);
                      // console.log(loggedIn);

                      //   console.log('manda pro login');
                      if (!loggedIn) {
                          event.preventDefault();
                          $state.go('home');
                      }
                      // $location.path('/login');

                      //else {
                      //    console.log('parei');
                      //}
                  })
              };

          });
      }]);

    function config($urlRouterProvider, $httpProvider) {
        // console.log('manda pra home');
        $urlRouterProvider.otherwise('/home');
        $httpProvider.interceptors.push('authInterceptor');


      
    }
}());