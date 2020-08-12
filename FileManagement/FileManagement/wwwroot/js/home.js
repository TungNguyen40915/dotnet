locateCenter();

function createFolder() {
    $('#form-create-folder').modal('show');
}

function locateCenter() {
    let w = $(window).width();
    let h = $(window).height();

    let cw = $('#form-create-folder').outerWidth();
    let ch = $('#form-create-folder').outerHeight();

    $('#form-create-folder').css({
        'left': ((w - cw) / 2) + 'px',
        'top': ((h - ch) / 2) + 'px'
    });
}

function create() {

    var curURL = document.URL;
    $.ajax(
        {
            url: curURL.replace("/home", "/folder/CreateFolder?name=" + $('#name').val()),
            processData: false,
            contentType: false,
            type: "GET",
            success: function (response) {
                location.reload(true);
            },
            error: function (result) {
                location.reload(true);
            }
        }
    );
}