@ModelType ScrapSmtWeb.ScrapRFAView

@Code
    'Detecta si el usuario es TECNICO o SUPERVISOR, solo esos pueden editar el RFA
    Dim esEditable = Session("Tipo") = "DataClerk" OrElse Session("Tipo") = "ADMIN"
End Code

<div class="container">
    <h5>Asignar RFA al Folio: @Model.Id_scrap</h5>

    @Html.HiddenFor(Function(m) m.Id_scrap, New With {.id = "Id_scrap"})

    <div class="form-group mt-2">
        <label for="RFA">RFA:</label>
        @If esEditable Then
            @Html.TextBoxFor(Function(m) m.RFA, New With {.id = "RFA", .class = "form-control"})
        Else
            @Html.TextBoxFor(Function(m) m.RFA, New With {.id = "RFA", .class = "form-control", .readonly = "readonly"})
        End If
    </div>

    <div class="form-group mt-2">
        <label for="Status">Status:</label>
        @If esEditable Then
            @Html.DropDownListFor(Function(m) m.Status, New SelectList(New List(Of SelectListItem) From {
                                      New SelectListItem With {.Value = "OPEN", .Text = "OPEN"},
                                      New SelectListItem With {.Value = "CLOSE", .Text = "CLOSE"}
                                      }, "Value", "Text"), New With {.id = "Status", .class = "form-control"})
        Else
            @Html.DropDownListFor(Function(m) m.Status, New SelectList(New List(Of SelectListItem) From {
                                            New SelectListItem With {.Value = "OPEN", .Text = "OPEN"},
                                            New SelectListItem With {.Value = "CLOSE", .Text = "CLOSE"}
                                            }, "Value", "Text"), New With {.id = "Status", .disabled = "disabled"})
        End If
    </div>

    <div class="text-end mt-3">
        @If esEditable Then
            @<button type="button" class="btn btn-success" id="btnGuardarRFA">Guardar</button>
        End If
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
    </div>
</div>
