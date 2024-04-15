$(document).ready(function () {
    getAllCustomerDetails();
})
function viewOrders(id) {
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
                <p class="text-muted">Qt: ${order.quantity} ${order.quantity > 1 ? 'items' : 'item'}</p>
                <h4 class="mb-3">₹${order.sellingAmount} <span class="small text-muted"> ${order.paymentMode} </span></h4>
                 <p>Status : <span class="badge ${order.status.toLowerCase() === 'cancelled' ? 'text-bg-danger' : 'text-bg-success'}">${order.status}</span></p>
              </div>
              <div>
                <img class="align-self-center img-fluid" src="${order.productImage}" width="100">
              </div>
            </div>
           
          </div>
          
        </div>
      `;
                });

                $("#orderHistoryDiv").empty();
                $("#orderHistoryDiv").append(html);
                $("#viewOrdersModal").modal('show');


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

function getAllCustomerDetails() {
    $("#loader").removeClass("d-none");
    $.ajax({
        url: apiUrls.get_all_customerdetails,
        type: "GET",
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {

            var html = '';
            if (response != null) {
                response.forEach(function (customer) {
                    html = html + `<tr><td>${customer.id}</td><td>${customer.userName}</td>
                    <td>
                    <button onclick="viewOrders(${customer.id})" class="btn btn-primary">View Orders</button></td></tr>`
                });
                $("#customerRecords").empty();
                $("#customerRecords").append(html);
            }
            $("#loader").addClass("d-none");
        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            alertFailed(xhr.responseText);

        }
    });
}