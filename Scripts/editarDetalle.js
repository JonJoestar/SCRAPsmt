// ====================================
// 🔹 Editar Scrap - Abrir Modal
// ====================================
$(document).on('click', '.btnEditarScrap', function () {
    const idScrap = $(this).data('id'); //Obtiene el ID del registro a editar
    mostrarSpinnerGlobal(true);

    $.get(window.rutas.editarDetalle, { id: $(this).data('id') }, function (html) {
        mostrarSpinnerGlobal(false);

        $('#contenedorEdicionScrap').html(html);

        $('#modalEditarScrap').modal({
            backdrop: 'static', //Evita cierre al dar clic fuera del modal
            keyboard: false //Evita cierre al presionar ESC
        }).modal('show');

    }).fail(() => {
        mostrarSpinnerGlobal(false);
        Swal.fire('error', 'No se pudo cargar el formulario de edicion', 'error');
    });
});

// ====================================
// 🔹 Guarda los cambios del formulario de editar
// ====================================
$(document).on('submit', '#formEditarScrap', function (e) {
    e.preventDefault();

    const form = $(this);
    const btn = $('#btnGuardarEdicion');

    //Spinner en boton
    mostrarSpinnerGlobal(true);
    mostrarSpinnerEnBoton(btn, true);

    //Si hubo cambios, ahora si hace el guardado AJAX
    $.post(window.rutas.guardarCambiosScrap, $(this).serialize(), function (response) {
        //se oculta despues del exito
        mostrarSpinnerGlobal(false);
        mostrarSpinnerEnBoton(btn, false);

        if (response.success) {
            Swal.fire({
                icon: 'success',
                title: 'Actualizado',
                text: response.message
            });
            $('#modalEditarScrap').modal('hide');

            // Recarga la tabla principal del Index (la agrupada)
            if (typeof window.recargarTablaPrincipal === 'function') {
                window.recargarTablaPrincipal();
            }

            //Recarga el contenido HTML del modal de detalles
            if (typeof window.recargarDetallesDelModal === 'function') {
                //Usa el ID guardado globalmente por visualisarDetalles.js
                window.recargarDetallesDelModal();
            }
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: response.message
            });
        }
    }).fail(() => {
        mostrarSpinnerGlobal(false);
        mostrarSpinnerEnBoton(btn, false);
        Swal.fire('Error', 'No se pudo procesar la solicitud', error)
    });
});