$(document).ready(function () {
    inicializarFormularioScrap(); //Llama la funcion declarada en scrapForm.js
    inicializarTablaRFA(); //Llama la funcion declarada en tablaScrapRfa.js

    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(tooltipEl => {
        new bootstrap.Tooltip(tooltipEl);
    });
});