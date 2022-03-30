
function initTable() {
    $('#companiesDataTable').DataTable({
        "bServerSide": false,
        "bSortCellsTop": true,
        "ajax": {
            "url": '/Company/GetCompanies',
            "type": "post",
        },
        "bProcessing": false,
        "filter": true, //set to false to Disable filter (search box)            
        "columns": [
            { data: "companyId", "sortable": false },
            { data: "name", "sortable": false },
            {
                "data": null,
                "defaultContent": "<button class=\"btn btn-default btn-sm\" onclick=\"getId(this,'edit')\"> <span class=\"flaticon-edit\"></span> </button> <button class=\"btn btn-default btn-sm\" onclick=\"getId(this,'delete')\"><span class=\"flaticon2-trash\"></span></button>"
            }
        ],
    });
}

$('#add-new').on('click', function () {
    $('#kt_modal_4_2').modal('show');
    $(".modal-body").load("/Company/Create", function () {
        initEvents();
    });
    $('.modal-title').text("Create Company");
});


var closeModal = function () {
    $('#kt_modal_4_2').modal('hide');
    $(".modal-body").html("");
}

function getId(event, buttonName) {
    const tr = $(event).closest("tr");
    const id = $(tr).find('td:eq(0)').text();
    if (buttonName == 'edit') {
        $('#kt_modal_4_2').modal('show');
        $(".modal-body").load('/Company/Edit?id=' + id, function () {
            initEvents();
        });
        $('.modal-title').text("Company Details");
    } else if (buttonName == 'delete') {
        var _delete = confirm("Are you sure you want to delete?");
        if (_delete) {
            $.ajax({
                url: '/company/disable',
                typeof: 'JSON',
                type: 'post',
                data: { data: id },
                success: function (data) {
                    if (data) {
                        $("#companiesDataTable").DataTable().row($(tr)).remove().draw();
                        var successMsg = { IsError: false, Message: "Record has been successfully deleted.", Title: "success" };
                        AppAlerts.actionAlert(successMsg);
                    } else {
                        var errorMsg = { IsError: true, Message: "Operation failed, please try again.", Title: "error" };
                        AppAlerts.actionAlert(errorMsg);
                    }
                }
            })
            return true;
        }
        else {
            return false;
        }
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
                    alert(data.message);
                    //const successMsg = { IsError: false, Message: data.message, Title: "success" }
                    //AppAlerts.actionAlert(successMsg);
                    //utils.resetForm('.kt-form');
                    $('#kt_modal_4_2').modal('hide');
                    $('#companiesDataTable').DataTable().ajax.reload();
                    closeModal();
                } else {
                    alert(data.message);
                    //const errorMsg = { IsError: true, Message: data.message, Title: "error" }
                    //AppAlerts.actionAlert(errorMsg);
                }
            }
        });
    });
}

jQuery(document).ready(function () {
    initTable();
    $.fn.dataTable.ext.errMode = 'none';
});