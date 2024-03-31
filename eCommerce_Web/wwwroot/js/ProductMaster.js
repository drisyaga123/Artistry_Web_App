var updateId = 0;
$(document).ready(function () {
    getAllProducts();
})
function enableImgEdit() {
    $("#imgUploadDiv").removeClass('d-none');
    $("#closeEdit").removeClass('d-none');
    $("#imgEditDiv").addClass('d-none');
}
function closeImgEdit() {
    $("#imgUploadDiv").addClass('d-none');
    $("#imgEditDiv").removeClass('d-none');
}
function viewImage(id) {
    if (id != null && id > 0) {
        $("#loader").removeClass("d-none");
        $.ajax({
            url: apiUrls.get_product_image,
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
                        $("#prod_image").attr("src", response.message);
                        $("#viewProdImg").modal('show');
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
function deleteProduct(id) {
    if (id != null && id > 0) {
        $("#loader").removeClass("d-none");
        $.ajax({
            url: apiUrls.delete_product,
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
                        getAllProducts();
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
function editProduct(id) {
    if (id != null && id > 0) {
        $("#loader").removeClass("d-none");
        $.ajax({
            url: apiUrls.get_product,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                "Id": id
            }),
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },

            success: function (response) {
                $("#loader").addClass("d-none");
                if (response != null) {
                    $("#product_name").val(response.productName);
                    $("#product_desc").val(response.productDescription);
                    $("#product_category").val(response.productCategory);
                    $("#product_Mrp").val(response.mrpAmount);
                    $("#product_Sellingprice").val(response.sellingAmount);
                    $("#imgName").html(response.productImage);
                    $("#imgUploadDiv").addClass('d-none');
                    $("#imgEditDiv").removeClass('d-none');
                    $("#btnAddProd").addClass('d-none');
                    $("#btnUpdateProd").removeClass('d-none');
                    $("#addProductModal").modal('show');
                    updateId = response.id;
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
function getAllProducts() {
    $("#loader").removeClass("d-none");
    $.ajax({
        url: apiUrls.get_all_products,
        type: "GET",
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {
         
            var html = '';
            if (response != null) {
                response.forEach(function (product) {
                    html = html + `<tr><td>${product.productName}</td><td>${product.productDescription}</td><td>${product.productCategory}</td>
                    <td>${product.subCategory === `null` ? "--" : product.subCategory}</td><td>${product.mrpAmount}</td><td>${product.sellingAmount}</td><td><div class="d-flex">
                    <button onclick="viewImage(${product.id})" class="btn p-1" title="View image"><i class="fa fa-eye text-primary" aria-hidden="true"></i></button>
                    <button onclick="editProduct(${product.id})" class="btn p-1" title="Edit product"><i class="fas fa-edit text-warning"></i></button>
                    <button onclick="deleteProduct(${product.id})" class="btn p-1" title="Delete product"><i class="fa fa-trash text-danger" aria-hidden="true"></i></button></td></tr>`
                });
                $("#tBodyProducts").empty();
                $("#tBodyProducts").append(html);
            }
            $("#loader").addClass("d-none");
        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            alertFailed(xhr.responseText);

        }
    });
}
function closeModal() {
    $("#addProductModal").modal('hide');
    $("#product_name").val('');
    $("#product_desc").val('');
    $("#product_category").val('');
    $("#product_Mrp").val('');
    $("#product_Sellingprice").val('');
    $("#imgName").html('');
    $("#imgUploadDiv").removeClass('d-none');
    $("#imgEditDiv").addClass('d-none');
    $("#btnAddProd").removeClass('d-none');
    $("#btnUpdateProd").addClass('d-none');
    $("#closeEdit").addClass('d-none');
    $('#prodfilebutton').val('');
}
function validateProductMaster() {
    
    let isValid = true;
    let productName = $("#product_name").val();
    let description = $("#product_desc").val();
    let category = $("#product_category").val();
    let productMrp = $("#product_Mrp").val();
    let productSellingPrice = $("#product_Sellingprice").val();
    var fileInput = $('#prodfilebutton')[0]; // Get the DOM element
    var file = fileInput.files[0]; 
    
    if (productName == "" || productName == null) {
        isValid = false;
        $("#product_name").removeClass("is-valid").addClass("is-invalid");
        $("#product-name-error").text("Product Name is required!");
    } else {
        $("#product_name").removeClass("is-invalid").addClass("is-valid");
    }

   
    if (description == "" || description == null) {
        isValid = false;
        $("#product_desc").removeClass("is-valid").addClass("is-invalid");
        $("#description-error").text("Description is required!");
    } else {
        $("#product_desc").removeClass("is-invalid").addClass("is-valid");
    }

   
    if (category == "" || category == null) {
        isValid = false;
        $("#product_category").removeClass("is-valid").addClass("is-invalid");
        $("#category-error").text("Category is required!");
    } else {
        $("#product_category").removeClass("is-invalid").addClass("is-valid");
    }


    if (productMrp == "" || productMrp == null) {
        isValid = false;
        $("#product_Mrp").removeClass("is-valid").addClass("is-invalid");
        $("#product-mrp-error").text("Product Mrp is required!");
    } else {
        $("#product_Mrp").removeClass("is-invalid").addClass("is-valid");
    }


    if (productSellingPrice == "" || productSellingPrice == null) {
        isValid = false;
        $("#product_Sellingprice").removeClass("is-valid").addClass("is-invalid");
        $("#product-sellingprice-error").text("Selling price is required!");
    } else {
        $("#product_Sellingprice").removeClass("is-invalid").addClass("is-valid");
    }
    if ($("#imgEditDiv").hasClass('d-none')) {
        if (file) {
            $("prodfilebutton").removeClass("is-invalid").addClass("is-valid");
        
        } else {
            isValid = false;
            $("#prodfilebutton").removeClass("is-valid").addClass("is-invalid");
            $("#image-error").text("Selling price is required!");
        }
    }

    return isValid;
}
function addProduct() {
    if (validateProductMaster()) {
        $("#loader").removeClass("d-none");
        var productName = $("#product_name").val();
        var productDesc = $("#product_desc").val();
        var productCategory = $("#product_category").val();
        var productSubCategory = $("#product_sub_category").val();
        var productMrp = $("#product_Mrp").val();
        var productSellingPrice = $("#product_Sellingprice").val();
        var fileInput = $("#prodfilebutton")[0].files[0];

        var formData = new FormData();
        formData.append("product_name", productName);
        formData.append("product_desc", productDesc);
        formData.append("product_category", productCategory);
        formData.append("product_sub_category", productSubCategory);
        formData.append("product_Mrp", productMrp);
        formData.append("product_Sellingprice", productSellingPrice);
        formData.append("product_image", fileInput);

        $.ajax({
            url: apiUrls.add_product,
            type: "POST",
            data: formData,
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
            processData: false,
            contentType: false,
            success: function (response) {
                $("#loader").addClass("d-none");
                closeModal();
                if (response != null) {
                    if (response.status.toLowerCase() === "success") {
                        alertSuccess(response.message);
                        getAllProducts();
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
                closeModal();
                alertFailed(xhr.responseText);

            }
        });
    };
}

function updateProduct() {
    if (validateProductMaster() && updateId>0) {
        $("#loader").removeClass("d-none");
        var productName = $("#product_name").val();
        var productDesc = $("#product_desc").val();
        var productCategory = $("#product_category").val();
        var productSubCategory = $("#product_sub_category").val();
        var productMrp = $("#product_Mrp").val();
        var productSellingPrice = $("#product_Sellingprice").val();
        var fileInput = '';
        if ($("#imgEditDiv").hasClass('d-none')) {
            fileInput = $("#prodfilebutton")[0].files[0];
        }

        var formData = new FormData();
        formData.append("product_id", updateId);
        formData.append("product_name", productName);
        formData.append("product_desc", productDesc);
        formData.append("product_category", productCategory);
        formData.append("product_sub_category", productSubCategory);
        formData.append("product_Mrp", productMrp);
        formData.append("product_Sellingprice", productSellingPrice);
        formData.append("product_image", fileInput);

        $.ajax({
            url: apiUrls.update_product,
            type: "POST",
            data: formData,
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
            processData: false,
            contentType: false,
            success: function (response) {
                $("#loader").addClass("d-none");
                closeModal();
                if (response != null) {
                    if (response.status.toLowerCase() === "success") {
                        alertSuccess(response.message);
                        getAllProducts();
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
                closeModal();
                alertFailed(xhr.responseText);

            }
        });
    };
}