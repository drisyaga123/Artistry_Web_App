$(document).ready(function () {
    getAllOrders();
})
function viewMore(id) {
   
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
                    pricehtml = pricehtml + `<div class="row">
                                    <div class="col-6">Price</div>
                                    <div class="col-6">₹ ${response.sellingAmount}</div>
                                </div>
                                <div class="row">
                                    <div class="col-6">Quantity</div>
                                    <div class="col-6">${response.quantity}</div>
                                </div>
                                <div class="row">
                                    <div class="col-6">Discount</div>
                                    <div class="col-6">₹ ${discount}</div>
                                </div>
                                <div class="row">
                                    <div class="col-6">Delivery Charge</div>
                                    <div class="col-6">${response.deliveryCharge > 0 ? '₹' + response.deliveryCharge : '<span class="text-success">Free</span>'}</div>
                                </div>`;
                    $("#priceDetailsDiv").empty();
                    $("#priceDetailsDiv").append(pricehtml);
                    $('#viewMoreModal').modal('show');
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
function cancelOrder(id) {
    
    $('#modalContent').html('Are you sure you want to cancel this order?');
    $('#confirmModal').modal('show');
    $('#confirmBtn').on('click', function () {
        if (id > 0) {

            $("#loader").removeClass("d-none");
            $.ajax({
                url: apiUrls.cancel_order,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({
                    "Id": id
                }),
                headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },

                success: function (response) {
                    $("#loader").addClass("d-none");
                    $('#confirmModal').modal('hide');
                    if (response != null) {
                        if (response.status.toLowerCase() === "success") {
                            getAllOrders();
                         
                            alertSuccess(response.message);
                         
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
    });
}
function getAllOrders() {
    $("#loader").removeClass("d-none");
    $.ajax({
        url: apiUrls.get_all_orders,
        type: "POST",
        data: JSON.stringify({
            Id: id
        }),
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {
            var html = '';
            if (response.length > 0) {
                response.forEach(function (order) {
                    html += `
        <div class="card card-stepper mb-3" style="border-radius: 16px;">
          <div class="card-header p-4">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <p class="text-muted mb-2">Order ID <span class="fw-bold text-body">${order.orderId}</span></p>
                <p class="text-muted mb-0">Placed On <span class="fw-bold text-body">${formatDate(order.createdDate)}</span></p>
              </div>
             
            </div>
          </div>
          <div class="card-body p-4">
            <div class="d-flex flex-row mb-4 pb-2">
              <div class="flex-fill">
                <h5 class="bold">${order.productName}</h5>
                <p class="text-muted">Qt: ${ order.quantity } ${ order.quantity > 1 ? 'items' : 'item' }</p>
                <h4 class="mb-3">₹${order.sellingAmount} <span class="small text-muted"> ${order.paymentMode} </span></h4>
              </div>
              <div>
                <img class="align-self-center img-fluid" src="${order.productImage}" width="100">
              </div>
            </div>
            <ul id="progressbar-1" class="mx-0 mt-0 mb-5 px-0 pt-0 pb-4 ${order.status.toLowerCase() === 'cancelled' ?'stepperChange':''}">
              <li class="step0 ${order.status.toLowerCase() === 'placed' || order.status.toLowerCase() === 'shipped' || order.status.toLowerCase() === 'delivered' || order.status.toLowerCase() === 'cancelled' ? 'active' : ''}" id="step1"><span style="margin-left: 22px; margin-top: 12px;">PLACED</span></li>
              <li class="step0 ${order.status.toLowerCase() === 'delivered' || order.status.toLowerCase() === 'shipped' ? 'active' : ''} ${order.status.toLowerCase() === 'cancelled' ? 'd-none' : ''} text-center" id="step2"><span>SHIPPED</span></li>
              <li class="step0 ${order.status.toLowerCase() === 'cancelled' ? 'active' : 'd-none'} text-center text-danger" id="step2"><span style="margin-left: 210px;">CANCELLED</span></li>
              <li class="step0 ${order.status.toLowerCase() === 'delivered' ? 'active' : ''} ${order.status.toLowerCase() === 'cancelled' ? 'd-none' : ''} text-muted text-end" id="step3"><span style="margin-right: 22px;">DELIVERED</span></li>
            </ul>
          </div>
          <div class="card-footer p-4">
            <div class="d-flex justify-content-between align-items-center">
              
              <h5 class="fw-normal mb-0 w-100 text-center rounded bg-danger-subtle shadow ${order.status.toLowerCase() === 'cancelled' || order.status.toLowerCase() === 'delivered' ? 'd-none' : ''}" style="transform: rotate(0);"><a class="btn btn-block stretched-link" onclick="cancelOrder(${order.id})">Cancel</a></h5>
              <div class="border-start h-100"></div>
              <h5 class="fw-normal mb-0 w-100 text-center rounded bg-primary-subtle shadow ms-3" style="transform: rotate(0);"><a onclick="viewMore(${order.id})" class="btn btn-block stretched-link">View Details</a></h5>
              <div class="border-start h-100"></div>
              
            </div>
          </div>
        </div>
      `;
                });
  
                $("#orderHistoryDiv").empty();
                $("#orderHistoryDiv").append(html);
               
            }
            else {
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

