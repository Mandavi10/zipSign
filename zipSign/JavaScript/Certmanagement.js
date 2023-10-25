var IsValidate = 0;
var pagecount = 1;
var keyword = '';
$(document).ready(function () {
    GetData(pagecount);

});
function GetData(pagecount, keyword) {
    $("#myGrid").html("");
    var columnDefs = [
        { headerName: 'Sr No.', field: 'Row', width: 80, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'Certificate Name', field: 'CertificateName', width: 300, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'Certificate Type', field: 'CertificateType', width: 220, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'Created By', field: 'UploadedBy', width: 150, resizable: false, sortable: true, suppressMovable: true },
        { headerName: 'Created On', field: 'UploadedOn', width: 150, resizable: false, sortable: true, suppressMovable: true },
        {
            headerName: 'Action', field: '', width: 150, sortable: true, resizable: false, suppressMovable: true, suppressMovable: true, cellRenderer: function (params) {

                return '<span class="fa fa-download gridIcon" department-code="' + params.data.Row + '" onclick="DownloadCertificate(this)"></span><span class="fa fa-trash gridIcon" id="DeleteCertificate" department-code="' + params.data.Row + '" onclick="DeleteCertificate(this)"></span>';
            }
        },
    ];
    
    var rowData = [];
    $.ajax({
        url: '/CertificateManagement/SearchandShowDataForCertificate',
        type: 'POST',
        dataType: 'json',
        data: {
            pagecount: pagecount,
            keyword: keyword
        },
        success: function (result) {
            ;
            var jsonData = result.Table1;
            var jsonData1 = result.Table2;
            $.each(jsonData, function (i, value) {
                rowData.push({
                    Row: value.Row,
                    CertificateName: value.CertificateName,
                    CertificateType: value.CertificateType,
                    UploadedBy: value.UploadedBy,
                    UploadedOn: value.UploadedOn,
                    
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
                    if (params.event.target.id === 'DeleteCertificate') {
                        DeleteCertificate(params.data.Row);
                    } else {
                        
                        DownloadCertificate(params.data.Row);
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

function DownloadCertificate(CerificateID) {
    ;
    var form = document.createElement('form');
    form.action = '/CertificateManagement/DownloadCertificate';
    form.method = 'POST';
    var input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'department-code';
    input.value = CerificateID;
    form.appendChild(input);
    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);
}


function DeleteCertificate(DepartmentId) {

    let deptId = parseInt(DepartmentId);
    if (!isNaN(deptId)) {
        window.location.href = "/CertificateManagement/DeleteCertificate?CertificateID=" + deptId;
    }
    else {
        console.error("Invalid DepartmentId:", DepartmentId);
    }
}

function Search() {
    var keyword = $('#searchInput').val(); 
    var pagecount = 1; 
    GetData(pagecount, keyword); 
}



