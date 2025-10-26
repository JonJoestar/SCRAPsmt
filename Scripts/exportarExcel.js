//$(document).ready(function () {
//    // ====================================
//    // 🔹 6. Exportar a Excel
//    // ====================================
//    $('#btnExportarExcel').click(function () {
//        const datosFormulario = {
//            Linea: $('#Linea').val(),
//            CFT: $('#CFT').val(),
//            CentroCosto: $('#Ccosto').val(),
//            Turno: $('#Turno').val(),
//            NumeroParte: $('#NumParte').val(),
//            CostoUnitario: $('#Costo').val(),
//            Cantidad: $('input[name="Cantidad"]').val(),
//            Defecto: $('#Defecto').val(),
//            Batch: $('input[name="Batch"]').val(),
//            Reference: $('#Reference').val(),
//            HU: $('#HU').val(),
//            Tipo: $('#Tipo').val(),
//            Categoria: $('#Categoria').val(),
//            ValidacionIngenieria: $('#Ing_validacion').val(),
//            Equipo: $('#Equipment').val()

//        };

//        //Crea una hoja con encabezado en primera fila y datos en la segunda
//        const headers = Object.keys(datosFormulario);
//        const values = Object.values(datosFormulario);
//        //crea libro y hoja
//        const ws = XLSX.utils.aoa_to_sheet([headers, values]); //primera fila = headers, segunda fila = datos
//        const wb = XLSX.utils.book_new();
//        XLSX.utils.book_append_sheet(wb, ws, "Formulario Scrap");

//        //Autoajusta el ancho de las columnas
//        ws['!cols'] = headers.map(h => ({ wch: Math.max(h.length, 15) })); //minimo 15 caracteres de ancho
//        XLSX.writeFile(wb, "ScrapFormulario.xlsx");  //Descargar el archivo excel
//    });
//});

/** 
* @param {object} data //arreglo de objetos donde cada objeto es una fila
* @param  {string} fileName //nombre del archivo de excel
*/
function exportarDatosExcel(data, fileName) {
    if (!data || (!data.detalles && !data.acciones)) {
        console.error("No hay datos para exportar");
        return;
    }

    //Funcion para el formato de fecha
    const parseAspNetDate = (dateString) => {
        if (!dateString || typeof dateString !== 'string' || dateString.indexOf('/Date(') !== 0) {
            return dateString;
        }
        const timestamp = parseInt(dateString.replace(/\/Date\((\d+)\)\//, '$1'), 10);
        return new Date(timestamp);
    }


    const transformarAcciones = data.acciones.map(item => {
        return {
            ...item,
            Cuando: parseAspNetDate(item.Cuando),
        };
    });

    //convierte el arreglo de objetos a una hoja de excel
    const wb = XLSX.utils.book_new();

    //Hoja1: Detalles Scrap
    if (data.detalles && data.detalles.length > 0) {
        const wsDetalles = XLSX.utils.json_to_sheet(data.detalles);
        const colWidthsDetalles = Object.keys(data.detalles[0]).map(key => ({ wch: Math.max(key.length, 15) }));
        wsDetalles['!cols'] = colWidthsDetalles;
        XLSX.utils.book_append_sheet(wb, wsDetalles, "DetallesScrap");
    }

    //Hoja 2: Acciones
    if (transformarAcciones && transformarAcciones.length > 0) {
        const wsAcciones = XLSX.utils.json_to_sheet(transformarAcciones);
        const colWidthsAcciones = Object.keys(transformarAcciones[0]).map(key => ({ wch: Math.max(key.length, 15) }));
        wsAcciones['!cols'] = colWidthsAcciones;
        XLSX.utils.book_append_sheet(wb, wsAcciones, "Acciones");
    }

    //descarga el archivo 
    XLSX.writeFile(wb, `${fileName}.xlsx`);
}