//var app = angular.module("app", ["chart.js"]);

app.controller("ChartNDController", ChartNDController);

ChartNDController.$inject = ['$scope', '$http'];

function ChartNDController($scope,$http) {
    var vm = this;

    $scope.labels = [];
    $scope.series = ['SK Hiện tại', 'SK Tháng trước'];
    //$scope.series = ['Series A', 'Series B'];

    $scope.data = [];
    $scope.tableData = [];

    //$scope.colours = ['#72C02C', '#3498DB', '#717984', '#F1C40F'];

    $http({
        url: '/Home/LoadDataThongKeSoKhachND',
        type: 'GET'
        //data: {
        //    tungay: "01/01/2016",
        //    denngay: "01/01/2019",
        //    chinhanh: "STS",
        //    khoi: "OB"
        //}
    }).then(function successCallback(response) {
        // this callback will be called asynchronously
        // when the response is available

        var labels = [];
        var chartData = [];
        var sokhachht = [];
        var sokhachtt = [];
        

        var ajaxdata = response.data;
        var tableData = ajaxdata.data;

        $.each(tableData, function (i, item) {
            labels.push(item.daiLyXuatVe);
            sokhachht.push(item.soKhachHT); 
            sokhachtt.push(item.soKhachTT);
        });

        chartData.push(sokhachht);
        chartData.push(sokhachtt);
        $scope.data = chartData;
        $scope.labels = labels;
        $scope.tableData = tableData;

    }, function errorCallback(response) {
        // called asynchronously if an error occurs
        // or server returns response with an error status.
        $scope.error = response.statusText;
    });

}
