(function () {
    'use strict';

    /**
     * @ngdoc service
     * @name archApp.factory:Error
     *
     * @description
     *
     */
    angular
      .module('CV')
      .factory('Error',['$rootScope', Error]);

    function Error($rootScope) {
        var ErrorBase = {};
        ErrorBase.errorObject = {
            type: '',
            title: '',
            message: '',
            autoClose: ''
        };

        ErrorBase.showError = function (type, title, message, autoClose) {
            ErrorBase.errorObject.type = type;
            ErrorBase.errorObject.title = title;
            ErrorBase.errorObject.message = message;
            ErrorBase.errorObject.autoClose = autoClose;
            $rootScope.$emit('alerta');
        };



        return ErrorBase;
    }
}());