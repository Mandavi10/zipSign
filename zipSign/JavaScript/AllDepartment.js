var pagecount = 1;
var keyword;
$(document).ready(function () {
   
    //var UserMasterID = sessionStorage.getItem('UserId');
    //if (UserMasterID == "" || UserMasterID == null) {
    //    window.location.href = "/Login/Index";
    //}
    GetData(pagecount);
    GetDataForSignedPDF(pagecount, keyword)

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

function GetData(pagecount, keyword) {
    //
    $("#myGrid").html("");
    var columnDefs = [
        { headerName: 'Sr No.', field: 'SR No.', width: 80, resizable: false, suppressMovable: true, valueGetter: "node.rowIndex + 1" },
        { headerName: 'Department Code', field: 'DepartmentCode', width:  310, resizable: false, suppressMovable: true, sortable: true, cellRenderer: function (params) { return '<a href="#" onclick="Click(\'' + params.data.DepartmentId + '\')" id="EditData">' + params.data.DepartmentCode + '</a>'; }, },
        { headerName: 'Department Name', field: 'DepartmentName', width: 350, resizable: false, suppressMovable: true, sortable: true },
        //{ headerName: 'Description', field: 'Description', width: 200, resizable: false,suppressMovable: true, sortable: true },
        //{ headerName: 'Created By', field: 'CreatedBy', width: 150, resizable: false,suppressMovable: true, sortable: true },
        //{ headerName: 'Created On', field: 'CreatedOn', width: 150, resizable: false,suppressMovable: true, sortable: true },
        //{ headerName: 'Updated By', field: 'UpdatedBy', width: 150, resizable: false,suppressMovable: true, sortable: true },
        //{ headerName: 'Updated On', field: 'UpdatedOn', width: 150, resizable: false,suppressMovable: true, sortable: true },
        { headerName: 'Status', field: 'IsActive', width: 160, resizable: false, suppressMovable: true, sortable: true },
        {
            headerName: 'Action', field: '', width: 100, sortable: true, resizable: false, suppressMovable: true, suppressMovable: true, cellRenderer: function (params) {

                return '<button type="button" class="ingridbtn view-more-btn" data-bs-toggle="modal" data-bs-target="#usermodal" data-department-code="' + params.data.DepartmentCode + '" onclick="ShowMore(this)">View More</button>';
            }
        },
        {
            headerName: '', field: '', width: 50, sortable: true, resizable: false, suppressMovable: true, suppressMovable: true, cellRenderer: function (params) {

                return '<span class="fa fa-trash gridIcon" id=""></span>';
            }
        },
    ];
    var rowData = [];
    //
    $.ajax({
        url: '/Masters/SearchData1',
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
                    DepartmentId: value.DepartmentId,
                    DepartmentCode: value.DepartmentCode,
                    DepartmentName: value.DepartmentName,
                    Description: value.Description,
                    CreatedBy: value.CreatedBy,
                    CreatedOn: value.CreatedOn,
                    UpdatedBy: value.UpdatedBy,
                    UpdatedOn: value.UpdatedOn,
                    IsActive: value.IsActive,

                });

            });
            //
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
                onRowClicked: function (params)
                {
                    if (params.event.target.id === 'EditData') {
                        Click(params.data.DepartmentId);
                    } else {
                        ShowMore(params.data.DepartmentCode);
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

function ShowMore(departmentCode) {
    //
    $.ajax({
        url: '/Masters/GetDepartmentDetails',
        type: 'POST',
        dataType: 'json',
        data: {
            departmentCode: departmentCode
        },
        success: function (result) {
            var jsonData = result.Table1;
            if (jsonData.length > 0) {
                var departmentDetails = jsonData[0];
                $('#departmentCode').text(departmentDetails.DepartmentCode);
                $('#departmentName').text(departmentDetails.DepartmentName);
                $('#description').text(departmentDetails.Description);
                $('#createdBy').text(departmentDetails.CreatedBy);
                $('#createdOn').text(departmentDetails.CreatedOn);
                $('#updatedBy').text(departmentDetails.UpdatedBy);
                $('#updatedOn').text(departmentDetails.UpdatedOn);
                $('#status').text(departmentDetails.IsActive);
            }
        },
        error: function () {
            alert('Failed to retrieve department details.');
        }
    });
}



















function Click(DepartmentId) {

    let deptId = parseInt(DepartmentId);
    if (!isNaN(deptId)) {
        window.location.href = "/Masters/DepartmentMaster?ID=" + deptId;
    }
    else {
        console.error("Invalid DepartmentId:", DepartmentId);
    }
}


function Search() {
    //
    var keyword = $('#searchInput').val(); // Retrieve the keyword from the search input field
    var pagecount = 1; // Reset the page count to 1 after performing a search
    //
    GetData(pagecount, keyword); // Call the modified GetData function with the keyword
}



