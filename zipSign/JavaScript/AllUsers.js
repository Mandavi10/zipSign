var pagecount = 1;
$(document).ready(function () {
    
    //var UserMasterID = sessionStorage.getItem('UserId');
    //if (UserMasterID == "" || UserMasterID == null) {
    //    window.location.href = "/Login/Index";
    //}
    GetData(pagecount);
});
function GetData(pagecount, keyword) {
    ////
    $("#myGrid").html("");
    var columnDefs = [
        { headerName: 'Sr. No.', field: 'srNo', width: 80, sortable: true, resizable: false, suppressMovable: true, valueGetter: "node.rowIndex + 1"},
        {
            headerName: 'User Code', field: 'UserCode', width: 150, resizable: false, sortable: true, suppressMovable: true, cellRenderer: function (params)
            {
                return '<a href="#" onclick="Click(\'' + params.data.Userid + '\')" id="EditData">' + params.data.UserCode + '</a>';
            }

        },
        //{ headerName: 'Department Code', field: 'DepartmentCode', width: 180, resizable: true, sortable: true, cellRenderer: function (params) { return '<a href="#" onclick="Click(\'' + params.data.DepartmentId + '\')" id="EditData">' + params.data.DepartmentCode + '</a>'; }, },

        { headerName: 'User Name', field: 'Username', width: 200, resizable: false, sortable: true, suppressMovable: true, },
        { headerName: 'User Type', field: 'UserType', width: 150, resizable: false, sortable: true, suppressMovable: true, },
        { headerName: 'Mobile App', field: 'Mobileapp', width: 150, resizable: false, sortable: true, suppressMovable: true, },
        { headerName: 'Status', field: 'Active', width: 150, resizable: false, sortable: true, suppressMovable: true, },
        {
            headerName: 'Action', field: '', width: 100, sortable: true, resizable: false, suppressMovable: true, cellRenderer: function (params) {

                return '<button type="button" class="ingridbtn view-more-btn" data-bs-toggle="modal" data-bs-target="#usermodal" data-User-Code="' + params.data.UserCode + '" onclick="ShowMore(this)">View More</button>';
            }
        
        },
        {
            headerName: '', field: '', width: 70, sortable: true, resizable: false, suppressMovable: true, suppressMovable: true, cellRenderer: function (params) {

                return '<span class="fa fa-trash gridIcon" id=""></span>';
            }
        },

    ];
    var rowData = [];
    $.ajax({
        url: '/Users/SearchData1',
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
                    Userid: value.Userid,
                    UserCode: value.UserCode,
                    Username: value.Username,
                    EmailId: value.EmailId,
                    MobileNo: value.MobileNo,
                    UserType: value.UserType,
                    Department: value.Department,
                    Designation: value.Designation,
                    CreatedBy: value.CreatedBy,
                    CreatedOn: value.CreatedOn,
                    ModifyBy: value.ModifyBy,
                    ModifyOn: value.ModifyOn,
                    Active: value.Active,
                    Mobileapp: value.Mobileapp,
                    SpecificDomaincontrol: value.SpecificDomaincontrol
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
                //For Previous Count
                html1 += "<li class='page-item'><a href='#' class='page-link' onclick='GetData(" + (parseInt(page) - 1) + ")'>Previous</a></li>";
            } else {
                html1 += "<li class='page-item disabled'><a href='#' class='page-link'>Previous</a></li>";
            }

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
                    if (params.event.target.id === 'EditData')
                    {
                        Click(params.data.DepartmentId);
                    } else
                    {
                        ShowMore(params.data.UserCode);
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


var keyword;

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

                                   
function update()
{
    //
    var userCode = $('#usercode').text();
    var isBlocked = $('#blockCheckbox').prop('checked');
    $.ajax({
        url: '/Users/UpdateUserDetails',
        type: 'POST',
        data: { UserCode: userCode, IsBlocked: isBlocked },
        success: function (result) {
            if (result.status==1) {
                window.location.href = '../Users/AllUsers'
            } else {
                //alert('Failed to update user details.');
            }
        },
        error: function () {
            // alert('An error occurred during the update.');
        }
    });
}

   
   




