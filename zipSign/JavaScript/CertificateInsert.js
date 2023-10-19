var IsValidate = 0;
var pagecount = 1;
var keyword;
$(document).ready(function () {
    GetDataForUserGrid(pagecount, keyword);
});

function UploadImages(FileUploader) {
    var fileInput = document.getElementById('SignImage');
    var file = fileInput.files[0];
    var allowedExtensions = [".p12", ".pfx", "p12", "pfx"];
    var fileExtension = file.name.split('.').pop().toLowerCase();
    if (!allowedExtensions.includes(fileExtension)) {
        alert("Only .pfx and .p12 certificates are allowed.");
        fileInput.value = '';
        return false;
    }
    $("#CertType").val(fileExtension);
    $("#CertType").attr("disabled", true);
    var fileData = new FormData();
    fileData.append("HelpSectionImages", file);
    $.ajax({
        url: '/CertificateManagement/UploadDocument',
        type: "POST",
        contentType: false,
        processData: false,
        data: fileData,
        success: function (result) {

            sessionStorage.setItem('CertPath', result.status);
        },
        error: function (err) {
            console.log(err);
        }
    });
}

function validatepassword() {
    $("#password").siblings(".alermsg").remove();
    $("#confirmpassword").siblings(".alermsg").remove();
    var password = $("#password").val();
    var confirm_password = $("#confirmpassword").val();
    if (password === "") {
        $("#password").siblings(".alermsg").remove();
        row = '<div class="alermsg col-md-12 p-1" role="alert">Please Enter Password</div>';
        $("#password").after(row).focus();
        return false;
    }
    else if (confirm_password === "") {
        $("#confirmpassword").siblings(".alermsg").remove();
        row = '<div class="alermsg col-md-12 p-1" role="alert">Please Enter Confirm Password</div>';
        $("#confirmpassword").after(row).focus();
        return false;
    }
    var certificate = sessionStorage.getItem('CertPath')
    if (password === confirm_password) {
        $.ajax({
            url: '/CertificateManagement/ValidateCertWithPassword',
            type: "POST",
            async: false,
            dataType: "text",
            data: {
                certificateforvalidate: certificate,
                password: password
            },
            success: function (result) {

                if (result === "True") {
                    IsValidate = 1;
                    return true;
                } else {
                    IsValidate = 0;
                    return false;
                }
            },
            error: function (err) {
                IsValidate = 0;
                return false;
            }
        });
        return IsValidate;
    }

    else {
        alert("Password is not matching.");
        IsValidate = 0;
        return false;
    }
}

function SaveCertificate() {
    var CertName = $("#CertName").val();
    $("#CertName").siblings(".alermsg").remove();
    if (CertName === "") {
        $("#CertName").siblings(".alermsg").remove();
        row = '<div class="alermsg col-md-12 p-1" role="alert">Please Enter Certificate Name</div>';
        $("#CertName").after(row).focus();
        return false;
    }

    if (!$("#password").val() || !$("#CertType").val() || !$("#CertName").val()) {
        alert('Please fill out all the required fields.');
        return;
    }
    else if (!validatepassword()) {
        alert('Password is not valid.');
        return;
    }
    else {
    var password = $("#password").val();
    var certificatePath = sessionStorage.getItem('CertPath');
    var CertType = $("#CertType").val();
    var CertName = $("#CertName").val();
    var checkedRadioButtonId = $("input[name='control']:checked").attr('id');
    var selectedText = $('#AccesUserType option:selected').text();
    var selectedRowsJSON = JSON.stringify(selectedRows);

    $.ajax({
        url: '/CertificateManagement/SaveCertificate',
        type: "POST",
        dataType: "json",
        contentType: 'application/json',
        data: JSON.stringify({
            CertificateName: CertName,
            CertificateType: CertType,
            UploadedBy: "1",
            Path: certificatePath,
            password: password,
            PasswordType: checkedRadioButtonId,
            Role: selectedText,
            Table: selectedRowsJSON
        }),
        success: function (result) {

        },
        error: function (err) {
            console.log(err);
        }
    });
    }
}
//GridView_Bind_For_User_To_Give_Roles
function GetDataForUserGrid(pagecount, keyword) {
    $("#myGrid1").html("");
    var columnDefs = [
        { headerName: '', field: '', width: 40, resizable: false, sortable: true, suppressMovable: true, checkboxSelection: true, headerCheckboxSelection: true, },
        { headerName: 'Sr No.', field: 'Userid', width: 80, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'User Name', field: 'Username', width: 300, resizable: false, sortable: true, suppressMovable: true, },
        { headerName: 'Email ID', field: 'EmailId', width: 300, resizable: false, sortable: true, suppressMovable: true, },
        { headerName: 'Mobile No.', field: 'MobileNo', width: 270, resizable: false, sortable: true, suppressMovable: true, },
    ];
    var rowData = [];
    $.ajax({
        url: '/CertificateManagement/SearchDataForUsers',
        type: 'POST',
        dataType: 'json',
        data: {
            pagecount: pagecount,
            keyword: keyword
        },
        success: function (result) {
            var jsonData = result.Table1;
            $.each(jsonData, function (i, value) {
                rowData.push({
                    Userid: value.Userid,
                    UserCode: value.UserCode,
                    Username: value.Username,
                    EmailId: value.EmailId,
                    MobileNo: value.MobileNo,
                    UserType: value.UserType,
                  
                });
            });
            var gridOptions = {
                columnDefs: columnDefs,
                rowData: rowData,
                pagination: false,
                onRowClicked: function (params) {
                    if (params.event.target.id === 'EditData') {
                        Click(params.data.DepartmentId);
                    } else {
                        ShowMore(params.data.UserCode);
                    }
                },
                onSelectionChanged: function () {
                    selectedRows = gridOptions.api.getSelectedRows();
                },

                rowSelection: 'multiple',
            };

            var gridDiv = document.querySelector('#myGrid1');
            new agGrid.Grid(gridDiv, gridOptions);
        },
        error: function () {
            alert('Something went wrong');
        }
    });
}


function Search() {
    var keyword = $('#searchInput').val(); // Retrieve the keyword from the search input field
    var pagecount = 1; // Reset the page count to 1 after performing a search

    GetData(pagecount, keyword); // Call the modified GetData function with the keyword
}
function ShowMore(UserCode) {
    $.ajax({
        url: '/Users/GetUserDetails',
        type: 'POST',
        dataType: 'json',
        data: {
            UserCode: UserCode
        },
        success: function (result) {

            var jsonData = result.Table1;

            if (jsonData.length > 0) {
                var UserDetails = jsonData[0];
                $('#usercode').text(UserDetails.UserCode);
                $('#username').text(UserDetails.Username);
                $('#usertype').text(UserDetails.UserType);
                $('#emailid').text(UserDetails.EmailId);
                $('#mobile').text(UserDetails.MobileNo);
                $('#department').text(UserDetails.Department);
                $('#designation').text(UserDetails.Designation);
                $('#active').text(UserDetails.Active);
                $('#createdby').text(UserDetails.CreatedBy);
                $('#createdon').text(UserDetails.CreatedOn);
                $('#updatedby').text(UserDetails.ModifyBy);
                $('#updatedon').text(UserDetails.ModifyOn);
                $('#mobileapp').text(UserDetails.Mobileapp);
                $('#status').text(UserDetails.Active);
                $('#domaincontrol').text(UserDetails.SpecificDomaincontrol);
            }
        },
        error: function () {
            alert('Failed to retrieve department details.');
        }
    });
}