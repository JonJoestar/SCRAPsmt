Imports System.Web.Mvc
Imports System.Data.SqlClient
Imports System.Configuration
Imports Antlr.Runtime.Misc
Public Class HomeController
    Inherits System.Web.Mvc.Controller

    'GET: Home/Index
    Function Index() As ActionResult
        'Determina el estado e autenticacion
        Dim estaAutenticado As Boolean = Session("UsuarioAutenticado") IsNot Nothing

        'Asigna el ViewBag [ara los botones(deferente depediendo la autenticacion)
        ViewBag.BotonesHabilitados = estaAutenticado

        'Asiga los datos RFA 
        ViewBag.DatosRFA = ObtenerDatosRFA()

        Return View()
    End Function

    Function VerRFAModal() As ActionResult
        Dim datos = ObtenerDatosRFA()
        Return PartialView("_GenerarRFA", datos)
    End Function

    Function ObtenerDatosRFA() As JsonResult
        Dim datos As New List(Of Object)()
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim query As String = "
                        SELECT cast(Scrap_header.Date_time as Date) as DateInput , Scrap_header.Id_scrap, Scrap_detalle.CFT, sum(Scrap_detalle.Total_cost) AS TotalScrap,
                        Scrap_header.RFA FROM Scrap_header INNER JOIN Scrap_detalle ON Scrap_header.Id_scrap = Scrap_detalle.Id_scrap Where Scrap_detalle.Status = 'OPEN'
                        group by cast(Scrap_header.Date_time as Date), Scrap_header.Id_scrap, Scrap_detalle.CFT, Scrap_header.RFA order by Scrap_detalle.CFT"

                Using cmd As New SqlCommand(query, conn)
                    Dim reader = cmd.ExecuteReader()
                    While reader.Read()
                        datos.Add(New With {
                                  .DateInput = reader("DateInput").ToString(),
                                  .Id_scrap = reader("Id_scrap").ToString(),
                                  .CFT = reader("CFT").ToString(),
                                  .TotalScrap = reader("TotalScrap").ToString(),
                                  .RFA = reader("RFA").ToString()
                        })
                    End While
                End Using
            End Using

            'Datatable espera un objeto con propiedad "data"
            Return Json(New With {.data = datos}, JsonRequestBehavior.AllowGet)

        Catch ex As Exception
            Return Json(New With {.data = New List(Of Object)(), .error = ex.Message}, JsonRequestBehavior.AllowGet)
        End Try
    End Function

    Function ObtenerResumenScrap() As JsonResult
        Dim resumen As New List(Of Object)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString

        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Dim query As String = "SELECT Line, SUM(Qty) AS TotalQty, SUM(Total_cost) AS TotalCosto FROM Scrap_detalle GROUP BY Line ORDER BY SUM(Total_cost) DESC"
            Using cmd As New SqlCommand(query, conn)
                Dim reader = cmd.ExecuteReader()
                While reader.Read()
                    resumen.Add(New With {
                                .Linea = reader("Line").ToString(),
                                .Cantidad = Convert.ToInt32(reader("TotalQty")),
                                .Costo = Convert.ToDecimal(reader("TotalCosto"))
                                })
                End While
            End Using
        End Using

        Return Json(resumen, JsonRequestBehavior.AllowGet)
    End Function

    'POST: Home/Login
    <HttpPost>
    Function Login(empleadoNUM As String) As ActionResult
        'Validacion de longitud vacia
        'y que la longitud sea menor o igual a 5
        If String.IsNullOrEmpty(empleadoNUM) OrElse empleadoNUM.Length > 5 Then
            ViewBag.ErrorMessage = "Numero de Empleado Invalido (longitud maxima: 5 digitos)"
            Return View("Index")
        End If

        Dim empleado As Autorizadores = Nothing
        Try
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim queryAutorizadores As String = "SELECT Nombre, Tipo, Numero FROM Autorizadores WHERE Numero = @Numero AND Tipo IN ('TECNICO', 'SUPERVISOR', 'DataClerk', 'ADMIN')"
                Using cmd As New SqlCommand(queryAutorizadores, conn)
                    cmd.Parameters.AddWithValue("@Numero", empleadoNUM)
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        'usuario valido
                        empleado = New Autorizadores() With {
                            .Nombre = reader("Nombre").ToString(),
                            .Tipo = reader("Tipo").ToString(),
                            .Numero = reader("Numero").ToString()
                            }
                    End If
                End Using
            End Using
        Catch ex As Exception
            ViewBag.ErrorMessage = "Error de Conexion: " & ex.Message
            Return View("Index")
        End Try


        If empleado IsNot Nothing Then
            'Usuario encontrado como tecnico o supervisor
            Session("UsuarioAutenticado") = empleado.Nombre
            Session("UsuarioNumero") = empleado.Numero
            Session("Tipo") = empleado.Tipo 'TECNICO, SUPERVISOR
            ViewBag.MensajeBienvenida = "Bienvenido, " & empleado.Nombre & "!"
            Session("MostrarToast") = "SI"
            Return RedirectToAction("Index")
        Else
            'Busca en la tabla de Empleados para validar si es Operador
            Dim operador As Empleados = BuscarOperador(empleadoNUM)
            If operador IsNot Nothing Then
                Session("UsuarioAutenticado") = operador.Nombre
                Session("UsuarioNumero") = operador.EMPLEADOID.ToString()
                Session("Tipo") = "OPERADOR"
                ViewBag.MensajeBienvenida = "Bienvenido, " & operador.Nombre & "!"
                Session("MostrarToast") = "SI"
                Return RedirectToAction("Index")
            Else
                ViewBag.ErrorMessage = "Empleado no encontrado O no tiene permisos"
                Return View("Index")
            End If
        End If
    End Function

    Function ObtenerDatosGrafico(fechaInicio As DateTime?, fechaFin As DateTime?) As JsonResult
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
        Dim lineas As New List(Of String)
        Dim cantidades As New List(Of Integer)
        Dim costos As New List(Of Decimal)

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim query As String = "SELECT Line, SUM(Qty) AS TotalQty, SUM(Total_cost) AS TotalCost
                                           FROM Scrap_detalle WHERE 1=1 "
                If fechaInicio.HasValue Then
                    query &= " AND Date_time >= @FechaInicio"
                End If
                If fechaFin.HasValue Then
                    query &= " AND Date_time <= @FechaFin"
                End If

                query &= " GROUP BY Line"

                Using cmd As New SqlCommand(query, conn)
                    If fechaInicio.HasValue Then
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value)
                    End If
                    If fechaFin.HasValue Then
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value)
                    End If

                    Dim reader = cmd.ExecuteReader()
                    While reader.Read()
                        lineas.Add(reader("Line").ToString())
                        cantidades.Add(Convert.ToInt32(reader("TotalQty")))
                        costos.Add(Convert.ToDecimal(reader("TotalCost")))
                    End While
                End Using
            End Using

            Return Json(New With {
                        .success = True,
                        .lineas = lineas,
                        .cantidades = cantidades,
                        .costos = costos
                        }, JsonRequestBehavior.AllowGet)

        Catch ex As Exception
            Return Json(New With {.success = False, .message = ex.Message}, JsonRequestBehavior.AllowGet)
        End Try
    End Function

    Function ObtenerScrapPendienteRFA() As JsonResult
        Dim datos As New List(Of Object)()
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim query As String = " SELECT cast(Scrap_header.Date_time as Date) as DateInput, 
                            Scrap_header.Id_scrap, 
                            Scrap_detalle.CFT, 
                            SUM(Scrap_detalle.Total_cost) AS TotalScrap,
                            Scrap_header.RFA,
                            CASE WHEN Scrap_header.RFA IS NULL OR Scrap_header.RFA = '' THEN 'Pendiente' ELSE 'Asignado' END AS EstadoRFA
                        FROM Scrap_header 
                        INNER JOIN Scrap_detalle ON Scrap_header.Id_scrap = Scrap_detalle.Id_scrap 
                        Where Scrap_detalle.Status = 'OPEN' 
                        GROUP BY cast(Scrap_header.Date_time as Date), Scrap_header.Id_scrap, Scrap_detalle.CFT, Scrap_header.RFA 
                        ORDER BY Scrap_detalle.CFT"

                Using cmd As New SqlCommand(query, conn)
                    Dim reader = cmd.ExecuteReader()
                    While reader.Read()
                        datos.Add(New With {
                                  .DateInput = If(IsDBNull(reader("DateInput")), "", reader("DateInput").ToString()),
                                  .Id_scrap = If(IsDBNull(reader("Id_scrap")), "", reader("Id_scrap").ToString()),
                                  .CFT = If(IsDBNull(reader("CFT")), "", reader("CFT").ToString()),
                                  .TotalScrap = If(IsDBNull(reader("TotalScrap")), "0", reader("TotalScrap").ToString()),
                                  .RFA = If(IsDBNull(reader("RFA")), "", reader("RFA").ToString())
                                  })
                    End While
                End Using
            End Using

            Return Json(New With {.data = datos}, JsonRequestBehavior.AllowGet)
        Catch ex As Exception
            Return Json(New With {.data = New List(Of Object)(), .error = ex.Message}, JsonRequestBehavior.AllowGet)
        End Try
    End Function

    Function ObtenerRFA(id As Integer) As ActionResult
        Dim model As New ScrapRFAView()                'Crea objeto vacio para guardar datos de la base de datos
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString    'Obtiene a cadena de conexion de web.config

        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Dim query As String = "SELECT RFA, Status FROM Scrap_detalle WHERE Id_scrap = @id"    'Consula a la base de datos para obtener el RFA y el Status del registro correspondiente

            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id", id)
                Dim reader = cmd.ExecuteReader()
                If reader.Read() Then   'Si encuentra el registro, llena el modelo con los datos
                    model.Id_scrap = id
                    model.RFA = If(IsDBNull(reader("RFA")), "", reader("RFA").ToString())   'Si algun campo esta vacio en la base de datos, lo reemplaza con un valor default
                    model.Status = If(IsDBNull(reader("Status")), "OPEN", reader("Status").ToString())
                End If
            End Using
        End Using
        Return PartialView("_AsignarRFA", model)    'Manda los datos a la vista
    End Function

    Function GuardarRFA(model As ScrapRFAView) As JsonResult    'Metodo que recive los datos del formulario por AJAX y los guarda en la base de datos
        Dim tipoUsuario = Session("Tipo")   'Si no es tecnico o supervisor, no podra guardar
        If tipoUsuario <> "TECNICO" AndAlso tipoUsuario <> "ADMIN" AndAlso tipoUsuario <> "DataClerk" Then
            Return Json(New With {.success = False, .message = "No tienes permisos para realizar esta accion"}) 'Regresa un mensaje de error si no tiene permisos
        End If

        Try
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Using conn As New SqlConnection(connectionString)
                conn.Open()

                If model.Status = "OPEN" Then
                    'Actualiza RFA y la fecha de apertura en Scrap_detalle
                    Dim sqlDetalle As String = "UPDATE Scrap_detalle SET RFA = @RFA, RFA_date_open = GETDATE(), Status = 'OPEN' WHERE Id_scrap = @Id_scrap" 'Si el Status es OPEN asigna el RFA y la fecha de apertura
                    Using cmd As New SqlCommand(sqlDetalle, conn)  'Actualiza la tabla de Scrap_detalle
                        cmd.Parameters.AddWithValue("@RFA", model.RFA)
                        cmd.Parameters.AddWithValue("@Id_scrap", model.Id_scrap)
                        Dim filasAfectadas As Integer = cmd.ExecuteNonQuery()
                        'Valida si se actualizo el registro
                        If filasAfectadas = 0 Then
                            'Si no se actualiza, el Id_scrap posiblemente no exista en la tabla
                            Return Json(New With {.success = False, .message = "No se encontro el registro en Scrap_detalle para actualizar el RFA"})
                        End If
                    End Using

                    'Actualiza Scrap_header si hay RFA valido
                    If Not String.IsNullOrEmpty(model.RFA) Then
                        Dim sqlHeader As String = "UPDATE Scrap_header SET RFA = @RFA, RFA_date =  GETDATE() WHERE Id_scrap = @Id_scrap" 'Si el Status es OPEN y hay un RFA valido se actualiza el Scrap_header
                        Using cmdHeader As New SqlCommand(sqlHeader, conn)
                            cmdHeader.Parameters.AddWithValue("@RFA", model.RFA)
                            cmdHeader.Parameters.AddWithValue("@Id_scrap", model.Id_scrap)
                            Dim filasHeader As Integer = cmdHeader.ExecuteNonQuery()
                            If filasHeader = 0 Then
                                Return Json(New With {.success = False, .message = "No se encontro el registro en Scrap_header para actualizar el RFA"})
                            End If
                        End Using
                    End If

                ElseIf model.Status = "CLOSE" Then
                    'Cierra el detalle
                    Dim sqlDetalle As String = "UPDATE Scrap_detalle SET RFA_date_Close = GETDATE(), Status = 'CLOSE' WHERE Id_scrap = @Id_scrap" 'Si el Status es CLOSE se cierra el registro y se marca la fecha de cierre
                    Using cmd As New SqlCommand(sqlDetalle, conn)
                        cmd.Parameters.AddWithValue("@Id_scrap", model.Id_scrap)
                        Dim filasAfectadas As Integer = cmd.ExecuteNonQuery()
                        If filasAfectadas = 0 Then
                            Return Json(New With {.success = False, .message = "No se encontró el registro en Scrap_detalle para Cerrar"})
                        End If
                    End Using
                End If
            End Using

            Return Json(New With {.success = True, .message = "RFA actualizado correctamente"})

        Catch ex As Exception
            Dim usuario = If(Session("UsuarioAutenticado"), "Desconocido")
            Utilidades.RegistrarError($"[GuardarRFA] Usuario: {usuario} | Error: {ex.ToString()}")
            Return Json(New With {.success = False, .message = "Error al actualizar el RFA"})
        End Try
    End Function

    'GET: Home/Logout
    Function Logout() As ActionResult
        Session.Clear()
        Return RedirectToAction("Index")
    End Function

    Private Function BuscarOperador(empleadoID As String) As Empleados
        Dim operador As Empleados = Nothing
        Try
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim queryEmpleados As String = "SELECT EMPLEADOID, Nombre FROM CDatadb.dbo.Empleados WHERE EMPLEADOID = @EMPLEADOID"
                Using cmd As New SqlCommand(queryEmpleados, conn)
                    cmd.Parameters.AddWithValue("@EMPLEADOID", empleadoID)
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        operador = New Empleados() With {
                            .Nombre = reader("Nombre").ToString(),
                            .EMPLEADOID = Convert.ToInt32(reader("EMPLEADOID"))
                        }
                    End If
                End Using
            End Using
        Catch ex As Exception
            Utilidades.RegistrarError($"[BuacarOperador] Empleado ID: {empleadoID} | Error: {ex.ToString()}")
        End Try
        Return operador
    End Function

    <HttpGet>
    Function ExportarDetallesExcel(idScrap As Integer) As JsonResult
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
        Dim detallesScrap As New List(Of Object)()
        Dim accionesScrap As New List(Of Object)()

        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()

                '1. Obtener la primera tabla: Detalles Scrap
                Dim queryDetalles As String = "
                    SELECT
                        sh.Id_scrap, sd.Line, sd.Part_number, sd.Ccost, sd.Unit_cost, sd.Total_cost,
                        sd.CausaRaiz, sd.Defecto_Espanol, sd.HU, sd.Qty, sd.Batch,
                        sd.Categoria, sd.Tipo, sd.Shift, sd.Equipment
                    FROM Scrap_detalle sd
                    INNER JOIN Scrap_header sh ON sd.Id_scrap = sh.Id_scrap
                    WHERE sh.Id_Scrap = @idScrap"

                Using cmdDetalles As New SqlCommand(queryDetalles, conn)
                    cmdDetalles.Parameters.AddWithValue("@idScrap", idScrap)
                    'Usa un bloque Using para el DataReader asegurando que se cierre
                    Using readerDetalles As SqlDataReader = cmdDetalles.ExecuteReader()
                        While readerDetalles.Read()
                            detallesScrap.Add(New With {
                            .ScrapID = readerDetalles("Id_Scrap").ToString(),
                            .NumeroDeParte = readerDetalles("Part_number").ToString(),
                            .HU = If(IsDBNull(readerDetalles("HU")), "", readerDetalles("HU").ToString()),
                            .Cantidad = If(IsDBNull(readerDetalles("Qty")), 0, Convert.ToInt32(readerDetalles("Qty"))),
                            .Batch = If(IsDBNull(readerDetalles("Batch")), "", readerDetalles("Batch").ToString()),
                            .CostoUnitario = Convert.ToDecimal(readerDetalles("Unit_cost")),
                            .CostoTotal = Convert.ToDecimal(readerDetalles("Total_cost")),
                            .Linea = readerDetalles("Line").ToString(),
                            .Ccostos = If(IsDBNull(readerDetalles("Ccost")), "", readerDetalles("Ccost").ToString()),
                            .Defecto = If(IsDBNull(readerDetalles("Defecto_Espanol")), "", readerDetalles("Defecto_Espanol").ToString()),
                            .Categoria = If(IsDBNull(readerDetalles("Categoria")), "", readerDetalles("Categoria").ToString()),
                            .CausaRaiz = If(IsDBNull(readerDetalles("CausaRaiz")), "", readerDetalles("CausaRaiz").ToString()),
                            .Tipo = If(IsDBNull(readerDetalles("Tipo")), "", readerDetalles("Tipo").ToString()),
                            .Equipo = If(IsDBNull(readerDetalles("Equipment")), "", readerDetalles("Equipment").ToString()),
                            .Turno = If(IsDBNull(readerDetalles("Shift")), "", readerDetalles("Shift").ToString())
                        })
                        End While
                    End Using
                End Using

                '2. Obtener a egunda tabla: Acciones Scrap
                Dim queryAcciones As String = "
                         SELECT
                              sd.Line, sd.Part_number, sd.Batch, sd.CausaRaiz, sd.ACorrectiva,
                              sd.Quien, sd.Cuando
                         FROM Scrap_detalle sd
                         WHERE sd.Id_scrap = @idScrap AND sd.ACorrectiva IS NOT NULL" 'Se icluye la condicion de que solo exporte los registros que tengan datos

                Using cmdAcciones As New SqlCommand(queryAcciones, conn)
                    cmdAcciones.Parameters.AddWithValue("@idScrap", idScrap)
                    'Usa un nuevo bloque Using para el segundo DataReader
                    Using readerAcciones As SqlDataReader = cmdAcciones.ExecuteReader()
                        While readerAcciones.Read()
                            accionesScrap.Add(New With {
                            .Linea = readerAcciones("Line").ToString(),
                            .NumeroDeParte = readerAcciones("Part_number").ToString(),
                            .Batch = If(IsDBNull(readerAcciones("Batch")), "", readerAcciones("Batch").ToString()),
                            .CausaRaiz = If(IsDBNull(readerAcciones("CausaRaiz")), "", readerAcciones("CausaRaiz").ToString()),
                            .AccionCorrectiva = If(IsDBNull(readerAcciones("ACorrectiva")), "", readerAcciones("ACorrectiva").ToString()),
                            .Quien = If(IsDBNull(readerAcciones("Quien")), "", readerAcciones("Quien").ToString()),
                            .Cuando = If(IsDBNull(readerAcciones("Cuando")), Nothing, Convert.ToDateTime(readerAcciones("Cuando")))
                        })
                        End While
                    End Using
                End Using

            End Using

            'Devolvemos objeto JSON con ambas tablas
            Return Json(New With {.success = True, .detalles = detallesScrap, .acciones = accionesScrap}, JsonRequestBehavior.AllowGet)

        Catch ex As Exception
            'Manejo de errores
            Dim usuario = If(Session("UsuarioAutenticado"), "Desconocido")
            Utilidades.RegistrarError($"[ExportarDetallesExcel] Usuario: {usuario} | Error: {ex.ToString()}")
            Return Json(New With {.success = False, .message = "Error al obetner los datos a exportar"}, JsonRequestBehavior.AllowGet)
        End Try
    End Function
End Class
