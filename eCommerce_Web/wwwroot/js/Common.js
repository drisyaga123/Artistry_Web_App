var apiBaseUrl = "https://localhost:7033";
var webBaseUrl = window.location.origin;
var apiUrls = {
    login: apiBaseUrl + "/api/Auth/login",
    reg_user: apiBaseUrl + "/api/Auth/register-user",
    reg_seller: apiBaseUrl +"/api/Auth/register-seller",
    register_deliverymanager : apiBaseUrl +"/api/Auth/register-deliverymanager",
    add_product: apiBaseUrl +"/api/Product/add-product",
    get_all_products : apiBaseUrl +"/api/Product/get-all-products",
    get_product_image : apiBaseUrl +"/api/Product/get-product-image",
    delete_product: apiBaseUrl +"/api/Product/delete-product",
    get_product: apiBaseUrl +"/api/Product/get-product",
    update_product: apiBaseUrl +"/api/Product/update-product",
    get_user_details: apiBaseUrl +"/api/Seller/get-user-details",
    update_dp: apiBaseUrl +"/api/Seller/update-dp",
    list_all_products: apiBaseUrl +"/api/Product/list-all-products",
    add_to_cart: apiBaseUrl +"/api/Cart/add-to-cart",
    get_cart_items: apiBaseUrl +"/api/Cart/get-cart-items",
    update_item_quantity : apiBaseUrl +"/api/Cart/update-item-quantity",
    delete_cart_item: apiBaseUrl +"/api/Cart/delete-cart-item",
    get_cart_itemscount: apiBaseUrl +"/api/Cart/get-cart-itemscount",
    get_all_address: apiBaseUrl +"/api/Address/get-all-address",
    add_address: apiBaseUrl +"/api/Address/add-address",
    delete_address: apiBaseUrl +"/api/Address/delete-address",
    update_address : apiBaseUrl +"/api/Address/update-address",
    get_address : apiBaseUrl +"/api/Address/get-address",
    place_order: apiBaseUrl +"/api/Order/place-order",
    get_all_orders : apiBaseUrl +"/api/Order/get-all-orders",
    get_all_usersorders: apiBaseUrl +"/api/Order/get-all-usersorders",
    cancel_order : apiBaseUrl +"/api/Order/cancel-order",
    get_orderbyid : apiBaseUrl +"/api/Order/get-orderbyid",
    get_all_orderstoship : apiBaseUrl +"/api/Order/get-all-orderstoship",
    approve_order : apiBaseUrl +"/api/Order/approve-order",
    get_all_sellerdetails : apiBaseUrl +"/api/Seller/get-all-sellerdetails",
    get_all_customerdetails: apiBaseUrl +"/api/Customer/get-all-customerdetails",
    deliver_order: apiBaseUrl +"/api/Order/deliver-order",
    rate_product : apiBaseUrl +"/api/Order/rate-product",
    get_all_reviews: apiBaseUrl +"/api/Product/get-all-reviews",

}

var redirectUrl = {
    userLoginRedirection: webBaseUrl +"/Home/Main",
    sellerDashBoard: webBaseUrl +"/SellerDashboard/Account",
    userDashBoard: webBaseUrl + "/UserDashboard/UserDashboard",
    adminDashBoard: webBaseUrl + "/AdminDashboard/Customers",
    deliveryDashBoard: webBaseUrl + "/UserDashboard/DeliveryManagement",
    orderSummaryPage: webBaseUrl +"/Product/OrderSummary",
    
    loginPage:webBaseUrl+"/Login/UserLogin"
}
$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    if (jqxhr.status === 401) {
        // Redirect to the login page
        logoutUser();

    }
});
function logoutUser() {
    localStorage.removeItem("jwtToken");
    localStorage.removeItem("username");
    localStorage.removeItem("islogin");
    localStorage.removeItem("isCustomerLogin");
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

function formatDate(time) {

    let date = new Date(time);

    // Get day, month, and year
    let day = date.getUTCDate();
    let month = date.getUTCMonth() + 1; // Month starts from 0
    let year = date.getUTCFullYear();

    // Format day, month, and year with leading zeros if necessary
    let formattedDay = day < 10 ? "0" + day : day;
    let formattedMonth = month < 10 ? "0" + month : month;

    // Construct the date string in dd-mm-yyyy format
    let formattedDate = `${formattedDay}-${formattedMonth}-${year}`;
    return formattedDate;
}
