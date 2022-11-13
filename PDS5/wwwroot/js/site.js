function GenerateSignature(isFromFile)
{
    const value = $(isFromFile ? '#filePath' : '#inputValue').val();

    $.ajax({
        type: 'POST',
        url: 'GenerateSignature',
        data: {
            value: value,
            isFromFile: isFromFile
        },
        success: function (data) {
            $('#signature').val(data);
        },
        error: function (data) {
            alert(JSON.stringify(data));
        }
    });
}

function SaveSignature()
{
    const filePath = $('#signaturePath').val();
    const value = $('#signature').val();

    $.ajax({
        type: 'POST',
        url: 'SaveSignature',
        data: {
            filePath: filePath,
            value: value
        },
        success: function (data) {
            alert("Signature has been saved");
        },
        error: function (data) {
            alert(JSON.stringify(data));
        }
    });
}

function Check(isFromFile)
{
    const signatureValue = $(isFromFile ? '#signaturePath' : '#signature').val();
    const checkValue = $(isFromFile ? '#fileCheck' : '#inputCheck').val();

    $.ajax({
        type: 'POST',
        url: 'Check',
        data: {
            signatureValue: signatureValue,
            isFromFile: isFromFile,
            checkValue: checkValue
        },
        success: function (data) {
            const msg = data ? "Verified" : "Not Verified";
            alert(msg);
        },
        error: function (data) {
            alert(JSON.stringify(data));
        }
    });
}