using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ClosedXML.Excel;


namespace ZAKATFFA
{
    public partial class FormImportExcel : Form
    {
        // Sama persis dengan connection string di Form1/Form2.
        // (Lihat catatan di README/USER_NOTES soal pemindahan ke laptop client)
        string connectionString = "Server=LAPTOP-1QL3V291\\PAW;Database=zakatdb;User Id=sa;Password=123;TrustServerCertificate=True;";

        // DataTable hasil baca file Excel, dipakai untuk preview di dgvPreview
        // dan sebagai sumber data saat proses import baris-per-baris.
        private DataTable dtPreview;

        public FormImportExcel()
        {
            InitializeComponent();
        }

        // =====================================================================
        // TOMBOL: UNDUH TEMPLATE
        // =====================================================================
        // Membuat file Excel kosong dengan header kolom yang benar, supaya
        // pengguna (panitia masjid) tidak salah format saat mengisi data.
        private void btnUnduhTemplate_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Files|*.xlsx";
                sfd.FileName = "Template_Import_Pembayaran_Zakat.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Pembayaran Zakat");

                            ws.Column("C").Style.NumberFormat.Format = "@";

                            string[] headers = { "nama", "alamat", "no_hp", "tanggal", "jumlah_jiwa", "jenis_pembayaran", "total_bayar" };
                            for (int i = 0; i < headers.Length; i++)
                                ws.Cell(1, i + 1).Value = headers[i];

                            ws.Row(1).Style.Font.Bold = true;

                            // Baris contoh supaya format jelas
                            ws.Cell(2, 1).Value = "Ahmad Fauzi";
                            ws.Cell(2, 2).Value = "Jl. Mawar No.1, Wonosobo";
                            ws.Cell(2, 3).Style.NumberFormat.Format = "@";
                            ws.Cell(2, 3).Value = "+6281234567890";
                            ws.Cell(2, 4).Value = DateTime.Today.ToString("yyyy-MM-dd");
                            ws.Cell(2, 5).Value = 4;
                            ws.Cell(2, 6).Value = "uang";
                            ws.Cell(2, 7).Value = 60000;

                            ws.Cell(3, 1).Value = "Budi Santoso";
                            ws.Cell(3, 2).Value = "Jl. Melati No.5, Wonosobo";
                            ws.Cell(2, 3).Style.NumberFormat.Format = "@";
                            ws.Cell(3, 3).Value = "+6282345678901";
                            ws.Cell(3, 4).Value = DateTime.Today.ToString("yyyy-MM-dd");
                            ws.Cell(3, 5).Value = 3;
                            ws.Cell(3, 6).Value = "beras";
                            ws.Cell(3, 7).Value = 7.5;

                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Template berhasil disimpan di:\n" + sfd.FileName, "Sukses",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal membuat template: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // =====================================================================
        // TOMBOL: PILIH FILE -> BACA & PREVIEW
        // =====================================================================
        private void btnPilihFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel Files|*.xlsx;*.xls";
                ofd.Title = "Pilih File Excel Data Pembayaran Zakat";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = ofd.FileName;
                    BacaFileExcel(ofd.FileName);
                }
            }
        }

        // Membaca file .xlsx dengan ClosedXML lalu menampilkannya sebagai
        // preview di dgvPreview SEBELUM benar-benar di-import ke database.
        // Kolom tambahan "Validasi" diisi setelah validasi format dilakukan,
        // supaya pengguna bisa lihat baris mana yang formatnya salah
        // sebelum menekan tombol "Import Sekarang".
        private void BacaFileExcel(string filePath)
        {
            try
            {
                dtPreview = new DataTable();
                dtPreview.Columns.Add("Baris", typeof(int));
                dtPreview.Columns.Add("nama", typeof(string));
                dtPreview.Columns.Add("alamat", typeof(string));
                dtPreview.Columns.Add("no_hp", typeof(string));
                dtPreview.Columns.Add("tanggal", typeof(string));
                dtPreview.Columns.Add("jumlah_jiwa", typeof(string));
                dtPreview.Columns.Add("jenis_pembayaran", typeof(string));
                dtPreview.Columns.Add("total_bayar", typeof(string));
                dtPreview.Columns.Add("Validasi", typeof(string));

                using (var workbook = new XLWorkbook(filePath))
                {
                    var ws = workbook.Worksheet(1);
                    // Baris pertama dianggap header, mulai baca dari baris ke-2
                    var rows = ws.RangeUsed().RowsUsed();
                    int baris = 1;
                    bool headerSkipped = false;

                    foreach (var row in rows)
                    {
                        if (!headerSkipped) { headerSkipped = true; continue; }

                        string nama = row.Cell(1).GetString().Trim();
                        string alamat = row.Cell(2).GetString().Trim();
                        string noHp = row.Cell(3).GetString().Trim();
                        string tanggalStr = row.Cell(4).GetString().Trim();
                        string jumlahJiwaStr = row.Cell(5).GetString().Trim();
                        string jenisPembayaran = row.Cell(6).GetString().Trim().ToLower();
                        string totalBayarStr = row.Cell(7).GetString().Trim();

                        // Baris kosong total -> lewati (bukan error, biasanya cuma baris akhir kosong)
                        if (string.IsNullOrWhiteSpace(nama) && string.IsNullOrWhiteSpace(alamat))
                            continue;

                        string validasi = ValidasiBaris(nama, alamat, noHp, tanggalStr, jumlahJiwaStr, jenisPembayaran, totalBayarStr);

                        dtPreview.Rows.Add(baris, nama, alamat, noHp, tanggalStr, jumlahJiwaStr, jenisPembayaran, totalBayarStr, validasi);
                        baris++;
                    }
                }

                dgvPreview.DataSource = dtPreview;
                dgvPreview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                int totalBaris = dtPreview.Rows.Count;
                int baseValid = 0;
                foreach (DataRow r in dtPreview.Rows)
                    if (r["Validasi"].ToString() == "OK") baseValid++;

                lblStatus.Text = $"Ditemukan {totalBaris} baris data. Valid: {baseValid}, Bermasalah: {totalBaris - baseValid}.";
                btnImport.Enabled = totalBaris > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal membaca file Excel: " + ex.Message + "\n\n" +
                    "Pastikan file menggunakan format template yang disediakan (tombol 'Unduh Template').",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnImport.Enabled = false;
            }
        }

        // Validasi format SEBELUM dikirim ke database. Ini validasi lapis
        // pertama di sisi C# (cepat, langsung terlihat di preview tanpa
        // round-trip ke server). Validasi lapis kedua tetap ada di
        // sp_ImportPembayaran (jaga-jaga kalau ada yang lolos dari sini).
        private string ValidasiBaris(string nama, string alamat, string noHp, string tanggalStr,
            string jumlahJiwaStr, string jenisPembayaran, string totalBayarStr)
        {
            if (string.IsNullOrWhiteSpace(nama) || !Regex.IsMatch(nama, @"^[A-Za-z' ]+$"))
                return "Nama kosong/tidak valid";

            if (string.IsNullOrWhiteSpace(alamat))
                return "Alamat kosong";

            if (string.IsNullOrWhiteSpace(noHp) || !Regex.IsMatch(noHp, @"^[0-9+]+$"))
                return "No HP tidak valid";

            if (!DateTime.TryParse(tanggalStr, out _))
                return "Format tanggal salah";

            if (!int.TryParse(jumlahJiwaStr, out int jiwa) || jiwa <= 0)
                return "Jumlah jiwa tidak valid";

            if (jenisPembayaran != "uang" && jenisPembayaran != "beras")
                return "Jenis pembayaran harus 'uang' atau 'beras'";

            if (!decimal.TryParse(totalBayarStr, out decimal totalBayar) || totalBayar <= 0)
                return "Total bayar tidak valid";

            return "OK";
        }

        // =====================================================================
        // TOMBOL: IMPORT SEKARANG
        // =====================================================================
        // Memanggil sp_ImportPembayaran satu kali PER BARIS valid (bukan
        // satu transaksi besar untuk seluruh file). Tujuannya supaya kalau
        // ada satu baris gagal (misal data sudah ada / melanggar constraint),
        // baris lain yang valid tetap berhasil diimpor -- baris yang gagal
        // dilaporkan ke pengguna di akhir proses, bukan membatalkan semuanya.
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (dtPreview == null || dtPreview.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk diimpor.", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult konfirmasi = MessageBox.Show(
                $"Akan mengimpor {dtPreview.Rows.Count} baris data (baris bermasalah akan dilewati). Lanjutkan?",
                "Konfirmasi Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (konfirmasi != DialogResult.Yes) return;

            btnImport.Enabled = false;
            btnPilihFile.Enabled = false;

            int sukses = 0;
            int gagal = 0;
            int dilewati = 0;
            StringBuilder logGagal = new StringBuilder();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = dtPreview.Rows.Count;
            progressBar1.Value = 0;

            using (SqlConnection kon = new SqlConnection(connectionString))
            {
                kon.Open();

                foreach (DataRow row in dtPreview.Rows)
                {
                    progressBar1.Value++;
                    Application.DoEvents(); // supaya progress bar & UI tetap responsif

                    if (row["Validasi"].ToString() != "OK")
                    {
                        dilewati++;
                        logGagal.AppendLine($"Baris {row["Baris"]}: dilewati ({row["Validasi"]})");
                        continue;
                    }

                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_ImportPembayaran", kon))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@nama", row["nama"].ToString());
                            cmd.Parameters.AddWithValue("@alamat", row["alamat"].ToString());
                            cmd.Parameters.AddWithValue("@no_hp", row["no_hp"].ToString());
                            cmd.Parameters.AddWithValue("@tanggal", DateTime.Parse(row["tanggal"].ToString()));
                            cmd.Parameters.AddWithValue("@jumlah_jiwa", int.Parse(row["jumlah_jiwa"].ToString()));
                            cmd.Parameters.AddWithValue("@total_bayar", decimal.Parse(row["total_bayar"].ToString()));
                            cmd.Parameters.AddWithValue("@jenis_pembayaran", row["jenis_pembayaran"].ToString());

                            SqlParameter pStatus = cmd.Parameters.Add("@status", SqlDbType.VarChar, 10);
                            pStatus.Direction = ParameterDirection.Output;
                            SqlParameter pPesan = cmd.Parameters.Add("@pesan", SqlDbType.VarChar, 255);
                            pPesan.Direction = ParameterDirection.Output;

                            cmd.ExecuteNonQuery();

                            string status = pStatus.Value?.ToString() ?? "GAGAL";
                            string pesan = pPesan.Value?.ToString() ?? "Tidak diketahui";

                            if (status == "SUKSES")
                            {
                                sukses++;
                            }
                            else
                            {
                                gagal++;
                                logGagal.AppendLine($"Baris {row["Baris"]} ({row["nama"]}): {pesan}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        gagal++;
                        logGagal.AppendLine($"Baris {row["Baris"]} ({row["nama"]}): {ex.Message}");
                    }
                }
            }

            lblStatus.Text = $"Selesai. Sukses: {sukses}, Gagal: {gagal}, Dilewati: {dilewati}.";

            string ringkasan = $"Import selesai!\n\nBerhasil: {sukses}\nGagal: {gagal}\nDilewati (format salah): {dilewati}";
            if (logGagal.Length > 0)
            {
                ringkasan += "\n\nDetail:\n" + logGagal.ToString();
            }

            MessageBox.Show(ringkasan, "Hasil Import", MessageBoxButtons.OK,
                sukses > 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            btnImport.Enabled = true;
            btnPilihFile.Enabled = true;

            if (sukses > 0)
            {
                this.DialogResult = DialogResult.OK; // memberi tahu Form2 untuk refresh data
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
