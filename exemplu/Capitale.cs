using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ExempluControale
{
    public partial class Capitale : Form
    {
        private List<Tara> tari = new List<Tara>();
        private readonly Random rnd = new Random();

        private int indexGalerie;

        private int scor;
        private int vieti;
        private int intrebariPuse;
        private List<Tara> ramase = new List<Tara>();
        private Tara curenta;
        private bool rundaPornita = false;

        public Capitale()
        {
            InitializeComponent();
        }

        private void Capitale_Load(object sender, EventArgs e)
        {
            string cale = Path.Combine(Application.StartupPath, "tari.txt");
            List<string> erori = new List<string>();
            tari = Date.CitesteTari(cale, erori);
            if (erori.Count > 0)
            {
                MessageBox.Show(string.Join(Environment.NewLine, erori), "Capitale",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            cmbContinent.SelectedIndex = 0;
            dtpData.Value = DateTime.Today;
            trkIntrebari.Maximum = Math.Max(1, tari.Count);
            lblIntrebari.Text = "Intrebari pe runda: " + trkIntrebari.Value;

            indexGalerie = 0;
            AfiseazaGalerie();
            ActualizeazaStare();
        }

        private Image Steag(Tara tara)
        {
            string cale = Path.Combine(Application.StartupPath, "steaguri", tara.Steag);
            if (!File.Exists(cale))
            {
                return null;
            }
            return Image.FromFile(cale);
        }

        private void AfiseazaGalerie()
        {
            bool areTari = tari.Count > 0;
            btnInapoi.Enabled = areTari && indexGalerie > 0;
            btnInainte.Enabled = areTari && indexGalerie < tari.Count - 1;
            prgGalerie.Maximum = Math.Max(1, tari.Count);
            if (!areTari)
            {
                lblTara.Text = "Nu exista tari.";
                picGalerie.Image = null;
                prgGalerie.Value = 0;
                return;
            }

            Tara tara = tari[indexGalerie];
            picGalerie.Image = Steag(tara);
            lblTara.Text = (indexGalerie + 1) + " / " + tari.Count + " - " + tara.Nume + ", capitala " + tara.Capitala;
            prgGalerie.Value = indexGalerie + 1;
        }

        private void btnInapoi_Click(object sender, EventArgs e)
        {
            indexGalerie--;
            AfiseazaGalerie();
        }

        private void btnInainte_Click(object sender, EventArgs e)
        {
            indexGalerie++;
            AfiseazaGalerie();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (tari.Count < 3)
            {
                MessageBox.Show("Sunt necesare cel putin 3 tari.", "Quiz",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            scor = 0;
            vieti = 3;
            intrebariPuse = 0;
            ramase = new List<Tara>(tari);
            rundaPornita = true;
            btnStart.Enabled = false;
            ActualizeazaStare();
            IntrebareNoua();
        }

        private void IntrebareNoua()
        {
            int i = rnd.Next(0, ramase.Count);
            curenta = ramase[i];
            ramase.RemoveAt(i);
            intrebariPuse++;

            List<string> variante = tari
                .Where(t => t.Capitala != curenta.Capitala)
                .OrderBy(t => rnd.Next())
                .Take(2)
                .Select(t => t.Capitala)
                .ToList();
            variante.Insert(rnd.Next(0, 3), curenta.Capitala);

            lblIntrebare.Text = "Intrebarea " + intrebariPuse + ": care este capitala tarii " + curenta.Nume + "?";
            rdbA.Text = variante[0];
            rdbB.Text = variante[1];
            rdbC.Text = variante[2];
            rdbA.Checked = false;
            rdbB.Checked = false;
            rdbC.Checked = false;
            grpVariante.Enabled = true;
            btnRaspunde.Enabled = false;
            btnUrmatoarea.Enabled = false;
        }

        private void Varianta_CheckedChanged(object sender, EventArgs e)
        {
            btnRaspunde.Enabled = rdbA.Checked || rdbB.Checked || rdbC.Checked;
        }

        private void btnRaspunde_Click(object sender, EventArgs e)
        {
            RadioButton ales = rdbA.Checked ? rdbA : rdbB.Checked ? rdbB : rdbC;
            if (ales.Text == curenta.Capitala)
            {
                scor++;
                MessageBox.Show("Corect!", "Quiz", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                vieti--;
                MessageBox.Show("Gresit! Capitala este " + curenta.Capitala + ".", "Quiz",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            grpVariante.Enabled = false;
            btnRaspunde.Enabled = false;
            ActualizeazaStare();

            if (vieti == 0 || intrebariPuse == trkIntrebari.Value || ramase.Count == 0)
            {
                MessageBox.Show("Joc terminat. Scor: " + scor + ".", "Quiz",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnStart.Enabled = true;
                return;
            }
            btnUrmatoarea.Enabled = true;
        }

        private void btnUrmatoarea_Click(object sender, EventArgs e)
        {
            IntrebareNoua();
        }

        private void ActualizeazaStare()
        {
            lblScor.Text = "Scor: " + scor;
            lblVieti.Text = "Vieti: " + vieti;
        }

        private void cmbContinent_SelectedIndexChanged(object sender, EventArgs e)
        {
            string continent = cmbContinent.SelectedItem.ToString();
            List<Tara> filtrate = continent == "Toate"
                ? tari
                : tari.Where(t => t.Continent == continent).ToList();

            cmbTara.Items.Clear();
            cmbTara.Items.Add("Selecteaza o tara");
            foreach (Tara t in filtrate)
            {
                cmbTara.Items.Add(t.Nume);
            }
            cmbTara.SelectedIndex = 0;
        }

        private void cmbTara_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTara.SelectedIndex <= 0)
            {
                picTara.Image = null;
                return;
            }
            Tara tara = tari.FirstOrDefault(t => t.Nume == cmbTara.SelectedItem.ToString());
            picTara.Image = tara == null ? null : Steag(tara);
        }

        private void trkIntrebari_ValueChanged(object sender, EventArgs e)
        {
            lblIntrebari.Text = "Intrebari pe runda: " + trkIntrebari.Value;
        }

        private void mnuGalerie_Click(object sender, EventArgs e)
        {
            tabPagini.SelectedTab = tabGalerie;
        }

        private void mnuQuiz_Click(object sender, EventArgs e)
        {
            tabPagini.SelectedTab = tabQuiz;
        }

        private void mnuSetari_Click(object sender, EventArgs e)
        {
            tabPagini.SelectedTab = tabSetari;
        }

        private void mnuRezultat_Click(object sender, EventArgs e)
        {
            if (!rundaPornita)
            {
                MessageBox.Show("Porneste intai o runda!", "Quiz",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("Scor: " + scor + ", vieti: " + vieti + ".", "Rezultat",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnuIesire_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mnuDespre_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Capitale - exemplu pentru lectia 5.", "Despre",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
