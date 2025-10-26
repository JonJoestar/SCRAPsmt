''' <summary>
''' Clase que representa un autorizador con sus propiedades (supervisor o admin) con sus credenciales.
''' </summary>
Public Class Autorizadores
    ''' <summary>Nombre del autorizador.</summary>
    Public Property Nombre As String
    ''' <summary>contraseña empleado del autorizador.</summary>
    Public Property PasswordAuto As String
    ''' <summary>Tipo de autorizador: "SUPERVISOR", "ADMIN", "TECNICO", etc</summary>
    Public Property Tipo As String
    ''' <summary>Número de empleado del autorizador.</summary>
    Public Property Numero As String
End Class
