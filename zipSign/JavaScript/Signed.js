var pagecount = 1;
var keyword = '';
$(document).ready(function () {
    
    GetDataForSignedPDF(pagecount, keyword);
    
});

function GetDataForSignedPDF(pagecount, keyword) {
   
    $("#myGrid1").html("");
    var columnDefs = [
        { headerName: 'Sr No.', field: 'DocumentUploadId', width: 80, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'File Name', field: 'Uploaded_Document_Name', width: 250, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'Document Name', field: 'DocumentName', width: 250, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'Status', field: 'SignStatus', width: 200, resizable: false, sortable: true, suppressMovable: true },
        //{ headerName: 'Document Uploaded On', field: 'don', width: 200, resizable: false, sortable: true,suppressMovable: true   },
        //{ headerName: 'Document Uploaded By', field: 'uby', width: 200, resizable: false, sortable: true,suppressMovable: true   },
        //{ headerName: 'Signing Initiated On', field: 'sio', width: 200, resizable: false, sortable: true, suppressMovable: true   },
        //{ headerName: 'Signing Initiated By', field: 'sib', width: 200, resizable: false, sortable: true, suppressMovable: true   },
        {
            headerName: 'Action', field: '', width: 240, sortable: true, resizable: false, suppressMovable: true, suppressMovable: true, cellRenderer: function (params) {
                //;

                return '<button type="button" class="ingridbtn view-more-btn" data-bs-toggle="modal" data-bs-target="#usermodal" data-file-Code="' + params.data.DocumentUploadId + '" onclick="ShowMoreForSigned(this)">View More</button> <span class="fa fa-trash gridIcon" id=""></span>';
            }
        },
    ];
    var rowData = [];
    //
    $.ajax({
        url: '/Masters/SearchDataForSigned',
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
                    SignStatus: value.signerInfos, // Update this line to match the actual field name
                    UploadedOn: value.UploadedOn, // You should map this to the correct field as well
                    UploadedBy: value.UploadedBy, // Similarly, map this to the correct field
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

            html1 += "<ul class='pagination'>";

            if (parseInt(page) > 1) {
                // For Previous Count
                html1 += "<li class='page-item'><a href='#' class='page-link' onclick='GetDataForSignedPDF(" + (parseInt(page) - 1) + ")'>Previous</a></li>";
            } else {
                html1 += "<li class='page-item disabled'><a href='#' class='page-link'>Previous</a></li>";
            }

            if (jsonData1.length >= 1) {
                var FirstPageNo = Math.max(pagecount - 4, 1);
                for (var i = FirstPageNo; i <= pagecount; i++) {
                    if (pagecount == i) {
                        // Current Page
                        html1 += "<li class='page-item active'><a href='#' class='page-link'>" + i + "</a></li>";
                    } else if (count >= i) {
                        html1 += "<li class='page-item'><a href='#' class='page-link' onclick='GetDataForSignedPDF(" + i + ")'>" + i + "</a></li>";
                    } else {
                        html1 += "<li class='page-item disabled'><a href='#' class='page-link'>" + i + "</a></li>";
                    }
                }
            }

            if (parseInt(page) < count) {
                html1 += "<li class='page-item'><a href='#' class='page-link' onclick='GetDataForSignedPDF(" + (parseInt(page) + 1) + ")'>Next</a></li>";
            } else {
                html1 += "<li class='page-item disabled'><a href='#' class='page-link'>Next</a></li>";
            }

            html1 += "</ul>";

            $("#pagination1").append(html1);

            var gridOptions = {
                columnDefs: columnDefs,
                rowData: rowData,
                pagination: false, // Set this to true if you want to enable grid pagination
                onRowClicked: function (params) {
                    if (params.event.target.classList.contains('fa-trash')) {
                        var fileCode = params.event.target.getAttribute('data-file-Code');
                        
                        Delete(fileCode);
                    }
                    else {
                        var fileCode = params.event.target.getAttribute('data-file-Code');
                        ShowMoreForSigned(fileCode)
                    }
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
function ShowMoreForSigned(FileCode) {
    
    $.ajax({
        url: '/Masters/GetSignedFileDetails',
        type: 'POST',
        dataType: 'json',
        data: {
            FileCode: FileCode
        },
        success: function (result) {
            
            var jsonData = result.Table1;
            if (jsonData.length > 0) {
                var FileDeatils = jsonData[0];
                $('#FileName').text(FileDeatils.UploadedFileName);
                $('#DocumentName').text(FileDeatils.DocumentName);
                //$('#SignedOn').text(DateTimeParsed);
                $('#SignedBy').text(FileDeatils.UploadedBy);
                $('#Signedate').text(FileDeatils.UploadedOn);
                $('#UploadedBy').text(FileDeatils.UploadedBy);
                $('#UploadedOn').text(FileDeatils.SignedOn);
                $('#Status').text(FileDeatils.SignStatus);
            }
        },
        error: function () {
            alert('Failed to retrieve department details.');
        }
    });
}