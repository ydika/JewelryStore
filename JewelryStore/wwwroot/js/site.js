$('.quantity .cart-plus-minus > div').on('click', function () {
    NumerickClick($(this));
});

$('.quantity .cart-plus-minus > div').closest('.cart-plus-minus').find('input').on('input', function () {
    NumerickInput($(this));
});

$('.quantity .cart-plus-minus > div').closest('.cart-plus-minus').find('input').on('focus', function () {
    $(this).select();
});

function NumerickClick(obj) {
    let input = obj.closest('.cart-plus-minus').find('input');
    let value = parseInt(input.val()) || 0;
    if (obj.hasClass('dec')) {
        value = value - 1 > 0 ? value - 1 : 1;
    }
    else if (obj.hasClass('inc')) {
        value = value + 1 < 5 ? value + 1 : 5;
    }
    input.val(value).change();
}

function NumerickInput(obj) {
    let value = parseInt(obj.val(), 10) || 1;
    if (value > 5) {
        obj.val(5).change();
    } else {
        obj.val(value).change();
    }
}

function NumerickFocus(obj) {
    obj.select();
}

let searchOptions = {
    options: ko.observable()
};
ko.applyBindings(searchOptions, document.getElementById('searchOptions'));

$('#searchInput').on('input', function () {
    $.getJSON('/catalog/getitemnames?searchName=' + $(this).val(),
        function (data) {
            searchOptions.options(data);
        }
    );
});

if ($('#itemsInCart').text() === "") {
    $('#itemsInCart').hide();
}

function ClickOnSearchName(value) {
    let si = document.getElementById('searchInput');
    si.value = value;
    let form = si.parentNode;
    form.submit();
}