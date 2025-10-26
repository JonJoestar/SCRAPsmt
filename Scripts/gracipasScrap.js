// ====================================
// 🔹 Script para Graficas y filtros
// ====================================
let chartCantidad = null;
let chartCosto = null;

//funcion para renderizar las graficas con datos recibidos
function renderGraficos(lineas, cantidades, costos) {
    if (chartCantidad) chartCantidad.destroy();
    if (chartCosto) chartCosto.destroy();

    //Grafico de costos
    chartCosto = new Chart(document.getElementById('graficoCosto'), {
        type: 'bar',
        data: {
            labels: lineas,
            datasets: [{
                label: 'Costo Total ($)',
                data: costos,
                backgroundColor: 'rgb(54, 162, 235, 0.6)'
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Costo Total por Linea'
                }
            }
        }
    });

    //Grafico de Cantidades total
    chartCantidad = new Chart(document.getElementById('graficoCantidad'), {
        type: 'bar',
        data: {
            labels: lineas,
            datasets: [{
                label: 'Cantidad de Scrap',
                data: cantidades,
                backgroundColor: 'rgba(255, 99, 132, 0.6)'
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Cantidad de Scrap por Linea'
                },
                legend: {
                    display: false
                }
            }
        }
    });
}

//funcion para cargar los datos sin filtros (inicio)
function cargarGraficas() {
    $.get(window.rutas.obtenerResumenScrap, function (data) {
        let lineas = data.map(d => d.Linea);
        let cantidades = data.map(d => d.Cantidad);
        let costos = data.map(d => d.Costo.toFixed(2));

        renderGraficos(lineas, cantidades, costos);
    });
}

//funcion para cargar con filtros de fecha
function cargarGraficos() {
    var fechaInicio = $('#fechaInicio').val();
    var fechaFin = $('#fechaFin').val();

    $.ajax({
        type: 'POST',
        url: window.rutas.obtenerDatosGrafico,
        data: {
            fechaInicio: fechaInicio,
            fechaFin: fechaFin
        },
        success: function (data) {
            if (data.success) {
                if (data.lineas.length === 0) {
                    Swal.fire({
                        icon: 'info',
                        title: 'Sin resultados',
                        text: 'No se encontraron datos en el rango de fecha'
                    });
                } else {
                    renderGraficos(data.lineas, data.cantidades, data.costos);
                }
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: data.message
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Error al obtener las graficas'
            });
        }
    });
}

//ejecuta al cargar la pagina
$(document).ready(function () {
    cargarGraficas(); //carga inicial

    $('#btnFiltrarGrafico').click(function () {
        cargarGraficos(); //filtro por fechas
    });

    //Boton para restablecer filtros
    $('#btnResetGraficos').click(function () {
        $('#fechaInicio').val('');
        $('#fechaFin').val('');
        cargarGraficas(); //Carga general sin filtros
    
        //Mostrar el toast
        const toast = new bootstrap.Toast(document.getElementById('toastGraficos'));
        toast.show();
    });
});