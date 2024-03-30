let updateId = 0;
$(document).ready(function () {
    $("#loader").removeClass("d-none");
    getCustomerDetails(function () {
        getAllAddresses(function () {
            $("#loader").addClass("d-none");
        })
    })
})

function removeAddress(id) {
    if (id > 0) {
        $("#loader").removeClass("d-none");
        $.ajax({
            url: apiUrls.delete_address,
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
                        getAllAddresses(function () {
                          
                        })
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
function editAddress(id) {
    if (id != null && id > 0) {
        $("#loader").removeClass("d-none");
        $.ajax({
            url: apiUrls.get_address,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                "Id": id
            }),
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },

            success: function (response) {
                $("#loader").addClass("d-none");
                if (response != null) {
                    $("#addrfirstname").val(response.firstName);
                    $("#addrlastname").val(response.lastName);
                    $("#addraddress").val(response.address);
                    $("#addrlandmark").val(response.landmark);
                    $("#addrstate").val(response.state);
                    $("#addrzipcode").val(response.pincode);
                    $("#addrcity").val(response.city);
                    $("#addrphone").val(response.phone);
                    $("#btnAddAddr").addClass('d-none');
                    $("#btnUpdateAddr").removeClass('d-none');
                    $("#addAddressModal").modal('show');
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
function updateAddress() {
    if (validateAddressForm() && updateId > 0) {
        $("#loader").removeClass("d-none");
        var obj = {
            Id: updateId,
            FirstName: $("#addrfirstname").val(),
            LastName: $("#addrlastname").val(),
            Address: $("#addraddress").val(),
            Landmark: $("#addrlandmark").val(),
            State: $("#addrstate").val(),
            Pincode: $("#addrzipcode").val(),
            City: $("#addrcity").val(),
            Phone: $("#addrphone").val()
        };
        $.ajax({
            url: apiUrls.update_address,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(obj),
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
            success: function (response) {
                $("#loader").addClass("d-none");
                $("#addAddressModal").modal('hide');
                if (response != null) {
                    if (response.status.toLowerCase() === "success") {
                        alertSuccess(response.message);
                        getAllAddresses(function () {

                        })
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
                $("#addAddressModal").modal('hide');
                alertFailed(xhr.responseText);

            }
        });
    };
}
function getAllAddresses(callback) {
    $.ajax({
        url: apiUrls.get_all_address,
        type: "GET",
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {
            var html = '';
            if (response.length > 0) {
                response.forEach(function (addr) {
                    html = html + `<div class="card bg-light mb-2"><div class="card-body justify-content-between d-flex align-items-baseline"><div><p><span>${addr.firstName} </span> <span>${addr.lastName} </span></p>
                    <p>${addr.address}, ${addr.state},${addr.city}, ${addr.pincode}</p><p>${addr.landmark}</p><p>${addr.phone}</p></div>
                    <div><button class="btn p-0" onclick="editAddress(${addr.id})"><i class="fas fa-edit text-warning"></i></button>
                    <button class="btn" onclick="removeAddress(${addr.id})"><i class="fa fa-trash text-danger" aria-hidden="true"></i></button>
                    </div></div></div>`
                })
                $("#addrSect").removeClass('d-none');
                $("#noAddress").addClass('d-none');
                $("#addrSect").empty();
                $("#addrSect").append(html);
            }
            else {
                $("#addrSect").addClass('d-none');
                $("#noAddress").removeClass('d-none');
            }
            callback();
       
        },
        error: function (xhr, status, error) {
            alertFailed(xhr.responseText);

        }
    });
}
function getCustomerDetails(callback) {
   
    $.ajax({
        url: apiUrls.get_user_details,
        type: "GET",
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {
            if (response != null) {
                $("#spnUserFullName").html(response.userName);
                $("#spnUserEmail").html(response.email);
            }
            callback();
        },
        error: function (xhr, status, error) {
            alertFailed(xhr.responseText);

        }
    });
}