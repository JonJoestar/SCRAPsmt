//Detecta cierre de cualquier modal y limpia el foco
$(document).on('hidden.bs.modal', '.modal', function () {
    //Limpia el foco global, no importa donde este
    const active = document.activeElement;
    if (active && $(this).has(active).length > 0) {
        active.blur();
    }
});