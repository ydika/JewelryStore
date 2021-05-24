let cart = {
    cartContent: ko.observable()
};
ko.applyBindings(cart);

GetCart();
function GetCart() {
    $.getJSON('/cart/getcart',
        function (data) {
            cart.cartContent(data);
        }
    );
}

function RemoveFromCart(value) {
    $.ajax({
        url: '/cart/remove',
        type: 'POST',
        dataType: 'json',
        data: { 'jewelryid': value },
        success: function (data) {
            cart.cartContent(data);
        }
    });
}

function ChangeQuantity(jewelryid, obj) {
    let input = obj.closest('.cart-plus-minus').find('input');
    let quantity = parseInt(input.val()) || 0;
    $.ajax({
        url: '/cart/changequantity',
        type: 'GET',
        dataType: 'json',
        data: { 'jewelryid': jewelryid, 'quantity': quantity },
        success: function (data) {
            cart.cartContent(data);
        }
    });
    lastInputValue = quantity;
}