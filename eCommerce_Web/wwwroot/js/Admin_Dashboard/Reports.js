function getUsersList(role)
{
    $("#loader").removeClass("d-none");
    $.ajax({
        url: apiUrls.get_users_byrole,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            "Role": role
        }),
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },

        success: function (response) {
            $("#loader").addClass("d-none");
            if (response.length > 0) {
                var selectElement = $("#addedBy");
                selectElement.empty(); // Clear previous options
                selectElement.append('<option value="" selected disabled>Select user</option>');

                response.forEach(function (item) {
                    var option = $("<option></option>")
                        .attr("value", item.id) // Set the value attribute
                        .text(item.userName); // Set the text of the option
                    selectElement.append(option); // Append the option to the select element
                });
            }

        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            alertFailed(xhr.responseText);

        }
    });
}
$(document).ready(function () {
    getUsersList("Customer");
})
$("#report_types").change(function () {
    var selected = $("#report_types").val();
    if (selected == "orders") {
        getUsersList("Customer");
        $(".userRole").addClass('d-none');
        $(".filterDate").removeClass('d-none');
        $(".orderStatus").removeClass('d-none');
        $(".addedByUser").removeClass('d-none');
    }
    else if (selected == "products") {
        getUsersList("Seller");
        $(".userRole").addClass('d-none');
        $(".orderStatus").addClass('d-none');
        $(".filterDate").removeClass('d-none');
        $(".addedByUser").removeClass('d-none');
    }
    else if (selected == "users") {
        $(".userRole").removeClass('d-none');
        $(".orderStatus").addClass('d-none');
        $(".filterDate").addClass('d-none');
        $(".addedByUser").addClass('d-none');
    }
})
function b64toBlob(b64Data, contentType = '', sliceSize = 512) {
    const byteCharacters = atob(b64Data);
    const byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        const slice = byteCharacters.slice(offset, offset + sliceSize);

        const byteNumbers = new Array(slice.length);
        for (let i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }

    const blob = new Blob(byteArrays, { type: contentType });
    return blob;
}
function getReport() {
    
    var repType=$("#report_types").val()
    
    var startDate = $("#start_date").val();
    var endDate = $("#end_date").val();
    if (startDate && endDate) {
        if (startDate > endDate) {
            alertFailed("Start date should be less than End date");
            return false;
        }
    }
    $("#loader").removeClass("d-none");
    var obj = {
        CreatedBy: $("#addedBy").val(),
        StartDate: $("#start_date").val(),
        EndDate: $("#end_date").val(),
        Status: $("#order_status").val(),
        DocContent: $("#report_types").val(),
        Role: $("#user_roles").val()
    }
    $.ajax({
        url: apiUrls.doc_download,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(obj),
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },

        success: function (response) {
            $("#loader").addClass("d-none");
            if (response.status.toLowerCase() == "success") {
                
                var pdfBlob = b64toBlob(response.message, 'application/pdf');
                // Create a temporary anchor element
                var a = document.createElement('a');
                // Set the anchor's href attribute to the blob URL
                a.href = window.URL.createObjectURL(pdfBlob);
                var name = "";
                // Set the anchor's download attribute to specify the filename
                if (repType == "orders") {
                     name = 'Orders_details.pdf';
                }
                else if (repType == "products") {
                    name = 'Products_details.pdf';
                }
                else if (repType == "users") {
                    name = 'Users_details.pdf';
                }
                a.download = name;
                // Append the anchor to the document body
                document.body.appendChild(a);
                // Trigger a click event on the anchor to start the download
                a.click();
                // Remove the anchor from the document body
                document.body.removeChild(a);
            }
            else {
                alertFailed(response.message);
            }

        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            alertFailed(xhr.responseText);

        }
    });
}