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
            // Agar form bisa mendeteksi tombol Enter
            this.KeyPreview = true;

            // Sembunyikan karakter password
            textBox2.PasswordChar = '*';


            // ===== SETTING BACKGROUND FORM =====
            this.Text = "Login - Aplikasi Pencatatan Zakat";
            this.Size = new Size(450, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Set background warna hijau gelap seperti tema MUI
            this.BackColor = Color.FromArgb(0, 100, 0);

            // ===== PANEL PUTIH DI TENGAH =====
            Panel panelLogin = new Panel();
            panelLogin.Size = new Size(350, 320);
            panelLogin.Location = new Point(50, 200);
            panelLogin.BackColor = Color.White;
            panelLogin.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(panelLogin);

            // ===== LOGO MUI DI ATAS FORM =====
            PictureBox picLogo = new PictureBox();
            picLogo.Size = new Size(120, 120);
            picLogo.Location = new Point(165, 30);
            picLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            picLogo.BorderStyle = BorderStyle.None;
            picLogo.BackColor = Color.Transparent;

            try
            {
                // Ganti "logo_mui.png" dengan nama file gambar kamu
                picLogo.Image = Image.FromFile(Application.StartupPath + @"\Untitleddesign.png");
            }
            catch
            {
                // Jika gambar tidak ditemukan, tidak error
            }
            this.Controls.Add(picLogo);
            picLogo.BringToFront();

            // ===== LABEL JUDUL =====
            Label lblJudul = new Label();
            lblJudul.Text = "APLIKASI PENCATATAN ZAKAT";
            lblJudul.Font = new Font("Arial", 13, FontStyle.Bold);
            lblJudul.ForeColor = Color.White;
            lblJudul.AutoSize = false;
            lblJudul.Size = new Size(350, 30);
            lblJudul.Location = new Point(50, 160);
            lblJudul.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblJudul);

            // ===== LABEL & TEXTBOX DI DALAM PANEL =====

            // Label Nama
            Label lblNama = new Label();
            lblNama.Text = "Nama Pengguna";
            lblNama.Font = new Font("Arial", 9, FontStyle.Bold);
            lblNama.Location = new Point(30, 30);
            lblNama.AutoSize = true;
            panelLogin.Controls.Add(lblNama);

            // TextBox Nama (textBox1)
            textBox1.Location = new Point(30, 55);
            textBox1.Size = new Size(290, 30);
            textBox1.Font = new Font("Arial", 10);
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            panelLogin.Controls.Add(textBox1);

            // Label Password
            Label lblPassword = new Label();
            lblPassword.Text = "Password";
            lblPassword.Font = new Font("Arial", 9, FontStyle.Bold);
            lblPassword.Location = new Point(30, 100);
            lblPassword.AutoSize = true;
            panelLogin.Controls.Add(lblPassword);

            // TextBox Password (textBox2)
            textBox2.Location = new Point(30, 125);
            textBox2.Size = new Size(290, 30);
            textBox2.Font = new Font("Arial", 10);
            textBox2.BorderStyle = BorderStyle.FixedSingle;
            panelLogin.Controls.Add(textBox2);

            // Tombol Login
            Button btnLoginPanel = new Button();
            btnLoginPanel.Text = "LOGIN";
            btnLoginPanel.Size = new Size(130, 40);
            btnLoginPanel.Location = new Point(30, 190);
            btnLoginPanel.BackColor = Color.FromArgb(0, 128, 0);
            btnLoginPanel.ForeColor = Color.White;
            btnLoginPanel.Font = new Font("Arial", 10, FontStyle.Bold);
            btnLoginPanel.FlatStyle = FlatStyle.Flat;
            btnLoginPanel.FlatAppearance.BorderSize = 0;
            btnLoginPanel.Click += btnLogin_Click;
            panelLogin.Controls.Add(btnLoginPanel);

            // Label copyright
            Label lblCopyright = new Label();
            lblCopyright.Text = "© 2026 Aplikasi Zakat Fitrah";
            lblCopyright.Font = new Font("Arial", 8);
            lblCopyright.ForeColor = Color.LightGreen;
            lblCopyright.AutoSize = false;
            lblCopyright.Size = new Size(350, 20);
            lblCopyright.Location = new Point(50, 530);
            lblCopyright.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblCopyright);

            
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
    }
}
