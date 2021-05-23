$('.quantity .cart-plus-minus > div').on('click', function () {
    let input = $(this).closest('.cart-plus-minus').find('input');
    let value = parseInt(input.val()) || 0;
    if ($(this).hasClass('dec')) {
        value = value - 1 > 0 ? value - 1 : 1;
    }
    if ($(this).hasClass('inc')) {
        value = value + 1 < 10 ? value + 1 : 10;
    }
    input.val(value).change();
});

let lastValue = 1;
$('.quantity .cart-plus-minus > div').closest('.cart-plus-minus').find('input').on('input', function () {
    let value = parseInt($(this).val()) || 0;
    if (value > 10 || value < 1) {
        $(this).val(lastValue).change();
    }
    else {
        lastValue = value;
    }
});