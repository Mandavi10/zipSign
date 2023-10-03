var row = '';
$(document).ready(function () {
    
    var url = window.location.href;
    var userCode = getUrlParameter('UserCode');
    if (userCode) {
        $('#oldpassword').hide();
        $.ajax({
            url: '/Login/GetDataForPasswordReset?UserCode=' + userCode,
            type: 'GET',
            dataType: 'json',
            success: function (result) {
                
                var linkCreatedOn = result.CreatedOn;
                var linkExpiredOn = result.ExpiredOn;
                var currentDateTime = getCurrentDateTime();
                if (result.IsExpired == true) {
                    window.location.href = "/zipSign/Link_Expired";
                }
                else if (currentDateTime > linkExpiredOn) {
                    window.location.href = "/zipSign/Link_Expired";
                }
                else {
                    sessionStorage.setItem('UserCode', result.CreatedBy);
                    sessionStorage.setItem('Email', result.Email);
                }
            },
            error: function (xhr, textStatus, errorThrown) {

                console.log('AJAX Error:');
                console.log('Status:', textStatus);
                console.log('Error:', errorThrown);
            }
        });
    } else {
        console.log('UserCode parameter not found in the URL');
        // Handle the case when the UserCode parameter is not present in the URL
    }
});
function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

function getCurrentDateTime() {
    var currentDate = new Date();
    var year = currentDate.getFullYear();
    var month = ('0' + (currentDate.getMonth() + 1)).slice(-2); // Months are zero-based
    var day = ('0' + currentDate.getDate()).slice(-2);
    var hours = ('0' + currentDate.getHours()).slice(-2);
    var minutes = ('0' + currentDate.getMinutes()).slice(-2);
    var seconds = ('0' + currentDate.getSeconds()).slice(-2);

    var formattedDateTime = `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
    return formattedDateTime;
}



function UpdatePassword() {
    
    var Newpassword = $("#newpassword").val();
    var confirmpassword = $("#confirmpassword").val();
    var UserCode = sessionStorage.getItem('UserCode');
    var Email = sessionStorage.getItem('Email');
    $("#message").empty();
    if (Newpassword === "") {
        var row = '<div class="alermsg col-md-12 p-1" role="alert">Please enter a password</div>';
        $("#message").append(row);
        $("#newpassword").focus();
        return false;
    } else if (!/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}/.test(Newpassword)) {
        var row = '<div class="alermsg col-md-12 p-1" role="alert">Password must contain one lowercase letter, one uppercase letter, one numeric digit, at least 8 characters, and one special character</div>';
        $("#message").append(row);
        $("#newpassword").focus();
        return false;
    }

    if (confirmpassword === "") {
        var row = '<div class="alermsg col-md-12 p-1" role="alert">Please confirm the password</div>';
        $("#message").append(row);
        $("#confirmpassword").focus();
        return false;
    }

    if (Newpassword !== confirmpassword) {
        var row = '<div class="alermsg col-md-12 p-1" role="alert">Passwords do not match</div>';
        $("#message").append(row);
        $("#confirmpassword").focus();
        return false;
    }
    $.ajax({
        url: '/Login/UpdatePassword',
        type: 'POST', 
        dataType: 'json',
        data: {
            userCode: UserCode,
            Email: Email,
            NewPassword: Newpassword, 
            confirmPassword: confirmpassword 
        },
        success: function (result) {
            if (result.success) {
                console.log('Password updated successfully');
                window.location.href = '/Login/Index';
                sessionStorage.clear();
            } else if (result.error) {
                var row = '<div class="alermsg col-md-12 p-1" role="alert">' + result.error + '</div>';
                $("#message").append(row);
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            // AJAX request to server failed
            // Display a generic error message
            var row = '<div class="alermsg col-md-12 p-1" role="alert">An error occurred while processing your request.</div>';
            $("#message").append(row);
        }
    });
}



