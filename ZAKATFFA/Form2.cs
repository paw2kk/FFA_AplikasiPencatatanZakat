using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ZAKATFFA
{
    public partial class Form2 : Form
    {
        SqlConnection kon;
        SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt;

        string connectionString = "Server=LAPTOP-1QL3V291\\PAW;Database=zakatdb;Trusted_Connection=True;";

        private BindingSource _bindingSource = new BindingSource();
        private DataTable _dt = new DataTable();

        public Form2()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtNama.Text = row.Cells["nama"].Value.ToString();
                txtAlamat.Text = row.Cells["alamat"].Value.ToString();
                txtNoHP.Text = row.Cells["no_hp"].Value.ToString();
                dtp1.Value = Convert.ToDateTime(row.Cells["tanggal"].Value);
                txtJumlahJiwa.Text = row.Cells["jumlah_jiwa"].Value.ToString();
                txtBayar.Text = row.Cells["total_bayar"].Value.ToString();

                // Set dropdown sesuai jenis pembayaran
                string jenis = row.Cells["jenis_pembayaran"].Value.ToString();
                cmbJenisBerasAtauUang.SelectedItem = jenis;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Silakan pilih data yang ingin diubah pada tabel!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                // 2. Ambil id_pembayaran dari baris DataGridView yang sedang aktif
                // Jika nama kolom manual Anda adalah "id_muzakki", pastikan disesuaikan atau gunakan index sel [1]
                int idPembayaran = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id_muzakki"].Value);

                using (SqlConnection kon = new SqlConnection(connectionString))
                {
                    kon.Open();

                    // 3. Panggil nama Stored Procedure yang ada di SQL Server Anda
                    using (SqlCommand cmd = new SqlCommand("sp_UpdatePembayaran", kon))
                    {
                        // Wajib set jenis command menjadi StoredProcedure
                        cmd.CommandType = CommandType.StoredProcedure;

                        // 4. Daftarkan seluruh parameter (Harus sama persis dengan variabel di SQL)
                        cmd.Parameters.AddWithValue("@id_pembayaran", idPembayaran);
                        cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                        cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                        cmd.Parameters.AddWithValue("@no_hp", txtNoHP.Text);

                        // Mengambil nilai tanggal saja dari DateTimePicker
                        cmd.Parameters.AddWithValue("@tanggal", dtp1.Value.Date);

                        cmd.Parameters.AddWithValue("@jumlah_jiwa", Convert.ToInt32(txtJumlahJiwa.Text));
                        cmd.Parameters.AddWithValue("@total_bayar", Convert.ToDecimal(txtBayar.Text));

                        // Pastikan teks bernilai 'uang' atau 'beras' (huruf kecil agar sesuai logika IF di SP Anda)
                        cmd.Parameters.AddWithValue("@jenis_pembayaran", cmbJenisBerasAtauUang.Text.ToLower().Trim());

                        // 5. Eksekusi program ke database
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data transaksi pembayaran dan profil muzakki berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 6. Refresh GridView agar data terupdate langsung muncul
                btnTampilData_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (kon != null && kon.State == ConnectionState.Open) kon.Close();
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Silakan pilih data pada tabel terlebih dahulu yang ingin dihapus!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 2. AMBIL ID DARI KOLOM YANG VALID
                // Karena kita mengubah urutan kolom manual, gunakan kolom "id_muzakki" atau indeks kolom [1] 
                // yang memuat data kunci (Primary Key) dari database Anda.

                string idDihapus = "";

                if (dataGridView1.Columns["id_muzakki"] != null)
                {
                    idDihapus = dataGridView1.CurrentRow.Cells["id_muzakki"].Value.ToString();
                }
                else
                {
                    // Jika nama kolom text-nya tidak ketemu, kita paksa ambil cell indeks ke-1 (kolom kedua dari kiri)
                    idDihapus = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                }

                // 3. Konfirmasi sebelum benar-benar menghapus data
                DialogResult dialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus data dengan ID " + idDihapus + "?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                {
                    using (SqlConnection kon = new SqlConnection(connectionString))
                    {
                        kon.Open();

                        // SESUAIKAN: Ganti 'id_muzakki' dan nama 'tabel_zakat' sesuai nama field & tabel asli di database Anda
                        string queryHapus = "EXEC sp_HapusPembayaran @id";

                        using (SqlCommand cmd = new SqlCommand(queryHapus, kon))
                        {
                            cmd.Parameters.AddWithValue("@id", idDihapus);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Data berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 4. Refresh tampilan data setelah berhasil dihapus
                    btnTampilData_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (kon != null && kon.State == ConnectionState.Open) kon.Close();
            }
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection kon = new SqlConnection(connectionString))
                {
                    kon.Open();
                    if (!Regex.IsMatch(txtNama.Text, @"^[A-Za-z' ]+$"))
                    {
                        MessageBox.Show("Nama hanya boleh berisi huruf (A-Z / a-z) dan tanda '.",
                            "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Validasi No HP: hanya angka dan tanda +
                    if (!Regex.IsMatch(txtNoHP.Text, @"^[0-9+]+$"))
                    {
                        MessageBox.Show("Nomor HP hanya boleh berisi angka (0-9) dan tanda +.",
                            "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    using (SqlCommand cmd = new SqlCommand("sp_TambahPembayaran", kon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Masukkan seluruh parameter bawaan
                        cmd.Parameters.AddWithValue("@nama", txtNama.Text.Trim());
                        cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text.Trim());
                        cmd.Parameters.AddWithValue("@no_hp", txtNoHP.Text.Trim());
                        cmd.Parameters.AddWithValue("@tanggal", dtp1.Value.Date);
                        cmd.Parameters.AddWithValue("@jumlah_jiwa", Convert.ToInt32(txtJumlahJiwa.Text));

                        // ===== PERBAIKAN DI BARIS INI =====
                        // Ganti nama parameter dari "@total_bayar" menjadi "@total_bayar_input"
                        cmd.Parameters.AddWithValue("@total_bayar_input", Convert.ToDecimal(txtBayar.Text));

                        cmd.Parameters.AddWithValue("@jenis_pembayaran", cmbJenisBerasAtauUang.Text.ToLower().Trim());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data muzakki dan transaksi pembayaran berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnTampilData_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (kon != null && kon.State == ConnectionState.Open) kon.Close();
            }
        }

        private void btnTampilData_Click(object sender, EventArgs e)
        {
            try
            {
                kon = new SqlConnection(connectionString);
                string query = "SELECT * FROM v_pembayaran_zakat";
                da = new SqlDataAdapter(query, kon);
                dt = new DataTable();
                da.Fill(dt);

                // ===== KOLOM PENUH OTOMATIS =====
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.Columns.Clear();
                dataGridView1.AutoGenerateColumns = false;

                // ===== ATUR LEBAR KOLOM TERTENTU =====
                // 1. Kolom Nama (Taruh paling kiri setelah kolom nomor bawaan)
                dataGridView1.Columns.Add("nama", "Nama");
                dataGridView1.Columns["nama"].DataPropertyName = "nama"; // Harus sama persis dengan nama kolom di database

                // 2. Kolom ID Muzakki (Menggeser ID ke sebelah kanan nama)
                dataGridView1.Columns.Add("id_muzakki", "ID Muzakki");
                dataGridView1.Columns["id_muzakki"].DataPropertyName = "id_muzakki";

                // 3. Kolom Alamat
                dataGridView1.Columns.Add("alamat", "Alamat");
                dataGridView1.Columns["alamat"].DataPropertyName = "alamat";

                // 4. Kolom Nomer Handphone
                dataGridView1.Columns.Add("no_hp", "Nomer Handphone");
                dataGridView1.Columns["no_hp"].DataPropertyName = "no_hp";

                // 5. Kolom Tanggal
                dataGridView1.Columns.Add("tanggal", "Tanggal");
                dataGridView1.Columns["tanggal"].DataPropertyName = "tanggal";

                // 6. Kolom Jumlah Jiwa
                dataGridView1.Columns.Add("jumlah_jiwa", "Jumlah Jiwa");
                dataGridView1.Columns["jumlah_jiwa"].DataPropertyName = "jumlah_jiwa";

                // 7. Kolom Jenis Pembayaran
                dataGridView1.Columns.Add("jenis_pembayaran", "Jenis Pembayaran");
                dataGridView1.Columns["jenis_pembayaran"].DataPropertyName = "jenis_pembayaran";

                // 8. Kolom Total Bayar
                dataGridView1.Columns.Add("total_bayar", "Total Bayar");
                dataGridView1.Columns["total_bayar"].DataPropertyName = "total_bayar";

                // ===== BARU SETELAH ITU ISI DATA SOURCE-NYA =====
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            kon = new SqlConnection(connectionString);

            // TODO: This line of code loads data into the 'zakatdbDataSet.pembayaran_zakat' table. You can move, or remove it, as needed.
            this.pembayaran_zakatTableAdapter.Fill(this.zakatdbDataSet.pembayaran_zakat);
            // TODO: This line of code loads data into the 'zakatdbDataSet.muzakki' table. You can move, or remove it, as needed.
            this.muzakkiTableAdapter.Fill(this.zakatdbDataSet.muzakki);
            // ===== FULLSCREEN =====
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Text = "Aplikasi Zakat - Data Pembayaran";
            this.BackColor = Color.FromArgb(0, 100, 0); // Hijau gelap sama seperti Form1

            // ===== STYLING LABEL =====
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label)
                {
                    ctrl.ForeColor = Color.White;
                    ctrl.Font = new Font("Arial", 9, FontStyle.Bold);
                }
            }

            // ===== STYLING TEXTBOX =====
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox)
                {
                    ctrl.BackColor = Color.White;
                    ctrl.ForeColor = Color.Black;
                    ctrl.Font = new Font("Arial", 9);
                }
            }

            // ===== STYLING BUTTON =====
            btnTampilData.BackColor = Color.FromArgb(0, 128, 0);
            btnTampilData.ForeColor = Color.White;
            btnTampilData.Font = new Font("Arial", 9, FontStyle.Bold);
            btnTampilData.FlatStyle = FlatStyle.Flat;
            btnTampilData.FlatAppearance.BorderSize = 0;

            btnTambah.BackColor = Color.FromArgb(0, 128, 0);
            btnTambah.ForeColor = Color.White;
            btnTambah.Font = new Font("Arial", 9, FontStyle.Bold);
            btnTambah.FlatStyle = FlatStyle.Flat;
            btnTambah.FlatAppearance.BorderSize = 0;

            btnHapus.BackColor = Color.FromArgb(180, 0, 0);
            btnHapus.ForeColor = Color.White;
            btnHapus.Font = new Font("Arial", 9, FontStyle.Bold);
            btnHapus.FlatStyle = FlatStyle.Flat;
            btnHapus.FlatAppearance.BorderSize = 0;

            btnUpdate.BackColor = Color.FromArgb(180, 130, 0);
            btnUpdate.ForeColor = Color.White;
            btnUpdate.Font = new Font("Arial", 9, FontStyle.Bold);
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.FlatAppearance.BorderSize = 0;

            this.btnLogOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogOut.FlatAppearance.BorderSize = 0;
            this.btnLogOut.Font = new Font("Arial", 9, FontStyle.Bold);
            this.btnLogOut.ForeColor = Color.White;

            // ===== LOGO MUI DI POJOK KIRI ATAS =====
            PictureBox picLogo = new PictureBox();
            picLogo.Size = new Size(60, 60);
            picLogo.Location = new Point(10, 5);
            picLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            picLogo.BackColor = Color.Transparent;
            try
            {
                picLogo.Image = Image.FromFile(Application.StartupPath + @"\logo_mui.png");
            }
            catch { }
            this.Controls.Add(picLogo);
            picLogo.BringToFront();

            // ===== LABEL JUDUL DI ATAS =====
            Label lblJudul = new Label();
            lblJudul.Text = "APLIKASI ZAKAT FITRAH";
            lblJudul.Font = new Font("Arial", 14, FontStyle.Bold);
            lblJudul.ForeColor = Color.White;
            lblJudul.AutoSize = false;
            lblJudul.Size = new Size(400, 35);
            lblJudul.Location = new Point(75, 20);
            lblJudul.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(lblJudul);
            lblJudul.BringToFront();

            // ===== GARIS PEMISAH HEADER =====
            Panel panelGaris = new Panel();
            panelGaris.Size = new Size(this.Width, 3);
            panelGaris.Location = new Point(0, 53);
            panelGaris.BackColor = Color.LightGreen;
            this.Controls.Add(panelGaris);
            panelGaris.BringToFront();

            // ===== STYLING DATAGRIDVIEW =====
            dataGridView1.BackgroundColor = Color.FromArgb(0, 80, 0);
            dataGridView1.GridColor = Color.LightGreen;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Dock = DockStyle.Bottom;

            // Header kolom
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 60, 0);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.EnableHeadersVisualStyles = false;

            // Baris data
            dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(240, 255, 240);
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Font = new Font("Arial", 9);

            // Baris terpilih
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 150, 0);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            // Baris selang seling
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(210, 240, 210);

            // ===== STYLING DATETIMEPICKER =====
            dtp1.CalendarMonthBackground = Color.FromArgb(0, 100, 0);
            dtp1.CalendarForeColor = Color.White;
            dtp1.CalendarTitleBackColor = Color.FromArgb(0, 60, 0);
            dtp1.CalendarTitleForeColor = Color.White;

            // ===== STYLING COMBOBOX =====
            cmbJenisBerasAtauUang.BackColor = Color.White;
            cmbJenisBerasAtauUang.ForeColor = Color.Black;
            cmbJenisBerasAtauUang.Font = new Font("Arial", 9);

            // ===== ISI DROPDOWN =====
            cmbJenisBerasAtauUang.Items.Clear();
            cmbJenisBerasAtauUang.Items.Add("beras");
            cmbJenisBerasAtauUang.Items.Add("uang");
            cmbJenisBerasAtauUang.SelectedIndex = 0;

            // ===== SET TANGGAL HARI INI =====
            dtp1.Value = DateTime.Now;

            // ===== BATASI TANGGAL MINIMUM (tidak bisa mundur 3 tahun ke belakang) =====
            dtp1.MinDate = new DateTime(DateTime.Now.Year - 3, 1, 1); // 1 Januari, 3 tahun lalu
            dtp1.MaxDate = DateTime.Today.AddDays(1); // Maksimal hari ini (tidak bisa pilih masa depan)

            // ===== LABEL COPYRIGHT =====
            Label lblCopyright = new Label();
            lblCopyright.Text = "© 2026 Aplikasi Zakat Fitrah - Majelis Ulama Indonesia";
            lblCopyright.Font = new Font("Arial", 8);
            lblCopyright.ForeColor = Color.LightGreen;
            lblCopyright.AutoSize = false;
            lblCopyright.Size = new Size(500, 20);
            lblCopyright.Location = new Point(10, this.Height - 40);
            lblCopyright.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(lblCopyright);
            lblCopyright.BringToFront();
        }

        private void KosongkanForm()
        {
            txtNama.Text = "";
            txtAlamat.Text = "";
            txtNoHP.Text = "";
            txtJumlahJiwa.Text = "";
            txtBayar.Text = "";
            dtp1.Value = DateTime.Today;
            cmbJenisBerasAtauUang.SelectedIndex = 0;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Apakah Anda yakin ingin logout?",
                "Konfirmasi Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Form1 formLogin = new Form1();
                formLogin.Show();
                this.Close();
            }
        }

        private void btnCari_Click(object sender, EventArgs e)
        {
            try
            {
                kon = new SqlConnection(connectionString);
                string query = "EXEC sp_CariPembayaran @nama";
                da = new SqlDataAdapter(query, kon);
                da.SelectCommand.Parameters.AddWithValue("@nama", "%" + txtCari.Text + "%");

                dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Columns["nama"] != null)
                    dataGridView1.Columns["nama"].FillWeight = 20;

                if (dataGridView1.Columns["id_muzakki"] != null)
                    dataGridView1.Columns["id_muzakki"].FillWeight = 5;

                if (dataGridView1.Columns["alamat"] != null)
                    dataGridView1.Columns["alamat"].FillWeight = 25;

                if (dataGridView1.Columns["no_hp"] != null)
                    dataGridView1.Columns["no_hp"].FillWeight = 12;

                if (dataGridView1.Columns["tanggal"] != null)
                    dataGridView1.Columns["tanggal"].FillWeight = 10;

                if (dataGridView1.Columns["jumlah_jiwa"] != null)
                    dataGridView1.Columns["jumlah_jiwa"].FillWeight = 8;

                if (dataGridView1.Columns["jenis_pembayaran"] != null)
                    dataGridView1.Columns["jenis_pembayaran"].FillWeight = 10;

                if (dataGridView1.Columns["total_bayar"] != null)
                    dataGridView1.Columns["total_bayar"].FillWeight = 10;

                // Kode pengecekan baris kosong Anda yang sudah ada
                if (dt.Rows.Count == 0)

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Data dengan nama tersebut tidak ditemukan.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (kon != null && kon.State == System.Data.ConnectionState.Open)
                    kon.Close();
            }
        }
    }
}