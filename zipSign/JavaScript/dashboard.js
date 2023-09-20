$(document).ready(function () {
    //$('#overlay').css('display', 'block');
    //$('#loader').css('display', 'block');
    var UserMasterID = sessionStorage.getItem('UserId');
    if (UserMasterID == "" || UserMasterID == null) {
        window.location.href = "/Login/Index";
    }

    function fetchUserProfile() {
        $.ajax({
            type: 'GET',
            url: '/Dashboard/Profileview',
            success: function (result) {
                $('#profile-picture').attr('src', result.profilePictureUrl);
                $('#user-name').text(result.firstName + ' ' + result.lastName);
                $('#user-email').text(result.email);
            },
            error: function (ex) {
                console.log('Error fetching user profile:', ex);
            }
        });
    }
});