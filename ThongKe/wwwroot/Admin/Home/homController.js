var app = angular.module("app", ["chart.js"]);

app.config(['ChartJsProvider', function (ChartJsProvider) {
    // Configure all charts
    ChartJsProvider.setOptions({
        //colours: ['#72C02C', '#3498DB', '#717984', '#F1C40F'],
        colours: [
            { // blue
                fillColor: "rgba(62, 92, 154,1)",
                strokeColor: "rgba(151,200,205,1)",
                pointColor: "rgba(151,187,205,1)",
                pointStrokeColor: "#fff",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(151,187,205,0.8)"
            },
            { // dark grey
                fillColor: "rgba(255, 242, 0,1)",
                strokeColor: "rgba(255,200,96,1)",
                pointColor: "rgba(77,83,96,1)",
                pointStrokeColor: "#fff",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(77,83,96,1)"
            }
        ],
        responsive: true
    });
    // Configure all line charts
    //ChartJsProvider.setOptions('Bar', {
    //    datasetFill: false
    //});

}])

app.controller("ChartController", ChartController);

ChartController.$inject = ['$scope', '$http'];

function ChartController($scope,$http) {
    var vm = this;

    $scope.labels = [];
    $scope.series = ['SK Hiện tại', 'SK Tháng trước'];
    //$scope.series = ['Series A', 'Series B'];

    $scope.data = [];
    $scope.tableData = [];

    //$scope.colours = ['#72C02C', '#3498DB', '#717984', '#F1C40F'];

    $http({
        url: '/Home/LoadDataThongKeSoKhachOB',
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
