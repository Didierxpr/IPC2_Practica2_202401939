using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Practica2_TurnosMedicos;

public class Form1 : Form
{
    private readonly ColaTurnos _cola = new();

    private TextBox txtNombre = null!;
    private NumericUpDown numEdad = null!;
    private ComboBox cmbEspecialidad = null!;
    private Button btnRegistrar = null!;
    private Button btnAtender = null!;
    private Button btnLimpiar = null!;

    private DataGridView dgvCola = null!;
    private Label lblAtendido = null!;

    private PictureBox picGraphviz = null!;
    private TextBox txtDot = null!;

    public Form1()
    {
        Text = "Sistema de Turnos Médicos (FIFO)";
        Width = 1200;
        Height = 760;

        InicializarControles();
        ActualizarVista();
    }

    private void InicializarControles()
    {
        var panelEntrada = new Panel
        {
            Left = 10,
            Top = 10,
            Width = 420,
            Height = 220,
            BorderStyle = BorderStyle.FixedSingle
        };
        Controls.Add(panelEntrada);

        var lblNombre = new Label { Left = 10, Top = 20, Width = 120, Text = "Nombre:" };
        panelEntrada.Controls.Add(lblNombre);

        txtNombre = new TextBox { Left = 140, Top = 18, Width = 250 };
        panelEntrada.Controls.Add(txtNombre);

        var lblEdad = new Label { Left = 10, Top = 55, Width = 120, Text = "Edad:" };
        panelEntrada.Controls.Add(lblEdad);

        numEdad = new NumericUpDown
        {
            Left = 140,
            Top = 53,
            Width = 120,
            Minimum = 0,
            Maximum = 130
        };
        panelEntrada.Controls.Add(numEdad);

        var lblEspecialidad = new Label { Left = 10, Top = 90, Width = 120, Text = "Especialidad:" };
        panelEntrada.Controls.Add(lblEspecialidad);

        cmbEspecialidad = new ComboBox
        {
            Left = 140,
            Top = 88,
            Width = 250,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbEspecialidad.Items.AddRange(new object[]
        {
            "Medicina General",
            "Pediatria",
            "Ginecologia",
            "Dermatologia"
        });
        cmbEspecialidad.SelectedIndex = 0;
        panelEntrada.Controls.Add(cmbEspecialidad);

        btnRegistrar = new Button { Left = 40, Top = 130, Width = 150, Text = "Registrar Turno" };
        btnRegistrar.Click += BtnRegistrar_Click;
        panelEntrada.Controls.Add(btnRegistrar);

        btnAtender = new Button { Left = 210, Top = 130, Width = 150, Text = "Atender Siguiente" };
        btnAtender.Click += BtnAtender_Click;
        panelEntrada.Controls.Add(btnAtender);

        btnLimpiar = new Button { Left = 110, Top = 165, Width = 200, Text = "Limpiar Cola" };
        btnLimpiar.Click += BtnLimpiar_Click;
        panelEntrada.Controls.Add(btnLimpiar);

        lblAtendido = new Label
        {
            Left = 10,
            Top = 240,
            Width = 420,
            Height = 70,
            Text = "Atendido: (sin pacientes atendidos todavía)"
        };
        Controls.Add(lblAtendido);

        dgvCola = new DataGridView
        {
            Left = 10,
            Top = 320,
            Width = 820,
            Height = 380,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        Controls.Add(dgvCola);

        dgvCola.Columns.Add("Pos", "Posición");
        dgvCola.Columns.Add("Nombre", "Nombre");
        dgvCola.Columns.Add("Edad", "Edad");
        dgvCola.Columns.Add("Especialidad", "Especialidad");
        dgvCola.Columns.Add("Servicio", "Tiempo Servicio (min)");
        dgvCola.Columns.Add("Espera", "Tiempo Espera (min)");
        dgvCola.Columns.Add("Total", "Tiempo Total Estimado (min)");

        var panelGrafica = new Panel
        {
            Left = 840,
            Top = 10,
            Width = 330,
            Height = 520,
            BorderStyle = BorderStyle.FixedSingle
        };
        Controls.Add(panelGrafica);

        picGraphviz = new PictureBox
        {
            Left = 10,
            Top = 10,
            Width = 310,
            Height = 310,
            BorderStyle = BorderStyle.FixedSingle,
            SizeMode = PictureBoxSizeMode.Zoom
        };
        panelGrafica.Controls.Add(picGraphviz);

        txtDot = new TextBox
        {
            Left = 10,
            Top = 330,
            Width = 310,
            Height = 160,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true
        };
        panelGrafica.Controls.Add(txtDot);
    }

    private void BtnRegistrar_Click(object? sender, EventArgs e)
    {
        string nombre = txtNombre.Text.Trim();
        int edad = (int)numEdad.Value;
        string especialidad = cmbEspecialidad.SelectedItem?.ToString() ?? "";

        if (string.IsNullOrWhiteSpace(nombre))
        {
            MessageBox.Show("Ingrese el nombre del paciente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (edad < 0)
        {
            MessageBox.Show("La edad no puede ser negativa.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int tiempoServicio = ObtenerTiempoServicioMin(especialidad);

        var turno = new Turno
        {
            Nombre = nombre,
            Edad = edad,
            Especialidad = especialidad,
            TiempoServicioMin = tiempoServicio
        };

        _cola.Enqueue(turno);
        lblAtendido.Text = "Atendido: (sin cambios, solo se registro el turno)";
        ActualizarVista();
    }

    private void BtnAtender_Click(object? sender, EventArgs e)
    {
        Turno? atendido = _cola.Dequeue();
        if (atendido == null)
        {
            MessageBox.Show("No hay turnos pendientes.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            lblAtendido.Text = "Atendido: no hay turnos pendientes";
            ActualizarVista();
            return;
        }

        lblAtendido.Text =
            $"Atendido: {atendido.Nombre} | {atendido.Especialidad} | " +
            $"Servicio: {atendido.TiempoServicioMin} min | Total: {atendido.TiempoTotalEstimadoMin} min";

        ActualizarVista();
    }

    private void BtnLimpiar_Click(object? sender, EventArgs e)
    {
        _cola.Clear();
        lblAtendido.Text = "Atendido: (sin pacientes atendidos todavía)";
        ActualizarVista();
    }

    private int ObtenerTiempoServicioMin(string especialidad) =>
        especialidad switch
        {
            "Medicina General" => 10,
            "Pediatria" => 15,
            "Ginecologia" => 20,
            "Dermatologia" => 25,
            _ => throw new InvalidOperationException($"Especialidad no reconocida: {especialidad}")
        };

    private void ActualizarVista()
    {
        ActualizarGrid();
        GraficarCola();
    }

    private void ActualizarGrid()
    {
        dgvCola.Rows.Clear();

        var snapshot = _cola.ObtenerTurnosSnapshot();
        for (int i = 0; i < snapshot.Count; i++)
        {
            var t = snapshot[i];
            dgvCola.Rows.Add(
                (i + 1).ToString(),
                t.Nombre,
                t.Edad.ToString(),
                t.Especialidad,
                t.TiempoServicioMin.ToString(),
                t.TiempoEsperaMin.ToString(),
                t.TiempoTotalEstimadoMin.ToString()
            );
        }
    }

    private void GraficarCola()
    {
        // Construye el DOT con la cola actual.
        string dot = ConstruirDotCola();
        txtDot.Text = dot;

        // Genera imagen PNG si el ejecutable 'dot' está disponible.
        try
        {
            string tmpDir = Path.Combine(Path.GetTempPath(), "Practica2_TurnosMedicos");
            Directory.CreateDirectory(tmpDir);

            string dotPath = Path.Combine(tmpDir, "cola.dot");
            string pngPath = Path.Combine(tmpDir, "cola.png");

            File.WriteAllText(dotPath, dot, Encoding.UTF8);

            var psi = new ProcessStartInfo
            {
                FileName = "dot",
                Arguments = $"-Tpng \"{dotPath}\" -o \"{pngPath}\"",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            if (proc == null)
                throw new InvalidOperationException("No se pudo iniciar Graphviz 'dot'.");

            // Graphviz puede tardar más dependiendo del equipo.
            bool finished = proc.WaitForExit(15000);
            string stderr = proc.StandardError.ReadToEnd();
            string stdout = proc.StandardOutput.ReadToEnd();

            if (!finished)
            {
                try { proc.Kill(true); } catch { /* ignore */ }
                throw new TimeoutException("Tiempo agotado al ejecutar Graphviz 'dot'.");
            }

            if (proc.ExitCode != 0 || !File.Exists(pngPath))
            {
                var extra = new StringBuilder();
                extra.AppendLine($"ExitCode: {proc.ExitCode}");
                if (!string.IsNullOrWhiteSpace(stderr))
                    extra.AppendLine($"stderr: {stderr.Trim()}");
                if (!string.IsNullOrWhiteSpace(stdout))
                    extra.AppendLine($"stdout: {stdout.Trim()}");

                throw new InvalidOperationException("Graphviz 'dot' no generó la imagen.\n" + extra);
            }

            // Copiamos la imagen para evitar bloqueos del archivo.
            using var img = Image.FromFile(pngPath);
            picGraphviz.Image = (Image)img.Clone();
        }
        catch (Exception ex)
        {
            // Si Graphviz no está instalado o 'dot' no está en PATH, dejamos el DOT visible.
            // Además mostramos el error para que puedas corregir la configuración.
            txtDot.Text = dot + "\n\n=== ERROR Graphviz ===\n" + ex.Message;
            picGraphviz.Image = null;
        }
    }

    private string ConstruirDotCola()
    {
        var snapshot = _cola.ObtenerTurnosSnapshot();

        var sb = new StringBuilder();
        sb.AppendLine("digraph ColaTurnos {");
        sb.AppendLine("  rankdir=LR;");
        sb.AppendLine("  node [shape=box, style=rounded];");

        if (snapshot.Count == 0)
        {
            sb.AppendLine("  empty [label=\"Cola vacia\"];");
            sb.AppendLine("}");
            return sb.ToString();
        }

        for (int i = 0; i < snapshot.Count; i++)
        {
            var t = snapshot[i];
            string nodo = $"n{i}";
            string label = $"Pos {i + 1}\\n{EscaparLabelDot(t.Nombre)}\\n{EscaparLabelDot(t.Especialidad)}\\nTotal {t.TiempoTotalEstimadoMin} min";

            sb.AppendLine($"  {nodo} [label=\"{label}\"];");

            if (i > 0)
            {
                sb.AppendLine($"  n{i - 1} -> {nodo};");
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string EscaparLabelDot(string texto)
    {
        // Escapa comillas y reemplaza saltos de linea.
        return texto
            .Replace("\"", "\\\"")
            .Replace("\r", "")
            .Replace("\n", " ");
    }
}

