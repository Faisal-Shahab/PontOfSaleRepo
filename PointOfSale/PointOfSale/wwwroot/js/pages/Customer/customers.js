"use strict";
var KTAppEcommerceCustomers =
    function () {
        var t, e, n = () => {
            t.querySelectorAll('[data-kt-ecommerce-customer-filter="delete_row"]').
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
                                $.post('/customer/Delete', { id: o }, function (res) {
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
                (t = document.querySelector("#kt_ecommerce_customer_table")) && ((e = $(t).DataTable({
                    //info: !1,
                    //order: [],
                    "bServerSide": true,
                    "ajax": {
                        "url": '/customer/GetCustomers',
                        "type": "post",
                        "data": function (data) {
                            return posDataTable.setDataTableFilters(data);
                        }
                    },
                    "columns": [
                        { data: "customerId", "sortable": false },
                        { data: "name", "sortable": false },
                        { data: "email", "sortable": false },
                        { data: "contactNo", "sortable": false },
                        { data: "balance", "sortable": false, "className": "text-end", render: $.fn.dataTable.render.number(',', '.', 2) },
                        { data: "address", "sortable": false },
                        {
                            "data": null,
                            "className": "text-end",
                            "defaultContent": `<button class="details-control btn btn-sm"> </button>
<button class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm" onclick="KTAppEcommerceCustomers.getId(this,'payment')">
                                                                            <span class="svg-icon svg-icon-3">
																				<svg version="1.1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 510.327 510.327" xmlns:xlink="http://www.w3.org/1999/xlink" enable-background="new 0 0 510.327 510.327">
  <g>
    <path d="m418.85,249.359c-20.443-30.311-46.579-53.594-67.578-72.302-18.822-16.768-35.416-31.559-39.589-44.336 8.757-3.42 14.981-11.941 14.981-21.895 0-9.704-5.912-18.052-14.324-21.633l17.366-68.112c1.858-6.575 0.671-12.648-3.265-16.68-3.961-4.056-10.054-5.379-16.716-3.629-0.127,0.033-0.252,0.069-0.377,0.109-0.326,0.104-32.697,10.438-43.453,13.527-6.989,2.005-19.259,2.016-26.258,0.023-10.869-3.097-43.583-13.473-43.912-13.578-0.124-0.039-0.249-0.075-0.375-0.108-6.665-1.739-12.757-0.409-16.716,3.651-3.959,4.06-5.135,10.186-3.228,16.806 0.022,0.08 0.047,0.159 0.072,0.237l22.042,67.965c-8.161,3.688-13.857,11.9-13.857,21.422 0,9.954 6.224,18.476 14.981,21.895-4.174,12.776-20.768,27.567-39.589,44.335-21,18.708-47.135,41.991-67.578,72.302-24.099,35.731-35.814,74.155-35.814,117.468 0,39.061 15.695,74.899 44.193,100.914 30.52,27.86 73.159,42.586 123.307,42.586h64c50.148,0 92.787-14.726 123.307-42.586 28.499-26.015 44.194-61.854 44.194-100.914 0-43.311-11.715-81.736-35.814-117.467zm-147.824-130.032c-0.006,0-0.012,0-0.018,0h-31.686c-0.011,0-0.022,0-0.032,0h-32.126c-4.687,0-8.5-3.813-8.5-8.5 0-4.682 3.804-8.49 8.484-8.499 0.005,0 0.01,0.001 0.014,0.001 0.008,0 0.017-0.002 0.025-0.002h39.948c0.008,0 0.015,0.002 0.023,0.002 0.01,0 0.021-0.002 0.032-0.002h15.948c0.011,0 0.021,0.002 0.032,0.002 0.008,0 0.015-0.002 0.023-0.002h39.971c4.687,0 8.5,3.813 8.5,8.5s-3.813,8.5-8.5,8.5h-32.138zm-81.518-104.326c0.357-0.026 0.972-0.006 1.889,0.218 3.086,0.979 33.387,10.579 44.13,13.64 9.679,2.755 24.834,2.743 34.505-0.031 10.627-3.052 40.59-12.607 43.666-13.588 0.903-0.222 1.512-0.244 1.871-0.221 0.016,0.382-0.026,1.042-0.312,2.032-0.021,0.074-0.042,0.149-0.061,0.224l-17.861,70.053h-24.567l5.67-22.681c1.005-4.018-1.438-8.09-5.457-9.095-4.013-1.003-8.09,1.439-9.095,5.457l-6.58,26.319h-4.289l-6.58-26.319c-1.005-4.019-5.08-6.463-9.095-5.457-4.019,1.004-6.462,5.077-5.457,9.095l5.67,22.681h-24.942l-22.823-70.382c-0.263-0.945-0.299-1.577-0.282-1.945zm210.849,441.662c-27.709,25.294-66.851,38.664-113.194,38.664h-64c-46.342,0-85.484-13.37-113.193-38.664-25.347-23.138-39.307-55.042-39.307-89.836 0-90.935 56.855-141.586 98.37-178.57 22.38-19.938 40.627-36.198 44.751-53.93h14.973l-4.709,14.128c-1.31,3.93 0.814,8.177 4.744,9.487 0.787,0.262 1.586,0.387 2.373,0.387 3.14,0 6.066-1.988 7.114-5.13l6.291-18.872h21.188l6.291,18.872c1.048,3.143 3.974,5.13 7.114,5.13 0.786,0 1.586-0.125 2.373-0.387 3.93-1.31 6.053-5.557 4.744-9.487l-4.709-14.128h14.974c4.124,17.732 22.371,33.993 44.751,53.93 41.515,36.984 98.37,87.635 98.37,178.57-0.002,34.794-13.961,66.698-39.309,89.836z"/>
    <path d="m255.163,286.327c17.617,0 32.5,9.388 32.5,20.5 0,4.142 3.358,7.5 7.5,7.5s7.5-3.358 7.5-7.5c0-17.995-17.05-32.462-40-35.076v-8.924c0-4.142-3.358-7.5-7.5-7.5-4.142,0-7.5,3.358-7.5,7.5v8.924c-22.95,2.614-40,17.081-40,35.076 0,19.907 20.864,35.5 47.5,35.5 17.617,0 32.5,9.388 32.5,20.5s-14.883,20.5-32.5,20.5-32.5-9.388-32.5-20.5c0-4.142-3.358-7.5-7.5-7.5s-7.5,3.358-7.5,7.5c0,17.995 17.05,32.462 40,35.076v8.924c0,4.142 3.358,7.5 7.5,7.5 4.142,0 7.5-3.358 7.5-7.5v-8.924c22.95-2.614 40-17.081 40-35.076 0-19.907-20.864-35.5-47.5-35.5-17.617,0-32.5-9.388-32.5-20.5s14.883-20.5 32.5-20.5z"/>
    <path d="m255.163,215.327c-65.893,0-119.5,53.607-119.5,119.5s53.607,119.5 119.5,119.5 119.5-53.607 119.5-119.5-53.607-119.5-119.5-119.5zm0,224c-57.622,0-104.5-46.878-104.5-104.5s46.878-104.5 104.5-104.5 104.5,46.878 104.5,104.5-46.878,104.5-104.5,104.5z"/>
  </g>
</svg>
																			</span></button>
<button class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm" onclick="KTAppEcommerceCustomers.getId(this,'edit')">
                                                                            <span class="svg-icon svg-icon-3">
																				<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
																					<path opacity="0.3" d="M21.4 8.35303L19.241 10.511L13.485 4.755L15.643 2.59595C16.0248 2.21423 16.5426 1.99988 17.0825 1.99988C17.6224 1.99988 18.1402 2.21423 18.522 2.59595L21.4 5.474C21.7817 5.85581 21.9962 6.37355 21.9962 6.91345C21.9962 7.45335 21.7817 7.97122 21.4 8.35303ZM3.68699 21.932L9.88699 19.865L4.13099 14.109L2.06399 20.309C1.98815 20.5354 1.97703 20.7787 2.03189 21.0111C2.08674 21.2436 2.2054 21.4561 2.37449 21.6248C2.54359 21.7934 2.75641 21.9115 2.989 21.9658C3.22158 22.0201 3.4647 22.0084 3.69099 21.932H3.68699Z" fill="currentColor"></path>
																					<path d="M5.574 21.3L3.692 21.928C3.46591 22.0032 3.22334 22.0141 2.99144 21.9594C2.75954 21.9046 2.54744 21.7864 2.3789 21.6179C2.21036 21.4495 2.09202 21.2375 2.03711 21.0056C1.9822 20.7737 1.99289 20.5312 2.06799 20.3051L2.696 18.422L5.574 21.3ZM4.13499 14.105L9.891 19.861L19.245 10.507L13.489 4.75098L4.13499 14.105Z" fill="currentColor"></path>
																				</svg>
																			</span></button> 
                        <button class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm" data-kt-ecommerce-customer-filter="delete_row">
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
                })), document.querySelector('[data-kt-ecommerce-customer-filter="search"]').addEventListener("keyup", (function (t) {
                    e.search(t.target.value).draw()
                })), n())


                $('#kt_ecommerce_customer_table').off().on("click", "button.details-control", function () {

                    var tr = $(this).closest("tr");
                    var row = e.row(tr);

                    if (row.child.isShown()) {
                        row.child.remove();
                        tr.removeClass("shown");
                        $('#in' + row.data().customerId).DataTable().destroy();
                    }
                    else {
                        row.child(formatTable(row.data())).show();
                        var id = row.data().customerId;
                        var childTable = $('#in' + id).DataTable({
                            "bServerSide": false,
                            ajax: {
                                "url": '/Customer/GetCustTransactions',
                                "type": "POST",
                                "data": {
                                    id: id,
                                }
                            },
                            columns: [
                                { data: "debit", "sortable": false, "className": "text-end", render: $.fn.dataTable.render.number(',', '.', 2) },
                                { data: "credit", "sortable": false, "className": "text-end", render: $.fn.dataTable.render.number(',', '.', 2) },
                                { data: "balance", "sortable": false, "className": "text-end", render: $.fn.dataTable.render.number(',', '.', 2) },
                                { data: "createdAt", "sortable": false },
                            ],
                            "destroy": true,
                            select: false,
                            "bProcessing": false,
                            "filter": false, //set to false to Disable filter (search box)  
                        });
                        tr.addClass('shown');
                    }
                });

            },
            getId: function (ele, button) {
                const tr = $(ele).closest('tr');
                const id = tr.find('td:eq(0)').text();
                if (button === 'edit')
                    window.location.href = '/customer/Edit/' + id;
                else
                    window.location.href = '/customer/customerPayment/' + id;
            }
        }
    }();

function formatTable(rowData) {
    var childTable = '<table id="in' + rowData.customerId + '" class="table table-striped table-bordered responsive no-wrap" width="100%">' +
        '<thead><th>Debit</th><th>Credit</th><th>Balance</th><th>Date</th>' +
        '</thead> ' +
        '</table>';
    return $(childTable).toArray();
}

KTUtil.onDOMContentLoaded((function () {
    KTAppEcommerceCustomers.init();
}));