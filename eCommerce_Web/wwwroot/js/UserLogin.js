var virtualPath = $("#hdnVirtualPath").val();
function submitLogin() {

    $("#username,#password").removeAttr("style");
    var validation = true;
    if ($("#username").val() === "") {
        validation = false;
        $("#username").attr("style", 'border-color:red');
    }

    if ($("#password").val() === "") {
        validation = false;
        $("#password").attr("style", 'border-color:red');
    }

    if (validation) {
        $("#loader").removeClass("d-none");
        var formData = {
            username: $("#username").val(),
            password: $("#password").val()
        };

        $("#btnsubmit").text("Login.....");
        $("#btnsubmit").attr("disabled", true);
        $.ajax({
            type: "POST",
            url: apiUrls.login,
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                
                localStorage.setItem("jwtToken", response.token);
                localStorage.setItem("username", $("#username").val());
                localStorage.setItem("islogin", "1");
                if (response.role.toLowerCase() === "customer") {
                    window.location.href = redirectUrl.userLoginRedirection;
                    localStorage.setItem("isCustomerLogin", "1");
                }
                else if (response.role.toLowerCase() === "admin") {
                    window.location.href = redirectUrl.adminDashBoard;
                }
                else if (response.role.toLowerCase() === "seller") {
                    window.location.href = redirectUrl.sellerDashBoard;
                }
                else if (response.role.toLowerCase() === "deliverymanager") {
                    window.location.href = redirectUrl.deliveryDashBoard;
                }


            },
            error: function (xhr, status, error) {
                $("#loader").addClass("d-none");
                alertFailed(xhr.responseText);

            }

        });
    }

}
$(document).ready(function () {
    const node = document.getElementById("username");
    const node2 = document.getElementById("password");
    node.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            submitLogin();
        }
    });

    node2.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            submitLogin();
        }
    });

    $('#username').keypress(function (e) {
        var regex = new RegExp("^[a-zA-Z0-9_.]+$");
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }
        e.preventDefault();
        return false;
    });

    $("#login-form").submit(function (event) {
        event.preventDefault();
        submitLogin();
    });


    $("#showPassword").on("click", function (event) {
        event.preventDefault();

        if ($(this).text() == "Show password") {
            $(this).text("Hide password");
            $('#password').prop("type", "text");
        }
        else {
            $(this).text("Show password");
            $('#password').prop("type", "password");
        }

    });


   
});