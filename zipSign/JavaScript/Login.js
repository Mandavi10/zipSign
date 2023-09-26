var row = '';
$(document).ready(function () {
    //;
    //$("#email, #password, #signin-password", "#mobileotp").on('change', function () {
    //    $("#message").empty();
    //    row = '';
    //});
    $("#email, #password, #signin-password, #mobileotp").on('input', function () {
        $("#message").empty();
        row = '';
    });
});
var isvalidate = 0;
//To Reload Captcha
function ReloadCaptcha() {
    $("#email, #password, #signin-password, #mobileotp").on('input', function () {
        $("#message").empty();
        row = '';
    });
    var imageElement = document.getElementById('Image');
    var timestamp = new Date().getTime();
    var imageUrl = '/Login/GetCaptchaImage?' + timestamp;
    imageElement.src = imageUrl;
}
//Timer Function
function startTimer(duration, display) {
    var timer = duration, minutes, seconds;
    var intervalId = setInterval(function () {
        minutes = parseInt(timer / 60, 10);
        seconds = parseInt(timer % 60, 10);
        minutes = minutes < 10 ? "0" + minutes : minutes;
        seconds = seconds < 10 ? "0" + seconds : seconds;
        display.text(minutes + ":" + seconds);
        $('#lblresend').hide();
        if (--timer < 0) {
            //;
            clearInterval(intervalId);
            $('#lblresend').show();
            $(".enterotpdiv").hide();
            $(".alermsg ").attr("hidden", true);
            $("#mobileotp").val('');
            startTimer().hide();
        }
    }, 1000);
}
//Resend OTP
$('#lblresend').click(function () {
    $("#email, #password, #signin-password, #mobileotp").on('input', function () {
        $("#message").empty();
        row = '';
    });
    var timerDuration = 60;
    var display = $('#timer');
    $("#mobileotp").val('');
    startTimer(timerDuration, display);
    SendLoginEmailOTP(textbox, username, mobile);
    //SendLoginMobileOTP(username1, mobile1);
})
//Login Function
function Login() {
    //;
    $("#email, #password, #signin-password, #mobileotp").on('input', function () {
        $("#message").empty();
        row = '';
    });
    validations();
    if (isvalidate === 0) {
        return false;
    }
    var textbox = $("#email").val();
    var password = $("#password").val();
    window.textbox1 = textbox;
    var captchaInput = $('#signin-password').val();
    $.ajax({
        url: '/Login/Login',
        type: 'POST',
        data: {
            captchaInput: captchaInput,
            Email: textbox,
            Mobile: textbox,
            Password: password,
        },
        success: function (result) {
            //;
            if (result && result.length > 0) {
                sessionStorage.setItem('UserId', result[0].UserId);

                var username = result[0].UserName;
                var email = result[0].Email;
                var mobile = result[0].Mobile;
                $("#txtname").val(username);
                $("#txtemail").val(email);
                $("#txtphone").val(mobile);

                window.username1 = username;
                window.mobile1 = mobile;
                ShowProfile($("#txtname").val(username),
                    $("#txtemail").val(email),
                    $("#txtphone").val(mobile));

                SendLoginEmailOTP(email, username, mobile);

                //To check if Textbox is filled With Email Or Mobile
                //var emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
                //var mobileRegex = /^\d{10}$/;
                //if (emailRegex.test(textbox)) {

                //    slide('next');
                //    $("#password").val('');
                //    $('#signin-password').val('');
                //    var timerDuration = 60;
                //    var display = $('#resmobotp');
                //    startTimer(timerDuration, display);
                //} else if (mobileRegex.test(textbox)) {
                //    SendLoginMobileOTP(username, mobile);
                //    slide('next');
                //    $("#password").val('');
                //    $('#signin-password').val('');
                //    var timerDuration = 60;
                //    var display = $('#resmobotp');
                //    startTimer(timerDuration, display);
                //}
            }
            //else if (isvalidate === 0) {
            //    return false;
            //}
            else if (typeof result.status === 'string') {
                if (result.status == "Validation failed") {
                    // Handle validation errors
                    $.each(result.errors, function (index, error) {
                        row += '<div class="alermsg col-md-12 p-1" role="alert">' + error + '</div>';
                        $('#confirmationpopup').modal('hide');
                    });
                }
                else if (result.status == "Email/Mobile can't Empty" || result.status == "Password can't Empty" || result.status == "Invalid password format" || result.status == "Please enter captcha") {
                    // Handle the "Email/Mobile can't Empty" case
                    var row = '<div class="alermsg col-md-12 p-1" role="alert">' + result.status + '</div>';
                    $("#message").append(row);
                }
                else if (result.success === false) {
                    $("#message").empty();
                    var row = '<div class="alermsg  col-md-12 p-1" role="alert">Incorrect Captcha.</div>';
                    $("#message").append(row);
                    //ReloadCaptcha();
                    $("#signin-password").val('');
                    return false;
                }
            }
            else {
                var row = '<div class="alermsg  col-md-12 p-1" role="alert">Invalid Credentials.</div>';
                $("#message").empty();
                $("#message").append(row);
                $("#password").val('');
                //ReloadCaptcha();
                $("#signin-password").val('');
                return false;
            }
        },
        error: function (error) {
            alert("Login Failed");
            console.log(error);
        }
    });
}
function SendLoginEmailOTP(textbox, username, mobile) {
    //;
    $("#email, #password, #signin-password, #mobileotp").on('input', function () {
        $("#message").empty();
        row = '';
    });
    $.ajax({
        url: '/Login/SendOTP',
        type: 'POST',
        data: {
            Email: textbox,
            CusName: username,
            MobileNo: mobile
        },
        success: function (response) {

            var Email = textbox;
            var atIndex = Email.indexOf('@');
            if (atIndex !== -1) {
                var localPart = Email.slice(0, atIndex);
                var domainPart = Email.slice(atIndex);

                var remainingChars = localPart.length - 2; // Calculate the number of characters to replace

                if (remainingChars > 0) {
                    var xChars = '*'.repeat(remainingChars); // Create a string of 'x' characters of the calculated length
                    localPart = localPart[0] + xChars + localPart[localPart.length - 1];
                }
                var formattedEmail = localPart + domainPart;
                var formattedMobile = "xxxxx" + "xxx00";

                var span = $("#lblemail .enterddata");
                span.text("Please enter the OTP sent to " + formattedEmail + " or ending in " + formattedMobile);
                slide('next');
                $("#password").val('');
                $('#signin-password').val('');
                var timerDuration = 60;
                var display = $('#resmobotp');
                startTimer(timerDuration, display);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function SendLoginMobileOTP(username, textbox) {
    $("#email, #password, #signin-password, #mobileotp").on('input', function () {
        $("#message").empty();
        row = '';
    });
    $.ajax({
        url: '/Login/GetSMSData1',
        type: 'POST',
        data: {

            CusName: username,
            MobileNo: textbox,
        },
        success: function (response) {
            var mobile = textbox;
            var formattedMobile = "xxxxx" + mobile.slice(-5);
            var span = $("#lblemail .enterddata");
            span.text(formattedMobile);
            console.log(response);
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function validations() {
    //;
    var emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    var mobileRegex = /^\d{10}$/;
    $("#email, #password, #signin-password, #mobileotp").on('input', function () {
        $("#message").empty();
        row = '';
    });
    if ($("#email").val() === "" && $("#password").val() === "") {
        if ($("#message").children().length === 0) {
            var row = '<div class="alermsg col-md-12 p-1" role="alert">Email and Password are required.</div>';
            $("#message").append(row);
            $("#email").focus();
            return false;
        }
    }

    if ($("#email").val() === "") {
        row = '<div class="alermsg col-md-12 p-1" role="alert">Email is Required.</div>';
        $("#message").append(row);
        $("#textbox").focus();
        return false;
        isvalidate = 0;
    }
    //else if (!emailRegex.test($("#email").val())) {
    //    row = '<div class="alermsg col-md-12 p-1" role="alert">You have entered an invalid email address! (e.g. xxxx@gmail.com)".</div>';
    //    $("#message").append(row);
    //    $("#textbox").focus();
    //    return false;
    //    isvalidate = 0;
    //}
    //else if (!mobileRegex.test($("#email").val())) {
    //    row = '<div class="alermsg col-md-12 p-1" role="alert">You have entered an invalid Mobile Number".</div>';
    //    $("#message").append(row);
    //    $("#textbox").focus();
    //    return false;
    //    isvalidate = 0;
    //}

    else if ($("#password").val() === "") {
        $("#message").empty();
        row = '<div class="alermsg col-md-12 p-1" role="alert">Password is Required.</div>';
        $("#message").append(row);
        $("#password").focus();
        return false;
        isvalidate = 0;
    }
    else if ($('#signin-password').val() === "") {
        $("#message").empty();
        row = '<div class="alermsg col-md-12 p-1" role="alert">Please Verify CAPTCHA.</div>';
        $("#message").append(row);
        $("#signin-password").focus();
        return false;
        isvalidate = 0;
    }
    else {
        isvalidate = 1;
    }
}
function VerifyOTP() {
    $("#email, #password, #signin-password, #mobileotp").on('input', function () {
        $("#message").empty();
        row = '';
    });

    var otp = $("#mobileotp").val();
    var isVerified = 0;
    //
    $.ajax({
        type: 'POST',
        url: '/Login/VerifyOTP',
        dataType: 'json',
        data: {
            VOTP: otp
        },
        async: false, // Make the request synchronous to wait for the response
        success: function (result) {
            //
            //var result = response;
            if (result === 1) {
                $("#mobileotp").val('');
                //sessionStorage.setItem('UserId', result.UserId);
                $('#overlay').css('display', 'block');
                $('#loader').css('display', 'block');
                window.location.href = "/Dashboard/Index2";
                isVerified = 1;
            }
            else {
                $("#mobileotp").focus();
                $(".alermsg ").removeAttr("hidden");
                return false;
                isVerified = 0;
            }
        },
        error: function (ex) {
            console.log("Error occurred during OTP verification");
            isVerified = 0;
        }
    });
    return isVerified;
}
function ShowProfile() {

    $("#usernameDisplay").text($("#txtname").val());
    $("#emailDisplay").text($("#txtemail").val());
    $("#phoneDisplay").text($("#txtmobile").val());
}
function SignOut() {
    UserMasterID = sessionStorage.getItem('UserId')
    $.ajax({
        url: '/Login/SignOut',
        type: 'POST',
        data: {
            UserMasterID: UserMasterID
        },
        success: function (data) {
            //
            sessionStorage.clear();
            window.location.href = '/Login/Index';
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function ForgotPassword() {
    
    var email = $("#emailresetpassword").val();
    var emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    if (!emailRegex.test(email)) {
        $("#message").empty();
        var row = '<div class="alermsg  col-md-12 p-1" role="alert">Invalid Email Format.</div>';
        $("#message").append(row);
        $("#emailresetpassword").val('');
        return false;
    }
    else
    {
        $.ajax({
            url: '/Login/ResetPassword',
            type: 'POST',
            data: {
                Email: email,
            },
            success: function (result) {
                
                if (result.StatusCode === 7) {
                    SendPasswordResetLink(result.UserCode, result.UserEmail);
                }
                else if (result.success === false) {
                    $("#message").empty();
                    var row = '<div class="alermsg  col-md-12 p-1" role="alert">Incorrect Captcha.</div>';
                    $("#message").append(row);
                    //ReloadCaptcha();
                    $("#signin-password").val('');
                    return false;
                }
                else {
                    var row = '<div class="alermsg  col-md-12 p-1" role="alert">Invalid Credentials.</div>';
                    $("#message").empty();
                    $("#message").append(row);
                    $("#password").val('');
                    //ReloadCaptcha();
                    $("#signin-password").val('');
                    return false;
                }
            },
            error: function (error) {
                alert("Login Failed");
                console.log(error);
            }
        });
    }
}

function SendPasswordResetLink(UserCode, Email) {
    
    $.ajax({
        url: '/Login/SendLinkviaEmail',
        type: 'POST',
        data: {
            Email: Email,
            UserCode: UserCode
        },
        success: function (response) {
            var mobile = textbox;
            var formattedMobile = "xxxxx" + mobile.slice(-5);
            var span = $("#lblemail .enterddata");
            span.text(formattedMobile);
            console.log(response);
        },
        error: function (error) {
            console.log(error);
        }
    });
}