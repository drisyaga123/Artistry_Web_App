var apiBaseUrl = "https://localhost:7033";
var webBaseUrl = window.location.origin;
var apiUrls = {
    login: apiBaseUrl + "/api/Auth/login",
    reg_user: apiBaseUrl + "/api/Auth/register-user",
    reg_seller: apiBaseUrl +"/api/Auth/register-seller",
    add_product: apiBaseUrl +"/api/Product/add-product",
    get_all_products : apiBaseUrl +"/api/Product/get-all-products",
    get_product_image : apiBaseUrl +"/api/Product/get-product-image",
    delete_product: apiBaseUrl +"/api/Product/delete_product",
    get_product: apiBaseUrl +"/api/Product/get-product",
    update_product: apiBaseUrl +"/api/Product/update-product",
    get_user_details: apiBaseUrl +"/api/Seller/get-user-details",
    update_dp: apiBaseUrl +"/api/Seller/update-dp",
    list_all_products: apiBaseUrl +"/api/Product/list-all-products",
    add_to_cart: apiBaseUrl +"/api/Cart/add-to-cart",
    get_cart_items: apiBaseUrl +"/api/Cart/get-cart-items",
    update_item_quantity : apiBaseUrl +"/api/Cart/update-item-quantity",
    delete_cart_item: apiBaseUrl +"/api/Cart/delete-cart-item",
    get_all_address: apiBaseUrl +"/api/Address/get-all-address",
    add_address: apiBaseUrl +"/api/Address/add-address",
    delete_address: apiBaseUrl +"/api/Address/delete-address",
    update_address : apiBaseUrl +"/api/Address/update-address",
    get_address : apiBaseUrl +"/api/Address/get-address",
    place_order: apiBaseUrl +"/api/Order/place-order",

}

var redirectUrl = {
    userLoginRedirection: webBaseUrl +"/Home/Main",
    sellerDashBoard: webBaseUrl +"/SellerDashboard/Account",
    userDashBoard: webBaseUrl +"/UserDashboard/UserDashboard",
    orderSummaryPage: webBaseUrl +"/Product/OrderSummary",
    adminLoginRedirection: "",
    sellerLoginRedirection:"",
    loginPage:webBaseUrl+"/Login/UserLogin"
}
$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    if (jqxhr.status === 401) {
        // Redirect to the login page
        window.location.href = '/Login/UserLogin';
    }
});
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

function toggleActive(element) {
    var parent = element.closest('.nav-item-bg');
    var siblings = parent.parentElement.querySelectorAll('.nav-item-bg');
    siblings.forEach(function (sibling) {
        sibling.classList.remove('active');
    });

    parent.classList.add('active');
}
function showAddrModal() {
    $("#btnUpdateAddr").addClass('d-none');
    $("#btnAddAddr").removeClass('d-none');
    $("#addAddressModal").modal('show');
}
function hideAddrModal() {
    $(".addrInput").each(function () {
        if ($(this).is("input, textarea")) {
            $(this).val("");
        } else if ($(this).is("select")) {
            $(this).val(null);
        }
    });
    $("#addAddressModal").modal('hide');
}
function validateAddressForm() {
    let isValid = true;
    var fields = $(".addrInput");
    fields.each(function () {
        if ($(this).val() == "" || ($(this).is("select") && $(this).val() == null)) {
            isValid = false;
            return false;
        }
    })
    return isValid;
}
function addAddress() {
    if (validateAddressForm()) {
        $("#loader").removeClass("d-none");
        var obj = {
            FirstName: $("#addrfirstname").val(),
            LastName: $("#addrlastname").val(),
            Address: $("#addraddress").val(),
            Landmark: $("#addrlandmark").val(),
            State: $("#addrstate").val(),
            Pincode: $("#addrzipcode").val(),
            City: $("#addrcity").val(),
            Phone: $("#addrphone").val()
        };
        $.ajax({
            url: apiUrls.add_address,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(obj),
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
            success: function (response) {
                $("#loader").addClass("d-none");
                hideAddrModal();
                if (response != null) {
                    if (response.status.toLowerCase() === "success") {
                        alertSuccess(response.message);
                        getAllAddresses();
                    }
                    else {
                        alertFailed(response.message);
                    }

                }
                else {
                    alertFailed("Failed");
                }
            },
            error: function (xhr, status, error) {
                $("#loader").addClass("d-none");
                hideAddrModal();
                alertFailed(xhr.responseText);

            }
        });
    }
    else {
        alertFailed("Please fill all fields");
    }
}