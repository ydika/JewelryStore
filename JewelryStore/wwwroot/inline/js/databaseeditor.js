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

function GetTable(url, obj) {
    $.get(`${url}`,
        function (data) {
            $('.active').addClass('link-dark').removeClass('active');
            $('#dbTable').html(data);
            $(obj).addClass('active');
            $('main').removeClass('loading');
        }
    );
}