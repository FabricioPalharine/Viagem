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
    .factory('Consulta', ['$resource', Consulta]);

  function Consulta($resource) {
      var parseArray = function (data, header) {
          data = angular.fromJson(data);
          angular.forEach(data, parseItem);

          return data;
      };

      var UserBase = $resource('api/Consulta/:controller/:id', { id: '@id' }, {      
          
          ConsultarExtratoMoeda: {
              method: 'GET',
              params: {
                  controller: 'ConsultarExtratoMoeda',
                  json: 'json'
              },
            //  transformResponse: parseArray,
              isArray: true
          },
          ListarGastosAcerto: {
              method: 'GET',
              params: {
                  controller: 'ListarGastosAcerto',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: true
          },
          ListarRelatorioGastos: {
              method: 'GET',
              params: {
                  controller: 'ListarRelatorioGastos',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: true
          },
          ListarTimeline: {
              method: 'GET',
              params: {
                  controller: 'ConsultarTimeline',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: true
          }
      });



    return UserBase;
  }
}());