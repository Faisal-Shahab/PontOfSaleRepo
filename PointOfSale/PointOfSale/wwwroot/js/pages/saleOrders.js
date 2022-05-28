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
                let fromDate = moment().subtract(1, 'days').format("YYYY-MM-DD");
                let toDate = moment().format("YYYY-MM-DD");

                (t = document.querySelector("#kt_ecommerce_sales_table")) && ((e = $(t).DataTable({
                    //info: !1,
                    //order: [],
                    "bServerSide": true,
                    "ajax": {
                        "url": '/Order/GetSaleOrders',
                        "type": "post",
                        "data": function (data) {
                            let params = posDataTable.setDataTableFilters(data);
                            params.fromDate = fromDate;
                            params.toDate = toDate;
                            return params;
                        }
                    },
                    "columns": [
                        { data: "orderId", "sortable": false },
                        { data: "customerName", "sortable": false },
                        { data: "total", "sortable": false, "className": "text-end pe-0" },
                        { data: "orderDate", "sortable": false, "className": "text-end" },
                        { data: "updatedDate", "sortable": false, "className": "text-end" },
                        {
                            "data": null,
                            "className": "text-end",
                            "defaultContent": `<button class="details-control btn btn-sm" > </button>
                                                <button class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm" onclick="getId(this)" >
                                                <span class="svg-icon svg-icon-2">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
                                                <path d="M22 7H2V11H22V7Z" fill="currentColor"></path>
                                                <path opacity="0.3" d="M21 19H3C2.4 19 2 18.6 2 18V6C2 5.4 2.4 5 3 5H21C21.6 5 22 5.4 22 6V18C22 18.6 21.6 19 21 19ZM14 14C14 13.4 13.6 13 13 13H5C4.4 13 4 13.4 4 14C4 14.6 4.4 15 5 15H13C13.6 15 14 14.6 14 14ZM16 15.5C16 16.3 16.7 17 17.5 17H18.5C19.3 17 20 16.3 20 15.5C20 14.7 19.3 14 18.5 14H17.5C16.7 14 16 14.7 16 15.5Z" fill="currentColor"></path>
                                                </svg></span></button>`
                        }
                    ]
                })).on("draw", (function () {
                    n()
                })), document.querySelector('[data-kt-ecommerce-order-filter="search"]').addEventListener("keyup", (function (t) {
                    e.search(t.target.value).draw()
                })), n())


                $('#kt_ecommerce_sales_table').off().on("click", "button.details-control", function () {

                    var tr = $(this).closest("tr");
                    var row = e.row(tr); 

                    if (row.child.isShown()) {
                        row.child.remove();
                        tr.removeClass("shown");
                        $('#in' + row.data().orderId).DataTable().destroy();
                    }
                    else {
                        row.child(formatTable(row.data())).show();
                        var id = row.data().orderId;
                        var childTable = $('#in' + id).DataTable({
                            "bServerSide": false,
                            ajax: {
                                "url": '/Order/GetSaleOrderDetails',
                                "type": "POST",
                                "data": {
                                    id: id,
                                }
                            },
                            columns: [
                                { data: "productName", "sortable": false },
                                { data: "costPrice", "sortable": false, "className": "text-right", render: $.fn.dataTable.render.number(',', '.', 2) },
                                { data: "salePrice", "sortable": false, "className": "text-right", render: $.fn.dataTable.render.number(',', '.', 2) },
                                { data: "discount", "sortable": false, "className": "text-right" },
                                { data: "quantity", "sortable": false, "className": "text-right", render: $.fn.dataTable.render.number(',', '.', 0) },
                                { data: "subTotal", "sortable": false, "className": "text-right", render: $.fn.dataTable.render.number(',', '.', 2) },
                            ],
                            "destroy": true,
                            select: false,
                            "bProcessing": false,
                            "filter": false, //set to false to Disable filter (search box)  
                        });
                        tr.addClass('shown');
                    }
                });

                const start = moment();
                const end = moment();

                $("#kt_ecommerce_sales_flatpickr").daterangepicker({
                    showDropdowns: true,
                    minYear: 2022,
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
                    $("#kt_ecommerce_sales_table").DataTable().draw();
                });

                $("#exportButton").click(function () {
                    const searchTerm = $('[data-kt-ecommerce-order-filter="search"]').val();
                    const url = `/Order/ExportSaleOrders?fromDate=${fromDate}&toDate=${toDate}&searchTerm=${searchTerm}`;

                    window.location.href = url;

                });
            },
        }
    }();
function formatTable(rowData) {
    var childTable = '<table id="in' + rowData.orderId + '" class="table table-striped table-bordered responsive no-wrap" width="100%">' +
        '<thead><th>Product Name</th><th>Cost Price</th><th>Sale Price</th><th>Disc</th><th>Qty</th><th>Total</th>' +
        '</thead> ' +
        '</table>';
    return $(childTable).toArray();
}

function getId(ele) {
    const tr = $(ele).closest('tr');
    const id = tr.find('td:eq(0)').text();
    $.post('/order/PrintSaleOrderDetails', { id: id }, function (response) {
        if (printerSize === "Thermal") {
            report(response);
        } else {
            reportAFour(response);
        }
    });
}

function report(data) {

    $('#orderId').text("#" + data.orderId)

    $('#invNum').text("Inv-" + data.invNumber)
    $('#orderTime').text(data.date)
    $('#qrCode').attr('src', data.qrCode);

    let _html = "";
    $('#tbl_data tbody').html(_html);
    let total = 0;
    $.each(data.details, function (i, val) {
        _html += `<tr><td>${val.productName}</td><td>${val.salePrice}</td><td>${val.quantity}</td><td>${val.subtotal}</td></tr>`;
        total += parseFloat(val.subtotal);
    });
    $('#totalDisc').text($('.totalDisc').text())
    $('#totalPay').text(total.toFixed(2))
    $('#tbl_data tbody').append(_html);

    printInvoice();
}

function reportAFour(data) {

    $('#orderId').text("#" + data.orderId)
    $('#invNum').text("#Inv-" + data.invNumber)
    $('#invDate').text(data.date)
    $('#qrCode').attr('src', data.qrCode);

    let _html = "";
    $('#itemsBody').html(_html);
    let total = 0;
    $.each(data.details, function (i, val) {
        _html += `<tr><td><div class="d-flex align-items-center"><div class="ms-5"><div class="fw-bolder">${val.productName}</div></div></div>
                        </td><td class="text-end">${val.salePrice}</td><td class="text-end">${val.quantity}</td><td class="text-end">${val.subtotal}</td></tr>`;
        total += parseFloat(val.subtotal);
    });
    $('#itemsBody').append(_html);
    $('#totalDisc').text($('.totalDisc').text())
    $('#grandTotal').text(total.toFixed(2))


    printInvoice();
}

function printInvoice() {
    const _head = printerSize === "Thermal" ? "" : '<head><link href="https://localhost:44374/assets/css/style.bundle.css" rel="stylesheet" type="text/css"></head>';
    var myPrintContent = document.getElementById('invoice-POS');
    var myPrintWindow = window.open('', 'invoice-POS');
    myPrintWindow.document.write(`<html><title>Print</title>${_head}<body>`);
    myPrintWindow.document.write(myPrintContent.innerHTML);
    myPrintWindow.document.write('</body></html>');
    myPrintWindow.document.close();

    setTimeout(function () {
        myPrintWindow.focus();
        myPrintWindow.print();
        myPrintWindow.close();
    }, 100);
    return false;
}

KTUtil.onDOMContentLoaded((function () {
    KTAppEcommerceCategories.init();
}));