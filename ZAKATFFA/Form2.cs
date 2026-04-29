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
using System.Text. RegularExpressions;

namespace ZAKATFFA
{
    public partial class Form2 : Form
    {

        SqlConnection kon;
        SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt;

        string connectionString = "Server=LAPTOP-1QL3V291\\PAW;Database=zakatdb;Trusted_Connection=True;";

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
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Pilih data yang ingin diupdate!", "Peringatan",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validasi Nama
                if (!Regex.IsMatch(txtNama.Text, @"^[A-Za-z' ]+$"))
                {
                    MessageBox.Show("Nama hanya boleh berisi huruf (A-Z / a-z) dan tanda apostrof (').",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validasi No HP
                if (!Regex.IsMatch(txtNoHP.Text, @"^[0-9+]+$"))
                {
                    MessageBox.Show("Nomor HP hanya boleh berisi angka (0-9) dan tanda '+'.",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ✅ TAMBAHKAN INI - Validasi dropdown jenis pembayaran
                string jenisInput = cmbJenisBerasAtauUang.SelectedItem?.ToString();
                if (jenisInput != "beras" && jenisInput != "uang")
                {
                    MessageBox.Show("Jenis pembayaran harus dipilih: 'beras' atau 'uang'!",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idPembayaran = Convert.ToInt32(
                    dataGridView1.SelectedRows[0].Cells["id_pembayaran"].Value);

                kon = new SqlConnection(connectionString);
                kon.Open();

                // Update muzakki
                string updateMuzakki = @"
                    UPDATE muzakki SET 
                        nama   = @nama,
                        alamat = @alamat,
                        no_hp  = @no_hp
                    WHERE id_muzakki = (
                        SELECT id_muzakki FROM pembayaran_zakat 
                        WHERE id_pembayaran = @id_pembayaran)";

                cmd = new SqlCommand(updateMuzakki, kon);
                cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                cmd.Parameters.AddWithValue("@no_hp", txtNoHP.Text);
                cmd.Parameters.AddWithValue("@id_pembayaran", idPembayaran);
                cmd.ExecuteNonQuery();

                // Tentukan jumlah_uang atau jumlah_beras
                object jumlahUang = DBNull.Value;
                object jumlahBeras = DBNull.Value;
                string jenis = cmbJenisBerasAtauUang.SelectedItem.ToString();

                if (jenis == "uang")
                    jumlahUang = Convert.ToDecimal(txtBayar.Text);
                else
                    jumlahBeras = Convert.ToDecimal(txtBayar.Text);

                decimal totalBayar = Convert.ToDecimal(txtBayar.Text);

                // Update pembayaran
                string updatePembayaran = @"
                    UPDATE pembayaran_zakat SET
                        tanggal          = @tanggal,
                        jumlah_jiwa      = @jumlah_jiwa,
                        jumlah_uang      = @jumlah_uang,
                        jumlah_beras     = @jumlah_beras,
                        total_bayar      = @total_bayar,
                        jenis_pembayaran = @jenis_pembayaran
                    WHERE id_pembayaran  = @id_pembayaran";

                cmd = new SqlCommand(updatePembayaran, kon);
                cmd.Parameters.AddWithValue("@tanggal", dtp1.Value.Date);
                cmd.Parameters.AddWithValue("@jumlah_jiwa", Convert.ToInt32(txtJumlahJiwa.Text));
                cmd.Parameters.AddWithValue("@jumlah_uang", jumlahUang);
                cmd.Parameters.AddWithValue("@jumlah_beras", jumlahBeras);
                cmd.Parameters.AddWithValue("@total_bayar", totalBayar);
                cmd.Parameters.AddWithValue("@jenis_pembayaran", jenis);
                cmd.Parameters.AddWithValue("@id_pembayaran", idPembayaran);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Data berhasil diupdate!", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                KosongkanForm();
                btnTampilData_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnHapus_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Pilih data yang ingin dihapus!", "Peringatan",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult konfirmasi = MessageBox.Show(
                    "Apakah yakin ingin menghapus data ini?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                if (konfirmasi == DialogResult.Yes)
                {
                    int idPembayaran = Convert.ToInt32(
                        dataGridView1.SelectedRows[0].Cells["id_pembayaran"].Value);

                    kon = new SqlConnection(connectionString);
                    kon.Open();
                    cmd = new SqlCommand(
                        "DELETE FROM pembayaran_zakat WHERE id_pembayaran = @id", kon);
                    cmd.Parameters.AddWithValue("@id", idPembayaran);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data berhasil dihapus!", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    KosongkanForm();
                    btnTampilData_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnTambah_Click(object sender, EventArgs e)
        {
            try
            {
                // Validasi input kosong
                if (txtNama.Text.Trim() == "" || txtAlamat.Text.Trim() == "" ||
                    txtNoHP.Text.Trim() == "" || txtJumlahJiwa.Text.Trim() == "" ||
                    txtBayar.Text.Trim() == "" || cmbJenisBerasAtauUang.SelectedItem == null)
                {
                    MessageBox.Show("Semua field harus diisi! " + (cmbJenisBerasAtauUang.SelectedItem != null ? "ada isi" : "gada isi"), "Peringatan",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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


                // ✅ TAMBAHKAN INI - Validasi dropdown jenis pembayaran
                string jenisInput = cmbJenisBerasAtauUang.SelectedItem?.ToString();
                if (jenisInput != "beras" && jenisInput != "uang")
                {
                    MessageBox.Show("Jenis pembayaran harus dipilih: 'beras' atau 'uang'!",
                        "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtJumlahJiwa.Text, out int jumlahJiwa))
                {
                    MessageBox.Show("Jumlah jiwa harus berupa angka!",
           "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtBayar.Text, out int jumlahBayar))
                {
                    MessageBox.Show("Jumlah bayar harus berupa angka!",
           "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                kon = new SqlConnection(connectionString);
                kon.Open();

                // Cek atau tambah muzakki dulu
                string cekMuzakki = "SELECT id_muzakki FROM muzakki WHERE nama = @nama AND no_hp = @no_hp";
                cmd = new SqlCommand(cekMuzakki, kon);
                cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@no_hp", txtNoHP.Text);
                object hasilCek = cmd.ExecuteScalar();

                int idMuzakki;

                if (hasilCek == null)
                {
                    string tambahMuzakki = @"
                        INSERT INTO muzakki (nama, alamat, no_hp) 
                        VALUES (@nama, @alamat, @no_hp);
                        SELECT SCOPE_IDENTITY();";
                    cmd = new SqlCommand(tambahMuzakki, kon);
                    cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                    cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                    cmd.Parameters.AddWithValue("@no_hp", txtNoHP.Text);
                    idMuzakki = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    idMuzakki = Convert.ToInt32(hasilCek);
                }

                // Tentukan jumlah_uang atau jumlah_beras
                object jumlahUang = DBNull.Value;
                object jumlahBeras = DBNull.Value;
                string jenis = cmbJenisBerasAtauUang.SelectedItem.ToString();

                if (jenis == "uang")
                    jumlahUang = Convert.ToDecimal(txtBayar.Text);
                else
                    jumlahBeras = Convert.ToDecimal(txtBayar.Text);


                string tambahPembayaran = @"
                    INSERT INTO pembayaran_zakat 
                        (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
                    VALUES 
                        (@id_muzakki, @tanggal, @jumlah_jiwa, @jumlah_uang, @jumlah_beras, @total_bayar, @jenis_pembayaran)";

                cmd = new SqlCommand(tambahPembayaran, kon);
                cmd.Parameters.AddWithValue("@id_muzakki", idMuzakki);
                cmd.Parameters.AddWithValue("@tanggal", dtp1.Value.Date);
                cmd.Parameters.AddWithValue("@jumlah_jiwa", jumlahJiwa);
                cmd.Parameters.AddWithValue("@jumlah_uang", jumlahUang);
                cmd.Parameters.AddWithValue("@jumlah_beras", jumlahBeras);
                cmd.Parameters.AddWithValue("@total_bayar", jumlahBayar);
                cmd.Parameters.AddWithValue("@jenis_pembayaran", jenis);
                cmd.ExecuteNonQuery();


                MessageBox.Show("Data berhasil ditambahkan!", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                KosongkanForm();
                btnTampilData_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTampilData_Click(object sender, EventArgs e)
        {
            try
            {
                kon = new SqlConnection(connectionString);
                string query = @"
            SELECT 
                p.id_pembayaran,
                m.nama,
                m.alamat,
                m.no_hp,
                p.tanggal,
                p.jumlah_jiwa,
                p.jenis_pembayaran,
                p.jumlah_uang,
                p.jumlah_beras,
                p.total_bayar,
                CASE 
                    WHEN p.jenis_pembayaran = 'beras' 
                        THEN CAST(p.jumlah_beras AS VARCHAR) + ' kg'
                    WHEN p.jenis_pembayaran = 'uang'  
                        THEN 'Rp ' + FORMAT(p.jumlah_uang, 'N0')
                END AS jumlah_dengan_satuan
            FROM pembayaran_zakat p
            JOIN muzakki m ON p.id_muzakki = m.id_muzakki";

                da = new SqlDataAdapter(query, kon);
                dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // ===== KOLOM PENUH OTOMATIS =====
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // ===== ATUR LEBAR KOLOM TERTENTU =====
                dataGridView1.Columns["id_pembayaran"].FillWeight = 5;
                dataGridView1.Columns["nama"].FillWeight = 15;
                dataGridView1.Columns["alamat"].FillWeight = 20;
                dataGridView1.Columns["no_hp"].FillWeight = 10;
                dataGridView1.Columns["tanggal"].FillWeight = 8;
                dataGridView1.Columns["jumlah_jiwa"].FillWeight = 7;
                dataGridView1.Columns["jenis_pembayaran"].FillWeight = 8;
                dataGridView1.Columns["jumlah_uang"].FillWeight = 8;
                dataGridView1.Columns["jumlah_beras"].FillWeight = 8;
                dataGridView1.Columns["total_bayar"].FillWeight = 8;
                dataGridView1.Columns["jumlah_dengan_satuan"].FillWeight = 10;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
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
            dtp1.MaxDate = DateTime.Now; // Maksimal hari ini (tidak bisa pilih masa depan)

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
            dtp1.Value = DateTime.Now;
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
                kon.Open();

                string query = @"
            SELECT 
                p.id_pembayaran,
                m.nama,
                m.alamat,
                m.no_hp,
                p.tanggal,
                p.jumlah_jiwa,
                p.jenis_pembayaran,
                p.jumlah_uang,
                p.jumlah_beras,
                p.total_bayar,
                CASE 
                    WHEN p.jenis_pembayaran = 'beras' 
                        THEN CAST(p.jumlah_beras AS VARCHAR) + ' kg'
                    WHEN p.jenis_pembayaran = 'uang'  
                        THEN 'Rp ' + FORMAT(p.jumlah_uang, 'N0')
                END AS jumlah_dengan_satuan
            FROM pembayaran_zakat p
            JOIN muzakki m ON p.id_muzakki = m.id_muzakki
            WHERE m.nama LIKE @nama";

                da = new SqlDataAdapter(query, kon);
                da.SelectCommand.Parameters.AddWithValue("@nama", "%" + txtCari.Text + "%");

                dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // Samakan styling kolom seperti btnTampilData
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.Columns["id_pembayaran"].FillWeight = 5;
                dataGridView1.Columns["nama"].FillWeight = 15;
                dataGridView1.Columns["alamat"].FillWeight = 20;
                dataGridView1.Columns["no_hp"].FillWeight = 10;
                dataGridView1.Columns["tanggal"].FillWeight = 8;
                dataGridView1.Columns["jumlah_jiwa"].FillWeight = 7;
                dataGridView1.Columns["jenis_pembayaran"].FillWeight = 8;
                dataGridView1.Columns["jumlah_uang"].FillWeight = 8;
                dataGridView1.Columns["jumlah_beras"].FillWeight = 8;
                dataGridView1.Columns["total_bayar"].FillWeight = 8;
                dataGridView1.Columns["jumlah_dengan_satuan"].FillWeight = 10;

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



