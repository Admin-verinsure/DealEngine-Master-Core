var app = angular.module('MyApp', []);
app.controller('InformationController', ['$Scope', '$http', function ($scope, $http){
    $scope.SiteMenu = [];
    $http.get('URL for fetch data (sitemenu)').then(function (data){
        $scope.SiteMenu = data.data;
    }, function (error) {
        alert('Error');

    })
}])
