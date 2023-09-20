var Email = '';
var emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;

$(document).ready(function () {

    //$("#signin-email,#signin-otp").on('input', function () {
    //    $("#successmsg1").empty();

    //});
    //$("#successmsg1").empty();

    //var UserMasterID = sessionStorage.getItem('UserId');
    //if (UserMasterID == "" || UserMasterID == null) {
    //    window.location.href = "/Login/Index";
    //}

    if (getUrlParameter('Emailid') != null) {

        RowClickEventHandler();
    }
    //if (getUrlParameter1('File') != null) {
    //    //
    //    //RowClickEventHandler1();
    //}

    //$("#btndownload").attr('disabled', true);
    //$("#lblemailresend").css("display", "none");
    //$("#timer").hide();
    //$("#successmsg1").hide();
    //LocalFilePath = sessionStorage.getItem('LoaclPath')
    //uploadedFilePath = sessionStorage.getItem('uploadedFilePath')
    //$('#DocumentPreview').attr("src", LocalFilePath);

});
//var Gemail = '';
function SendVerifyEmailOTP() {

    $("#signin-email,#signin-otp").on('input', function () {
        $("#successmsg1").empty();

    });
    ;
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
            url: '/Login/GetEmailData',
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
                //startTimer(timerDuration, displayElement);
                console.log(response);
            },
            error: function (error) {
                console.log(error);
            }
        });
    }

}

function VerifyOTP() {
    $("#signin-email,#signin-otp").on('input', function () {
        $("#successmsg1").empty();

    });
    var otp = $("#signin-otp").val();
    var FilePath = getUrlParameter('File'); // Get the 'File' parameter from the URL

    if (!FilePath) {
        // Handle the case where the 'File' parameter is not present in the URL
        console.log("FileID not found in the URL.");
        return;
    }
    var SignerName = getUrlParameter('SignerName');
    var Fileid = getUrlParameter('Fileid');
    var Emailid = getUrlParameter('Emailid');
    var SignerID = getUrlParameter('SignerID');
    var UploadedDocumentId = getUrlParameter('UploadedDocumentId');
    $.ajax({
        type: 'POST',
        url: '/Login/VerifyMobile',
        dataType: 'json',
        data: {
            VOTP: otp,
            FilePath: FilePath,
            SignerID: SignerID,
            UploadedDocumentId: UploadedDocumentId
        },
        async: false,
        success: function (result) {

            if (result.Status == 1) {
                if (result.msg == 1) {
                    $("#successmsg1").hide();
                    window.location.href = "/zipSign/SigningRequest?File=" + result.SignedPDF + "&SignerName=" + SignerName + "&Fileid=" + Fileid + "&Emailid=" + Emailid + "&SignerID=" + SignerID + "&UploadedDocumentId=" + UploadedDocumentId;

                } else {
                    $("#successmsg1").empty();
                    var row = '<div class="alermsg col-md-12 p-1" role="alert">Please Enter Correct OTP.</div>';
                    $("#successmsg1").append(row);
                    $("#signin-otp").val('');
                }
            }
            else {
                if (result.msg == 1) {
                    window.location.href = "/zipSign/SigningRequest?File=" + result.Path + "&SignerName=" + SignerName + "&Fileid=" + Fileid + "&Emailid=" + Emailid + "&SignerID=" + SignerID + "&UploadedDocumentId=" + UploadedDocumentId;
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
//Resend OTP
$('#lblemailresend').click(function () {
    var timerDuration = 60;
    var display = $('#timer');
    startTimer(timerDuration, display);
    SendVerifyEmailOTP();
    $(".otpdiv").show();
    $(".submitbtn").hide();
})

//var getUrlParameter1 = function getUrlParameter1(sParam) {

//    var sPageURL = window.location.search.substring(1),
//        sURLVariables = sPageURL.split('&'),
//        sParameterName,
//        i;

//    for (i = 0; i < sURLVariables.length; i++) {
//        sParameterName = sURLVariables[i].split('=');

//        if (sParameterName[0] === sParam) {
//            var value = sParameterName[1];
//            return decodeURIComponent(value);
//        }
//    }
//}

var getUrlParameter = function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            var value = sParameterName.slice(1).join('=');
            return decodeURIComponent(value);
        }
    }
};


function RowClickEventHandler() {
    ;
    var fileId = getUrlParameter('Emailid');

    $.ajax({
        url: '/zipSign/GetFileData',
        type: 'POST',
        dataType: 'json',
        data: {
            SignUploadId1: fileId,
        },
        async: false,
        success: function (result) {
            ;
            var Email = result.filePath.split('\\').pop();

            $("#hdnEmail").val(Email);
            //sessionStorage.setItem('Email1', Email);
            //$("#signin-email").attr("disabled", true); 
        },
        error: function () {
            alert('Something Went Wrong');
        }
    });
}


