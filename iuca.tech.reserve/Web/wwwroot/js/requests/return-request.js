var returnRequest = async function (requestId) {
    try {
        const response = await $.ajax({
            url: "/Requests/ReturnEquipment",
            type: "POST",
            data: {
                requestId: requestId
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
        showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while returning.');
    }
}
