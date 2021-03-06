(function () {
    'use strict';

    /**
     * @ngdoc directive
     * @name archApp.directive:onLongPress
     * @restrict EA
     * @element
     *
     * @description
     *
     * @example
       <example module="archApp">
         <file name="index.html">
          <on-long-press></on-long-press>
         </file>
       </example>
     *
     */
    angular
      .module('CV')
      .directive('onLongPress', ['$timeout', onLongPress])
    .directive('onViewPoint', ['$timeout', onViewPoint]);


    function onViewPoint($timeout) {
        return {
            restrict: 'A',
            scope:
                {
                    onVisible: "&",
                    enabled: "=",
                    endOfPage: "="
                },
            link: function ($scope, $elm, $attrs) {

                var visibleOnPage = $scope.endOfPage = $(window).scrollTop() + $(window).height() >= $elm.offset().top;
                var elemento = $elm;
                $elm.on("css-change", function (event, change) {
                    var newVisibleOnPage = $(window).scrollTop() + $(window).height() >= $elm.offset().top;
                    if (newVisibleOnPage && !visibleOnPage) {
                        $scope.onVisible();
                    }
                    visibleOnPage = $scope.endOfPage = newVisibleOnPage;
                });

                $(window).scroll(function () {
                    if ($scope.enabled) {
                        var newVisibleOnPage = $(window).scrollTop() + $(window).height() >= elemento.offset().top;
                        if (newVisibleOnPage && !visibleOnPage) {
                            $scope.onVisible();
                        }
                        visibleOnPage = $scope.endOfPage = newVisibleOnPage;
                    }
                });

                $elm.csswatch({
                    props: 'top',
                    props_functions: {
                        "top": "offset().top",
                    }
                });

                if (!$scope.enabled)
                    $elm.csswatch('stop')

                $scope.$watch(function () { return $scope.enabled }, function (newVal) {
                    if (newVal) {
                        $elm.csswatch('start');
                        //alert('b');
                    }
                    else {
                        // alert('a');
                        $elm.csswatch('stop');
                    }
                });


                $elm.on('$destroy', function () {
                    $elm.csswatch('destroy')
                });

            }
        };
    };


    function onLongPress($timeout) {
        return {
            restrict: 'A',
            link: function ($scope, $elm, $attrs) {
                $elm.bind('touchstart mousedown', function (evt) {
                    // Locally scoped variable that will keep track of the long press
                    $scope.longPress = true;

                    // We'll set a timeout for 600 ms for a long press
                    $timeout(function () {
                        if ($scope.longPress) {
                            // If the touchend event hasn't fired,
                            // apply the function given in on the element's on-long-press attribute
                            $scope.$apply(function () {
                                $scope.$eval($attrs.onLongPress)
                            });
                        }
                    }, 600);
                });

                $elm.bind('touchend mouseup', function (evt) {
                    // Prevent the onLongPress event from firing
                    $scope.longPress = false;
                    // If there is an on-touch-end function attached to this element, apply it
                    if ($attrs.onTouchEnd) {
                        $scope.$apply(function () {
                            $scope.$eval($attrs.onTouchEnd)
                        });
                    }
                });
            }
        };
    }
}());