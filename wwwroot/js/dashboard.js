document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('btnIr').addEventListener('click', function () {
        var fecha = document.getElementById('fecha').value;
        fetch(`https://localhost:5001/Asistencia/GetAsistenciaByDate?fecha=${fecha}`)
            .then(response => response.json())
            .then(data => {
                var tbody = document.getElementById('tabla-body');
                tbody.innerHTML = '';
                data.forEach(item => {
                    var row = `<tr>
                                <td>${item.hora}</td>
                                <td>${item.nombre}</td>
                                <td>${item.rut}</td>
                                <td>${item.tramite}</td>
                                <td>${item.asistencia}</td>
                            </tr>`;
                    tbody.innerHTML += row;
                });
            })
            .catch(error => console.error('Error:', error));
    });
});