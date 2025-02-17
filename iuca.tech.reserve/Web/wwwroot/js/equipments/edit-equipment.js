var editEquipment = async function (id) {

    event.preventDefault();

    var formData = new FormData(document.getElementById("equipmentForm-" + id));

    formData.append("id", id);

    try {
        const response = await $.ajax({
            url: "/Equipment/Edit",
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
        showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while editing.');
    }
}