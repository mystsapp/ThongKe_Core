
var kinhDoanhOnlineNgayDiController = {
    init: function () {

        kinhDoanhOnlineNgayDiController.registerEvent();
    },

    registerEvent: function () {

        //$('.modal-dialog').draggable();

        $('.btnExportDetail').off('click').on('click', function () {

            var tungay = $('#txtTuNgay').val();
            var denngay = $('#txtDenNgay').val();
            var cn = $(this).data('chinhanh');
            var khoi = '';

            if ($('#hidNhom').val() !== "Users") {
                khoi = $('#ddlKhoi').val();
            } else {
                khoi = $('#hidKhoi').data('khoi');
            }

            $('#hidTuNgay').val(tungay);
            $('#hidDenNgay').val(denngay);
            $('#hidChiNhanh').val(cn);
            $('#hidKhoi').val(khoi);

            $('#frmDetail').submit();
        });


    }

}
kinhDoanhOnlineNgayDiController.init();