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
    let value = parseInt(obj.val()) || 0;
    if (value > 5 || value < 1) {
        obj.val(lastValue).change();
    }
    else {
        obj.val(parseInt(obj.val(), 10)).change();
    }
}

function NumerickFocus(obj) {
    obj.select();
}