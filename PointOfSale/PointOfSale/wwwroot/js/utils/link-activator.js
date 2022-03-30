var activateLink = function () {

    $(function () {

        //Get the current page link
        var currentPageLink = document.location.href;

        var root = utils.getSiteBase();
        var $activeLink;
        var parentMenu;
        //Search your menu for a linkURL that is similar to the active pageURL
        $(".kt-nav a").each(function () {

            $(".kt-nav a").removeClass("active");

            var linkLoop = [root, $(this).attr("href")].join("");

            if (linkLoop === currentPageLink) {
                var foundUrl = $(this).attr("href");

                $activeLink = $('a[href="' + foundUrl + '"]');

                $activeLink.closest(".kt-nav__item").addClass("active");

                //parentMenu = $activeLink.closest(".kt-nav__item").parent().closest(".kt-nav__item");

                //if (parentMenu.length) {

                //    parentMenu.addClass("active");

                //    parentMenu.find(".arrow").addClass("open");
                //}
            }
            else {


                $activeLink = $('a[href*="' + window.location.href.split("/")[4] + '"]');
                // $activeLink.closest('.nav-item').addClass('active');

                //parentMenu = $activeLink.closest(".nav-item").parent().closest(".nav-item");

                //if (parentMenu.length) {

                //    parentMenu.addClass("active");

                //    parentMenu.find(".arrow").addClass("open");
                //}
            }
        });

        $(".kt-menu__item").removeClass("kt-menu__item--open");
        $(".kt-menu__item").removeClass("kt-menu__item--here");

        $(".kt-menu__nav .kt-menu__item a").each(function () {
      
            var linkLoop = [root, $(this).attr("href")].join("");

            if (linkLoop.split('/')[3] === currentPageLink.split('/')[3]) {
                $(this).closest(".kt-menu__item").addClass("kt-menu__item--open");
                $(this).closest(".kt-menu__item").addClass("kt-menu__item--here");

            }
        });
  
    });

}();