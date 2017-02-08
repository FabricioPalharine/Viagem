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
          },
          ListarLocaisVisitados: {
              method: 'GET',
              params: {
                  controller: 'ListarLocaisVisitados',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: true
          },
          ListarPontosViagem: {
              method: 'GET',
              params: {
                  controller: 'ListarPontosViagem',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: true
          },
          ListarLinhasViagem: {
              method: 'GET',
              params: {
                  controller: 'ListarLinhasViagem',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: true
          },
          ListarCalendarioRealizado: {
              method: 'GET',
              params: {
                  controller: 'ConsultarCalendarioRealizado',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: true
          },
          ListarRankings: {
              method: 'GET',
              params: {
                  controller: 'ListarRankings',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: true
          },
          ListarAvaliacoesRankings: {
              method: 'GET',
              params: {
                  controller: 'ListarAvaliacoesRankings',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: true
          },
          CarregarDetalhesAtracao: {
              method: 'GET',
              params: {
                  controller: 'ConsultarDetalheAtracao',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: false
          },
          CarregarDetalhesHotel: {
              method: 'GET',
              params: {
                  controller: 'ConsultarDetalheHotel',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: false
          },
          CarregarDetalhesRestaurante: {
              method: 'GET',
              params: {
                  controller: 'ConsultarDetalheRestaurante',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: false
          },
          CarregarDetalhesLoja: {
              method: 'GET',
              params: {
                  controller: 'ConsultarDetalheLoja',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: false
          },
          CarregarResumo: {
              method: 'GET',
              params: {
                  controller: 'CarregarResumo',
                  json: 'json'
              },
              //  transformResponse: parseArray,
              isArray: false
          },

      });



    return UserBase;
  }
}());