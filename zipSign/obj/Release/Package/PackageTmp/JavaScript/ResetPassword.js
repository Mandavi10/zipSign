﻿var row = '';
$(document).ready(function () {
    $('#newpassword, #confirmpassword').on('paste copy', function (event) {
        event.preventDefault();
    });
    var url = window.location.href;
    var userCode = getUrlParameter('UserCode');
    var TxnId = getUrlParameter('TxnId');
    if (userCode) {
        $('#oldpassword').hide();
        $.ajax({
            url: '/Login/GetDataForPasswordReset?UserCode=' + userCode + '&TxnId=' + TxnId,
            type: 'GET', 
            dataType: 'json',
            async: false,
            success: function (result) {
                if (result.IsExpired === "True") {
                    window.location.href = "/zipSign/Link_Expired";
                }
                else if (result === 0) {
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
    var OldPassword = $("#oldpassword").val();
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
    }
    else if (!/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}/.test(Newpassword)) {
        var row = '<div class="alermsg col-md-12 p-1" role="alert">Password must contain one lowercase letter; one uppercase letter;  one numeral; one special character and at least 8 characters.</div>';
        $("#message").append(row);
        $("#newpassword").focus();
        return false;
    }

    if (confirmpassword === "") {
        var row = '<div class="alermsg col-md-12 p-1" role="alert">Please enter confirm password</div>';
        $("#message").append(row);
        $("#confirmpassword").focus();
        return false;
    }

    if (Newpassword !== confirmpassword) {
        var row = '<div class="alermsg col-md-12 p-1" role="alert">Password and confirm password do not match.</div>';
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
                $('#successpopup1').modal('show');
                //window.location.href = '/Login/Index';
                sessionStorage.clear();
            } else if (result.error) {
                $("#message").empty();
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


