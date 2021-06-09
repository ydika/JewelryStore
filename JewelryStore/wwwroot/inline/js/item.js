let item = {
    price: ko.observable(null),
    quantity: ko.observable(1)
};
ko.applyBindings(item, document.getElementById('itemPrice'));

function AddItemToCart(jewelryid) {
    let size = $('#size').val();
    let input = $('#qInput');
    let quantity = parseInt(input.val()) || 0;
    $.ajax({
        url: '/cart/add',
        type: 'POST',
        dataType: 'json',
        data: { 'jewelryid': jewelryid, 'quantity': quantity, 'size': size },
        success: function (data) {
            $('#itemsInCart').html(data);
            $('#itemsInCart').show();
        }
    });
}

function ChangeQuantity(jewelryid) {
    let size = $('#size').val();
    let input = $('#qInput');
    let quantity = parseInt(input.val()) || 0;
    $.ajax({
        url: '/catalog/getitemprice',
        type: 'GET',
        dataType: 'json',
        data: { 'jewelryid': jewelryid, 'quantity': quantity, 'size': size },
        success: function (data) {
            item.price(data)
                .quantity(quantity);
        }
    });
}