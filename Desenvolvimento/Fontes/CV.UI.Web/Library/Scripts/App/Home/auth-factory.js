(function () {
    'use strict';

    /**
     * @ngdoc service
     * @name account.factory:Auth
     *
     * @description
     *
     */
    angular
      .module('home')
      .factory('Auth',['$location', '$rootScope', '$http', '$cookies', '$q', '$translate', '$state', Auth]);

    function Auth($location, $rootScope, $http, $cookies, $q, $translate, $state) {
        var AuthBase = {};
        AuthBase.someValue = 'Auth';
        $rootScope.isLogged = false;
        AuthBase.currentUser = {};
        


        $rootScope.$on('logout', function (event) {
            AuthBase.currentUser = {};
        });

        AuthBase.AlterarSenha = function (user, callback) {
            var cb = callback || angular.noop;
            var deferred = $q.defer();

            $http.post('./api/Acesso/AlterarSenha', {                
                Login: user.login,
                Senha: user.senha,
                NovaSenha: user.novasenha,
                ConfirmarSenha: user.confirmarsenha
            }).
            success(function (data) {
                if (data.Sucesso == true) {
                 
                }

                deferred.resolve(data);
                return cb();
            }).
            error(function (err) {
                deferred.reject(err);
                return cb(err);
            });

            return deferred.promise;
        }

        AuthBase.login = function (user, callback) {
            // console.log('tenta logar');
            var cb = callback || angular.noop;
            var deferred = $q.defer();

            $http.post('./api/Acesso/Autenticar', {
                CodigoEmpresa: user.codigoEmpresa,
                Login: user.login,
                Senha: user.senha
            }).
            success(function (data) {
                //   console.log(data);
                if (data.Sucesso == true) {
                    $cookies.put('token', data.AuthenticationToken);
                    //  console.log($cookies.get('token'));
                    AuthBase.currentUser = data;
                   // $translate.use(data.Cultura.toLowerCase());                    
                    AuthBase.retornarAcessos(function (data) {
                        // console.log('chama o login');
                        // console.log(data);
                        AuthBase.currentUser.access = data;
                        $rootScope.$emit('loggin');
                        $state.go('home');
                    });
                    $rootScope.isLogged = true;

                }

                deferred.resolve(data);
                return cb();
            }).
            error(function (err) {
                this.logout();
                deferred.reject(err);
                return cb(err);
            }.bind(this));

            return deferred.promise;
        };

        AuthBase.CheckToken = function ( callback) {
            // console.log('tenta logar');
            var cb = callback || angular.noop;
            var deferred = $q.defer();

            $http.post('./api/Acesso/VerificarLogado', {

            }).
            success(function (data) {
                //   console.log(data);
                if (data.Sucesso == true) {
                    AuthBase.currentUser = data;
		     $translate.use(data.Cultura.toLowerCase());
                    AuthBase.retornarAcessos(function (data) {
                        // console.log('chama o login');
                        // console.log(data);
                        AuthBase.currentUser.access = data;
                        
                        $rootScope.$emit('loggin');

                    });
                }
                return cb(data.Sucesso);
            }).
            error(function (err) {               
                return cb(false);
            });

        };

        AuthBase.logout = function () {

            var deferred = $q.defer();
            $http.get('./api/Acesso/Logoff', {
            }).
           success(function (data) {               
               $cookies.remove('token');
               AuthBase.currentUser = {};
               $state.go('login');
              
           }).
              error(function (err) {
                
              });
        };

        AuthBase.isLoggedIn = function () {
            return AuthBase.currentUser.hasOwnProperty('Sucesso');
        };

        AuthBase.isLoggedInAsync = function (cb) {
            // console.log(AuthBase.currentUser);
            if (AuthBase.currentUser.hasOwnProperty('$promise')) {
                AuthBase.currentUser.$promise.then(function () {
                    $rootScope.isLogged = true;
                    cb(true);
                }).catch(function () {
                    $rootScope.isLogged = false;
                    cb(false);
                });
            } else if (AuthBase.currentUser.hasOwnProperty('Sucesso')) {
                $rootScope.isLogged = true;
                cb(true);
            } else {
                AuthBase.CheckToken(function(retorno)
                {
                    $rootScope.isLogged = retorno;
                    cb(retorno);
                }
                )              
            }
        };

        AuthBase.isAdmin = function () {
            return currentUser.perfil === 'admin';
        };

        AuthBase.getToken = function () {
            return $cookies.get('token');
        };

        AuthBase.retornarAcessos = function (cb) {
            $http({
                url: './api/Acesso/RetornarAcessos',
                method: "GET"
            }).
         success(function (data) {
             //  console.log(data);
             cb(data);
         }).
         error(function (err) {
             console.log('erro');
             cb(err);
         }.bind(this));

        };

        AuthBase.Paginas = {


           

        };


        return AuthBase;
    }
}());