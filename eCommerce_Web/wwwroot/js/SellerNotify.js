$(document).ready(function () {
    getAllOrdersForShipping();
})

function approveOrder(id) {
    $('#modalContent').html('Are you sure you want to approve this order?');
    $('#confirmModal').modal('show');
    $('#confirmBtn').on('click', function () {
        if (id > 0) {

            $("#loader").removeClass("d-none");
            $.ajax({
                url: apiUrls.approve_order,
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
                            getAllOrdersForShipping()

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
function getAllOrdersForShipping() {
    $("#loader").removeClass("d-none");
    $.ajax({
        url: apiUrls.get_all_orderstoship,
        type: "GET",
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {

            var html = '';
            if (response != null) {
                response.forEach(function (order) {
                    html = html + `<tr>
                <td>${order.orderId}</td>
                <td>${order.productName}</td>
                <td><img src="${order.productImage}" alt="product image" style="width: 100px;" class="img-thumbnail"></td>
                <td>${order.quantity}</td>
                <td><p><span>${order.firstName}</span> <span>${order.lastName}</span></p>
                                <p>${order.address}, ${order.state}, ${order.city}, ${order.pincode}</p>
                                <p>${order.landmark}</p><p class="mb-0">${order.phone}</p></td>
                                <td>${formatDate(order.createdDate)}</td>
                <td><button class="btn btn-success" onclick="approveOrder(${order.id})">Approve</button></td>

            </tr>`
                });
                $("#tBodyOrders").empty();
                $("#tBodyOrders").append(html);
            }

            $("#loader").addClass("d-none");
        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            $("#tBodyOrders").empty();
            alertFailed(xhr.responseText);

        }
    });
}