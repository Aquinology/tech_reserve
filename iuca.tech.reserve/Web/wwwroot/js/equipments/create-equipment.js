﻿var createEquipment = async function () {

    var formData = new FormData(document.getElementById("equipmentForm"));

    try {
        const response = await $.ajax({
            url: "/Equipments/Create",
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
        showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while creatting.');
    }
}