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
    .factory('Gasto', ['$resource', Gasto]);

  function Gasto($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Gasto/:controller/:id', { id: '@id' }, {
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
          }
         ,
          save: {
              method: 'POST',
              params: {
                  controller: 'Post'
              }
          },
          SalvarCustoAtracao: {
              method: 'POST',
              params: {
                  controller: 'SalvarCustoAtracao'
              }
          },
          SalvarCustoRefeicao: {
              method: 'POST',
              params: {
                  controller: 'SalvarCustoRefeicao'
              }
          },
          SalvarCustoHotel: {
              method: 'POST',
              params: {
                  controller: 'SalvarCustoHotel'
              }
          },
          SalvarCustoViagemAerea: {
              method: 'POST',
              params: {
                  controller: 'SalvarCustoViagemAerea'
              }
          },
      });
    return UserBase;
  }
}());