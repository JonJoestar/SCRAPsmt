// ====================================
// 🔹 Script para Abrir el Modal y cargar vista parcial
// ====================================
let tablaGenerarRFA = null;
let esEditable = window.tipoUsuarioEditable ?? false; // Determina si el usuario puede editar RFA

$(document).ready(function () {
    $('#btnGenerarRFA').click(function () {
        $('#modalGenerarRFA').modal({
            backdrop: 'static', //Evita cierre al dar clic fuera del modal
            keyboard: false //Evita cierre al presionar ESC
        }).modal('show');

        setTimeout(() => {
            mostrarSpinnerGlobal(true); // Muestra el spinner global mientras se carga la tabla

            // Si ya está inicializada, destruirla y limpiar contenido
            if ($.fn.DataTable.isDataTable('#tablaGenerarRFA')) {
                tablaGenerarRFA.destroy();
                $('#tablaGenerarRFA').empty(); // Limpia columnas viejas
            }

                let columnasRFA = [
                    { "data": "DateInput", "defaultContent": "" },
                    { "data": "Id_scrap", "defaultContent": "" },
                    { "data": "CFT", "defaultContent": "" },
                    { "data": "TotalScrap", "defaultContent": "" },
                    { "data": "RFA", "defaultContent": "" },
                    {
                        "data": "Id_scrap",
                        "render": function (data, type, row) { //se agreag row como parametro
                            if (esEditable) {
                                //El row.EstadoRFA es la nueva columna que se agrega en la cosulta
                                if (row.RFA === null || row.RFA.trim() === '') {
                                    return `<button type="button" class="btn btn-sm btn-primary editar-rfa" data-id="${data}">Generar RFA</button>`;
                                } else {
                                    return `<button type="button" class="btn btn-sm btn-info editar-rfa" data-id="${data}">Editar Status</button>`;
                                }
                            }
                            return '';
                        },
                        "orderable": false //evita que esta columna se pueda ordenar
                    }
                ];

                tablaGenerarRFA = $('#tablaGenerarRFA').DataTable({
                    "ajax": {
                        "url": window.rutas.obtenerScrapPendienteRFA,
                        "type": "GET",
                        "dataSrc": "data"
                    },
                    columns: columnasRFA,
                    paging: true, // Habilitar paginación
                    pageLength: 10, // Mostrar 10 filas por página
                    language: {
                        url: '/ScrapSmtWeb/Scripts/DataTables/Spanish.json' // Traducir al español
                    },
                    initComplete: function () {
                        mostrarSpinnerGlobal(false);
                    }
                });
        }, 300); //Espera medio segundo a que el modal este en el DOM
    });
});
