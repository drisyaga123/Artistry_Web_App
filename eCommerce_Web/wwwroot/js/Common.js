﻿var apiBaseUrl = "https://localhost:7033";
var webBaseUrl = window.location.origin;
var apiUrls = {
    login: apiBaseUrl + "/api/Auth/login",
    reg_user: apiBaseUrl + "/api/Auth/register-user",
    reg_seller: apiBaseUrl +"/api/Auth/register-seller"
}

var redirectUrl = {
    userLoginRedirection: webBaseUrl +"/Home/Main",
    adminLoginRedirection: "",
    sellerLoginRedirection:"",
    loginPage:webBaseUrl+"/Login/UserLogin"
}

function logoutUser() {
    localStorage.removeItem("jwtToken");
    localStorage.removeItem("username");
    localStorage.removeItem("islogin");
    window.location.href = redirectUrl.loginPage;
}
function loginPage() {
    window.location.href = redirectUrl.loginPage;
}

function showSection(sectionId) {
    // Hide all sections
   $('.sections').addClass('d-none');

    var selectedSection = $("#"+sectionId);
    if (selectedSection) {
        selectedSection.removeClass('d-none')
    }
}
//common validations
function validateEmail(email) {
    var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}



// Custom Alert functions
function alertSuccess(msg) {
    var growl = $.bootstrapGrowl(msg, {
        type: "success",
        ele: "body",
        offset: {
            from: "top",
            amount: 100
        },
        align: "center",
        width: 250,
        delay: 1000,
        allow_dismiss: true,
        stackup_spacing: 10
    });

    // Find the close button inside the growl alert
    var closeButton = $(growl).find(".close");

    // Attach click event listener to the close button
    closeButton.on("click", function () {
        $(growl).remove(); // Remove the growl alert when close button is clicked
    });
}

function alertFailed(msg) {
    var growl = $.bootstrapGrowl(msg, {
        type: "danger",  // info, success, warning and danger
        ele: "body",
        offset: {
            from: "top",
            amount: 100
        },
        align: "center",
        width: 250,
        delay: 1000,
        allow_dismiss: true,
        stackup_spacing: 10
    });

    // Find the close button inside the growl alert
    var closeButton = $(growl).find(".close");

    // Attach click event listener to the close button
    closeButton.on("click", function () {
        $(growl).remove(); // Remove the growl alert when close button is clicked
    });
}