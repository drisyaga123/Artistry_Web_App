$(document).ready(function () {
    getCartItemCount();
})
function getCartItemCount() {
    debugger
    $.ajax({
        url: apiUrls.get_cart_itemscount,
        type: "GET",
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {
            if (response != null) {
                $("#cardItemCount").html(response);
            }

        },
        error: function (xhr, status, error) {

            alertFailed(xhr.responseText);

        }
    });
}