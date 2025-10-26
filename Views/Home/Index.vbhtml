@Code
    ViewData("Title") = "SCRAP SMT"
End Code

<!--Estilos css-->
<nav class="navbar navbar-expand-lg navbar-ligh bg-light">
    <div class="container">
        <!--se agrega contenedor para margen a los lados-->
        <!--campo contrasena y boton de salida-->
        <div class="d-flex align-items-center">
            @Using Html.BeginForm("Login", "Home", FormMethod.Post, New With {.id = "loginForm", .class = "d-flex"})
                @Html.TextBox("empleadoNUM", "", New Dictionary(Of String, Object) From {
                                                         {"placeholder", "Numero de Empleado"},
                                                         {"class", "form-control me-2"},
                                                         {"id", "empleadoNUMInput"},
                                                         {"maxlength", "5"}
                                                   })
            End Using
            <button class="btn btn-outline-danger ms-2" onclick="location.href='@Url.Action("Logout", "Home")'">Salir</button>
        </div>

        <!--Mensaje de bienvenida-->
        @If Session("UsuarioAutenticado") IsNot Nothing Then
            @<div class="navbar-text ms-3">
                Bienvenido, <strong>@Session("UsuarioAutenticado").ToString()</strong>!
            </div>
        End If

        <div class="navbar-nav ms-auto">
            <!--ms-auto para alinear a la derecha-->
            @If Session("UsuarioAutenticado") IsNot Nothing Then
                @Code
                    Dim tipoUsuario = Session("Tipo")
                    Dim esDataClerk = tipoUsuario = "DataClerk"
                    Dim atributoDeshabilitado = If(esDataClerk, "btn-secondary", "btn-primary")
                    Dim titleScrap = If(esDataClerk, "Funcion deshabilitada para tu rol", "Iniciar la captura de un nuevo Scrap")
                End Code

                @If esDataClerk Then
                    'Caso deshabilitado: envolvemos en un span con el title
                    @<span class="d-lg-inline-block me-2" tabindex="0" data-bs-toggle="tooltip" title="@titleScrap">
                        <button class="btn @atributoDeshabilitado" id="btnAbrirModal" disabled="disabled">
                            Agregar Scrap
                        </button>
                     </span>
                Else
                    'Caso habilitado: Title va directamente en el boton
                    @<button class="btn @atributoDeshabilitado me-2"
                             id="btnAbrirModal"
                             title="@titleScrap">
                           Agregar Scrap
                     </button>
                End If


                    'Modal
                @<div class="modal fade" id="modalFormularioScrap" tabindex="-1" aria-hidden="true">
                    <div class="modal-dialog modal-xl">
                        <div class="modal-content fondo-azul-claro shadow">
                            <div class="modal-header fondo-azul-cielo text-black">
                                <h5 class="modal-title text-uppercase fw-bold">Registro de Nuevo Scrap</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                            </div>
                            <div class="modal-body" id="contenedorFormulario">

                            </div>
                        </div>
                    </div>
                </div>
                'Boton Generar RFA, visible solo para rol Data Clerk
                If tipoUsuario = "DataClerk" OrElse tipoUsuario = "ADMIN" Then
                    @<button id="btnGenerarRFA" class="btn btn-primary me-2" title="Asigna un FRA a los folios de Scrap pendientes">Generar RFA</button>
                Else
                    'Caso deshabilitado: envolvemos en un Span con el title
                    @<span class="d-inline-block me-2" tabindex="0" data-bs-toggle="tooltip" title="Funcion deshabilitada para tu rol">
                        <button id="btnGenerarRFA" class="btn btn-secondary me-2" disabled="disabled">Generar RFA</button>
                     </span>
                    End If

                Else

                        'Si el usuario no esta autenticado, se muestran los botones deshabilitados
                    @<span class="d-inline-block me-2" tabindex="0" data-bs-toggle="tooltip" title="Debes iniciar sesion para usar esta funcion">
                        <button id="btnAgregarScrap" Class="btn btn-secondary me-2" disabled="disabled">Agregar Scrap</button>
                    </span>
                    @<span class="d-inline-block" tabindex="0" data-bs-toggle="tooltip" title="Debes iniciar sesion para usar esta funcion">
                       <button id="btnGenerarRFA" Class="btn btn-secondary me-2" disabled="disabled">Generar RFA</button>
                     </span>
             End If
        </div>
    </div>
</nav>

<!-- Mensajes de error -->
@If ViewBag.ErrorMessage IsNot Nothing Then
    @<div class="alert alert-danger mt-3 container">
        @ViewBag.ErrorMessage
    </div>
End If

<!--Tabal de datos-->
<div class="container mt-4">
    <table id="tablaRFA" class="table table-bordered table-striped">
        <thead>
            <tr>
                <th>Fecha Captura</th>
                <th>Folio Scrap</th>
                <th>CFT</th>
                <th>Costo Total</th>
                <th>RFA</th>
                <th>Acciones</th>
            </tr>
        </thead>
    </table>
</div>

<!--Modal para RFA-->
<div class="modal fade" id="modalGenerarRFA" tabindex="-1" aria-labelledby="modalGenerarRFALabel" aria-hidden="true">
    <div class="modal-dialog modal-xl">
        <div class="modal-content fondo-azul-claro shadow">
            <div class="modal-header fondo-azul-cielo text-black">
                <h5 class="modal-title text-uppercase fw-bold" id="modalGenerarRFALabel">Folios de Scrap Pendientes de RFA</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
            </div>
            <div class="modal-body">
                <table id="tablaGenerarRFA" class="table table-bordered table-hover">
                    <thead>
                        <tr>
                            <th>Fecha Captura</th>
                            <th>Folio Scrap</th>
                            <th>CFT</th>
                            <th>Costo Total</th>
                            <th>RFA</th>
                            <th>Editar</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
    </div>
</div>

<!--Graficas-->
<div class="container mt-5">
    <h4 class="mb-3">Resumen de Scrap por Linea</h4>

    <div class="row mb-3">
        <div class="col-md-3">
            <label for="fechaInicio" class="form-label">Desde</label>
            <input type="date" id="fechaInicio" class="form-control" />
        </div>
        <div class="col-md-3">
            <label for="fechaFin" class="form-label">Hasta</label>
            <input type="date" id="fechaFin" class="form-control" />
        </div>
        <div class="col-md-3 d-flex align-items-end">
            <button id="btnFiltrarGrafico" class="btn btn-primary w-100">Filtrar</button>
            <button id="btnResetGraficos" class="btn btn-secondary w-100">Restablecer</button>
        </div>
        <div class="toast-container position-fixed bottom-0 end-0 p-3">
            <div id="toastGraficos" class="toast align-items-center text-white bg-primary border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        Filtros restablecidos correctamente.
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-md-6">
            <canvas id="graficoCosto" height="300"></canvas>
        </div>
        <div class="col-md-6">
            <canvas id="graficoCantidad" height="300"></canvas>
        </div>
    </div>
</div>

<!--Modal para ver detalles-->
<div class="modal fade" id="modalDetalleScrap" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl">
        <div class="modal-content fondo-azul-claro shadow">
            <div class="modal-header fondo-azul-cielo text-black">
                <h5 class="modal-title text-uppercase fw-bold" id="tituloModalDetalleScrap">Detalles y Acciones del Folio</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
            </div>
            <div class="modal-body">
                <div class="table-responsive" style="max-height: 60vh; overflow-y: auto;">
                    <div id="contenidoDetalleScrap">
                        <!--Se carga contenido aqui-->
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<!--Modal para edicion-->
<div class="modal fade" id="modalEditarScrap" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl">
        <div class="modal-content fondo-azul-claro shadow">
            <div class="modal-header fondo-azul-cielo text-black">
                <h5 class="modal-title text-uppercase fw-bold" id="tituloModalEditarScrap">CAPTURA DE ACCIONES CORRECTIVAS, CAUSA RAIZ Y LA CONTENCION DEL EVENTO</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="table-responsive" style="max-height: 60vh; overflow-y: auto;">
                    <div id="contenedorEdicionScrap">
                        <!--Se carga contenido aqui-->
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<!--Modal para Asigna RFA-->
<div class="modal fade" id="modalAsignarRFA" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-xl">
        <div class="modal-content fondo-azul-claro shadow">
            <div class="modal-header fondo-azul-cielo text-black">
                <h5 class="modal-title text-uppercase fw-bold">Asignacion de RFA</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
            </div>
            <div class="modal-body" id="contenedorModalRFA">
                <!--Vista parcial cargada aqui-->

            </div>
        </div>
    </div>
</div>



@section scripts
    <script>
    window.rutas = {
        obtenerDatosRFA: '@Url.Action("ObtenerDatosRFA", "Home")', //Get
        seleccionarFolio: '@Url.Action("SeleccionarFolio", "AgregarScrap")',
        obtenerFormularioScrap: '@Url.Action("Create", "AgregarScrap")', //Get
        guardarScrap: '@Url.Action("Create", "AgregarScrap")', //POST
        finalizarCaptura: '@Url.Action("FinalizarCaptura", "AgregarScrap")', //POST
        obtenerCFTyCcostos: '@Url.Action("ObtenerCFTyCcostos", "AgregarScrap")', //Get
        obtenerPartNumbers: '@Url.Action("ObtenerPartNumber", "AgregarScrap")', //Get
        obtenerDefectos: '@Url.Action("ObtenerDefectos", "AgregarScrap")', //Get
        obtenerCosto: '@Url.Action("ObtenerCosto", "AgregarScrap")', //Get
        guardarCambiosScrap: '@Url.Action("GuardarCambiosScrap", "AgregarScrap")', //POST
        eliminarScrap: '@Url.Action("EliminarScrap", "AgregarScrap")', //POST
        visualizarDetalle: '@Url.Action("VisualizarDetalle", "AgregarScrap")', //Get
        obtenerResumenScrap: '@Url.Action("ObtenerResumenScrap", "Home")', //Get
        obtenerDatosGrafico: '@Url.Action("ObtenerDatosGrafico", "Home")', //Get
        obtenerScrapPendienteRFA: '@Url.Action("ObtenerScrapPendienteRFA", "Home")', //Get
        obtenerRFA: '@Url.Action("ObtenerRFA", "Home")', //Get
        guardarRFA: '@Url.Action("GuardarRFA", "Home")', //POST
        editarDetalle: '@Url.Action("EditarDetalle", "AgregarScrap")', //GET
        obtenerNotas: '@Url.Action("GetNotes", "PatchNotes")', //GET
        exportarDatosExcel: '@Url.Action("ExportarDetallesExcel", "Home")' //GET

    };
    </script>
    <!--Varibale compartida para JS-->
    <script>
        @Code

            'Cambiar la version de la aplicacion al hacer cabios importantes a scripts
            Dim AppVersion As String = "1.0.1"

            'Obtiene el rol en mayusculas solo una vez
            Dim tipoUsuarioUpper As String = If(Session("Tipo") IsNot Nothing, Session("Tipo").ToString().ToUpper(), "")

            'Define la logica de edicion
            'se usa OrElse para seguridad y se compara contra caedena en mayusculas
            Dim esEditable As Boolean = (tipoUsuarioUpper = "SUPERVISOR" OrElse tipoUsuarioUpper = "TECNICO" OrElse tipoUsuarioUpper = "DATACLERK" OrElse tipoUsuarioUpper = "ADMIN")

        End Code

        window.tipoUsuarioEditable = @(esEditable.ToString().ToLower());
    </script>

    <script src="~/Scripts/DataTables/datatables.min.js?v=@AppVersion"></script>
    <script src="~/Scripts/chart.min.js?v=@AppVersion"></script>
    <script src="~/Scripts/xlsx.full.min.js?v=@AppVersion"></script>

    <!--Funciones comunes / utilitarias-->
    <script src="~/Scripts/spinners.js?v=@AppVersion"></script>
    <script src="~/Scripts/app.js?v=@AppVersion"></script>

    <!--Modulos funcionales por categoria-->
    <script src="~/Scripts/tablaScrapRfa.js?v=@AppVersion"></script>
    <script src="~/Scripts/scrapForm.js?v=@AppVersion"></script>
    <script src="~/Scripts/eventosDinamicosScrap.js?v=@AppVersion"></script>
    <script src="~/Scripts/editarDetalle.js?v=@AppVersion"></script>
    <script src="~/Scripts/eliminarScrap.js?v=@AppVersion"></script>
    <script src="~/Scripts/exportarExcel.js?v=@AppVersion"></script>
    <script src="~/Scripts/visualizarDetalles.js?v=@AppVersion"></script>


    <script src="~/Scripts/modalGenerarRfa.js?v=@AppVersion"></script>
    <script src="~/Scripts/asignarRfa.js?v=@AppVersion"></script>
    <script src="~/Scripts/guardarRFA.js?v=@AppVersion"></script>

    <script src="~/Scripts/gracipasScrap.js?v=@AppVersion"></script>

    <!--Eventos generales-->
    <script src="~/Scripts/login.js?v=@AppVersion"></script>
    <script src="~/Scripts/modalesUtilitarios.js?v=@AppVersion"></script>
End section