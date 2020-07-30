﻿$(document).ready(function () {
    changeInput();
    $('#fromDate').datepicker({
        onSelect: function () {
            changeInput();
        }
    });
    $('#toDate').datepicker({
        onSelect: function () {
            changeInput();
        }
    });
});

$("#userId").on('input', changeInput);
$("#fileName").on('input', changeInput);
$("#fromDate").on('input', changeInput);
$("#toDate").on('input', changeInput);
$("#upload").click(changeInput);
$("#download").click(changeInput);


function changeInput() {
    if (checkInput())
        $("#submitButton").prop("disabled", false);
    else
        $("#submitButton").prop("disabled", true);
}

function checkInput() {
    return !($("#userId").val().trim() == "" && $("#fileName").val().trim() == "" && $("#fromDate").val() == "" && $("#toDate").val() == "" && $("#upload").prop("checked") == false && $("#download").prop("checked") == false);
}

function checkDate(date) {
    if (!/^([0-9]{4}年[0-1]{0,1}[0-9]月[0-3]{0,1}[0-9]日)$/.test(date))
        return false;

    var bits = date.split(/年|月|日/);
    var d = new Date(bits[0], bits[1] - 1, bits[2]);
    return d && (d.getMonth() + 1) == bits[1];
}

function submitForm() {
    var validation = validateInput();
    var inputCheck = checkInput();
    if (validation && inputCheck)
    {
        $("#page").val(1);
        $("#searchForm").submit();
    }
}

function validateInput() {
    if ($("#fileName").val().length > maxLengthSearchFileName) {
        resetModal();
        $('#form-modal-message .modal-body').text("ファイル名 " + maxLengthSearchFileName + " 文字以内でご入力ください。");
        $('#form-modal-message').modal('show');
        return false;
    }
    if ($("#userId").val().length > maxLengthSearchUserId) {
        resetModal();
        $('#form-modal-message .modal-body').text("ユーザーID " + maxLengthSearchUserId + " 文字以内でご入力ください。");
        $('#form-modal-message').modal('show');
        return false;
    }
    if (($('#fromDate').datepicker('getDate') != null && !checkDate($('#fromDate').val())) || ($('#toDate').datepicker('getDate') != null && !checkDate($('#toDate').val()))) {
        resetModal();
        $('#form-modal-message .modal-body').text("「アクセス日時の期間を設定する」に誤りがあります");
        $('#form-modal-message').modal('show');
        return false;
    }
    if ($('#fromDate').datepicker('getDate') != null && $('#toDate').datepicker('getDate') != null && $('#fromDate').datepicker('getDate') > $('#toDate').datepicker('getDate')) {
        resetModal();
        $('#form-modal-message .modal-body').text("終了日付に開始日付より前の日付入力できません、再度確認してください。");
        $('#form-modal-message').modal('show');
        return false;
    }
    return true;
}

function resetModal() {
    $('#form-modal-message').modal('hide');
}

function exportCSV() {
    $("#type").val("csv");
    console.log($("#type").val());
    $("#ExportFile").submit();
}
function exportExcel() {
    $("#type").val("excel");
    console.log($("#type").val());
    $("#ExportFile").submit();
}

function paging(num) {
    if (num < 1 || num > parseInt(pageCount)) return;
    $("#page").val(num);
    $("#fileName").val($("#fileNameInput").val());
    $("#userId").val($("#userIdInput").val());
    $("#fromDate").val($("#fromDateInput").val());
    $("#toDate").val($("#toDateInput").val());
    $("#upload").prop('checked', $("#uploadInput").prop('checked'));
    $("#download").prop('checked', $("#downloadInput").prop('checked'));
    $("#searchForm").submit();
}
