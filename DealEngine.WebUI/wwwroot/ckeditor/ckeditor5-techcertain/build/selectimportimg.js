// Note this isn't included in build time, if you can please do so - didn't have time to do so.
// Use these three sources wherever you use the build in your page script and stylesheet references
// <script src="~/ckeditor/ckeditor5-techcertain/build/ckeditor.js"></script>
// <script src="~/ckeditor/ckeditor5-techcertain/build/selectimportimg.js"></script>
// <link href="~/ckeditor/ckeditor5-techcertain/build/ckeditor.css" rel="stylesheet" />

function selectImg(image) {
    var imgName = image.getAttribute('name');
    $('#selImg').val(imgName);
}

function importImg() {
    var triggerMutationObserver = $('#importImg').val();
    triggerMutationObserver += "1";
    $('#importImg').val(triggerMutationObserver);
}