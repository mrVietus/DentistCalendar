dc = {
    onEditClick: function (buttonClicked) {
        var id = buttonClicked.dataset.id;
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: '/LicenseOwner/EditDentistOffice',
            contentType: "application/json; charset=utf-8",
            data: { "Id": id },
            datatype: "json",
            success: function (data) {
                $('#dcModalContent').html(data);
                $('#dcModal').modal(options);
                $('#dcModal').modal('show');

            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    },

    onAddClick: function () {
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: '/LicenseOwner/AddDentistOffice',
            success: function (data) {
                $('#dcModalContent').html(data);
                $('#dcModal').modal(options);
                $('#dcModal').modal('show');
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    },

    onRemoveClick: function (buttonClicked, dentistOfficeName) {
        var id = buttonClicked.dataset.id;

        swal({
            title: 'Czy napewno chcesz trwale usunąć gabinet?',
            html: '<div class="h3">' + dentistOfficeName + '</div>',
            showCancelButton: true,
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(function (result) {
            if (result.value === true) {

                $.ajax({
                    type: "POST",
                    url: '/LicenseOwner/RemoveDentistOffice?id='+id,
                    success: function (data) {
                        if (data.status === "success") {
                            swal({
                                type: 'success',
                                html: '<p>Gabinet ' + dentistOfficeName + ' został usunięty </p>',
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
    },

    onCloseClick: function () {
        $('#dcModal').modal('hide');
    }
};