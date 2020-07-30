$(document).ready(function () {
    $("#uploadDateFrom").datepicker({
    });
    $('#uploadDateTo').datepicker({
    });
    checkAllEmpty();
});
function checkAllEmpty() {
    if ($("#fileName").val().trim() == "" && $("#uploadDateFrom").val() == "" && $("#uploadDateTo").val() == "" && $("#keyWord").val().trim() == "") {
        $("#btnSearch").addClass('disabled')
    } else {
        $("#btnSearch").removeClass('disabled');
    }
}
function submitForm() {
    if ($('#uploadDateFrom').datepicker('getDate') == null && $('#uploadDateTo').datepicker('getDate') == null && $("#fileName").val().trim() == "" && $("#keyWord").val().trim() == "") {
        return;
    }

    if ($('#uploadDateFrom').datepicker('getDate') != null && !checkDate($('#uploadDateFrom').val()) || $('#uploadDateTo').datepicker('getDate') != null && !checkDate($('#uploadDateTo').val())) {
        $('#form-modal-message .modal-body').html("<div>「ファイルアップロード日時」に誤りがあります</div>");
        $('#form-modal-message').modal('show');
    } else if ($('#uploadDateFrom').datepicker('getDate') != null && $('#uploadDateTo').datepicker('getDate') != null && $('#uploadDateFrom').datepicker('getDate') > $('#uploadDateTo').datepicker('getDate')) {
        $('#form-modal-message .modal-body').html("<div>終了日付に開始日付より前の日付入力できません、再度確認してください。</div>");
        $('#form-modal-message').modal('show');
    } else if ($("#fileName").val().length > maxLengthSearchFileName) {
        $('#form-modal-message .modal-body').html("<div>ファイル名は" + maxLengthSearchFileName+ "文字以内でご入力ください。</div>");
        $('#form-modal-message').modal('show');
    }
    else if ($("#keyWord").val().length > maxLengthSearchKeyword) {
        $('#form-modal-message .modal-body').html("<div>キーワードは" + maxLengthSearchKeyword+ "文字以内でご入力ください。</div>");
        $('#form-modal-message').modal('show');
    }
    else {
        $("#currentPage").val(1);
        $("#searchViewModel").submit();
    }
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
    $("#keyWord").val($("#keyWordVB").val());
    $("#uid").val($("#uidVB").val());
    $("#at").val($("#atVB").val());
    $("#searchViewModel").submit();
}