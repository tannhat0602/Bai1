$(document).ready(function () {
    // Toggle form tìm kiếm
    $('#searchToggle').on('click', function (e) {
        e.stopPropagation();
        $('#searchForm').toggle();
        $('#searchForm input').focus();
    });

    // Ẩn form khi click ngoài
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.search-container').length) {
            $('#searchForm').hide();
        }
    });

    // Ẩn form khi nhấn ESC
    $(document).on('keydown', function (e) {
        if (e.key === "Escape") {
            $('#searchForm').hide();
        }
    });
});
