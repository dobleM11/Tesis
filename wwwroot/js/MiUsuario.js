function showAlert() {
    var result = confirm("¿Desea cerrar la sesión?");
    if (result) {
        // Redireccionar al controlador y acción especificados
        window.location.href = "/Auth/Logout";
    } else {
        // Cerrar el alert box sin realizar ninguna acción adicional
        return;
    }
}