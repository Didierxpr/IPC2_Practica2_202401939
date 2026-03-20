using System.Collections.Generic;
using System.Linq;

namespace Practica2_TurnosMedicos;

/// <summary>
/// Implementa una cola dinámica FIFO usando solo List<T> (sin usar Queue<T> nativa).
/// </summary>
public class ColaTurnos
{
    private readonly List<Turno> _turnos = new();
    private int _headIndex = 0;

    public int Count => _turnos.Count - _headIndex;
    public bool IsEmpty => Count == 0;

    public int TiempoServicioPendiente()
    {
        int sum = 0;
        for (int i = _headIndex; i < _turnos.Count; i++)
        {
            sum += _turnos[i].TiempoServicioMin;
        }
        return sum;
    }

    public void Enqueue(Turno turno)
    {
        int espera = TiempoServicioPendiente();
        turno.TiempoEsperaMin = espera;
        turno.TiempoTotalEstimadoMin = espera + turno.TiempoServicioMin;
        _turnos.Add(turno);
    }

    public Turno? Dequeue()
    {
        if (IsEmpty) return null;

        Turno turno = _turnos[_headIndex];
        _headIndex++;
        CompactarSiHaceFalta();
        return turno;
    }

    public void Clear()
    {
        _turnos.Clear();
        _headIndex = 0;
    }

    public IEnumerable<Turno> EnumerarTurnos()
    {
        for (int i = _headIndex; i < _turnos.Count; i++)
            yield return _turnos[i];
    }

    private void CompactarSiHaceFalta()
    {
        // Evita que _headIndex crezca indefinidamente.
        if (_headIndex == 0) return;

        // Umbral simple: si ya se atendió “bastante” o la proporción es alta, compactar.
        if (_headIndex >= 64 && _headIndex * 2 >= _turnos.Count)
        {
            _turnos.RemoveRange(0, _headIndex);
            _headIndex = 0;
        }
    }

    public IReadOnlyList<Turno> ObtenerTurnosSnapshot()
    {
        if (_headIndex == 0) return _turnos;
        return _turnos.Skip(_headIndex).ToList();
    }
}

