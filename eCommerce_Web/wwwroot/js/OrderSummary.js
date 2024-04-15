let cartId = 0;
var prdId = 0;
$(document).ready(function () {
    $("#summaryList").removeClass("d-none");
    $("#orderSuccess").addClass("d-none");
    $("#loader").removeClass("d-none");
    var hdnProdField = $("#hdnProductId");

    if (hdnProdField.length > 0) {
        prdId = hdnProdField.val();
    }
    listAllCartItems(prdId,function () {
        getAllDelAddresses(function () {
            $("#loader").addClass("d-none");

        })
    })

})
function placeOrder() {
    $("#loader").removeClass("d-none");
    var hdnProdField = $("#hdnProductId");
    var prodId = 0;
    if (hdnProdField.length > 0) {
        prodId = hdnProdField.val();
    }
    var selectedAddressId = $('input[name="addressRadio"]:checked').val();
    var selectedPayMode = $('input[name="paymentMethod"]:checked').val();
    var obj = {
        ProductId: prodId,
        TotalPrice: $('[data-totalprice]').data('totalprice'),
        DeliveryCharge: $('[data-deliverycharge]').data('deliverycharge'),
        Discount: $('[data-discount]').data('discount'),
        AddressId: selectedAddressId,
        PaymentMode: selectedPayMode,
        Quantity: 1
    };
 
    $.ajax({
        url: apiUrls.place_order,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(obj),
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },

        success: function (response) {
            $("#loader").addClass("d-none");
            $("#summaryList").addClass("d-none");
            if (response != null) {
                if (response.status.toLowerCase() === "success") {
                    $("#summaryList").addClass("d-none");
                    $("#orderSuccess").removeClass("d-none");
                    getCartItemCount();
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
            alertFailed(xhr.responseText);

        }
    });
}
function getAllDelAddresses(callback) {
    $.ajax({
        url: apiUrls.get_all_address,
        type: "GET",
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {
            var html = '';
            if (response.length > 0) {
                response.forEach(function (addr) {
                    html += `
            <div class="form-check mb-2">
                <input class="form-check-input" type="radio" name="addressRadio" id="address${addr.id}" value="${addr.id}">
                <label class="form-check-label" for="address${addr.id}">
                    <div class="card bg-light mb-0">
                        <div class="card-body">
                                <p><span>${addr.firstName}</span> <span>${addr.lastName}</span></p>
                                <p>${addr.address}, ${addr.state}, ${addr.city}, ${addr.pincode}</p>
                                <p>${addr.landmark}</p>
                                <p class="mb-0">${addr.phone}</p>
                        </div>
                    </div>
                </label>
            </div>`;
                });
                $("#addrSect").removeClass('d-none');
                $("#noAddress").addClass('d-none');
                $("#addrSect").empty();
                $("#addrSect").append(html);
                callback();
            }
            else {
                $("#addrSect").addClass('d-none');
                $("#noAddress").removeClass('d-none');
            }

        },
        error: function (xhr, status, error) {
            alertFailed(xhr.responseText);

        }
    });
}
function listAllCartItems(id,callback) {

    $("#loader").removeClass("d-none");
    $.ajax({
        url: apiUrls.get_cart_items,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            Id:id
        }),
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },

        success: function (response) {
            $("#loader").addClass("d-none");
            if (response.length > 0) {
                $("#cartEmptyDiv").addClass('d-none');
                var html = '';
                var subTotal = 0;
                var totalPrice = 0;
                var itemCount = 0;
                var itemKeyword = '';
                var deliveryCharge = 40;
                var totalMrp = 0;
                var discount = 0;
                response.forEach(function (product) {
                    html = html + `<div class="row  justify-content-between mb-2">
    <div class="col-auto col-md-7">
        <div class="media flex-column flex-sm-row">
            <img class=" img-fluid" src="${product.productImage}" width="62" height="62">
            <div class="media-body  my-auto">
                <div class="row ">
                    <div class="col-auto"><p class="mb-0"><b>${product.productName}</b></div>
                </div>
            </div>
        </div>
    </div>
    <div class=" pl-0 flex-sm-col col-auto  my-auto"> <p class="boxed-1">${product.quantity} Nos</p></div>
    <div class=" pl-0 flex-sm-col col-auto  my-auto "><p><b>₹${product.sellingAmount}</b></p></div>
</div>
<hr class="my-0">`;
                   
                    subTotal = subTotal + parseFloat(product.sellingAmount);
                    totalMrp = totalMrp + (parseFloat(product.mrpAmount) * parseFloat(product.quantity));
                    itemCount++;
                });
                itemKeyword = (itemCount <= 1) ? "item" : "items";
                deliveryCharge = (subTotal >= 500 ? 0 : 40);
                totalPrice = subTotal + parseFloat(deliveryCharge)
                discount = totalPrice - totalMrp;
                if (html != '') {
                    html = html + `<div class="row justify-content-between">
    <div class="col-6"><p class="mb-1"><b>Subtotal (${itemCount} ${itemKeyword})</b></p></div>
    <div class="flex-sm-col col-auto"><p class="mb-1" data-subtotal="${subTotal}"><b>₹${subTotal}</b></p></div>
</div>
<div class="row justify-content-between">
    <div class="col-6"><p class="mb-1"><b>Discount</b></p></div>
    <div class="flex-sm-col col-auto"><p class="mb-1 text-success" data-discount="${discount}"><b>₹${discount}</b></p></div>
</div>
<div class="row justify-content-between">
    <div class="col-6"><p><b>Delivery Charges</b></p></div>
    <div class="flex-sm-col col-auto"><p class="mb-1" data-deliverycharge="${deliveryCharge}"><b>${deliveryCharge > 0 ? '₹' + deliveryCharge : '<span class="text-success">Free</span>'}</b></p> </div>
</div>
<div class="row justify-content-between">
    <div class="col-6"><p><b>Total</b></p></div>
    <div class="flex-sm-col col-auto"><p class="mb-1" data-totalprice="${totalPrice}"><b>${totalPrice}</b></p> </div>
</div>
<hr class="my-0">`
                }

                $("#itemDetails").empty();
                $("#itemDetails").append(html);
            }
            callback();

        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            alertFailed(xhr.responseText);

        }
    });

}