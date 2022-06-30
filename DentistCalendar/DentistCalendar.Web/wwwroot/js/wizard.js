dc = {
    onReserveServiceClick: function () {
        var selectedDentistOfficeId = $('#officePicker').val();
        var selecterdServiceId = $("input[name='selectService']:checked").val();
        var selecterdDentistId = $("input[name='selectDentist']:checked").val();

        var date = $('#datePicker').val();
        var selectedServiceTime = $("input[name='selectTime']:checked").val();
        var serviceTime = date + ':' + selectedServiceTime;

        swal({
            title: 'Czy napeno chcesz zarezerwować ten termin?',
            html: '<div class="h3">' + serviceTime + '</div>',
            showCancelButton: true,
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(function (result) {
            if (result.value === true) {
                $.ajax({
                    type: "POST",
                    url: '/Appointment/ScheduleService',
                    data: {
                        "dentistOfficeId": selectedDentistOfficeId,
                        "serviceId": selecterdServiceId,
                        "dentistId": selecterdDentistId,
                        "serviceTime": serviceTime
                    },
                    success: function (data) {
                        if (data.status === "success") {
                            swal({
                                type: 'success',
                                html: '<p>Termin został zarezerwowany.</p>',
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

    initMaterialWizard: function () {
        // Wizard Initialization
        $('.card-wizard').bootstrapWizard({
            'tabClass': 'nav nav-pills',
            'nextSelector': '.btn-next',
            'previousSelector': '.btn-previous',

            onNext: function (tab, navigation, index) {
                if (index === 1) {
                    var cityName = $('#cityPicker').val();

                    if (cityName === '') {
                        return false;
                    }

                    $.ajax({
                        type: "GET",
                        url: '/Appointment/GetAvaliableOffices?cityName=' + cityName,
                        success: function (data) {
                            var mySelect = $('#officePicker');

                            mySelect
                                .find('option')
                                .remove()
                                .end()
                                .append('<option disabled>Wybierz gabinet</option>');

                            $.each(data, function (val, dentistOffice) {
                                mySelect.append(
                                    $('<option></option>').val(dentistOffice.id).html(dentistOffice.name)
                                );
                            });

                            mySelect.selectpicker('refresh');
                        },
                        error: function () {
                            alert("Dynamic content load failed.");
                        }
                    });
                }

                if (index === 2) {
                    var officeId = $('#officePicker').val();

                    if (officeId === '') {
                        return false;
                    }

                    $.ajax({
                        type: "GET",
                        url: '/Appointment/GetAvaliableServices?dentistOfficeId=' + officeId,
                        success: function (data) {
                            var mySelect = $('#servicePicker');

                            mySelect
                                .find('div')
                                .remove()
                                .end();

                            $.each(data, function (val, service) {
                                var controlId = 'serviceControl_' + val;
                                var divBody = '<input type="radio" id="' + controlId + '" name="selectService" value="' + service.id + '"><label for="' + controlId + '"><h2>' + service.name + '</h2><p>Cena: ' + service.price + 'zł' + '</p><p>' + service.description + '</p></label >';
                                mySelect.append(
                                    $('<div class="col-md-5"></div>').html(divBody)
                                );
                            });
                        },
                        error: function () {
                            alert("Dynamic content load failed.");
                        }
                    });
                }

                if (index === 3) {
                    var selecterdServiceId = $("input[name='selectService']:checked").val();
                    if (typeof selecterdServiceId === 'undefined') {
                        return false;
                    }

                    var dentistOfficeId = $('#officePicker').val();

                    $.ajax({
                        type: "GET",
                        url: '/Appointment/GetAvaliableDentists',
                        data: {
                            "serviceId": selecterdServiceId,
                            "dentistOfficeId": dentistOfficeId
                        },
                        success: function (data) {
                            var mySelect = $('#dentistPicker');

                            mySelect
                                .find('div')
                                .remove()
                                .end();

                            if (data.length === 0) {
                                var divBody = '<label><h3>Brak dentystów wykonujących tą usługę.</h3></label>';
                                mySelect.append(
                                    $('<div class="col-md-10"></div>').html(divBody)
                                );
                            } else {
                                $.each(data, function (val, dentist) {
                                    var controlId = 'dentistControl_' + val;
                                    var src = '../../assets/img/placeholder.jpg';
                                    if (dentist.profileImageUrl !== 'noAvatar') {
                                        src = dentist.profileImageUrl;
                                    }
                                    var fullNameWithTittle = dentist.doctorTitle + ' ' + dentist.name + ' ' + dentist.lastName;

                                    var divBody = '<input type="radio" id="' + controlId + '" name="selectDentist" value="' + dentist.id + '"><label for="' + controlId + '" ><div class="thumbnail"><img class="img-circle pull-left" src="' + src + '" alt="Avatar"></div><h3>' + fullNameWithTittle + '</h3></label>';
                                    mySelect.append(
                                        $('<div class="col-md-5"></div>').html(divBody)
                                    );
                                });
                            }
                        },
                        error: function () {
                            alert("Dynamic content load failed.");
                        }
                    });
                }

                if (index === 4) {
                    var selecterdDentistId = $("input[name='selectDentist']:checked").val();
                    if (typeof selecterdDentistId === 'undefined') {
                        return false;
                    }

                    var date = $('#datePicker').val();
                    if (date === '') {
                        return false;
                    }

                    var serviceId = $("input[name='selectService']:checked").val();

                    $.ajax({
                        type: "GET",
                        url: '/Appointment/GetAvaliableServiceHours',
                        data: {
                            "serviceId": serviceId,
                            "dentistId": selecterdDentistId,
                            "dentistOfficeId": dentistOfficeId,
                            "date": date
                        },
                        success: function (data) {
                            var mySelect = $('#timeServicePicker');

                            mySelect
                                .find('div')
                                .remove()
                                .end();

                            $.each(data, function (val, serviceTime) {
                                var controlId = 'timeControl_' + val;
                                
                                var divBody = '<input type="radio" id="' + controlId + '" name="selectTime" value="' + serviceTime + '"><label for="' + controlId + '" ><h3>' + serviceTime + '</h3></label>';
                                mySelect.append(
                                    $('<div class="col-md-2"></div>').html(divBody)
                                );
                            });
                        },
                        error: function () {
                            alert("Dynamic content load failed.");
                        }
                    });
                }
            },

            onInit: function (tab, navigation, index) {
                //check number of tabs and fill the entire row
                var $total = navigation.find('li').length;
                var $wizard = navigation.closest('.card-wizard');

                $first_li = navigation.find('li:first-child a').html();
                $moving_div = $('<div class="moving-tab">' + $first_li + '</div>');
                $('.card-wizard .wizard-navigation').append($moving_div);

                refreshAnimation($wizard, index);

                $('.moving-tab').css('transition', 'transform 0s');

                $.ajax({
                    type: "GET",
                    url: '/Appointment/GetAvaliableCities',
                    success: function (data) {
                        var mySelect = $('#cityPicker');

                        $.each(data, function (val, text) {
                            mySelect.append(
                                $('<option></option>').val(text).html(text)
                            );
                        });

                        mySelect.selectpicker('refresh');
                    },
                    error: function () {
                        alert("Dynamic content load failed.");
                    }
                });

                $('.datetimepicker').datetimepicker({
                    format: 'L',
                    icons: {
                        time: "fa fa-clock-o",
                        date: "fa fa-calendar",
                        up: "fa fa-chevron-up",
                        down: "fa fa-chevron-down",
                        previous: 'fa fa-chevron-left',
                        next: 'fa fa-chevron-right',
                        today: 'fa fa-screenshot',
                        clear: 'fa fa-trash',
                        close: 'fa fa-remove'
                    },
                    locale: 'pl'
                });
            },

            onTabClick: function (tab, navigation, index) {
                var $valid = $('.card-wizard form').valid();

                if (!$valid) {
                    return false;
                } else {
                    return true;
                }
            },

            onTabShow: function (tab, navigation, index) {
                var $total = navigation.find('li').length;
                var $current = index + 1;

                var $wizard = navigation.closest('.card-wizard');

                // If it's the last tab then hide the last button and show the finish instead
                if ($current >= $total) {
                    $($wizard).find('.btn-next').hide();
                    $($wizard).find('.btn-finish').show();
                } else {
                    $($wizard).find('.btn-next').show();
                    $($wizard).find('.btn-finish').hide();
                }

                button_text = navigation.find('li:nth-child(' + $current + ') a').html();

                setTimeout(function () {
                    $('.moving-tab').text(button_text);
                }, 150);

                var checkbox = $('.footer-checkbox');

                if (!index === 0) {
                    $(checkbox).css({
                        'opacity': '0',
                        'visibility': 'hidden',
                        'position': 'absolute'
                    });
                } else {
                    $(checkbox).css({
                        'opacity': '1',
                        'visibility': 'visible'
                    });
                }

                refreshAnimation($wizard, index);
            }
        });

        $('[data-toggle="wizard-radio"]').click(function () {
            wizard = $(this).closest('.card-wizard');
            wizard.find('[data-toggle="wizard-radio"]').removeClass('active');
            $(this).addClass('active');
            $(wizard).find('[type="radio"]').removeAttr('checked');
            $(this).find('[type="radio"]').attr('checked', 'true');
        });

        $('[data-toggle="wizard-checkbox"]').click(function () {
            if ($(this).hasClass('active')) {
                $(this).removeClass('active');
                $(this).find('[type="checkbox"]').removeAttr('checked');
            } else {
                $(this).addClass('active');
                $(this).find('[type="checkbox"]').attr('checked', 'true');
            }
        });

        $('.set-full-height').css('height', 'auto');

        $(window).resize(function () {
            $('.card-wizard').each(function () {
                $wizard = $(this);

                index = $wizard.bootstrapWizard('currentIndex');
                refreshAnimation($wizard, index);

                $('.moving-tab').css({
                    'transition': 'transform 0s'
                });
            });
        });

        function refreshAnimation($wizard, index) {
            $total = $wizard.find('.nav li').length;
            $li_width = 100 / $total;

            total_steps = $wizard.find('.nav li').length;
            move_distance = $wizard.width() / total_steps;
            index_temp = index;
            vertical_level = 0;

            mobile_device = $(document).width() < 600 && $total > 3;

            if (mobile_device) {
                move_distance = $wizard.width() / 2;
                index_temp = index % 2;
                $li_width = 50;
            }

            $wizard.find('.nav li').css('width', $li_width + '%');

            step_width = move_distance;
            move_distance = move_distance * index_temp;

            $current = index + 1;

            if ($current === 1 || (mobile_device === true && (index % 2 === 0))) {
                move_distance -= 8;
            } else if ($current === total_steps || (mobile_device === true && (index % 2 === 1))) {
                move_distance += 8;
            }

            if (mobile_device) {
                vertical_level = parseInt(index / 2);
                vertical_level = vertical_level * 38;
            }

            $wizard.find('.moving-tab').css('width', step_width);
            $('.moving-tab').css({
                'transform': 'translate3d(' + move_distance + 'px, ' + vertical_level + 'px, 0)',
                'transition': 'all 0.5s cubic-bezier(0.29, 1.42, 0.79, 1)'

            });
        }
    }
};