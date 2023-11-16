// Inside your script tag or external script file
$(document).ready(function () {
    $("#Proceed").on("click", function () {
        var UserMasterID = sessionStorage.getItem('UserId');
        if (UserMasterID == "" || UserMasterID == null) {
            window.location.href = "/Login/Index";
        }
        var checkedCheckboxData = [];

        $("[type='checkbox']:checked").each(function () {
            
            var checkboxId = $(this).attr("id");
            var checkboxName = $(this).siblings("span").text();
            var readValue = $(this).closest(".partipage").find(".readwr input[type='checkbox']:eq(0)").prop("checked");
            var writeValue = $(this).closest(".partipage").find(".readwr input[type='checkbox']:eq(1)").prop("checked");
            var link = $(this).siblings("a.hidden-link").attr("href");
            checkedCheckboxData.push({
                id: checkboxId,
                name: checkboxName,
                read: readValue,
                write: writeValue,
                link: link
            });
        });
        var roleName = $('#text-input1').val();
        var description = $('#text-input2').val();
        $.ajax({
            url: "/zipSign/SaveRolesAndPermissions",
            method: "POST",
            data: {
                roleName: roleName,
                description: description,
                checkboxData: JSON.stringify(checkedCheckboxData)
            },
            success: function (result) {
                $("#message").html(result.message);
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
                $("#message").html("Error occurred while saving roles and permissions.");
            }
        });
    });
});

