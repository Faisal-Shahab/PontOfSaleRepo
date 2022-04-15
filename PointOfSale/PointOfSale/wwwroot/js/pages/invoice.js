﻿let products = [];
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
}

function submitForm() {

    const customerId = ($("#customerId").data("uiAutocomplete").selectedItem ?.id || 0);

    details = [];

    const saleOrder = { customerId: customerId, paymentTypeId: $('input[name="paymentType"]:checked').val(), total: $('.grandTotal').last().text() };

    $('#itemsList tbody tr').each(function () {

        const productId = $(this).find('td:eq(0)').text();
        const productName = $(this).find('td:eq(1)').text();
        const disountedPrice = $(this).find('.discPrice').val();
        const qty = $(this).find('.qty').val();
        const subtotal = $(this).find('td:eq(6)').text();

        details.push({ productId: productId, productName: productName, salePrice: disountedPrice, quantity: qty, subtotal: subtotal });
    });

    if (details.length > 0) {
        $.post('/Order/CreateSaleOrder', { saleOrder: saleOrder, details: details }, function (response) {
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
                    report(response.companyDetails);
                    $('#itemsList tbody').html('');
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
            const disountedPrice = (price - (((discount / 100) || 1) * price));

            const subtotal = ((disountedPrice === 0 ? price : disountedPrice) * qty);

            if (!isProductExist(productId)) {
                let _html = `<tr class="border-bottom border-bottom-dashed"><td style="display:none">${productId}</td>
                                         <td class="pe-7">${productName}</td>
                                         <td><input type="text" class="form-control text-end form-control-solid price" onkeyup="priceCalculat(this)" value="${price}" /></td>
                                         <td><input type="text" class="form-control form-control-solid dicPrecent" onkeyup="claculatPrecentage(this)" value="${discount}" /></td>
                                         <td><input type="text" value="${(disountedPrice == 0 ? price : disountedPrice)}" disabled class="form-control form-control-solid discPrice"/></td>
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
    const price = qtyTd.prev().children('.discPrice').val();
    qtyTd.next().text(qty * price);
    grandTotal();
}

function grandTotal() {
    let total = 0;
    $('#itemsList tbody tr').each(function () {
        const subtotal = parseFloat($(this).find('td:eq(6)').text());
        total += subtotal;
    });
    $('.grandTotal').text(total);
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
    $('#companyName').text(data.companyName)
    $('#regNum').text(data.crNumber)
    $('#taxNum').text(data.taxNumber)
    $('#invNum').text("Inv-" + data.invNumber)
    $('#orderTime').text(data.date)
    $('#thankNote').text(data.thankyouNote);

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

function printInvoice() {
    var myPrintContent = document.getElementById('invoice-POS');
    var myPrintWindow = window.open('', 'invoice-POS');
    myPrintWindow.document.write(myPrintContent.innerHTML);
    myPrintWindow.document.close();
    myPrintWindow.focus();
    myPrintWindow.print();
    myPrintWindow.close();
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