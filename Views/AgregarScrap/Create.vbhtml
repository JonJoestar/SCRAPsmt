@ModelType ScrapSmtWeb.FormularioScrap
@Code
    Layout = Nothing
End Code
<div class="container mt-4">
    <form id="formScrap">
        <div class="col-md-12 mb-3">
            <div id="contenedorFolio" style="display: none; justify-content: flex-start; align-items: center; padding: 5px;">
                <p class="mb-0 me-3 fw-bold">Folio de Captura Activo: </p>

                <span id="folioScrapActivo" class="badge bg-primary fs-5 fw-bold p-2 shadow-sm"></span>
            </div>
        </div>
        @Html.AntiForgeryToken()

        <!--Seccion 1: Linea, CFT, Ccosto-->
        <div class="row mb-3">
            <div class="col-md-4">
                <label for="Linea" class="form-label">Linea</label>
                @Html.DropDownListFor(Function(m) m.Line, New SelectList(ViewBag.Linea), "Seleccione una Linea", New With {.class = "form-control", .id = "Linea"})
            </div>
            <div class="col-md-4">
                <label for="CFT" class="form-label">CFT</label>
                @Html.TextBoxFor(Function(m) m.CFT, New With {.class = "campo-resaltado form-control", .id = "CFT", .readonly = "readonly"})
            </div>
            <div class="col-md-4">
                <label for="Ccosto" class="form-label">Centro de Costos</label>
                @Html.TextBoxFor(Function(m) m.Ccost, New With {.class = "campo-resaltado form-control", .id = "Ccosto", .readonly = "readonly"})
            </div>
        </div>

        <!--Seccion 2: Turno, Num de Parte, Costo-->
        <div class="row mb-3">
            <div class="col-md-4">
                <label for="Shift" class="form-label">Turno</label>
                @Html.DropDownListFor(Function(m) m.Shift, New SelectList(ViewBag.Turno), "Seleccione Turno", New With {.id = "Shift", .class = "form-control"})
            </div>
            <div class="col-md-4">
                <label for="NumParte" class="form-label">Numero de Parte</label>
               @Html.TextBoxFor(Function(m) m.Part_number, New With {.class = "form-control", .id = "NumParte", .autocomplete = "off", .oninput = "this.value = this.value.toUpperCase();"})
            </div>
            <div class="col-md-4">
                <label for="Costo" class="form-label">Costo Unitario</label>
                @Html.TextBoxFor(Function(m) m.Unit_cost, New With {.class = "campo-resaltado form-control", .id = "Costo", .readonly = "readonly"})
            </div>
        </div>

        <!--Seccion 3: Catidad, Defecto, Batch-->
        <div class="row mb-3">
            <div class="col-md-4">
                <label for="Qty" class="form-label">Cantidad</label>
                @Html.TextBoxFor(Function(m) m.Qty, New With {.id = "Qty", .class = "form-control", .type = "number", .min = "1", .max = "99"})
                @Html.ValidationMessageFor(Function(m) m.Qty)
            </div>
            <div class="col-md-4">
                <label for="Defecto" class="form-label">Defecto</label>
                @Html.DropDownListFor(Function(m) m.Defecto_Espanol, New SelectList(Enumerable.Empty(Of SelectListItem)), "Selecciona Defecto", New With {.class = "form-control", .id = "Defecto"})
            </div>
            <div class="col-md-4" id="campoBatch">
                <label for="Batch" class="form-label">Batch</label>
                @Html.TextBoxFor(Function(m) m.Batch, New With {.id = "Batch", .class = "form-control", .oninput = "this.value = this.value.toUpperCase();"})
            </div>
        </div>

        <!--Seccion 4: Referencia, HU, Tipo-->
        <div class="row mb-3">
            <div class="col-md-6">
                <label for="Referencia" class="form-label">Referencia</label>
                @Html.TextBoxFor(Function(m) m.Reference, New With {.id = "Referencia", .class = "form-control", .oninput = "this.value = this.value.toUpperCase();"})
            </div>
            @*<div class="col-md-4" id="campoHU">
                <label for="HU" class="form-label">HU</label>
                @Html.TextBoxFor(Function(m) m.HU, New With {.id = "HU", .class = "form-control", .oninput = "this.value = this.value.toUpperCase();", .onkeypress = "if (event.keyCode === 13) { event.preventDefault(); return false; }"})
            </div>*@
            <div class="col-md-6">
                <label for="Tipo" class="form-label">Tipo</label>
                @Html.DropDownListFor(Function(m) m.Tipo, New SelectList(New List(Of SelectListItem) From {
                               New SelectListItem With {.Value = "EVENTO", .Text = "EVENTO"},
                               New SelectListItem With {.Value = "PCB Virgen", .Text = "PCB Virgen"},
                               New SelectListItem With {.Value = "Scrap General", .Text = "Scrap General"},
                               New SelectListItem With {.Value = "Rango A", .Text = "Rango A"},
                               New SelectListItem With {.Value = "Material Resagado", .Text = "Material Resagado"}
                            }, "Value", "Text"), "Seleccione un Tipo", New With {.id = "Tipo", .class = "form-control"})
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-md-4">
                <label for="Categoria" class="form-label">Categoria</label>
                @Html.DropDownListFor(Function(m) m.Categoria, New SelectList(New List(Of SelectListItem) From {
                         New SelectListItem With {.Value = "Colocacion Componentes", .Text = "Colocacion Componentes"},
                         New SelectListItem With {.Value = "Error de Operacion", .Text = "Error de Operacion"},
                         New SelectListItem With {.Value = "Excepcion Panacim", .Text = "Excepcion Panacim"},
                         New SelectListItem With {.Value = "Ingenieria", .Text = "Ingenieria"},
                         New SelectListItem With {.Value = "Master Expirado", .Text = "Master Expirado"},
                         New SelectListItem With {.Value = "Obsoleto", .Text = "Obsoleto"},
                         New SelectListItem With {.Value = "Corte de Electricidad", .Text = "Corte de Electricidad"},
                         New SelectListItem With {.Value = "Peoblemas de Instalacion", .Text = "Peoblemas de Instalacion"},
                         New SelectListItem With {.Value = "Problemas de Laser", .Text = "Problemas de Laser"},
                         New SelectListItem With {.Value = "Problema de Maquina", .Text = "Problema de Maquina"},
                         New SelectListItem With {.Value = "Problema de Panacim", .Text = "Problema de Panacim"},
                         New SelectListItem With {.Value = "Problema de Proveedor", .Text = "Problema de Proveedor"},
                         New SelectListItem With {.Value = "Problema de Soldadura", .Text = "Problema de Soldadura"},
                         New SelectListItem With {.Value = "Problema de Saldos", .Text = "Problema de Saldos"},
                         New SelectListItem With {.Value = "Problema de Transferencia", .Text = "Problema de Transferencia"},
                         New SelectListItem With {.Value = "Proceso de Reparacion", .Text = "Proceso de Reparacion"}
                         }, "Value", "Text"), "Selecciona una Categoria", New With {.id = "Categoria", .class = "form-control"})
            </div>
            <div class="col-md-4">
                <label for="ValidacionING" class="form-label">Validacion Ingenieria</label>
                @Html.DropDownListFor(Function(m) m.Ing_validacion, CType(ViewBag.ListaValidacionING, IEnumerable(Of SelectListItem)), "Seleccione un Autorizador", New With {.id = "ValidacionING", .class = "form-control"})
            </div>
            <div class="col-md-4">
                <label for="Equipment" class="form-label">Equipo</label>
                @Html.DropDownListFor(Function(m) m.Equipment, CType(ViewBag.Equipos, IEnumerable(Of SelectListItem)), "Seleccione un Equipo", New With {.class = "form-control", .id = "Equipment"})
            </div>
        </div>

        <!-Botones--->
        <div class="row">
            <div class="col-md-12 text-end">
                <button type="submit" class="btn btn-primary" id="btnGuardarScrap">Guardar</button>
                <button type="button" class="btn btn-secondary" id="btnFinalizarCaptura">Regresar</button>
            </div>
        </div>
    </form>
</div>

<!--Scripts para cargar datos diamicos-->
<script src="~/Scripts/xlsx.full.min.js"></script>
<script src="~/Scripts/jquery.validate.min.js"></script>
<script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>