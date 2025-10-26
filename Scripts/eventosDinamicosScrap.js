// ====================================
// 🔹 NUEVA FUNCIÓN AUXILIAR: Carga el Costo
// ====================================
function cargarCosto(partNumber) {
    if (partNumber) {
        $.ajax({
            type: "POST",
            url: window.rutas.obtenerCosto,
            data: { Partnum: partNumber },
            success: function (data) {
                //formato a 2 decimales
                $('#Costo').val(parseFloat(data.costo).toFixed(2));
            },
            error: function () {
                $('#Costo').val('');
                Swal.fire('Error', 'No se pudo obtener el costo', 'error');
            }
        });
    } else {
        $('#Costo').val('');
    }
}

// ====================================
// 🔹 Eventos dinámicos dentro del modal scrap
// ====================================
$(document).ready(function () {
    //Carga el CFT y el Centro de Costos al Seleccionar Linea
    $(document).on('change', '#Linea', function () {
        var linea = $(this).val();
        if (linea) {
            $.getJSON(window.rutas.obtenerCFTyCcostos, { linea: linea }, function (data) {
                $('#CFT').val(data.cft[0] || ""); //si hay mas de 1, selecciona el primero
                $('#Ccosto').val(data.Cc);
            });
        } else {
            $('#CFT').val('');
            $('#Ccosto').val('');
        }
    });

    //Caraga Numeros de parte de forma dinamica al escribir al menos 5 caracteres
    $(document).on('focus', '#NumParte', function () {
        const $input = $(this);

        //evita reinicializar
        if ($input.data('ui-autocomplete')) return;



        $input.autocomplete({
            appendTo: "#modalFormularioScrap", //Ayuda a que la lista de seleccion no se vaya al fondo
            source: function (request, response) {
                $.ajax({
                    type: 'POST',
                    url: window.rutas.obtenerPartNumbers,
                    data: { filtro: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.Text,
                                value: item.Value
                            };
                        }));
                    }
                });
            },
            minLength: 5,
            select: function (event, ui) {
                $input.val(ui.item.value); // Asigna el valor seleccionado

                cargarCosto(ui.item.value);
                return false;
            }
        });

    });

    //Evento nuevo: se carga el costo si se escanea o se introduce manualmente el numero de parte
    $(document).on('blur', '#NumParte', function () {
        var partNumber = $(this).val();

        //ejecuta la logica solo si tiene un valor escaneado o escrito
        if (partNumber && partNumber.length >= 5) {
            cargarCosto(partNumber);
        }
    });

    //Cargar Defectos al abrir modal
    $(document).on('shown.bs.modal', '#modalFormularioScrap', function () {
        $('#NumParte').focus(); // Enfoca el campo de Numero de Parte al abrir el modal
        $.getJSON(window.rutas.obtenerDefectos, function (data) {
            $('#Defecto').empty().append('<option value="">Seleccione un Defecto</option>');
            $.each(data, (i, item) => {
                $('#Defecto').append(`<option value="${item}">${item}</option>`);
            });
        });
    });
});

