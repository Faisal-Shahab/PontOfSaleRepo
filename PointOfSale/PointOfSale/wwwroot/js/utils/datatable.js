
var posDataTable = {
    services: {
        controller: "Account",
        actions: {
            Login: "Login"
        }
    },
    htmlWrapers: {
        ActionMenu: "<div class='btn-group'>" +
            "<a class='btn btn-xs green dropdown-toggle' href='javascript:;' data-toggle='dropdown' aria-expanded='false'>" +
            "  Actions  " +
            "<i class='fa fa-angle-down'></i>" +
            "</a>" +
            "<ul class='dropdown-menu' role='menu'>" +
            "{li}" +
            "</ul>" +
            "</div>"
    },

    createActionLink: function (actions, row) {
        var config = actions || {
            classes: "btnEdit",
            icon: "fa fa-edit",
            actionName: "Edit",
            dataAttr: ["id"], //
            href: "/controller/action",
            hrefParams: [{ Name: "id", ValueColumn: "id" }]
        };
        var options = "";
        $.map(config, function (item) {

            var $div = $("<div />");
            var actionLink = $("<a />").addClass(item.classes).html("<i class='" + item.icon + "'></i>" + item.actionName);

            $.map(item.dataAttr, function (attr) {
                $(actionLink).attr("data-" + attr, row[attr]);
            });

            if (item.href) {

                var href = item.href;

                var i = 0;
                $.map(item.hrefParams, function (ele) {
                    var seperator = "";
                    if (i === 0) {
                        seperator = "?";
                    } else {
                        seperator = "&";
                    }
                    href += seperator + ele.Name + "=" + row[ele.ValueColumn];
                    i++;
                });

                $(actionLink).attr("href", href);


            }

            if (item.target) {
                $(actionLink).attr("target", item.target);
            }
            options += $div.append($("<li />").append(actionLink)).html();

        });

        return options;

    },
    setDataTableFilters: function (data) {
        var obj = {};
        obj.PageLength = data.length;
        obj.OrderDirection = data.order[0].dir === "asc" ? 1 : -1;
        obj.Start = data.start;
        obj.SearchTerm = data.search.value;
        obj.Draw = data.draw;
        obj.OrderBy = data.columns[data.order[0].column].name;
        return obj;
    }

};
