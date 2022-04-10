
function initEvents() {
    $('#forgetPasswordForm').submit(function (event) {
        event.preventDefault(); //prevent default action 
        const formData = $(this).serialize();
        const post_url = $(this).attr("action");

        $.ajax({
            url: post_url,
            type: 'post',
            data: formData,
            dataType: 'json',
            success: function (data) {
                alert(data.message);
            }
        });
    });
}

jQuery(document).ready(function () {
    initEvents();
});