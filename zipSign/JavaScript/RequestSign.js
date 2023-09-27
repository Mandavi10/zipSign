var emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
var $checkbox = $("#chckbox");
var $submitButton = $("#submitBtn");
var $message = $("#message");
$(document).ready(function () {
    //$submitButton.prop('disabled', true);
    $("#timer").hide();
    $("#lblemailresend").hide();
    $("#signin-email,#signin-otp").on('input', function () {
        $("#successmsg1").empty();
    });
    $("#successmsg1").empty();
    var UId = getParameterByName('UId');
    if (UId !== null) {
        
        RowClickEventHandler(UId);
    }
    $submitButton.on("click", function (e) {
        if (!$checkbox.prop("checked")) {
            e.preventDefault();
            $message.show();
        } else {
            $message.hide();
            SendVerifyEmailOTP();
        }
    });
    
    //$checkbox.on("change", function () {
        
    //    // Check if the checkbox is checked
    //    if ($checkbox.prop("checked")) {
    //        // Enable the submit button
    //        $submitButton.prop('disabled', false);
    //    } else {
    //        // Disable the submit button
    //        $submitButton.prop('disabled', true);
    //        $checkbox.focus();
    //    }
    //});
});
function RowClickEventHandler(UId) {
    
    $.ajax({
        url: '/NSDL/GetDocumentAllData',
        type: 'POST',
        dataType: 'json',
        data: {
            Link: UId,
        },
        async: false,
        success: function (result) {
            
            if (result.IsExpired == 'True') {
                alert("Link Expired");
                return false;
            }
            else {
                var EmailID = result.EmailID;
                $("#hdnEmail").val(EmailID);
            }
        },
        error: function () {
            alert('Something Went Wrong');
        }
    });
}
function getParameterByName(name) {
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(window.location.href);
    if (!results) return null;
    if (!results[2]) return '';
    var decodedValue = decodeURIComponent(results[2].replace(/\+/g, " "));
    return decodedValue;
}




function SendVerifyEmailOTP() {
    
    $("#signin-email,#signin-otp").on('input', function () {
        $("#successmsg1").empty();

    });

    $("#successmsg1").empty();
    if (!emailRegex.test($("#signin-email").val())) {
        $("#successmsg1").append("You have entered an incorrect email id..!");
        $("#successmsg1").show();
    }
    else if ($("#hdnEmail").val() != $("#signin-email").val()) {
        $("#successmsg1").append("You have entered an invalid email id..!");
        $("#successmsg1").show();
    }
    else {
        $.ajax({
            url: '/Login/GetEmailDataForSignLogin',
            type: 'POST',
            data: {
                Email: $("#signin-email").val()
            },
            success: function (response) {
                var timerDuration = 60;
                var display = $('#timer');
                startTimer(timerDuration, display);
                $("#timer").show();
                $("#successmsg1").append("One Time Password Sent To Your Email Address..");
                $("#successmsg1").show();
                $(".otpdiv").show();
                $(".otpbtn").hide();
                $(".submitbtn").show();
                console.log(response);
            },
            error: function (error) {
                console.log(error);
            }
        });
    }

}

function startTimer(duration, display) {

    var timer = duration, minutes, seconds;
    var intervalId = setInterval(function () {
        minutes = parseInt(timer / 60, 10);
        seconds = parseInt(timer % 60, 10);
        minutes = minutes < 10 ? "0" + minutes : minutes;
        seconds = seconds < 10 ? "0" + seconds : seconds;
        display.text(minutes + ":" + seconds);
        $('#lblemailresend').hide();
        if (--timer < 0) {
            clearInterval(intervalId);
            $('#lblemailresend').show();
            display.hide(); // Hide the timer display when the timer reaches 0.
        }
    }, 1000);
}

function VerifyOTP() {
    $("#signin-email,#signin-otp").on('input', function () {
        $("#successmsg1").empty();

    });
    var otp = $("#signin-otp").val();
    var UID = getParameterByName('UId');
    $.ajax({
        type: 'POST',
        url: '/Login/VerifyMobile',
        dataType: 'json',
        data: {
            VOTP: otp,
            UploadedDocumentId: UID
        },
        async: false,
        success: function (result) {

            if (result.Status == 1) {
                if (result.msg == 1) {
                    $("#successmsg1").hide();
                    window.location.href = "/zipSign/SigningRequest?UId="+UID;

                } else {
                    $("#successmsg1").empty();
                    var row = '<div class="alermsg col-md-12 p-1" role="alert">Please Enter Correct OTP.</div>';
                    $("#successmsg1").append(row);
                    $("#signin-otp").val('');
                }
            }
            else {
                if (result.msg == 1) {
                    window.location.href = "/zipSign/SigningRequest?UId=" + UID;
                    //window.location.href = "/zipSign/SigningRequest?File=" + result.Path + "&SignerName=" + SignerName + "&Fileid=" + Fileid + "&Emailid=" + Emailid + "&SignerID=" + SignerID + "&UploadedDocumentId=" + UploadedDocumentId;
                } else {
                    $("#successmsg1").empty();
                    var row = '<div class="alermsg col-md-12 p-1" role="alert">Please Enter Correct OTP.</div>';
                    $("#successmsg1").append(row);
                    $("#signin-otp").val('');
                }
            }

        },
        error: function (ex) {
            console.log("Error occurred during OTP verification");
        }
    });
}