$(document).ready(function () {
    var UserMasterID = sessionStorage.getItem('UserId');
    if (UserMasterID == "" || UserMasterID == null) {
        window.location.href = "/Login/Index";
    }

    $('#btnInsert').click(function () {
        if (isvaliddata() === false) {
            return false;
        } else {
            $('#confirmationpopup').modal('show');
        }
    });
    $('#btnsave').click(function () {
        InsertDepartment();
    });
    
    if (getUrlParameter('ID') != '') {
        RowClickEventHandler();
    }
    
});

var getUrlParameter = function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
};

function InsertDepartment() {
    /////
    $("#DepartmentCode, #DepartmentName").on('input', function () {
        $("#message").empty();
        row = '';
    })
    var IsActive = $("#chkIsActive").is(":checked");
    //
        $.ajax({
            type: 'POST',
            url: '/Masters/DeptInsert',
            data: {
                DepartmentId: $('#DepartmentId').val(),
                DepartmentCode: $("#DepartmentCode").val(),
                DepartmentName: $("#DepartmentName").val(),
                Description: $("#Description").val(),
                IsActive: IsActive
            },
            success: function (result) {
                if (result.status = "200") {
                    $("#DepartmentCode").val('');
                    $("#DepartmentName").val('');
                    $("#Description").val('');
                    window.location.href = '../Masters/AllDepartments';
                    GetData();
                }
            },
            error: function (ex) {
                alert("Error");
            }
        });
    



}

function isvaliddata() {
    var ValidatorFor = [];
    ValidatorFor.push(["DepartmentCode", "Required", "", "Please enter Department Code"]);
    ValidatorFor.push(["DepartmentCode", "DepartmentCode", "", "Maximum Length is 20, No space allowed"]);
    ValidatorFor.push(["DepartmentName", "Required", "", "Please enter Department Name"]);
    ValidatorFor.push(["DepartmentName", "DepartmentName", "", "Please enter a valid alphabetic value"]);
    var status = ValidateMe(ValidatorFor);
    if (status == false) {
        return false;
    }
    else {
        return true;
    }
}



function RowClickEventHandler() {
    //
    var IsActive = $("#chkIsActive").is(":checked");
    var departmentid = getUrlParameter('ID');
    $.ajax({
        url: '/Masters/GetUserData',
        type: 'POST',
        dataType: 'json',
        data: {
            Departmentid: departmentid,
        },
        async: false,
        success: function (result) {
            //
            $('#DepartmentCode').val(result.DepartmentCode);
            $('#DepartmentName').val(result.DepartmentName);
            $('#Description').val(result.Description);
            $('#DepartmentId').val(departmentid);
            IsActive: IsActive
        },
        error: function () {
            alert('Something Went Wrong');
        }
    });
}
