//Script utilitario para mostrar u ocultar el spinner
function mostrarSpinnerGlobal(mostrar) {
    if (mostrar) {
        $('#spinnerGlobal').removeClass('d-none');
    } else {
        $('#spinnerGlobal').addClass('d-none')
    }
}
//Funcion de activacion de loader en boton
function mostrarSpinnerEnBoton(boton, activar) {
    if (activar) {
        boton.data('texto-original', boton.html()); // Guarda texto
        boton.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Procesando...');
        boton.prop('disabled', true);
    } else {
        boton.html(boton.data('texto-original'));
        boton.prop('disabled', false);
    }
}