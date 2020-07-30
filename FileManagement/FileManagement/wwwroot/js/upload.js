$('#fileupload').fileupload({
    dropZone: $('#dropzone')
});

locateCenter();

var filesList = [], elem = $("form");

file_upload = elem.fileupload({
    formData: { extra: 1 },
    autoUpload: false,
    fileInput: $("input:file"),
}).on("fileuploadadd", function (e, data) {
    filesList.push(data.files[0]);

    if (filesList.length <= parseInt(maxFileCount) && filesList.length > 0) {
        ($('#uploadbutton')).prop("disabled", false);
    }
    else {
        ($('#uploadbutton')).prop("disabled", true);
    }

})
    .on('fileuploadfail', function (e, data) {
        var deleteIndex = -1;

        for (var i = 0; i < filesList.length; i++) {
            if (data.files[0] == filesList[i]) {
                deleteIndex = i;
                break;
            }
        }

        if (deleteIndex != -1) filesList.splice(deleteIndex, 1);

        if (filesList.length <= parseInt(maxFileCount)) {
            ($('#uploadbutton')).prop("disabled", false);
        }
        if (filesList.length <= 0) {
            ($('#uploadbutton')).prop("disabled", true);
        }
    });

function submitForm() {
    var fileNames = "<ul>" + "\n";
    for (var i = 0; i < filesList.length; i++) {
        fileNames = fileNames + "<li class=\"upload-filename-progress\"> " + filesList[i].name + "</li>" + "\n";
    }
    fileNames = fileNames + "</ul>" + "\n";

    resetModal();
    $('#form-modal-progress .filelist').html(fileNames);
    $('#form-modal-progress').modal('show');

    var isValid = filesValidatation(filesList);

    if (isValid) {
        var formData = new FormData();
        for (var i = 0; i < filesList.length; i++) {
            formData.append("files", filesList[i]);
        }
        var curURL = document.URL.toLocaleLowerCase();
        $.ajax(
            {
                url: curURL.replace("/upload", "/upload/onupload"),
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function (response) {
                    if (response.success == 'true') {
                        resetModal();
                        $('#form-modal-success .filelist').html(fileNames);
                        $('#form-modal-success').modal('show');
                    } else if (response.success == 'false') {
                        resetModal();
                        $('#form-modal-error .filelist').html(response.message);
                        $('#form-modal-error').modal('show');
                    } else {
                        window.location.replace(response.url);
                    }

                },
                error: function (result) {
                    resetModal();
                    window.location.href = '/Upload/Error/';
                }
            }
        );
    }
}



function resetModal() {
    $('#form-modal-progress').modal('hide');
    $('#form-modal-success').modal('hide');
    $('#form-modal-error').modal('hide');
}


function filesValidatation(fileList) {
    if (fileList.length > parseInt(maxFileCount)) {
        resetModal();
        $('#form-modal-error .filelist').html("The number of file exceeds the allowed total file of  " + maxFileCount + " files each time");
        $('#form-modal-error').modal('show');
        return false;
    }


    var fileNames = [];

    for (var i = 0; i < fileList.length; i++) {
        var file = fileList[i];

        if (file.size == 0) {
            resetModal();
            $('#form-modal-error .filelist').html("Unable to upload 0B file");
            $('#form-modal-error').modal('show');
            return false;
        }

        if (file.size > parseInt(maxFileSize) * 1024 * 1024) {
            resetModal();
            $('#form-modal-error .filelist').html("File [" + file.name + "] is larger than the maximum size of " + maxFileSize + " MB perfile");
            $('#form-modal-error').modal('show');
            return false;
        }

        var fileExtension = file.name.substring(file.name.lastIndexOf('.') + 1, file.name.length) || file.name;

        if (!(validExtension.indexOf(fileExtension.toLowerCase()) > -1 && file.name.indexOf('.') > -1)) {
            resetModal();
            $('#form-modal-error .filelist').html("Invalid file extension. Please check again.");
            $('#form-modal-error').modal('show');
            return false;
        }

        if (fileNames.indexOf(file.name.toLowerCase()) > -1) {
            resetModal();
            $('#form-modal-error .filelist').html("Duplicate file name. Please check again");
            $('#form-modal-error').modal('show');
            return false;
        }
        fileNames.push(file.name.toLowerCase());
    }
    return true;
}

function closeModal() {
    resetModal();
}

function locateCenter() {
    let w = $(window).width();
    let h = $(window).height();

    let cw = $('#form-modal-progress, #form-modal-success, #form-modal-error').outerWidth();
    let ch = $('#form-modal-progress, #form-modal-success, #form-modal-error').outerHeight();

    $('#form-modal-progress, #form-modal-success, #form-modal-error').css({
        'left': ((w - cw) / 2) + 'px',
        'top': ((h - ch) / 2) + 'px'
    });
}