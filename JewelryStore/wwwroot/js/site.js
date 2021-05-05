$("#showCatalogSidebar").click(function () {
    $('#filters').addClass('opened');
    $('#bgCatalogSidebar').fadeIn();
});

$("#bgCatalogSidebar").click(function () {
    $('#filters').removeClass('opened');
    $('#bgCatalogSidebar').fadeOut();
});

let o = [];
let page = 1;

function updateInfo() {
    $.ajax({
        url: '/catalog/cards',
        type: 'POST',
        dataType: 'json',
        traditional: true,
        data: { 'o': o, 'page': page },
        success: function (data) {
            $('#cards').html(
                '<div class="row">' +
                data.jewelries.map(j => (
                    `<div class="col-12 col-sm-6 col-md-4 mb-4">
                        <div class="card shadow text-center h-100">
                        <img class="card-img" src="${j.imageSrc}">
                        <div class="card-body pb-0 pt-2 px-1">
                            <div class="card-title">${j.name}</div>
                        </div>
                        <div class="pb-4">
                            <div class="text-danger">
                                от 
                                <span class="h5">${j.price.toFixed(2)}</span> 
                                руб.</div>
                            </div>
                        </div>
                     </div>`
                )).join('') +
                '</div>'
            );
            pageCount = Math.ceil(data.jewelriesCount / 3);
            if (pageCount > 1) {
                str = '<ul class="pagination">';
                for (var i = 1; i < pageCount + 1; i++) {
                    if (i === page) {
                        str += `<li class="page-item active">`;
                    } else {
                        str += `<li class="page-item">`;
                    }
                    str += `<a href="javascript: undefined;" class="page-link"; onclick="pageclick(${i})">${i}</a></li>`;
                }
                str += '</ul>';
                $('#paging').html(str);
            } else {
                $('#paging').html("");
            }
        }
    });
}

$("input:checkbox").click(function () {
    o = [];
    $("input:checkbox:checked").each(function () {
        o.push($(this).val());
    });
    page = 1;
    updateInfo();
});

function pageclick(a) {
    page = a;
    updateInfo();
}

//$("#paging").find("a").click(function () {
//    page = $(this).text();
//    updateInfo();
//});