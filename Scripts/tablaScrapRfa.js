function inicializarTablaRFA() {
    if (!$.fn.DataTable.isDataTable('#tablaRFA')) {
        // ====================================
        // 🔹 1. DataTable principal Scrap RFA
        // ====================================
        window.tablaRFA = $('#tablaRFA').DataTable({
            "ajax": {
                "url": window.rutas.obtenerDatosRFA,
                "type": "GET",
                "datatype": "json"
            },
            "columns": [
                { "data": "DateInput" },
                { "data": "Id_scrap" },
                { "data": "CFT" },
                { "data": "TotalScrap" },
                { "data": "RFA" },
                {
                    "data": "Id_scrap",
                    "render": function (data) {
                        return `<button type="button" class="btn btn-sm btn-info visualizar-detalle" data-id="${data}" title="Ver los detalles y registros individuales del Folio ${data}">Visualizar</button>`;
                    },
                    "orderable": false //evita que esta columna se pueda ordenar
                }
            ],
            "paging": true, // Habilitar paginación
            "pageLength": 10, // Mostrar 10 filas por página
            "language": {
                "url": '/ScrapSmtWeb/Scripts/DataTables/Spanish.json' // Traducir al español
            }
        });

        //Declaracion de la funcion global ara la recarga de la tabla
        window.recargarTablaPrincipal = function () {
            if (window.tablaRFA && typeof window.tablaRFA.ajax !== "undefined") {
                //El null, false evita que la paginacipon se reinicie
                window.tablaRFA.ajax.reload(null, false);
                //console.log("Tabla RFA recargada");
            } else {
                console.error("Error: window.tablaFRA no esta disponible para recargar");
            }
        }
    }
}