"use strict";
var KTAppEcommerceSavesupplier = function () {

    return {
        init: function () {
            (() => {
                let e;
                const t = document.getElementById("kt_ecommerce_add_supplier_form"),

                    o = document.getElementById("kt_ecommerce_add_supplier_submit");


                e = FormValidation.formValidation(t, {
                    fields: {
                        Name: {
                            validators: {
                                notEmpty: {
                                    message: "supplier name is required"
                                }
                            }
                        }
                    },
                    plugins: {
                        trigger: new FormValidation.plugins.Trigger,
                        bootstrap: new FormValidation.plugins.Bootstrap5({
                            rowSelector: ".fv-row",
                            eleInvalidClass: "",
                            eleValidClass: ""
                        })
                    }
                }),

                    o.addEventListener("click", (a => {
                        a.preventDefault(), e && e.validate().then((function (e) {

                            "Valid" === e ? (o.setAttribute("data-kt-indicator", "on"),
                                o.disabled = !0,
                                setTimeout((function () {

                                    //var bg = $('.image-input-wrapper').css('background-image');
                                    //bg = bg.replace('url(', '').replace(')', '').replace(/\"/gi, "");
                                    //$("#avatar_base64").val(bg);
                                    //o.removeAttribute("data-kt-indicator");
                                    var data = $('#kt_ecommerce_add_supplier_form').serializeArray();

                                    $.post("/supplier/create", data, function (res) {
                                   
                                        if (res.status) {
                                            Swal.fire({
                                                text: "Form has been successfully submitted!",
                                                icon: "success",
                                                buttonsStyling: !1,
                                                confirmButtonText: "Ok, got it!",
                                                customClass: {
                                                    confirmButton: "btn btn-primary"
                                                }
                                            }).then((function (e) {
                                                e.isConfirmed && (o.disabled = !1)
                                                window.location.href = '/supplier/Index';
                                            }))
                                        } else {
                                            Swal.fire({
                                                text: "Sorry, looks like there are some errors detected, please try again.",
                                                icon: "error",
                                                buttonsStyling: !1,
                                                confirmButtonText: "Ok, got it!",
                                                customClass: {
                                                    confirmButton: "btn btn-primary"
                                                }
                                            })
                                        }
                                    });

                                })))
                                : console.log(e);
                        }))
                    }))
            })()
        }
    }
}();
KTUtil.onDOMContentLoaded((function () {
    KTAppEcommerceSavesupplier.init()
}));