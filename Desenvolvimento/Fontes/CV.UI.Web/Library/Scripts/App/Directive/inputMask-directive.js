(function () {
    'use strict';

    angular
      .module('CV')
      .directive('inputMask', function () {
          return {
              restrict: 'A',
              link: function (scope, el, attrs) {
                  $(el).inputmask(scope.$eval(attrs.inputMask));
                  $(el).on('change', function () {
                      if (el.val()) {
                          if (!attrs.datapattern)
                              scope.$eval(attrs.ngModel + "='" + el.val() + "'");
                          else
                              scope.$eval(attrs.ngModel + "='" + moment(el.val(), attrs.datapattern.toUpperCase()).toISOString()+ "'");
                      }
                      else
                          scope.$eval(attrs.ngModel + "=null");
                      // or scope[attrs.ngModel] = el.val() if your expression doesn't contain dot.
                  });
              }
          };

      })
    .directive('integerInput', function () {
        return {
            restrict: 'A',
            link: function (scope, el, attrs) {
                $(el).inputmask('integer',{ allowMinus: false, allowPlus: true, radixPoint:  '' });
            }
        };

    })
    .directive('decimalInput', function () {
        return {
            restrict: 'A',
            require: '?ngModel',

            link: function (scope, el, attrs, ctrl) {
                var Propriedades = {
                    radixPoint: ',',
                    autoGroup: true,
                    groupSeparator: '.',
                    groupSize: 3,
                    digits: 2,
                    integerDigits: 8
                }
                var atributos = scope.$eval(attrs.decimalInput);
                if (atributos.radixPoint)
                    Propriedades.radixPoint = atributos.radixPoint;
                if (atributos.groupSeparator)
                    Propriedades.groupSeparator = atributos.groupSeparator;
                if (atributos.digits)
                    Propriedades.digits = atributos.digits;
                if (atributos.integerDigits)
                    Propriedades.integerDigits = atributos.integerDigits;

                $(el).inputmask('decimal', Propriedades);
                ctrl.$formatters.unshift(function (a) {
                    if (ctrl.$modelValue)
                        return ctrl.$modelValue.toLocaleString("pt-Br", {
                            useGrouping: true,
                            minimumSignificantDigits: 4

                        });
                    else
                        return ctrl.$modelValue;
                });
                ctrl.$parsers.unshift(function (viewValue) {
                    var plainNumber = viewValue.split(Propriedades.groupSeparator).join("");
                    plainNumber = plainNumber.split(Propriedades.radixPoint).join(".");
                    plainNumber = parseFloat(plainNumber);
                    return plainNumber;
                });


                //$(el).on('change', function () {

                //    if (!attrs.datapattern)
                //        scope.$eval(attrs.ngModel + "='" + el.val() + "'");
                //    else
                //        scope.$eval(attrs.ngModel + "='" + Date.parseString(el.val(), attrs.datapattern) + "'");
                //    // or scope[attrs.ngModel] = el.val() if your expression doesn't contain dot.
                //});
            }
        };

    })
      .directive(
        'dateInput', ['dateFilter',        function (dateFilter) {
            return {
                require: 'ngModel',
                template: '<input type="text"></input>',
                replace: true,
                link: function (scope, elm, attrs, ngModelCtrl) {
                    ngModelCtrl.$formatters.unshift(function (modelValue) {
                        if (modelValue)
                            return moment(modelValue).format(attrs.datapattern.toUpperCase());
                    });


                },
            };
        }])

    .directive("ngInject", function ($parse, $interpolate, $controller, $compile) {
        return {
            terminal: true,
            transclude: true,
            priority: 510,
            link: function (scope, element, attrs, ctrls, transclude) {

                if (!attrs.ngController) {
                    element.removeAttr("ng-inject");
                    $compile(element)(scope);
                    return;
                }

                var controllerName = attrs.ngController;

                var newScope = scope.$new(false);

                var locals = $parse(attrs.ngInject)(scope);
                locals.$scope = newScope;

                var controller = $controller(controllerName, locals);

                element.data("ngControllerController", controller);

                element.removeAttr("ng-inject").removeAttr("ng-controller");
                $compile(element)(newScope);
                transclude(newScope, function (clone) {
                    element.append(clone);
                });
                // restore to hide tracks
                element.attr("ng-controller", controllerName);
            }
        };
    });

}());