var contrasenaInput = document.getElementById("contrasena");
var iconoOjo = document.getElementById("iconoOjo");
var iconoOjoCerrado = document.getElementById("iconoOjoCerrado");

function mostrarContrasena() {
    if (contrasenaInput.type === "password") {
        contrasenaInput.type = "text";
        iconoOjo.style.display = "inline";
        iconoOjoCerrado.style.display = "none";
    } else {
        contrasenaInput.type = "password";
        iconoOjo.style.display = "none";
        iconoOjoCerrado.style.display = "inline";
    }
}

function formatRun(input) {
    // Obtén el valor ingresado por el usuario
    var run = input.value;

    // Elimina cualquier caracter que no sea un número o una "k" (en mayúscula o minúscula)
    if (run.length < 8 || run.length>12) {
        run = run.replace(/[^0-9]/g, '');
    } else {
        run = run.replace(/[^0-9kK]/g, '');
    }
    // Verifica si el último valor es una "k" (en mayúscula o minúscula) en la posición 11 o 12
    if (run.length >= 11) {
        var lastChar = run.charAt(run.length - 1);
        if (lastChar.toLowerCase() === 'k' && (run.length === 11 || run.length === 12)) {
            run = run.substring(0, run.length - 1) + 'K';
        }
    }

    // Aplica el formato x.xxx.xxx-x o xx.xxx.xxx-x
    if (run.length >= 8) {
        run = run.substring(0, run.length - 7) + '.' + run.substring(run.length - 7, run.length - 4) + '.' + run.substring(run.length - 4, run.length - 1) + '-' + run.substring(run.length - 1);
    }
    else if (run.length >= 2) {
        run = run.substring(0, run.length - 1) + '-' + run.substring(run.length - 1);
    }

    // Asigna el valor formateado al campo de texto
    input.value = run;
}