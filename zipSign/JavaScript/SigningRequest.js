var filepathsss = '';
var signerType = '';
var documentid = '';
var filePath = '';
var TxnId = '';
var uploadedFileName = '';
var uploadedFileDate = '';
var SignerID = '';
var DateTime = '';
var DateTimeParsed = '';
var targetElement = $("#Trail_Div");
var row = '';
var pagecount = '';
var keyword = '';
var selectedRadio = '';
var iframeSrcSet = false;
$(document).ready(function () {
   
    var UId = getParameterByName('UId');
    if (UId !== null) {
        RowClickEventHandler1(UId);
    }
    var userDataString = sessionStorage.getItem('user_data');
    var userData = JSON.parse(userDataString);
    $('#btncomplete').hide();
    $('#btncomplete1').hide();
    $('#continueButton').prop('disabled', true);
    signerType = sessionStorage.getItem('Single_Signer');
    if (signerType == "Single_Signer") {
        //var userName = userData.username;
        //var userEmail = userData.email;
        $('#Btn_rec').hide();
        $('#btnDownload').attr('disabled', true);
        $("#btnreject").hide();
        uploadedFileName = sessionStorage.getItem('uploadedFileName');
        uploadedFileDate = sessionStorage.getItem('uploadedFileDate');
        var activityRole = `${userName} (${userEmail})`;
        $("label#uploadedFileName").next("span").text(uploadedFileName);
        $("label#uploadedFileStatus").next("span").text("Unsigned");
        $("label#uploadedFileDate").next("span").text(uploadedFileDate);
        filepathsss = sessionStorage.getItem('LoaclPath');
        $("#PreviewSignImage1").attr("src", filepathsss);

        appendActivity(uploadedFileDate, activityRole, "File Uplaoded");

    }
    //else {
    //    $('#Trail_Div').show();
    //    $('#Btn_rec').show();
    //    $('#btnDownload').attr('disabled', true);
    //    $("#btnreject").hide();
    //    $("#btnproceed").show();
    //    var UploadedDocumentId = getUrlParameter('UploadedDocumentId');
    //    var SignerID = getUrlParameter('SignerID');
    //    var SignerName = getUrlParameter('SignerName');
    //    var Emailid = getUrlParameter('Emailid');
    //    $.ajax({
    //        type: 'POST',
    //        url: '/NSDL/GetDocumentAllData',
    //        data: {
    //            UploadedDocumentId: UploadedDocumentId,
    //            SignerID: SignerID,
    //            SignerName: SignerName,
    //            Emailid: Emailid
    //        },
    //        success: function (result) {
    //            var uploadedFileName = result.result.DocumentName;
    //            var DocumentOpenOn = result.result.DocumentOpenOn;
    //            sessionStorage.setItem('DocumentOpenOn', DocumentOpenOn);
    //            sessionStorage.setItem('uploadedFileName', uploadedFileName);
    //            var IsSigned = result.result.IsSigned;
    //            UploadedDocumentId = result.UploadedDocument;
    //            sessionStorage.setItem('UploadedDocumentId', UploadedDocumentId);
    //            SignerName = result.SignName;
    //            sessionStorage.setItem('SignerName', SignerName);
    //            Emailid = result.EmailID;
    //            sessionStorage.setItem('Emailid', Emailid);
    //            $("label#uploadedFileID").next("span").text(UploadedDocumentId);
    //            $("label#uploadedFileName").next("span").text(uploadedFileName);
    //            $("label#uploadedFileDate").next("span").text(DocumentOpenOn);
    //            if (IsSigned == 0) {
    //                $("label#uploadedFileStatus").next("span").text("Unsigned");
    //                appendActivity(DocumentOpenOn, SignerName, Emailid, "Document Open");
    //            }
    //            else {
    //                $("label#uploadedFileStatus").next("span").text("Signed");
    //                appendActivity(DocumentOpenOn, SignerName, Emailid, "Document Signed");
    //            }
    //        },
    //        error: function (ex) {
    //        }
    //    });
    //}
    //$("#PreviewSignImage1").removeAttr("src");
    filePath = getParameterByName("FilePath");
    SignedfilePath = getParameterByName("SignedFilePath");
    TxnId = getParameterByName("TxnId");
    DateTime = getParameterByName("Date");
    DateTimeParsed = convertDateFormat(DateTime);
    if (filePath != null && filePath != "") {
        
        $("#btnproceed").hide();
       // $(".btnSign").hide();
        $("#hdntxn").css("display", "block");
        $("#hdnSigningmode").css("display", "block");
        $("label#uploadedFileStatus").next("span").text("Signed");
        $("label#uploadedFileDate").next("span").text(DateTimeParsed);
        $("label#Txnno").next("span").text(TxnId);
        $("label#signingmode").next("span").text("Aadhaar");
        $("#PreviewSignImage1").removeAttr("src");
        $("#PreviewSignImage1").attr("src", filePath);
        $('#Btn_rec').hide();
        $('#Trail_Div').hide();
        $('.doc-details').hide();
        $('#btncomplete').hide();
        $('#btncomplete1').hide();
        $("#btnreject").hide();
        if (signerType == "Single_Signer") {
            var userDataString = sessionStorage.getItem('user_data');
            var userData = JSON.parse(userDataString);
            var userName = userData.username;
            var userEmail = userData.email;
            var activityRole = `${userName} (${userEmail})`;
            appendActivity(DateTimeParsed, activityRole , "Document Signed");
           // $(".btnSign").hide();
            $("#hdntxn").css("display", "block");
            $("label#uploadedFileStatus").next("span").text("Signed");
            $("#PreviewSignImage1").removeAttr("src");
            $("#PreviewSignImage1").attr("src", filePath);
            $("label#uploadedFileDate").next("span").text(DateTimeParsed);
            $("label#Txnno").next("span").text(TxnId);
            $('#Btn_rec').hide();
            $('#btncomplete').show();
        }
        else {
            //appendActivity(DateTimeParsed, SignerName, Emailid, "Document Signed");
           // $(".btnSign").hide();
            $("label#uploadedFileStatus").next("span").text("Signed");
            $("#PreviewSignImage1").removeAttr("src");
            $("#PreviewSignImage1").attr("src", filePath);
            $('#Btn_rec').hide();
            $('#btncomplete').show();
            $("label#uploadedFileID").next("span").text(UploadedDocumentId);
            $("label#uploadedFileName").next("span").text(uploadedFileName);
            $("label#Txnno").next("span").text(TxnId);
        }
        $("#btnproceed").hide();
        $('#Btn_rec').hide();
        $('#btnDownload').prop('disabled', false);
        $('#btncomplete').click(function () {
            $('#completePopup').modal('show');
        });
        $('#btnok1').click(function () {
            sessionStorage.clear();
            window.location.href = "/zipSign/Signed";
            $.each(sessionStorage, function (key) {
                sessionStorage.removeItem(key);
            });
        });
        $('#btnok2').click(function () {

        });
      
    }
    else if (SignedfilePath != null && SignedfilePath != "") {
       // $(".btnSign").hide();
        $("#hdntxn").css("display", "block");
        $("#hdnSigningmode").css("display", "block");
        $("label#uploadedFileStatus").next("span").text("Signed");
        $("label#uploadedFileDate").next("span").text(DateTimeParsed);
        $("label#Txnno").next("span").text(TxnId);
        $("label#signingmode").next("span").text("Aadhaar");
        $("#PreviewSignImage1").removeAttr("src");
        $("#PreviewSignImage1").attr("src", SignedfilePath);
        $('#Btn_rec').hide();
        $('#btncomplete1').show();
    }
    else {
        $('#continueButton').prop('disabled', true);

        signerType = sessionStorage.getItem('Single_Signer');
        if (signerType == "Single_Signer") {

            //$(".btnSign").hide();

            $('#Btn_rec').hide();
            $('#btnDownload').attr('disabled', true);
            $("#btnreject").hide();
        }
        else {
           // $(".btnSign").hide();
            $('#Btn_rec').hide();
            $('#btnDownload').attr('disabled', true);
            $("#btnreject").hide();
        }
        //appendActivity(uploadedFileDate, "Mandavi (mandavi@yoekisoft.com)", "File Uploaded");
        //$("#PreviewSignImage1").attr("src", filepathsss);
        $('.select').click(function () {
            $('.select-container').toggleClass('active');
        });
    }
    $('.option').click(function () {
        var selectedOption = $(this).text();
        $('#input1').val(selectedOption);
        $('.select-container').removeClass('active');
        $('#continueButton').prop('disabled', false);
    });
    $('#input1').change(function () {
        if ($(this).val() === '') {
            $('#continueButton').prop('disabled', true);
        } else {
            $('#continueButton').prop('disabled', false);
        }
    });
    $('input[type="radio"]').click(function () {
        ;
        $("#dscmsg").empty();
        row = '';
        ;
        if ($(this).attr('id') == 'rdo3') {
            $('#btnAgree').prop('disabled', false);
        }
        else if ($(this).attr('id') == 'rdo2') {
            $('#rdo3').prop('checked', true);
            $('#btnAgree').prop('disabled', false);
            selectedRadio = 'rdo2';
            //window.location.href = "/zipSign/SigningRequest";
        }
        else if ($(this).attr('id') == 'aadhaar') {
            selectedRadio = 'aadhaar'
            row = '';
            $("#dscmsg").empty();
            $('#continueButton').prop('disabled', false);
        } else if ($(this).attr('id') == 'dsc') {
            selectedRadio = 'dsc';
            $('#continueButton').prop('disabled', false);
        }
    });
   
});

function getParameterByName(name) {
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(window.location.href);
    if (!results) return null;
    if (!results[2]) return '';
    var decodedValue = decodeURIComponent(results[2].replace(/\+/g, " "));
    return decodedValue;
}
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


function Continue() {
    if (selectedRadio == 'dsc') {
        row = '';
        $("#dscmsg").empty();
        // row += ' <div style="background-color: #cce5ff; color: #004085; font-size: 11px;">DSC functionality will be available soon..!</div >';
        // $("#dscmsg").append(row);
        $('#dscPopup').modal('show');
        $('#continueButton').prop('disabled', true);
    }
    else {
        selectedRadio == 'aadhaar'
        signerType = sessionStorage.getItem('Single_Signer');
        if (signerType == "Single_Signer") {
            $("#AgreeBtnProceed").show();
            $("#btnAgree").hide();
        }
        else {
            $("#AgreeBtnProceed").hide();
            $("#btnAgree").show()
        }

        $(".iframeDiv").css("display", "block");
        $("#IframeDiv").css("display", "block");
        $("#consentDiv").css("display", "block");


        $(".buttonCon").css("display", "none");
        $(".signersList").css("display", "none");
        $("#CancelBtn").css("display", "none");
    }

    //$("#iNSDL").val(filePathss);
}

function AgreeBtnOnProceed() {
    if (selectedRadio == 'rdo2') {
        window.location.href = "/zipSign/SigningRequest";
        return;
    }
    else {
        SignerID = sessionStorage.getItem('SignerID');
        documentid = sessionStorage.getItem('UploadedDocumentId');
        //var Coordinates = result.UploadedName;
        if (signerType == "Single_Signer" && !iframeSrcSet) {
            var Coordinates = 0;
            documentid = sessionStorage.getItem('UploadedDocumentId');
            UniqueSignerID = sessionStorage.getItem('UniqueSignerID');
            iframeSrcSet = true;
            $('#consentDiv').hide();
            //$('#loader').css('display', 'block');
            $("#NSDLiframe").show();
            $("#NSDLiframe").attr("src", "/NSDL/PDFSignature?File=" + encodeURIComponent(filepathsss) + "&UploadedDocumentId=" + encodeURIComponent(documentid) + "&SignerID=" + encodeURIComponent(SignerID) + "&Coordinates=" + encodeURIComponent(Coordinates));

        }
    }
}
function AgreeBtn() {

    if (!iframeSrcSet) {
        iframeSrcSet = true;
        $('#consentDiv').hide();
        $("#NSDLiframe").show();
        $("#NSDLiframe").attr("src", "/NSDL/PDFSignature?file=" + encodeURIComponent($("#hdnFilePath").val()) + "&Fileid=" + encodeURIComponent($("#hdnFileId").val()) + "&Emailid=" + encodeURIComponent($("#hdnEmailId").val()) + "&SignerID=" + encodeURIComponent($("#hdnSignerId").val()) + "&SignerName=" + encodeURIComponent($("#hdnSignerName").val()) + "&UploadedDocumentId=" + encodeURIComponent($("#hdnUploadedDocumentId").val()) + "&Coordinates=" + encodeURIComponent(0));
    }
}
function RowClickEventHandler1(UId) {
    
    $.ajax({
        url: '/NSDL/GetDocumentAllData1',
        type: 'POST',
        dataType: 'json',
        data: {
            Link: UId,
        },
        async: false,
        success: function (result) {
            var linkExpiredOn = result.responseData.LinkExpiredOn;
            var currentDateTime = getCurrentDateTime();
            var txtfrom = [];
            txtfrom = linkExpiredOn.split(' ')[0].split('-');
            var txtfromnew = txtfrom[2] + '-' + txtfrom[1] + '-' + txtfrom[0];
            var txtTo = [];
            txtTo = currentDateTime.split(' ')[0].split('-');
            var txtTonew = txtTo[2] + '-' + txtTo[1] + '-' + txtTo[0];
            var _datetxtFrom = new Date(txtfromnew)
            var _datetxtTo = new Date(txtTonew) //txtFrom //txtTo
            if (_datetxtTo > _datetxtFrom) {
                
                window.location.href = "/zipSign/Link_Expired";
            }
            else {
                
                var table3Data = result.responseData.Table3Data;
                var trailDiv = $("#Trail_Div");
                // Clear any existing content in the Trail_Div
                trailDiv.empty();
                for (var i = 0; i < table3Data.length; i++) {
                    var rowData = table3Data[i];
                    var activityTime1 = rowData["CreatedOn"]; 
                    var userName = rowData["UserName"]; 
                    var userEmail = rowData["EmailID"]; 
                    var date1 = parseInt(activityTime1.match(/\d+/)[0]);;
                    var date = new Date(date1);
                    var formattedDate =
                        ('0' + date.getDate()).slice(-2) + '/' +
                        ('0' + (date.getMonth() + 1)).slice(-2) + '/' +
                        ('' + date.getFullYear()).slice(-2) + ' ' +
                        ('0' + (date.getHours() % 12 || 12)).slice(-2) + ':' +
                        ('0' + date.getMinutes()).slice(-2) + ' ' +
                        (date.getHours() >= 12 ? 'PM' : 'AM');

                    var activityTime = `${formattedDate}`;
                    var activityRole = `${userName} (${userEmail})`;

                    var activityTitle = rowData["Action"]; 

                    // Create a new activity box
                    var activityBox = $(
                        `<div class="col-lg-3 col-md-3">
        <div class="element-box-tp">
            <div class="activity-boxes-w">
                <div class="activity-box-w">
                    <div class="activity-time">${activityTime}</div>
                    <div class="activity-box">
                        <div class="activity-info">
                            <div class="activity-role">${activityRole}</div>
                            <strong class="activity-title font-weight-400">${activityTitle}</strong>
                        </div>
                    </div>
                </div>
            </div>
        </div >
    </div>`
                    );


                    // Append the activityBox to the Trail_Div
                    trailDiv.append(activityBox);
                }
            }

            //// Update other elements with data as needed

            var EmailID = result.EmailID;
            $("#PreviewSignImage1").attr("src", "");
            $("#PreviewSignImage1").attr("src", result.responseData.FilePath);
            $("#hdnFilePath").val(result.responseData.FilePath);
            $("#hdnUploadedDocumentId").val(result.responseData.UploadedDocumentId);
            $("#hdnSignerId").val(result.responseData.SignerId);
            $("#hdnFileId").val(result.responseData.FileID);
            $("#hdnEmailId").val(result.responseData.EmailID);
            $("#hdnSignerName").val(result.responseData.SignerName);
            $("label#uploadedFileName").next("span").text(result.responseData.UploadedFileName);
            $("label#uploadedFileStatus").next("span").text("Unsigned");
            $("label#uploadedFileDate").next("span").text(result.responseData.UploadedOn);
            //}
        },
        error: function () {
            alert('Something Went Wrong');
        }
    });
}
$('#btncomplete1').click(function () {
    window.location.href = "/zipSign/SignLogin";
})
function Download() {
    window.location.href = '/zipsign/downloadfile?filepath=' + decodeURIComponent(filePath);
}
function appendActivity(time, role, title) {
    var activityHtml = `
    <div class="col-lg-3 col-md-3">
        <div class="element-box-tp">
            <div class="activity-boxes-w">
                <div class="activity-box-w">
                    <div class="activity-time">${time}</div>
                    <div class="activity-box green-blink">
                        <div class="activity-info">
                            <div class="activity-role">${role}</div>
                            <strong class="activity-title font-weight-400">${title}</strong>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    `;
    var previousActivities = JSON.parse(sessionStorage.getItem('activities')) || [];
    var isDuplicate = previousActivities.some(function (activity) {
        return activity === activityHtml;
    });

    if (!isDuplicate) {
        previousActivities.push(activityHtml);

        // Limit the number of stored activities, if needed
        // For example, to keep only the last 5 activities:
        const maxActivities = 5;
        if (previousActivities.length > maxActivities) {
            previousActivities.shift(); // Remove the oldest activity
        }

        // Store the updated activities in sessionStorage
        sessionStorage.setItem('activities', JSON.stringify(previousActivities));

        // Clear the target element
        targetElement.empty();

        // Append all activities to the target element
        for (var i = 0; i < previousActivities.length; i++) {
            targetElement.append(previousActivities[i]);
        }
    }
}
function convertDateFormat(inputDate) {
    var formattedDate = new Date(inputDate);
    var month = (formattedDate.getMonth() + 1).toString().padStart(2, '0');
    var day = formattedDate.getDate().toString().padStart(2, '0');
    var year = formattedDate.getFullYear();
    var hours = formattedDate.getHours();
    var minutes = formattedDate.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // Handle midnight (12:00 AM)
    return day + '/' + month + '/' + year + ' ' + hours + ':' + minutes.toString().padStart(2, '0') + ' ' + ampm;
}

function getCurrentDateTime() {
    var currentDate = new Date();
    var year = currentDate.getFullYear();
    var month = ('0' + (currentDate.getMonth() + 1)).slice(-2); // Months are zero-based
    var day = ('0' + currentDate.getDate()).slice(-2);
    var hours = ('0' + currentDate.getHours()).slice(-2);
    var minutes = ('0' + currentDate.getMinutes()).slice(-2);
    var seconds = ('0' + currentDate.getSeconds()).slice(-2);

    var formattedDateTime = `$'{day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
    return formattedDateTime;
}


