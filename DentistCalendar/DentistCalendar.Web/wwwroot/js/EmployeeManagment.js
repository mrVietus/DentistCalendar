dc = {
    onAddDentistClick: function (dentistOfficeId) {
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: '/EmployeeManagment/InviteDentist?dentistOfficeId=' + dentistOfficeId,
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

    onAddRecepcionistClick: function (dentistOfficeId) {
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: '/EmployeeManagment/AddRecepcionist?dentistOfficeId='+dentistOfficeId,
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

    onRemoveRecepcionistClick: function (buttonClicked, recepcionistName) {
        var id = buttonClicked.dataset.id;

        swal({
            title: 'Czy napewno chcesz trwale usunąć konto pracownika recepcji?',
            html: '<div class="h3">' + recepcionistName + '</div>',
            showCancelButton: true,
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(function (result) {
            if (result.value === true) {

                $.ajax({
                    type: "POST",
                    url: '/EmployeeManagment/RemoveRecepcionist?id=' + id,
                    success: function (data) {
                        if (data.status === "success") {
                            swal({
                                type: 'success',
                                html: '<p>Pracownik recepcji ' + recepcionistName + ' został usunięty.</p>',
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

    onRemoveDentistClick: function (buttonClicked, recepcionistName, dentistOfficeId) {
        var id = buttonClicked.dataset.id;

        swal({
            title: 'Czy napewno chcesz trwale usunąć dentystę?',
            html: '<div class="h3">' + recepcionistName + '</div>',
            showCancelButton: true,
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(function (result) {
            if (result.value === true) {

                $.ajax({
                    type: "POST",
                    url: '/EmployeeManagment/RemoveDentistFromDentistOffice?dentistId=' + id + '&dentistOfficeId=' + dentistOfficeId,
                    success: function (data) {
                        if (data.status === "success") {
                            swal({
                                type: 'success',
                                html: '<p>Dentysta ' + recepcionistName + ' został usunięty.</p>',
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
    }
};