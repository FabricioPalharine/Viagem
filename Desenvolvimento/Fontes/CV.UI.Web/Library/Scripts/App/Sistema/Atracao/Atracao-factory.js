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
    .factory('Atracao', ['$resource', Atracao]);

  function Atracao($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Atracao/:controller/:id', { id: '@id' }, {
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
          VerificarAtracaoAberto: {
              method: 'GET',
              params: {
                  controller: 'VerificarAtracaoAberto'
              },
              isArray: false
          },

      });
    return UserBase;
  }
}());