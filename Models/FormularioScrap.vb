''' <summary>
''' Clase que representa la estructura de datos utilizada para el formulario de scrap
''' </summary>
Public Class FormularioScrap
    ''' <summary>Identificador único del scrap ID generalmente asignado al guardar el registro</summary>
    Public Property Id_scrap As Integer
    ''' <summary>La linea de produccion donde courrio el scrap </summary>
    Public Property Line As String
    ''' <summary>El CFT asignado referente a la linea</summary>
    Public Property CFT As String
    ''' <summary>El turno donde se esta registrando el scrap</summary>
    Public Property Shift As String
    ''' <summary>El centro de costos asociado al material desechado</summary>
    Public Property Ccost As String
    ''' <summary>Numero de [arte del componente o PCB danado</summary>
    Public Property Part_number As String
    ''' <summary>Costo de cada pieza de scrap</summary>
    Public Property Unit_cost As Decimal
    ''' <summary>La referencia del material</summary>
    Public Property Reference As String
    ''' <summary>Cantidad de piezas por registro de scrap</summary>
    Public Property Qty As Integer
    ''' <summary>Descripcion del defecto encontrado</summary>
    Public Property Defecto_Espanol As String
    ''' <summary>Batch del modelo asociado al scrap</summary>
    Public Property Batch As String
    ''' <summary>Numero de HU o unidad de manejo donde se encontraba el material</summary>
    Public Property HU As String
    ''' <summary>El tipo de scrap</summary>
    Public Property Tipo As String
    ''' <summary>La persona que valida en primera instancia el registro de scrap</summary>
    Public Property Ing_validacion As String
    ''' <summary>Equipo/Maquinaria donde se genero el defecto</summary>
    Public Property Equipment As String
    ''' <summary>Categoria a la que pertenece el scrap</summary>
    Public Property Categoria As String

End Class
