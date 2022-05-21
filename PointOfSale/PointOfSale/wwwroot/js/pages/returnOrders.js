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
                let fromDate = moment().subtract(1, 'days').format("YYYY-MM-DD");
                let toDate = moment().format("YYYY-MM-DD");
                (t = document.querySelector("#kt_ecommerce_return_table")) && ((e = $(t).DataTable({
                    //info: !1,
                    //order: [],
                    "bServerSide": true,
                    "ajax": {
                        "url": '/Order/GetReturnOrders',
                        "type": "post",
                        "data": function (data) {
                            let params = posDataTable.setDataTableFilters(data);
                            params.fromDate = fromDate;
                            params.toDate = toDate;
                            return params;
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

                const start = moment();
                const end = moment();

                $("#kt_ecommerce_sales_flatpickr").daterangepicker({
                    showDropdowns: true,
                    minYear: 2021,
                    maxYear: parseInt(moment().format("YYYY"), 10),
                    start: start,
                    end: end,
                    locale: {
                        format: 'YYYY-MM-DD'
                    }
                }, function (start, end) {
                    fromDate = start.format("YYYY-MM-DD");
                    toDate = end.format("YYYY-MM-DD");
                    $("#kt_ecommerce_sales_flatpickr").val(fromDate + " - " + toDate);
                    $("#kt_ecommerce_return_table").DataTable().draw();
                });

                $("#exportButton").click(function () {
                    const searchTerm = $('[data-kt-ecommerce-return-filter="search"]').val();
                    const url = `/Order/ExportReturnOrders?fromDate=${fromDate}&toDate=${toDate}&searchTerm=${searchTerm}`;

                    window.location.href = url;

                });
            },
        }
    }();
KTUtil.onDOMContentLoaded((function () {
    KTAppEcommerceReturn.init();
}));