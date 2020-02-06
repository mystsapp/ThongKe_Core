
var quayTheoNgayDiController = {
    init: function () {

        quayTheoNgayDiController.registerEvent();
    },

    registerEvent: function () {

        //$('.modal-dialog').draggable();

        $('.btnExportDetail').off('click').on('click', function () {

            var tungay = $('#txtTuNgay').val();
            var denngay = $('#txtDenNgay').val();
            var daily = $(this).data('daily');
            var cn = $('#hidCn').data('cn');
            var chinhanh = $(this).data('cn');
            //var khoi = '';
            //if (cn === "") {
            //    khoi = $('#ddlKhoi').val();
            //} else {
            //    khoi = $('#hidKhoi').data('khoi');
            //}

            var khoi = $('#ddlKhoi').val();

            $('#hidTuNgay').val(tungay);
            $('#hidDenNgay').val(denngay);
            $('#hidQuay').val(daily);
            $('#hidChiNhanh').val(chinhanh);
            $('#hidKhoi').val(khoi);

            $('#frmDetail').submit();
        });


    }

};
quayTheoNgayDiController.init();