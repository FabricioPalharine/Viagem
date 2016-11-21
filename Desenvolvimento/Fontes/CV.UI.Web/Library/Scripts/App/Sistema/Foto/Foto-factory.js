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
    .factory('Foto', ['$resource', Foto]);

  function Foto($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Foto/:controller/:id', { id: '@id' }, {
          get: {
              method: 'GET',
              params: {
                  controller: 'Get'
              }
          },
          RetornarAlbum: {
              method: 'Post',
              params: {
                  controller: 'RetornarAlbum'
              },
              isArray: true
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
          SubirImagem: {
              method: 'Post',
              params: {
                  controller: 'SubirImagem'
              }
          },
          SubirVideo: {
              method: 'Post',
              params: {
                  controller: 'SubirVideo'
              }
          },
          save: {
              method: 'Post',
              params: {
                  controller: 'Post'
              }
          },
          saveFotoAtracao: {
              method: 'Post',
              params: {
                  controller: 'saveFotoAtracao'
              }
          },
          saveFotoHotel: {
              method: 'Post',
              params: {
                  controller: 'saveFotoHotel'
              }
          },
          saveFotoRefeicao: {
              method: 'Post',
              params: {
                  controller: 'saveFotoAtracao'
              }
          },
          saveFoto: {
              method: 'Post',
              params: {
                  controller: 'saveFoto'
              }
          }

      });



    return UserBase;
  }
}());