var deleteUser = async function (userId) {
    try {
        const response = await $.ajax({
            url: "/Users/Delete",
            type: "POST",
            data: {
                userId: userId
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