function ValidateMe(ValidatorFor) {
    var status = true;
    var row = '';
    ////
    $.each(ValidatorFor, function (key, value) {
        if (value[1] == "Required") {
            $("#message").empty();
            if ($("#" + value[0]).val() == value[2]) {
                row += '<div class="alermsg col-md-12 p-1" role="alert">' + (value[3]) + '</div>';
                $("#message").append(row);
                //alert(value[3]);
                status = false;
                $("#" + value[0]).focus();
                //$('html, body').animate({
                //    scrollTop: $("#" + value[0]).offset().top - 30
                //}, 1000);
                return status;
            }
        }
        else if (value[1] == "Email") {
            $("#message").empty();
            if (!validateEmail($("#" + value[0]).val().trim())) {
                row += '<div class="alermsg col-md-12 p-1" role="alert">' + (value[3]) + '</div>';
                $("#message").append(row);
                //alert(value[3]);
                status = false;
                $("#" + value[0]).focus();
                //$('html, body').animate({
                //    scrollTop: $("#" + value[0]).offset().top - 30
                //}, 1000);
                return status;
            }
        }
        else if (value[1] == "panNumber")
        {
            $("#message").empty();
            if (!validatePAN($("#" + value[0]).val().trim())) {
                row += '<div class="alermsg col-md-12 p-1" role="alert">' + (value[3]) + '</div>';
                $("#message").append(row);
                //alert(value[3]);
                status = false;
                $("#" + value[0]).focus();
                //$('html, body').animate({
                //    scrollTop: $("#" + value[0]).offset().top - 30
                //}, 1000);
                return status;
            }

        }
        else if (value[1] == "Password") {
            $("#message").empty();
            if (!validatePassword($("#" + value[0]).val().trim())) {
                row += '<div class="alermsg col-md-12 p-1" role="alert">' + (value[3]) + '</div>';
                $("#message").append(row);
                //alert(value[3]);
                status = false;
                $("#" + value[0]).focus();
                //$('html, body').animate({
                //    scrollTop: $("#" + value[0]).offset().top - 30
                //}, 1000);
                return status;
            }
        }
        else if (value[1] == "Phoneno") {
            $("#message").empty();
            if (!validatePhoneno($("#" + value[0]).val().trim())) {
                row += '<div class="alermsg col-md-12 p-1" role="alert">' + (value[3]) + '</div>';
                $("#message").append(row);
                //alert(value[3]);
                status = false;
                $("#" + value[0]).focus();
                //$('html, body').animate({
                //    scrollTop: $("#" + value[0]).offset().top - 30
                //}, 1000);
                return status;
            }

        }
        else if (value[1] == "State") {
            $("#message").empty();
            if (!validatePhoneno($("#" + value[0]).val().trim())) {
                row += '<div class="alermsg col-md-12 p-1" role="alert">' + (value[3]) + '</div>';
                $("#message").append(row);
                //alert(value[3]);
                status = false;
                $("#" + value[0]).focus();
                //$('html, body').animate({
                //    scrollTop: $("#" + value[0]).offset().top - 30
                //}, 1000);
                return status;
            }

        }
        else if (value[1] == "CorpName")
        {
            $("#message").empty();
            if (!validateCorp($("#" + value[0]).val().trim())) {
                row += '<div class="alermsg col-md-12 p-1" role="alert">' + (value[3]) + '</div>';
                $("#message").append(row);
                //alert(value[3]);
                status = false;
                $("#" + value[0]).focus();
                //$('html, body').animate({
                //    scrollTop: $("#" + value[0]).offset().top - 30
                //}, 1000);
                return status;
            }


        }
        else if (value[1] == "DepartmentCode")
        {
            $("#message").empty();
            if (!validateDeptcode($("#" + value[0]).val().trim())) {
                row += '<div class="alermsg col-md-12 p-1" role="alert">' + (value[3]) + '</div>';
                $("#message").append(row);
                //alert(value[3]);
                status = false;
                $("#" + value[0]).focus();
                //$('html, body').animate({
                //    scrollTop: $("#" + value[0]).offset().top - 30
                //}, 1000);
                return status;
            }
        }
            
       
        else if (value[1] == "DepartmentName") {
            $("#message").empty();
            if (!validateDeptName($("#" + value[0]).val().trim())) {
                row += '<div class="alermsg col-md-12 p-1" role="alert">' + (value[3]) + '</div>';
                $("#message").append(row);
                //alert(value[3]);
                status = false;
                $("#" + value[0]).focus();
                //$('html, body').animate({
                //    scrollTop: $("#" + value[0]).offset().top - 30
                //}, 1000);
                return status;
            }
        }
        else if (value[1] == "Checkbox") {
            var dataChack = "";
            $('input:checkbox.' + value[0]).each(function () {
                var sThisVal = (this.checked ? $(this).val() : "");
                if (sThisVal != "") {
                    dataChack = sThisVal;
                }
            });
            if (dataChack == "") {
                alert(value[3]);
                status = false;
                $("#" + value[0]).focus();
                //$('html, body').animate({
                //    scrollTop: $("#" + value[2]).offset().top - 30
                //}, 1000);
                return status;
            }
        }
    });
    return status;
}
function ClearMe(ValidatorFor) {
    $.each(ValidatorFor, function (key, value) {
        if (value[1] == "Required") {
            $("#" + value[0]).val(value[2]);
        }
    });
}
function validateName($name) {
    var nameReg = /^[a-zA-Z]{1,}$/;
    return nameReg.test($name);
}
function validateEmail($email) {
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    return emailReg.test($email);
}
function validatePAN($pan) {
    var panReg = /^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$/;
    return panReg.test($pan);
}

function validateCorp($corpname) {
    /*var corpReg = /^[a-zA-Z0-9-@.{}#&!()]+(\s[a-zA-Z0-9-@{}.#&!()]+)+(\s[a-zA-Z-@.#&!()]+)?$/;*/
    var corpReg = /[a-zA-Z]{6,}/;
    return corpReg.test($corpname);
}
///Validations For Department insertion

function validateDeptcode($DepartmentCode) {
    /*var corpReg = /^[a-zA-Z0-9-@.{}#&!()]+(\s[a-zA-Z0-9-@{}.#&!()]+)+(\s[a-zA-Z-@.#&!()]+)?$/;*/
    var deptcodereg = /^(?!\s)[^\s]{1,20}(?<!\s)$/;
    return deptcodereg.test($DepartmentCode);
}
function validateDeptName($Deptname) {
    var DeptnameReg = /^[a-zA-Z\s]+$/;
    return DeptnameReg.test($Deptname);
}

////////////TO GET CHECKBOX DATA
$('input:checkbox.Whichcredentialdoyoucurrentlyhold').each(function () {
    var sThisVal = (this.checked ? $(this).val() : "");
});
$(document).on("input", ".numeric", function () {
    this.value = this.value.replace(/\D/g, '');
});

function validatePassword($pass) {
    var passReg = /^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{7,15}$/;
    return passReg.test($pass);
}
function validatePhoneno($phone) {
    var phoneReg = /^[6-9]\d{9}$/;
    return phoneReg.test($phone);
}
function validateState($state) {
    var stateReg = /^(Andhra Pradesh|Arunachal Pradesh|Assam|Bihar|Chhattisgarh|Goa|Gujarat|Haryana|Himachal Pradesh|Jharkhand|Karnataka|Kerala|Madhya Pradesh|Maharashtra|Manipur|Meghalaya|Mizoram|Nagaland|Odisha|Punjab|Rajasthan|Sikkim|Tamil Nadu|Telangana|Tripura|Uttar Pradesh|Uttarakhand|West Bengal)$/;
    return stateReg.test($state);
}

  