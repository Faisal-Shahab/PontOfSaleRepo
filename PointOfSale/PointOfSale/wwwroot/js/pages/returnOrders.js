"use strict";
var KTAppEcommerceReturn =
    function () {
        var t, e, n = () => {
            t.querySelectorAll('[data-kt-ecommerce-order-filter="delete_row"]').
                forEach((t => {
                    t.addEventListener("click", (function (t) {
                        t.preventDefault();
                        const n = t.target.closest("tr"),
                            o = n.querySelector('td').innerText;
                        Swal.fire({
                            text: "Are you sure you want to delete " + o + "?",
                            icon: "warning",
                            showCancelButton: !0,
                            buttonsStyling: !1,
                            confirmButtonText: "Yes, delete!",
                            cancelButtonText: "No, cancel",
                            customClass: {
                                confirmButton: "btn fw-bold btn-danger",
                                cancelButton: "btn fw-bold btn-active-light-primary"
                            }
                        }).then((function (t) {
                            if (t.value) {
                                $.post('/Order/DeleteSaleOrders', { id: o }, function (res) {
                                    if (res.status) {
                                        Swal.fire({
                                            text: "You have deleted " + o + "!.",
                                            icon: "success",
                                            buttonsStyling: !1,
                                            confirmButtonText: "Ok, got it!",
                                            customClass: {
                                                confirmButton: "btn fw-bold btn-primary"
                                            }
                                        }).then((function () {
                                            e.row($(n)).remove().draw()
                                        }))
                                    } else {
                                        Swal.fire({
                                            text: "Operation failed",
                                            icon: "error",
                                            buttonsStyling: !1,
                                            confirmButtonText: "Ok, got it!",
                                            customClass: {
                                                confirmButton: "btn fw-bold btn-primary"
                                            }
                                        })
                                    }
                                });
                            }
                        }))
                    }))
                }))
        };
        return {
            init: function () {
                (t = document.querySelector("#kt_ecommerce_return_table")) && ((e = $(t).DataTable({
                    //info: !1,
                    //order: [],
                    "bServerSide": true,
                    "ajax": {
                        "url": '/Order/GetReturnOrders',
                        "type": "post",
                        "data": function (data) {
                            return posDataTable.setDataTableFilters(data);
                        }
                    },
                    "columns": [
                        { data: "id", "sortable": false },
                        { data: "saleOrderId", "sortable": false },
                        { data: "customerName", "sortable": false },
                        { data: "total", "sortable": false, "className": "text-end pe-0" },
                        { data: "orderDate", "sortable": false, "className": "text-end" },
                        { data: "updatedDate", "sortable": false, "className": "text-end" }                        
                    ]
                })).on("draw", (function () {
                    n()
                })), document.querySelector('[data-kt-ecommerce-return-filter="search"]').addEventListener("keyup", (function (t) {
                    e.search(t.target.value).draw()
                })), n())
            },
        }
    }();
KTUtil.onDOMContentLoaded((function () {
    KTAppEcommerceReturn.init();
}));