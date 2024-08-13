function readOnlyTagController($scope, $controller) {
    angular.extend(this, $controller('Umbraco.PropertyEditors.TagsController', { $scope: $scope }));
}
angular.module('umbraco').controller("readOnlyTagController", readOnlyTagController);
