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
    .factory('Carro', ['$resource', Carro]);

  function Carro($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Carro/:controller/:id', { id: '@id' }, {
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
          save: {
              method: 'POST',
              params: {
                  controller: 'Post'
              }
          },
          SalvarCarroDeslocamento: {
              method: 'POST',
              params: {
                  controller: 'SalvarCarroDeslocamento'
              }
          },

      });
    return UserBase;
  }
}());