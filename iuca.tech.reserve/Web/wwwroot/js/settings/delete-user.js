var deleteUser = async function (userId) {
    try {
        const response = await $.ajax({
            url: "/User/Delete",
            type: "POST",
            data: {
                userId: userId
            },
            cache: false
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