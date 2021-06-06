let x = 0;
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

function AddSubspeciesInput() {
    var str = `<input type='text' name='sRuName' style='width: 30%' placeholder='Название на русском' />
    <input type='text' name='sEnName' style='width: 30%' placeholder='Название на английском' /><br /><div id=subspeciesInputs${x + 1}></div>`;
    document.getElementById('subspeciesInputs' + x).innerHTML = str;
    x++;
}