let cart = {
    cartContent: ko.observableArray(),
    cartIsEmpty: ko.observable(),
    totalPrice: ko.observable()
};
ko.applyBindings(cart, document.getElementById('cart'));

GetCart();
function GetCart() {
    $.getJSON('/cart/getcart',
        function (data) {
            cart.cartContent(data)
                .cartIsEmpty(data.length > 0 ? false : true)
                .totalPrice(Sum(data));
        }
    );
}

if ($('#itemsInCart').text() === "") {
    $('#itemsInCart').hide();
}

function RemoveFromCart(value) {
    $('.cart').addClass('loading');
    $.ajax({
        url: '/cart/remove',
        type: 'POST',
        dataType: 'json',
        data: { 'jewelryid': value },
        success: function (data) {
            cart.cartContent(data)
                .cartIsEmpty(data.length > 0 ? false : true);
            $('#itemsInCart').html(data.length);
            if (!cart.cartIsEmpty || data.length === 0) {
                $('#itemsInCart').hide();
            }
            $('.cart').removeClass('loading');
        }
    });
}

function ChangeQuantity(jewelryid, obj) {
    let input = obj.closest('.cart-plus-minus').find('input');
    $('.cart').addClass('loading');
    let quantity = parseInt(input.val()) || 0;
    $.ajax({
        url: '/cart/changequantity',
        type: 'GET',
        dataType: 'json',
        data: { 'jewelryid': jewelryid, 'quantity': quantity },
        success: function (data) {
            cart.cartContent(data)
                .totalPrice(Sum(data));
            $('.cart').removeClass('loading');
        }
    });
}

function Sum(data) {
    let s = 0;
    for (i = 0; i < data.length; i++) {
        s += parseFloat(data[i].total_price.replace(',', '.'));
    }
    return s.toFixed(2);
}