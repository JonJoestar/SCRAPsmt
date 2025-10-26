Imports System.Web.Mvc
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Net.Configuration
Imports OfficeOpenXml
Imports System.IO
Imports System.IO.Packaging

Namespace Controllers
    ''' <summary>
    ''' Controlador encargado de la captura inicial, registro y edicion de scrap
    ''' contiene las cacciones para crear, guardar, editar y eliminar registros de scrap
    ''' </summary>
    ''' <remarks>Este controlado gestiona la vista principal de captura, la validacion y el guardado de los datos en Scrap_detalle y Scrap_header</remarks>
    Public Class AgregarScrapController
        Inherits Controller

        ''' <summary>
        ''' Accion principal que carga la vista de captura de scrap Index para el proceso de registro de scrap
        ''' </summary>
        ''' <returns>Retorna la vista Index</returns>
        Function Index() As ActionResult
            Return View()
        End Function

        ''' <summary>
        ''' Accion GET que carga el formulario de captura de scrap.
        ''' Carga las listas desplegables necesarias desde la base de datos (Linea, Turno, Numero de Parte, Equipo)
        ''' </summary>
        ''' <returns>Retorna una vista parcial con el formulario de creacion precargado con el modelo vacio</returns>
        Function Create() As ActionResult
            Try
                'Inicia un modelo vacio
                Dim model As New FormularioScrap()

                'Carga los datos iniciales
                ViewBag.Linea = ObtenerLineas()
                ViewBag.Turno = New List(Of String) From {"1A", "1B", "2A", "2B"}
                CargarAutorizadores()
                ViewBag.Equipos = ObtenerEquipos()

                'Carga la lista de los numeros de parte desde la base de datos
                Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
                Dim listaPartNum As New List(Of SelectListItem)()

                Using conn As New SqlConnection(connectionString)
                    conn.Open()
                    Dim query As String = "SELECT PartNum FROM Scrap_Partnumber"
                    Using cmd As New SqlCommand(query, conn)
                        Dim reader = cmd.ExecuteReader()
                        While reader.Read()
                            listaPartNum.Add(New SelectListItem With {
                                         .Text = reader("PartNum").ToString(),
                                         .Value = reader("PartNum").ToString()
                                         })
                        End While
                    End Using
                End Using

                ViewBag.PartNum = listaPartNum
                Return PartialView("Create", model)

            Catch ex As Exception
                Dim usuario = If(Session("UsuarioAutenticado"), "Desconocido")
                Utilidades.RegistrarError($"[Create - Formulario] Usuario: {usuario} | Error: {ex.ToString()}")
                Return Content("Ocurrio un Error al Cargar el Formulario")
            End Try
        End Function

        ''' <summary>
        ''' Accion POST que recibe y guarda los datos una nueva captura de scrap.
        ''' Valida los campos requeridos antes de guardar y posteriormente guarda los datos en la base de datos.
        ''' </summary>
        ''' <param name="formulario">El modelo <see cref="FormularioScrap"/> con los datos enviados desde la vista</param>
        ''' <returns>Retorna un <see cref="JsonResult"/>con el resultado de la operacion(exito o error) y el ID de scrap generado</returns>
        <HttpPost>
        Function Create(formulario As FormularioScrap) As JsonResult
            'If ModelState.IsValid Then
            Dim camposFaltantes As New List(Of String)

            If String.IsNullOrWhiteSpace(formulario.Line) Then camposFaltantes.Add("Linea")
            If String.IsNullOrWhiteSpace(formulario.Shift) Then camposFaltantes.Add("Turno")
            If String.IsNullOrWhiteSpace(formulario.Part_number) Then camposFaltantes.Add("Numero de Parte")
            If formulario.Qty <= 0 Then camposFaltantes.Add("Cantidad") 'valida que cantidad no sea 0
            If String.IsNullOrWhiteSpace(formulario.Defecto_Espanol) Then camposFaltantes.Add("Defecto")
            If String.IsNullOrWhiteSpace(formulario.Reference) Then camposFaltantes.Add("Referencia")
            If String.IsNullOrWhiteSpace(formulario.Tipo) Then camposFaltantes.Add("Tipo")
            If String.IsNullOrWhiteSpace(formulario.Categoria) Then camposFaltantes.Add("Categoria")
            If String.IsNullOrWhiteSpace(formulario.Ing_validacion) Then camposFaltantes.Add("Validacio de Ingenieria")
            If String.IsNullOrWhiteSpace(formulario.Equipment) Then camposFaltantes.Add("Equipo")
            'If String.IsNullOrWhiteSpace(formulario.HU) Then camposFaltantes.Add("HU")

            'Validacion en backend para HU y Batch
            If formulario.Tipo IsNot Nothing AndAlso formulario.Tipo.Trim().ToUpper() <> "PCB VIRGEN" Then
                If String.IsNullOrWhiteSpace(formulario.Batch) Then camposFaltantes.Add("Batch")
            End If

            If camposFaltantes.Count > 0 Then
                Return Json(New With {.success = False, .message = "Favor de completar los siguientes campos:<br><b>" & String.Join(", ", camposFaltantes) & "</b>"})
            End If

            Try
                'Captura el Id_scrap generado/reutilizado
                Dim idScrapUsado As Integer = GuardarEnBD(formulario)

                'Devuelve el Id_scrap en la respuesta
                Return Json(New With {.success = True, .message = "Registro guardado! Puedes seguir capturando mas registros", .idScrap = idScrapUsado})
            Catch ex As Exception
                'Manejo de error en GuardarBD
                Return Json(New With {.success = False, .message = "Error al intentar guardar: " & ex.Message})
            End Try
        End Function

        'Metodos auxiliares
        Private Function ObtenerLineas() As List(Of String)
            Dim lineas As New List(Of String)()
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim query As String = "Select DISTINCT Linea FROM Scrap_cft"
                Using cmd As New SqlCommand(query, conn)
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        lineas.Add(reader("Linea").ToString())
                    End While
                End Using
            End Using
            Return lineas
        End Function

        Private Function ObtenerAutorizadores() As List(Of String)
            Dim autorizadores As New List(Of String)()
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim query As String = "Select Nombre FROM Autorizadores"
                Using cmd As New SqlCommand(query, conn)
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        autorizadores.Add(reader("Nombre").ToString())
                    End While
                End Using
            End Using
            Return autorizadores
        End Function

        Private Function GuardarEnBD(formulario As FormularioScrap) As Integer '<--retorna integer
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString

            If String.IsNullOrEmpty(formulario.Line) OrElse String.IsNullOrEmpty(formulario.CFT) Then
                Throw New Exception("La Linea o el CFT no pueden estar vacios.")
            End If

            Dim weekActual As Integer = Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                DateTime.Now,
                Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday)

            Try
                'Verifica si hay un Id_scrap en sesion
                Dim idScrapGenerado As Integer

                If Session("IdScrapActual") Is Nothing Then
                    'Si no hay lo genera
                    idScrapGenerado = GuardarHistorial(Session("UsuarioNumero"), formulario.Line)
                    Session("IdScrapActual") = idScrapGenerado
                Else
                    'Si ya hay uno, lo reutiliza
                    idScrapGenerado = Convert.ToInt32(Session("IdScrapActual"))
                End If

                'Luego continua el guardado en Scrap_detalle usndo el Id_scrap generado
                Using conn As New SqlConnection(connectionString)
                    conn.Open()


                    'Realiza el calculo de Costo Total
                    Dim totalCost As Decimal = Convert.ToDecimal(formulario.Qty) * Convert.ToDecimal(formulario.Unit_cost)

                    Dim query As String = "INSERT INTO Scrap_detalle (Id_scrap, Line, CFT, Usuario, Date_time, Shift, Part_number, Batch, Qty, Unit_cost, Total_cost, Reference, Ccost, Equipment, Status, week, Ing_validacion, Defecto_Espanol, HU, Tipo, Categoria) " &
                                          "VALUES (@Id_scrap, @Line, @CFT, @Usuario, @Date_time, @Shift, @Part_number, @Batch, @Qty, @Unit_cost, @Total_cost, @Reference, @Ccost, @Equipment, @Status, @week, @Ing_validacion, @Defecto_Espanol, @HU, @Tipo, @Categoria)"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@Id_scrap", idScrapGenerado)
                        cmd.Parameters.AddWithValue("@Line", formulario.Line)
                        cmd.Parameters.AddWithValue("@CFT", formulario.CFT)
                        cmd.Parameters.AddWithValue("@Usuario", Session("UsuarioNumero"))
                        cmd.Parameters.AddWithValue("@Date_time", DateTime.Now)
                        cmd.Parameters.AddWithValue("@Shift", formulario.Shift)
                        cmd.Parameters.AddWithValue("@Part_number", formulario.Part_number)
                        If String.IsNullOrEmpty(formulario.Batch) Then
                            cmd.Parameters.AddWithValue("@Batch", "")
                        Else
                            cmd.Parameters.AddWithValue("@Batch", formulario.Batch)
                        End If
                        cmd.Parameters.AddWithValue("@Qty", formulario.Qty)
                        cmd.Parameters.AddWithValue("@Unit_cost", formulario.Unit_cost)
                        cmd.Parameters.AddWithValue("@Total_cost", totalCost)
                        cmd.Parameters.AddWithValue("@Reference", formulario.Reference)
                        cmd.Parameters.AddWithValue("@Ccost", formulario.Ccost)
                        cmd.Parameters.AddWithValue("@Equipment", formulario.Equipment)
                        cmd.Parameters.AddWithValue("@Status", "OPEN")
                        cmd.Parameters.AddWithValue("@week", weekActual)
                        cmd.Parameters.AddWithValue("@Ing_validacion", formulario.Ing_validacion)
                        cmd.Parameters.AddWithValue("@Defecto_Espanol", formulario.Defecto_Espanol)
                        cmd.Parameters.AddWithValue("@HU",
                        If(String.IsNullOrWhiteSpace(formulario.HU),
                        DBNull.Value, formulario.HU))
                        cmd.Parameters.AddWithValue("@Tipo", formulario.Tipo)
                        cmd.Parameters.AddWithValue("@Categoria", formulario.Categoria)
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

                Return idScrapGenerado '<--devolvemos el Id_scrap

            Catch ex As Exception
                Dim usuario = If(Session("UsuarioAutenticado"), "Desconocido")
                Utilidades.RegistrarError($"[GuardarEnBD] Usuario: {usuario} | Error: {ex.ToString()}")
                Throw New Exception("Error al guardar en la BD: " & ex.Message)
            End Try
        End Function

        'Metodos para AJAX
        <HttpGet>
        Public Function ObtenerCFTyCcostos(linea As String) As JsonResult
            Dim cftList As New List(Of String)()
            Dim Cc As String = ""
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString

            Try
                Using conn As New SqlConnection(connectionString)
                    conn.Open()

                    'Obtener CFT
                    Dim queryCFT As String = "SELECT CFT FROM Scrap_cft WHERE Linea = @Linea"
                    Using cmdCFT As New SqlCommand(queryCFT, conn)
                        cmdCFT.Parameters.AddWithValue("@Linea", linea)
                        ' Usa Using para asegurar el cierre del reader
                        Using readerCFT As SqlDataReader = cmdCFT.ExecuteReader()
                            While readerCFT.Read()
                                cftList.Add(readerCFT("CFT").ToString())
                            End While
                        End Using
                    End Using

                    'Obtener Ccosto
                    Dim queryCc As String = "SELECT SC.CCosto FROM Scrap_cft Scft INNER JOIN Scrap_ccosto SC ON Scft.Linea = SC.LineaAux WHERE Scft.Linea = @Linea"
                    Using cmdCc As New SqlCommand(queryCc, conn)
                        cmdCc.Parameters.AddWithValue("@Linea", linea)
                        Dim result = cmdCc.ExecuteScalar()
                        If result IsNot Nothing Then
                            Cc = result.ToString()
                        End If
                    End Using
                End Using
                'Devuelve respuesta en caso de exito
                Return Json(New With {.cft = cftList, .Cc = Cc}, JsonRequestBehavior.AllowGet)
            Catch ex As SqlException
                'Captura errores especificos en SQL
                Return Json(New With {.error = "Error en la consulta a SQL", .message = ex.Message}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                'Captura otros errores generales
                Dim usuario = If(Session("UsuarioAutenticado"), "Desconocido")
                Utilidades.RegistrarError($"[ObtenerCFTyCcostos] Usuario: {usuario} | Error: {ex.ToString()}")
                Return Json(New With {.success = False, .message = "Error inesperado al obtener la informacion"})
            End Try
        End Function

        Private Sub CargarAutorizadores()
            Dim lista As New List(Of SelectListItem)

            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString)
                conn.Open()
                Dim cmd As New SqlCommand("SELECT DISTINCT Nombre FROM Autorizadores ORDER BY Nombre", conn)
                Dim reader As SqlDataReader = cmd.ExecuteReader()

                While reader.Read()
                    lista.Add(New SelectListItem With {
                              .Value = reader("Nombre").ToString(),
                              .Text = reader("Nombre").ToString()
                              })
                End While
            End Using
            ViewBag.ListaValidacionING = lista
        End Sub

        Public Function ObtenerCosto(Partnum As String) As JsonResult
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Dim costo As Decimal = 0
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim query As String = "SELECT cost FROM Scrap_Partnumber WHERE PartNum = @PartNum"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@PartNum", Partnum)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then
                        costo = Convert.ToDecimal(result)
                    End If
                End Using
            End Using
            Return Json(New With {.costo = costo}, JsonRequestBehavior.AllowGet)
        End Function

        <HttpPost>
        Public Function ObtenerPartNumber(filtro As String) As JsonResult
            Dim lista As New List(Of SelectListItem)
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                'La consulta filtra por los primeros caracteres
                Dim query As String = "SELECT PartNum FROM Scrap_Partnumber WHERE PartNum LIKE @Filtro + '%'"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Filtro", filtro)
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            lista.Add(New SelectListItem With {
                                      .Value = reader("PartNum").ToString(),
                                      .Text = reader("PartNum").ToString()
                                      })
                        End While
                    End Using
                End Using
            End Using
            Return Json(lista, JsonRequestBehavior.AllowGet)
        End Function

        Public Function ObtenerDefectos() As JsonResult
            Dim defectos As New List(Of String)()
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim query As String = "SELECT Defecto_Espanol FROM Scrap_defectos"
                Using cmd As New SqlCommand(query, conn)
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        defectos.Add(reader("Defecto_Espanol").ToString())
                    End While
                End Using
            End Using
            Return Json(defectos, JsonRequestBehavior.AllowGet)
        End Function

        Private Function ObtenerEquipos() As List(Of SelectListItem)
            Dim lista As New List(Of SelectListItem)()
            Dim conectionString = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Using conn As New SqlConnection(conectionString)
                conn.Open()
                Dim query = "SELECT DISTINCT Maquina FROM DDowntime.dbo.TBL_Maquinas ORDER BY Maquina"
                Using cmd As New SqlCommand(query, conn)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            lista.Add(New SelectListItem With {
                                      .Value = reader("Maquina").ToString(),
                                      .Text = reader("Maquina").ToString()
                                      })
                        End While
                    End Using
                End Using
            End Using
            Return lista
        End Function

        Private Function GuardarHistorial(usuario As String, linea As String) As Integer
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Try
                Using conn As New SqlConnection(connectionString)
                    conn.Open()
                    Dim query As String = "INSERT INTO Scrap_header (Line, Usuario, Date_time) VALUES (@Line, @Usuario, GETDATE()); SELECT SCOPE_IDENTITY();"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@Usuario", Session("UsuarioNumero"))
                        cmd.Parameters.AddWithValue("@Line", linea)
                        Dim idGenerado As Object = cmd.ExecuteScalar()
                        Return Convert.ToInt32(idGenerado)
                    End Using
                End Using
            Catch ex As Exception
                Dim usuarioGuardar = If(Session("UsuarioAutenticado"), "Desconocido")
                Utilidades.RegistrarError($"[GuardarHistorial] Usuario: {usuarioGuardar} | Error: {ex.ToString()}")
                Throw New Exception("Error al guardar el historial de scrap: " & ex.Message)
            End Try
        End Function

        Function FinalizarCaptura() As JsonResult
            Session("IdScrapActual") = Nothing
            Return Json(New With {.success = True})
        End Function

        Function VisualizarDetalle(idScrap As Integer) As ActionResult
            Dim listaDetalles As New List(Of ScrapDetalle)
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString

            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim query As String = "SELECT * FROM Scrap_detalle WHERE Id_scrap = @Id_scrap"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Id_scrap", idScrap)
                    Dim reader = cmd.ExecuteReader()
                    While reader.Read()
                        listaDetalles.Add(New ScrapDetalle With {
                            .Id_scrap = reader("Id_scrap").ToString(),
                            .Line = reader("Line").ToString(),
                            .CFT = reader("CFT").ToString(),
                            .RFA = reader("RFA").ToString(),
                            .Part_number = reader("Part_number").ToString(),
                            .Unit_cost = reader("Unit_cost").ToString(),
                            .Total_cost = Convert.ToDecimal(reader("Total_cost")),
                            .Ccost = reader("Ccost").ToString(),
                            .Contencion = reader("Contencion").ToString(),
                            .ACorrectiva = reader("ACorrectiva").ToString(),
                            .CausaRaiz = reader("CausaRaiz").ToString(),
                            .Defecto_Espanol = reader("Defecto_Espanol").ToString(),
                            .Tabla = reader("id").ToString()
                        })
                    End While
                End Using
            End Using
            Return PartialView("_DetalleScrap", listaDetalles)
        End Function

        <HttpPost>
        Function SeleccionarFolio(idScrap As Integer?) As JsonResult
            Try
                'Valida que el Id_scrap no sea nulo o menor o igual a cero
                If idScrap Is Nothing OrElse idScrap.Value <= 0 Then '<--cambio: revisar por nothing y usar .value
                    Return Json(New With {.success = False, .message = "No se propociono un Folio valido para reabrir"})
                End If

                'Reutiliza la logica de sesion del controlador
                'Sobreescribe cualquier Id_scrap que pudiera estar activo
                Session("IdScrapActual") = idScrap.Value '<--usa .value para obtener el entero

                'Retorna exito. el js se encargara de abrir el modal
                Return Json(New With {.success = True, .message = $"El Folio {idScrap} ha sudo seleccionado para agregar registros"})
            Catch ex As Exception
                Utilidades.RegistrarError($"[SeleccionarFolio] Error al selecconar folio: {ex.ToString()}")
                Return Json(New With {.success = False, .message = "Errir al intentar reabrir el folio"})
            End Try
        End Function

        Function EditarDetalle(id As Integer) As ActionResult
            Try
                'se reutiliza la nueva funcion auxiliar para cargar el modelo
                Dim detalle = ObtenerDetalleOriginal(id)

                CargarAutorizadores()

                'se asigna el tipo de usuario
                ViewBag.TipoUsuario = Session("Tipo")

                'Devuelve al modal
                Return PartialView("_ScrapDetalleEditar", detalle)

            Catch ex As Exception
                Dim usuario = If(Session("UsuarioAutenticado"), "Desconocido")
                Utilidades.RegistrarError($"[EditarDetalle] Usuario: {usuario} | Error: {ex.ToString()}")
                Return Content("Error al cargar los detalles")
            End Try
        End Function

        Private Function ObtenerDetalleOriginal(idDetalle As Integer) As ScrapDetalleEditar
            Dim detalle As New ScrapDetalleEditar()
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString

            Try
                Using conn As New SqlConnection(connectionString)
                    conn.Open()
                    'Consulta que buscas todos los campos de un reistro por su ID individual
                    Dim query As String = "SELECT * FROM Scrap_detalle WHERE id = @id"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@id", idDetalle)
                        Dim reader = cmd.ExecuteReader()

                        If reader.Read() Then
                            'Mapeo de todos los campos del modelo ScrapDetalleEditar
                            detalle.Tabla = If(IsDBNull(reader("id")), 0, Convert.ToInt32(reader("id")))
                            detalle.Id_scrap = If(IsDBNull(reader("Id_scrap")), 0, Convert.ToInt32(reader("Id_scrap")))
                            detalle.Line = If(IsDBNull(reader("Line")), "", reader("Line").ToString())
                            detalle.CFT = If(IsDBNull(reader("CFT")), "", reader("CFT").ToString())
                            detalle.Shift = If(IsDBNull(reader("Shift")), "", reader("Shift").ToString())
                            detalle.Ccost = If(IsDBNull(reader("Ccost")), "", reader("Ccost").ToString())
                            detalle.Unit_cost = If(IsDBNull(reader("Unit_cost")), 0D, Convert.ToDecimal(reader("Unit_cost")))
                            detalle.Total_cost = If(IsDBNull(reader("Total_cost")), 0D, Convert.ToDecimal(reader("Total_cost")))
                            detalle.HU = If(IsDBNull(reader("HU")), "", reader("HU").ToString())
                            detalle.Reference = If(IsDBNull(reader("Reference")), "", reader("Reference").ToString())
                            detalle.Defecto_Espanol = If(IsDBNull(reader("Defecto_Espanol")), "", reader("Defecto_Espanol").ToString())
                            detalle.Equipment = If(IsDBNull(reader("Equipment")), "", reader("Equipment").ToString())
                            detalle.Part_number = If(IsDBNull(reader("Part_number")), "", reader("Part_number").ToString())
                            detalle.Batch = If(IsDBNull(reader("Batch")), "", reader("Batch").ToString())
                            detalle.Qty = If(IsDBNull(reader("Qty")), 0, Convert.ToInt32(reader("Qty")))
                            detalle.RFA = If(IsDBNull(reader("RFA")), "", reader("RFA").ToString())
                            detalle.Ing_validacion = If(IsDBNull(reader("Ing_validacion")), "", reader("Ing_validacion").ToString())

                            'Campos de edicion/validacion
                            detalle.Quien = If(IsDBNull(reader("Quien")), "", reader("Quien").ToString())
                            detalle.Cuando = If(IsDBNull(reader("Cuando")), Nothing, Convert.ToDateTime(reader("Cuando")))
                            detalle.Contencion = If(IsDBNull(reader("Contencion")), "", reader("Contencion").ToString())
                            detalle.CausaRaiz = If(IsDBNull(reader("CausaRaiz")), "", reader("CausaRaiz").ToString())
                            detalle.ACorrectiva = If(IsDBNull(reader("ACorrectiva")), "", reader("ACorrectiva").ToString())
                        End If
                    End Using
                End Using
            Catch ex As Exception
                Utilidades.RegistrarError($"[ObtenerDetallesOriginal] Error: {ex.ToString()}")
            End Try
            Return detalle
        End Function

        Function GuardarCambiosScrap(model As ScrapDetalleEditar) As JsonResult
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Dim tipoUsuario = Session("Tipo").ToString.ToUpper() 'se usa ToUpper para consistencia

            If tipoUsuario <> "ADMIN" AndAlso tipoUsuario <> "TECNICO" AndAlso tipoUsuario <> "SUPERVISOR" Then
                Return Json(New With {.success = False, .message = "No tienes permisos para editar este registro."})
            End If

            Try
                'Obtiene los valores originales para la proteccion del servidor
                Dim detalleOriginal = ObtenerDetalleOriginal(model.Tabla)

                'Aplicar la logica de permisos y revertir campos no autirizados
                If tipoUsuario = "TECNICO" Then
                    'revertir campos del Supervisor (HU, Quien y Cuando)
                    model.HU = detalleOriginal.HU
                    model.Quien = detalleOriginal.Quien
                    model.Cuando = detalleOriginal.Cuando

                ElseIf tipoUsuario = "SUPERVISOR" Then
                    'Revertir los campos del Tecnico(Contencion, Causa Raiz, ACorrectiva)
                    model.Contencion = detalleOriginal.Contencion
                    model.CausaRaiz = detalleOriginal.CausaRaiz
                    model.ACorrectiva = detalleOriginal.ACorrectiva

                    'Si es ADMIN no se hacen serversiones
                End If

                'Ejecuta la actualizacion con del modelo corregido
                Using conn As New SqlConnection(connectionString)
                    conn.Open()

                    Dim query As String = "UPDATE Scrap_detalle SET Contencion = @Contencion, CausaRaiz = @CausaRaiz, ACorrectiva = @ACorrectiva, Quien = @Quien, Cuando = @Cuando, HU = @HU WHERE id = @id"

                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@Contencion", model.Contencion)
                        cmd.Parameters.AddWithValue("@CausaRaiz", model.CausaRaiz)
                        cmd.Parameters.AddWithValue("@ACorrectiva", model.ACorrectiva)
                        cmd.Parameters.AddWithValue("@Quien", model.Quien)
                        cmd.Parameters.AddWithValue("@Cuando", If(model.Cuando.HasValue, model.Cuando.Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("HU", If(String.IsNullOrWhiteSpace(model.HU), DBNull.Value, model.HU))
                        cmd.Parameters.AddWithValue("@id", model.Tabla)

                        cmd.ExecuteNonQuery()
                    End Using
                End Using
                Return Json(New With {.success = True, .message = "Cambios guardados correctamente."})
            Catch ex As Exception
                Dim usuario = If(Session("UsuarioAutenticado"), "Desconocido")
                Utilidades.RegistrarError($"[GuardarCambiosScrap] Usuario: {usuario} | Error: {ex.ToString()}")
                Return Json(New With {.success = False, .message = "Ocurrio un error inesperado al guardar los cambios"})
            End Try
        End Function

        Function EliminarScrap(id As Integer) As JsonResult
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Smt_TraceContext").ConnectionString
            Dim tipoUsuario = Session("Tipo")

            If tipoUsuario <> "ADMIN" AndAlso tipoUsuario <> "TECNICO" AndAlso tipoUsuario <> "SUPERVISOR" Then
                Return Json(New With {.success = False, .message = "No tienes permisos para eliminar este registro."})
            End If
            Try
                Using conn As New SqlConnection(connectionString)
                    conn.Open()
                    Dim query As String = "DELETE FROM Scrap_detalle WHERE id = @id"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@id", id)
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
                Return Json(New With {.success = True, .message = "Registro eliminado correctamente."})
            Catch ex As Exception
                Dim usuario = If(Session("UsuarioAutenticado"), "Desconocido")
                Utilidades.RegistrarError($"[EliminarScrap] Usuario: {usuario} | Error: {ex.ToString()}")
                Return Json(New With {.success = False, .message = "Error al eliminar"})
            End Try
        End Function
    End Class
End Namespace