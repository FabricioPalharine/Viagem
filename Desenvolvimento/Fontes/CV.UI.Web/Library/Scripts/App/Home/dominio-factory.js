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
    .module('home')
    .factory('Dominio', ['$resource', Dominio]);

  function Dominio($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Dominio/:controller/:id', { id: '@id' }, {
                    
          CarregaMoedas: {
              method: 'GET',
              params: {
                  controller: 'CarregaMoedas',
              },
            //  transformResponse: parseArray,
              isArray: true
          },
          CarregaTipoTransporte: {
              method: 'GET',
              params: {
                  controller: 'CarregaTipoTransporte',
              },
              //  transformResponse: parseArray,
              isArray: true
          },
          CarregaTipoParada: {
              method: 'GET',
              params: {
                  controller: 'CarregaTipoParada',
              },
              //  transformResponse: parseArray,
              isArray: true
          },

      });
    return UserBase;
  }
}());