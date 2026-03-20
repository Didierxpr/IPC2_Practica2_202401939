namespace Practica2_TurnosMedicos;

public class Turno
{
    public string Nombre { get; set; } = "";
    public int Edad { get; set; }
    public string Especialidad { get; set; } = "";

    // Tiempos en minutos
    public int TiempoServicioMin { get; set; }

    // Calculados al momento de entrar a la cola (asumiendo FIFO y tiempos fijos)
    public int TiempoEsperaMin { get; set; }
    public int TiempoTotalEstimadoMin { get; set; }
}

