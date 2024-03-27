$(document).ready(function () {
    getAllCartItems();

})

function updateQuantity(quantity, productId) {
    if (quantity > 0 && productId > 0) {
        $("#loader").removeClass("d-none");
        $.ajax({
            url: apiUrls.update_item_quantity,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                "ProductId": productId,
                "Quantity": quantity
            }),
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },

            success: function (response) {
                $("#loader").addClass("d-none");
                if (response != null) {
                    if (response.status.toLowerCase() === "success") {
                        alertSuccess(response.message);
                        getAllCartItems();
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
}
function plusQuantity(element,productId) {
    var inputElement = element.parentNode.querySelector('input[type=number]');
    inputElement.stepUp();
    updateQuantity(inputElement.value, productId);
}
function minusQuantity(element, productId) {
    var inputElement = element.parentNode.querySelector('input[type=number]');
    inputElement.stepDown();
    updateQuantity(inputElement.value, productId);
}

function removeCartItem(id) {
    if (id > 0) {
        $("#loader").removeClass("d-none");
        $.ajax({
            url: apiUrls.delete_cart_item,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                "Id": id
            }),
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },

            success: function (response) {
                $("#loader").addClass("d-none");
                if (response != null) {
                    if (response.status.toLowerCase() === "success") {
                        alertSuccess(response.message);
                        getAllCartItems();
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
}
 
          
function getAllCartItems() {

        $("#loader").removeClass("d-none");
        $.ajax({
            url: apiUrls.get_cart_items,
            type: "GET",
            contentType: "application/json",
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },

            success: function (response) {
                $("#loader").addClass("d-none");
                if (response.length>0) {
                    $("#cartEmptyDiv").addClass('d-none');
                    var html = '';
                    var totalPrice = 0;
                    var itemCount = 0;
                    var itemKeyword = '';
                    var deliveryCharge = 40;
                    var totalMrp = 0;
                    var discount = 0;
                    response.forEach(function (product) {
          
                        html = html + `<div id="cartItemsDiv"><div class="card rounded-3 mb-4"><div class="card-body p-4"><div class="row d-flex justify-content-between align-items-center"><div class="col-md-2 col-lg-2 col-xl-2"><img src="${product.productImage}" class="img-fluid rounded-3" alt="Cotton T-shirt"></div><div class="col-md-3 col-lg-3 col-xl-3">  <p class="lead fw-normal mb-2">${product.productName}</p></div><div class="col-md-3 col-lg-3 col-xl-2 d-flex"><button class="btn btn-link px-2" onclick="minusQuantity(this,${product.productId})"><i class="fas fa-minus"></i></button>
                        <input onchange="updateQuantity();" min="1" name="quantity" value="${product.quantity}" type="number" class="form-control form-control-sm" /><button class="btn btn-link px-2" onclick="plusQuantity(this,${product.productId})"><i class="fas fa-plus"></i></button></div><div class="col-md-3 col-lg-2 col-xl-2 offset-lg-1"><h5 class="mb-0">₹${product.sellingAmount}</h5></div><div class="col-md-1 col-lg-1 col-xl-1 text-end"><a onclick="removeCartItem(${product.cartId})" class="text-danger"><i class="fas fa-trash fa-lg"></i></a></div></div></div></div>`
                        totalPrice = totalPrice + parseFloat(product.sellingAmount);
                        totalMrp = totalMrp + (parseFloat(product.mrpAmount) * parseFloat(product.quantity));
                        itemCount++;
                    });
                    itemKeyword = (itemCount <= 1) ? "item" : "items";
                    deliveryCharge = (totalPrice >= 500 ? 0 : 40);
                    discount = totalPrice - totalMrp;
                    if (html != '') {
                        html = html + ` <div class="card mb-4"> <div class="card-body"><h5>PRICE DETAILS</h5><hr><div class="d-flex row"><div class="col-3">Price (${itemCount} ${itemKeyword})</div><div class="col-3">₹${totalPrice}</div></div>
                        <div class="d-flex row mt-2"><div class="col-3">Discount</div><div class="col-3 text-success">₹${discount}</div></div><div class="d-flex row mt-2"><div class="col-3">Delivery Charges</div><div class="col-3">${deliveryCharge > 0 ? '₹' + deliveryCharge : '<span class="text-success">Free</span>'}</div></div></div></div>
                        <div class="card"><div class="card-body"><button type="button" class="btn btn-warning w-100 btn-block btn-lg">Proceed to Pay</button></div></div>`
                    }
                    html=html+'</div>'
                    $("#cartContainer").empty();
                    $("#cartContainer").append(html);
                }
                else {
                    $("#cartEmptyDiv").removeClass('d-none');
                    $("#cartItemsDiv").addClass('d-none');
                }
            },
            error: function (xhr, status, error) {
                $("#loader").addClass("d-none");
                alertFailed(xhr.responseText);

            }
        });
    
}