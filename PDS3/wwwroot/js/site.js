function OpenFile(dec) {
    const password1 = $('#password1').val();
    const password2 = $('#password2').val();
    const filePath = $('#filePath2').val();

    if (password1 === password2) {
        $.ajax({
            type: 'POST',
            url: 'OpenFile',
            data: {
                filepath: filePath,
                dec: dec
            },
            success: function (data) {
                console.log(data);
            },
            error: function (data) {
                alert("Cannot open file");
            }
        });
    }
}

function Encrypt() {
    const password = $('#password1').val();
    const filePath = $('#filePath1').val();
    $.ajax({
        type: 'POST',
        url: 'Encrypt',
        data: {
            password: password,
            filePath: filePath
        },
        success: function (data) {
            alert("Encrypted");
        },
        error: function (data) {
            alert(data);
        }
    });
}

function Decrypt() {
    const password = $('#password2').val();
    const filePath = $('#filePath2').val();
    $.ajax({
        type: 'POST',
        url: 'Decrypt',
        data: {
            password: password,
            filePath: filePath
        },
        success: function (data) {
            alert("Decrypted");
        },
        error: function (data) {
            alert(data);
        }
    });
}