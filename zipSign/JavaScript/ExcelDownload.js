$(document).ready(function () {
    //
    $('#downloadButton').click(function () {
        //
        $.ajax({
            url: '/ZipSign/DownloadExcelFile',
            type: 'POST',
            xhrFields: {
                responseType: 'blob' // Set the response type to 'blob'
            },
            success: function (data) {
                // Create a download link using the response data
                var downloadLink = document.createElement('a');
                var blobUrl = URL.createObjectURL(data);//Creates a string containing a URL representing the object given name to the parameter
                downloadLink.href = blobUrl;
                downloadLink.download = 'DemoExcel.csv';
                downloadLink.click();
                // Clean up the temporary URL
                URL.revokeObjectURL(blobUrl);
            }
        });
    });
});

$('#downloadButton').click(function () {
    $.ajax({
        //DataInsertion
        type: 'POST',
        url: '/zipSign/SignInsert',
        data: {
            DocumentName: $("#text-input1").val(),
            ReferenceNumber: $("#text-input2").val(),
            DocumentName: $("#SignImage").val(),
            UploadedDoc: $("#SignImage").val(),
        },
        success: function (result) {
            if (result.status = "200") {
                $("#text-input1").val('');
                $("#text-input2").val('');
                $("#SignImage").val('');
                showSuccessMessage();
                //window.location.href = '../ZipSign/Draft'
                GetData();
            }
        },
        error: function (ex) {
            alert("Error");
        }
    });
});

$(document).ready(function () {
    //
    $('#downloadButton1').click(function () {
        //
        $.ajax({
            url: '/Masters/DownloadExcelFile',
            type: 'POST',
            xhrFields: {
                responseType: 'blob' // Set the response type to 'blob'
            },
            success: function (data) {
                // Create a download link using the response data
                var downloadLink = document.createElement('a');
                var blobUrl = URL.createObjectURL(data);//Creates a string containing a URL representing the object given name to the parameter
                downloadLink.href = blobUrl;
                downloadLink.download = 'DepartmentDetails.csv';
                downloadLink.click();
                // Clean up the temporary URL
                URL.revokeObjectURL(blobUrl);
            }
        });
    });
});
