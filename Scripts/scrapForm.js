function inicializarFormularioScrap() {

    //Abrir y cerrar el modal
    $('#btnAbrirModal').click(function () {
        mostrarSpinnerGlobal(true); //Spinner visible

        $.get(window.rutas.obtenerFormularioScrap, function (data) {
            $('#contenedorFormulario').html(data);
            mostrarSpinnerGlobal(false); //Quita spinner
            $('#modalFormularioScrap').modal({
                backdrop: 'static', //Evita cierre al dar clic fuera del modal
                keyboard: false //Evita cierre al presionar ESC
            }).modal('show');

            //coulta el boton de cierre de la x
            $('#modalFormularioScrap .modal-header .btn-close').hide();
        })
            .fail(function () {
                mostrarSpinnerGlobal(false);
                Swal.fire('Error', 'No se pudo cargar el formulario', 'error');
            });
    });

    //Finalizar Captura
    $(document).on('click', '#btnFinalizarCaptura', function () {
        const btn = $(this);
        mostrarSpinnerEnBoton(btn, true)

        $.post(window.rutas.finalizarCaptura, function (response) {
            mostrarSpinnerGlobal(false);
            mostrarSpinnerEnBoton(btn, false);
            if (response.success) {
                $('#modalFormularioScrap').modal('hide');
            }
        });
    });

    //Enviar formulario de scrap
    $(document).on('submit', '#formScrap', function (e) {
        e.preventDefault(); //Previene el submit normal

        const btn = $('#btnGuardarScrap');
        //Validacion general
        let camposVacios = [];

        //Validar campos requeridos
        let camposRequeridos = [
            { id: '#Linea', nombre: 'Linea' },
            { id: '#Shift', nombre: 'Turno' },
            { id: '#NumParte', nombre: 'Numero de Parte' },
            { id: 'input[name="Qty"]', nombre: 'Cantidad' },
            { id: '#Defecto', nombre: 'Defecto' },
            { id: '#Referencia', nombre: 'Referencia' },
            { id: '#Tipo', nombre: 'Tipo' },
            { id: '#Categoria', nombre: 'Categoria' },
            { id: '#ValidacionING', nombre: 'Validacion de Ingenieria' },
            { id: '#Equipment', nombre: 'Equipo' }
            //{ id: '#HU', nombre: 'HU' }
        ];

        //Verifica si Tipo esta vacio
        let tipoScrap = $('#Tipo').val();

        //Si no es PCB Virgen -> agregamos HU y Batch
        if (tipoScrap === "PCB Virgen") {
        } else {
            //Si Tipo no es PCB Virgen, agrega HU y Batch como requeridos
            camposRequeridos.push({ id: '#Batch', nombre: 'Batch' });
        }

        //Validacion generarl de todos los campos requeridos
        camposRequeridos.forEach(function (campo) {
            let valor = $(campo.id).val();
            //Ademas de verifivar si esta vacio, se debe verificar si el campo es visible y esta habilitado
            if (!valor || valor.trim() === "") {
                camposVacios.push(campo.nombre);
            }
        });

        if (camposVacios.length > 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Campos requeridos faltantes',
                html: 'Por favor completa los siguientes campos:<br><b>' + camposVacios.join(', ') + '</b>'
            });
            let primerCampoVacioId = camposRequeridos.find(c => !$(c.id).val() || $(c.id).val().trim() === "");
            if (primerCampoVacioId) {
                $(primerCampoVacioId).focus(); //Enfoca el primer campo vacio
            }
            return; //Detiene la ejecucion si faltan campos
        }

        mostrarSpinnerEnBoton(btn, true);
        mostrarSpinnerGlobal(true);

        $.ajax({
            type: "POST",
            url: window.rutas.guardarScrap,
            data: $(this).serialize(),
            success: function (response) {
                mostrarSpinnerGlobal(false);
                mostrarSpinnerEnBoton(btn, false);

                if (response.success) {     //Muestra mensaje
                    //Muestra el Folio y actualiza la interfaz
                    if (response.idScrap) {
                        $('#folioScrapActivo').text(response.idScrap);
                        $('#contenedorFolio').show();
                    }
                    //por si no se muestra el Id_scrap, pero si se guardo
                    Swal.fire({
                        icon: 'success',
                        title: 'Guardado',
                        text: response.message,
                        timer: 2000,
                        showConfirmButton: false
                    });
                    $('#formScrap')[0].reset(); //Limpia formulario
                    $('#NumParte').empty().append('<option value="">Seleccione una opcion</option>'); //fuerza cierre de eventos dinamicos
                    $('#Costo').val('');

                    //Recarga la tabla principal
                    if (window.tablaRFA && typeof window.tablaRFA.ajax !== "undefined") {
                        window.tablaRFA.ajax.reload(null, false)
                    }
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message
                    });
                }
            },
            error: function (xhr, status, error) {
                mostrarSpinnerGlobal(false);
                mostrarSpinnerEnBoton(btn, false);
                Swal.fire('Error', 'Error al guardar el scrap', 'error');
            }
        });
    });
}

//Nuevo evento para Agregar al Folio dentro del modal de detalles
$(document).on('click', '#btnAgregarAlFolioDetalle', function () {
    const btn = $(this);
    //Capturamos el Id_scrap, se usara parseInt para asegurar que sea un numero
    const idScrap = parseInt(btn.data('id-scrap'));

    //validamos que sa u numero valido y mayor a 0
    if (isNaN(idScrap) || idScrap <= 0) {
        Swal.fire('Error', 'No se pudo obtener un Folio valido (numerico) para reabrir. El registro de detalle podria estar vacio', 'error');
        return;
    }

    mostrarSpinnerGlobal(true);

    //Notifica al servidor que folio se va a utilizar
    $.ajax({
        type: "POST",
        url: window.rutas.seleccionarFolio,
        data: { idScrap: idScrap },
        success: function (response) {
            if (response.success) { //Abre el modal de captura (GET Create)
                $.get(window.rutas.obtenerFormularioScrap)
                    .done(function (data) {
                        $('#contenedorFormulario').html(data);

                        //asegurar que el Folio se muestre inmediatamente en el formulario
                        $('#folioScrapActivo').text(idScrap);
                        $('#contenedorFolio').show();

                        mostrarSpinnerGlobal(false);

                        $('#modalFormularioScrap').modal({
                            backdrop: 'static',
                            keyboard: false
                        }).modal('show');

                        $('#modalFormularioScrap .modal-header .btn-close').hide();

                        Swal.fire('Folio Abierto', `El Folio **${idScrap}** esta activo. Los nuevos registros se agregaran a este folio`, 'info');
                    })
                    .fail(function () {
                        mostrarSpinnerGlobal(false);
                        Swal.fire('Error', 'No se pudo cargar el formulario de captura', 'error');
                    });
            } else {
                mostrarSpinnerGlobal(false);
                Swal.fire('Error', response.message, 'error');
            }
        },
        error: function () {
            mostrarSpinnerGlobal(false);
            Swal.fire('Error', 'Error de comunicacion al reabrir el folio', 'error');
        }
    });
});


//logica para mostrar u ocultar campos dependiendo del tipo de scrap seleccionado
function actualizarVisibilidadCampos() {
    //si el tipo es "PCB Virgen", oculta los campos Batch y HU
    const tipo = $('#Tipo').val();

    if (tipo === "PCB Virgen") {
        //deshabilita los campos y los limpia
        $('#Batch').prop('disabled', true).val('');

        //Quita el requerido
        $('#Batch').prop('required', false);
    } else {
        //habilita los campos
        $('#Batch').prop('disabled', false);

        //vuelve a agregrar requerido
        $('#Batch').prop('required', true);
    }
}

//Detecta cambios en el select de Tipo
$(document).on('change', '#Tipo', actualizarVisibilidadCampos);

//Al cargar el formulario por primera vez, actualiza visibilidad
$(document).ready(function () {
    actualizarVisibilidadCampos();
});