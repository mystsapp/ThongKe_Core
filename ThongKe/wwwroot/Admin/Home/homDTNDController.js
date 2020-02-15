//var app = angular.module("app", ["chart.js"]);

app.controller("ChartDTNDController", ChartDTNDController);

ChartDTNDController.$inject = ['$scope', '$http'];

function ChartDTNDController($scope,$http) {
    var vm = this;

    $scope.labels = [];
    $scope.series = ['Doanh Thu Hiện tại', 'Doanh Thu Tháng trước'];
    //$scope.series = ['Series A', 'Series B'];

    $scope.data = [];
    $scope.tableData = [];

    //$scope.colours = ['#72C02C', '#3498DB', '#717984', '#F1C40F'];

    $http({
        url: '/Home/LoadDataThongKeDoanhThuND',
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
        var doanhthuht = [];
        var doanhthutt = [];
        

        var ajaxdata = response.data;
        var tableData = ajaxdata.data;

        $.each(tableData, function (i, item) {
            labels.push(item.daiLyXuatVe);
            doanhthuht.push(item.doanhThuHT); 
            doanhthutt.push(item.doanhThuTT);
        });

        chartData.push(doanhthuht);
        chartData.push(doanhthutt);
        $scope.data = chartData;
        $scope.labels = labels;
        $scope.tableData = tableData;

    }, function errorCallback(response) {
        // called asynchronously if an error occurs
        // or server returns response with an error status.
        $scope.error = response.statusText;
    });

}
