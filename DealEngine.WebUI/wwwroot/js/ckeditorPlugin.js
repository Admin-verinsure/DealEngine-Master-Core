
function selectImg(img) {
    var filename = img.name;
    $('#selImg').val(filename);
}
function importImg() {
    var filename = $('#selImg').val();
    $('#importImg').val(filename);
}

