dc = {
    onAddServiceClick: function (dentistOfficeId) {
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: '/ServiceManagment/AddService?dentistOfficeId=' + dentistOfficeId,
            success: function (data) {
                $('#dcModalContent').html(data);
                $('#dcModal').modal(options);
                $('#dcModal').modal('show');
                $('.selectpicker').selectpicker();
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    },

    onEditServiceClick: function (buttonClicked, dentistOfficeId) {
        var id = buttonClicked.dataset.id;
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: '/ServiceManagment/EditService',
            contentType: "application/json; charset=utf-8",
            data: {
                "Id": id,
                "DentistOfficeId": dentistOfficeId
            },
            datatype: "json",
            success: function (data) {
                $('#dcModalContent').html(data);
                $('#dcModal').modal(options);
                $('#dcModal').modal('show');
                $('.selectpicker').selectpicker();
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    },

    onRemoveServiceClick: function (buttonClicked, serviceName, dentistOfficeId) {
        var id = buttonClicked.dataset.id;

        swal({
            title: 'Czy napewno chcesz trwale usunąć usługę?',
            html: '<div class="h3">' + serviceName + '</div>',
            showCancelButton: true,
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(function (result) {
            if (result.value === true) {

                $.ajax({
                    type: "POST",
                    url: '/ServiceManagment/RemoveService?serviceId=' + id + '&dentistOfficeId=' + dentistOfficeId,
                    success: function (data) {
                        if (data.status === "success") {
                            swal({
                                type: 'success',
                                html: '<p>Usługa ' + serviceName + ' została usunięta.</p>',
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