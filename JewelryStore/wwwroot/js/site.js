let openSidebarFilters = function (need) {
    if (need == 'open') {
        $('#filters').addClass('opened');
        $('.bg-sidebarFilters').fadeIn();
    } else {
        $('#filters').removeClass('opened');
        $('.bg-sidebarFilters').fadeOut();
    }
}

let sortByPrice = "Без сортировки";
let jKind = "Все";
let jOtherChar = [];

function updateInfo() {
    $.ajax({
        url: '/Catalog/Cards',
        type: 'GET',
        data: { 'jKind': jKind, 'sortByPrice': sortByPrice, 'jOtherChar': jOtherChar },
        success: function (data) {
            $('#cards').html(data);
        }
    });
}

$("#sortByPrice").change(function () {
    sortByPrice = $(this).val();
    updateInfo();
});

$("#filters").find("a").click(function () {
    jKind = $(this).children("span").text();
    updateInfo();
});

$("input:checkbox").click(function () {
    jOtherChar = [];
    $("input:checkbox:checked").each(function () {
        jOtherChar.push($(this).val());
    });
    updateInfo();
});