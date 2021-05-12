let filtersSection = {
    characteristics: ko.observable()
};
ko.applyBindings(filtersSection, document.getElementById('filterSection'));

$.getJSON('/catalog/getcatalogfilter',
    function (data) {
        filtersSection.characteristics(data);
        $("input:checkbox").click(function () {
            o = [];
            $("input:checkbox:checked").each(function () {
                o.push($(this).val());
            });
            currentPage = 1;
            GetJewelries();
        });
    }
);

let o = [];
let currentPage = 1;

let jewelriesSection = {
    jewelries: ko.observable(),
    jewelriesNotFound: ko.observable(),
    current: ko.observable(),
    pages: ko.observable(),
    pagesVisible: ko.observable(),
    selectedNewPage: function NewPage(a) {
        currentPage = a;
        GetJewelries();
    }
};
ko.applyBindings(jewelriesSection, document.getElementById('productsSection'));

GetJewelries();
function GetJewelries() {
    $.ajax({
        url: document.location.pathname + '/getjewelriescards',
        type: 'GET',
        dataType: 'json',
        traditional: true,
        data: { 'o': o, 'page': currentPage },
        success: function (data) {
            pageNumbers = [];
            for (let i = 1; i < data.page_count + 1; i++) {
                pageNumbers.push(i);
            }
            jewelriesSection
                .jewelries(data.jewelries)
                .jewelriesNotFound(data.jewelries.length > 0 ? false : true)
                .current(data.current_page)
                .pages(pageNumbers)
                .pagesVisible(pageNumbers.length > 1 ? true : false);
        }
    });
}

$("#showCatalogSidebar").click(function () {
    $('#filters').addClass('opened');
    $('#bgCatalogSidebar').fadeIn();
});

$("#bgCatalogSidebar").click(function () {
    $('#filters').removeClass('opened');
    $('#bgCatalogSidebar').fadeOut();
});