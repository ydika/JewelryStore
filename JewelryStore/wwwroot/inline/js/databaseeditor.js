function GetJewelrysTable(obj) {
    $('main').addClass('loading');
    GetTable('getjewelrystable', obj);
}

function GetCharacteristicsTable(obj) {
    $('main').addClass('loading');
    GetTable('getcharacteristicstable', obj);
}

function GetJewelryKindsTable(obj) {
    $('main').addClass('loading');
    GetTable('getjewelrykindstable', obj);
}

function GetDiscountsTable(obj) {
    $('main').addClass('loading');
    GetTable('getdiscountstable', obj);
}

function GetUsersTable(obj) {
    $('main').addClass('loading');
    GetTable('getuserstable', obj);
}

let currentPage = 1;
function SelectedNewPage(a, obj) {
    if (currentPage != a) {
        $('main').addClass('loading');
        currentPage = a;
        window.scrollTo(0, 0);
        GetTable(`/dbchangering/getjewelrystable?page=${a}`, obj);
    }
}

function GetTable(url, obj) {
    $.get(`${url}`,
        function (data) {
            $('#dbTable').html(data);
            if (obj != null) {
                $('.active').addClass('link-dark').removeClass('active');
                $(obj).addClass('active');
            }
            $('main').removeClass('loading');
        }
    );
}

$('form').on('submit', function () {
    return confirm('Подтвердите выполнение действия.');;
});