var pagecount = 1;
var row = '';
var isFileSelected = '';
var filePathss = '';
var SignerType = '';
var UpoladedId = '';
var keyword = $('#searchInput').val();
var i;
$(document).ready(function () {
    
    //DeleteOldFilesUptoCustomDays();
    var UserMasterID = sessionStorage.getItem('UserId');
    if (UserMasterID == "" || UserMasterID == null) {
        window.location.href = "/Login/Index";
    }
 
    var userDataString = sessionStorage.getItem('user_data');
    $('#SendLink').click(function () {
        if (isValidData() == false) {
            $('#successpopup').modal('hide');
            return false;
            //$("#RecipientDiv").hide();
        }
        else {
            $('#successpopup').modal('show');
        }
    });

    $('#btnok').click(function () {
        SignInsert();
        window.location.href = "/Dashboard/Index2";
    });
    $("#SendLink").hide();
    $("#Proceed").hide();
    $("#SendLink").hide();
    $("#Name, #Email, #phone, #ExpDate,#Email, #text-input1,#SignImage,#ExpDate").on('input', function () {
        $("#message").empty();
        row = '';
    });
    $('#viewFile').attr('disabled', true);
    $("#Proceed").hide();
    $("#SendLink").hide();
    $("#OnlySigner").val('');
    $("#AddRecipient").val('');
    $("#viewrec").hide();
    GetData(pagecount, keyword)
    $('.suggestions-list').hide();
    $("#viewrec").click(function () {
        updateGridData();
    });
});
var suggestionsList = $(".suggestions-list");
$("#AddRecipient").click(function () {
    AddRecipiants(1);
    $("#SendLink").show();
    $("#Proceed").hide();
    $(".numdiv").show();
    $(".reccol1").show();
    $(".recipientcheck").show();
});
$("#OnlySigner").click(function () {
    SignerType = "Single_Signer";
    sessionStorage.setItem('Single_Signer', SignerType);
    $("#Proceed").show();
    $("#SendLink").hide();
    $(".numdiv").hide();
    $(".reccol1").hide();
    $(".recipientcheck").hide();
});
function addRecipientData() {
    $("#Name, #Email, #phone, #ExpDate,#Email, #text-input1,#SignImage,#ExpDate").on('input', function () {
        $("#message").empty();
        row = '';
    });
    recipientsData = [];
    var AddRValue = parseInt($("#hiddena").val());
    for (var i = 1; i <= AddRValue; i++) {
        var Name = $('#Name' + i).val();
        var Email = $('#Email' + i).val();
        var Mobile = $('#phone' + i).val();
        var recipient = {
            DocumentUploadId: i,
            SignerName: Name,
            SignerEmail: Email,
            SignerMobile: Mobile
        };
        recipientsData.push(recipient);
    }
}
$("#Proceed").click(function () {
    var checkedRadioButtonId = $('input[name="control"]:checked').attr('id');
    if (isValidData() == false) {
        return false;
    }
    else {
        $("#Proceed").val(filePathss);
        SignInsert();
    }
    $(".closesignerdiv").click(function () {
    });
});

$("#viewrec").click(function () {
    updateGridData();
});
function isValidData() {

    var checkedRadioButtonId = $('input[name="control"]:checked').attr('id');
    $("#Name, #Email, #phone, #ExpDate,#Email, #text-input1,#SignImage,#ExpDate").on('input', function () {
        $("#message").empty();
        row = '';
    });
    $("#message").empty();
    if (checkedRadioButtonId == "AddRecipient") {
        var isDuplicate = false;
        var uniqueValues = {};
        $('input[id^="Email"], input[id^="phone"]').each(function () {
            var value = $(this).val().trim();
            if (value !== "") {
                if (uniqueValues[value]) {
                    isDuplicate = true;
                    $(this).focus();
                    return false;
                }
                uniqueValues[value] = true;
            }
        });
    }

    if (isDuplicate) {
        var row = '<div class="col-md-12 p-1" role="alert">Duplicate email or phone number found.</div>';
        $("#message").append(row);
        return false;
    }

    if ($("#text-input1").val().trim() === '') {
        var row = '<div class="col-md-12 p-1" role="alert">Please Enter Document Name</div>';
        $("#message").append(row);
        $("#text-input1").focus();
        return false;
    }
    if ($('#SignImage').get(0).files.length === 0) {
        var row = '<div class="col-md-12 p-1" role="alert">Please Select A File</div>';
        $("#message").append(row);
        $("#SignImage").focus();
        return false;
    }
    if (checkedRadioButtonId == "AddRecipient") {
        var ValidatorFor = [];
        ValidatorFor.push(["DocName", "Required", "Please enter Document Name"]);
        ValidatorFor.push(["DocName", "Required", "Please enter Document Name"]);
        ValidatorFor.push(["DocName", "Required", "Please enter Document Name"]);
        var i = $("#hiddena").val();
        for (var j = 1; j <= i; j++) {
            var nameField = $("#Name" + j).val().trim();
            var emailField = $("#Email" + j).val().trim();
            var phoneField = $("#phone" + j).val().trim();
            var expiryday = $("#ExpDate" + j).val().trim();
            if (nameField === "") {
                var row = '<div class="col-md-12 p-1" role="alert">Recipient ' + j + ' name should not be empty.</div>';
                $("#message").append(row);
                $("#Name" + j).focus();
                return false;
            }

            if (emailField === "") {
                var row = '<div class="col-md-12 p-1" role="alert">Recipient ' + j + ' email should not be empty.</div>';
                $("#message").append(row);
                $("#Email" + j).focus();
                return false;
            }
            else if (!/^[\w\.\-]+@[a-zA-Z\d\-]+(\.[a-zA-Z]+)*\.[a-zA-Z]{2,}$/.test(emailField)) {
                row = '<div class="col-md-12 p-1" role="alert">You have entered an invalid email address! (e.g. mailto:xxxx@gmail.com)</div>';
                $("#message").append(row);
                $("#Email").focus();
                return false;
            }

            if (phoneField === "") {
                var row = '<div class="col-md-12 p-1" role="alert">Recipient ' + j + ' mobile no. should not be empty.</div>';
                $("#message").append(row);
                $("#phone" + j).focus();
                return false;
            }
            else if (!/^[6-9]\d{9}$/.test(phoneField)) {
                row = '<div class="col-md-12 p-1" role="alert">Mobile Number should be 10 Digits and only starts with 6/7/8/9</div>';
                $("#message").append(row);
                $("#Phoneno").focus();
                return false;
            }
            if (expiryday === "") {
                var row = '<div class="col-md-12 p-1" role="alert">Recipient ' + j + ' Expiry day should not be empty.</div>';
                $("#message").append(row);
                $("#ExpDate" + j).focus();
                return false;
            }
            else if (!/^[0-9]+$/.test(expiryday)) {
                row = '<div class="col-md-12 p-1" role="alert">Give expiry days in day</div>';
                $("#message").append(row);
                $("#ExpDate" + j).focus();
                return false;
            }

            ValidatorFor.push(["Name" + j, "Required", "", "Please enter Recipient " + j + " name"]);
            ValidatorFor.push(["Email" + j, "Required", "", "Please enter Recipient " + j + " Email"]);
            ValidatorFor.push(["Email" + j, "Email", "", "Please enter Recipient " + j + " valid email-id"]);
            ValidatorFor.push(["phone" + j, "Required", "", "Please enter Recipient " + j + " Mobile no."]);
            ValidatorFor.push(["ExpDate" + j, "Required", "", "Please enter Expiry Day " + j + ""]);
        }
        var status = ValidateMe(ValidatorFor);
        if (status == false) {
            return false;
        } else {
            return true;
        }


    }
}
function UploadImages(FileUploader, Preview, ColumnName) {
    $("#Name, #Email, #phone, #ExpDate,#Email, #text-input1,#SignImage,#ExpDate").on('input', function () {
        $("#message").empty();
        row = '';
    });
    event.preventDefault();

    var fileReference = "";
    var allowedExtensions = ["pdf"];
    var fileInput = document.getElementById('SignImage');
    var file = fileInput.files[0];
    var fileExtension = file.name.split('.').pop().toLowerCase();
    if (!allowedExtensions.includes(fileExtension)) {
        // File extension validation
        $("#message").empty();
        var row = '<div class="col-md-12 p-1" role="alert">Only PDF files are allowed.</div>';
        $("#message").append(row);
        fileInput.value = '';
        return false;
    } else {
        $("#message").empty();
    }
    var reader = new FileReader();
    reader.onload = function (e) {
        $("#" + Preview).attr("src", e.target.result);
        $("#document-preview-frame").attr("src", e.target.result); // Set the src attribute of the iframe
        var fileName = $("#" + FileUploader)[0].files[0].name;
        var fileSize = formatBytes(file.size);
        sessionStorage.setItem('uploadedFileName', fileName);
        sessionStorage.setItem('uploadedFileSize', fileSize);
        sessionStorage.setItem('uploadedFileData', e.target.result);
    };
    reader.readAsDataURL($("#" + FileUploader)[0].files[0]);
    if (window.FormData !== undefined) {
        var fileUpload = $("#" + FileUploader).get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        for (var i = 0; i < files.length; i++) {
            fileData.append("HelpSectionImages", files[i]);
        }
        $.ajax({
            url: '/zipSign/UploadFiles',
            type: "POST",
            contentType: false,
            processData: false,
            data: fileData,
            success: function (result) {
                //;
                $('#viewFile').attr('disabled', false);
                sessionStorage.setItem('UpoladedId', result.UploadId);
                $("#RemoveImage").css("display", "block");
                var currentDate = new Date();
                var day = String(currentDate.getDate()).padStart(2, '0');
                var month = String(currentDate.getMonth() + 1).padStart(2, '0'); // Months are zero-indexed, so add 1
                var year = currentDate.getFullYear();
                var hours = String(currentDate.getHours()).padStart(2, '0');
                var minutes = String(currentDate.getMinutes()).padStart(2, '0');
                var amOrPm = currentDate.getHours() >= 12 ? 'PM' : 'AM';
                var formattedDate = day + '/' + month + '/' + year + ' ' + hours + ':' + minutes + ' ' + amOrPm;
                sessionStorage.setItem('uploadedFileDate', formattedDate);
                var filePath = result.status;
                var LoaclPath = result.LocalPath;
                var FileName = result.uniquefileName;
                $("#hdnfilepath").val(filePath);
                //$("#" + ColumnID).val(filePath);
                filePathss = filePath;
                sessionStorage.setItem('uploadedFilePath', filePath);
                sessionStorage.setItem('LoaclPath', LoaclPath);
                var absoluteFilePath = result.status;
                var relativePath = absoluteFilePath.replace("D:\\Project\\ZipSign\\zipSign\\zipSign", ""); // Adjust this based on your project structure
                $("#hdnfilepath").val(relativePath);
                $("#Hdnfield").val(filePath);
                $("#" + Preview).attr("src", relativePath);
            },
            error: function (err) {
                console.log(err);
            }
        });
    } else {
        alert("Format is not supported.");
    }
}

function formatBytes(bytes) {
    if (bytes === 0) {
        return '0 Bytes';
    }
    var k = 1024;
    var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    var i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

var recipientsData = [];
$("#MoreRecipient").click(function () {
    AddRecipiants(2);
});

$(".suggestions-list").hide();
function AddRecipiants(a) {
    $("#Name, #Email, #phone, #ExpDate,#Email, #text-input1,#SignImage,#ExpDate").on('input', function () {
        $("#message").empty();
        row = '';
    });
    if (a === 1) {
        $("#RecipientDiv").empty();
        var i = 1;
        var Recipients = '<div class="d-flex flex-wrap">';
        Recipients += '<div class="col-md-1">';
        Recipients += '<div class="numdiv" id="SignerDiv' + i + '">Signer ' + i + '</div>';
        Recipients += '</div>';
        Recipients += '<div class="col-md-11">';
        Recipients += '<ul class="columns">';
        Recipients += '<li class="column reccol1">';
        Recipients += '<div class="col-md-12 signerdiv">';
        Recipients += '<div class="col-md-12 d-flex flex-wrap position-relative pe-4">';
        Recipients += '<div class="form-group col-md-4 col-12 p-1">';
        Recipients += '<input type="text" class="form-control" placeholder="Name" autocomplete="off" oninput="this.value = this.value.toUpperCase()" id="Name' + i + '">';
        Recipients += '<ul class="suggestions-list" style="position: absolute; width: 100%; z-index: 1; background-color: #fff; border: 1px solid #ccc; max-height: 150px; overflow-y: auto; margin-top: 5px; border-radius: 4px; display: none;"></ul>';
        Recipients += '</div>';
        Recipients += '<div class="form-group col-md-4 col-12 p-1">';
        Recipients += '<input type="text" class="form-control" placeholder="Email ID" autocomplete="off" oninput="this.value = this.value.toLowerCase()" id="Email' + i + '">';
        Recipients += '</div>';
        Recipients += '<div class="form-group col-md-2 col-12 p-1">';
        Recipients += '<input type="text" class="form-control" placeholder="Mobile Number" onkeypress="return onlyNumbers(event)" autocomplete="off" maxlength="10" id="phone' + i + '">';
        Recipients += '</div>';
        Recipients += '<div class="form-group col-md-1 col-12 p-1">';
        Recipients += '<input type="text" class="form-control" style="padding-left: 8px !important;" placeholder="Expired in Days" autocomplete="off" onkeypress="return onlyNumbers(event)" maxlength="3" id="ExpDate' + i + '" onchange="CheckExpiryDate()">';
        Recipients += '</div>';
        Recipients += '<div class="form-group col-md-1 col-12 p-1">';
        Recipients += '<label class="p-1">Days</label>';
        Recipients += '</div>';
        Recipients += '<i class="fa fa-trash-o delbtn" aria-hidden="true"></i>';
        Recipients += '</div>';
        Recipients += '</div>';
        Recipients += '</li>';
        Recipients += '</ul>';
        Recipients += '</div>';
        Recipients += '</div>';
        $("#RecipientDiv").append(Recipients);
        $("#hiddena").val(i);
        $("#viewrec").hide();
        RecipientEmail = $("#Email1").val();
        //SendLinkToRecipient(RecipientEmail);
    }
    else {

        var i = parseInt($("#hiddena").val()) + 1;
        var Recipients = '<div class="d-flex flex-wrap MySigndiv">';
        Recipients += '<div class="col-md-1">';
        Recipients += '<div class="numdiv">Signer ' + i + '</div>';
        Recipients += '</div>';
        Recipients += '<div class="col-md-11">';
        Recipients += '<ul class="columns">';
        Recipients += '<li class="column reccol1">';
        Recipients += '<div class="col-md-12 signerdiv">';
        Recipients += '<div class="col-md-12 d-flex flex-wrap position-relative pe-4">';
        Recipients += '<div class="form-group col-md-4 col-12 p-1">';
        Recipients += '<input type="text" class="form-control" placeholder="Name" oninput="this.value = this.value.toUpperCase()" autocomplete="off" id="Name' + i + '">';
        Recipients += '</div>';
        Recipients += '<ul class="suggestions-list" style="position: absolute; width: 100%; z-index: 1; background-color: #fff; border: 1px solid #ccc; max-height: 150px; overflow-y: auto; margin-top: 5px; border-radius: 4px; display: none;"></ul>';
        Recipients += '<div class="form-group col-md-4 col-12 p-1" id="SignerDiv">';
        Recipients += '<input type="text" class="form-control" placeholder="Email ID" autocomplete="off" oninput="this.value = this.value.toLowerCase()" id="Email' + i + '">';
        Recipients += '</div>';
        Recipients += '<div class="form-group col-md-2 col-12 p-1">';
        Recipients += '<input type="text" class="form-control" placeholder="Mobile Number" autocomplete="off" maxlength="10" id="phone' + i + '">';
        Recipients += '</div>';
        Recipients += '<div class="form-group col-md-1 col-12 p-1">';
        Recipients += '<input type="text" class="form-control" style="padding-left: 8px !important;" autocomplete="off" placeholder="Expired in Days" maxlength="3" id="ExpDate' + i + '" onchange="CheckExpiryDate()">';
        Recipients += '</div>';
        Recipients += '<div class="form-group col-md-1 col-12 p-1">';
        Recipients += '<label class="p-1">Days</label>';
        Recipients += '</div>';
        Recipients += '<i class="fa fa-trash-o delbtn" aria-hidden="true"></i>';
        Recipients += '</div>';
        Recipients += '</div>';
        Recipients += '</li>';
        Recipients += '</ul>';
        Recipients += '</div>';
        Recipients += '</div>';
        $("#hiddena").val(i);
        $("#RecipientDiv").append(Recipients);
        RecipientEmail = $("#Email" + i).val();

    }
    $(".delbtn").off().on("click", function () {
        $(this).closest("div.d-flex.flex-wrap.MySigndiv").remove();
        updateSignerNumbers();
        updateGridData();
    });
}
function updateSignerNumbers() {
    var signerDivs = $("div.d-flex.flex-wrap.MySigndiv");
    signerDivs.each(function (index) {
        var signerNumber = index + 2;
        $(this).find(".numdiv").text("Signer " + signerNumber);
    });
    //var i = i - 1;
    //$("#hiddena").val(i);
    var i = $("#hiddena").val();
    i = i - 1;
    $("#hiddena").val(i);
}
function storeRecipientsData() {
    recipientsData = [];
    for (var i = 1; i <= parseInt($("#hiddena").val()); i++) {
        recipientsData.push({
            DocumentUploadId: i,
            SignerName: $("#Name" + i).val(),
            SignerEmail: $("#Email" + i).val(),
            SignerMobile: $("#phone" + i).val(),
            expday: $("#ExpDate" + i).val(),
        });
    }
}

function updateGridData() {
    $("#Name, #Email, #phone, #ExpDate,#Email, #text-input1,#SignImage,#ExpDate").on('input', function () {
        $("#message").empty();
        row = '';
    });
    var columnDefs1 = [
        { headerName: 'Sr No.', field: 'DocumentUploadId', width: 80, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'Name', field: 'SignerName', width: 200, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'Email ID', field: 'SignerEmail', width: 300, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'Phone No.', field: 'SignerMobile', width: 260, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'Expire in Days', field: 'expday', width: 150, resizable: false, sortable: true, suppressMovable: true },
    ];
    var gridOptions1 = {
        columnDefs: columnDefs1,
        rowData: recipientsData,
        pagination: false,
        rowSelection: 'multiple',
    };

    var gridDiv = document.querySelector('#myGrid1');
    if (gridDiv.hasChildNodes()) {
        agGrid.Grid.destroy(gridDiv);
    }
    new agGrid.Grid(gridDiv, gridOptions1);
}

$("#viewrec").click(function () {
    storeRecipientsData();
    updateGridData();
});
function GetData(pagecount, keyword) {
    $("#myGrid").html("");
    var columnDefs = [
        { headerName: 'Sr. No.', field: 'Sr. No.', width: 80, sortable: true, resizable: false, suppressMovable: true, valueGetter: "node.rowIndex + 1" },
        { headerName: 'Uploaded Document Name', field: 'DocumentName', width: 210, resizable: false, sortable: true, suppressMovable: true, },
        { headerName: 'DocumentName', field: 'DocumentName', width: 150, resizable: false, sortable: true, suppressMovable: true, },
        { headerName: 'Status', field: 'SignStatus', width: 100, resizable: false, sortable: true, suppressMovable: true, },
        { headerName: 'Uploaded On', field: 'UploadedOn', width: 170, resizable: false, sortable: true, suppressMovable: true, },
        { headerName: 'Uploaded By', field: 'UploadedBy', width: 120, resizable: false, sortable: true, suppressMovable: true, },
        {
            headerName: 'Work History', field: 'vwh', width: 120, resizable: false, sortable: true, suppressMovable: true, cellRenderer: function (params) {
                return '<button type="button" class="ingridbtn" data-bs-toggle="modal" data-bs-target="#gridviewmodal">View</button>'
            }
        },
        {
            headerName: 'Action',
            field: '',
            width: 100,
            sortable: true,
            resizable: false,
            suppressMovable: true,
            cellRenderer: function (params) {
                return '<span class="fa fa-trash gridIcon" data-file-Code="' + params.data.DocumentUploadId + '"></span>';
            }
        },
    ];
    var rowData = [];
    //
    $.ajax({
        url: '/zipSign/SearchData1',
        type: 'POST',
        dataType: 'json',
        data: {
            pagecount: pagecount,
            keyword: keyword
        },
        success: function (result) {
            var jsonData = result.Table1;
            var jsonData1 = result.Table2;
            $.each(jsonData, function (i, value) {
                rowData.push({
                    DocumentUploadId: value.DocumentUploadId,
                    Uploaded_Document_Name: value.DocumentName.substring(value.DocumentName.lastIndexOf('\\') + 1),
                    DocumentName: value.DocumentName,
                    SignStatus: value.SignStatus,
                    UploadedOn: value.UploadedOn,
                    UploadedBy: value.UploadedBy,
                });
            });
            $("#Ddd").empty();
            var page = jsonData1[0].page;
            var size = jsonData1[0].size;
            var count = jsonData1[0].count;
            var startEntry = ((page - 1) * size) + 1;
            var endEntry = Math.min(startEntry + size - 1, count);
            $("#Ddd").append('Showing ' + startEntry + ' to ' + endEntry + ' of ' + count + ' entries');
            $('#pagination1').empty();
            var html1 = " ";
            var count = jsonData1[0].pagecount;
            var page = jsonData1[0].page;
            //
            html1 += "<ul class='pagination'>";
            if (parseInt(page) > 1) {
                //For Previous Count
                html1 += "<li class='page-item'><a href='#' class='page-link' onclick='GetData(" + (parseInt(page) - 1) + ")'>Previous</a></li>";
            } else {
                html1 += "<li class='page-item disabled'><a href='#' class='page-link'>Previous</a></li>";
            }
            //
            if (jsonData1.length >= 1) {
                var FirstPageNo = Math.max(pagecount - 4, 1);
                for (var i = FirstPageNo; i <= pagecount; i++) {
                    if (pagecount == i) {
                        //Page 1
                        html1 += "<li class='page-item active'><a href='#' class='page-link'>" + i + "</a></li>";
                    } else if (count >= i) {
                        html1 += "<li class='page-item'><a href='#' class='page-link' onclick='GetData(" + i + ")'>" + i + "</a></li>";
                    } else {
                        html1 += "<li class='page-item disabled'><a href='#' class='page-link'>" + i + "</a></li>";
                    }
                }
            }

            if (parseInt(page) < count) {
                html1 += "<li class='page-item'><a href='#' class='page-link' onclick='GetData(" + (parseInt(page) + 1) + ")'>Next</a></li>";
            } else {
                html1 += "<li class='page-item disabled'><a href='#' class='page-link'>Next</a></li>";
            }

            html1 += "</ul>";

            $("#pagination1").append(html1);

            var gridOptions = {
                columnDefs: columnDefs,
                rowData: rowData,
                pagination: false,
                onRowClicked: function (params) {
                    if (params.event.target.classList.contains('fa-trash')) {
                        var fileCode = params.event.target.getAttribute('data-file-Code');
                        Delete(fileCode);
                    }
                    else {
                        var fileCode = params.event.target.getAttribute('data-file-Code');
                        SendToSigningRequest(fileCode);
                    }
                },
                rowSelection: 'multiple',
            };

            var gridDiv = document.querySelector('#myGrid');
            new agGrid.Grid(gridDiv, gridOptions);
        },
        error: function () {
            alert('Something went wrong');
        }
    });
}

function Delete(DocumentUploadId) {
    $.ajax({
        url: '/zipSign/Delete',
        type: 'POST',
        dataType: 'json',
        data: {
            fileCode: DocumentUploadId
        },
        success: function (result) {
            GetData(pagecount, keyword);
        },
        error: function () {
            alert('Failed to delete the file.');
        }
    });
}


function SendToSigningRequest(fileCode) {
    $.ajax({
        type: 'POST',
        url: '/Login/VerifyMobile',
        dataType: 'json',
        data: {
            fileCode: fileCode
            
        },
        async: false,
        success: function (result) {

            if (result.Status == 1) {
                if (result.msg == 1) {
                    $("#successmsg1").hide();
                    window.location.href = "/zipSign/SigningRequest?UId=" + UID;

                } else {
                    $("#successmsg1").empty();
                    var row = '<div class="col-md-12 p-1" role="alert">Please Enter Correct OTP.</div>';
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
                    var row = '<div class="col-md-12 p-1" role="alert">Please Enter Correct OTP.</div>';
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



















function SendLinkToRecipient(UniqueSignerID, Email, SignerID, SignerName, UploadedDocumentId, SignerExpiry) {
    var FilePath = sessionStorage.getItem('LoaclPath');
    $.ajax({
        url: '/NSDL/SendVerifyLinkByEmail',
        type: "POST",
        data: {
            Email: Email,
            fileid: UniqueSignerID,
            SignerName: SignerName,
            SignerID: SignerID,
            UploadedDocumentId: UploadedDocumentId,
            FilePath: FilePath,
            SignerExpiry: SignerExpiry
        },
        success: function (result) {
            //alert('success');
        },
        error: function (err) {
            console.log(err);
        }
    });
}

function SignInsert() {
    var fileName = $("#SignImage").val();
    var cleanFileName = fileName.replace(/^.*\\/, "");
    console.log("DocumentName: " + cleanFileName);

    var isAddRecipient = $("#AddRecipient").prop("checked");

    if (isAddRecipient) {
        signerType = "Multiple Signers";
    } else {
        signerType = "Single Signer";
    }

    var recipientsData = [];
    $('.signerdiv').each(function (index) {

        var recipient = {
            Name: $(this).find('[id^="Name"]').val(),
            Email: $(this).find('[id^="Email"]').val(),
            MobileNumber: $(this).find('[id^="phone"]').val(),
            ExpireInDays: $(this).find('[id^="ExpDate"]').val(),
            signerType: signerType,
            DocumentExpiryDay: $("#ExpDate").val(),
        };
        recipientsData.push(recipient);
    });
    $.ajax({
        type: 'POST',
        url: '/zipSign/SignInsert',
        data: {
            DocumentName1: $("#text-input1").val(),
            ReferenceNumber: $("#text-input2").val(),
            DocumentName: cleanFileName,
            UploadedDoc: cleanFileName,
            filePath: $("#hdnfilepath").val(),
            signerInfos: recipientsData,
            UserType: signerType
        },
        success: function (result) {
            var UploadedDocumentId = result.UploadedDocumentId
            sessionStorage.setItem('UploadedDocumentId', UploadedDocumentId);
            sessionStorage.setItem('UniqueSignerID', UniqueSignerID);;
            if (result.UserType == "Single Signer") {
                var SignerID = result.SignerID;
                sessionStorage.setItem('SignerID', SignerID);
                var redirectUrl = "/zipSign/SigningRequest?UType=" + encodeURIComponent(result.UserType) + "&UploadedDocumentId=" + encodeURIComponent(UploadedDocumentId);
                window.location.href = redirectUrl;
            }
            else {
                var UploadedDocumentId = result.UploadedDocumentId;
                var UniqueSignerID = result.UniqueID;
                var Email = result.EmailToSend;
                var SignerID = result.SignerID;
                var SignerName = result.SignerName;
                var signerT = result.UserType;
                var ExpireInDays = result.SignerExpiry;
                //$("#successpopup").modal("show");
                SendLinkToRecipient(UniqueSignerID, Email, SignerID, SignerName, UploadedDocumentId, ExpireInDays);
            }
        },
        error: function (ex) {
            //alert("Error");
        }
    });
}
$(document).on("keyup", 'input[id^="Email"], input[id^="phone"]', function () {
    var currentValue = $(this).val().trim();
    var currentFieldId = $(this).attr("id");
    var isDuplicate = false;
    $('input[id^="Email"], input[id^="phone"]').each(function () {
        var value = $(this).val().trim();
        var fieldId = $(this).attr("id");
        if (currentFieldId !== fieldId && value !== "" && value === currentValue) {
            isDuplicate = true;
            return false;
        }
    });
    if (isDuplicate) {
        $(this).addClass("duplicate-entry");
    } else {
        $(this).removeClass("duplicate-entry");
    }
});

function CheckExpiryDate() {
    document.getElementById("MoreRecipient").disabled = false;
    var Total = parseInt($("#ExpDate").val());
    var a = parseInt($("#ExpDate1").val()) || 0;
    var b = parseInt($("#ExpDate2").val()) || 0;
    var c = parseInt($("#ExpDate3").val()) || 0;
    if (isNaN(a) || isNaN(b) || isNaN(c)) {
        $("#message").empty();
        var row = '<div class="col-md-12 p-1" role="alert">Invalid Input! Please Enter Valid Numbers</div>';
        $("#message").append(row);
        $("#ExpDate").focus();
        document.getElementById("MoreRecipient").disabled = true;
        return;
    }

    var sum = a + b + c;

    if (sum >= Total) {
        $("#message").empty();
        var row = '<div class="col-md-12 p-1" role="alert">You reached the Maximum</div>';
        $("#message").append(row);
        $("#ExpDate").focus();
        document.getElementById("MoreRecipient").disabled = true;

    }
    else {
        document.getElementById("MoreRecipient").disabled = false;
    }
}
$("#viewrec").click(function () {
    addRecipientData();
    updateGridData();
});



function DownloadOriginalFile() {
    window.location = '/zipSign/DownloadFile'; // Simply navigate to the download URL
}
function Search() {
    var keyword = $('#searchInput').val(); // Retrieve the keyword from the search input field
    var pagecount = 1; 
    GetData(pagecount, keyword); // Call the modified GetData function with the keyword
}
