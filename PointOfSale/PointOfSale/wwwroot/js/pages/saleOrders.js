"use strict";
var KTAppEcommerceCategories =
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
                (t = document.querySelector("#kt_ecommerce_sales_table")) && ((e = $(t).DataTable({
                    //info: !1,
                    //order: [],
                    "bServerSide": true,
                    "ajax": {
                        "url": '/Order/GetSaleOrders',
                        "type": "post",
                        "data": function (data) {
                            return posDataTable.setDataTableFilters(data);
                        }
                    },
                    "columns": [
                        { data: "orderId", "sortable": false },
                        { data: "customerName", "sortable": false },
                        { data: "total", "sortable": false, "className":"text-end pe-0" },
                        { data: "orderDate", "sortable": false, "className": "text-end" },
                        { data: "updatedDate", "sortable": false, "className": "text-end"  },
                        {
                            "data": null,
                            "className": "text-end",
                            "defaultContent": `
                        <button class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm" data-kt-ecommerce-order-filter="delete_row">
                        <span class="svg-icon svg-icon-3">
																				<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
																					<path d="M5 9C5 8.44772 5.44772 8 6 8H18C18.5523 8 19 8.44772 19 9V18C19 19.6569 17.6569 21 16 21H8C6.34315 21 5 19.6569 5 18V9Z" fill="currentColor"></path>
																					<path opacity="0.5" d="M5 5C5 4.44772 5.44772 4 6 4H18C18.5523 4 19 4.44772 19 5V5C19 5.55228 18.5523 6 18 6H6C5.44772 6 5 5.55228 5 5V5Z" fill="currentColor"></path>
																					<path opacity="0.5" d="M9 4C9 3.44772 9.44772 3 10 3H14C14.5523 3 15 3.44772 15 4V4H9V4Z" fill="currentColor"></path>
																				</svg>
																			</span>
                                                            </button>`
                        }
                    ]
                })).on("draw", (function () {
                    n()
                })), document.querySelector('[data-kt-ecommerce-order-filter="search"]').addEventListener("keyup", (function (t) {
                    e.search(t.target.value).draw()
                })), n())
            }
        }
    }();
KTUtil.onDOMContentLoaded((function () {
    KTAppEcommerceCategories.init();
}));