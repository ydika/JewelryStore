$("#showCatalogSidebar").click(function () {
    $('#filters').addClass('opened');
    $('#bgCatalogSidebar').fadeIn();
});

$("#bgCatalogSidebar").click(function () {
    $('#filters').removeClass('opened');
    $('#bgCatalogSidebar').fadeOut();
});

let o = [];
let currentPage = 1;

var viewModel = {
    jewelries: ko.observable(),
    current: ko.observable(),
    pages: ko.observableArray(),
    pagesVisible: ko.observable(),
    selectedNewPage: function NewPage(a) {
        currentPage = a;
        GetJewelries();
    }
};
ko.applyBindings(viewModel);

GetJewelries();
function GetJewelries() {
    $.ajax({
        url: '/catalog/cards',
        type: 'GET',
        dataType: 'json',
        traditional: true,
        data: { 'o': o, 'page': currentPage },
        success: function (data) {
            pageNumbers = [];
            for (var i = 1; i < data.page_count + 1; i++) {
                pageNumbers.push(i);
            }
            viewModel.jewelries(data.jewelries);
            viewModel.current(data.current_page);
            viewModel.pages(pageNumbers);
            viewModel.pagesVisible(pageNumbers.length > 1 ? true : false);
        }
    });
}

$("input:checkbox").click(function () {
    o = [];
    $("input:checkbox:checked").each(function () {
        o.push($(this).val());
    });
    currentPage = 1;
    GetJewelries();
});