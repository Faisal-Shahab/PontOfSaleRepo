var AjaxInterceptor = function () {

    $(document)

        .ajaxStart(function () {

            KTApp.block(".loaderDiv", {
                overlayColor: '#000000',
                state: 'primary',
                message: 'Processing...'
            });
            $(".btnSubmit").addClass("disabled");
        })
        .ajaxStop(function () {
            KTApp.unblock(".loaderDiv", {});
            $(".btnSubmit").removeClass("disabled");
        });

}();
