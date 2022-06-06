"use strict";
var KTAppEcommerceSaveCustomer = function () {

    return {
        init: function () {
            (() => {
                let e;
                const t = document.getElementById("kt_ecommerce_add_customer_form"),

                    o = document.getElementById("kt_ecommerce_add_customer_submit");

                e = FormValidation.formValidation(t, {
                    fields: {
                        Debit: {
                            validators: {
                                notEmpty: {
                                    message: "amount paid is required"
                                }
                            }
                        },
                        SupplierId: {
                            validators: {
                                notEmpty: {
                                    message: "amount paid is required"
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

                                    const debit = parseFloat($('#Debit').val());
                                    const credit = parseFloat($('#Credit').val());
                                    const supplierId = $('#SupplierId').val();
                                    let balance = debit > 0 ? debit : 0.00;

                                    $.post("/supplier/createtransaction", { supplierId: supplierId, balance: balance, debit: 0.00, credit: credit }, function (res) {

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
                                                $('input[type=text]').val(0.00);
                                                $("#SupplierId").val('').trigger('change');

                                                o.removeAttribute("data-kt-indicator");
                                                window.location.href = '/Supplier/Index';
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

            //$('#SupplierId').change(function () {
            //    const custId = $(this).val();
            //    $.post('/supplier/getSupTransactionBalace', { id: custId }, function (response) {
            //        $('input[type=text]').val(0.00);
            //        $('#Balance').val(response);
            //        $('#Debit').val(response);
            //    });
            //});

            $('#Credit').keyup(function () {
                const balance = parseFloat($('#Balance').val());
                const paid = parseFloat($(this).val());
                if (paid > balance) {
                    $(this).val(balance);
                    $('#Debit').val(0.00);
                    return false;
                }
                $('#Debit').val((balance - paid).toFixed(2))
            });

        }
    }
}();
KTUtil.onDOMContentLoaded((function () {
    KTAppEcommerceSaveCustomer.init()
}));