$(function () {
    $(".IEslider").each(function () {
        var begin = $(this).data("begin"),
            end = $(this).data("end"),
            def = $(this).data("def"),
            step = $(this).data("step");

        $(this).slider({
            range: "min",
            value: def,
            min: begin,
            max: end,
            step: step,
            slide: function (event, ui) {
                var slideramount = $(this).next();
                $(slideramount).val(ui.value);
            }
        });        
    });
    $('.amount').change(function () {
        var value = this.value,
            selector = ("#IEslider_" + this.id);
        $(selector).slider("value", value);
    })
});


function updateSliderIE() {
    $('.amount').each(function () {
        var value = this.value,
                selector = ("#IEslider_" + this.id);
        $(selector).slider("value", value);
    });
}

