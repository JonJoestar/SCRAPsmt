$(document).ready(function () {
    const themeToggle = $('#themeToggle');
    const themeIcon = $('#themeIcon');
    const patchNotes = $("#patchNotes");
    const patchPopup = $("#patchPopup");
    const patchHeader = $(".patch-header");
    const patchList = $("#patchList");
    let hideTimeout;
    const CLOSE_DELAY = 500; // Tiempo en milisegundos para cerrar el popup

    //Verifica si hay tema guardado en localStorage
    if (localStorage.getItem('theme') === 'dark') {
        $('body').addClass('dark-mode');
        themeIcon.removeClass('fa-sun').addClass('fa-moon');
    }

    //Click en el boton para cambio
    themeToggle.on('click', function () {
        $('body').toggleClass('dark-mode');

        if ($('body').hasClass('dark-mode')) {
            themeIcon.removeClass('fa-sun').addClass('fa-moon');
            localStorage.setItem('theme', 'dark');
        } else {
            themeIcon.removeClass('fa-moon').addClass('fa-sun');
            localStorage.setItem('theme', 'light');
        }
    });

    function openPopup() {
        clearTimeout(hideTimeout);
        patchPopup.addClass("is-open"); //solo abrir
        //no expandir
        patchPopup.removeClass("is-expanded");
    }

    function closePopup() {
        clearTimeout(hideTimeout);
        patchPopup.removeClass("is-open is-expanded");
    }

    //Accesible: click en el icono tambien lo abre
    patchNotes.on("click", openPopup);

    //Hover en el icono: abrir, mientras este encima
    patchNotes.on("mouseenter", openPopup);
    patchNotes.on("mouseleave", function () {
        //si no estoy sobre el popup, programa cierre
        hideTimeout = setTimeout(() => {
            if (!patchPopup.is(":hover")) closePopup();
        }, CLOSE_DELAY);
    });

    //Mantener abierto si el cursor esta sobre el popup
    patchPopup.on("mouseenter", function () {
        clearTimeout(hideTimeout); //Limpiar timeout si el cursor entra al popup
    });

    //Al salie del popup, se cierra con retraso(a menos que se vuelva al icono rapido)
    patchPopup.on("mouseleave", function () {
        hideTimeout = setTimeout(() => {
            if (!patchNotes.is(":hover")) closePopup();
        }, CLOSE_DELAY);
    });

    //Expandir/colapsar al hacer click en el header
    patchHeader.on("click", function () {
        patchPopup.toggleClass("is-expanded");
    });

    //Cerrar el popup si hacer click fuera de el
    $(document).on("click", function (e) {
        if (!patchPopup.is(e.target) && patchPopup.has(e.target).length === 0 &&
            !patchNotes.is(e.target) && patchNotes.has(e.target).length === 0) {
            closePopup(); //coierra lista si estaba expandida
        }
    });

    //Notas del pache
    $.getJSON(window.rutas.obtenerNotas, function (response) {
        if (response.success) {
            patchList.empty(); //Limpiar lista existente
            //Agregar cada nota a la lista
            response.data.forEach(nota => {
                patchList.append(`<li>✅ <b>${nota.Titulo}</b><br>${nota.Descripcion}</li>`);
            });
        }
    });
});