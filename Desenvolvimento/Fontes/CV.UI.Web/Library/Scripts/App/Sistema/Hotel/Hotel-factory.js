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
    .factory('Hotel', ['$resource', Hotel]);

  function Hotel($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Hotel/:controller/:id', { id: '@id' }, {
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
          save:
              {
                  method: 'POST',
                  params: {
                      controller: 'Post'
                  }
              },
          SalvarHotelEvento: 
          {
              method: 'POST',
              params: {
                  controller: 'SalvarHotelEvento'
              }
          }
         

      });
    return UserBase;
  }
}());