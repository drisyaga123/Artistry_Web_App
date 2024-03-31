$(document).ready(function () {
    
    $('input[name="inlineRadioOptions"]').change(function () {
        if ($('#rdArtist').is(':checked')) {

            $('#divOrgaztn').addClass('d-none');
        } else if ($('#rdOrganization').is(':checked')) {
            $('#divOrgaztn').removeClass('d-none');
        }
    });
})
function validateRegSeller() {
    
    let name = $("#seller_Name").val();
    let email = $("#seller_Email").val();
    let pswd = $("#seller_Psd").val();
    let cPswd = $("#seller_CPsd").val();
    let bio = $("#seller_Bio").val();
    let org = $("#seller_Org").val();

    let isValid = true;
    if (name == "" || name == null) {
        isValid = false;
        $("#name-error").text("Name is required!");
        $("#seller_Name").removeClass("is-valid").addClass("is-invalid");
    }
    else {
        $("#seller_Name").removeClass("is-invalid").addClass("is-valid");

    }
    if (email == "" || email == null) {
        isValid = false;
        $("#seller_Email").removeClass("is-valid").addClass("is-invalid");
        $("#email-error").text("Email is required!");
    }
    else if (!validateEmail(email)) {
        isValid = false;
        $("#seller_Email").removeClass("is-valid").addClass("is-invalid");
        $("#email-error").text("Invalid email!");
    }

    else {
        $("#seller_Email").removeClass("is-invalid").addClass("is-valid");

    }

    if (pswd == "" || pswd == null) {
        isValid = false;
        $("#seller_Psd").removeClass("is-valid").addClass("is-invalid");
        $("#pswd-error").text("Password is required!");
    }
    else {
        $("#seller_Psd").removeClass("is-invalid").addClass("is-valid");
    }

    if (cPswd == "" || cPswd == null) {
        isValid = false;
        $("#seller_CPsd").removeClass("is-valid").addClass("is-invalid");
        $("#cpswd-error").text("Confirm password is required!");
    }
    else {
        if (pswd != cPswd) {
            isValid = false;
            $("#seller_CPsd").removeClass("is-valid").addClass("is-invalid");
            $("#cpswd-error").text("Password mismatch!");
        }
        else {
            $("#seller_CPsd").removeClass("is-invalid").addClass("is-valid");

        }

    }

    if (bio == "" || bio == null) {
        isValid = false;
        $("#seller_Bio").removeClass("is-valid").addClass("is-invalid");
        $("#bio-error").text("Biography is required!");
    }
    else {
        $("#seller_Bio").removeClass("is-invalid").addClass("is-valid");

    }
    if ($('#rdOrganization').is(':checked')) {
        if (org == "" || org == null) {
            isValid = false;
            $("#seller_Org").removeClass("is-valid").addClass("is-invalid");
            $("#org-error").text("Organization is required!");
        }
        else {
            $("#seller_Org").removeClass("is-invalid").addClass("is-valid");

        }
    }
   
    return isValid;

}

function registerSeller() {
    if (validateRegSeller()) {
        
        let obj = {
            Username: $("#seller_Name").val().trim(),
            Email: $("#seller_Email").val().trim(),
            Password: $("#seller_Psd").val().trim(),
            Bio: $("#seller_Bio").val().trim(),
            Organization: $("#seller_Org").val().trim()
        }
        $.ajax({
            type: "POST",
            url: apiUrls.reg_seller,
            contentType: "application/json",
            data: JSON.stringify(obj),
            success: function (response) {
                if (response != null) {
                    alert(response.message);
                    window.location.href = redirectUrl.loginPage;
                }
            },
            error: function (xhr, status, error) {

                alert(xhr.responseJSON.message);

            }

        });
    }

}