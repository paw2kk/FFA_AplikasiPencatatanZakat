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
