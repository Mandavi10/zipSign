$(document).ready(function () {
    var UserMasterID = sessionStorage.getItem('UserId');
    if (UserMasterID == "" || UserMasterID == null) {
        window.location.href = "/Login/Index";
    }
    var inactivityTimeout;
    var maxInactivityTime = 900000; // 15 minutes in milliseconds

    $(document).on('mousemove click keydown', function () {
        
        resetInactivityTimeout();
    });

    function resetInactivityTimeout() {
        
        clearTimeout(inactivityTimeout);
        inactivityTimeout = setTimeout(SignOut, maxInactivityTime);
    }

    function SignOut() {
        
        var UserMasterID = sessionStorage.getItem('UserId');
        $.ajax({
            url: '/Login/SignOut',
            type: 'POST',
            data: {
                UserMasterID: UserMasterID
            },
            success: function (data) {
                sessionStorage.clear();
                window.location.href = '/Login/Index';
            },
            error: function (error) {
                // Handle errors if needed
                console.error('Logout failed:', error);
            }
        });
    }
});
