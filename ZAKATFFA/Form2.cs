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