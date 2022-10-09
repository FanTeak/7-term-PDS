function OpenFile() {
    $.ajax({
        type: 'POST',
        url: 'OpenFile',
        success: function (data) {
            console.log(data);
        },
        error: function (data) {
            alert("Cannot save file");
        }
    });
}

function Compare() {
    const filePath = $('#filePath').val();
    const stringToCompare = $('#stringToCompare').val();

    $.ajax({
        type: 'POST',
        url: 'Compare',
        data: {
            filePath: filePath,
            stringToCompare: stringToCompare
        },
        success: function (data) {
            if (data.equals) {
                alert("Files are equals!");
            }
        },
        error: function (data) {
            alert("Cannot save file");
        }
    });
}

function ReadFile() {
    const filePath = $('#filePath').val();
    $.ajax({
        type: 'POST',
        url: 'ReadFile',
        data: {
            filePath: filePath
        },
        success: function (data) {
            $('#hashFile').val(data);
        },
        error: function (data) {
            alert("Cannot save file");
        }
    });
}