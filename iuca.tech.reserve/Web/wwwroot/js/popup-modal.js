const CONSTS = {
    MODAL_SUCCESS: 'btn-success',
    MODAL_FAIL: 'btn-danger'
}

var showPopupModal = function (modalType, title, message) {
    var modal = $('#popupModal');
    var closeButton = modal.find('.modal-footer .btn');

    closeButton.removeClass(CONSTS.MODAL_FAIL);
    closeButton.removeClass(CONSTS.MODAL_SUCCESS);

    closeButton.addClass(modalType);

    modal.find('.modal-title').html(title);
    modal.find('.modal-body').html(message);
    modal.modal('show');
}