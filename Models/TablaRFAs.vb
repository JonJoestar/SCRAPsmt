''' <summary>
''' Clase que representa una tabla de RFAs con sus propiedades.
''' </summary>
Public Class TablaRFAs
    ''' <summary>La fecha en la que se le dio RFA a este Folio</summary>
    Public Property DateInput As Date
    ''' <summary>Folio del cumulo de registros de scrap</summary>
    Public Property Id_scrap As Integer
    ''' <summary>CFT al que pertenecen esos registros de scrap</summary>
    Public Property CFT As String
    ''' <summary>Csoto total de los registros de ese Folio</summary>
    Public Property TotalScrap As String
    ''' <summary>RFA asignado a ese Folio de scrap, asignado por las Data Clerk</summary>
    Public Property RFA As String
End Class
