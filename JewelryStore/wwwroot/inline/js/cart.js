let cart = {
    cartContent: ko.observableArray(),
    cartIsEmpty: ko.observable()
};
ko.applyBindings(cart, document.getElementById('cart'));

GetCart();
function GetCart() {
    $.getJSON('/cart/getcart',
        function (data) {
            cartLength.length(data.length);
            cart.cartContent(data)
                .cartIsEmpty(data.length > 0 ? false : true);
        }
    );
}

function RemoveFromCart(value) {
    $('.cart').addClass('loading');
    $.ajax({
        url: '/cart/remove',
        type: 'POST',
        dataType: 'json',
        data: { 'jewelryid': value },
        success: function (data) {
            cartLength.length(data.length);
            cart.cartContent(data)
                .cartIsEmpty(data.length > 0 ? false : true);
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
            cart.cartContent(data);
            $('.cart').removeClass('loading');
        }
    });
}