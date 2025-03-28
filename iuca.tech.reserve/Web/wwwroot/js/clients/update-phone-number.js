var updatePhoneNumber = async function () {

    var formData = new FormData(document.getElementById("phoneNumberForm"));

    try {
        const response = await $.ajax({
            url: "/Clients/UpdatePhoneNumber",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false
        });

        if (response.isSuccess) {
            window.location.reload();
        } else {
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', response.message);
        }
    }
    catch (error) {
        console.log(error);
        showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while updating.');
    }
}