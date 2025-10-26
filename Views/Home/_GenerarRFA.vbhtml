@ModelType List(Of ScrapSmtWeb.TablaRFAs)
@Code
    Dim tipoUsuario = Session("Tipo").ToString.ToUpper() 'se convierte a mayusculas para evitar problemas de comparacion
    Dim puedeEditarRFA = tipoUsuario = "DATACLERK" Or tipoUsuario = "ADMIN" 'comparacion usando mayusculas
End Code

<table class="table table-bordered table-striped table-sm" id="tablaGenerarRFA">
    <thead class="table-dark">
        <tr>
            <th>Fecha Captura</th>
            <th>Folio Scrap</th>
            <th>CFT</th>
            <th>Costo Total</th>
            <th>RFA</th>
            <th>Editar</th>
        </tr>
    </thead>
    <tbody>
        @For Each item In Model
            @<tr>
                <td>@item.DateInput</td>
                <td>@item.Id_scrap</td>
                <td>@item.CFT</td>
                <td>@String.Format("{0:C}", item.TotalScrap)</td>
                <td>@item.RFA</td>
                <td>
                    @If puedeEditarRFA Then
                       @<Button Class="btn btn-sm btn-primary btnEditarRFA" data-id="@item.Id_scrap">
                           Editar
                        </button>
                    End If
                </td>
             </tr>
        Next
    </tbody>
</table>