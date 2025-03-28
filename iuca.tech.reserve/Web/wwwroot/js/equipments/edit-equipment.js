var editEquipment = async function (equipmentId) {

    var formData = new FormData(document.getElementById("equipmentForm-" + equipmentId));

    formData.append("equipmentId", equipmentId);

    try {
        const response = await $.ajax({
            url: "/Equipments/Edit",
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