var removeEquipmentFromRequest = async function (equipmentId) {
    try {
        const response = await $.ajax({
            url: "/Request/RemoveEquipmentFromRequest",
            type: "POST",
            data: {
                equipmentId: equipmentId
            }
        });

        if (response.isSuccess) {
            window.location.reload();
        } else {
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', response.message);
        }
    }
    catch (error) {
        console.log(error);
        showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while removing.');
    }
}