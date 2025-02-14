var addEquipment = async function () {

    event.preventDefault();

    var formData = new FormData(document.getElementById("equipmentForm"));

    try {
        const response = await $.ajax({
            url: "/Equipment/Create",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false
        });

        if (response.isSuccess) {
            window.location.reload();
        } else {
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', response.message);
        }
    }
    catch (error) {
        console.log(error);
        showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while adding.');
    }
}