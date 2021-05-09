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
      .factory('Auth',['$location', '$rootScope', '$http', '$cookies', '$q', '$translate', '$state','SignalR', Auth]);

    function Auth($location, $rootScope, $http, $cookies, $q, $translate, $state,SignalR) {
        var AuthBase = {};
        AuthBase.someValue = 'Auth';
        $rootScope.isLogged = false;
        AuthBase.currentUser = {};
        
        AuthBase.apiKey = '';
        AuthBase.clientId = '';
       

        AuthBase.scopes = 'profile https://www.googleapis.com/auth/youtube.upload https://www.googleapis.com/auth/youtube https://www.googleapis.com/auth/photoslibrary.sharing https://www.googleapis.com/auth/photoslibrary';
        AuthBase.auth2 = null;
       

        $rootScope.$on('logout', function (event) {
            AuthBase.currentUser = {};
        });

       

        AuthBase.logout = function () {
            SignalR.DesconectarUsuario(AuthBase.currentUser.Codigo);
            sessionStorage.removeItem('token');
            $cookies.remove('IdentificadorViagem');
            AuthBase.currentUser = {};           
            $rootScope.isLogged = false;
            AuthBase.auth2.signOut();

            $state.go('home');
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
                AuthBase.CheckLoginValido(function (retorno)
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
            return sessionStorage.getItem('token');
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

        AuthBase.init = function () {
            $http.get('./api/Acesso/RetornarChave').success(function (data) {
                AuthBase.apiKey = data.Descricao;
                AuthBase.clientId = data.Codigo;
                SignalR.startHub();
                gapi.load('client:auth2', AuthBase.initAuth);
            });
        };

        AuthBase.initAuth = function () {
          
            
            gapi.client.setApiKey(AuthBase.apiKey);
            gapi.auth2.init({
                client_id: AuthBase.clientId,
                scope: AuthBase.scopes
            }).then(function () {
                AuthBase.auth2 = gapi.auth2.getAuthInstance();

                // Listen for sign-in state changes.
                //AuthBase.auth2.isSignedIn.listen(AuthBase.updateSigninStatus);

                
                // Handle the initial sign-in state.
                //AuthBase.updateSigninStatus(AuthBase.auth2.isSignedIn.get());


            });
            
        };

        AuthBase.CheckLoginValido = function (callback) {
            $http.get('./api/Acesso/RetornarChave').success(function (data) {
                AuthBase.apiKey = data.Descricao;
                AuthBase.clientId = data.Codigo;
                if (AuthBase.auth2 == null) {
                    gapi.load('client:auth2', function () {
                        gapi.client.setApiKey(AuthBase.apiKey);
                        gapi.auth2.init({
                            client_id: AuthBase.clientId,
                            scope: AuthBase.scopes
                        }).then(function () {
                            AuthBase.auth2 = gapi.auth2.getAuthInstance();

                            // Listen for sign-in state changes.
                            //AuthBase.auth2.isSignedIn.listen(AuthBase.updateSigninStatus);
                            AuthBase.VerificarLoginValido(callback);

                        })
                    });
                }
                else {
                    AuthBase.VerificarLoginValido(callback);
                }
            });
        };

        AuthBase.VerificarLoginValido = function (callback) {
            if (AuthBase.auth2.isSignedIn.get()) {
                var User = AuthBase.auth2.currentUser.get();
                var DadosRetorno = User.getAuthResponse();

                $http.post('./api/Acesso/ValidarUsuario', {
                    Codigo: User.getId(),
                    IdentificadorViagem: $cookies.get('IdentificadorViagem')
                }).
               success(function (data) {
                   //   console.log(data);
                   if (data.Sucesso == true) {
                       sessionStorage.setItem('token', data.AuthenticationToken);
                       AuthBase.currentUser =
                        {
                            Codigo: data.Codigo,
                            Nome: User.getBasicProfile().getName(),
                            ImageURL: User.getBasicProfile().getImageUrl(),
                            Mail: User.getBasicProfile().getEmail(),
                            Sucesso: true,
                            PrimeiroNome: User.getBasicProfile().getGivenName(),
                            Viagens: data.Viagens,
                            IdentificadorViagem: data.IdentificadorViagem,
                            NomeViagem: data.NomeViagem,
                            PermiteEdicao: data.PermiteEdicao,
                            VerCustos: data.VerCustos,
                            Aberto: data.Aberto
                        };
                       SignalR.ConectarUsuario(data.Codigo);

                       if (data.IdentificadorViagem)
                       {
                           var expireDate = new Date();
                           expireDate.setDate(expireDate.getDate() + 30);
                           // Setting a cookie
                           $cookies.put('IdentificadorViagem', data.IdentificadorViagem, { 'expires': expireDate });
                           SignalR.ConectarViagem(data.IdentificadorViagem, data.PermiteEdicao);

                       }
                       $translate.use(data.Cultura.toLowerCase());

                       callback(true);

                   }
               });

            }
        };
               

        AuthBase.updateSigninStatus = function (isSignedIn) {
            if (!isSignedIn) {
            }
        };

        AuthBase.GetOfflineAcess = function (callback) {
            AuthBase.auth2.signIn({ 'scope': AuthBase.scopes, 'prompt': 'consent' }).then(function (resp2) {
            AuthBase.auth2.grantOfflineAccess({ 'redirect_uri': 'postmessage', 'scope': AuthBase.scopes, 'prompt': 'consent' }).then(function (resp) {
                var auth_code = resp.code;
                

                    var User = AuthBase.auth2.currentUser.get();
                    var DadosRetorno = User.getAuthResponse();
                    var Nome = User.getBasicProfile().getName();
                    var Mail = User.getBasicProfile().getEmail();
                    var ImageURL = User.getBasicProfile().getImageUrl();
                    var PrimeiroNome = User.getBasicProfile().getGivenName();
                    $http.post('./api/Acesso/LoginGoogle', {
                        CodigoValidacao: auth_code,
                        Nome: User.getBasicProfile().getName(),
                        EMail: User.getBasicProfile().getEmail(),
                        Codigo: User.getId(),
                        IdentificadorViagem: $cookies.get('IdentificadorViagem')
                    }).
                       success(function (data) {
                           //   console.log(data);
                           if (data.Sucesso == true) {
                               sessionStorage.setItem('token', data.AuthenticationToken);
                               AuthBase.currentUser =
                                {
                                    Codigo: data.Codigo,
                                    Nome: Nome,
                                    ImageURL: ImageURL,
                                    Mail: Mail,
                                    Sucesso: true,
                                    PrimeiroNome: PrimeiroNome,
                                    Viagens: data.Viagens,
                                    IdentificadorViagem: data.IdentificadorViagem,
                                    NomeViagem: data.NomeViagem,
                                    PermiteEdicao: data.PermiteEdicao,
                                    VerCustos: data.VerCustos,
                                    Aberto: data.Aberto
                                };
                               SignalR.ConectarUsuario(data.Codigo);

                               if (data.IdentificadorViagem) {
                                   SignalR.ConectarViagem(data.IdentificadorViagem, data.PermiteEdicao);

                                   var expireDate = new Date();
                                   expireDate.setDate(expireDate.getDate() + 30);
                                   // Setting a cookie
                                   $cookies.put('IdentificadorViagem', data.IdentificadorViagem, { 'expires': expireDate });
                               }
                               $translate.use(data.Cultura.toLowerCase());


                               $rootScope.$emit('loggin');
                               $state.go('home');

                               $rootScope.isLogged = true;

                           }
                           callback(data);
                       });
                }).catch(function (err) {
                });
                    ;
                
            }).catch(function (err) {
            });
        }

        AuthBase.SelecionarViagem = function (IdentificadorViagem, callback) {
            $http.post('./api/Acesso/SelecionarViagem', {
                IdentificadorViagem: IdentificadorViagem
            }).
              success(function (data) {
                  sessionStorage.setItem('token', data.AuthenticationToken);
                 
                  AuthBase.currentUser.IdentificadorViagem = data.IdentificadorViagem;
                  AuthBase.currentUser.NomeViagem = data.NomeViagem;
                  AuthBase.currentUser.PermiteEdicao = data.PermiteEdicao;
                  AuthBase.currentUser.VerCustos = data.VerCustos;
                  AuthBase.currentUser.Aberto = data.Aberto;
                  SignalR.ConectarViagem(data.IdentificadorViagem, data.PermiteEdicao);
                  var expireDate = new Date();
                  expireDate.setDate(expireDate.getDate() + 30);
                  // Setting a cookie
                  $cookies.put('IdentificadorViagem', data.IdentificadorViagem, { 'expires': expireDate });
                  if (callback)
                      callback();
              });
        };

        AuthBase.CarregarAlertas = function (callback) {
            $http.get('./api/Acesso/CarregarAlertas').
              success(function (data) {
                  callback(data);
              });
        };

        AuthBase.TrocarSituacaoViagem = function () {
            $http.get('./api/Viagem/TrocarSituacaoViagem').success(function (data) {
                AuthBase.currentUser.Aberto = !AuthBase.currentUser.Aberto;
            });
        };

        $rootScope.$on('SignalRConnected', function (event) {
            if (AuthBase.currentUser.Codigo)
            {
                SignalR.ConectarUsuario(AuthBase.currentUser.Codigo);
                if (AuthBase.currentUser.IdentificadorViagem)
                    SignalR.ConectarViagem(AuthBase.currentUser.IdentificadorViagem, AuthBase.currentUser.PermiteEdicao);

            }

        });

        AuthBase.init();
        return AuthBase;
    }
}());