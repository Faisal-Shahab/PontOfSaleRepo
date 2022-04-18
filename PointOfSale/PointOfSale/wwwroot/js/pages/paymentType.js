
function initTable() {
    const dataTable = $('#paymentTypeDataTable').DataTable({
        "bServerSide": false,
        "bSortCellsTop": true,
        "ajax": {
            "url": '/PaymentType/GetPaymentTypes',
            "type": "post"
        },
        "bProcessing": false,
        "filter": true, //set to false to Disable filter (search box)            
        "columns": [
            { data: "paymentId", "sortable": false },
            { data: "name", "sortable": false },
            {
                "data": null,
                "defaultContent": `<button class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm" onclick="getId(this,'edit')">
                                                                            <span class="svg-icon svg-icon-3">
																				<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
																					<path opacity="0.3" d="M21.4 8.35303L19.241 10.511L13.485 4.755L15.643 2.59595C16.0248 2.21423 16.5426 1.99988 17.0825 1.99988C17.6224 1.99988 18.1402 2.21423 18.522 2.59595L21.4 5.474C21.7817 5.85581 21.9962 6.37355 21.9962 6.91345C21.9962 7.45335 21.7817 7.97122 21.4 8.35303ZM3.68699 21.932L9.88699 19.865L4.13099 14.109L2.06399 20.309C1.98815 20.5354 1.97703 20.7787 2.03189 21.0111C2.08674 21.2436 2.2054 21.4561 2.37449 21.6248C2.54359 21.7934 2.75641 21.9115 2.989 21.9658C3.22158 22.0201 3.4647 22.0084 3.69099 21.932H3.68699Z" fill="currentColor"></path>
																					<path d="M5.574 21.3L3.692 21.928C3.46591 22.0032 3.22334 22.0141 2.99144 21.9594C2.75954 21.9046 2.54744 21.7864 2.3789 21.6179C2.21036 21.4495 2.09202 21.2375 2.03711 21.0056C1.9822 20.7737 1.99289 20.5312 2.06799 20.3051L2.696 18.422L5.574 21.3ZM4.13499 14.105L9.891 19.861L19.245 10.507L13.489 4.75098L4.13499 14.105Z" fill="currentColor"></path>
																				</svg>
																			</span></button> 
                        <button class="btn btn-icon btn-bg-light btn-active-color-primary btn-sm" onclick=\"getId(this,'delete')\">
                        <span class="svg-icon svg-icon-3">
																				<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
																					<path d="M5 9C5 8.44772 5.44772 8 6 8H18C18.5523 8 19 8.44772 19 9V18C19 19.6569 17.6569 21 16 21H8C6.34315 21 5 19.6569 5 18V9Z" fill="currentColor"></path>
																					<path opacity="0.5" d="M5 5C5 4.44772 5.44772 4 6 4H18C18.5523 4 19 4.44772 19 5V5C19 5.55228 18.5523 6 18 6H6C5.44772 6 5 5.55228 5 5V5Z" fill="currentColor"></path>
																					<path opacity="0.5" d="M9 4C9 3.44772 9.44772 3 10 3H14C14.5523 3 15 3.44772 15 4V4H9V4Z" fill="currentColor"></path>
																				</svg>
																			</span>
                                                            </button>`
            }
        ],
    });

    $('[data-kt-user-table-filter="search"]').keyup(function () {
        dataTable.search($(this).val()).draw();
    });
}

$('#add-new').on('click', function () {
    $('#kt_modal_add_user').modal('show');
    $(".modal-body").load("/PaymentType/Create", function () {
        initEvents();

        $('#kt_modal_add_user_header h2').text("Create Payment Type");
    });
});


var closeModal = function () {
    $('#kt_modal_add_user').modal('hide');
    $(".modal-body").html("");
}

function getId(event, buttonName) {
    const tr = $(event).closest("tr");
    const id = $(tr).find('td:eq(0)').text();
    if (buttonName == 'edit') {
        $('#kt_modal_add_user').modal('show');
        $(".modal-body").load('/PaymentType/Edit?id=' + id, function () {
            initEvents();
        });
        $('#kt_modal_add_user_header h2').text("Payment Type Details");
    } else if (buttonName == 'delete') {
        Swal.fire({
            text: "Are you sure you want to delete " + id + "?",
            icon: "warning",
            showCancelButton: !0,
            buttonsStyling: !1,
            confirmButtonText: "Yes, delete!",
            cancelButtonText: "No, cancel",
            customClass: {
                confirmButton: "btn fw-bold btn-danger",
                cancelButton: "btn fw-bold btn-active-light-primary"
            }
        }).then((function (_delete) {
            if (_delete.value) {
                $.ajax({
                    url: '/PaymentType/Delete',
                    typeof: 'JSON',
                    type: 'post',
                    data: { id: id },
                    success: function (data) {
                        if (data) {
                            Swal.fire({
                                text: "Record has been deleted successfully",
                                icon: "success",
                                buttonsStyling: !1,
                                confirmButtonText: "Ok, got it!",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-primary"
                                }
                            }).then((function () {
                                $("#paymentTypeDataTable").DataTable().row($(tr)).remove().draw();
                            }))
                        } else {
                            Swal.fire({
                                text: "Operation failed, please try again.",
                                icon: "error",
                                buttonsStyling: !1,
                                confirmButtonText: "Ok, got it!",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-primary"
                                }
                            })
                        }
                    }
                })
                return true;
            }
            else {
                return false;
            }
        }));
    }
}
function initEvents() {
    $('#companyForm').submit(function (event) {
        event.preventDefault(); //prevent default action 
        // var form_data = $(this).serialize();
        const post_url = $(this).attr("action");
        const formData = new FormData(this);

        $.ajax({
            url: post_url,
            type: 'post',
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.status) {
                    Swal.fire({
                        text: data.message,
                        icon: "success",
                        buttonsStyling: !1,
                        confirmButtonText: "Ok, got it!",
                        customClass: {
                            confirmButton: "btn fw-bold btn-primary"
                        }
                    }).then((function () {
                        $('#kt_modal_add_user').modal('hide');
                        $('#paymentTypeDataTable').DataTable().ajax.reload();
                        closeModal();
                    }))

                } else {
                    Swal.fire({
                        text: data.message,
                        icon: "error",
                        buttonsStyling: !1,
                        confirmButtonText: "Ok, got it!",
                        customClass: {
                            confirmButton: "btn fw-bold btn-primary"
                        }
                    })
                }
            }
        });
    });
}

jQuery(document).ready(function () {
    initTable();
    $.fn.dataTable.ext.errMode = 'none';
})