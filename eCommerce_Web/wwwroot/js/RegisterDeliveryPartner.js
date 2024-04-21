
function validateRegUser() {
    let name = $("#userName").val();
    let email = $("#userEmail").val();
    let pswd = $("#userPswd").val();
    let cPswd = $("#userCPswd").val();
    let isValid = true;
    if (name == "" || name == null) {
        isValid = false;
        $("#name-error").text("Name is required!");
        $("#userName").removeClass("is-valid").addClass("is-invalid");
    }
    else {
        $("#userName").removeClass("is-invalid").addClass("is-valid");

    }
    if (email == "" || email == null) {
        isValid = false;
        $("#userEmail").removeClass("is-valid").addClass("is-invalid");
        $("#email-error").text("Email is required!");
    }
    else if (!validateEmail(email)) {
        isValid = false;
        $("#userEmail").removeClass("is-valid").addClass("is-invalid");
        $("#email-error").text("Invalid email!");
    }
    else {
        $("#userEmail").removeClass("is-invalid").addClass("is-valid");

    }
    if (pswd == "" || pswd == null) {
        isValid = false;
        $("#userPswd").removeClass("is-valid").addClass("is-invalid");
        $("#pswd-error").text("Password is required!");
    }
    else {
        $("#userPswd").removeClass("is-invalid").addClass("is-valid");
    }
    if (cPswd == "" || cPswd == null) {
        isValid = false;
        $("#userCPswd").removeClass("is-valid").addClass("is-invalid");
        $("#cpswd-error").text("Confirm password is required!");

    }
    else {

        if (pswd != cPswd) {
            isValid = false;
            $("#userCPswd").removeClass("is-valid").addClass("is-invalid");
            $("#cpswd-error").text("Password mismatch!");
        }
        else {
            $("#userCPswd").removeClass("is-invalid").addClass("is-valid");

        }
    }


    return isValid;
}

function registerUser() {
    if (validateRegUser()) {
        let obj = {
            Username: $("#userName").val().trim(),
            Email: $("#userEmail").val().trim(),
            Password: $("#userPswd").val().trim()
        }
        $.ajax({
            type: "POST",
            url: apiUrls.register_deliverymanager,
            contentType: "application/json",
            data: JSON.stringify(obj),
            success: function (response) {
                if (response != null) {
                    alert(response.message);
                    window.location.href = redirectUrl.loginPage;
                }
            },
            error: function (xhr, status, error) {

                alert(xhr.responseText);

            }

        });
    }

}