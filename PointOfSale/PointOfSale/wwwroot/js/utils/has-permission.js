var hasPermission = {
 
    evaluatePermissions: function () {
      
        $("[data-has-permission='False']").each(function(index) {

           $(this).remove();
        });

    }
}

$(function() {
 
    hasPermission.evaluatePermissions();
});