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
