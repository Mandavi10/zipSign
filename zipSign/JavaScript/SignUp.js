var OTP1 = OTP1;
var row = '';
var mobileverified = 0;
var EmailVerify = 0;
var MessageShown = false;
var mobileTimerIntervalId;
var isMobileTimerRunning = false;
var emailTimerIntervalId;
var isEmailTimerRunning = false;
$(document).ready(function () {

    $('#password, #ConfirmPassword').on('paste copy', function (event) {
        event.preventDefault();
    });
    $("#UserName, #Email, #Phoneno, #password,#Email, #panNumber,#CorpName,#ConfirmPassword,#emailOTP").on('input', function () {
        $("#message").empty();
        row = '';
    });
    $("#State").on('change', function () {
        $("#message").empty();
    });
    function resetForm() {
        $('.form')[0].reset();
    }
    $(window).on('unload', function () {
        resetForm();
    });
    $(window).on('popstate', function (event) {
        resetForm();
    });
    $("#btnmobotp").attr('disabled', 'disabled');
    $("#Phoneno").attr('disabled', 'disabled'); 
    $('#OnlySigner').change(function () {
        if ($(this).is(':checked')) {
            $('#btnSign').prop('disabled', false);
        } else {
            $('#btnSign').prop('disabled', true);
        }
    });
    //startTimer().hide();
    //$(".verifybtn").click(function () {
    //    if ($("#UserName").val() === "") {
    //        $("#message").empty();
    //        row = '<div class="col-md-12 p-1" role="alert">Enter User Name</div>';
    //        $("#message").append(row);
    //        $("#UserName").focus();
    //        return false;
    //    }
    //    else if ($("#Email").val() === "") {
    //        $("#message").empty();
    //        row = '<div class="col-md-12 p-1" role="alert">Please enter Email</div>';
    //        $("#message").append(row);
    //        $("#Email").focus();
    //        return false;
    //    }
    //    else if (!/^[\w\.\-]+@[a-zA-Z\d\-]+(\.[a-zA-Z]+)*\.[a-zA-Z]{2,}$/.test($("#Email").val())) {
    //        row = '<div class="col-md-12 p-1" role="alert">You have entered an invalid email address! (e.g. mailto:xxxx@gmail.com)</div>';
    //        $("#message").empty().append(row);
    //        $("#Email").focus();
    //        return false;
    //    }
    //    else {
    //        //$(".enterotpdiv").show();
    //        $(".verifybtn").hide();
    //        //$(".timer").show();
    //    }
    //});

    //$(".verifybtn1").click(function () {
    //    $("#UserName, #Email, #Phoneno, #password,#Email, #panNumber,#CorpName,#ConfirmPassword").on('input', function () {
    //        $("#message").empty();
    //        row = '';
    //    });
    //    $("#State").on('change', function () {
    //        $("#message").empty(); // Clear the message when a state is selected
    //    });
    //    if ($("#UserName").val() === '') {

    //        row += '<div class="col-md-12 p-1" role="alert">Please enter name of the user</div>';
    //        $("#message").empty().append(row);
    //        $("#UserName").focus();
    //        return false;
    //        MessageShown = true;
    //    }
    //    else if ($("#Phoneno").val() === "") {
    //        row = '<div class="col-md-12 p-1" role="alert">Please enter mobile number</div>';
    //        $("#message").empty().append(row);
    //        $("#Phoneno").focus();
    //        return false;
    //    } else if (!/^[6-9]\d{9}$/.test($("#Phoneno").val())) {
    //        row = '<div class="col-md-12 p-1" role="alert">Please enter a valid 10-digit mobile number starting with 6, 7, 8, or 9</div>';
    //        $("#message").empty().append(row);
    //        $("#Phoneno").focus();
    //        return false;
    //    }
    //    else {
    //        $(".verifybtn1").hide();
    //    }
    //});

    $('#btnSign').click(function () {
        if (isValidData() == false) {
            return false;
        }
        else {
            $('#confirmationpopup').modal('show');
        }
    });

    $('#btnsave').click(function () {
        SignUp();
    });

    $('#btnok').click(function () {
        window.location.href = "/Dashboard/Index2";
    });
});
$('input[type="radio"]').click(function () {
    if ($(this).attr('id') == 'corpradio') {
        var anyInputFilled = $("#UserName").val() != '' || $("#Email").val() != '' || $("#Phoneno").val() != '' || $("#password").val() != '' || $("#ConfirmPassword").val() != '';
        if (anyInputFilled) {
            $('#confirmationpopup1').modal('show');
            $('#btnsave1').click(function () {
                ClearOnSwitchCheckBox();
                $('#confirmationpopup1').modal('hide');
                $('.corporatediv').show();
            });
            $('#cancel').click(function () {
                $("#indivradio").prop('checked', true);
                $('#confirmationpopup1').modal('hide');
            });
        } else {
            $('.corporatediv').show();
        }
    } else {
        //$('.corporatediv').hide();
        var anyInputFilled = $("#CorpName").val() != '' || $("#UserName").val() != '' || $("#Email").val() != '' || $("#Phoneno").val() != '' || $("#password").val() != '' || $("#ConfirmPassword").val() != '';
        if (anyInputFilled) {
            $('#confirmationpopup2').modal('show');
            $('#btnsave2').click(function () {
                ClearOnSwitchCheckBox();
                $('#confirmationpopup2').modal('hide');
                $('.corporatediv').hide();
            });
            $('#cancel1').click(function () {
                $("#corpradio").prop('checked', true);
                $('#confirmationpopup2').modal('hide');
            });
        } else {
            $('.corporatediv').hide();
        }
    }
});

//Timer during SignUP click on verify mobile & Resend 
function startTimer(duration, display, ismobile) {
    if (isMobileTimerRunning) {
        clearInterval(mobileTimerIntervalId);
    }
    isMobileTimerRunning = true;

    var timer = duration, minutes, seconds;
    mobileTimerIntervalId = setInterval(function () {
        minutes = parseInt(timer / 60, 10);
        seconds = parseInt(timer % 60, 10);
        minutes = minutes < 10 ? "0" + minutes : minutes;
        seconds = seconds < 10 ? "0" + seconds : seconds;
        display.text(minutes + ":" + seconds);
        $('#lblsmsresend').hide();
        if (--timer < 0) {
            clearInterval(mobileTimerIntervalId);
            $('#lblsmsresend').show();
            $("#timer3").hide();
        }
    }, 1000);
}

// Timer during signup Email Verify
function startTimer1(duration, display, isemail) {
    $('#lblemailresend').hide();
    if (isEmailTimerRunning) {
        clearInterval(emailTimerIntervalId);
    }
    var timer1 = duration, minutes, seconds;
    isEmailTimerRunning = true;
    emailTimerIntervalId = setInterval(function () {
        minutes = parseInt(timer1 / 60, 10);
        seconds = parseInt(timer1 % 60, 10);
        minutes = minutes < 10 ? "0" + minutes : minutes;
        seconds = seconds < 10 ? "0" + seconds : seconds;
        display.text(minutes + ":" + seconds);
        //$('#lblemailresend').hide();
        if (--timer1 < 0) {
            clearInterval(emailTimerIntervalId);
            $('#lblemailresend').show();
            $('#timeremail').hide();
            //$('#emailOTP').val('');
            //$(".enterotpdiv2").hide();
            //  startTimer1().hide();
        }
    }, 1000);
}
//Resend OTP on mail during signup 
$('#lblemailresend').click(function () {
    $("#message").empty();
    if (emailTimerIntervalId) {
        clearInterval(emailTimerIntervalId);
    }
    var timerDuration = 60;
    var display = $('#timeremail');
    isTimerRunning = true;
    startTimer1(timerDuration, display, isEmailTimerRunning);
    SendEmailOTP();
    $(".enterotpdiv").show();
    $(".verifybtn").hide();
    $("#emailOTP").val('');
})
$('#lblsmsresend').click(function () {
    $("#message").empty();
    var timerDuration = 60;
    var display = $('#timer3');
    startTimer(timerDuration, display, isMobileTimerRunning);
    SendMobileOTP();
    $(".enterotpdiv1").show();
    $(".verifybtn1").hide();
})
function SignUp() {
    var row = '';
    $("#UserName, #Email, #Phoneno, #password,#Email, #panNumber,#CorpName,#ConfirmPassword").on('input', function () {
        $("#message").empty();
        row = '';
    });
    $("#State").on('change', function () {
        $("#message").empty(); // Clear the message when a state is selected
    });

    var UserType = $('input[name="selectone"].selectonerad:checked').val();
    $.ajax({
        type: 'POST',
        url: '/Login/SignUp',
        dataType: 'json',
        data: {
            Name: $("#UserName").val(),
            CorpName: $("#CorpName").val(),
            Email: $("#Email").val(),
            UserType: UserType,
            Mobile: $("#Phoneno").val(),
            MobileOTP: $("mobileotp").val(),
            State: $("#State").val(),
            Password: $("#password").val(),
            ConfirmPassword: $("#ConfirmPassword").val(),
            panNumber: $("#panNumber").val(),
        },
        success: function (result) {
            if (result.status == "Validation failed") {
                $.each(result.errors, function (index, error) {
                    row += '<div class="col-md-12 p-1" role="alert">' + error + '</div>';
                    $('#confirmationpopup').modal('hide');
                });
            } else if (result.status == "Enter Name" || result.status == "Invalid email format" || result.status == "Mobile Number should be 10 Digits and only starts with 6/7/8/9" || result.status == "Please select state" || result.status == "Password must contain one lowercase letter, one uppercase letter, one numeric digit, at least 8 characters, and one special character" || result.status == "Invalid Confirm password format" || result.status == "Password and Confirm Password do not match" || result.status == "Invalid PAN format") {
                row += '<div class="col-md-12 p-1" role="alert">' + result.status + '</div>';
                $('#confirmationpopup').modal('hide');

            } else if (result.status == 1) {
                $("#UserName").val('');
                $("#CorpName").val('');
                $("#Email").val('');
                $("#Phoneno").val('');
                $("#State").empty();
                $("#password").val('');
                $("#ConfirmPassword").val('');
                $("#panNumber").val('');
                $('#successpopup').modal('show');
                $('#confirmationpopup').modal('hide');
            } else if (result.status == 2) {
                $("#message").empty();
                row += '<div class="col-md-12 p-1" role="alert">PAN Already Registered</div>';
                $("#message").append(row);
                $("#Email").focus();
                $('#successpopup').modal('hide');
                $('#confirmationpopup').modal('hide');
                return false;
                MessageShown = true;
            } else if (result.status == 4) {
                $("#message").empty();
                row += '<div class="col-md-12 p-1" role="alert">E-Mail Already Exists</div>';
                $("#message").append(row);
                $("#Email").focus();
                $('#Email').removeAttr('disabled');
                EmailVerify = 0;
                $(".verifybtn").show();
                $(".afterverify").hide();
                $("#emailOTP").val('');
                $('#successpopup').modal('hide');
                $('#confirmationpopup').modal('hide');
                return false;
                MessageShown = true;
            } else if (result.status == 3) {
                $("#message").empty();
                row += '<div class="col-md-12 p-1" role="alert">Account with this mobile number already exists. Please use a different one.</div>';
                $("#message").append(row);
                $("#Phoneno").focus();
                $('#Phoneno').removeAttr('disabled');
                mobileverified = 0;
                $(".afterverify1").hide();
                $('#successpopup').modal('hide');
                $('#confirmationpopup').modal('hide');
                $(".verifybtn1").show();
                $("#mobileotp").val('');
                return false;
                MessageShown = true;s
            }

            // Append error messages to the message container
            $("#message").empty().append(row);
        },
        error: function (ex) {
            // Handle AJAX errors
        }
    });
}
//Send OTP on Mobile during SignUp
function SendMobileOTP() {
    if ($("#UserName").val() ==="") {
        $("#message").empty();
        row = '<div class="col-md-12 p-1" role="alert">Please Enter User Name</div>';
        $("#message").append(row);
        $("#UserName").focus();
        return false;
    }
    else if (/[0-9!@#$%^&*(),.?":{}|<>]/.test($("#UserName").val())) {
        row = '<div class="col-md-12 p-1" role="alert">Please enter a valid name</div>';
        $("#message").empty().append(row);
        $("#UserName").focus();
        return false;
    }
    else if ($("#Email").val().trim() === "") {
        row = '<div class="col-md-12 p-1" role="alert">Please enter email address</div>';
        $("#message").empty().append(row);
        $("#Email").focus();
        return false;
    }
    var row = '';
    var ValidatorFor = []
    ValidatorFor.push(["Phoneno", "Required", "", "Please enter mobile number"]);
    ValidatorFor.push(["Phoneno", "Phoneno", "", "Please enter a valid 10-digit mobile number starting with 6, 7, 8, or 9"]);
    var status = ValidateMe(ValidatorFor);
    if (status == false) {
        return false;
    }
    if ($("#Phoneno").val() == '') {
        $("#message").empty();
        row += '<div class="col-md-12 p-1" role="alert">Please Enter Mobile Number</div>';
        $("#message").append(row);
        $("#Phoneno").focus();
        return false;
    }
    //else if ($("#UserName").val() == '') {
    //    $("#message").empty();
    //    row += '<div class="col-md-12 p-1" role="alert">Please Enter User Name</div>';
    //    $("#message").append(row);
    //    $("#UserName").focus();
    //}

    else {
        $("#loaderrr").show();
        
        $.ajax({
            url: '/Login/GetSMSData',
            type: 'POST',
            data: {
                CusName: $("#UserName").val(),
                MobileNo: $("#Phoneno").val()
            },
            success: function (response) {

                var mobile = $("#Phoneno").val();
                var start2digit = mobile.slice(0, 2);
                var formattedMobile = start2digit+"xxxxxx" + mobile.slice(-2);
                var span = $("#lblmobile .enterddata");
                span.text("Please enter the OTP sent to " + formattedMobile);
                console.log(response);
                $('#overlay').css('display', 'block');
                $('#loader').css('display', 'block');
                var timerDuration = 60;
                var display = $('#timer3');
                $('#mobileotp').show();
                $('#verifyBtn').show();
                //$('#timer3').hide();
                $('#Phoneno').attr('disabled', 'disabled');
                startTimer(timerDuration, display, isMobileTimerRunning);
                $("#lblsmsresend").hide();
                $(".verifybtn1").hide();
                $("#timer3").show();
                $(".timer1").show();
                $("#loaderrr").hide();
                $(".enterotpdiv1").show();
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}
//During Signup Send otp on Email(clicked on verify)
function SendEmailOTP() {
    
    if ($("#UserName").val() === "") {
        $("#message").empty();
        row = '<div class="col-md-12 p-1" role="alert">Please enter name of the user</div>';
        $("#message").append(row);
        $("#UserName").focus();
        return false;
    }
    else if (/[0-9!@#$%^&*(),.?":{}|<>]/.test($("#UserName").val())) {
        row = '<div class="col-md-12 p-1" role="alert">Please enter a valid name</div>';
        $("#message").empty().append(row);
        $("#UserName").focus();
        return false;
    }
    var ValidatorFor = [];
    ValidatorFor.push(["Email", "Required", "", "Please enter email address"]);
    ValidatorFor.push(["Email", "Email", "", "Please enter a valid email address"]);
    var status = ValidateMe(ValidatorFor);
    if (status === false) {
        $(".enterotpdiv").attr("style", "display:none");
        return false;
    }
    
    else {
        $("#loaderr").show();
        $.ajax({
            url: '/Login/GetEmailData',
            type: 'POST',
            data: {
                Email: $("#Email").val()
            },
            success: function (response) {
                var Email = $("#Email").val();
                var atIndex = Email.indexOf('@');
                if (atIndex !== -1) {
                    var localPart = Email.slice(0, atIndex);
                    var domainPart = Email.slice(atIndex);

                    var remainingChars = localPart.length - 4; // Calculate the number of characters to replace

                    if (remainingChars > 0) {
                        var xChars = '*'.repeat(remainingChars); // Create a string of 'x' characters of the calculated length
                        localPart = localPart[0] + localPart[1] + xChars + localPart[localPart.length - 2] + localPart[localPart.length -1 ];
                    }
                    var formattedEmail = localPart + domainPart;
                    var span = $("#lblemail .enterddata");
                    span.text("Please enter the OTP sent to " + formattedEmail);
                    var timerDuration = 60;
                    var display = $('#timeremail');
                    startTimer1(timerDuration, display, isEmailTimerRunning);
                    $('#timeremail').show();
                    $(".timer").show();
                    $(".enterotpdiv").show();
                    $(".verifybtn").hide();
                    $("#Email").prop("disabled", true);
                    $(".spinload").hide();
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}
function VerifyOTP() {
    var otp = $("#mobileotp").val();
    if (otp=== "") {
        $("#message").empty();
        row = '<div class="col-md-12 p-1" role="alert">Please enter the OTP</div>';
        $("#message").append(row);
        $("#UserName").focus();
        return false;
    }
    $.ajax({
        type: 'POST',
        url: '/Login/VerifyMobileOTPForSignUp',
        dataType: 'json',
        data: {
            VOTP: otp
        },
        async: false, // Make the request synchronous to wait for the response
        success: function (response) {
            var result = response;
            if (result === 1) {
                $("#message").empty();
                $("#formattedMobile").hide();
                $('#divmobotp').hide();
                $('#Phoneno').attr('disabled', 'disabled');
                $('#Phoneno').css('border-style', 'solid').css('border-color', 'green');

                $('.afterverify1').show();
                $('.enterotpdiv1').hide();
                $("#verifyBtn").hide();
                $('#mobileotp').hide();
                mobileverified = 1;
            }
            else if (result === 0) {
                $("#message").empty();
                row = '<div class="col-md-12 p-1" role="alert">OTP Expired</div>';
                $("#message").append(row);
                $("#mobileotp").focus();
                mobileverified = 0;
                MessageShown = true;
            }
            else if (result === 2) {
                $("#message").empty();
                row = '<div class="col-md-12 p-1" role="alert">Please enter the valid OTP</div>';
                $("#message").append(row);
                $("#mobileotp").focus();
                mobileverified = 0;
                MessageShown = true;
            }
            else {
                $("#message").empty();
                row = '<div class="col-md-12 p-1" role="alert">Please enter the valid OTP</div>';
                $("#message").append(row);
                $("#mobileotp").focus();
                mobileverified = 0;
                MessageShown = true;
            }
        },
        error: function (ex) {
            console.log("Error occurred during OTP verification");
            mobileverified = 0;
        }
    });
}
function VerifyEmailOTP() {
    $("#message").empty();
    var EmailOTP = $("#emailOTP").val();
    if (EmailOTP === "") {
        $("#message").empty();
        row = '<div class="col-md-12 p-1" role="alert">Please enter the OTP</div>';
        $("#message").append(row);
        $("#emailOTP").focus();
        return false;
    }
    var otp = EmailOTP
    var isVerified = false;
    var row = '';
    $.ajax({
        type: 'POST',
        url: '/Login/VerifyEmailOTP',
        dataType: 'json',
        data: {
            VOTP: otp
        },
        async: false,
        success: function (response) {
            result = response;
            if (result === 1) {
                $("#message").empty();
                $('.enterotpdiv2').css('display', 'none');
                $("#formattedEmail").hide();
                $("#enterotpdiv2").hide();
                $(".verifybtn1").removeAttr('disabled');
                $("#Phoneno").removeAttr('disabled');
                $('.timer mt-1').hide();
                $('#Email').attr('disabled', 'disabled');
                $('#Email').css('border-style', 'solid').css('border-color', 'green');
                $(".afterverify").show();
                $("#resendemailotp").hide();
                $("#lblemailresend").hide();
                $('#lblotp').hide();
                EmailVerify = 1;
                isVerified = true;
            }
            else if (result === 2) {
                $("#message").empty();
                row += '<div class="col-md-12 p-1" role="alert">Please enter the valid OTP</div>';
                $("#message").append(row);
                $("#emailOTP").focus();
                EmailVerify = 0;
                isVerified = false;
                MessageShown = true;
            }
            else if (result === 0) {
                $("#message").empty();
                row += '<div class="col-md-12 p-1" role="alert">The OTP has expired. Please generate a new OTP to continue</div>';
                $("#message").append(row);
                $("#emailOTP").focus();
                EmailVerify = 0;
                isVerified = false;
                MessageShown = true;
            }
            else {
                $("#message").empty();
                row += '<div class="col-md-12 p-1" role="alert">Please enter the OTP</div>';
                $("#message").append(row);
                $("#emailOTP").focus();
                EmailVerify = 0;
                isVerified = false;
                MessageShown = true;
            }
        },
        error: function (ex) {
            console.log("Error occurred during OTP verification");
            EmailVerify = 0;
            isVerified = false;

        }
    });

    return isVerified;
}
function isValidData() {
    var row = '';
    $("#UserName, #Email, #Phoneno, #password,#Email, #panNumber,#CorpName,#ConfirmPassword").on('input', function () {
        $("#message").empty();
        row = '';
    });
    $("#State").on('change', function () {
        $("#message").empty(); // Clear the message when a state is selected
    });

    //$("#btnmobotp, #btnmailotp").on('click', function () {
    //    $("#message").empty();
    //    row = '';
    //});

    var UserType = $('input[name="selectone"].selectonerad:checked').val();

    if (UserType != 'individual') {
        if ($("#CorpName").val().trim() === "") {
            row = '<div class="col-md-12 p-1" role="alert">Please Enter Corporate Name</div>';
            $("#message").empty().append(row);
            $("#CorpName").focus();
            return false;
        }
        else if (!/^.{6,}$/.test($("#CorpName").val())) {
            row = '<div class="col-md-12 p-1" role="alert">Corporate Name Should be Minimum 6 Characters</div>';
            $("#message").empty().append(row);
            $("#CorpName").focus();
            return false;
        }
    }

    if ($("#UserName").val().trim() === "") {
        row = '<div class="col-md-12 p-1" role="alert">Please enter name of the user</div>';
        $("#message").empty().append(row);
        $("#UserName").focus();
        return false;
    }
    else if (/[0-9!@#$%^&*(),.?":{}|<>]/.test($("#UserName").val())) {
        row = '<div class="col-md-12 p-1" role="alert">Please enter a valid name</div>';
        $("#message").empty().append(row);
        $("#UserName").focus();
        return false;
    }

    if ($("#Email").val().trim() === "") {
        row = '<div class="col-md-12 p-1" role="alert">Please enter email address</div>';
        $("#message").empty().append(row);
        $("#Email").focus();
        return false;
    } else if (!/^[\w\.\-]+@[a-zA-Z\d\-]+(\.[a-zA-Z]+)*\.[a-zA-Z]{2,}$/.test($("#Email").val())) {
        row = '<div class="col-md-12 p-1" role="alert">Please enter a valid email address</div>';
        $("#message").empty().append(row);
        $("#Email").focus();
        return false;
    }
    if (EmailVerify == 0) {
        row = '<div class="col-md-12 p-1" role="alert">Please verify the email</div>';
        $("#message").empty().append(row);
        $("#emailOTP").focus();
        return false;
    } else if ($("#emailOTP").val().trim() === "") {
        row = '<div class="col-md-12 p-1" role="alert">Please Enter Email OTP</div>';
        $("#message").empty().append(row);
        $("#emailOTP").focus();
        return false;
    }
    if ($("#Phoneno").val().trim() === "") {
        row = '<div class="col-md-12 p-1" role="alert">Please enter mobile number</div>';
        $("#message").empty().append(row);
        $("#Phoneno").focus();
        return false;
    } else if (!/^[6-9]\d{9}$/.test($("#Phoneno").val())) {
        row = '<div class="col-md-12 p-1" role="alert">Please enter a valid 10-digit mobile number starting with 6, 7, 8, or 9</div>';
        $("#message").empty().append(row);
        $("#Phoneno").focus();
        return false;
    }
    if (mobileverified == 0) {
        row = '<div class="col-md-12 p-1" role="alert">Please verify the mobile number</div>';
        $("#message").empty().append(row);
        return false;
    } else if ($("#mobileotp").val().trim() === "") {
        row = '<div class="col-md-12 p-1" role="alert">Please enter the OTP</div>';
        $("#message").empty().append(row);    
        $("#mobileotp").focus();
        return false;
    }
    if ($("#State").val() === "Select State") {
        var row = '<div class="col-md-12 p-1" role="alert">Please select your state</div>';
        $("#message").empty().append(row);
        $("#State").focus();
        return false;
    }

    if ($("#password").val().trim() === "") {
        row = '<div class="col-md-12 p-1" role="alert">Please enter a password</div>';
        $("#message").empty().append(row);
        $("#password").focus();
        return false;
    } else if (!/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}/.test($("#password").val())) {
        row = '<div class="col-md-12 p-1" role="alert">Password must contain one lowercase letter; one uppercase letter;  one numeral; one special character and at least 8 characters</div>';
        $("#message").empty().append(row);
        $("#password").focus();
        return false;
    }

    if ($("#ConfirmPassword").val().trim() === "") {
        row = '<div class="col-md-12 p-1" role="alert">Please enter confirm password</div>';
        $("#message").empty().append(row);
        $("#ConfirmPassword").focus();
        return false;
    }

    if ($("#password").val().trim() !== $("#ConfirmPassword").val().trim()) {
        row = '<div class="col-md-12 p-1" role="alert">Password and confirm password do not match</div>';
        $("#message").empty().append(row);
        $("#ConfirmPassword").focus();
        return false;
    }
    return true;
}
$(window).load(function () {
    $('form').get(0).reset();
})
function restartEmailTimer() {
    startTimer(60, $('#timeremail'), false);
    isEmailTimerRunning = true;
}
function restartMobileTimer() {
    startTimer(60, $('#timer3'), true);
    isMobileTimerRunning = true;               
}
function stopEmailTimer() {
    clearInterval(emailTimerIntervalId);
    isEmailTimerRunning = false;
    $('#lblemailresend').show();
    $('#timeremail').hide();
}
function stopMobileTimer() {
    clearInterval(mobileTimerIntervalId);
    isMobileTimerRunning = false;
    $('#lblsmsresend').show();
    $('#timer3').hide();
}
function ClearOnSwitchCheckBox() {
    stopMobileTimer();
    stopEmailTimer();
    $("#UserName, #CorpName, #Email, #Phoneno, #password, #ConfirmPassword").val('');
    $("#message").empty();
    $('#timeremail').hide();
    $('#timer3').hide();
    $('#lblsmsresend').hide();
    $('#lblemailresend').hide();
    $('.enterotpdiv1').hide();
    $('.afterverify1').hide();
    $('.enterotpdiv2').hide();
    $('.afterverify').hide();
    $('#divmobotp').hide();
    $('#mobileotp').val('');
    $('#emailOTP').val('');
    $('#Phoneno').css('border-style', '').css('border-color', '');
    $('#Email').css('border-style', '').css('border-color', '');
    $('#Email').removeAttr('disabled', true);
    $("#btnmobotp").attr('disabled', 'disabled'); 
    $("#Phoneno").attr('disabled', 'disabled');
    //span.text('');
    mobileverified = 0;
    EmailVerify = 0;
    $(".verifybtn").show();
    $(".verifybtn1").show();
}
function onlyNumbers(event) {
    var charCode = event.charCode || event.keyCode;
    if (charCode < 48 || charCode > 57 || charCode == 46 || charCode == 45) {
        event.preventDefault();
    }
}