function SaveEmailTemplate() {
	debugger;
	var body = window.editor.getData();
	var bodyElement = $("#@Html.IdFor(m => m.Body)");
	bodyElement.text(bodyElement.text(body).html());

	$.ajax({
		type: "POST",
		url: '@Url.Action("SendEmailTemplates", "Programme")',
		data: $("#update-form").serialize() // serializes the form's elements.
	})
	.done(function (json) {
		debugger; // do nothing (was debugger without a semi-colon...?)
	})
	.fail(function (err, ajaxOptions, thrownError) {
		debugger;
		console.log(err);
		console.log(ajaxOptions);
		console.log(thrownError);
	});
};