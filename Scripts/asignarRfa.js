// ====================================
// 🔹 Script para abrir el Modal de asignacion de RFA
// ====================================
$(document).on('click', '.editar-rfa', function () { //Al hacer click en el boton Editar RFA, hace peticion a AJAX
    var id = $(this).data('id');
    //Antes de cargar el nuevo contenido, limpia el contenedor del modale anterior
    $('#contenedorModalRFA').empty();

    $.get(window.rutas.obtenerRFA, { id: id }, function (html) {
        $('#contenedorModalRFA').html(html);    //Carga la vista parcial dentro del modal
        $('#modalAsignarRFA').modal({
            backdrop: 'static', //Evita cierre al dar clic fuera del modal
            keyboard: false //Evita cierre al presionar ESC
        }).modal('show');
    });
});