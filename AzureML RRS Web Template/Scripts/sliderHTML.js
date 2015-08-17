function updatesliderHTML() {
    var x = document.getElementsByClassName("amount");
    var i;
    for (i = 0; i < x.length; i++) {
        var id = "slider_" + x[i].id;
        document.getElementById(id).value = x[i].value;
    };
};