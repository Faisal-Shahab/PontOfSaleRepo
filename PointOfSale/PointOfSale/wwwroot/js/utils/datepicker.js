
var daterangepickerInit = function (callback, _startdate, _enddate) {
    const start = _startdate || moment();
    const end = _enddate || moment();

    function cb(start, end) {
        callback(start.format('YYYY-MM-DD'), end.format('YYYY-MM-DD'));
        $("#kt_daterangepicker_4").val(start.format('YYYY-MM-DD') + " - " + end.format('YYYY-MM-DD'));
    }

    $("#kt_daterangepicker_4").daterangepicker({
        startDate: start,
        endDate: end,
        ranges: {
            "Today": [moment(), moment()],
            "Yesterday": [moment().subtract(1, "days"), moment().subtract(1, "days")],
            "Last 7 Days": [moment().subtract(6, "days"), moment()],
            "Last 30 Days": [moment().subtract(29, "days"), moment()],
            "This Month": [moment().startOf("month"), moment().endOf("month")],
            "Last Month": [moment().subtract(1, "month").startOf("month"), moment().subtract(1, "month").endOf("month")]
        }
    }, cb);

    cb(start, end);
}
