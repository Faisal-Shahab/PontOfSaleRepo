let products = [];
let details = [];

function initEvents() {

    $("#invoiceSearch").keydown(function (event) {
        //event.preventDefault();
        if (event.keyCode == 13) {
            const valu = $(this).val();
            if (valu.length > 0) {
                event.preventDefault();
                getInvoice(valu);
            }
        }
    });

    $("#selectAll").change(function () {
        if ($(this).is(':checked')) {
            $('.returnCheckBox').prop('checked', true);
        } else {
            $('.returnCheckBox').prop('checked', false);
        }

        grandTotal();
    });    
}

function submitForm() {

    const customerId = ($("#customerId").val() || null);

    details = [];

    const returnOrder = { customerId: customerId, saleOrderId: $('#invoiceSearch').val(), paymentTypeId: $('input[name="paymentType"]:checked').val(), total: $('.grandTotal').last().text() };

    $('.returnCheckBox:checked').each(function () {
        const tr = $(this).closest('tr');

        const productId = $(tr).find('td:eq(0)').text();
        const productName = $(tr).find('td:eq(1)').text();
        const qty = $(tr).find('.qty').val();
        const price = $(tr).find('.price').val();
        const subtotal = $(tr).find('td:eq(4)').text();

        details.push({ productId: productId, productName: productName, salePrice: price, quantity: qty, subtotal: subtotal });
    });

    if (details.length > 0) {
        $.post('/Order/CreateReturnOrder', { returnOrder: returnOrder, details: details }, function (response) {
            if (response.status) {
                Swal.fire({
                    text: response.message,
                    icon: "success",
                    buttonsStyling: !1,
                    confirmButtonText: "Ok, got it!",
                    customClass: {
                        confirmButton: "btn fw-bold btn-primary"
                    }
                }).then((function () {
                    console.log(printerSize);
                    if (printerSize === "Thermal")
                        report(response.orderData);
                    else reportAFour(response.orderData);

                    $('#itemsList').html('');
                    $('.grandTotal').text('0.00');
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
    } else {
        Swal.fire({
            text: "Cart is empty",
            icon: "error",
            buttonsStyling: !1,
            confirmButtonText: "Ok, got it!",
            customClass: {
                confirmButton: "btn fw-bold btn-primary"
            }
        })
    }
}

function claculatPrecentage(ele) {
    const element = $(ele);
    const discount = parseFloat((element.val() || 0));
    const price = parseFloat(element.parent('td').prev().children('.price').val());

    const disountedPrice = (price - (((discount / 100) || 1) * price));
    const discEle = element.parent('td').next().children('.discPrice');
    discEle.val((disountedPrice == 0 ? price : disountedPrice));
    const qtyEletd = discEle.parent('td').next();
    qtyEletd.next().text((disountedPrice == 0 ? price : disountedPrice) * qtyEletd.children('.qty').val());

    grandTotal();
}

function claculatQty(ele) {
    const element = $(ele);
    const qty = element.val();
    const qtyTd = element.parent('td');
    const price = qtyTd.prev().children('.price').val();
    qtyTd.next().text((qty * price).toFixed(2));
    grandTotal();
}

function grandTotal() {
    let total = 0;
    $('.returnCheckBox:checked').each(function () {
        const tr = $(this).closest('tr');
        const subtotal = parseFloat($(tr).find('td:eq(4)').text());  
        total += subtotal;
    });
    $('.grandTotal').text(total);
}

function selectItem(ele) {
 //   const element = $(ele);

    grandTotal();
}

function report(data) {

    $('#invNum').text("Inv-" + data.invNumber)
    $('#orderTime').text(data.date)
    $('#qrCode').attr('src', data.qrCode);

    let _html = "";
    $('#tbl_data tbody').html(_html);
    let total = 0;
    $.each(details, function (i, val) {
        _html += `<tr><td>${val.productName}</td><td>${val.salePrice}</td><td>${val.quantity}</td><td>${val.subtotal}</td></tr>`;
        total += parseFloat(val.subtotal);
    });
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
    $.each(details, function (i, val) {
        _html += `<tr><td><div class="d-flex align-items-center"><div class="ms-5"><div class="fw-bolder">${val.productName}</div></div></div>
                        </td><td class="text-end">${val.salePrice}</td><td class="text-end">${val.quantity}</td><td class="text-end">${val.subtotal}</td></tr>`;
        total += parseFloat(val.subtotal);
    });
    $('#itemsBody').append(_html);
    $('#subTotal').text(total.toFixed(2))
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

function priceCalculat(ele) {
    const element = $(ele);
    const discEle = element.parent().next().children('.dicPrecent');
    claculatPrecentage(discEle);
}

function getInvoice(invoiceNumber) {

    clearForm();

    $.post('/Order/GetOrderDetails', { id: invoiceNumber }, function (response) {


        if (!response) {
            Swal.fire({
                text: "no record found",
                icon: "error",
                buttonsStyling: !1,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
            return false;
        }

        $('.grandTotal').text(response.total);
        $('#customerName').val(response.customerName);
        $('#customerId').val(response.customerId);
        let _html = "";

        $('#itemsList').html(_html);
        $.each(response.orderDetails, function (i, val) {
            _html += `<tr class="border-bottom border-bottom-dashed"><td style="display:none">${val.productId}</td>
                                         <td class="pe-7">${val.productName}</td>
                                         <td><input type="text" class="form-control text-end form-control-solid price" disabled value="${val.salePrice}" /></td>                                                                                  
                                         <td class="ps-0"><input type="text" onkeyup="claculatQty(this)" value="${val.quantity}" class="form-control form-control-solid qty"/></td>
                                         <td class="pt-8 text-end text-nowrap">${val.subTotal}</td>
                                         <td class="pt-5 text-end">
                                                 <span><input type="checkbox" class="returnCheckBox" onchange="selectItem(this)" /></span>
                                         </td></tr>`;
        });
        $('#itemsList').append(_html);

    });
}

function clearForm() {
    $('.grandTotal').text('0.00');
    $('#customerName').val('');
    $('#customerId').val('');
    $('#itemsList').html('');
}


$(document).ready(function () {
    initEvents();
});