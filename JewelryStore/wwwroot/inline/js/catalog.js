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
let minPrice = document.getElementById("minPrice");
let maxPrice = document.getElementById("maxPrice");
const sliders = document.querySelectorAll('input[type="range"]');

sliders[0].addEventListener('input', (e) => {
    if (+sliders[0].value > +sliders[1].value) {
        sliders[1].value = +sliders[0].value;
    }
    UpdateSliderResult(sliders[0].value, sliders[1].value);
});

sliders[1].addEventListener('input', (e) => {
    if (+sliders[1].value < +sliders[0].value) {
        sliders[0].value = +sliders[1].value;
    }
    UpdateSliderResult(sliders[0].value, sliders[1].value);
});

sliders.forEach((slider) => {
    slider.addEventListener('change', () => {
        GetJewelries();
    })
});

minPrice.oninput = function () {
    sliders[0].value = this.value;
    if (+sliders[0].value > +sliders[1].value) {
        sliders[1].value = +sliders[0].value;
        UpdateSliderResult(sliders[0].value, sliders[1].value);
    }
    GetJewelries();
}

maxPrice.oninput = function () {
    sliders[1].value = this.value;
    if (+sliders[1].value < +sliders[0].value) {
        sliders[1].value = +sliders[0].value;
        UpdateSliderResult(sliders[0].value, sliders[1].value);
    }
    GetJewelries();
}

function UpdateSliderResult(minP, maxP) {
    minPrice.value = minP;
    maxPrice.value = maxP;
}

let o = [];
let currentPage = 1;

$("input:checkbox").click(function () {
    $('#productsSection').addClass('loading');
    currentPage = 1;
    GetJewelries();
});

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

let queryString;
GetJewelries();
function GetJewelries() {
    queryString = new URLSearchParams(document.location.search);
    o = [];
    $("input:checkbox:checked").each(function () {
        o.push($(this).val());
    });
    $.ajax({
        url: document.location.pathname + '/getjewelriescards',
        type: 'GET',
        dataType: 'json',
        traditional: true,
        data: { 'subspecies': queryString.get('subspecies'), 'searchName': queryString.get('searchName'), 'o': o, 'page': currentPage, 'minPrice': sliders[0].value, 'maxPrice': sliders[1].value },
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
            $('#itemsInCart').html(data);
            $('#itemsInCart').show();
        }
    });
}

function GoTo(url) {
    document.location.href = url;
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

$(".filter-block .show-button").click(function () {
    $(this).hide();
    $(this).siblings(".hiden-block").slideDown();
    $(this).siblings(".hide-button").show();
});

$(".filter-block .hide-button").click(function () {
    $(this).hide();
    $(this).siblings(".hiden-block").slideUp();
    $(this).siblings(".show-button").show();
});