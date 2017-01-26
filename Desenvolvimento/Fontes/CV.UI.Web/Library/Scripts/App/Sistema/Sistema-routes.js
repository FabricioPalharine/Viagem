(function () {
    'use strict';

    angular
      .module('Sistema')
      .config(['$stateProvider', config]);

    function config($stateProvider) {
        $stateProvider
             .state('Usuario', {
                 url: '/Usuario',
                 templateUrl: 'Sistema/Usuario',
                 controller: 'UsuarioCtrl',
                 controllerAs: 'itemUsuario',
                 authenticate: true,
                 params: {
                     filtro: null
                 },
                 resolve: {
                     deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                         return $ocLazyLoad.load([
                              {
                                  name: 'UsuarioFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                  ]
                              },
                             {
                                 name: 'UsuarioController',
                                 files: [
                                     'library/scripts/app/Sistema/Usuario/Usuario-edit-controller.js',
                                     'library/scripts/app/Sistema/Usuario/Usuario-controller.js'
                                 ]
                             }, ]);

                     }]
                 }
             })
            .state('UsuarioEdicao', {
                url: '/UsuarioEdicao/:id',
                templateUrl: 'Sistema/UsuarioEdicao',
                controller: 'UsuarioEditCtrl',
                controllerAs: 'itemUsuarioEdit',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'UsuarioFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                 ]
                             },
                            {
                                name: 'UsuarioController',
                                files: [
                                    'library/scripts/app/Sistema/Usuario/Usuario-edit-controller.js',
                                    'library/scripts/app/Sistema/Usuario/Usuario-controller.js'
                                ]
                            }, ]);

                    }]
                }
            })
            .state('Amigo', {
                url: '/Amigo',
                templateUrl: 'Sistema/Amigo',
                controller: 'AmigoCtrl',
                controllerAs: 'itemAmigo',
                authenticate: true,
                params: {
                    filtro: null,
                    AbrirAprovacao: false
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'AmigoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Amigo/Amigo-factory.js'
                                 ]
                             },
                            {
                                name: 'AmigoController',
                                files: [
                                    'library/scripts/app/Sistema/Amigo/Amigo-edit-controller.js',
                                    'library/scripts/app/Sistema/Amigo/Amigo-controller.js'
                                ]
                            }, {
                                name: 'UsuarioFactory',
                                files: [
                                    'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                ]
                            },
                              {
                                  name: 'RequisicaoAmizadeFactory',
                                  files: [
                                      'library/scripts/app/Sistema/RequisicaoAmizade/RequisicaoAmizade-factory.js'
                                  ]
                              },
                        ]);

                    }]
                }
            })
            .state('AmigoEdicao', {
                url: '/AmigoEdicao/:id',
                templateUrl: 'Sistema/AmigoEdicao',
                controller: 'AmigoEditCtrl',
                controllerAs: 'itemAmigoEdit',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'AmigoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Amigo/Amigo-factory.js'
                                 ]
                             },
                            {
                                name: 'AmigoController',
                                files: [
                                    'library/scripts/app/Sistema/Amigo/Amigo-edit-controller.js',
                                    'library/scripts/app/Sistema/Amigo/Amigo-controller.js'
                                ]
                            }, {
                                name: 'UsuarioFactory',
                                files: [
                                    'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                ]
                            },
                              {
                                  name: 'UsuarioFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                  ]
                              },
                        ]);

                    }]
                }
            })
            .state('AporteDinheiro', {
                url: '/AporteDinheiro',
                templateUrl: 'Sistema/AporteDinheiro',
                controller: 'AporteDinheiroCtrl',
                controllerAs: 'itemAporteDinheiro',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'AporteDinheiroFactory',
                                 files: [
                                     'library/scripts/app/Sistema/AporteDinheiro/AporteDinheiro-factory.js'
                                 ]
                             },
                            {
                                name: 'AporteDinheiroController',
                                files: [
                                    'library/scripts/app/Sistema/AporteDinheiro/AporteDinheiro-edit-controller.js',
                                    'library/scripts/app/Sistema/AporteDinheiro/AporteDinheiro-controller.js'
                                ]
                            }, {
                                name: 'UsuarioFactory',
                                files: [
                                    'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                ]
                            },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                        ]);

                    }]
                }
            })
            .state('Atracao', {
                url: '/Atracao',
                templateUrl: 'Sistema/Atracao',
                controller: 'AtracaoCtrl',
                controllerAs: 'itemAtracao',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'AtracaoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Atracao/Atracao-factory.js'
                                 ]
                             },
                             {
                                 name: 'GastoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Gasto/Gasto-factory.js'
                                 ]
                             },
                             {
                                 name: 'GastoController',
                                 files: [
                                     'library/scripts/app/Sistema/Gasto/Gasto-edit-controller.js',
                                     'library/scripts/app/Sistema/Gasto/Gasto-controller.js'
                                 ]
                             },
                            {
                                name: 'AtracaoController',
                                files: [
                                    'library/scripts/app/Sistema/Atracao/Atracao-edit-controller.js',
                                    'library/scripts/app/Sistema/Atracao/Atracao-controller.js'
                                ]
                            }, {
                                name: 'CidadeFactory',
                                files: [
                                    'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                                ]
                            },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                               {
                                   name: 'FotoFactory',
                                   files: [
                                       'library/scripts/app/Sistema/Foto/Foto-factory.js'
                                   ]
                               },
                        ]);

                    }]
                }
            })
            .state('CalendarioPrevisto', {
                url: '/CalendarioPrevisto',
                templateUrl: 'Sistema/CalendarioPrevisto',
                controller: 'CalendarioPrevistoCtrl',
                controllerAs: 'itemCalendarioPrevisto',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'CalendarioPrevistoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/CalendarioPrevisto/CalendarioPrevisto-factory.js'
                                 ]
                             },
                            {
                                name: 'CalendarioPrevistoController',
                                files: [
                                    'library/scripts/app/Sistema/CalendarioPrevisto/CalendarioPrevisto-edit-controller.js',
                                    'library/scripts/app/Sistema/CalendarioPrevisto/CalendarioPrevisto-controller.js'
                                ]
                            }, {
                                name: 'ViagemFactory',
                                files: [
                                    'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                ]
                            },
                        ]);

                    }]
                }
            })
            .state('Carro', {
                url: '/Carro',
                templateUrl: 'Sistema/Carro',
                controller: 'CarroCtrl',
                controllerAs: 'itemCarro',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'CarroFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Carro/Carro-factory.js'
                                 ]
                             },
                            {
                                name: 'CarroController',
                                files: [
                                    'library/scripts/app/Sistema/Carro/Carro-edit-controller.js',
                                    'library/scripts/app/Sistema/Carro/Carro-controller.js'
                                ]
                            },
                             {
                                 name: 'CarroDeslocamentoController',
                                 files: [
                                     'library/scripts/app/Sistema/CarroDeslocamento/CarroDeslocamento-edit-controller.js',
                                 ]
                             },
                            {
                                name: 'ViagemFactory',
                                files: [
                                    'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                ]
                            },
                             {
                                 name: 'ReabastecimentoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Reabastecimento/Reabastecimento-factory.js'
                                 ]
                             },
                            {
                                name: 'ReabastecimentoController',
                                files: [
                                    'library/scripts/app/Sistema/Reabastecimento/Reabastecimento-edit-controller.js',
                                    'library/scripts/app/Sistema/Reabastecimento/Reabastecimento-controller.js'
                                ]
                            },
                            {
                                name: 'GastoFactory',
                                files: [
                                    'library/scripts/app/Sistema/Gasto/Gasto-factory.js'
                                ]
                            },
                             {
                                 name: 'GastoController',
                                 files: [
                                     'library/scripts/app/Sistema/Gasto/Gasto-edit-controller.js',
                                     'library/scripts/app/Sistema/Gasto/Gasto-controller.js'
                                 ]
                             },
                        ]);

                    }]
                }
            })
            .state('CidadeGrupo', {
                url: '/CidadeGrupo',
                templateUrl: 'Sistema/CidadeGrupo',
                controller: 'CidadeGrupoCtrl',
                controllerAs: 'itemCidadeGrupo',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'CidadeGrupoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/CidadeGrupo/CidadeGrupo-factory.js'
                                 ]
                             },
                            {
                                name: 'CidadeGrupoController',
                                files: [
                                    'library/scripts/app/Sistema/CidadeGrupo/CidadeGrupo-edit-controller.js',
                                    'library/scripts/app/Sistema/CidadeGrupo/CidadeGrupo-controller.js'
                                ]
                            }, {
                                name: 'CidadeFactory',
                                files: [
                                    'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                                ]
                            },
                              {
                                  name: 'CidadeFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                                  ]
                              },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                        ]);

                    }]
                }
            })
            .state('Comentario', {
                url: '/Comentario',
                templateUrl: 'Sistema/Comentario',
                controller: 'ComentarioCtrl',
                controllerAs: 'itemComentario',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'ComentarioFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Comentario/Comentario-factory.js'
                                 ]
                             },
                            {
                                name: 'ComentarioController',
                                files: [
                                    'library/scripts/app/Sistema/Comentario/Comentario-edit-controller.js',
                                    'library/scripts/app/Sistema/Comentario/Comentario-controller.js'
                                ]
                            }, {
                                name: 'CidadeFactory',
                                files: [
                                    'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                                ]
                            },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                        ]);

                    }]
                }
            })
            .state('CotacaoMoeda', {
                url: '/CotacaoMoeda',
                templateUrl: 'Sistema/CotacaoMoeda',
                controller: 'CotacaoMoedaCtrl',
                controllerAs: 'itemCotacaoMoeda',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'CotacaoMoedaFactory',
                                 files: [
                                     'library/scripts/app/Sistema/CotacaoMoeda/CotacaoMoeda-factory.js'
                                 ]
                             },
                            {
                                name: 'CotacaoMoedaController',
                                files: [
                                    'library/scripts/app/Sistema/CotacaoMoeda/CotacaoMoeda-edit-controller.js',
                                    'library/scripts/app/Sistema/CotacaoMoeda/CotacaoMoeda-controller.js'
                                ]
                            }, {
                                name: 'ViagemFactory',
                                files: [
                                    'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                ]
                            },
                        ]);

                    }]
                }
            })
            .state('Foto', {
                url: '/Foto',
                templateUrl: 'Sistema/Foto',
                controller: 'FotoCtrl',
                controllerAs: 'itemFoto',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'FotoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Foto/Foto-factory.js'
                                 ]
                             },
                            {
                                name: 'FotoController',
                                files: [
                                    'library/scripts/app/Sistema/Foto/Foto-edit-controller.js',
                                    'library/scripts/app/Sistema/Foto/Foto-controller.js'
                                ]
                            }, {
                                name: 'CidadeFactory',
                                files: [
                                    'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                                ]
                            },
                              {
                                  name: 'UsuarioFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                  ]
                              },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                               {
                                   name: 'LojaFactory',
                                   files: [
                                       'library/scripts/app/Sistema/Loja/Loja-factory.js'
                                   ]
                               },
                            {
                                name: 'RefeicaoFactory',
                                files: [
                                    'library/scripts/app/Sistema/Refeicao/Refeicao-factory.js'
                                ]
                            },
                              {
                                  name: 'HotelFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Hotel/Hotel-factory.js'
                                  ]
                              },
                              {
                                  name: 'AtracaoFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Atracao/Atracao-factory.js'
                                  ]
                              },
                              {
                                  name: 'CidadeFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                                  ]
                              },
                        ]);

                    }]
                }
            })
            .state('Gasto', {
                url: '/Gasto',
                templateUrl: 'Sistema/Gasto',
                controller: 'GastoCtrl',
                controllerAs: 'itemGasto',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'GastoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Gasto/Gasto-factory.js'
                                 ]
                             },
                            {
                                name: 'GastoController',
                                files: [
                                    'library/scripts/app/Sistema/Gasto/Gasto-edit-controller.js',
                                    'library/scripts/app/Sistema/Gasto/Gasto-controller.js'
                                ]
                            }, {
                                name: 'UsuarioFactory',
                                files: [
                                    'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                ]
                            },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                        ]);

                    }]
                }
            })
            .state('Hotel', {
                url: '/Hotel',
                templateUrl: 'Sistema/Hotel',
                controller: 'HotelCtrl',
                controllerAs: 'itemHotel',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'HotelFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Hotel/Hotel-factory.js'
                                 ]
                             },
                            {
                                name: 'HotelController',
                                files: [
                                    'library/scripts/app/Sistema/Hotel/Hotel-edit-controller.js',
                                    'library/scripts/app/Sistema/Hotel/Hotel-controller.js'
                                ]
                            }, {
                                name: 'GastoFactory',
                                files: [
                                    'library/scripts/app/Sistema/Gasto/Gasto-factory.js'
                                ]
                            },
                             {
                                 name: 'GastoController',
                                 files: [
                                     'library/scripts/app/Sistema/Gasto/Gasto-edit-controller.js',
                                     'library/scripts/app/Sistema/Gasto/Gasto-controller.js'
                                 ]
                             },
                           {
                               name: 'CidadeFactory',
                               files: [
                                   'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                               ]
                           },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                               {
                                   name: 'FotoFactory',
                                   files: [
                                       'library/scripts/app/Sistema/Foto/Foto-factory.js'
                                   ]
                               },
                        ]);

                    }]
                }
            })
            .state('ListaCompra', {
                url: '/ListaCompra',
                templateUrl: 'Sistema/ListaCompra',
                controller: 'ListaCompraCtrl',
                controllerAs: 'itemListaCompra',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'ListaCompraFactory',
                                 files: [
                                     'library/scripts/app/Sistema/ListaCompra/ListaCompra-factory.js'
                                 ]
                             },
                            {
                                name: 'ListaCompraController',
                                files: [
                                    'library/scripts/app/Sistema/ListaCompra/ListaCompra-edit-controller.js',
                                    'library/scripts/app/Sistema/ListaCompra/ListaCompra-controller.js'
                                ]
                            }, {
                                name: 'UsuarioFactory',
                                files: [
                                    'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                ]
                            },
                              {
                                  name: 'UsuarioFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                  ]
                              },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                        ]);

                    }]
                }
            })
            .state('PedidoCompra', {
                url: '/PedidoCompra',
                templateUrl: 'Sistema/PedidoCompra',
                controller: 'PedidoCompraCtrl',
                controllerAs: 'itemListaCompra',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'ListaCompraFactory',
                                 files: [
                                     'library/scripts/app/Sistema/ListaCompra/ListaCompra-factory.js'
                                 ]
                             },
                            {
                                name: 'PedidoCompraController',
                                files: [
                                    'library/scripts/app/Sistema/PedidoCompra/PedidoCompra-edit-controller.js',
                                    'library/scripts/app/Sistema/PedidoCompra/PedidoCompra-controller.js'
                                ]
                            }, {
                                name: 'UsuarioFactory',
                                files: [
                                    'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                ]
                            },
                              {
                                  name: 'UsuarioFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                  ]
                              },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                        ]);

                    }]
                }
            })
            .state('Posicao', {
                url: '/Posicao',
                templateUrl: 'Sistema/Posicao',
                controller: 'PosicaoCtrl',
                controllerAs: 'itemPosicao',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'PosicaoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Posicao/Posicao-factory.js'
                                 ]
                             },
                            {
                                name: 'PosicaoController',
                                files: [
                                    'library/scripts/app/Sistema/Posicao/Posicao-edit-controller.js',
                                    'library/scripts/app/Sistema/Posicao/Posicao-controller.js'
                                ]
                            }, {
                                name: 'UsuarioFactory',
                                files: [
                                    'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                ]
                            },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                        ]);

                    }]
                }
            })
            .state('PosicaoEdicao', {
                url: '/PosicaoEdicao/:id',
                templateUrl: 'Sistema/PosicaoEdicao',
                controller: 'PosicaoEditCtrl',
                controllerAs: 'itemPosicaoEdit',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'PosicaoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Posicao/Posicao-factory.js'
                                 ]
                             },
                            {
                                name: 'PosicaoController',
                                files: [
                                    'library/scripts/app/Sistema/Posicao/Posicao-edit-controller.js',
                                    'library/scripts/app/Sistema/Posicao/Posicao-controller.js'
                                ]
                            }, {
                                name: 'UsuarioFactory',
                                files: [
                                    'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                ]
                            },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                        ]);

                    }]
                }
            })
            .state('Loja', {
                url: '/Loja',
                templateUrl: 'Sistema/Loja',
                controller: 'LojaCtrl',
                controllerAs: 'itemLoja',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'LojaFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Loja/Loja-factory.js'
                                 ]
                             },
                            {
                                name: 'LojaController',
                                files: [
                                    'library/scripts/app/Sistema/Loja/Loja-edit-controller.js',
                                    'library/scripts/app/Sistema/Loja/Loja-controller.js',
                                    'library/scripts/app/Sistema/Loja/Compra-edit-controller.js',
                                    'library/scripts/app/Sistema/Loja/ItemCompra-edit-controller.js'
                                ]
                            },
                             {
                                 name: 'ListaCompraFactory',
                                 files: [
                                     'library/scripts/app/Sistema/ListaCompra/ListaCompra-factory.js'
                                 ]
                             },
                            {
                                name: 'AtracaoFactory',
                                files: [
                                    'library/scripts/app/Sistema/Atracao/Atracao-factory.js'
                                ]
                            },
                             {
                                 name: 'GastoFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Gasto/Gasto-factory.js'
                                 ]
                             },
                             {
                                 name: 'GastoController',
                                 files: [
                                     'library/scripts/app/Sistema/Gasto/Gasto-edit-controller.js',
                                     'library/scripts/app/Sistema/Gasto/Gasto-controller.js'
                                 ]
                             },
                           {
                               name: 'CidadeFactory',
                               files: [
                                   'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                               ]
                           },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },
                               {
                                   name: 'FotoFactory',
                                   files: [
                                       'library/scripts/app/Sistema/Foto/Foto-factory.js'
                                   ]
                               },
                               {
                                   name: 'UsuarioFactory',
                                   files: [
                                       'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                   ]
                               },
                        ]);

                    }]
                }
            })
           .state('Refeicao', {
               url: '/Refeicao',
               templateUrl: 'Sistema/Refeicao',
               controller: 'RefeicaoCtrl',
               controllerAs: 'itemRefeicao',
               authenticate: true,
               params: {
                   filtro: null
               },
               resolve: {
                   deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([
                            {
                                name: 'RefeicaoFactory',
                                files: [
                                    'library/scripts/app/Sistema/Refeicao/Refeicao-factory.js'
                                ]
                            },
                           {
                               name: 'RefeicaoController',
                               files: [
                                   'library/scripts/app/Sistema/Refeicao/Refeicao-edit-controller.js',
                                   'library/scripts/app/Sistema/Refeicao/Refeicao-controller.js'
                               ]
                           },
                           {
                               name: 'AtracaoFactory',
                               files: [
                                   'library/scripts/app/Sistema/Atracao/Atracao-factory.js'
                               ]
                           },
                            {
                                name: 'GastoFactory',
                                files: [
                                    'library/scripts/app/Sistema/Gasto/Gasto-factory.js'
                                ]
                            },
                            {
                                name: 'GastoController',
                                files: [
                                    'library/scripts/app/Sistema/Gasto/Gasto-edit-controller.js',
                                    'library/scripts/app/Sistema/Gasto/Gasto-controller.js'
                                ]
                            },
                          {
                              name: 'CidadeFactory',
                              files: [
                                  'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                              ]
                          },
                             {
                                 name: 'ViagemFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                 ]
                             },
                              {
                                  name: 'FotoFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Foto/Foto-factory.js'
                                  ]
                              },
                       ]);

                   }]
               }
           })
           .state('RequisicaoAmizade', {
               url: '/RequisicaoAmizade',
               templateUrl: 'Sistema/RequisicaoAmizade',
               controller: 'RequisicaoAmizadeCtrl',
               controllerAs: 'itemRequisicaoAmizade',
               authenticate: true,
               params: {
                   filtro: null
               },
               resolve: {
                   deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([
                            {
                                name: 'RequisicaoAmizadeFactory',
                                files: [
                                    'library/scripts/app/Sistema/RequisicaoAmizade/RequisicaoAmizade-factory.js'
                                ]
                            },
                           {
                               name: 'RequisicaoAmizadeController',
                               files: [
                                   'library/scripts/app/Sistema/RequisicaoAmizade/RequisicaoAmizade-edit-controller.js',
                                   'library/scripts/app/Sistema/RequisicaoAmizade/RequisicaoAmizade-controller.js'
                               ]
                           }, {
                               name: 'UsuarioFactory',
                               files: [
                                   'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                               ]
                           },
                             {
                                 name: 'UsuarioFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                 ]
                             },
                       ]);

                   }]
               }
           })
           .state('RequisicaoAmizadeEdicao', {
               url: '/RequisicaoAmizadeEdicao/:id',
               templateUrl: 'Sistema/RequisicaoAmizadeEdicao',
               controller: 'RequisicaoAmizadeEditCtrl',
               controllerAs: 'itemRequisicaoAmizadeEdit',
               authenticate: true,
               params: {
                   filtro: null
               },
               resolve: {
                   deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([
                            {
                                name: 'RequisicaoAmizadeFactory',
                                files: [
                                    'library/scripts/app/Sistema/RequisicaoAmizade/RequisicaoAmizade-factory.js'
                                ]
                            },
                           {
                               name: 'RequisicaoAmizadeController',
                               files: [
                                   'library/scripts/app/Sistema/RequisicaoAmizade/RequisicaoAmizade-edit-controller.js',
                                   'library/scripts/app/Sistema/RequisicaoAmizade/RequisicaoAmizade-controller.js'
                               ]
                           }, {
                               name: 'UsuarioFactory',
                               files: [
                                   'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                               ]
                           },
                             {
                                 name: 'UsuarioFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                 ]
                             },
                       ]);

                   }]
               }
           })
           .state('Sugestao', {
               url: '/Sugestao',
               templateUrl: 'Sistema/Sugestao',
               controller: 'SugestaoCtrl',
               controllerAs: 'itemSugestao',
               authenticate: true,
               params: {
                   filtro: null
               },
               resolve: {
                   deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([
                            {
                                name: 'SugestaoFactory',
                                files: [
                                    'library/scripts/app/Sistema/Sugestao/Sugestao-factory.js'
                                ]
                            },
                           {
                               name: 'SugestaoController',
                               files: [
                                   'library/scripts/app/Sistema/Sugestao/Sugestao-edit-controller.js',
                                   'library/scripts/app/Sistema/Sugestao/Sugestao-controller.js'
                               ]
                           }, {
                               name: 'CidadeFactory',
                               files: [
                                   'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                               ]
                           },
                             {
                                 name: 'UsuarioFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                 ]
                             },
                             {
                                 name: 'ViagemFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                 ]
                             },
                       ]);

                   }]
               }
           })
           .state('VerificarSugestao', {
               url: '/VerificarSugestao',
               templateUrl: 'Sistema/VerificarSugestao',
               controller: 'VerificarSugestaoCtrl',
               controllerAs: 'itemVerificarSugestao',
               authenticate: true,
               params: {
                   filtro: null
               },
               resolve: {
                   deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([
                            {
                                name: 'SugestaoFactory',
                                files: [
                                    'library/scripts/app/Sistema/Sugestao/Sugestao-factory.js'
                                ]
                            },
                           {
                               name: 'VerificarSugestaoController',
                               files: [
                                   'library/scripts/app/Sistema/VerificarSugestao/VerificarSugestao-edit-controller.js',
                                   'library/scripts/app/Sistema/VerificarSugestao/VerificarSugestao-controller.js'
                               ]
                           }, {
                               name: 'CidadeFactory',
                               files: [
                                   'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                               ]
                           },
                             {
                                 name: 'UsuarioFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                 ]
                             },
                             {
                                 name: 'ViagemFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                 ]
                             },
                       ]);

                   }]
               }
           })
            .state('Viagem', {
                url: '/Viagem',
                templateUrl: 'Sistema/Viagem',
                controller: 'ViagemCtrl',
                controllerAs: 'itemViagem',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'ViagemFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                 ]
                             },
                            {
                                name: 'ViagemController',
                                files: [
                                    'library/scripts/app/Sistema/Viagem/Viagem-edit-controller.js',
                                    'library/scripts/app/Sistema/Viagem/Viagem-controller.js'
                                ]
                            }, {
                                name: 'UsuarioFactory',
                                files: [
                                    'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                ]
                            },
                        ]);

                    }]
                }
            })
            .state('ViagemEdicao', {
                url: '/ViagemEdicao/:id',
                templateUrl: 'Sistema/ViagemEdicao',
                controller: 'ViagemEditCtrl',
                controllerAs: 'itemViagemEdit',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'ViagemFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                 ]
                             },
                            {
                                name: 'ViagemController',
                                files: [
                                    'library/scripts/app/Sistema/Viagem/Viagem-edit-controller.js',
                                    'library/scripts/app/Sistema/Viagem/Viagem-controller.js'
                                ]
                            }, {
                                name: 'UsuarioFactory',
                                files: [
                                    'library/scripts/app/Sistema/Usuario/Usuario-factory.js'
                                ]
                            },
                        ]);

                    }]
                }
            })
            .state('ViagemAerea', {
                url: '/ViagemAerea',
                templateUrl: 'Sistema/ViagemAerea',
                controller: 'ViagemAereaCtrl',
                controllerAs: 'itemViagemAerea',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'ViagemAereaFactory',
                                 files: [
                                     'library/scripts/app/Sistema/ViagemAerea/ViagemAerea-factory.js'
                                 ]
                             },
                            {
                                name: 'ViagemAereaController',
                                files: [
                                    'library/scripts/app/Sistema/ViagemAerea/ViagemAerea-edit-controller.js',
                                    'library/scripts/app/Sistema/ViagemAerea/ViagemAerea-controller.js'
                                ]
                            }, {
                                name: 'GastoFactory',
                                files: [
                                    'library/scripts/app/Sistema/Gasto/Gasto-factory.js'
                                ]
                            },
                             {
                                 name: 'GastoController',
                                 files: [
                                     'library/scripts/app/Sistema/Gasto/Gasto-edit-controller.js',
                                     'library/scripts/app/Sistema/Gasto/Gasto-controller.js'
                                 ]
                             },
                           {
                               name: 'CidadeFactory',
                               files: [
                                   'library/scripts/app/Sistema/Cidade/Cidade-factory.js'
                               ]
                           },
                              {
                                  name: 'ViagemFactory',
                                  files: [
                                      'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                  ]
                              },

                        ]);

                    }]
                }
            })
            .state('ConsultarExtratoMoeda', {
                url: '/ConsultarExtratoMoeda',
                templateUrl: 'Sistema/ConsultarExtratoMoeda',
                controller: 'ExtratoMoedaCtrl',
                controllerAs: 'itemExtratoMoeda',
                authenticate: true,
                params: {
                    filtro: null
                },
                resolve: {
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                             {
                                 name: 'ViagemFactory',
                                 files: [
                                     'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                 ]
                             },
                            {
                                name: 'ConsultarExtratoMoedaController',
                                files: [
                                    'library/scripts/app/Sistema/Consulta/ExtratoMoeda-controller.js'
                                ]
                            }, {
                                name: 'ConsultaFactory',
                                files: [
                                    'library/scripts/app/Sistema/Consulta/Consulta-factory.js'
                                ]
                            }

                        ]);

                    }]
                }
            })
           .state('ConsultarAcertoConta', {
               url: '/ConsultarAcertoConta',
               templateUrl: 'Sistema/ConsultarAcertoConta',
               controller: 'AcertoContaCtrl',
               controllerAs: 'itemExtratoMoeda',
               authenticate: true,
               params: {
                   filtro: null
               },
               resolve: {
                   deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                       return $ocLazyLoad.load([
                            {
                                name: 'ViagemFactory',
                                files: [
                                    'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                                ]
                            },
                           {
                               name: 'ConsultarAcertoContaController',
                               files: [
                                   'library/scripts/app/Sistema/Consulta/AcertoConta-controller.js'
                               ]
                           }, {
                               name: 'ConsultaFactory',
                               files: [
                                   'library/scripts/app/Sistema/Consulta/Consulta-factory.js'
                               ]
                           }

                       ]);

                   }]
               }
           })
        .state('ConsultarRelatorioGasto', {
            url: '/ConsultarRelatorioGasto',
            templateUrl: 'Sistema/ConsultarRelatorioGastos',
            controller: 'RelatorioGastoCtrl',
            controllerAs: 'itemExtratoMoeda',
            authenticate: true,
            params: {
                filtro: null
            },
            resolve: {
                deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load([
                         {
                             name: 'ViagemFactory',
                             files: [
                                 'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                             ]
                         },
                        {
                            name: 'RelatorioGastoController',
                            files: [
                                'library/scripts/app/Sistema/Consulta/RelatorioGasto-controller.js'
                            ]
                        }, {
                            name: 'ConsultaFactory',
                            files: [
                                'library/scripts/app/Sistema/Consulta/Consulta-factory.js'
                            ]
                        }

                    ]);

                }]
            }
        })

        .state('ConsultarTimeline', {
            url: '/ConsultarTimeline',
            templateUrl: 'Sistema/ConsultarTimeline',
            controller: 'TimelineCtrl',
            controllerAs: 'itemExtratoMoeda',
            authenticate: true,
            params: {
                filtro: null
            },
            resolve: {
                deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                    return $ocLazyLoad.load([
                         {
                             name: 'ViagemFactory',
                             files: [
                                 'library/scripts/app/Sistema/Viagem/Viagem-factory.js'
                             ]
                         },
                        {
                            name: 'TimelineController',
                            files: [
                                'library/scripts/app/Sistema/Consulta/Timeline-controller.js'
                            ]
                        }, {
                            name: 'ConsultaFactory',
                            files: [
                                'library/scripts/app/Sistema/Consulta/Consulta-factory.js'
                            ]
                        }

                    ]);

                }]
            }
        })

        ;
    }
}());