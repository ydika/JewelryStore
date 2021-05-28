////window.onload = function () {
////    window.setTimeout(
////        function () {
////            window.addEventListener(
////                "popstate",
////                function (e) {
////                    $.ajax({
////                        url: document.location.pathname + '/getjewelriescards',
////                        type: 'GET',
////                        dataType: 'json',
////                        traditional: true,
////                        data: { 'o': o, 'page': new URLSearchParams(document.location.search).get('page') },
////                        success: function (data) {
////                            jewelriesSection
////                                .jewelries(data.jewelries)
////                                .jewelriesNotFound(data.jewelries.length > 0 ? false : true)
////                                .current(data.current_page)
////                                .pages(PageRange(currentPage, data.page_count))
////                                .pageCount(data.page_count)
////                                .pagesVisible(data.page_count > 1 ? true : false);
////                            if (oldPage != currentPage) {
////                                window.scrollTo(0, 0);
////                            }
////                        }
////                    });
////                    e.preventDefault();
////                },
////                false
////            );
////        },
////    );
////}
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
            $('#productsSection').addClass('loading');
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
    pageCount: ko.observable(),
    pagesVisible: ko.observable(),
    selectedNewPage: function NewPage(a) {
        if (currentPage != a) {
            $('#productsSection').addClass('loading');
            currentPage = a;
            window.scrollTo(0, 0);
            GetJewelries();
        }
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
        data: { 'searchName': new URLSearchParams(document.location.search).get('searchName'), 'o': o, 'page': currentPage },
        success: function (data) {
            jewelriesSection
                .jewelries(data.jewelries)
                .jewelriesNotFound(data.jewelries.length > 0 ? false : true)
                .current(data.current_page)
                .pages(PageRange(currentPage, data.page_count))
                .pageCount(data.page_count)
                .pagesVisible(data.page_count > 1 ? true : false);
            $('#productsSection').removeClass('loading');
            /*history.pushState({ 'currentPage': currentPage, 'o': o }, '', `?page=${currentPage}`);*/
        }
    });
}

function AddToCart(value) {
    $.ajax({
        url: '/cart/add',
        type: 'POST',
        dataType: 'json',
        data: { 'jewelryid': value },
        success: function (data) {
            cartLength.length(data.length);
        }
    });
}

function PageRange(current, count) {
    numbers = [];
    if (count > 5) {
        for (let i = count - 3 < current ? count - 4 : (current - 2 > 0 ? current - 2 : 1); i <= (current < 3 ? 5 : (current + 2 < count ? current + 2 : count)); i++) {
            numbers.push(i);
        }
    }
    else {
        for (var i = 1; i <= count; i++) {
            numbers.push(i);
        }
    }
    return numbers;
}

$("#showCatalogSidebar").click(function () {
    $('#filters').addClass('opened');
    $('#bgCatalogSidebar').fadeIn();
});

$("#bgCatalogSidebar").click(function () {
    $('#filters').removeClass('opened');
    $('#bgCatalogSidebar').fadeOut();
});