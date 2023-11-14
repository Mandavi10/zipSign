$(document).ready(function () {
    var UserMasterID = sessionStorage.getItem('UserId');
    if (UserMasterID == "" || UserMasterID == null) {
        window.location.href = "/Login/Index";
    }
    //$(window).on('beforeunload', function () {

    //    $.ajax({
    //        url: '/Login/UpdateUserStatus',
    //        method: 'POST',
    //        data: { userId: UserMasterID },
    //        async: true,
    //    });
    //});
});
