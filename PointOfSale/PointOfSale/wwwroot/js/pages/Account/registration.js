
function initTable() {
    $('#accountsDataTable').DataTable({
        "bServerSide": true,
        "bSortCellsTop": true,
        "ajax": {
            "url": '/account/getaccounts',
            "type": "post",
            "data": function (data) {
                return posDataTable.setDataTableFilters(data);
            }
        },
        "bProcessing": false,
        "filter": true, //set to false to Disable filter (search box)            
        "columns": [
            { data: "userName", "sortable": false },
            { data: "phoneNumber", "sortable": false },
            { data: "email", "sortable": false },          
            { data: "roleName", "sortable": false },          
            {
                "sortable": false,
                "mRender": function (data, type, row) {
                    return '<label class="kt-checkbox kt-checkbox--brand"><input type="checkbox" class="is-active"' + (row.isActive ? ' checked' : '') + '/><span></span></label>';
                }
            },        
            {
                "data": null,
                "defaultContent": "<button class=\"btn btn-default btn-sm\" onclick=\"getId(this,'edit')\"> <span class=\"flaticon-edit\"></span> </button> <button class=\"btn btn-default btn-sm\" onclick=\"getId(this,'delete')\"><span class=\"flaticon2-trash\"></span></button>"
            }
        ],
    });
}

$('#add-new').on('click', function () {
    $('#kt_modal_4_2').modal('show');
    $(".modal-body").load("/Account/Registration", function () {
        initEvents();
    });
    $('.modal-title').text("Account Account");
});


var closeModal = function () {
    $('#kt_modal_4_2').modal('hide');
    $(".modal-body").html("");
}


function status() { 
    $("#accountsDataTable").find('tbody').off().on('change', 'input[type="checkbox"]', function () {
        var _class = $(this).attr("class");
        var tr = $(this).closest("tr");
        var data = $('#accountsDataTable').DataTable().row(tr).data();

        if (this.checked) {
            if (_class == 'is-active') { data.isActive = true; }
        } else {
            if (_class == 'is-active') { data.isActive = false; }
        }
        postStatus(data);
    });
}

function postStatus(data) {

    $.post('/Account/ActiveInActiveUser', { userName: data.userName, status: data.isActive }, function (response) {
        if (response.status) {
            alert(response.message)
            const successMsg = { IsError: false, Message: response.message, Title: "success" }
            //AppAlerts.actionAlert(successMsg);
        } else {
            const errorMsg = { IsError: true, Message: response.message, Title: "error" }
           // AppAlerts.actionAlert(errorMsg);
        }
    });

}

function getId(event, buttonName) {
    const tr = $(event).closest("tr");
    const id = $(tr).find('td:eq(0)').text();
    if (buttonName == 'edit') {
        $('#kt_modal_4_2').modal('show');
        $(".modal-body").load('/Account/EditRegistration?userName=' + id, function () {
            initEvents();
        });

        $('.modal-title').text("Account Details");

    } else if (buttonName == 'delete') {
        var _delete = confirm("Are you sure you want to delete?");
        if (_delete) {
            $.ajax({
                url: '/Account/Delete',
                typeof: 'JSON',
                type: 'post',
                data: { id: id },
                success: function (data) {
                    if (data) {
                        $("#accountsDataTable").DataTable().row($(tr)).remove().draw();
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
   
    $('#accountForm').submit(function (event) {
        event.preventDefault(); //prevent default action 
        const formData = $(this).serialize();
        const post_url = $(this).attr("action");


        $.ajax({
            url: post_url,
            type: 'post',
            data: formData,
            dataType: 'json',
            success: function (data) {
                if (data.status) {
                    alert(data.message);
                    //const successMsg = { IsError: false, Message: data.message, Title: "success" }
                    //AppAlerts.actionAlert(successMsg);
                    //utils.resetForm('.kt-form');
                    $('#kt_modal_4_2').modal('hide');
                    $('#accountsDataTable').DataTable().ajax.reload();
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
    status();
    $.fn.dataTable.ext.errMode = 'none';
});