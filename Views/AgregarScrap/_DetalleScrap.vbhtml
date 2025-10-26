@ModelType List(Of ScrapSmtWeb.ScrapDetalle)
@Code
    'Obtiene el primero elemento, usa if(condition, true_value, false_value) para vb.net
    Dim primerDetalle = Model.FirstOrDefault()
    'Extrae el Id_Scrap, usa if(condition, true_value, false_value) para manejo de nulo
    Dim folioActivo As Integer = If(primerDetalle IsNot Nothing, primerDetalle.Id_scrap, 0)
End Code

<div class="row mb-3">
    <div class="col-12 d-flex justify-content-between px-3">

        <button type="button"
                 id="btnAgregarAlFolioDetalle"
                 class="btn btn-outline-success me-2"
                 data-id-scrap="@folioActivo"
                 data-bs-dismiss="modal"
                 title="Abrir formulario para agregar mas registros a este folio">
            <i class="bi bi-plus-circle"></i> Agregar al Folio
        </button>
        <button type="button" id="btnExportarDetalles" class="btn btn-success" title="Exportar todos los registros de este Folio a un Excel">
            <i Class="fas fa-file-excel"></i> Exportar a Excel
        </button>
    </div>
</div>

<table class="table table-bordered table-hover">
    <thead class="table-light">
        <tr>
            @If Session("Tipo") = "ADMIN" OrElse Session("Tipo") = "SUPERVISOR" OrElse Session("Tipo") = "TECNICO" Then
                @<th>Acciones</th>
            End If
            <th>Folio Scrap</th>
            <th>Linea</th>
            <th>CFT</th>
            <th>Numero de Parte</th>
            <th>RFA</th>
            <th>Centro de Costos</th>
            <th>Costo Unitario</th>
            <th>Costo Total</th>
            <th>Contención</th>
            <th>Acción Correctiva</th>
            <th>Causa Raíz</th>
            <th>Defecto</th>
            <th>Tabla</th>
        </tr>
    </thead>
    <tbody>
        @For Each item In Model
            @<tr>
                @If Session("Tipo") = "ADMIN" OrElse Session("Tipo") = "SUPERVISOR" OrElse Session("Tipo") = "TECNICO" Then
                    @<td>
                        <button type="button" class="btn btn-sm btn-warning btnEditarScrap" data-id="@item.Tabla">Editar</button>
                    </td>
                End If

                <td>@item.Id_scrap</td>
                <td>@item.Line</td>
                <td>@item.CFT</td>
                <td>@item.Part_number</td>
                <td>@item.RFA</td>
                <td>@item.Ccost</td>
                <td>@String.Format("{0:C}", item.Unit_cost)</td>
                <td>@String.Format("{0:C}", item.Total_cost)</td>
                <td>@item.Contencion</td>
                <td>@item.ACorrectiva</td>
                <td>@item.CausaRaiz</td>
                <td>@item.Defecto_Espanol</td>
                <td>@item.Tabla</td>
            </tr>
        Next
    </tbody>
</table>