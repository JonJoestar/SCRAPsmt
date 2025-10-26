// ====================================
// 🔹 Eliminar Registros
// ====================================
$(document).on('click', '#btnEliminarScrap', function () {
    const btn = $(this);

    Swal.fire({
        title: 'Estas seguro de Eliminar?',
        text: 'Esta accion no se puede deshacer.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Si, eliminar',
        cancelButtonText: 'Cancelar'
    }).then(result => {
        if (result.isConfirmed) {
            mostrarSpinnerEnBoton(btn, true); //desactiva y muestra loader

            const idScrap = $('#Tabla').val(); //obtiene el ID del registro a eliminar

            $.post(window.rutas.eliminarScrap, { id: $('#Tabla').val() }, function (response) {
                mostrarSpinnerEnBoton(btn, false); //reactiva

                if (response.success) {
                    Swal.fire('Eliminado', response.message, 'success');
                    $('#modalEditarScrap').modal('hide'); //cierra modal
                    $('#modalDetalleScrap').modal('hide'); //cierra el modal de visualizacion
                    //Recarga la tabla principal
                    if (typeof window.recargarTablaPrincipal === 'function') {
                        window.recargarTablaPrincipal();
                    } else {
                        location.reload(); //Respaldo lento 
                    }
                } else {
                    Swal.fire('Error', response.message, 'error');
                }
            }).fail(() => {
                mostrarSpinnerEnBoton(btn, false);
                Swal.fire('Error', 'Ocurrio un error al eliminar', 'error')
            });
        }
    });
});