$(document).ready(function () {
    var UserMasterID = sessionStorage.getItem('UserId');
    if (UserMasterID == "" || UserMasterID == null) {
        window.location.href = "/Login/Index";
    }

});


function SaveProfile() {
    var gender = $("#chckmale").is(":checked");
    //
    $.ajax({
        type: 'POST',
        url: '/Users/ProfileInsert',
        data: {
            ProId: $('#DepartmentId').val(),
            FirstName: $('#fname').val(),
            LastName: $('#lname').val(),
            Add1: $('#add1').val(),
            Add2: $('#add2').val(),
            Pin: $('#pincode').val(),
            Gender: $('#gender').val(),
            DOB: $('#caldob').val(),
            GST: $('#gst').val(),
            ComWebURL: $('#comweburl').val(),
            EDomian: $('#edomain').val(),
        },
        success: function (result) {
            alert("Profile Saved");
        },
        error: function (ex) {
            alert("Error");
        }
    });
}

$(function () {
    //
    // Profile picture upload
    $('#btn-upload-photo').on('click', function () {
        $('#filePhoto').trigger('click');
    });
    //
    $('#filePhoto').on('change', function () {
        var file = $(this)[0].files[0];
        var formData = new FormData();
        formData.append('file', file);
        //
        $.ajax({
            type: 'POST',
            url: '/Users/UploadProfilePicture',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                if (result.status === "200") {
                    var fileName = result.fileName;
                    var imageUrl = '/Content/images/profiles/' + fileName;
                    $('#profilePhoto').attr('src', imageUrl);
                    $('.user-photo').attr('src', imageUrl);
                    //$('.user-photo').attr('alt', imageUrl);
                } else {
                    alert('Failed to upload profile picture.');
                }
            },
            error: function (ex) {
                alert('Failed to upload profile picture.');
            },
        });
    });
})