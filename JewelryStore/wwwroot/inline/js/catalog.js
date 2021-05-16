let o = [];
let currentPage = 1;

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

let jewelriesSection = {
    jewelries: ko.observable(),
    jewelriesNotFound: ko.observable(),
    current: ko.observable(),
    pages: ko.observable(),
    pagesVisible: ko.observable(),
    selectedNewPage: function NewPage(a) {
        currentPage = a;
        GetJewelries();
        ScrollUp();
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
            jewelriesSection
                .jewelries(data.jewelries)
                .jewelriesNotFound(data.jewelries.length > 0 ? false : true)
                .current(data.current_page)
                .pages(PageRange(currentPage, data.page_count))
                .pagesVisible(data.page_count > 1 ? true : false);
        }
    });
}

function PageRange(current, count) {
    numbers = [];
    count++;
    for (let i = count - 3 < current ? count - 5 : (current - 2 > 0 ? current - 2 : 1); i < (current < 3 ? 6 : (current + 3 < count ? current + 3 : count)); i++) {
        numbers.push(i);
    }
    return numbers;
}

var timeOut;
function ScrollUp() {
    var top = Math.max(document.body.scrollTop, document.documentElement.scrollTop);
    if (top > 0) {
        window.scrollBy(0, -100);
        timeOut = setTimeout('goUp()', 20);
    } else clearTimeout(timeOut);
}

$("#showCatalogSidebar").click(function () {
    $('#filters').addClass('opened');
    $('#bgCatalogSidebar').fadeIn();
});

$("#bgCatalogSidebar").click(function () {
    $('#filters').removeClass('opened');
    $('#bgCatalogSidebar').fadeOut();
});