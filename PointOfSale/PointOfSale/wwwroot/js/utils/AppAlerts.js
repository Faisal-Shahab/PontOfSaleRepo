var AppAlerts = {

    success: function (title, msg, html, autoHide) {

        swal({
            title: title,
            text: msg,
            timer: 2000,
            type: "success",
            showConfirmButton: false
        });

    },

    error: function (title, msg, html, autoHide) {

        swal({
            title: title,
            text: msg,
            timer: 2000,
            type: "error",
            showConfirmButton: false
        });

    },

    successConfirm: function (title, msg, confirmButtonText, callback) {

        swal({
            title: title,
            text: msg,
            type: "success",
            showCancelButton: false,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: confirmButtonText || "Ok",
            closeOnConfirm: false
        },
            function () {

                callback();
            });

    },
    inlineAlert: function (msg, type, parent, toggleAlert = true, timeout = 5000) {

        var $container = $('<div/>').addClass("alert").addClass('alert-bold').addClass('alert-solid-' + type);

        $container.append($('<button/>').addClass('close').attr('data-close', 'alert'));

        $container.append($('<span/>').addClass("alert-text").html(msg));

        $(parent).find('.alert').remove();

        $(parent).html($container);

        if (toggleAlert) {
            setTimeout(function () {
                $(parent).find('.alert').slideUp(200);
            }, timeout);
        }
    },
    // boostrap alert message
    showInlineAlert: function (msg, type, parent) {

        //var $label = $('<label/>').addClass('alert').addClass('alert-' + type).css({ "width": "100%" });

        var alert = `<div class="alert alert-${type} fade in alert-dismissible show" style="margin-top:18px;">
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true" style="font-size:20px">×</span>
            </button>
            <span id="alert-text">${msg}</span>
        </div>`
       // $label.text(msg);

        $(parent).prepend(alert);
    },
    // toaster notification message
    actionAlert: function (msg) {

        toastr.options = {
            "debug": false,
            "positionClass": "toast-top-right",
            "onclick": null,
            "fadeIn": 300,
            "fadeOut": 1000,
            "timeOut": 5000,
            "extendedTimeOut": 1000
        };


        if (msg.IsError) {
            toastr.error(msg.Message, msg.Title);
        } else {

            toastr.success(msg.Message, msg.Title);
        }
    },
    confirm: function (callback) {

        swal({
            title: "Are you sure?",
            text: "You will not be able to recover this record!",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: true,
            allowOutsideClick: false
        }, callback);

    },
    //confirm popup will show then use then() function to implement your logic for ok and cancel button 
    //i.e confirm().then((isTrue) => { if(isTrue) { alert(true) } else { alert(false); }});
    confirm: function (text) {
        return swal({
            title: "Are you sure?",
            text: text,
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Yes !",
            closeOnConfirm: true,
            allowOutsideClick: false,
            buttons: true,
            dangerMode: true,
        });
    }

};