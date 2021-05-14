let item = {
    imageSrc: ko.observable(),
    name: ko.observable()
};
ko.applyBindings(item);

$.getJSON(document.location.pathname + '/getitem',
    function (data) {
        item.imageSrc(data.imageSrc);
        item.name(data.name);
    }
);