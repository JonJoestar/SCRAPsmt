Imports System.IO

Public Module Utilidades
    Public Sub RegistrarError(mensaje As String)
        Try
            Dim rutaLog As String = HttpContext.Current.Server.MapPath("~/Logs/Errores.log")

            'Crea el directorio si no existe
            Dim directorio = Path.GetDirectoryName(rutaLog)
            If Not Directory.Exists(directorio) Then
                Directory.CreateDirectory(directorio)
            End If

            'Registra el error
            Using sw As New StreamWriter(rutaLog, True)
                sw.WriteLine($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss}] {mensaje}")
            End Using

        Catch ex As Exception
            'Si falla el log, no hace nada para no afectar al usuario
        End Try
    End Sub
End Module
