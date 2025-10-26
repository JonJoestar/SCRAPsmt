@ModelType ScrapSmtWeb.ScrapDetalleEditar
@Code
    Dim tipoUsuario As String = If(Session("Tipo") IsNot Nothing, Session("Tipo").ToString().ToUpper(), "").Trim()
    Dim esEditable As Boolean = (tipoUsuario = "ADMIN" Or tipoUsuario = "TECNICO" Or tipoUsuario = "SUPERVISOR")
    'Se define lo deshabilitado:
    '1. Campos del Tecnico(Contencion, causa raiz, accion correctiva) de deshabilitaran para el Supervisor
    Dim deshabilitarCamposTecnico = If(tipoUsuario = "SUPERVISOR", "disabled", "")

    '2. Campos del Supervisor(HU, Quien, Cuando) de deshablitan para el Tecnico
    Dim deshabilitarCamposSupervisor = If(tipoUsuario = "TECNICO", "disabled", "")
End Code

<div class="container">
    <h5>Folio @Model.Id_scrap</h5>
    <div class="row">
        <div class="col-md-6"><strong>Linea:</strong> @Model.Line</div>
        <div class="col-md-6"><strong>CFT:</strong> @Model.CFT</div>
        <div class="col-md-6"><strong>Turno:</strong> @Model.Shift</div>
        <div class="col-md-6"><strong>Centro de Costos:</strong> @Model.Ccost</div>
        <div class="col-md-6"><strong>Folio Scrap:</strong> @Model.Id_scrap</div>
        <div class="col-md-6"><strong>Equipo:</strong> @Model.Equipment</div>
        <div class="col-md-6"><strong>Numero de Parte:</strong> @Model.Part_number</div>
        <div class="col-md-6"><strong>Batch:</strong> @Model.Batch</div>
        <div class="col-md-6"><strong>Costo Unitario:</strong> $@Model.Unit_cost</div>
        <div class="col-md-6"><strong>Costo Total:</strong> $@Model.Total_cost</div>
        <div class="col-md-6"><strong>Cantidad de PCB:</strong> @Model.Qty</div>
        <div class="col-md-6"><strong>HU:</strong> @Model.HU</div>
        <div class="col-md-6"><strong>Referencia:</strong> @Model.Reference</div>
        <div class="col-md-6"><strong>Defecto:</strong> @Model.Defecto_Espanol</div>
        <div class="col-md-6"><strong>RFA:</strong> @Model.RFA</div>
        <div class="col-md-6"><strong>Validacion Ingenieria:</strong> @Model.Ing_validacion</div>
       
        @If esEditable Then
            @<hr />
            @<form id="formEditarScrap">
                @Html.HiddenFor(Function(m) m.Tabla, New With {.id = "Tabla"})
                
                @*Campos deshabilitados para Supervisor*@
                <div class="form-group mt-2">
                    <label>Contencion</label>
                    @If tipoUsuario = "SUPERVISOR" Then
                        @Html.TextAreaFor(Function(m) m.Contencion, New With {.class = "form-control", .disabled = "disabled"})
                    Else
                        @Html.TextAreaFor(Function(m) m.Contencion, New With {.class = "form-control"})
                    End If
                </div>
                <div class="form-group mt-2">
                    <label>Causa Raiz</label>
                    @If tipoUsuario = "SUPERVISOR" Then
                        @Html.TextAreaFor(Function(m) m.CausaRaiz, New With {.class = "form-control", .disabled = "disabled"})
                    Else
                        @Html.TextAreaFor(Function(m) m.CausaRaiz, New With {.class = "form-control"})
                    End If
                </div>
                <div class="form-group mt-2">
                    <label>Accion Correctiva</label>
                    @If tipoUsuario = "SUPERVISOR" Then
                        @Html.TextAreaFor(Function(m) m.ACorrectiva, New With {.class = "form-control", .disabled = "disabled"})
                    Else
                        @Html.TextAreaFor(Function(m) m.ACorrectiva, New With {.class = "form-control"})
                    End If
                </div>

                @*Campos deshabilitos para Tecnico*@
             <div class="form-group mt-2">
                 <label>HU</label>
                 @If tipoUsuario = "TECNICO" Then
                     @Html.TextBoxFor(Function(m) m.HU, New With {.class = "form-control", .readonly = "readonly"})
                 Else
                     @Html.TextBoxFor(Function(m) m.HU, New With {.class = "form-control"})
                 End If

             </div>
             <div class="form-group mt-2">
                 <label>Validado Por</label>
                 @If tipoUsuario = "TECNICO" Then
                     @Html.DropDownListFor(Function(m) m.Quien, CType(ViewBag.ListaValidacionING, IEnumerable(Of SelectListItem)), "Seleccione un Validador", New With {.class = "form-control", .disabled = "disabled"})
                     @Html.HiddenFor(Function(m) m.Quien)
                 Else
                     @Html.DropDownListFor(Function(m) m.Quien, CType(ViewBag.ListaValidacionING, IEnumerable(Of SelectListItem)), "Seleccione un Validador", New With {.class = "form-control"})
                 End If
             </div>
             <div class="form-group mt-2">
                 <label>Fecha de Validacion</label>
                 @If tipoUsuario = "TECNICO" Then
                     @Html.TextBoxFor(Function(m) m.Cuando, "{0:yyyy-MM-dd}", New With {.Class = "form-control", .type = "date", .id = "Cuando", .disabled = "disabled"})
                     @Html.HiddenFor(Function(m) m.Cuando)
                 Else
                     @Html.TextBoxFor(Function(m) m.Cuando, "{0:yyyy-MM-dd}", New With {.Class = "form-control", .type = "date", .id = "Cuando"})
                 End If
             </div>

                <div class="text-end mt-3">
                    <button type="submit" class="btn btn-success" id="btnGuardarEdicion">Guardar Cambios</button>
                    <button type="button" class="btn btn-danger" id="btnEliminarScrap">Eliminar</button>
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Regresar</button>
                </div>
             </form>
        Else
            @<div class="text-end mt-3">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Regresar</button>
             </div>
        End If
    </div>
</div>
