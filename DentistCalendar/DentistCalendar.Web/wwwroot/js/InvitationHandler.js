dc = {
    onAcceptClick: function (buttonClicked, invitationAccountType) {
        var id = buttonClicked.dataset.id;
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: '/Invitation/AcceptInvitation',
            contentType: "application/json; charset=utf-8",
            data: {
                "invitationGuid": id,
                "accountType": invitationAccountType
            },
            datatype: "json",
            success: function (data) {
                if (data.status === "success") {
                    swal({
                        type: 'success',
                        html: '<p>Zaproszenie zostało zaakceptowane.</p>',
                        showConfirmButton: false,
                        timer: 2000
                    });

                    setTimeout(function () {
                        window.location = window.location.origin + data.url;
                    }, 500);
                }
                if (data.status === "error") {
                    swal({
                        type: 'error',
                        title: 'Oops...',
                        text: 'Coś poszło nie tak!'
                    });
                }         
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    },

    onRemoveClick: function (buttonClicked, invitationAccountType, dentistOfficeName) {
        var id = buttonClicked.dataset.id;

        swal({
            title: 'Czy napewno chcesz usunąć zaproszenie?',
            html: '<div class="h3">' + dentistOfficeName + '</div>',
            showCancelButton: true,
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(function (result) {
            if (result.value === true) {

                $.ajax({
                    type: "POST",
                    url: '/Invitation/RejectInvitation',
                    data: {
                        "invitationGuid": id,
                        "accountType": invitationAccountType
                    },
                    success: function (data) {
                        if (data.status === "success") {
                            swal({
                                type: 'success',
                                html: '<p>Zaproszenie od ' + dentistOfficeName + ' został odrzucone.</p>',
                                showConfirmButton: false,
                                timer: 2000
                            });

                            setTimeout(function () {
                                window.location = window.location.origin + data.url;
                            }, 500);
                        }
                        if (data.status === "error") {
                            swal({
                                type: 'error',
                                title: 'Oops...',
                                text: 'Coś poszło nie tak!'
                            });
                        }
                    },
                    error: function () {
                        swal({
                            type: 'error',
                            title: 'Oops...',
                            text: 'Coś poszło nie tak!'
                        });
                    }
                });
            }
        }).catch(swal.noop);
    },

    showSwal: function (type) {
        if (type === 'input-field') {
            swal({
                title: 'Edycja danych gabinetu',
                html: '<div class="form-group">' +
                    '<input id="input-field-modal" type="text" class="form-control" />' +
                    '</div>',
                showCancelButton: true,
                confirmButtonClass: 'btn btn-success',
                cancelButtonClass: 'btn btn-danger',
                buttonsStyling: false
            }).then(function (result) {
                swal({
                    type: 'success',
                    html: 'You entered: <strong>' +
                        $('#input-field').val() +
                        '</strong>',
                    confirmButtonClass: 'btn btn-success',
                    buttonsStyling: false

                });
            }).catch(swal.noop);
        }
    }
};