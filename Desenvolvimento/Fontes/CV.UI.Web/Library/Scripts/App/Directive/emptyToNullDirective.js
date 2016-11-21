(function () {
    'use strict';

    angular
      .module('CV')
      .directive('emptyToNull', function () {
          return {
              restrict: 'A',
              require: 'ngModel',
              link: function (scope, elem, attrs, ctrl) {
                  ctrl.$parsers.push(function (viewValue) {
                      if (viewValue === "") {
                          return null;
                      }
                      return viewValue;
                  });
              }
          };
      }).filter('newlines', function () {
          return function (text) {
              return text.replace(/(?:\r\n|\r|\n)/g, '<br />');
          }
      })
.filter("sanitize", ['$sce', function ($sce) {
    return function (htmlCode) {
        return $sce.trustAsHtml(htmlCode);
    }
}])
    .filter('trustAsResourceUrl', ['$sce', function ($sce) {
        return function (val) {
            return $sce.trustAsResourceUrl(val);
        };
    }]).
    directive('iframeOnload', [function () {
        return {
            scope: {
                callBack: '&iframeOnload'
            },
            link: function (scope, element, attrs) {
                element.on('load', function () {
                    return scope.callBack();
                })
            }
        }
    }])
    .directive('fileDropzone', function () {
        return {
            restrict: 'A',
            scope: {
                file: '=',
                fileName: '=',
                onFileUploaded: "&"
            },
            link: function (scope, element, attrs) {
                var checkSize, isTypeValid, processDragOverOrEnter, validMimeTypes;
                processDragOverOrEnter = function (event) {
                    if (event != null) {
                        event.preventDefault();
                    }
                    if (!event.dataTransfer)
                        event.dataTransfer = event.originalEvent.dataTransfer;
                    event.dataTransfer.effectAllowed = 'copy';
                    return false;
                };
                validMimeTypes = attrs.fileDropzone;

                element.bind('dragover', processDragOverOrEnter);
                element.bind('dragenter', processDragOverOrEnter);
                return element.bind('drop', function (event) {
                    var file;
                    if (event != null) {
                        event.preventDefault();
                    }
                    if (!event.dataTransfer)
                        event.dataTransfer = event.originalEvent.dataTransfer;
                    for (var i = 0; i < event.dataTransfer.files.length; i++)
                        scope.onFileUploaded({ item: event.dataTransfer.files[i] });

                    return false;
                });
            }
        };
    })
    .directive('starRating', function(){
            return {
                restrict: 'EA',
                template:
                  '<ul class="star-rating" ng-class="{readonly: readonly}">' +
                  '  <li ng-repeat="star in stars" class="star" ng-class="{filled: star.filled}" ng-click="toggle($index)">' +
                  '    <i class="fa fa-star"></i>' + // or &#9733
                  '  </li>' +
                  '</ul>',
                scope: {
                    ratingValue: '=ngModel',
                    max: '=?', // optional (default is 5)
                    onRatingSelect: '&?',
                    readonly: '=?'
                },
                link: function(scope, element, attributes) {
                    if (scope.max == undefined) {
                        scope.max = 5;
                    }
                    updateStars();
                    function updateStars() {
                        scope.stars = [];
                        for (var i = 0; i < scope.max; i++) {
                            scope.stars.push({
                                filled: i < scope.ratingValue
                            });
                        }
                    };
                    scope.toggle = function(index) {
                        if (scope.readonly == undefined || scope.readonly === false){
                            scope.ratingValue = index + 1;
                            updateStars();
                            if (scope.onRatingSelect) {
                                scope.onRatingSelect({
                                    rating: index + 1
                                });
                            }
                        }
                    };
                    scope.$watch('ratingValue', function(oldValue, newValue) {
                        if (newValue) {
                            updateStars();
                        }
                    });
                }
            };
        }
    )
     .directive('classValidation', function () {
         return function (scope, element, attrs) {
             scope.$watch(function () { return element.attr('class'); },
                 function (newValue) {
                 });

         }
     });

 }());