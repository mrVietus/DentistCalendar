// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function SubmitForm(frm, caller) {
    caller.preventDefault();

    var fdata = new FormData(frm);

    $.ajax(
        {
            type: frm.method,
            url: frm.action,
            data: fdata,
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.status === "success") {
                    window.location = window.location.origin + data.url;
                }
                if (data.status === "error") {
                    var validationSummary = $('#validationSummary');
                    var validationSummaryErrors = $('#validation-summary-errors');

                    if (validationSummary.length === 0) {
                        var dcModalContent = $('#validation');
                        dcModalContent.append('<div id="validationSummary" class="col-xl-12 alert alert-danger alert-dismissible fade show" style="display: none">\
                                                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">\
                                                        <span aria-hidden="true">&times;</span>\
                                                    </button>\
                                                    <ul id="validation-summary-errors"></ul>\
                                                  </div>').html();

                        validationSummary = $('#validationSummary');
                        validationSummaryErrors = $('#validation-summary-errors');
                    }

                    var modelErrors = JSON.parse(data.modelErrors);

                    for (var i = 0; i < modelErrors.length; i++) {
                        validationSummaryErrors.append('<li>' + modelErrors[i] + '</li>');
                    }

                    validationSummary.show();
                }

            },
            error: function (data) {
                alert(data.status);
            }
        });
}