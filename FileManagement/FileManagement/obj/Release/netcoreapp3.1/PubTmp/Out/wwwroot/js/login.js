
locateCenter();


function doLogin() {
    var u = $("#username").val();
    var p = $("#password").val();

    if (u.trim() != "" && p.trim() != "") {
        var formData = new FormData();
        formData.append("username", u);
        formData.append("password", p);

        $.ajax(
            {
                url: window.location.origin + "/login/authenticate",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (response) {
                    if (response.success == true) {
                        window.location.href = "home";
                    } else if (response.success == false) {
                        resetModal();
                        $('#form-modal-message .filelist').html(response.message);
                        $('#form-modal-message').modal('show');
                    }
                },
                error: function (result) {

                }
            }
        );
    }
    else {
        resetModal();
        $('#form-modal-message .filelist').html("Invalid username and/or password. Please sign in again.");
        $('#form-modal-message').modal('show');
    }
}

function resetModal() {
    $('#form-modal-message').modal('hide');
}

function locateCenter() {
    let w = $(window).width();
    let h = $(window).height();

    let cw = $('#form-modal-message').outerWidth();
    let ch = $('#form-modal-message').outerHeight();

    $('#form-modal-message').css({
        'left': ((w - cw) / 2) + 'px',
        'top': ((h - ch) / 2) + 'px'
    });
}