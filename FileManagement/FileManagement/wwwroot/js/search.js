$(document).ready(function () {
    $("#uploadDateFrom").datepicker({
    });
    $('#uploadDateTo').datepicker({
    });
    checkAllEmpty();
});
function checkAllEmpty() {
    if ($("#fileName").val().trim() == "" && $("#uploadDateFrom").val() == "" && $("#uploadDateTo").val() == "") {
        $("#btnSearch").addClass('disabled')
    } else {
        $("#btnSearch").removeClass('disabled');
    }
}
function submitForm() {
    if ($('#uploadDateFrom').datepicker('getDate') == null && $('#uploadDateTo').datepicker('getDate') == null && $("#fileName").val().trim() == "") {
        return;
    }

    if ($('#uploadDateFrom').datepicker('getDate') != null && !checkDate($('#uploadDateFrom').val()) || $('#uploadDateTo').datepicker('getDate') != null && !checkDate($('#uploadDateTo').val())) {
        $('#form-modal-message .modal-body').html("<div>Invalid upload date</div>");
        $('#form-modal-message').modal('show');
        return;
    }

    if ($('#uploadDateFrom').datepicker('getDate') != null && $('#uploadDateTo').datepicker('getDate') != null && $('#uploadDateFrom').datepicker('getDate') > $('#uploadDateTo').datepicker('getDate')) {
        $('#form-modal-message .modal-body').html("<div>The end date could not be ealier than the start date</div>");
        $('#form-modal-message').modal('show');
        return;
    }

    $("#currentPage").val(1);
    $("#searchViewModel").submit();

}
function checkDate(date) {
    if (!/^([0-9]{4}年[0-1]{0,1}[0-9]月[0-3]{0,1}[0-9]日)$/.test(date))
        return false;
    var bits = date.split(/年|月|日/);
    var d = new Date(bits[0], bits[1] - 1, bits[2]);
    return d && (d.getMonth() + 1) == bits[1];
}

function resetModal() {
    $('#form-modal-message').modal('hide');
}

function paging(num) {
    if (num < 1 || num > parseInt(pageCount)) return;
    $("#currentPage").val(num);
    $("#fileName").val($("#fileNameVB").val());
    $("#uploadDateFrom").val($("#uploadDateFromVB").val());
    $("#uploadDateTo").val($("#uploadDateToVB").val());
    $("#searchViewModel").submit();
}