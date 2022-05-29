let products = [];
let details = [];

function initEvents() {
    $("#customerId").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Order/GetCustomers",
                dataType: "json",
                data: { value: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            id: item.value,
                            value: item.text
                        };
                    }));
                    //   customers = data;
                }
            });
        },
        minLength: 3,
        select: function (event, ui) {
            //console.log("Selected: " + ui.item.value + " aka " + ui.item.id);
            customerId = ui.item.id;
        }
    });

    $("#productId").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Order/GetProducts",
                dataType: "json",
                data: { value: request.term },
                success: function (data) {

                    response($.map(data, function (item) {
                        return {
                            id: item.id,
                            value: item.name
                        };
                    }));

                    products = data;
                }
            });
        },
        minLength: 3,
        select: function (event, ui) {
            // console.log("Selected: " + ui.item.value + " aka " + ui.item.id);          
            addItemToList(ui.item.id, ui.item.value);
            this.value = "";
            return false;
        }
    });

    $("#productId").keydown(function (event) {
        if (event.keyCode == 13) {
            if ($(this).val().length > 0) {
                event.preventDefault();
                addItemToList();
                $(this).val("");
            }
        }
    });

    $("#amountPaid").keyup(function () {
        const paidEle = $(this);
        const total = parseFloat($('.grandTotal').text());
        const amountPaid = parseFloat(paidEle.val());

        if (amountPaid > total) {
            paidEle.val(total);
            $('#remainingAmount').text(0.00)
            return false;
        }
        $('#remainingAmount').text((total - amountPaid).toFixed(2))

    });
}

function submitForm() {

    const customerId = ($("#customerId").data("uiAutocomplete").selectedItem ?.id || null);

    details = [];
    const grandTotal = $('.grandTotal').last().text();
    const amountPaid = $('#amountPaid').val();
    const remainingAmount = $('#remainingAmount').text();
    const saleOrder = { customerId: customerId, paymentTypeId: $('input[name="paymentType"]:checked').val(), discount: $('.totalDisc').text(), total: grandTotal, amountPaid: amountPaid, remainingAmount: remainingAmount };

    $('#itemsList tbody tr').each(function () {

        const productId = $(this).find('td:eq(0)').text();
        const productName = $(this).find('td:eq(1)').text();
        const disountedPrice = $(this).find('.discPrice').val();
        const qty = $(this).find('.qty').val();
        const subtotal = $(this).find('td:eq(6)').text();
        const _discount = $(this).find('.dicPrecent').val();

        details.push({ productId: productId, productName: productName, salePrice: disountedPrice, quantity: qty, discount: _discount, subtotal: subtotal });
    });

    if (details.length > 0) {
        $.post('/Order/CreateSaleOrder', { model: saleOrder, details: details }, function (response) {
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
                    if (printerSize === "Thermal") {
                        report(response.orderData);
                    } else {
                        reportAFour(response.orderData);
                    }
                    $('#itemsList tbody').html('');
                    $('.grandTotal').text('0.00');
                    $('.totalDisc').text('0.00');
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
            text: "please add some items to list",
            icon: "error",
            buttonsStyling: !1,
            confirmButtonText: "Ok, got it!",
            customClass: {
                confirmButton: "btn fw-bold btn-primary"
            }
        })
    }
}

function addItemToList(prodId = 0, prodName = "") {

    try {
        const productId = ($("#productId").data("uiAutocomplete").selectedItem ?.id || prodId);
        const productName = ($("#productId").data("uiAutocomplete").selectedItem ?.value || prodName);

        const data = products.filter(x => x.id == productId)[0];

        if (data) {
            const discount = data.discount;
            const price = data.price;
            const qty = 1;
            const discountedPrice = parseFloat((price - (((discount / 100) || 1) * price)).toFixed(2));

            const subtotal = ((discount == 0 ? price : discountedPrice) * qty);

            if (!isProductExist(productId)) {
                let _html = `<tr class="border-bottom border-bottom-dashed"><td style="display:none">${productId}</td>
                                         <td class="pe-7">${productName}</td>
                                         <td><input type="text" class="form-control text-end form-control-solid price" onkeyup="priceCalculat(this)" value="${price}" /></td>
                                         <td><input type="text" class="form-control form-control-solid dicPrecent" onkeyup="claculatPrecentage(this)" value="${discount}" /></td>
                                         <td><input type="text" value="${(discount == 0 ? price : discountedPrice)}" disabled class="form-control form-control-solid discPrice"/></td>
                                         <td class="ps-0"><input type="text" onkeyup="claculatQty(this)" value="${qty}" class="form-control form-control-solid qty"/></td>
                                         <td class="pt-8 text-end text-nowrap">${subtotal}</td>
                                         <td class="pt-5 text-end">
                                                 <button type="button" class="btn btn-sm btn-icon btn-active-color-primary" onclick="removeItem(this)">
                                                     <!--begin::Svg Icon | path: icons/duotune/general/gen027.svg-->
                                                     <span class="svg-icon svg-icon-3">
                                                         <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
                                                             <path d="M5 9C5 8.44772 5.44772 8 6 8H18C18.5523 8 19 8.44772 19 9V18C19 19.6569 17.6569 21 16 21H8C6.34315 21 5 19.6569 5 18V9Z" fill="currentColor" />
                                                             <path opacity="0.5" d="M5 5C5 4.44772 5.44772 4 6 4H18C18.5523 4 19 4.44772 19 5V5C19 5.55228 18.5523 6 18 6H6C5.44772 6 5 5.55228 5 5V5Z" fill="currentColor" />
                                                             <path opacity="0.5" d="M9 4C9 3.44772 9.44772 3 10 3H14C14.5523 3 15 3.44772 15 4V4H9V4Z" fill="currentColor" />
                                                         </svg>
                                                     </span>
                                                     <!--end::Svg Icon-->
                                                 </button>
                                         </td></tr>`;
                $('#itemsList tbody').prepend(_html);
            }

            grandTotal();
        }
    }
    catch (p) {
        console.log(p);
    }
}

function claculatPrecentage(ele) {
    const element = $(ele);
    const discEle = element.parent('td').next().children('.discPrice');
    let discount = parseFloat((element.val() || 0));
    const price = parseFloat(element.parent('td').prev().children('.price').val());

    const discountedPrice = parseFloat((price - (((discount / 100) || 1) * price)).toFixed(2));

    discEle.val(discount == 0 ? price : discountedPrice);

    const qtyEletd = discEle.parent('td').next();
    qtyEletd.next().text((discount == 0 ? price : discountedPrice) * qtyEletd.children('.qty').val());

    grandTotal();
}

function claculatQty(ele) {
    const element = $(ele);
    const qty = element.val();
    const qtyTd = element.parent('td');
    const price = qtyTd.prev().children('.discPrice').val();
    qtyTd.next().text((qty * price).toFixed(2));
    grandTotal();
}

function grandTotal() {
    let total = 0;
    let totalDisc = 0;
    $('#itemsList tbody tr').each(function () {
        const subtotal = parseFloat($(this).find('td:eq(6)').text());
        const price = parseFloat($(this).find('.price').val());
        const discPrice = parseFloat($(this).find('.discPrice').val());
        total += subtotal;
        totalDisc += parseFloat((price - discPrice).toFixed(2));
    });
    $('.totalDisc').text(totalDisc);
    $('.grandTotal').text(total);
    $('.grandTotal').text(total);

    const amountPaid = parseFloat($('#amountPaid').val());

    $('#remainingAmount').text((total - amountPaid).toFixed(2))
}

function isProductExist(productId) {
    isExist = false;


    $('#itemsList tbody tr').each(function () {
        const id = parseInt($(this).find('td:eq(0)').text());
        if (id == productId) {
            const qtyEle = $(this).find('.qty');
            qtyEle.val(parseInt(qtyEle.val()) + 1);

            const disPrice = $(this).find('.discPrice').val();

            $(this).find('td:eq(6)').text(parseInt(qtyEle.val()) * disPrice);
            isExist = true;
        }
    });
    return isExist;
}

function removeItem(ele) {
    const element = $(ele);
    element.closest('tr').remove();
    grandTotal();
}

function report(data) {

    $('#orderId').text("#" + data.orderId)

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
    $.each(details, function (i, val) {
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

function priceCalculat(ele) {
    const element = $(ele);
    const discEle = element.parent().next().children('.dicPrecent');
    claculatPrecentage(discEle);
}

$(document).ready(function () {
    initEvents();
});