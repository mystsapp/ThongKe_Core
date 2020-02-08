
var doanNgayDiController = {
    init: function () {

        doanNgayDiController.registerEvent();
    },

    registerEvent: function () {

        //$('.modal-dialog').draggable();

        $('.btnExportDetail').off('click').on('click', function () {

            var sgtcode = $(this).data('sgtcode');
            var khoi = '';


            if ($('#hidNhom').val() !== "Users") {
                khoi = $('#ddlKhoi').val();
            } else {
                khoi = $('#hidKhoi').data('khoi');
            }

            $('#hidSgtcode').val(sgtcode);
            $('#hidKhoi').val(khoi);

            $('#frmDetail').submit();
        });


    }

};
doanNgayDiController.init();