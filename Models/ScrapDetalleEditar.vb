''' <summary>
''' Clase que representa el detalle editable de un scrap con sus propiedades.
''' Nota. la maoria de las propiedades son solo lectura, excepto las indicadas como editables.
''' </summary>
Public Class ScrapDetalleEditar
    ''' <summary>Id de la tabla asociada al scrap tabla de sql oculto en el formulario</summary>
    Public Property Tabla As Integer
    ''' <summary>Folio del scrap</summary>
    Public Property Id_scrap As String 'solo lectura
    ''' <summary>Linea a la que corresponde el scrap</summary>
    Public Property Line As String  'solo lectura
    ''' <summary>CFT al que la linea corresponde</summary>
    Public Property CFT As String   'solo lectura
    ''' <summary>Turno de la captura del scrap</summary>
    Public Property Shift As String 'solo lectura
    ''' <summary>Centro de costo asociado al material desechado</summary>
    Public Property Ccost As String 'solo lectura
    ''' <summary>Costo individaul por pieza de scrap</summary>
    Public Property Unit_cost As Decimal    'solo lectura
    ''' <summary>Costo total de todos los registros de scrap de un mismo folio</summary>
    Public Property Total_cost As Decimal   'solo lectura
    ''' <summary>HU perteneciente al scrap registrado</summary>
    Public Property HU As String 'solo lectura
    ''' <summary>Referecia o descripcion breve</summary>
    Public Property Reference As String 'solo lectura
    ''' <summary>Defecto asociado al scrap</summary>
    Public Property Defecto_Espanol As String   'solo lectura
    ''' <summary>Equipo donde ocurrio el defecto</summary>
    Public Property Equipment As String 'solo lectura
    ''' <summary>Numero de parte perteneciente al componente</summary>
    Public Property Part_number As String   'solo lectura
    ''' <summary>Batch del modelo donde se origino el scrap</summary>
    Public Property Batch As String 'solo lectura
    ''' <summary>Cantidad de registros de scrap po folio</summary>
    Public Property Qty As Integer 'solo lectura
    ''' <summary>RFA asignado al folio de scrap, sirve para identificacion</summary>
    Public Property RFA As String   'solo lectura
    ''' <summary>Primera validacion del registro de scrap</summary>
    Public Property Ing_validacion As String 'solo lectura
    ''' <summary>Nombre del supervisor qe valida el scrap final (editable solo para supervisor)</summary>
    Public Property Quien As String 'editable
    ''' <summary>Fecha y hora de la validacion del scrap (editable para supervisor)</summary>
    Public Property Cuando As DateTime? 'editable
    ''' <summary>Descripcion detallada de la contencion aplicada (editable solo para tecnico)</summary>
    Public Property Contencion As String    'editable
    ''' <summary>Descripcion detallada de el causante de dicho scrap(editable para tecnico)</summary>
    Public Property CausaRaiz As String 'editable
    ''' <summary>Descripcion detallada de las acciones correctivas realizadas(eitable para tecnico)</summary>
    Public Property ACorrectiva As String   'editable
End Class
