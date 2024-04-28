let priceArray = [];
let categoryArray = [];
let subCategoryArray = [];
let productId = 0;
function handleCheckboxChange(event) {
    priceArray = [];
    categoryArray = [];
    subCategoryArray = [];
    var checkBoxes = $(".cbFilter");
    checkBoxes.each(function (index, cb) {
        var parentButton = $(cb).closest('.dropdown').find('button'); 
        if (parentButton.attr('data-btn') === 'price') {
            if (cb.checked) {
                // Add value to corresponding array
                priceArray.push(cb.value);
            } else {
                // Remove value from corresponding array if unchecked
                var index = priceArray.indexOf(cb.value);
                if (index !== -1) {
                    priceArray.splice(index, 1);
                }
            }
        }
        else if (parentButton.attr('data-btn') === 'category') {
            if (cb.checked) {
                // Add value to corresponding array
                categoryArray.push(cb.value);
            } else {
                // Remove value from corresponding array if unchecked
                var index = categoryArray.indexOf(cb.value);
                if (index !== -1) {
                    categoryArray.splice(index, 1);
                }
            }
        }
        else if (parentButton.attr('data-btn') === 'subcategory') {
            if (cb.checked) {
                // Add value to corresponding array
                subCategoryArray.push(cb.value);
            } else {
                // Remove value from corresponding array if unchecked
                var index = subCategoryArray.indexOf(cb.value);
                if (index !== -1) {
                    subCategoryArray.splice(index, 1);
                }
            }
        }
    })
  
    listProducts(1)
}
function addItemToCart(id, quantity) {
    if (id > 0) {
        $("#loader").removeClass("d-none");
        var obj = {
            "ProductId": id,
            "Quantity": quantity
        }
        $.ajax({
            url: apiUrls.add_to_cart,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(obj),
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
            success: function (response) {
                $("#loader").addClass("d-none");
                if (response != null) {
                    if (response.status.toLowerCase() === "success") {
                        getCartItemCount();
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
                if (xhr.status == 401) {
                    alertFailed("Please login");
                }
                else {
                    alertFailed(xhr.responseText);
                }


            }
        });
    };
}
function goToOrderPage(id) {
    window.location.href = redirectUrl.orderSummaryPage + "?id=" + id;
}
function viewAllReviews(id) {
    if (id > 0) {
        $.ajax({
            url: apiUrls.get_all_reviews,
            type: "POST",
            data: JSON.stringify({
                Id: id
            }),
            contentType: "application/json",
            success: function (response) {
                if (response.length > 0) {
                    var html = "";
                    response.forEach(function (review) {
                        debugger
                        html = html + `<div class="card mb-2">
                       <div class="card-body">
                         <div class="mb-2">`
                        for (var i = 0; i < review.rating; i++) {
                            html += `<span class="starFilledDisplay"></span>`;
                        }
                        // Render empty star ratings for the remaining stars
                        for (var i = review.rating; i < 5; i++) {
                            html += `<span class="starEmptyDisplay"></span>`;
                        }

                        html += `</div>
                            <div class="mb-2">
                            ${review.review}
                            </div>
                            <div>
                            <span>Review by ${review.username} </span>
                            <span>on ${formatDate(review.createdDate)}</span>
                               
                            </div>
                        </div>
                    </div>`
                    })
                    $("#reviewsContainer").empty();
                    $("#reviewsContainer").append(html);
                    $("#ratingModal").modal('show');
                }
                else {
                    alertFailed("No reviews found")
                }
                    
            },
            error: function (xhr, status, error) {
                $("#loader").addClass("d-none");
                alertFailed(xhr.responseText);

            }
        })


    }
}
function listProducts(pageIndex) {
    $("#loader").removeClass("d-none");
    var obj = {
        "PageIndex": pageIndex,
        "PageSize":8,
        "Category": categoryArray,
        "SubCategory": subCategoryArray,
        "Price": priceArray
    }
    $.ajax({
        url: apiUrls.list_all_products,
        type: "POST",
        data: JSON.stringify(obj),
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {
   
            var html = '';
            if (response.product_List.length>0) {

                var totalPages = parseInt(response.totalPages);
                response.product_List.forEach(function (product) {
                    var offPercentage = 100 - ((parseFloat(product.sellingAmount) / parseFloat(product.mrpAmount)) * 100);
                    offPercentage = offPercentage.toFixed(2);
                    html = html + `<div class="col-lg-3 mb-3"><div class="card" style="width: 15rem;"><img class="card-img-top card-img-fit" src="${product.productImage}" alt="Card image cap"><div class="card-body"><h4>${product.productName}</h4><p class="text-black-50 fst-italic">${product.productDescription}</p>
                    <span class="text-decoration-line-through text-black-50">₹${product.mrpAmount}</span><span class="ms-2 fw-medium">₹${product.sellingAmount}</span><span class="ms-2 text-danger" style="font-size: smaller;">${offPercentage}% off</span>${product.stockQuantity > 0 ? '' :'<h4 class="text-danger">Unavailable</h4>'}
                    <a href="#" class="ratingModalOpener" onclick="viewAllReviews(${product.id})"><div>`;
                    
                    for (var i = 0; i < product.averageRating; i++) {
                        html += `<span class="starFilledDisplay"></span>`;
                    }
                    // Render empty star ratings for the remaining stars
                    for (var i = product.averageRating; i < 5; i++) {
                        html += `<span class="starEmptyDisplay"></span>`;
                    }      
                    html += `</div></a>`;
                    html = html + `<div class="d-flex justify-content-center mt-2"><button class="btn btn-primary ${product.stockQuantity > 0 ? '' : 'disabled'}" onclick="addItemToCart(${product.id},1)">Add to cart</button><button class="btn btn-warning ms-2 ${product.stockQuantity > 0 ? '' : 'disabled'}" onclick="goToOrderPage(${product.id})">Buy now</button></div></div></div></div>`

                });
                $("#prodListRow").empty();
                $("#prodListRow").append(html);
                var paginationHtml = '<div class="row"><div class="col-12"><nav aria-label="Page navigation"><ul class="pagination justify-content-center">';
                paginationHtml += `<li class="page-item${pageIndex == 1 ? ' disabled' : ''}"><a class="page-link cursor_poniter" onclick="listProducts(${pageIndex - 1})" tabindex="-1" aria-disabled="true">Previous</a></li>`;
                for (var i = 1; i <= totalPages; i++) {
                    paginationHtml += `<li class="page-item${i === pageIndex ? ' active' : ''}"><a class="page-link cursor_poniter" onclick="listProducts(${i})" data-page="${i}">${i}</a></li>`;
                }
                paginationHtml += `<li class="page-item${pageIndex == totalPages ? ' disabled' : ''}"><a class="page-link cursor_poniter" onclick="listProducts(${pageIndex + 1})">Next</a></li>`;
                paginationHtml += '</ul></nav></div></div>';
                $("#paginationRow").empty();
                $("#paginationRow").append(paginationHtml);
            }
            else {
                $("#prodListRow").empty();
                $("#paginationRow").empty();
            }
            $("#loader").addClass("d-none");
        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            alertFailed(xhr.responseText);
            if (xhr.responseText.toLowerCase() == "no products found!") {
                $("#prodListRow").empty();
                $("#paginationRow").empty();
            }

        }
    });
}

$(document).ready(function () {
    listProducts(1);
    const checkboxes = document.querySelectorAll('.form-check-input');
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', handleCheckboxChange);
    });
})