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
var targetElement = jquery_3_6("#Trail_Div");
var row = '';
var pagecount = '';
var keyword = '';
var selectedRadio = '';
var iframeSrcSet = false;
jquery_3_6(document).ready(function () {
   
    var UId = getParameterByName('UId');
    if (UId !== null) {
        RowClickEventHandler1(UId);
    }
    var userDataString = sessionStorage.getItem('user_data');
    var userData = JSON.parse(userDataString);
    jquery_3_6('#btncomplete').hide();
    jquery_3_6('#btncomplete1').hide();
    jquery_3_6('#continueButton').prop('disabled', true);
    signerType = sessionStorage.getItem('Single_Signer');
    if (signerType == "Single_Signer") {
        //var userName = userData.username;
        //var userEmail = userData.email;
        jquery_3_6('#Btn_rec').hide();
        jquery_3_6('#btnDownload').attr('disabled', true);
        jquery_3_6("#btnreject").hide();
        uploadedFileName = sessionStorage.getItem('uploadedFileName');
        uploadedFileDate = sessionStorage.getItem('uploadedFileDate');
        var activityRole = `jquery_3_6{userName} (jquery_3_6{userEmail})`;
        jquery_3_6("label#uploadedFileName").next("span").text(uploadedFileName);
        jquery_3_6("label#uploadedFileStatus").next("span").text("Unsigned");
        jquery_3_6("label#uploadedFileDate").next("span").text(uploadedFileDate);
        filepathsss = sessionStorage.getItem('LoaclPath');
        jquery_3_6("#PreviewSignImage1").attr("src", filepathsss);

        //appendActivity(uploadedFileDate, activityRole, "File Uplaoded");

    }
    //else {
    //    jquery_3_6('#Trail_Div').show();
    //    jquery_3_6('#Btn_rec').show();
    //    jquery_3_6('#btnDownload').attr('disabled', true);
    //    jquery_3_6("#btnreject").hide();
    //    jquery_3_6("#btnproceed").show();
    //    var UploadedDocumentId = getUrlParameter('UploadedDocumentId');
    //    var SignerID = getUrlParameter('SignerID');
    //    var SignerName = getUrlParameter('SignerName');
    //    var Emailid = getUrlParameter('Emailid');
    //    jquery_3_6.ajax({
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
    //            jquery_3_6("label#uploadedFileID").next("span").text(UploadedDocumentId);
    //            jquery_3_6("label#uploadedFileName").next("span").text(uploadedFileName);
    //            jquery_3_6("label#uploadedFileDate").next("span").text(DocumentOpenOn);
    //            if (IsSigned == 0) {
    //                jquery_3_6("label#uploadedFileStatus").next("span").text("Unsigned");
    //                appendActivity(DocumentOpenOn, SignerName, Emailid, "Document Open");
    //            }
    //            else {
    //                jquery_3_6("label#uploadedFileStatus").next("span").text("Signed");
    //                appendActivity(DocumentOpenOn, SignerName, Emailid, "Document Signed");
    //            }
    //        },
    //        error: function (ex) {
    //        }
    //    });
    //}
    //jquery_3_6("#PreviewSignImage1").removeAttr("src");
    filePath = getParameterByName("FilePath");
    SignedfilePath = getParameterByName("SignedFilePath");
    TxnId = getParameterByName("TxnId");
    DateTime = getParameterByName("Date");
    DateTimeParsed = convertDateFormat(DateTime);
    if (filePath != null && filePath != "") {
        
        jquery_3_6("#btnproceed").hide();
       // jquery_3_6(".btnSign").hide();
        jquery_3_6("#hdntxn").css("display", "block");
        jquery_3_6("#hdnSigningmode").css("display", "block");
        jquery_3_6("label#uploadedFileStatus").next("span").text("Signed");
        jquery_3_6("label#uploadedFileDate").next("span").text(DateTimeParsed);
        jquery_3_6("label#Txnno").next("span").text(TxnId);
        jquery_3_6("label#signingmode").next("span").text("Aadhaar");
        jquery_3_6("#PreviewSignImage1").removeAttr("src");
        jquery_3_6("#PreviewSignImage1").attr("src", filePath);
        jquery_3_6('#Btn_rec').hide();
        jquery_3_6('#Trail_Div').hide();
        jquery_3_6('.doc-details').hide();
        jquery_3_6('#btncomplete').hide();
        jquery_3_6('#btncomplete1').hide();
        jquery_3_6("#btnreject").hide();
        if (signerType == "Single_Signer") {
            var userDataString = sessionStorage.getItem('user_data');
            var userData = JSON.parse(userDataString);
            var userName = userData.username;
            var userEmail = userData.email;
            var activityRole = `jquery_3_6{userName} (jquery_3_6{userEmail})`;
            appendActivity(DateTimeParsed, activityRole , "Document Signed");
           // jquery_3_6(".btnSign").hide();
            jquery_3_6("#hdntxn").css("display", "block");
            jquery_3_6("label#uploadedFileStatus").next("span").text("Signed");
            jquery_3_6("#PreviewSignImage1").removeAttr("src");
            jquery_3_6("#PreviewSignImage1").attr("src", filePath);
            jquery_3_6("label#uploadedFileDate").next("span").text(DateTimeParsed);
            jquery_3_6("label#Txnno").next("span").text(TxnId);
            jquery_3_6('#Btn_rec').hide();
            jquery_3_6('#btncomplete').show();
        }
        else {
            //appendActivity(DateTimeParsed, SignerName, Emailid, "Document Signed");
           // jquery_3_6(".btnSign").hide();
            jquery_3_6("label#uploadedFileStatus").next("span").text("Signed");
            jquery_3_6("#PreviewSignImage1").removeAttr("src");
            jquery_3_6("#PreviewSignImage1").attr("src", filePath);
            jquery_3_6('#Btn_rec').hide();
            jquery_3_6('#btncomplete').show();
            jquery_3_6("label#uploadedFileID").next("span").text(UploadedDocumentId);
            jquery_3_6("label#uploadedFileName").next("span").text(uploadedFileName);
            jquery_3_6("label#Txnno").next("span").text(TxnId);
        }
        jquery_3_6("#btnproceed").hide();
        jquery_3_6('#Btn_rec').hide();
        jquery_3_6('#btnDownload').prop('disabled', false);
        jquery_3_6('#btncomplete').click(function () {
            jquery_3_6('#completePopup').modal('show');
        });
        jquery_3_6('#btnok1').click(function () {
            sessionStorage.clear();
            window.location.href = "/zipSign/Signed";
            jquery_3_6.each(sessionStorage, function (key) {
                sessionStorage.removeItem(key);
            });
        });
        jquery_3_6('#btnok2').click(function () {

        });
      
    }
    else if (SignedfilePath != null && SignedfilePath != "") {
       // jquery_3_6(".btnSign").hide();
        jquery_3_6("#hdntxn").css("display", "block");
        jquery_3_6("#hdnSigningmode").css("display", "block");
        jquery_3_6("label#uploadedFileStatus").next("span").text("Signed");
        jquery_3_6("label#uploadedFileDate").next("span").text(DateTimeParsed);
        jquery_3_6("label#Txnno").next("span").text(TxnId);
        jquery_3_6("label#signingmode").next("span").text("Aadhaar");
        jquery_3_6("#PreviewSignImage1").removeAttr("src");
        jquery_3_6("#PreviewSignImage1").attr("src", SignedfilePath);
        jquery_3_6('#Btn_rec').hide();
        jquery_3_6('#btncomplete1').show();
    }
    else {
        jquery_3_6('#continueButton').prop('disabled', true);

        signerType = sessionStorage.getItem('Single_Signer');
        if (signerType == "Single_Signer") {

            //jquery_3_6(".btnSign").hide();

            jquery_3_6('#Btn_rec').hide();
            jquery_3_6('#btnDownload').attr('disabled', true);
            jquery_3_6("#btnreject").hide();
        }
        else {
           // jquery_3_6(".btnSign").hide();
            jquery_3_6('#Btn_rec').hide();
            jquery_3_6('#btnDownload').attr('disabled', true);
            jquery_3_6("#btnreject").hide();
        }
        //appendActivity(uploadedFileDate, "Mandavi (mandavi@yoekisoft.com)", "File Uploaded");
        //jquery_3_6("#PreviewSignImage1").attr("src", filepathsss);
        jquery_3_6('.select').click(function () {
            jquery_3_6('.select-container').toggleClass('active');
        });
    }
    jquery_3_6('.option').click(function () {
        var selectedOption = jquery_3_6(this).text();
        jquery_3_6('#input1').val(selectedOption);
        jquery_3_6('.select-container').removeClass('active');
        jquery_3_6('#continueButton').prop('disabled', false);
    });
    jquery_3_6('#input1').change(function () {
        if (jquery_3_6(this).val() === '') {
            jquery_3_6('#continueButton').prop('disabled', true);
        } else {
            jquery_3_6('#continueButton').prop('disabled', false);
        }
    });
    jquery_3_6('input[type="radio"]').click(function () {
        ;
        jquery_3_6("#dscmsg").empty();
        row = '';
        ;
        if (jquery_3_6(this).attr('id') == 'rdo3') {
            jquery_3_6('#btnAgree').prop('disabled', false);
        }
        else if (jquery_3_6(this).attr('id') == 'rdo2') {
            jquery_3_6('#rdo3').prop('checked', true);
            jquery_3_6('#btnAgree').prop('disabled', false);
            selectedRadio = 'rdo2';
            //window.location.href = "/zipSign/SigningRequest";
        }
        else if (jquery_3_6(this).attr('id') == 'aadhaar') {
            selectedRadio = 'aadhaar'
            row = '';
            jquery_3_6("#dscmsg").empty();
            jquery_3_6('#continueButton').prop('disabled', false);
        } else if (jquery_3_6(this).attr('id') == 'dsc') {
            selectedRadio = 'dsc';
            jquery_3_6('#continueButton').prop('disabled', false);
        }
    });
   
});

function getParameterByName(name) {
    name = name.replace(/[\[\]]/g, "\\jquery_3_6&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|jquery_3_6)"),
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
        jquery_3_6("#dscmsg").empty();
        // row += ' <div style="background-color: #cce5ff; color: #004085; font-size: 11px;">DSC functionality will be available soon..!</div >';
        // jquery_3_6("#dscmsg").append(row);
        jquery_3_6('#dscPopup').modal('show');
        jquery_3_6('#continueButton').prop('disabled', true);
    }
    else {
        selectedRadio == 'aadhaar'
        signerType = sessionStorage.getItem('Single_Signer');
        if (signerType == "Single_Signer") {
            jquery_3_6("#AgreeBtnProceed").show();
            jquery_3_6("#btnAgree").hide();
        }
        else {
            jquery_3_6("#AgreeBtnProceed").hide();
            jquery_3_6("#btnAgree").show()
        }

        jquery_3_6(".iframeDiv").css("display", "block");
        jquery_3_6("#IframeDiv").css("display", "block");
        jquery_3_6("#consentDiv").css("display", "block");


        jquery_3_6(".buttonCon").css("display", "none");
        jquery_3_6(".signersList").css("display", "none");
        jquery_3_6("#CancelBtn").css("display", "none");
    }

    //jquery_3_6("#iNSDL").val(filePathss);
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
            jquery_3_6('#consentDiv').hide();
            //jquery_3_6('#loader').css('display', 'block');
            jquery_3_6("#NSDLiframe").show();
            jquery_3_6("#NSDLiframe").attr("src", "/NSDL/PDFSignature?File=" + encodeURIComponent(filepathsss) + "&UploadedDocumentId=" + encodeURIComponent(documentid) + "&SignerID=" + encodeURIComponent(SignerID) + "&Coordinates=" + encodeURIComponent(Coordinates));

        }
    }
}
function AgreeBtn() {

    if (!iframeSrcSet) {
        iframeSrcSet = true;
        jquery_3_6('#consentDiv').hide();
        jquery_3_6("#NSDLiframe").show();
        jquery_3_6("#NSDLiframe").attr("src", "/NSDL/PDFSignature?file=" + encodeURIComponent(jquery_3_6("#hdnFilePath").val()) + "&Fileid=" + encodeURIComponent(jquery_3_6("#hdnFileId").val()) + "&Emailid=" + encodeURIComponent(jquery_3_6("#hdnEmailId").val()) + "&SignerID=" + encodeURIComponent(jquery_3_6("#hdnSignerId").val()) + "&SignerName=" + encodeURIComponent(jquery_3_6("#hdnSignerName").val()) + "&UploadedDocumentId=" + encodeURIComponent(jquery_3_6("#hdnUploadedDocumentId").val()) + "&Coordinates=" + encodeURIComponent(0));
    }
}
function RowClickEventHandler1(UId) {
    
    jquery_3_6.ajax({
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
                var trailDiv = jquery_3_6("#Trail_Div");
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

                    var activityTime = `jquery_3_6{formattedDate}`;
                    var activityRole = `jquery_3_6{userName} (jquery_3_6{userEmail})`;

                    var activityTitle = rowData["Action"]; 

                    // Create a new activity box
                    var activityBox = jquery_3_6(
                        `<div class="col-lg-3 col-md-3">
        <div class="element-box-tp">
            <div class="activity-boxes-w">
                <div class="activity-box-w">
                    <div class="activity-time">jquery_3_6{activityTime}</div>
                    <div class="activity-box">
                        <div class="activity-info">
                            <div class="activity-role">jquery_3_6{activityRole}</div>
                            <strong class="activity-title font-weight-400">jquery_3_6{activityTitle}</strong>
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
            jquery_3_6("#PreviewSignImage1").attr("src", "");
            jquery_3_6("#PreviewSignImage1").attr("src", result.responseData.FilePath);
            jquery_3_6("#hdnFilePath").val(result.responseData.FilePath);
            jquery_3_6("#hdnUploadedDocumentId").val(result.responseData.UploadedDocumentId);
            jquery_3_6("#hdnSignerId").val(result.responseData.SignerId);
            jquery_3_6("#hdnFileId").val(result.responseData.FileID);
            jquery_3_6("#hdnEmailId").val(result.responseData.EmailID);
            jquery_3_6("#hdnSignerName").val(result.responseData.SignerName);
            jquery_3_6("label#uploadedFileName").next("span").text(result.responseData.UploadedFileName);
            jquery_3_6("label#uploadedFileStatus").next("span").text("Unsigned");
            jquery_3_6("label#uploadedFileDate").next("span").text(result.responseData.UploadedOn);
            //}
        },
        error: function () {
            alert('Something Went Wrong');
        }
    });
}
jquery_3_6('#btncomplete1').click(function () {
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
                    <div class="activity-time">jquery_3_6{time}</div>
                    <div class="activity-box green-blink">
                        <div class="activity-info">
                            <div class="activity-role">jquery_3_6{role}</div>
                            <strong class="activity-title font-weight-400">jquery_3_6{title}</strong>
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

    var formattedDateTime = `jquery_3_6'{day}-jquery_3_6{month}-jquery_3_6{year} jquery_3_6{hours}:jquery_3_6{minutes}:jquery_3_6{seconds}`;
    return formattedDateTime;
}


