var permissionsStore;
NiceOne.Utilities = {
    services: {
        controller: {
            DashboardController: "Dashboard"
        },
        actions: {
            permissions: "Permissions"
        }
    },
    getSiteBase: function () {

        var siteRoot = [
            window.location.protocol,
            "//",
            window.location.hostname
        ].join("");

        if (window.location.port !== "") {
            siteRoot += ":" + window.location.port;
        };

        return siteRoot;
    },

    getSiteRoot: function () {

        var siteRoot = [
            window.location.protocol,
            "//",
            window.location.hostname
        ].join("");

        if (window.location.port !== "") {
            siteRoot += ":" + window.location.port;
        };

        siteRoot += "/" + window.location.pathname.split("/").splice(1, 1);

        if (siteRoot.lastIndexOf("/") !== siteRoot.length - 1) {
            siteRoot += "/";
        };

        return siteRoot;
    },

    getQueryString: function (field, url) {

        field = field.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + field + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return "";
        return decodeURIComponent(results[2].replace(/\+/g, " "));

        //var query = $.url(url).attr("query");

        //if (query.indexOf("?") < 0) {
        //    query = "?" + query;
        //}

        //return $.url(query).param(field);
    },


    formMethods: {

        Post: "POST",
        Get: "GET"
    },

    postToController: function (url, formMethod, data, callback) {

        $.ajax({
            url: url,
            type: formMethod,
            data: data,
            success: function (response) {
                callback(response);
            },
            error: function (err, exception) {
                console.log("Failed to post to server " + err.responseText);
            }
        });

    },

    getFromAction: function (url, data, callback) {

        $.get(url, function (result) {
            callback(result);

        }, function (err) {

            console.log(err.responseBody);
        });
    },
    reintializeJqueryValidations: function (form) {
        $(form).removeData("validator");
        $(form).removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse(form);
    },
    getPermissions: function () {
        var url = [utils.getSiteRoot(), utils.services.controller.DashboardController, "/", utils.services.actions.permissions].join("");
        $.ajax({
            url: url,
            dataType: "json",
            async: false,
            data: {},
            success: function (data) {
                console.log(data);
                permissionsStore = data;
            }
        });
    },
    resetJqueryValidations: function ($form) {

        $form.find("[data-valmsg-summary=true]")
            .removeClass("validation-summary-errors")
            .addClass("validation-summary-valid")
            .find("ul").empty();

        //reset unobtrusive field level, if it exists
        $form.find("[data-valmsg-replace]")
            .removeClass("field-validation-error")
            .addClass("field-validation-valid")
            .empty();

        $form.find(":input").removeClass("text-danger");

        $(".has-error").addClass("has-success").removeClass("has-error");

    },
    resetForm: function (form, resetAllForms) {

        $("select").not(".dataTables_length select , #CultureCode ").val("").trigger("change");
        utils.reintializeJqueryValidations($(form));
        utils.resetJqueryValidations($(form));
        if (resetAllForms) {
            var forms = $(form).length;
            for (var i = 0; i < forms; i++) {
                $(form)[i].reset();
            }
        } else {
            $(form)[0].reset();
        }


    },
    urlParam: function (name) {
        var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
        if (results === null) {
            return null;
        }
        else {
            return decodeURI(results[1]) || 0;
        }
    },
    select2Ajax: function ($this, controller, action) {
       
        var url = [controller, "/", action].join("");


        $($this).select2({

            //minimumInputLength: 1,
            ajax: {
                placeholder: "Select",
                allowClear: true,
                url: url,
                dataType: "json",
                type: "POST",
                placeholder: "Select option",
                placeholderOption:"first",
                data: function (item) {
                    return {
                        term: item.term || 'a',
                        page: item.page || 1
                    };
                },
                processResults: function (data, params) {                   
                    params.page = params.page || 1;
                    var mappedData = $.map(data, function (obj) {                      
                        obj.text = obj.name;
                        obj.id = obj.id;
                        return obj;
                    });
                    return {
                        results: mappedData,
                        pagination: {
                            more: (params.page * 30) < data.TotalCount
                        }
                    };
                }

            }
        });
    },
    showAlertMessage: function (options) {
        var config = {
            response: options.response,
            parent: options.parent || '.portlet-body.form',
            successMsg: options.successMsg || 'Record has been added successfully!',
            errorMsg: options.ErrorMsg || 'An error occurred please try again'
        };
        var res = config.response;

        if (res === 'true' || res === true) {
            AppAlerts.inlineAlert(config.successMsg, 'success', config.parent);
        } else {
            var errorsList = $("<ul />");
            res = JSON.parse(res);
            if (typeof res === 'object' && 'errors' in res) {
                for (var i = 0; i < res.errors.length; i++) {
                    errorsList.append($('<li />').html(res.errors[i].description));
                }
                AppAlerts.inlineAlert(errorsList, 'warning', config.parent);
            } else {

                AppAlerts.inlineAlert(config.errorMsg, 'warning', config.parent);
            }

        }
    },
    daterangepickerInit: function (callback, _startdate, _enddate) {

        var picker = $("#kt_dashboard_daterangepicker");
        var start = _startdate || moment();
        var end = _enddate || moment();
        function cb(start, end, label) {
            var title = '';
            var range = '';

            if ((end - start) < 100 || label === 'Today') {
                title = 'Today:';
                range = start.format('MMM D');
            } else if (label === 'Yesterday') {
                title = 'Yesterday:';
                range = start.format('MMM D');
            } else {
                range = start.format('MMM D') + ' - ' + end.format('MMM D');
            }

            $("#kt_dashboard_daterangepicker_date").html(range);
            $("#kt_dashboard_daterangepicker_title").html(title);

            $("#selectedDates").attr('data-startdate', start.format('YYYY-MM-DD'));
            $("#selectedDates").attr('data-enddate', end.format('YYYY-MM-DD'));

            callback(start.format('YYYY-MM-DD'), end.format('YYYY-MM-DD'));

        }

        picker.daterangepicker({
            direction: KTUtil.isRTL(),
            startDate: start,
            endDate: end,
            opens: 'left',
            ranges: {
                'Today': [moment(), moment()],
                'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                'This Month': [moment().startOf('month'), moment().endOf('month')],
                'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
                'This Year': [moment().startOf('year'), moment()]

            }
        }, cb);

        cb(start, end, '');
    },
};

var utils = NiceOne.Utilities;

if (typeof Number.prototype.NiceFormate !== "function") {
    Number.prototype.NiceFormate = function () {
        var nStr = '';
        var x = this.toFixed(2).toString().split('.');
        var x1 = x[0];
        var x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;

    };
}
if (typeof Number.prototype.NiceFormateNumber !== "function") {
    Number.prototype.NiceFormateNumber = function () {
        var nStr = '';
        var x = this.toFixed(2).toString().split('.');
        var x1 = x[0];

        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1;

    };
}

$(function () {
    $('.kt-form ').each(function () {
        $(this).find('input').keypress(function (e) {
            // Enter pressed?
            if (e.which == 10 || e.which == 13) {
                $("#serachButton").click();
            }
        });
    });
});