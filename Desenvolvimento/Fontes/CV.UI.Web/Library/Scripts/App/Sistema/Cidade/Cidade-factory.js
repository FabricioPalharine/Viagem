(function () {
  'use strict';

  /**
   * @ngdoc service
   * @name account.factory:User
   *
   * @description
   *
   */
  angular
    .module('Sistema')
    .factory('Cidade', ['$resource', Cidade]);

  function Cidade($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Cidade/:controller/:id', { id: '@id' }, {
          get: {
              method: 'GET',
              params: {
                  controller: 'Get'
              }
          },
          
          list: {
              method: 'GET',
              params: {
                  controller: 'Get',
                  json: 'json'
              },
            //  transformResponse: parseArray,
              isArray: false
          },
          CarregarFoto: {
              method: 'GET',
              params: {
                  controller: 'CarregarFoto'
              },
              isArray: true
          },
          CarregarAtracao: {
              method: 'GET',
              params: {
                  controller: 'CarregarAtracao'
              },
              isArray: true
          },
          CarregarRefeicao: {
              method: 'GET',
              params: {
                  controller: 'CarregarRefeicao'
              },
              isArray: true
          },
          CarregarHotel: {
              method: 'GET',
              params: {
                  controller: 'CarregarHotel'
              },
              isArray: true
          },
          CarregarLoja: {
              method: 'GET',
              params: {
                  controller: 'CarregarLoja'
              },
              isArray: true
          },
          CarregarComentario: {
              method: 'GET',
              params: {
                  controller: 'CarregarComentario'
              },
              isArray: true
          },
          CarregarSugestao: {
              method: 'GET',
              params: {
                  controller: 'CarregarSugestao'
              },
              isArray: true
          },
          CarregarViagemAerea: {
              method: 'GET',
              params: {
                  controller: 'CarregarViagemAerea'
              },
              isArray: true
          },
          ListarCidadePai: {
              method: 'GET',
              params: {
                  controller: 'ListarCidadePai'
              },
              isArray: true
          },
          ListarCidadeNaoAssociadasFilho: {
              method: 'GET',
              params: {
                  controller: 'ListarCidadeNaoAssociadasFilho'
              },
              isArray: true
          },
          ListarCidadeNaoAssociadasPai: {
              method: 'GET',
              params: {
                  controller: 'ListarCidadeNaoAssociadasPai'
              },
              isArray: true
          },
      });
    return UserBase;
  }
}());