function updateChart() {
    $("#bargraphresult").remove();
    dashboard('#topLoader', freqData);
}

function updateTitle(title) {
    var titleWidth = $("#lblTitle").width();
    var numberchar = Math.floor(titleWidth / 12) * 3;
    $("#lblTitle").text(title.substring(0, numberchar) + "...")
}

function updateResize() {    
    $(".slider").each(function () {
        var preOuterWidth = $(this).prev().outerWidth();
        var nextOuterWidth = $(this).next().outerWidth();
        var nextnextWidth = $(this).next().next().outerWidth();
        var colwidth = $("#col-1").width();

        $(this).outerWidth(colwidth - preOuterWidth - nextOuterWidth - nextnextWidth - 5);

    });
}

function ScrollView(id)
{
    var el = document.getElementById(id)
    if (el != null)
    {        
        el.scrollIntoView();
        el.focus();
    }
}
 