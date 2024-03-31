let fileInput;
document.addEventListener("DOMContentLoaded", function () {
    const selectFileButton = document.getElementById('selectFileButton');
    const closeFileButton = document.getElementById('closeFileButton');
    const imagePreviewContainer = document.getElementById('imagePreviewContainer');
    
    selectFileButton.addEventListener('click', function () {
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/*';
        input.style.display = 'none';
        input.id = 'fileDp';

        input.addEventListener('change', function (event) {
            const file = event.target.files[0];
            if (file) {
                fileInput = file;
                const reader = new FileReader();
                reader.onload = function (e) {
                    const img = document.createElement('img');
                    img.src = e.target.result;
                    img.classList.add('img-preview', 'w-50', 'rounded');
                    img.style.borderTopLeftRadius = '.5rem';
                    img.style.fontSize = '6em';
                    imagePreviewContainer.innerHTML = ''; // Clear previous content
                    imagePreviewContainer.appendChild(img);
                };
                reader.readAsDataURL(file);
            }
        });

        document.body.appendChild(input);
        input.click();
        document.body.removeChild(input);
    });

    closeFileButton.addEventListener('click', function () {
        imagePreviewContainer.innerHTML = '';
        // Append the person circle icon back to the container
        imagePreviewContainer.insertAdjacentHTML('beforeend', `<i class="bi bi-person-circle bi-3x edit-icon" style="cursor: pointer; color:lightgrey; font-size: 6em;"></i>`);    });
});
function getSellerDetails() {
    $("#loader").removeClass("d-none");
    $.ajax({
        url: apiUrls.get_user_details,
        type: "GET",
        contentType: "application/json",
        headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
        success: function (response) {

            var html = '';
            if (response != null) {
                $("#spnFullName").html(response.userName);
                $("#sellerNameHeader").html(response.userName);
                $("#spnEmail").html(response.email);
                $("#spnAddress").html(response.Address);
                $("#spnBio").html(response.bio);
                if (response.organization == null || response.organization == "") 
                {
                    $("#sellerRole").html("Artist");
                }
                else {
                    $("#sellerRole").html("Organization : " + response.organization);
                }
                if (response.profilePicture != null && response.profilePicture != "") {
                    $("#imgDp").attr("src", response.profilePicture);
                }
            }
            $("#loader").addClass("d-none");
        },
        error: function (xhr, status, error) {
            $("#loader").addClass("d-none");
            alertFailed(xhr.responseText);

        }
    });
}
$(document).ready(function () {
    getSellerDetails();
})

function updateDp(){
    if (fileInput) {
        var formData = new FormData();
        formData.append("dp", fileInput);

        $.ajax({
            url: apiUrls.update_dp,
            type: "POST",
            data: formData,
            headers: { "Authorization": 'Bearer ' + localStorage.getItem('jwtToken') },
            processData: false,
            contentType: false,
            success: function (response) {
                $("#loader").addClass("d-none");
                if (response != null) {
                    if (response.status.toLowerCase() === "success") {
                        alertSuccess(response.message);
                        $("#exampleModal").modal('hide');
                        getSellerDetails();
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
    else {
        alertFailed("No file choosen");
    }
}
function openDpModal() {
    const src = $('#imgDp').attr('src');
    if (src && src.startsWith('data:image')) {
        var html = `<img src="${src}" class='w-50'>`;
        imagePreviewContainer.innerHTML = '';
        imagePreviewContainer.insertAdjacentHTML('beforeend', html);
    }
    $("#exampleModal").modal('show');
  
}