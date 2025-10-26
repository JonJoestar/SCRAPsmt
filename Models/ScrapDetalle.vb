''' <summary>
''' Clase que representa el detalle de un scrap con sus propiedades.
''' </summary>
Public Class ScrapDetalle
    ''' <summary>Folio del scrap, normalmente son varios registros, comparten el mismo folio</summary>
    Public Property Id_scrap As Integer
    ''' <summary>Linea al que pertenece el registro del scrap</summary>
    Public Property Line As String
    ''' <summary>CFT al que pertenece la linea donde se registro el scrap</summary>
    Public Property CFT As String
    ''' <summary>Numero de parte del componente</summary>
    Public Property Part_number As String
    ''' <summary>IRFA asignado por las Data Clerk</summary>
    Public Property RFA As String
    ''' <summary>Costo unitario por pieza de scrap</summary>
    Public Property Unit_cost As Decimal
    ''' <summary>Costo total de todos los registros de scrap de un mismo FOLIO</summary>
    Public Property Total_cost As Decimal
    ''' <summary>Centro de costos asosciado al registro de scrap, se determina al seleccionar la linea</summary>
    Public Property Ccost As String
    ''' <summary>Contencion realizada por el tecnico</summary>
    Public Property Contencion As String
    ''' <summary>Descripcion detallada por el tecnico sobre las acciones correctias realizadas</summary>
    Public Property ACorrectiva As String
    ''' <summary>Descipcion detallada por el teccnico sobre la causa real del defecto</summary>
    Public Property CausaRaiz As String
    ''' <summary>Defecto encontrado en la pieza</summary>
    Public Property Defecto_Espanol As String
    ''' <summary>ID del registro asignado por la base de datos</summary>
    Public Property Tabla As Integer ' id de la tabla
End Class
