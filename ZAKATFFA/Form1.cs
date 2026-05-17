using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZAKATFFA
{
    public partial class Form1 : Form
    {
        SqlConnection kon;
        SqlCommand cmd;

        string connectionString = "Server=LAPTOP-1QL3V291\\PAW;Database=zakatdb;Trusted_Connection=True;";

        public Form1()
        {
            InitializeComponent();
            
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox2.Focus(); // Pindah ke textbox password
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin_Click(sender, e); // Langsung login
                e.Handled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            textBox2.PasswordChar = '*';

            // ===== FULLSCREEN =====
            this.Text = "Login - Aplikasi Pencatatan Zakat";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.BackColor = Color.FromArgb(0, 100, 0);

            button1.ForeColor = Color.White;
            button1.Font = new Font("Arial", 9, FontStyle.Bold);
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = System.Drawing.Color.DarkRed;

            btnKeluar.ForeColor = Color.White;
            btnKeluar.Font = new Font("Arial", 9, FontStyle.Bold);
            btnKeluar.FlatStyle = FlatStyle.Flat;
            btnKeluar.FlatAppearance.BorderSize = 0;
            btnKeluar.BackColor = System.Drawing.Color.DarkGreen;

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // Validasi input kosong
                if (textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("Nama dan Password harus diisi!", "Peringatan",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                kon = new SqlConnection(connectionString);
                kon.Open();

                string query = "SELECT COUNT(*) FROM pengguna WHERE nama = @nama AND password = @password";
                cmd = new SqlCommand(query, kon);
                cmd.Parameters.AddWithValue("@nama", textBox1.Text);
                cmd.Parameters.AddWithValue("@password", textBox2.Text);

                int hasil = Convert.ToInt32(cmd.ExecuteScalar());

                if (hasil > 0)
                {
                    MessageBox.Show("Login Berhasil! Selamat datang, " + textBox1.Text, "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Form2 formData = new Form2(); // Buka Form2 CRUD
                    formData.Show();              // Tampilkan Form2
                    this.Hide();                  // Sembunyikan Form1 Login
                }
                else
                {
                    MessageBox.Show("Nama atau Password salah!", "Login Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();
                    textBox2.Focus();
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void btnKeluar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Apakah Anda yakin ingin keluar dari aplikasi?",
        "Konfirmasi Keluar",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // !! QUERY RENTAN: input digabung langsung ke string !!
                    // Jangan gunakan pola ini di produksi!
                    string queryRentan =
                        "SELECT COUNT(*) FROM pengguna " +
                        "WHERE nama = '" + textBox1.Text + "' " +
                        "AND password = '" + textBox2.Text + "'";

                    // Query yang sebenarnya dieksekusi:
                    // SELECT COUNT(*) FROM pengguna
                    // WHERE nama = '' OR '1'='1' AND password = '' OR '1'='1'
                    // → selalu TRUE → COUNT > 0 → login berhasil!

                    using (SqlCommand cmdInject = new SqlCommand(queryRentan, conn))
                    {
                        int hasil = Convert.ToInt32(cmdInject.ExecuteScalar());

                        // Tampilkan query asli agar terlihat bahayanya
                        MessageBox.Show(
                            "=== DEMO SQL INJECTION ===\n\n" +
                            "Query yang dieksekusi:\n" +
                            queryRentan + "\n\n" +
                            $"Hasil COUNT(*) = {hasil}\n\n" +
                            (hasil > 0
                                ? "⚠ LOGIN BERHASIL tanpa password yang benar!\n" +
                                  "Injeksi membuat kondisi WHERE selalu TRUE."
                                : "Injeksi tidak berhasil pada database ini."),
                            "SQL Injection Demo",
                            MessageBoxButtons.OK,
                            hasil > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                        // Jika injeksi berhasil → buka Form2 seperti login normal
                        if (hasil > 0)
                        {
                            // Catat ke log (pakai parameterized agar log-nya aman)
                            string logQuery =
                                "INSERT INTO log_login (nama, status) VALUES (@nama, @status)";
                            using (SqlCommand cmdLog = new SqlCommand(logQuery, conn))
                            {
                                cmdLog.Parameters.AddWithValue("@nama", textBox1.Text);
                                cmdLog.Parameters.AddWithValue("@status", "INJECT");
                                cmdLog.ExecuteNonQuery();
                            }

                            Form2 formData = new Form2();
                            formData.Show();
                            this.Hide();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
