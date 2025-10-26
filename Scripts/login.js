$(document).ready(function () {
    // ====================================
    // 🔹 2. Formulario Login
    // ====================================
    $('#loginForm').on("submit", function () {
        mostrarSpinnerGlobal(true);
    });

    //captura el evento del teclado en el campo de contrasena
    $('#loginForm').on('keypress', function (event) {
        //si la tecla presanada es "Enter" (codigo 13)
        if (event.key === "Enter") {
            event.preventDefault(); //evita  el comportamiento por defecto
            this.submit();
        }
    });
});