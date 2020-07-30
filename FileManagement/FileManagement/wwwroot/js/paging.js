$(document).ready(function () {
    $('.custom-pagination').rpmPagination({
        limit: parseInt(numPerPage),
        total: parseInt(totalRecords),
        currentPage: parseInt(currentPage)
    });
});