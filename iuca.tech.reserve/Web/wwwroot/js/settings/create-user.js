var createUser = async function () {

    var email = $('#email').val();
    var firstName = $('#firstName').val();
    var lastName = $('#lastName').val();
    var role = $('#role').val();

    try {
        const response = await $.ajax({
            url: "/Users/Create",
            type: "POST",
            data: {
                email: email,
                firstName: firstName,
                lastName: lastName,
                role: role
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
        showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while creating.');
    }
}