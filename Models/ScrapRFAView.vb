''' <summary>
''' Clase que representa una vista simplificada de RFA de scrap con sus propiedades.
''' </summary>
Public Class ScrapRFAView
    ''' <summary>Folio del scrap</summary>
    Public Property Id_scrap As Integer
    ''' <summary>RFA asignado por las Data Clerk para ese Folio</summary>
    Public Property RFA As String
    ''' <summary>Status del Folio, Abierto o Cerrado</summary>
    Public Property Status As String
End Class
