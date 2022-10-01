function OpenFile(length) {
    $.ajax({
        type: 'POST',
        url: 'OpenFile',
        success: function (data) {
            console.log(data);
        },
        error: function(data) {
            alert("Cannot save file");
        }
    });
}