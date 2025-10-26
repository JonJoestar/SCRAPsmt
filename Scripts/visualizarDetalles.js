// ====================================
// 🔹 Funcion global para recargar la vista de Detalles
window.recargarDetallesDelModal = function (idScrap) {
    //Si no se proporciona un ID, se intenta usar el global guardado
    const id = idScrap || window.currentScrapId;

    if (!id) {
        console.error("No se encontro el Id_scrap para recargar los detalles");
        return;
    }

    //Usa la misma llamada AJAX pque ya usa en la funcion 'visualizar-detalles'
    $.get(window.rutas.visualizarDetalle, { idScrap: id }, function (data) {
        $('#contenidoDetalleScrap').html(data); //Carga vista parcial con los nuevos datos
        //console.log(`Contenidp de detalles de Scrap ID ${id} recargado`);
    }).fail(() => {
        console.error("Error al recargar el contenido de los detalles");
    });
};
// ====================================

// ====================================
// 🔹 Visualiza Detalles del Scrap
// ====================================
$(document).on('click', '.visualizar-detalle', function () {
    const idScrap = $(this).data('id'); //Obtiene el ID del registro a visualizar

    window.currentScrapId = idScrap; // linea para que el Id_scrap sea global

    mostrarSpinnerGlobal(true);

    $.get(window.rutas.visualizarDetalle, { idScrap: idScrap }, function (data) {
        $('#contenidoDetalleScrap').html(data); //Carga vista parcial
        $('#modalDetalleScrap').modal({
            backdrop: 'static', //Evita cierre al dar clic fuera del modal
            keyboard: false //Evita cierre al presionar ESC
        }).modal('show');
        mostrarSpinnerGlobal(false);
    }).fail(() => {
        mostrarSpinnerGlobal(false);
        Swal.fire('Error', 'No se pudo cargar los detalles', 'error');
    });
});

//Manejo de exportacion a excel desde el modal de visualizar detalles
$(document).on('click', '#btnExportarDetalles', function () {
    const idScrap = window.currentScrapId;

    if (idScrap) {
        mostrarSpinnerGlobal(true);
        //Nueva llamada AJAX para obtener todos los datos para Excel
        $.get(window.rutas.exportarDatosExcel, { idScrap: idScrap })
            .done(function (response) {
                mostrarSpinnerGlobal(false);
                if (response.success) {
                    if (response.detalles && response.detalles.length > 0) {
                        //Llamamos a la funccion de exportacion con la nueva estructura
                        exportarDatosExcel(response, `Scrap-${idScrap}`);
                    } else {
                        Swal.fire('Atencion', 'No hay registros para exportar', 'warning');
                    }
                } else {
                    Swal.fire('Error', response.message, 'error');
                }
            })
            .fail(() => {
                mostrarSpinnerGlobal(false);
                Swal.fire('Error', 'No se pudo obtener los datos para exportar', 'error');
            });
    } else {
        Swal.fire('Error', 'Nose pudo el Id de Scrap para exportar', 'error');
    }
});
