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
    .factory('Amigo', ['$resource', Amigo]);

  function Amigo($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Amigo/:controller/:id', { id: '@id' }, {
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
          save:
          {
              method: 'POST',
              params:
                  {
                      controller: 'Post'
                  }
          },
          RequisicaoAmizade:
          {
              method: 'POST',
              params:
                  {
                      controller: 'AjustarAmigo'
                  }
          }
         

      });
    return UserBase;
  }
}());