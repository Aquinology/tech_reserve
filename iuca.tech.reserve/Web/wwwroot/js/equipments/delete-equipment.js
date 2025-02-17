var deleteEquipment = async function (id) {
    try {
        const response = await $.ajax({
            url: "/Equipment/Delete",
            type: "POST",
            data: {
                equipmentId: id
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
        showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while deleting.');
    }
}