// Obtener el elemento del control de fecha
var fechaInput = document.getElementById('fechaInput');

// Agregar un evento change al control de fecha
fechaInput.addEventListener('change', function () {
    // Obtener la fecha seleccionada
    var fechaSeleccionada = new Date(fechaInput.value);

    // Verificar si es fin de semana
    if (fechaSeleccionada.getDay() === 0 || fechaSeleccionada.getDay() === 6) {
        // Restablecer el valor del control de fecha
        fechaInput.value = '';
        alert('No se pueden seleccionar fechas en fines de semana.');
    }

    // Verificar si la hora está fuera del rango permitido
    var horaSeleccionada = fechaSeleccionada.getHours();
    var minutosSeleccionados = fechaSeleccionada.getMinutes();
    if (horaSeleccionada < 9 || horaSeleccionada > 14 || minutosSeleccionados % 30 !== 0) {
        // Restablecer la hora al valor mínimo permitido
        fechaSeleccionada.setHours(9);
        fechaSeleccionada.setMinutes(0);
        // Establecer el nuevo valor en el control de fecha
        fechaInput.value = fechaSeleccionada.toISOString().slice(0, 16);
        alert('La hora debe estar entre las 9 a.m. y las 2 p.m., en incrementos de media hora.');
    }
});