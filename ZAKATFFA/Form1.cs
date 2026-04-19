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
    }
}
