$('form').on('submit', function () {
    return confirm('Подтвердите выполнение действия.');;
});

let nameSearch = {
    names: ko.observable()
};
ko.applyBindings(nameSearch, document.getElementById('jewelryNames'));

$('#nameSearch').on('input', function () {
    $.getJSON('/catalog/getitemnames?searchName=' + $(this).val(),
        function (data) {
            nameSearch.names(data);
        }
    );
});

function ClickOnName(name) {
    $('#nameSearch').val(name);
}