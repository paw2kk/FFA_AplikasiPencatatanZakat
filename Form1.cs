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

        string connectionString = "Server=LAPTOP-1QL3V291\\PAW;Database=zakatdb;User Id=sa;Password=123;TrustServerCertificate=True;";

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

        private void CenterPanel()
        {
            if (panel1 != null)
            {
                panel1.Left = (this.ClientSize.Width - panel1.Width) / 2;
                panel1.Top = (this.ClientSize.Height - panel1.Height) / 2;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Resize += (s, ev) => CenterPanel();

            this.KeyPreview = true;
            textBox2.PasswordChar = '*';

            // ===== FULLSCREEN =====
            this.Text = "Login - Aplikasi Pencatatan Zakat";
            this.WindowState = FormWindowState.Maximized;
            this.WindowState = FormWindowState.Maximized;
            pictureBox1.Dock = DockStyle.Fill;   // background selalu full-cover form, di ukuran berapa pun
            CenterPanel();                       // posisikan panel login ke tengah saat pertama kali load
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.BackColor = Color.FromArgb(0, 100, 0);

            btnLogin.ForeColor = Color.White;
            btnLogin.Font = new Font("Arial", 9, FontStyle.Bold);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.BackColor = System.Drawing.Color.DarkRed;

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
                    formData.WindowState = FormWindowState.Maximized; // ← tambah ini
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

                    // Query RENTAN - string concatenation
                    string queryRentan =
                        "SELECT COUNT(*) FROM pengguna " +
                        "WHERE nama = '" + textBox1.Text + "' " +
                        "AND password = '" + textBox2.Text + "'";

                    // Query yang terbentuk:
                    // SELECT COUNT(*) FROM pengguna
                    // WHERE nama = '' OR '1'='1'--' AND password = ''
                    // Bagian setelah -- dianggap komentar → password diabaikan!

                    using (SqlCommand cmdInject = new SqlCommand(queryRentan, conn))
                    {
                        int hasil = Convert.ToInt32(cmdInject.ExecuteScalar());

                        MessageBox.Show(
                            "=== DEMO SQL INJECTION ===\n\n" +
                            "Query yang dieksekusi:\n" +
                            queryRentan + "\n\n" +
                            $"COUNT(*) = {hasil}\n\n" +
                            "⚠ LOGIN BERHASIL tanpa password!\n" +
                            "Tanda -- membuat sisa query jadi komentar.",
                            "SQL Injection Demo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        if (hasil > 0)
                        {
                            Form2 formData = new Form2();
                            formData.WindowState = FormWindowState.Maximized; // ← tambah ini
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