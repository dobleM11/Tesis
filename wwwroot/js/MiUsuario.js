function showAlert() {
    var result = confirm("�Desea cerrar la sesi�n?");
    if (result) {
        // Redireccionar al controlador y acci�n especificados
        window.location.href = "/Auth/Logout";
    } else {
        // Cerrar el alert box sin realizar ninguna acci�n adicional
        return;
    }
}