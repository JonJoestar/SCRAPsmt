<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewBag.Title - Registro de SCRAP</title>

    <!--CSS-->
    <!--Bootstrap css-->
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <!--DataTable CSS-->
    <link rel="stylesheet" type="text/css" href="~/Content/datatables.min.css">
    <!--Estilos SweetAlert-->
    <link href="~/Content/sweetalert2.min.css" rel="stylesheet" />
    <!--Jquery UI-->
    <link href="~/Content/jquery-ui/jquery-ui.css" rel="stylesheet" />
    <!--Font Awesome-->
    <link href="~/Content/FontAwesome/css/all.min.css" rel="stylesheet" />
    <!--CSS-->
    <link href="~/Content/Site.css" rel="stylesheet" />
</head>
<body>

    @RenderBody()

    <!--JS - jQuery-->
    <!--jQuery-->
    <script src="~/Scripts/jquery-3.6.0.min.js"></script>

    <!--Jquery UI-->
    <script src="~/Scripts/jquery-ui.min.js"></script>

    <script src="~/Scripts/popper.min.js"></script>

    <script src="~/Scripts/bootstrap.bundle.min.js"></script>

    <!--Script-->
    <script src="~/Scripts/sweetalert2.min.js"></script>

    <!--DataTable JS-->
    <script type="text/javascript" src="~/Scripts/DataTables/datatables.min.js"></script>

    <!--Cambio de tema-->
    <script src="~/Scripts/CambiarTemaPagina.js"></script>

    <!--Otros Bunldes-->
    @Scripts.Render("~/bundles/modernizr")
    @Scripts.Render("~/bundles/bootstrap")

    <!--Scripts de la vista-->
    @RenderSection("scripts", required:=False)

    <!--Toast de bienvenida-->
    @If Session("MostrarToast") IsNot Nothing AndAlso Session("MostrarToast").ToString() = "SI" Then
        @<script>
            const Toast = Swal.mixin({
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.onmouseenter = Swal.stopTimer;
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });

            Toast.fire({
                icon: "success",
                title: 'Bienvenido @Session("UsuarioAutenticado")'
            });
        </script>
        @Code
            Session.Remove("MostrarToast")
        End Code
    End If

    <!--Cambio de modo de visualizacion-->
    <div id="themeToggle" class="theme-toggle">
        <i id="themeIcon" class="fa-solid fa-sun"></i>
    </div>

    <!--Notas de actualizacion-->
    <div id="patchNotes" class="patch-notes">
        <i class="fa-solid fa-clipboard-list"></i>
    </div>

    <!--Popup de notas-->
    <div id="patchPopup" class="patch-popup">
        <div class="patch-header">📌 Notas de actualización</div>
        <ul id="patchList"></ul>
    </div>

    <!-- Spinner global centrado -->
    <!-- Spinner Global -->
    <div id="spinnerGlobal" class="position-fixed top-0 start-0 w-100 h-100 bg-white bg-opacity-75 d-flex justify-content-center align-items-center d-none" style="z-index: 2000;">
        <div class="spinner-border text-primary" style="width: 4rem; height: 4rem;" role="status">
            <span class="visually-hidden">Cargando...</span>
        </div>
    </div>
</body>
</html>