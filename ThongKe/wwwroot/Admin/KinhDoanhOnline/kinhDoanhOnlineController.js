
var kinhDoanhOnlineController = {
    init: function () {

        kinhDoanhOnlineController.registerEvent();
    },

    registerEvent: function () {

        //$('.modal-dialog').draggable();

        $('.btnExportDetail').off('click').on('click', function () {

            var tungay = $('#txtTuNgay').val();
            var denngay = $('#txtDenNgay').val();
            var cn = $(this).data('chinhanh');
            var khoi = '';

            if (cn === "") {
                cn = $('#ddlChiNhanh').val();
                khoi = $('#ddlKhoi').val();
            } else {
                khoi = $('#hidKhoi').data('khoi');
            }

            if ($('#hidNhom').val() !== "Users") {
                khoi = $('#ddlKhoi').val();
            } else {
                khoi = $('#hidKhoi').data('khoi');
            }

            $('#hidTuNgay').val(tungay);
            $('#hidDenNgay').val(denngay);

            $('#hidKhoi').val(khoi);
            $('#hidChiNhanh').val(cn);
            $('#frmDetail').submit();
            //alert(daily);
            //quayTheoNgayDiController.ExportDetail();
        });


    }

}
kinhDoanhOnlineController.init();