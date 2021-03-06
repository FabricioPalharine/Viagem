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
    .factory('Viagem', ['$resource', Viagem]);

  function Viagem($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Viagem/:controller/:id', { id: '@id' }, {
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
          CarregarParticipantes: {
              method: 'GET',
              params: {
                  controller: 'CarregarParticipantes',
              },
              //  transformResponse: parseArray,
              isArray: true
          },
         
          CarregarParticipantesAmigo: {
              method: 'GET',
              params: {
                  controller: 'CarregarParticipantesAmigo',
              },
              //  transformResponse: parseArray,
              isArray: true
          },
      });
    return UserBase;
  }
}());