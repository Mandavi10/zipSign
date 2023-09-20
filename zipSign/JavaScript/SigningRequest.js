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
$(document).ready(function () {
  
    //;
    //;
    //GetDataForSignedPDF(pagecount, keyword);
    //$('input[type="radio"]').click(function () {
    //    if ($(this).attr('id') == 'rdo3') {
    //        $('#btnAgree').prop('disabled', false);
    //    }
    //    else {
    //        $('#rdo4').prop('checked', true); // Automatically select rdo1
    //        $('#btnAgree').prop('disabled', true);
    //        window.location.href = "/zipSign/SigningRequest";
    //    }
    //});

    //uploadedFileName = sessionStorage.getItem('uploadedFileName');
    //uploadedFileDate = sessionStorage.getItem('uploadedFileDate');
    //$("label#uploadedFileName").next("span").text(uploadedFileName);
    //$("label#uploadedFileStatus").next("span").text("Unsigned");
    //$("label#uploadedFileDate").next("span").text(uploadedFileDate);


    $('#btncomplete').hide();
    $('#btncomplete1').hide();
    $('#continueButton').prop('disabled', true);
    signerType = sessionStorage.getItem('Single_Signer');

    if (signerType == "Single_Signer") {
        //;
        $('#Btn_rec').hide();
        $('#btnDownload').attr('disabled', true);
        $("#btnreject").hide();
        uploadedFileName = sessionStorage.getItem('uploadedFileName');
        uploadedFileDate = sessionStorage.getItem('uploadedFileDate');
        $("label#uploadedFileName").next("span").text(uploadedFileName);
        $("label#uploadedFileStatus").next("span").text("Unsigned");
        $("label#uploadedFileDate").next("span").text(uploadedFileDate);
    }
    else {
        $('#Trail_Div').show();
        $('#Btn_rec').show();
        $('#btnDownload').attr('disabled', true);
        $("#btnreject").hide();
        $("#btnproceed").show();

        var UploadedDocumentId = getUrlParameter('UploadedDocumentId');
        var SignerID = getUrlParameter('SignerID');
        var SignerName = getUrlParameter('SignerName');
        var Emailid = getUrlParameter('Emailid');
        $.ajax({
            type: 'POST',
            url: '/NSDL/GetDocumentAllData',
            data: {
                UploadedDocumentId: UploadedDocumentId,
                SignerID: SignerID,
                SignerName: SignerName,
                Emailid: Emailid
            },
            success: function (result) {

                var uploadedFileName = result.result.DocumentName;
                var DocumentOpenOn = result.result.DocumentOpenOn;
                sessionStorage.setItem('DocumentOpenOn', DocumentOpenOn);
                sessionStorage.setItem('uploadedFileName', uploadedFileName);
                var IsSigned = result.result.IsSigned;

                UploadedDocumentId = result.UploadedDocument;
                sessionStorage.setItem('UploadedDocumentId', UploadedDocumentId);
                SignerName = result.SignName;
                sessionStorage.setItem('SignerName', SignerName);
                Emailid = result.EmailID;
                sessionStorage.setItem('Emailid', Emailid);

                $("label#uploadedFileID").next("span").text(UploadedDocumentId);
                $("label#uploadedFileName").next("span").text(uploadedFileName);
                $("label#uploadedFileDate").next("span").text(DocumentOpenOn);

                if (IsSigned == 0) {
                    $("label#uploadedFileStatus").next("span").text("Unsigned");
                    appendActivity(DocumentOpenOn, SignerName, Emailid, "Document Open");
                }
                else {
                    $("label#uploadedFileStatus").next("span").text("Signed");
                    appendActivity(DocumentOpenOn, SignerName, Emailid, "Document Signed");
                    //$('#AlreadySigned').modal('show');
                    //$("#btnproceed").hide();
                    //$('#btncomplete').show();
                }
                //appendActivity(DocumentOpenOn, SignerName,Emailid,"Document Open");

            },
            error: function (ex) {
                //alert("Error");
            }
        });
    }

    $("#PreviewSignImage1").removeAttr("src");

    filePath = getParameterByName("FilePath");
    SignedfilePath = getParameterByName("SignedFilePath");
    TxnId = getParameterByName("TxnId");
    DateTime = getParameterByName("Date");
    DateTimeParsed = convertDateFormat(DateTime);
    if (filePath != null && filePath != "") {
        //;
        appendActivity(DateTimeParsed, "Mandavi (mandavi@yoekisoft.com)", "File Signed");
        $(".btnSign").hide();
        $("#hdntxn").css("display", "block");
        $("#hdnSigningmode").css("display", "block");
        $("label#uploadedFileStatus").next("span").text("Signed");
        $("label#uploadedFileDate").next("span").text(DateTimeParsed);
        $("label#Txnno").next("span").text(TxnId);
        $("label#signingmode").next("span").text("Aadhaar");
        $("#PreviewSignImage1").removeAttr("src");
        $("#PreviewSignImage1").attr("src", filePath);
        $('#Btn_rec').hide();
        $('#btncomplete').show();
        if (signerType == "Single_Signer") {
            appendActivity(DateTimeParsed, "Mandavi", "mandavi@yoekisoft.com", "Document Signed");
            $(".btnSign").hide();
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
            appendActivity(DateTimeParsed, SignerName, Emailid, "Document Signed");
            $(".btnSign").hide();
            $("label#uploadedFileStatus").next("span").text("Signed");
            $("#PreviewSignImage1").removeAttr("src");
            $("#PreviewSignImage1").attr("src", filePath);
            $('#Btn_rec').hide();
            $('#btncomplete').show();
            $("label#uploadedFileID").next("span").text(UploadedDocumentId);
            $("label#uploadedFileName").next("span").text(uploadedFileName);
            $("label#Txnno").next("span").text(TxnId);
            //$("label#uploadedFileDate").next("span").text(DocumentOpenOn);
        }
        //appendActivity("02/08/2023 04:32 PM", "Mandavi (mandavi@yoekisoft.com)", "File Signed");
        //$(".btnSign").hide();
        //$("label#uploadedFileStatus").next("span").text("Signed");
        //$("#PreviewSignImage1").removeAttr("src");
        //$("#PreviewSignImage1").attr("src", filePath);
        //$('#Btn_rec').hide();
        //$('#btncomplete').show();


        //appendActivity(DateTimeParsed, "Mandavi (mandavi@yoekisoft.com)", "File Signed");
        //$(".btnSign").hide();
        //$("#hdntxn").css("display", "block");
        //$("label#uploadedFileStatus").next("span").text("Signed");
        //$("label#uploadedFileDate").next("span").text(DateTimeParsed);
        //$("label#Txnno").next("span").text(TxnId);
        //$("#PreviewSignImage1").removeAttr("src");
        //$("#PreviewSignImage1").attr("src", filePath);
        //$('#Btn_rec').hide();
        //$('#btncomplete').show();


        //$("#btnproceed").hide();
        //$('#Trail_Div').hide();
        //$('#Btn_rec').hide();

        $("#btnproceed").hide();


        $('#Btn_rec').hide();

        $('#btnDownload').prop('disabled', false);
        $('#btncomplete').click(function () {
            $('#completePopup').modal('show');
        });

        $('#btnok1').click(function () {
            ;
            sessionStorage.clear();
            window.location.href = "/zipSign/Signed";
            $.each(sessionStorage, function (key) {
                ;
                sessionStorage.removeItem(key);
            });
        });
        $('#btnok2').click(function () {
            //window.location.href = "/zipSign/Signed";
        });
    }
    else if (SignedfilePath != null && SignedfilePath != "")
    {
        $(".btnSign").hide();
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
            //;
            $(".btnSign").hide();

            $('#Btn_rec').hide();
            $('#btnDownload').attr('disabled', true);
            $("#btnreject").hide();
        }
        else {
            $(".btnSign").hide();
            $('#Btn_rec').hide();
            $('#btnDownload').attr('disabled', true);
            $("#btnreject").hide();
            //$('#AlreadySigned').modal('show');

        }
        filepathsss = sessionStorage.getItem('LoaclPath');
        appendActivity(uploadedFileDate, "Mandavi (mandavi@yoekisoft.com)", "File Uploaded");

        $("#PreviewSignImage1").attr("src", filepathsss);
        var fileId = getUrlParameter('File');
        $("#PreviewSignImage1").attr("src", fileId);


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
var iframeSrcSet = false; // Add this global variable

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

    var SignerName = getUrlParameter('SignerName');
    var Fileid = getUrlParameter('Fileid');
    var Emailid = getUrlParameter('Emailid');
    var SignerID = getUrlParameter('SignerID');
    var fileId = getUrlParameter('File');
    var UploadedDocumentId = getUrlParameter('UploadedDocumentId');

    $.ajax({
        type: 'POST',
        url: '/NSDL/GetSignerData',
        data: {
            UploadedDocumentId: UploadedDocumentId,
        },
        success: function (result) {
            var Coordinates = result.CoordinatesUpdate;
            UploadedDocumentId = result.UploadedDocument
            if (!iframeSrcSet) {
                iframeSrcSet = true;
                $('#consentDiv').hide();
                $("#NSDLiframe").show();
                $("#NSDLiframe").attr("src", "/NSDL/PDFSignature?file=" + encodeURIComponent(fileId) + "&Fileid=" + encodeURIComponent(Fileid) + "&Emailid=" + encodeURIComponent(Emailid) + "&SignerID=" + encodeURIComponent(SignerID) + "&SignerName=" + encodeURIComponent(SignerName) + "&UploadedDocumentId=" + encodeURIComponent(UploadedDocumentId) + "&Coordinates=" + encodeURIComponent(Coordinates));
            }
        },
        error: function (ex) {
            alert("Error");
        }
    });


}
$('#btncomplete1').click(function () {
    window.location.href = "/zipSign/SignLogin";
})


//if (signerType == "Single_Signer" && !iframeSrcSet) {
//    documentid = sessionStorage.getItem('UploadedDocumentId');
//    UniqueSignerID = sessionStorage.getItem('UniqueSignerID');
//    iframeSrcSet = true;
//    $('#consentDiv').hide();
//    $("#NSDLiframe").show();
//    $("#NSDLiframe").attr("src", "/NSDL/PDFSignature?filePath=" + encodeURIComponent(filepathsss) + "&Fileid=" + encodeURIComponent(documentid) + "&UniqueSignerID=" + encodeURIComponent(UniqueSignerID));
//}
//else {
//    iframeSrcSet = true;
//    $('#consentDiv').hide();
//    $("#NSDLiframe").show();
//    $("#NSDLiframe").attr("src", "/NSDL/PDFSignature?filePath=" + encodeURIComponent(filepathsss) + "&Fileid=" + encodeURIComponent(Fileid) + "&Emailid=" + encodeURIComponent(Emailid) + "&SignerID=" + encodeURIComponent(SignerID) + "&SignerName=" + encodeURIComponent(SignerName) + "&UploadedDocumentId=" + encodeURIComponent(UploadedDocumentId));
//}


//{
//    if (signerType == "Single_Signer" && !iframeSrcSet) {
//        documentid = sessionStorage.getItem('UploadedDocumentId');
//        UniqueSignerID = sessionStorage.getItem('UniqueSignerID');
//        iframeSrcSet = true;
//        $('#consentDiv').hide();
//        $("#NSDLiframe").show();
//        $("#NSDLiframe").attr("src", "/NSDL/PDFSignature?filePath=" + encodeURIComponent(filepathsss) + "&Fileid=" + encodeURIComponent(documentid) + "&UniqueSignerID=" + encodeURIComponent(UniqueSignerID));
//    }
//    else {
//        iframeSrcSet = true;
//        $('#consentDiv').hide();
//        $("#NSDLiframe").show();
//        $("#NSDLiframe").attr("src", "/NSDL/PDFSignature?filePath=" + encodeURIComponent(filepathsss) + "&Fileid=" + encodeURIComponent(Fileid) + "&Emailid=" + encodeURIComponent(Emailid) + "&SignerID=" + encodeURIComponent(SignerID) + "&SignerName=" + encodeURIComponent(SignerName) + "&UploadedDocumentId=" + encodeURIComponent(UploadedDocumentId));
//    }
//}

//$('#btndownload').click(function () {
//    //;
//    window.location.href = '/zipsign/downloadfile?filepath=' + decodeuricomponent(filepath);
//    //appendactivity("02/08/2023 04:32 pm", "mandavi (mandavi@yoekisoft.com)", "signed file downloaded");
//});

function Download() {
    //;
    window.location.href = '/zipsign/downloadfile?filepath=' + decodeURIComponent(filePath);
}

//$('#btnDownload').click(function () {
//    // Append activity immediately (before download request)

//    // Send an AJAX request to initiate the file download
//    $.ajax({
//        url: '/zipSign/DownloadFile',
//        type: 'GET',
//        data: { filePath: decodeURIComponent(filePath) },
//        success: function (data) {
//            var currentDate = new Date();
//            var day = String(currentDate.getDate()).padStart(2, '0');
//            var month = String(currentDate.getMonth() + 1).padStart(2, '0'); // Months are zero-indexed, so add 1
//            var year = currentDate.getFullYear();
//            var hours = String(currentDate.getHours()).padStart(2, '0');
//            var minutes = String(currentDate.getMinutes()).padStart(2, '0');
//            var amOrPm = currentDate.getHours() >= 12 ? 'PM' : 'AM';
//            var formattedDate = day + '/' + month + '/' + year + ' ' + hours + ':' + minutes + ' ' + amOrPm;
//            appendActivity(formattedDate, "Mandavi (mandavi@yoekisoft.com)", "File Download Requested");
//        },
//        error: function () {
//            console.error("Download failed.");
//        }
//    });
//});

//function appendActivity(time, role, title) {
//    var activityHtml = `
//    <div class="col-lg-3 col-md-3">
//        <div class="element-box-tp">
//            <div class="activity-boxes-w">
//                <div class="activity-box-w">
//                    <div class="activity-time">${time}</div>
//                    <div class="activity-box green-blink">
//                        <div class="activity-info">
//                            <div class="activity-role">${role}</div>
//                            <strong class="activity-title font-weight-400">${title}</strong>
//                        </div>
//                    </div>
//                </div>
//            </div>
//        </div>
//    </div>
//    `;
//    targetElement.append(activityHtml);
//}
//function appendActivity(time, role, title) {
//    //;
//    var activityHtml = `
//    <div class="col-lg-3 col-md-3">
//        <div class="element-box-tp">
//            <div class="activity-boxes-w">
//                <div class="activity-box-w">
//                    <div class="activity-time">${time}</div>
//                    <div class="activity-box green-blink">
//                        <div class="activity-info">
//                            <div class="activity-role">${role}</div>
//                            <strong class="activity-title font-weight-400">${title}</strong>
//                        </div>
//                    </div>
//                </div>
//            </div>
//        </div>
//    </div>
//    `;

//    // Retrieve previous activities from sessionStorage
//    var previousActivities = JSON.parse(sessionStorage.getItem('activities')) || [];

//    // Add the current activity to the beginning of the array
//    previousActivities.unshift(activityHtml);

//    // Limit the number of stored activities, if needed
//    // For example, to keep only the last 5 activities:
//    const maxActivities = 5;
//    if (previousActivities.length > maxActivities) {
//        previousActivities = previousActivities.slice(0, maxActivities);
//    }

//    // Store the updated activities in sessionStorage
//    sessionStorage.setItem('activities', JSON.stringify(previousActivities));

//    // Clear the target element
//    targetElement.empty();

//    // Append all activities to the target element
//    for (var i = 0; i < previousActivities.length; i++) {
//        targetElement.append(previousActivities[i]);
//    }
//}

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

    // Retrieve previous activities from sessionStorage
    var previousActivities = JSON.parse(sessionStorage.getItem('activities')) || [];

    // Check if the activity is a duplicate
    var isDuplicate = previousActivities.some(function (activity) {
        return activity === activityHtml;
    });

    if (!isDuplicate) {
        // Append the current activity to the end of the array
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










