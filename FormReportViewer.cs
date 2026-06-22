using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using CrystalDecisions.Windows.Forms;

namespace ZAKATFFA
{
    public partial class FormReportViewer : Form
    {
        string connectionString = "Server=LAPTOP-1QL3V291\\PAW;Database=zakatdb;Trusted_Connection=True;";
        private DataTable dtReport = new DataTable();
        private DataSet dsReport = new DataSet();
        private int halaman = 0;
        private int barisAwal = 0;

        public FormReportViewer()
        {
            InitializeComponent();
        }

        private void FormReportViewer_Load(object sender, EventArgs e)
        {
            TerapkanStyling();
            crystalReportViewer1.BackColor = SystemColors.Control;
            MuatData();
            TampilkanReport();
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // STYLING — disamakan persis dengan Form2.cs > TerapkanStyling()
        // Palet warna:
        //   Form background  : (0, 100, 0)   hijau tua
        //   Panel toolbar    : (0, 80, 0)    hijau gelap
        //   Label            : ForeColor White, Font Arial 9 Bold
        //   Tombol utama     : (0, 128, 0)   hijau medium
        //   Tombol Tutup     : (70, 70, 70)  abu-abu
        //   DataGridView     : sama dengan Form2
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void TampilkanReport()
        {
            try
            {
                string reportPath = Path.Combine(Application.StartupPath, "ReportPembayaranZakat.rpt");

                if (!File.Exists(reportPath))
                {
                    MessageBox.Show($"File tidak ditemukan:\n{reportPath}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dsReport == null || dsReport.Tables.Count == 0)
                {
                    MessageBox.Show("Tidak ada data.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ReportDocument rd = new ReportDocument();
                rd.Load(reportPath);

                // Push DataSet — bukan DataTable
                rd.SetDataSource(dsReport);

                crystalReportViewer1.ReportSource = rd;
                crystalReportViewer1.Refresh();
                crystalReportViewer1.BringToFront();
            }
            catch (Exception ex)
            {
                string msg = "Level 0: " + ex.Message;
                if (ex.InnerException != null)
                    msg += "\n\nLevel 1: " + ex.InnerException.Message;
                MessageBox.Show(msg, "Error Cetak", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TerapkanStyling()
        {
            // --- Form ---
            this.BackColor = Color.FromArgb(0, 100, 0);

            // --- Panel Toolbar ---
            panelToolbar.BackColor = Color.FromArgb(0, 80, 0);

            // --- Semua Label di panel toolbar ---
            foreach (Control c in panelToolbar.Controls)
            {
                if (c is Label lbl)
                {
                    lbl.ForeColor = Color.White;
                    lbl.Font = new Font("Arial", 9, FontStyle.Bold);
                }
            }

            // --- Label Status (di luar panel) ---
            lblStatus.ForeColor = Color.LightGreen;
            lblStatus.Font = new Font("Arial", 8);
            lblStatus.BackColor = Color.Transparent;

            // --- Tombol ---
            StyleButton(btnTampilkanReport, Color.FromArgb(0, 128, 0));
            StyleButton(btnCetak, Color.FromArgb(0, 128, 0));  // btnCetak
            StyleButton(btnTutup, Color.FromArgb(70, 70, 70));

            // --- DataGridView ---
            dataGridView1.BackgroundColor = Color.FromArgb(0, 80, 0);
            dataGridView1.GridColor = Color.LightGreen;
            dataGridView1.BorderStyle = BorderStyle.FixedSingle;

            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 60, 0);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);

            dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(240, 255, 240);
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Font = new Font("Arial", 9);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 150, 0);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(210, 240, 210);
        }

        private void StyleButton(Button btn, Color color)
        {
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Arial", 9, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(0, 60, 0);
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // MUAT DATA
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        private void MuatData()
        {
            try
            {
                using (SqlConnection kon = new SqlConnection(connectionString))
                {
                    kon.Open();

                    // Ambil tabel muzakki
                    SqlDataAdapter daMuzakki = new SqlDataAdapter(
                        "SELECT * FROM muzakki", kon);
                    DataTable dtMuzakki = new DataTable("muzakki");
                    daMuzakki.Fill(dtMuzakki);

                    // Ambil tabel pembayaran_zakat
                    SqlDataAdapter daPembayaran = new SqlDataAdapter(
                        "SELECT * FROM pembayaran_zakat ORDER BY tanggal DESC", kon);
                    DataTable dtPembayaran = new DataTable("pembayaran_zakat");
                    daPembayaran.Fill(dtPembayaran);

                    // Masukkan ke DataSet
                    dsReport = new DataSet("zakatdbDataSet");
                    dsReport.Tables.Add(dtMuzakki);
                    dsReport.Tables.Add(dtPembayaran);
                }

                // Update label status
                lblStatus.Text = $"Total data: {dsReport.Tables["pembayaran_zakat"].Rows.Count} transaksi";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error MuatData: " + ex.Message);
            }
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // KONFIGURASI KOLOM DATAGRIDVIEW
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        private void KonfigurasiKolomGrid()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;

            void Kolom(string name, string header, int weight, bool visible = true)
            {
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = name,
                    HeaderText = header,
                    DataPropertyName = name,
                    FillWeight = weight,
                    Visible = visible
                });
            }

            Kolom("id_pembayaran", "ID Bayar", 3, false);
            Kolom("id_muzakki", "ID Muzakki", 3, false);
            Kolom("nama", "Nama", 18);
            Kolom("alamat", "Alamat", 22);
            Kolom("no_hp", "No. HP", 13);
            Kolom("tanggal", "Tanggal", 10);
            Kolom("jumlah_jiwa", "Jiwa", 6);
            Kolom("jenis_pembayaran", "Jenis", 8);
            Kolom("total_bayar", "Total Bayar", 12);
            Kolom("jumlah_dengan_satuan", "Keterangan", 14);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // TOMBOL CETAK — tampilkan Crystal Report
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        private void button1_Click(object sender, EventArgs e)
        {
            TampilkanReport();
        }

        private void CetakHalaman(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            float y = 40;
            int barisDicetak = 0;
            int maxBarisPerHalaman = 25;
            halaman++;

            Font fontJudul = new Font("Arial", 14, FontStyle.Bold);
            Font fontHeader = new Font("Arial", 9, FontStyle.Bold);
            Font fontIsi = new Font("Arial", 8);
            Font fontFooter = new Font("Arial", 8, FontStyle.Italic);

            // ===== HEADER =====
            g.DrawString("LAPORAN PEMBAYARAN ZAKAT FITRAH", fontJudul, Brushes.DarkGreen,
                new PointF(e.PageBounds.Width / 2 - 180, y));
            y += 25;
            g.DrawString("Majelis Ulama Indonesia - Wonosobo", fontHeader, Brushes.Black,
                new PointF(e.PageBounds.Width / 2 - 120, y));
            y += 20;
            g.DrawString($"Dicetak: {DateTime.Now:dd MMMM yyyy HH:mm}    Halaman {halaman}",
                fontIsi, Brushes.Gray, new PointF(e.PageBounds.Width / 2 - 120, y));
            y += 15;
            g.DrawLine(Pens.DarkGreen, 40, y, e.PageBounds.Width - 40, y);
            y += 10;

            // ===== HEADER TABEL =====
            float[] xKol = { 40, 90, 220, 420, 510, 570, 650, 760 };
            string[] namaKol = { "No", "Nama", "Alamat", "No HP", "Tanggal", "Jiwa", "Jenis", "Total" };

            g.FillRectangle(Brushes.DarkGreen,
                new RectangleF(40, y, e.PageBounds.Width - 80, 20));

            for (int k = 0; k < namaKol.Length; k++)
                g.DrawString(namaKol[k], fontHeader, Brushes.White, new PointF(xKol[k], y + 3));
            y += 22;

            // ===== ISI TABEL =====
            decimal totalBayar = 0;
            int noUrut = barisAwal + 1;

            for (int i = barisAwal; i < dtReport.Rows.Count; i++)
            {
                if (barisDicetak >= maxBarisPerHalaman) break;

                DataRow r = dtReport.Rows[i];
                Brush warnaBaris = (barisDicetak % 2 == 0)
                    ? Brushes.White
                    : new SolidBrush(Color.FromArgb(230, 255, 230));

                g.FillRectangle(warnaBaris, new RectangleF(40, y, e.PageBounds.Width - 80, 18));

                g.DrawString(noUrut.ToString(), fontIsi, Brushes.Black, xKol[0], y + 2);
                g.DrawString(r["nama"].ToString(), fontIsi, Brushes.Black, xKol[1], y + 2);
                g.DrawString(PotongTeks(r["alamat"].ToString(), 28), fontIsi, Brushes.Black, xKol[2], y + 2);
                g.DrawString(r["no_hp"].ToString(), fontIsi, Brushes.Black, xKol[3], y + 2);
                g.DrawString(Convert.ToDateTime(r["tanggal"]).ToString("dd/MM/yy"), fontIsi, Brushes.Black, xKol[4], y + 2);
                g.DrawString(r["jumlah_jiwa"].ToString(), fontIsi, Brushes.Black, xKol[5], y + 2);
                g.DrawString(r["jenis_pembayaran"].ToString(), fontIsi, Brushes.Black, xKol[6], y + 2);
                g.DrawString(Convert.ToDecimal(r["total_bayar"]).ToString("N0"), fontIsi, Brushes.Black, xKol[7], y + 2);

                totalBayar += Convert.ToDecimal(r["total_bayar"]);
                y += 18;
                barisDicetak++;
                noUrut++;
                barisAwal = i + 1;
            }

            // ===== GARIS BAWAH =====
            g.DrawLine(Pens.DarkGreen, 40, y, e.PageBounds.Width - 40, y);
            y += 10;

            // ===== FOOTER TOTAL =====
            if (barisAwal >= dtReport.Rows.Count)
            {
                g.DrawString($"TOTAL KESELURUHAN: Rp {totalBayar:N0}",
                    fontHeader, Brushes.DarkGreen, new PointF(xKol[6], y));
                y += 20;
                g.DrawString("** Laporan ini digenerate otomatis oleh Aplikasi Zakat Fitrah **",
                    fontFooter, Brushes.Gray, new PointF(40, y));
            }

            e.HasMorePages = (barisAwal < dtReport.Rows.Count);
        }

        private string PotongTeks(string teks, int maxChar)
        {
            return teks.Length > maxChar ? teks.Substring(0, maxChar) + "..." : teks;
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblStatus_Click(object sender, EventArgs e) { }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void crystalReportViewer1_Load(object sender, EventArgs e) { }

        private void panelToolbar_Paint(object sender, PaintEventArgs e) { }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e) { }
    }
}