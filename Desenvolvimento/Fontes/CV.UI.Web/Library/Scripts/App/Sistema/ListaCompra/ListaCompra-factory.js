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
    .factory('ListaCompra', ['$resource', ListaCompra]);

  function ListaCompra($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/ListaCompra/:controller/:id', { id: '@id' }, {
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
              isArray: false
          },
          CarregarListaPedidos: {
              method: 'GET',
              params: {
                  controller: 'CarregarListaPedidos',
                  json: 'json'
              },
              isArray: true
          },
         

      });
    return UserBase;
  }
}());