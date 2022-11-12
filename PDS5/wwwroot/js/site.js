function OpenFile(dec, rc5) {
    let password1;
    let password2;
    if (rc5) {
        password1 = $('#password1').val();
        password2 = $('#password2').val();
    }
    const filePath = $(`#filePath${rc5 ? '2' : '4'}`).val();

    if (!rc5 || password1 === password2) {
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

function EncryptRC5() {
    const password = $('#password1').val();
    const filePath = $('#filePath1').val();
    $.ajax({
        type: 'POST',
        url: 'EncryptRC5',
        data: {
            password: password,
            filePath: filePath
        },
        success: function (data) {
            let elem = $("#time1");
            $(elem).removeClass("d-none");
            $(elem).children().text(data);
            alert("Encrypted");
        },
        error: function (data) {
            alert(data);
        }
    });
}

function DecryptRC5() {
    const password = $('#password2').val();
    const filePath = $('#filePath2').val();
    $.ajax({
        type: 'POST',
        url: 'DecryptRC5',
        data: {
            password: password,
            filePath: filePath
        },
        success: function (data) {
            let elem = $("#time2");
            $(elem).removeClass("d-none");
            $(elem).children().text(data);
            alert("Decrypted");
        },
        error: function (data) {
            alert(data);
        }
    });
}

function EncryptRSA() {
    const filePath = $('#filePath3').val();
    $.ajax({
        type: 'POST',
        url: 'EncryptRSA',
        data: {
            filePath: filePath
        },
        success: function (data) {
            let elem = $("#time3");
            $(elem).removeClass("d-none");
            $(elem).children().text(data);
            alert("Encrypted");
        },
        error: function (data) {
            alert(data);
        }
    });
}

function DecryptRSA() {
    const filePath = $('#filePath4').val();
    $.ajax({
        type: 'POST',
        url: 'DecryptRSA',
        data: {
            filePath: filePath
        },
        success: function (data) {
            let elem = $("#time4");
            $(elem).removeClass("d-none");
            $(elem).children().text(data);
            alert("Decrypted");
        },
        error: function (data) {
            alert(data);
        }
    });
}