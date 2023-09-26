$(document).ready(function () {
    
    var url = window.location.href;
    var userCode = getUrlParameter('UserCode');
    if (userCode) {
        $('#oldpassword').hide();
        
        $.ajax({
            url: '/Login/GetDataForPasswordReset?UserCode=' + userCode,
            type: 'GET',
            dataType: 'json',
            success: function (result) { // Use 'result' here instead of 'data'
                
                var linkCreatedOn = result.CreatedOn; // Use 'result' here instead of 'data'
                var linkExpiredOn = result.ExpiredOn; // Use 'result' here instead of 'data'
                var currentDateTime = getCurrentDateTime();
                if (result.IsExpired == true) {
                    alert("Link Has Expired");
                }
                //else if (currentDateTime > linkExpiredOn) {
                   // alert("Link Has Expired");
                //}
                else {
                    sessionStorage.setItem('UserCode', result.CreatedBy);
                    sessionStorage.setItem('Email', result.Email);
                    //UpdatePassword(result.CreatedBy, result.Email);
                    //alert('Link is still valid');
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
    var hours = currentDate.getHours();
    var minutes = ('0' + currentDate.getMinutes()).slice(-2);
    var seconds = ('0' + currentDate.getSeconds()).slice(-2);
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // If hours is 0, set it to 12
    var formattedDateTime = `${day}-${month}-${year} ${hours}:${minutes}:${seconds} ${ampm}`;
    return formattedDateTime;
}


function UpdatePassword() {
    
    Newpassword = $("#newpassword").val();
    confirmpassword = $("#confirmpassword").val();
    UserCode = sessionStorage.getItem('UserCode');
    Email = sessionStorage.getItem('Email');
    $.ajax({
        url: '/Login/UpdatePassword',
        type: 'GET',
        dataType: 'json',
        data: {
            userCode: UserCode,
            Email: Email,
            Newpassword,
            confirmpassword
        },
        success: function (result) { // Use 'result' here instead of 'data'
            
            var linkCreatedOn = result.CreatedOn; // Use 'result' here instead of 'data'
            var linkExpiredOn = result.ExpiredOn; // Use 'result' here instead of 'data'
            var currentDateTime = getCurrentDateTime();
            if (result.IsExpired == true) {
                alert("Link Has Expired");
            }
            else if (currentDateTime > linkExpiredOn) {

            }
            else {
                //UpdatePassword(result.CreatedBy, result.Email);
                //alert('Link is still valid');
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            
            console.log('AJAX Error:');
            console.log('Status:', textStatus);
            console.log('Error:', errorThrown);
        }
    });
}


