function GetProfilePartial(obj) {
    $.get('/getprofile',
        function (data) {
            $('.selected').removeClass('selected');
            $('#profilePage').html(data);
            obj.addClass('selected');
        }
    );
}

function GetOrdersPartial(obj) {
    $.get('/getorders',
        function (data) {
            $('.selected').removeClass('selected');
            $('#profilePage').html(data);
            obj.addClass('selected');
        }
    );
}