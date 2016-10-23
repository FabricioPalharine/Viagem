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
                    if ($cookies.get('token')) {

                        config.headers.Authorization = 'Bearer ' + $cookies.get('token');
                        // console.log(config);
                    }
                    return config;
                },

                // Intercept 401s and redirect you to login
                responseError: function (response) {
                    if (response.status === 401) {
                        $rootScope.isLogged = false;
                        $location.path('/login');
                        $rootScope.$emit('logout');
                        // remove any stale tokens
                        $cookies.remove('token');
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
              Auth.isLoggedInAsync(function (loggedIn) {
                  // console.log('to aqui');
                  //   console.log(next);
                  // console.log(loggedIn);
                  if (next.authenticate == true && !loggedIn) {
                      //   console.log('manda pro login');

                      event.preventDefault();
                      $state.go('login');
                      // $location.path('/login');
                  }
                  //else {
                  //    console.log('parei');
                  //}
              });

          });
      }]);

    function config($urlRouterProvider, $httpProvider) {
        // console.log('manda pra home');
        $urlRouterProvider.otherwise('/login');
        $httpProvider.interceptors.push('authInterceptor');


      
    }
}());