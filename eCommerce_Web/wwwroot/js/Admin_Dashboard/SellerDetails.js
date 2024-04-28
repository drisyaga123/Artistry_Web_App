$(document).ready(function () {
    getAllSellerDetails();
})
function viewProducts(id) {
    $("#loader").removeClass("d-none");
    $.ajax({
        url: apiUrls.get_all_products,
        type: "POST",
        data: JSON.stringify({
            Id:id
        }),
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {

            var html = '';
            if (response != null) {
                response.forEach(function (product) {
                    html = html + `<tr><td>${product.productName}</td><td>${product.productDescription}</td><td>${product.productCategory}</td>
                    <td>${product.subCategory === `null` ? "--" : product.subCategory}</td><td>${product.mrpAmount}</td><td>${product.sellingAmount}</td><td>${product.stockQuantity}</td><td>${formatDate(product.createdDate)}</td></tr>`
                });
                $("#tbodyViewProds").empty();
                $("#tbodyViewProds").append(html);
                $('#viewProductsModal').modal('show');
            }
            $("#loader").addClass("d-none");
        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            alertFailed(xhr.responseText);

        }
    });
}

function getAllSellerDetails() {
    $("#loader").removeClass("d-none");
    $.ajax({
        url: apiUrls.get_all_sellerdetails,
        type: "GET",
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {

            var html = '';
            if (response != null) {
                response.forEach(function (seller) {
                    html = html + `<tr><td>${seller.id}</td><td>${seller.userName}</td>
                    <td>
                    <button onclick="viewProducts(${seller.id})" class="btn btn-primary">View Products</button></td></tr>`
                });
                $("#sellerRecords").empty();
                $("#sellerRecords").append(html);
            }
            $("#loader").addClass("d-none");
        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            alertFailed(xhr.responseText);

        }
    });
}