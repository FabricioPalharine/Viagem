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
    .factory('Loja', ['$resource', Loja]);

  function Loja($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Loja/:controller/:id', { id: '@id' }, {
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
          save: {
              method: 'POST',
              params: {
                  controller: 'Post'
              }
          },
          saveCompra: {
              method: 'POST',
              params: {
                  controller: 'saveCompra'
              }
          },
          excluirCompra: {
              method: 'POST',
              params: {
                  controller: 'excluirCompra'
                  }
          },
          SalvarItemCompra: {
              method: 'POST',
              params: {
                  controller: 'SalvarItemCompra'
              }
          }
         

      });
    return UserBase;
  }
}());