Imports System.Web.Mvc
Imports System.Data.SqlClient

Public Class PatchNotesController
    Inherits Controller

    Private connectionString As String = "Data Source=10.131.40.190;Initial Catalog=patchNotesSMTSCRAPWEB;User ID=dluser;Password=P@sadl19"

    Function GetNotes() As ActionResult
        Dim notas As New List(Of PatchNote)

        Using cn As New SqlConnection(connectionString)
            cn.Open()
            Dim cmd As New SqlCommand("SELECT TOP 5 Id, Titulo, Descripcion FROM PatchNotes ORDER BY Fecha DESC", cn)
            Dim reader = cmd.ExecuteReader()

            While reader.Read()
                notas.Add(New PatchNote With {
                          .Id = Convert.ToInt32(reader("Id")),
                          .Titulo = reader("Titulo").ToString(),
                          .Descripcion = reader("Descripcion").ToString()
                          })
            End While
        End Using

        Return Json(New With {.success = True, .data = notas}, JsonRequestBehavior.AllowGet)
    End Function
End Class

Public Class PatchNote
    Public Property Id As Integer
    Public Property Titulo As String
    Public Property Descripcion As String

End Class