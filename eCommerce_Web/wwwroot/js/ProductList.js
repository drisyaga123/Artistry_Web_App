
function addItemToCart(id,quantity) {
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

function listProducts(pageIndex) {
    $("#loader").removeClass("d-none");
    var obj = {
        "PageIndex": pageIndex,
        "PageSize":3,
        "Category": [],
        "SubCategory":[]
    }
    $.ajax({
        url: apiUrls.list_all_products,
        type: "POST",
        data: JSON.stringify(obj),
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {
   
            var html = '';
            if (response != null) {

                var totalPages = parseInt(response.totalPages);
                response.product_List.forEach(function (product) {
                    var offPercentage = 100-((parseFloat(product.sellingAmount) / parseFloat(product.mrpAmount)) * 100);
                    html = html + `<div class="col-lg-3 mb-3"><div class="card" style="width: 18rem;"><img class="card-img-top card-img-fit" src="${product.productImage}" alt="Card image cap"><div class="card-body"><h4>${product.productName}</h4><p>${product.productDescription}</p>
                    <span class="text-decoration-line-through text-black-50">₹${product.mrpAmount}</span><span class="ms-2 fw-medium">₹${product.sellingAmount}</span><span class="ms-2 text-danger" style="font-size: smaller;">${offPercentage}% off</span><div class="d-flex justify-content-center mt-2"><button class="btn btn-primary" onclick="addItemToCart(${product.id},1)">Add to cart</button><button class="btn btn-warning ms-2">Buy now</button></div></div></div></div>`
                  
                });
                $("#prodListRow").empty();
                $("#prodListRow").append(html);
                var paginationHtml = '<div class="row"><div class="col-12"><nav aria-label="Page navigation"><ul class="pagination justify-content-center">';
                paginationHtml += `<li class="page-item${pageIndex == 1 ? ' disabled' : ''}"><a class="page-link cursor_poniter" onclick="listProducts(${pageIndex-1})" tabindex="-1" aria-disabled="true">Previous</a></li>`;
                for (var i = 1; i <= totalPages; i++) {
                    paginationHtml += `<li class="page-item${i === pageIndex ? ' active' : ''}"><a class="page-link cursor_poniter" onclick="listProducts(${i})" data-page="${i}">${i}</a></li>`;
                }
                paginationHtml += `<li class="page-item${pageIndex == totalPages ? ' disabled' : ''}"><a class="page-link cursor_poniter" onclick="listProducts(${pageIndex+1})">Next</a></li>`;
                paginationHtml += '</ul></nav></div></div>';
                $("#paginationRow").empty();
                $("#paginationRow").append(paginationHtml);
            }
            $("#loader").addClass("d-none");
        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            alertFailed(xhr.responseText);

        }
    });
}
var options = [];

$('.dropdown-menu a').on('click', function (event) {

    var $target = $(event.currentTarget),
        val = $target.attr('data-value'),
        $inp = $target.find('input'),
        idx;

    if ((idx = options.indexOf(val)) > -1) {
        options.splice(idx, 1);
        setTimeout(function () { $inp.prop('checked', false) }, 0);
    } else {
        options.push(val);
        setTimeout(function () { $inp.prop('checked', true) }, 0);
    }

    $(event.target).blur();

    console.log(options);
    return false;
});
$('body').on("click", ".dropdown-menu", function (e) {
    $(this).parent().is(".open") && e.stopPropagation();
});

$('.selectall').click(function () {
    if ($(this).is(':checked')) {
        $('.option').prop('checked', true);
        var total = $('input[name="options[]"]:checked').length;
        $(".dropdown-text").html('(' + total + ') Selected');
        $(".select-text").html(' Deselect');
    } else {
        $('.option').prop('checked', false);
        $(".dropdown-text").html('(0) Selected');
        $(".select-text").html(' Select');
    }
});

$("input[type='checkbox'].justone").change(function () {
    var a = $("input[type='checkbox'].justone");
    if (a.length == a.filter(":checked").length) {
        $('.selectall').prop('checked', true);
        $(".select-text").html(' Deselect');
    }
    else {
        $('.selectall').prop('checked', false);
        $(".select-text").html(' Select');
    }
    var total = $('input[name="options[]"]:checked').length;
    $(".dropdown-text").html('(' + total + ') Selected');
});
$(document).ready(function () {
    listProducts(1);
})