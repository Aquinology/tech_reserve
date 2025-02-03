var importClients = async function (button) {
    try {
        setButtonLoading(button);

        const response = await $.ajax({
            url: "/ImportData/ImportClients",
            type: "POST",
            cache: false
        });

        if (response.isSuccess) {
            showPopupModal(CONSTS.MODAL_SUCCESS, 'Success', response.message);
        } else {
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', response.message);
        }
    }
    catch (error) {
        console.log(error);
        showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while importing.');
    }
    finally {
        setButtonNormal(button);
    }
}