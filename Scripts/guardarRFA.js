// ====================================
// 🔹 Guardar RFA
// ====================================
$(document).on('click', '#btnGuardarRFA', function () { //Al hacer click en Guardar se mandan los datos al servidor

    //antes de enviar
    const btn = $('#this');
    mostrarSpinnerEnBoton(btn, true);

    let datos = {
        Id_scrap: $('#Id_scrap').val(),
        RFA: $('#RFA').val(),
        Status: $('#Status').val()
    };

    //Valida que no se envien datos vacios
    if (datos.Status === "OPEN" && !datos.RFA) {
        Swal.fire("Advertencia", "Debes ingresar un RFA para guardar el registro", "warning");
        mostrarSpinnerEnBoton(btn, false);
        return;
    }

    $.ajax({
        type: "POST",
        url: window.rutas.guardarRFA,
        data: datos,
        success: function (response) {
            mostrarSpinnerEnBoton(btn, false);

            if (response.success) {
                Swal.fire('Exito', response.message, 'success'); //si salio bien muestra mensaje de exito
                $('#modalAsignarRFA').modal('hide');    //Cierra el modal

                //Recarga la tabla para ver cambios
                if (tablaGenerarRFA) {
                    tablaGenerarRFA.ajax.reload(null, false);
                }
                if (tablaRFA) {
                    tablaRFA.ajax.reload(null, false);
                }

            } else {
                Swal.fire('Advertencia', response.message, 'warning');
            }
        },
        error: function (xhr, status, error) {
            mostrarSpinnerEnBoton(btn, false);
            Swal.fire('Error', 'Error al guardar el RFA', 'error');    //Si hubo error muestra mensaje de error al usuario
        }
    });
});