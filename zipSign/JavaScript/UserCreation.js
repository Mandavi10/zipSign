var row = '';
var i = 1;
$(document).ready(function () {


    $('#overlay').css('display', 'block');
    $('#loader').css('display', 'block');
    var UserMasterID = sessionStorage.getItem('UserId');
    if (UserMasterID == "" || UserMasterID == null) {
        window.location.href = "/Login/Index";
    }
    $('#overlay').css('display', 'none');
    $('#loader').css('display', 'none');
    $("#UserCode, #UserName,#Email,#Phoneno,#text-input5").on('input', function () {
        $("#message").empty();
        row = '';
    });

    $("#specificDomainCheckbox").click(function () {
        if ($(this).is(":checked")) {
            $("#specificDomainFields").show();
            createNewDiv();
        } else {
            $("#specificDomainFields").hide();
        }
    });

    $(".actionbtn").click(function () {
        $(".appendinp").show();

    });

    $(".crossfrimg").click(function () {
        $(".appendinp").hide();
    });
    var ddl = document.getElementById("ddldesig");
    $.ajax({
        url: "/Users/DeptDdl",
        type: "get",
        data: {},
        success: function (data) {
            $.each(data, function (index, element) {
                var option = document.createElement("option");
                option.text = element.Text;
                option.value = element.Value;
                ddl.add(option);
            });
        },
        error: function (eror) {
        }
    });

    $('#btnsave').click(function () {
        //;
        if (isValidData() == false) {
            return false;
        }
        else {
            $('#confirmationpopup').modal('show');
        }
    });

    $('#ModalOk').click(function () {
        UserInsert();
        window.location.href = "/Users/AllUsers";
    });
});

function UserInsert() {
    //;
    if (isValidData() == false) {
        return false;
    }
    else {
        var isCheckedSpecificDomain = $("#specificDomainCheckbox").is(":checked");
        var domainName = "";
        var CombineSpecificDomain = '';
        if (isCheckedSpecificDomain) {
            domainName = "SpecificDomain";
            CombineSpecificDomain = $("#SpecificDomainContainer input[type='text']").map(function () {
                return $(this).val();
            }).get().join(", ");
        } else {
            domainName = "All";
        }
        var active = $("#chkactive").is(":checked");
        var mobile = $("#chkmob").is(":checked");
        $("#message").empty();
        $.ajax({
            type: 'POST',
            url: '/Users/UserInsert',
            dataType: 'json',
            data: {
                UserCode: $('#UserCode').val(),
                Username: $('#UserName').val(),
                EmailId: $('#Email').val(),
                MobileNo: $('#Phoneno').val(),
                UserType: $('#ddlType :selected').text(),
                Department: $('#ddldesig :selected').text(),
                Designation: $('#text-input5').val(),
                CreatedBy: $('#caldob').val(),
                ModifyBy: $('#gst').val(),
                Active: active,
                Mobileapp: mobile,
                Domain: domainName,
                SpecificDomaincontrol: CombineSpecificDomain
            },

            success: function (result) {
                //;
                if (result.status == 1) {
                    window.location.href = "/Users/AllUsers";
                } else if (result.status == 2) {
                    $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Duplicate User Code.</div>');
                    return false;
                } else if (result.status == 3) {
                    $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Duplicate Email Exists.</div>');
                    return false;
                } else if (result.status == 4) {
                    $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Duplicate Mobile Number Exists.</div>');
                    return false;
                }
            }

        });

    }
}

function isValidData() {
    var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    var phonePattern = /^[6789]\d{9}$/;
    var userCodePattern = /^[A-Za-z0-9]+$/;
    var namePattern = /^[A-Za-z\s]+$/;
    var domainPattern = /@\w+(\.\w+)+/;
    var email = $('#Email').val();
    var phoneNo = $('#Phoneno').val();
    var userCode = $('#UserCode').val();
    var name = $('#UserName').val();
    if (userCode.trim() === '') {
        $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Please Enter User Code</div>');
        $("#UserCode").focus();
        return false;
    } else if (!userCodePattern.test(userCode)) {
        $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Invalid User Code Format</div>');
        $("#UserCode").focus();
        return false;
    }
    if (name.trim() === '') {
        $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Please enter Name</div>');
        $("#UserName").focus();
        return false;
    } else if (!namePattern.test(name)) {
        $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Invalid Name Format. Name can only contain alphabets and spaces.</div>');
        $("#UserName").focus();
        return false;
    }

    if (email.trim() === '') {
        $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Please enter Email</div>');
        $("#Email").focus();
        return false;
    } else if (!emailPattern.test(email)) {
        $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Invalid Email Format</div>');
        $("#Email").focus();
        return false;
    }

    if (phoneNo.trim() === '') {
        $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Please enter Mobile</div>');
        $("#Phoneno").focus();
        return false;
    } else if (!phonePattern.test(phoneNo)) {
        $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Invalid Phone Number Format</div>');
        $("#Phoneno").focus();
        return false;
    }

    //if (isCheckedSpecificDomain) {
    //    var isValidDomain = true;
    //    $("#SpecificDomainContainer input[type='text']").each(function () {
    //        var inputValue = $(this).val();
    //        if (!domainPattern.test(inputValue)) {
    //            isValidDomain = false;
    //            return false;
    //        }
    //    });

    //    if (!isValidDomain) {
    //        $("#message").html('<div class="alermsg col-md-12 p-1" role="alert">Invalid domain format. Please enter valid domains!</div>');
    //        return false;
    //    }
    //}
    else {
        return true;
    }
}


$("#AddMoreBtn").click(function () {
    //AddMore();
    createNewDiv();

});
function createNewDiv() {
    newDiv += '<div class="col-md-4 position-relative mt-1 appendinp">';
    newDiv += '<input type="text" class="form-control inputofnumbers" id="specificDomainName' + i + '">';
    newDiv += '<i class="fa fa-trash crossfrimg delbtn" id="btndelete" onclick="remove(this)"></i>';
    newDiv += '</div>';
    var i = parseInt($("#hidddiv").val()) + 1;
    var newDiv = '';
    newDiv += '<div class="col-md-4 position-relative mt-1 appendinp">';
    newDiv += '<input type="text" class="form-control inputofnumbers" id="specificDomainName' + i + '">';
    newDiv += '<i class="fa fa-trash crossfrimg delbtn" id="btndelete" onclick="remove(this)"></i>';
    newDiv += '</div>';
  
    $("#hidddiv").val(i);
    $('#SpecificDomainContainer').append(newDiv);

}


function remove(button) {
    $(button).closest("div.col-md-4.position-relative.mt-1.appendinp").remove();
}
