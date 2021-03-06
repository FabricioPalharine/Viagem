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
    .factory('Sugestao', ['$resource', Sugestao]);

  function Sugestao($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Sugestao/:controller/:id', { id: '@id' }, {
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
          listarConsulta:
              {
                  method: 'GET',
                  params: {
                      controller: 'listarConsulta',
                      json: 'json'
                  },
                  //  transformResponse: parseArray,
                  isArray: true
              },
          save: {
              method: 'POST',
              params: {
                  controller: 'Post'
              }
          },
          AgendarSugestao: {
              method: 'POST',
              params: {
                  controller: 'AgendarSugestao'
              }
          },
         

      });
    return UserBase;
  }
}());