
$(document).ready(function () {
    $(".sidebar").click(),
        $(".btn-toggle-fullwidth").on("click", function () {
            $("body").hasClass("layout-fullwidth") ? $("body").removeClass("layout-fullwidth") : $("body").addClass("layout-fullwidth"), $(this).find(".fa").toggleClass("fa-arrow-left fa-arrow-right");
        }),
        
        $(window).on("load", function () {
            $("#main-content").height() < $("#left-sidebar").height() && $("#main-content").css("min-height", $("#left-sidebar").innerHeight() - $("footer").innerHeight());
        }),
        $(window).on("load resize", function () {
            $(window).innerWidth() < 1280 ? $("body").addClass("layout-fullwidth sidebar_toggle") : $("body").removeClass("layout-fullwidth sidebar_toggle");
        });
});


   
