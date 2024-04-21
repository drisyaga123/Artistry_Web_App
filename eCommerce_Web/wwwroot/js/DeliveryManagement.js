var orderId = 0;
$(document).ready(function () {
    getAllOrders();
})
function getAllOrders() {
    $("#loader").removeClass("d-none");
    $.ajax({
        url: apiUrls.get_all_usersorders,
        type: "POST",
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {
            var html = '';
            if (response.length > 0) {
                response.forEach(function (order) {
               
                    html = html + `<tr><td>${order.orderId}</td><td>${order.productName}</td><td>${order.totalPrice}</td>
                    <td><p><span>${order.firstName}</span> <span>${order.lastName}</span></p>
                                <p>${order.address}, ${order.state}, ${order.city}, ${order.pincode}</p>
                                <p>${order.landmark}</p><p class="mb-0">${order.phone}</p></td><td>${order.paymentMode}</td><td><div class="d-flex">
                    
                   ${order.status.toLowerCase() === 'delivered'
                        ? '<span class="badge bg-success">Delivered</span>'
                        : '<button onclick="openOtpModal('+order.id+')" class="btn btn-primary">Deliver</button>'}

                    </td></tr>`
                    });
                    $("#tBodyOrders").empty();
                    $("#tBodyOrders").append(html);
            }
            else {
                alertFailed("No orders found");
                //$("#addrSect").addClass('d-none');
                //$("#noAddress").removeClass('d-none');
            }
            $("#loader").addClass("d-none");
        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            alertFailed(xhr.responseText);

        }
    });
}
function deliverOrder() {
    
    if (orderId > 0) {
      
        var otp = $("#delivery_otp").val();
        if (otp == "" || otp.length != 6) {
            $("#delivery_otp").removeClass("is-valid").addClass("is-invalid");
            return false;
        }
        else {
            $("#delivery_otp").addClass("is-valid").removeClass("is-invalid");
        }
        $("#loader").removeClass("d-none");
        $.ajax({
            url: apiUrls.deliver_order,
            type: "POST",
            data: JSON.stringify({
                Id: orderId,
                OTP: $("#delivery_otp").val()
            }),
            contentType: "application/json",
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
            success: function (response) {
                $("#deliverModal").modal("hide");
                $("#loader").addClass("d-none");
                if (response != null) {
                    if (response.status.toLowerCase() === "success") {
                        alertSuccess(response.message);
                        getAllOrders();
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
                $("#deliverModal").modal("hide");
                $("#loader").addClass("d-none");
                alertFailed(xhr.responseText);

            }
        });
    }
}
function openOtpModal(id) {
    orderId = id;
    $("#delivery_otp").val('');
    $("#delivery_otp").removeClass("is-valid").removeClass("is-invalid");
    $("#deliverModal").modal("show");
}
function viewAddress(id) {

    if (id > 0) {
        $("#loader").removeClass("d-none");
        $.ajax({
            url: apiUrls.get_orderbyid,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                "Id": id
            }),
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },

            success: function (response) {
                $("#loader").addClass("d-none");
                var addrhtml = '';
                var pricehtml = '';
                if (response != null) {
                    let discount = (parseFloat(response.sellingAmount) - parseFloat(response.mrpAmount)) * parseFloat(response.quantity);

                    addrhtml = addrhtml + `<p><span>${response.firstName}</span> <span>${response.lastName}</span></p>
                                <p>${response.address}, ${response.state}, ${response.city}, ${response.pincode}</p>
                                <p>${response.landmark}</p>
                                <p class="mb-0">${response.phone}</p>`;
                    $("#addrDetailsDiv").empty();
                    $("#addrDetailsDiv").append(addrhtml);
                    
                    $('#viewAddrModal').modal('show');
                }
                else {
                    alertFailed("Failed");
                }
            },
            error: function (xhr, status, error) {
                $("#loader").addClass("d-none");
                alertFailed(xhr.responseText);

            }
        });
    }
}