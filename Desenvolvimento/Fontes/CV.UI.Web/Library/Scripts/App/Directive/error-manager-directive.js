(function () {
    'use strict';

    angular
      .module('CV')
      .directive('errorManager',['Error', '$rootScope', '$timeout', errorManager]);

    function errorManager(Error, $rootScope, $timeout) {
        return {
            restrict: 'EA',
            scope: {},
            templateUrl: 'Home/Alerta',
            replace: false,
            controllerAs: 'errorManager',
            controller: function () {
                var vm = this;
                var errorObject = {};

                vm.show = false;
                $rootScope.$on('alerta', function (event) {
               //     console.log('alerta');
                    vm.show = true;
                    vm.errorObject = Error.errorObject;
                  //  console.log(vm.errorObject);
                    if (vm.errorObject.autoClose === true) {
                         $timeout(function() {

vm.show = false;
},3000);
                       
                    }
                });

                vm.close = function () {
                    vm.show = false;
                };
            },
            link: function (scope, element, attrs) {

                /* jshint unused:false */
                /* eslint "no-unused-vars": [2, {"args": "none"}] */
               // console.log('to na diretiva erro');
                //$(".alert").fadeTo(2000, 500).slideUp(500, function () {
                //    $(".alert").alert('close');
                //});
            }
        };
    }
}());