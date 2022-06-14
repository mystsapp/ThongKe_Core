function addCommas(x) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".");
}


var indexController = {
    init: function () {
        optionValue = $('#ddlRoles').val();
        indexController.DdlRolesChange(optionValue);

        indexController.registerEvent();
    },
    registerEvent: function () {

        // format .numbers
        $('input.numbers').keyup(function (event) {

            // Chỉ cho nhập số
            if (event.which >= 37 && event.which <= 40) return;

            $(this).val(function (index, value) {
                return addCommas(value);
            });
        });


        //$('#ddlRoles').on('change', function () {

        //    optionValue = $(this).val();

        //    indexController.DdlRolesChange(optionValue);

        //});

    },
    //DdlRolesChange: function (optionValue) {

    //    if (optionValue === '1' /*|| optionValue === '2'*/) {// Admins(1), Users(2)
    //        $('#ddlPhongBans').prop('disabled', true);
    //    }
    //    else {
    //        $('#ddlPhongBans').prop('disabled', false);
    //    }

    //}
};
indexController.init();