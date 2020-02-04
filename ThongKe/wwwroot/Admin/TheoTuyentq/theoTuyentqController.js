
var theoTuyentqController = {
    init: function () {

        //var nhom = $('#hidNhom').data('nhom');
        //var khoi = '';
        //if (nhom === 'Users') {
        //    khoi = $('#hidKhoi').data('khoi');
        //    theoTuyentqController.loadDdlTuyentq(khoi);
        //}
        //else {
        //theoTuyentqController.loadDdlTuyentq("OB");
        //}
        theoTuyentqController.registerEvent();
    },

    registerEvent: function () {


        $('#ddlKhoi').off('change').on('change', function () {
            var khoi = "";

            khoi = $('#ddlKhoi').val();
            theoTuyentqController.loadDdlTuyentq(khoi);
        });

        $('.btnExportDetail').off('click').on('click', function () {

            var tungay = $('#txtTuNgay').val();
            var denngay = $('#txtDenNgay').val();
            var nhanvien = $(this).data('nhanvien');
            var tuyentq = $(this).data('tuyentq');
            var khoi = '';


            if ($('#hidNhom').val() !== "Users") {
                khoi = $('#ddlKhoi').val();
            } else {
                khoi = $('#hidKhoi').data('khoi');
            }

            $('#hidTuNgay').val(tungay);
            $('#hidDenNgay').val(denngay);
            $('#hidNhanVien').val(nhanvien);
            $('#hidTuyentq').val(tuyentq);
            $('#hidKhoi').val(khoi);

            $('#frmDetail').submit();

        });
    },

    loadDdlTuyentq: function (khoi) {
        $('#ddlTuyentq').html('');
        var option = '';

        $.ajax({
            url: '/BaoCao/GetAllTuyentqByKhoi',
            type: 'GET',
            data: {
                khoi: khoi
            },
            dataType: 'json',
            success: function (response) {

                var data = JSON.parse(response.data);
                //$('#ddlTuyentq').html('');

                $.each(data, function (i, item) {
                    option = option + '<option value="' + item.tuyentq + '">' + item.tuyentq + '</option>'; //chinhanh1

                });

                //for (var i = 0; i < data.length; i++) {
                //    // set the key/property (input element) for your object
                //    var ele = data[i];
                //    //console.log(ele);
                //    option = option + '<option value="' + ele + '">' + ele + '</option>'; //chinhanh1
                //    // add the property to the object and set the value
                //    //params[ele] = $('#' + ele).val();
                //}

                $('#ddlTuyentq').html(option);

            }
        });

    }

};
theoTuyentqController.init();