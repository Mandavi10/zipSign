var row = '';
var intervalId;
$(document).ready(function () {
    $("#message").empty();
    row = '';
    $("#email, #password, #signin-password, #mobileotp").on('input', function () {
        $("#message").empty();
        row = '';
    });
    $("#mobileotp").on('input', function () {
        $("#message1").empty();
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
    intervalId = setInterval(function () {
        minutes = parseInt(timer / 60, 10);
        seconds = parseInt(timer % 60, 10);
        minutes = minutes < 10 ? "0" + minutes : minutes;
        seconds = seconds < 10 ? "0" + seconds : seconds;
        display.text(minutes + ":" + seconds);
        $('#lblresend').hide();
        if (--timer < 0) {

            clearInterval(intervalId);
            $('#lblresend').show();
            $(".enterotpdiv").hide();
            $("#resmobotp").hide();
            $(".alermsg ").attr("hidden", true);
            $("#mobileotp").val('');
            startTimer().hide();
        }
    }, 1000);
}
//Resend OTP
$('#lblresend').click(function () {
    $("#message1").empty();
    $("#message1").hide();
    $("#resmobotp").show();
    clearInterval(intervalId);
    var timerDuration = 60;
    var display = $('#resmobotp');
    $("#mobileotp").val('');
    startTimer(timerDuration, display);
    SendLoginEmailResendOTP($("#txtemail").val(), $("#txtnamehdn").val(), $("#txtmobilehdn").val());
    //SendLoginMobileOTP(username1, mobile1);
});
//Login Function
function Login() {
   
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
    //window.textbox1 = textbox;
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
            if (result && result.length > 0) {
                sessionStorage.setItem('UserId', result[0].UserId);
                var username = result[0].UserName;
                var email = result[0].Email;
                var mobile = result[0].Mobile;
                $("#txtname").val(username);
                $("#txtemail").val(email);
                $("#txtphone").val(mobile);
                var userData = {
                    username: username,
                    email: email,
                    mobile: mobile
                };
                var jsonString = JSON.stringify(userData);
                sessionStorage.setItem('user_data', jsonString);
                window.username1 = username;
                window.mobile1 = mobile;
                ShowProfile($("#txtname").val(username),
                    $("#txtemail").val(email),
                    $("#txtphone").val(mobile));
                $("#txtmobilehdn").val(mobile);
                $("#txtnamehdn").val(username);
                SendLoginEmailOTP(email, username, mobile);

            }
            else if (typeof result.status === 'string') {
                if (result.status == "Validation failed") {
                    // Handle validation errors
                    $.each(result.errors, function (index, error) {
                        row += '<div class=" col-md-12 p-1" role="alert">' + error + '</div>';
                        $('#confirmationpopup').modal('hide');
                    });
                }
                else if (result.status == "Email/Mobile can't Empty" || result.status == "Invalid email/mobile format" || result.status == "Password can't Empty" || result.status == "Invalid password format" || result.status == "Please enter captcha") {
                    $("#message").empty();
                    // Handle the "Email/Mobile can't Empty" case
                    var row = '<div class=" col-md-12 p-1" role="alert">' + result.status + '</div>';
                    $("#message").append(row);
                }

            }
            else if (result.status == 0) {    
                $("#message").empty();
                var row = '<div class="  col-md-12 p-1" role="alert">Please enter valid CAPTCHA</div>';
                $("#message").append(row);
                $("#signin-password").focus();
                //ReloadCaptcha();
                $("#signin-password").val('');
                return false;
            }
            else if (result.status == 7) {
                var row = '<div class="  col-md-12 p-1" role="alert">Account Blocked.Kindly Contact to Admin</div>';
                $("#message").empty();
                $("#message").append(row);
                $("#password").val('');
                //ReloadCaptcha();
                $("#signin-password").val('');
                return false;
            }
            else if (result.status == 8) {
                var row = '<div class="  col-md-12 p-1" role="alert">Account already Logged in</div>';
                $("#message").empty();
                $("#message").append(row);
                $("#password").val('');
                //ReloadCaptcha();
                $("#signin-password").val('');
                return false;
            }
            else {
                var row = '<div class="  col-md-12 p-1" role="alert">Please enter valid credentials</div>';
                $("#message").empty();
                $("#message").append(row);
                $("#email").val('');
                $("#password").val('');
                $("#signin-password").val('');
                //ReloadCaptcha();
                return false;
            }
        },
        error: function (error) {
            alert("Login Failed");
            //console.log(error);
        }
    });
}
//Send OPT During Login Time
function SendLoginEmailOTP(textbox, username, mobile) {
    //$("$message1").empty();
    //$("$message1").hide();
    //$("#email, #password, #signin-password, #mobileotp").on('input', function () {
    //    $("#message").empty();
    //    row = '';
    //});
    $("#loaderrr").show();
    $("#btnsubmit").attr('disabled', true);
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
            var mobileNumber = response.MobileNo;
            var lastTwoDigits = mobileNumber.slice(-2);
            var StartingTwoNumber = mobileNumber.slice(0,2);
            var formattedMobile = StartingTwoNumber+"xxxxxx" + lastTwoDigits;  
            var atIndex = Email.indexOf('@');
            if (atIndex !== -1) {
                var localPart = Email.slice(0, atIndex);
                var domainPart = Email.slice(atIndex);
                var remainingChars = localPart.length - 4; // Calculate the number of characters to replace
                if (remainingChars > 0) {
                    var xChars = '*'.repeat(remainingChars); // Create a string of 'x' characters of the calculated length
                    localPart = localPart[0] + localPart[1] + xChars + localPart[localPart.length - 2] + localPart[localPart.length - 1];
                }
                var formattedEmail = localPart + domainPart; 
                var span = $("#lblemail .enterddata");
                span.text("Please enter the OTP sent to " + formattedEmail + " and " + formattedMobile);
                slide('next');
                $("#loaderrr").hide();
                $("#password").val('');
                $('#signin-password').val('');
                var timerDuration = 60;
                var display = $('#resmobotp');
                startTimer(timerDuration, display);
            }

        },
        error: function (error) {
            ////console.log(error);
        }
    });
}
function SendLoginEmailResendOTP(textbox, username, mobile) {

    $("#email, #password, #signin-password, #mobileotp").on('input', function () {
        $("#message").empty();
        row = '';
    });
    clearInterval(intervalId);
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
                var mobileNumber = response.MobileNo;
                var lastTwoDigits = mobileNumber.slice(-2);
                var StartingTwoNumber = mobileNumber.slice(0, 2);
                var formattedMobile = StartingTwoNumber + "xxxxxx" + lastTwoDigits;
                var formattedEmail = localPart + domainPart;
                //var formattedMobile = "xxxxx" + "xxx00";

                var span = $("#lblemail .enterddata");
                span.text("Please enter the OTP sent to " + formattedEmail + " and " + formattedMobile);
                //slide('next');
                $("#password").val('');
                $('#signin-password').val('');
                var timerDuration = 60;
                var display = $('#resmobotp');
                startTimer(timerDuration, display);
            }

        },
        error: function (error) {
            ////console.log(error);
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
            //console.log(response);
        },
        error: function (error) {
            //console.log(error);
        }
    });
}
function validations() {
    var emailRegex = /.+\@.+\..+/;
    var mobileRegex = /^[6789]\d{9}$/;
    $("#email, #password, #signin-password, #mobileotp").on('input', function () {
        $("#message").empty();
        row = '';
    });

    if ($("#email").val().trim() === "") {
        row = '<div class=" col-md-12 p-1" role="alert">Please enter your email or mobile number</div>';
        $("#message").empty();
        $("#message").append(row);
        $("#email").focus();
        isvalidate = 0;
        return false;
    }

    else if (!emailRegex.test($("#email").val()) && !mobileRegex.test($("#email").val())) {
        row = '<div class=" col-md-12 p-1" role="alert">Please enter a valid  email or mobile number</div>';

        $("#message").empty();
        $("#message").append(row);
        $("#email").focus();
        isvalidate = 0;
        return false;
    }

    else if ($("#password").val().trim() === "") {
        $("#message").empty();
        //row = '<div class=" col-md-12 p-1" role="alert">Password is Required.</div>';
        row = 'Please enter your password';
        $("#message").append(row);
        $("#password").focus();
        isvalidate = 0;
        return false;
    }
    else if (!/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}/.test($("#password").val())) {
        //var row = '<div class=" col-md-12 p-1" role="alert">Password must contain one lowercase letter, one uppercase letter, one numeric digit, at least 8 characters, and one special character</div>';
        row = 'Password must contain one lowercase letter; one uppercase letter;  one numeral; one special character and at least 8 characters';
        $("#message").empty();
        $("#message").append(row);
        $("#password").focus();
        isvalidate = 0;
        return false;
    }
    else if ($('#signin-password').val().trim() === "") {
        $("#message").empty();
        //row = '<div class=" col-md-12 p-1" role="alert">Please Verify CAPTCHA.</div>';
        row = 'Please Verify CAPTCHA.';
        $("#message").append(row);
        $("#signin-password").focus();
        isvalidate = 0;
        return false;
    }
    else {
        isvalidate = 1;
    }
}
//During Login Verify OTP
function VerifyOTP() {
    //$("#email, #password, #signin-password, #mobileotp").on('input', function () {
    //    $("#message").empty();
    //    row = '';
    //});

    var otp = $("#mobileotp").val();
    if (otp === "") {
        $("#message1").empty();
        //row = '<div class=" col-md-12 p-1" role="alert" >Please enter the OTP</div>';
        row = '<div style="color: red;" class="col-md-12 p-1" role="alert">Please enter the OTP</div>';
        $("#message1").append(row);
        $("#mobileotp").focus();
        return false;
    }
    var isVerified = 0;

    $.ajax({
        type: 'POST',
        url: '/Login/VerifyOTP',
        dataType: 'json',
        data: {
            VOTP: otp,
        },
        async: false, // Make the request synchronous to wait for the response
        success: function (result) {
            if (result === 1) {
                $("#mobileotp").val('');
                //sessionStorage.setItem('UserId', result.UserId);
                $('#overlay').css('display', 'block');
                $('#loader').css('display', 'block');
                window.location.href = "/Dashboard/Index2";
                isVerified = 1;
            }

            else if (result === 0) {
                $("#message1").empty();
                row = '<div style="color: red;" class=" col-md-12 p-1" role="alert">The OTP has expired. Please generate a new OTP to continue</div>';
                $("#message1").append(row);
                $("#mobileotp").focus();
                return false;
            }
            else if (result === 2) {
                $("#message1").empty();
                row = '<div style="color: red;" class=" col-md-12 p-1" role="alert">Please enter a valid  OTP</div>';
                $("#message1").append(row);
                $("#mobileotp").focus();
                return false;
            }
        },
        error: function (ex) {
            //console.log("Error occurred during OTP verification");
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
            sessionStorage.clear();
            window.location.href = '/Login/Index';
        },
        error: function (error) {
            //console.log(error);
        }
    });
}
function ForgotPassword() {
    var email = $("#emailresetpassword").val();
    var captchaInput = $("#signin-password").val();

    if (email.trim() === '') {
        $("#message").empty();
        var row = '<div class=" col-md-12 p-1" role="alert">Please enter your registered email address.</div>';
        $("#emailresetpassword").focus();
        $("#message").append(row);
        return false;
    }

    var emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    if (!emailRegex.test(email)) {
        $("#message").empty();
        var row = '<div class=" col-md-12 p-1" role="alert">Invalid Email Format.</div>';
        $("#emailresetpassword").focus();
        $("#message").append(row);
        return false;
    }

    if (captchaInput.trim() === '') {
        $("#message").empty();
        var row = '<div class=" col-md-12 p-1" role="alert">Please Verify CAPTCHA.</div>';
        $("#signin-password").focus();
        $("#message").append(row);
        return false;
    }

    else {
        $("#loaderrr").show();
        $.ajax({
            url: '/Login/ResetPassword',
            type: 'POST',
            data: {
                Email: email,
                captchaInput: captchaInput
            },
            success: function (result) {
                if (result.StatusCode === 9) {
                    SendPasswordResetLink(result.UserCode, result.UserEmail);
                }
                else if (result.StatusCode === 10) {
                    $("#message").empty();
                    var row = '<div class="  col-md-12 p-1" role="alert">Email Id not Exist</div>';
                    $("#emailresetpassword").focus();
                    $("#message").append(row);
                    $("#signin-password").val('');
                    return false;
                }

                else if (result.success === false) {
                    $("#message").empty();
                    var row = '<div class="  col-md-12 p-1" role="alert">Incorrect Captcha.</div>';
                    $("#message").append(row);
                    $("#signin-password").val('').focus();
                    return false;
                }
                else if (result.status == "Email/Mobile can't Empty" || result.status == "Invalid Email format" || result.status == "Please enter captcha" || result.status == "Invalid captcha input") {
                    var row = '<div class="  col-md-12 p-1" role="alert">' + result.status + '</div>';
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
                //console.log(error);
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
            $("#loaderrr").hide();
            $('#successpopup1').modal('show');
           // window.location.href = '/Login/Index';
            sessionStorage.clear();
            //console.log(response);
        },
        error: function (error) {
            //console.log(error);
        }
    });
}
function onlyNumbers(event) {
    var charCode = event.charCode || event.keyCode;
    if (charCode < 48 || charCode > 57 || charCode == 46 || charCode == 45) {
        event.preventDefault();
    }
}