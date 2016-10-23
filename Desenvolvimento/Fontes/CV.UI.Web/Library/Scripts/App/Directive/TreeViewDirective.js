(function () {
    'use strict';

    angular
      .module('CV')
      .config(function($provide) {
  $provide.decorator('ivhTreeviewCheckboxDirective', function($delegate) {
    $delegate.shift();
    return $delegate;
  });
})
.directive('ivhTreeviewCheckbox', function() {
  return {
    scope: {
      node: '=ivhTreeviewCheckbox'
    },
    template: '<input type="checkbox" ng-model="node.selected" ng-click="resolveIndeterminateClick()" ng-change="trvw.select(node, isSelected)"   />'
  };
});


 }());