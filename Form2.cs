// ╔══════════════════════════════════════════════════════════════════════╗
// ║  Form2.cs — Aplikasi Zakat Fitrah (Versi Final)                       ║
// ║  Ringkasan perubahan dari versi sebelumnya:                           ║
// ║  1. Validasi No HP wajib format +62 (prefix terkunci, tak bisa hapus) ║
// ║  2. Total Bayar dihitung otomatis & read-only:                        ║
// ║       Beras → jumlah_jiwa × 2,5 kg                                    ║
// ║       Uang  → jumlah_jiwa × Rp 15.000                                 ║
// ║  3. BindingNavigator ↔ DataGridView1 sinkron dua arah penuh           ║
// ║     (geser navigator → baris grid ikut ter-select & scroll;           ║
// ║      klik baris grid → field form ikut terisi)                       ║
// ║  4. Konstanta tarif disatukan di satu tempat (TARIF_BERAS_KG /        ║
// ║     TARIF_UANG_RP) agar konsisten dengan sp_ImportPembayaran di SQL   ║
// ║  5. id_pembayaran diambil dari BindingSource, bukan dari grid         ║
// ║  6. Parameter SP konsisten dengan nama di database                   ║
// ╚══════════════════════════════════════════════════════════════════════╝

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ZAKATFFA
{
    public partial class Form2 : Form
    {
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // KONSTANTA TARIF
        // Disatukan di satu tempat agar logika form selalu konsisten
        // dengan logika perhitungan ulang di sp_ImportPembayaran (SQL).
        // Jika tarif zakat berubah tahun depan, cukup ubah di sini DAN
        // di stored procedure sp_ImportPembayaran.
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        private const decimal TARIF_BERAS_KG = 2.5m;
        private const decimal TARIF_UANG_RP = 15000m;

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // FIELDS PRIVATE
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private const string CONNECTION_STRING =
            @"Server=LAPTOP-1QL3V291\PAW;Database=zakatdb;User Id=sa;Password=123;TrustServerCertificate=True;";

        // Satu BindingSource menjadi "hub" — menghubungkan DataGridView,
        // BindingNavigator, dan form input secara bersamaan.
        private readonly BindingSource _bindingSource = new BindingSource();
        private readonly DataTable _dt = new DataTable();

        // Mencegah HitungTotalBayar() menimpa nilai DB saat navigasi sedang berjalan
        private bool _isNavigating = false;
        // Mencegah rekursi tak terbatas di txtNoHP_TextChanged
        private bool _isSettingNoHP = false;
        // Mencegah loop balik saat sinkronisasi grid -> binding source
        private bool _isSyncingGrid = false;


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // KONSTRUKTOR
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        public Form2()
        {
            InitializeComponent();
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // FORM LOAD
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void Form2_Load(object sender, EventArgs e)
        {
            TerapkanStyling();
            SetupBindingNavigator();
            SetupDefaultForm();
            MuatData();
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // SETUP: BINDING NAVIGATOR & BINDING SOURCE
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void SetupBindingNavigator()
        {
            // Designer otomatis membuat DataBindings ke muzakkiBindingSource (typed dataset).
            // Kita hapus semua itu karena kita pakai pendekatan ADO.NET manual dengan
            // _bindingSource sebagai satu-satunya sumber kebenaran.
            txtNama.DataBindings.Clear();
            txtAlamat.DataBindings.Clear();
            txtNoHP.DataBindings.Clear();

            // Alihkan BindingNavigator agar menunjuk ke _bindingSource kita,
            // bukan ke muzakkiBindingSource bawaan Designer.
            bindingNavigator1.BindingSource = _bindingSource;

            // Sembunyikan tombol Add/Delete bawaan navigator —
            // sudah ada tombol Tambah dan Hapus tersendiri di form.
            if (bindingNavigatorAddNewItem != null) bindingNavigatorAddNewItem.Visible = false;
            if (bindingNavigatorDeleteItem != null) bindingNavigatorDeleteItem.Visible = false;

            // Handler ini dipanggil setiap kali posisi navigator berubah,
            // baik lewat tombol navigator (geser <</>>) maupun klik baris di grid
            // (karena DataGridView1.DataSource = _bindingSource yang sama).
            _bindingSource.PositionChanged += BindingSource_PositionChanged;
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // SETUP: DEFAULT NILAI FORM
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void SetupDefaultForm()
        {
            // ── ComboBox ──────────────────────────────────────────────────
            cmbJenisBerasAtauUang.Items.Clear();
            cmbJenisBerasAtauUang.Items.Add("beras");
            cmbJenisBerasAtauUang.Items.Add("uang");
            cmbJenisBerasAtauUang.DropDownStyle = ComboBoxStyle.DropDownList; // cegah user ketik manual
            cmbJenisBerasAtauUang.SelectedIndex = 0;

            // ── No HP: awali dengan +62, prefix tidak bisa dihapus ────────
            txtNoHP.Text = "+62";
            txtNoHP.Enter += txtNoHP_Enter;
            txtNoHP.KeyPress += txtNoHP_KeyPress;
            txtNoHP.TextChanged += txtNoHP_TextChanged;
            txtNoHP.MaxLength = 16; // +62 + maksimal 13 digit

            // ── Total Bayar: hanya baca, nilai dihitung otomatis ──────────
            // Dikunci penuh: ReadOnly mencegah ketik manual, TabStop dimatikan
            // supaya tab/fokus tidak berhenti di kolom yang memang tak boleh diubah.
            txtBayar.ReadOnly = true;
            txtBayar.TabStop = false;
            txtBayar.BackColor = Color.FromArgb(215, 215, 215);
            txtBayar.ForeColor = Color.FromArgb(60, 60, 60);
            txtBayar.Cursor = Cursors.No;

            // ── DateTimePicker ────────────────────────────────────────────
            dtp1.Value = DateTime.Today;
            dtp1.MinDate = new DateTime(DateTime.Now.Year - 3, 1, 1);
            dtp1.MaxDate = DateTime.Today; // tidak boleh input tanggal masa depan

            // ── Wire auto-kalkulasi total bayar ───────────────────────────
            // Dipicu setiap kali user mengubah jumlah jiwa atau jenis pembayaran.
            cmbJenisBerasAtauUang.SelectedIndexChanged += (s, ev) => HitungTotalBayar();
            txtJumlahJiwa.TextChanged += (s, ev) => HitungTotalBayar();

            // ── Jumlah Jiwa: hanya boleh angka ─────────────────────────────
            txtJumlahJiwa.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && !char.IsControl(ev.KeyChar))
                    ev.Handled = true;
            };
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // AUTO-KALKULASI: TOTAL BAYAR (READ-ONLY)
        // Aturan zakat fitrah:
        //   Beras → jumlah_jiwa × 2,5 kg
        //   Uang  → jumlah_jiwa × Rp 15.000
        // Nilai ini bersifat final — user tidak bisa mengubahnya secara
        // manual dari UI (txtBayar.ReadOnly = true), sehingga satu-satunya
        // cara nilai berubah adalah lewat method ini.
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void HitungTotalBayar()
        {
            // Jangan jalankan saat sedang navigasi/sinkron grid agar nilai
            // dari database tidak tertimpa oleh hasil kalkulasi form kosong.
            if (_isNavigating || _isSyncingGrid) return;
            if (cmbJenisBerasAtauUang.SelectedItem == null) return;

            if (!int.TryParse(txtJumlahJiwa.Text.Trim(), out int jiwa) || jiwa <= 0)
            {
                txtBayar.Text = string.Empty;
                return;
            }

            string jenis = cmbJenisBerasAtauUang.SelectedItem.ToString();
            txtBayar.Text = jenis == "beras"
                ? (jiwa * TARIF_BERAS_KG).ToString("0.##")  // satuan kg, contoh: 10 atau 12.5
                : (jiwa * TARIF_UANG_RP).ToString("0");      // satuan rupiah, contoh: 60000
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // EVENT: BINDING SOURCE POSITION CHANGED
        // Dipicu oleh: tombol navigasi BindingNavigator ATAU klik baris DataGridView
        // (karena keduanya terikat ke _bindingSource yang sama).
        // Tugas: isi field form dari baris aktif, lalu sinkronkan seleksi grid
        // agar baris yang sedang ditampilkan di form selalu ter-highlight di grid.
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void BindingSource_PositionChanged(object sender, EventArgs e)
        {
            if (!(_bindingSource.Current is DataRowView row)) return;

            // Naikkan flag agar HitungTotalBayar() tidak menimpa nilai dari DB
            _isNavigating = true;
            try
            {
                txtNama.Text = row["nama"]?.ToString() ?? string.Empty;
                txtAlamat.Text = row["alamat"]?.ToString() ?? string.Empty;
                txtNoHP.Text = row["no_hp"]?.ToString() ?? "+62";
                txtJumlahJiwa.Text = row["jumlah_jiwa"]?.ToString() ?? string.Empty;
                txtBayar.Text = row["total_bayar"]?.ToString() ?? string.Empty;

                if (row["tanggal"] != DBNull.Value)
                    dtp1.Value = Convert.ToDateTime(row["tanggal"]);

                string jenis = row["jenis_pembayaran"]?.ToString() ?? string.Empty;
                int idx = cmbJenisBerasAtauUang.Items.IndexOf(jenis);
                if (idx >= 0) cmbJenisBerasAtauUang.SelectedIndex = idx;
            }
            finally
            {
                _isNavigating = false;
            }

            // ── Sinkronkan seleksi baris di DataGridView1 ──────────────────
            // Inilah bagian yang memastikan: kalau navigator digeser,
            // baris yang sesuai di datagridview1 ikut ter-select & ter-scroll.
            _isSyncingGrid = true;
            try
            {
                int pos = _bindingSource.Position;
                if (pos >= 0 && pos < dataGridView1.Rows.Count)
                {
                    if (!dataGridView1.Rows[pos].Selected)
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[pos].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[pos].Cells[
                            dataGridView1.Columns["nama"]?.Index ?? 0];
                    }
                    try { dataGridView1.FirstDisplayedScrollingRowIndex = pos; }
                    catch { /* Abaikan jika grid sedang kosong/loading */ }
                }
            }
            finally
            {
                _isSyncingGrid = false;
            }
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // VALIDASI NOMOR HP: FORMAT +62
        // Prefix "+62" tidak bisa dihapus oleh user dalam kondisi apapun,
        // dan hanya digit yang boleh diketik setelahnya.
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void txtNoHP_Enter(object sender, EventArgs e)
        {
            if (!txtNoHP.Text.StartsWith("+62"))
                txtNoHP.Text = "+62";
            txtNoHP.SelectionStart = txtNoHP.Text.Length;
        }

        private void txtNoHP_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool isBackspace = (e.KeyChar == (char)Keys.Back);
            bool isDigit = char.IsDigit(e.KeyChar);

            // Hanya izinkan karakter digit (0-9) dan Backspace
            if (!isDigit && !isBackspace)
            {
                e.Handled = true;
                return;
            }

            // Blokir Backspace jika panjang teks sudah di batas "+62" (3 karakter),
            // supaya prefix tidak bisa terhapus.
            if (isBackspace && txtNoHP.Text.Length <= 3)
                e.Handled = true;
        }

        private void txtNoHP_TextChanged(object sender, EventArgs e)
        {
            // Safety net tambahan: cegah loop rekursif, pastikan +62 tidak hilang
            // walau lewat cara lain seperti paste (Ctrl+V) atau drag-drop teks.
            if (_isSettingNoHP) return;

            _isSettingNoHP = true;
            try
            {
                string teks = txtNoHP.Text;

                if (!teks.StartsWith("+62"))
                {
                    txtNoHP.Text = "+62";
                    txtNoHP.SelectionStart = txtNoHP.Text.Length;
                    return;
                }

                // Buang karakter non-digit yang mungkin masuk lewat paste,
                // tapi tetap pertahankan prefix +62.
                string sisaAngka = teks.Substring(3);
                string sisaBersih = Regex.Replace(sisaAngka, "[^0-9]", "");
                if (sisaAngka != sisaBersih)
                {
                    int posKursor = txtNoHP.SelectionStart;
                    txtNoHP.Text = "+62" + sisaBersih;
                    txtNoHP.SelectionStart = Math.Min(posKursor, txtNoHP.Text.Length);
                }
            }
            finally
            {
                _isSettingNoHP = false;
            }
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // VALIDASI FORM (terpusat, dipanggil sebelum Tambah & Update)
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private bool ValidasiForm(out string pesan)
        {
            pesan = string.Empty;

            // Nama: tidak boleh kosong, hanya huruf + spasi + apostrof
            if (string.IsNullOrWhiteSpace(txtNama.Text))
            { pesan = "Nama tidak boleh kosong."; return false; }

            if (!Regex.IsMatch(txtNama.Text.Trim(), @"^[A-Za-z' ]+$"))
            { pesan = "Nama hanya boleh berisi huruf (A-Z / a-z) dan tanda apostrof (')."; return false; }

            // Alamat
            if (string.IsNullOrWhiteSpace(txtAlamat.Text))
            { pesan = "Alamat tidak boleh kosong."; return false; }

            // No HP: wajib format +62 diikuti 8 sampai 13 digit
            // Contoh valid  : +6281234567890  (13 digit setelah +62)
            // Contoh invalid: 08123456789, +62abc, +621
            if (!Regex.IsMatch(txtNoHP.Text.Trim(), @"^\+62[0-9]{8,13}$"))
            {
                pesan = "Nomor HP harus berformat +62 diikuti 8–13 digit angka.\n" +
                        "Contoh benar : +6281234567890";
                return false;
            }

            // Jumlah jiwa: harus angka bulat positif
            if (!int.TryParse(txtJumlahJiwa.Text, out int jiwa) || jiwa <= 0)
            { pesan = "Jumlah jiwa harus berupa angka bulat lebih dari 0."; return false; }

            // Jenis pembayaran
            if (cmbJenisBerasAtauUang.SelectedItem == null)
            { pesan = "Pilih jenis pembayaran (beras atau uang) terlebih dahulu."; return false; }

            // Total bayar: dihitung otomatis oleh sistem, tapi tetap divalidasi
            // sebagai jaring pengaman terakhir sebelum dikirim ke database.
            if (!decimal.TryParse(txtBayar.Text, out decimal total) || total <= 0)
            { pesan = "Total bayar tidak valid. Pastikan kolom Jumlah Jiwa sudah terisi dengan benar."; return false; }

            // Tanggal tidak boleh di masa depan
            if (dtp1.Value.Date > DateTime.Today)
            { pesan = "Tanggal pembayaran tidak boleh melebihi hari ini."; return false; }

            return true;
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // TOMBOL: TAMBAH DATA
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void btnTambah_Click(object sender, EventArgs e)
        {
            if (!ValidasiForm(out string pesan))
            {
                MessageBox.Show(pesan, "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var kon = new SqlConnection(CONNECTION_STRING))
                {
                    kon.Open();
                    using (var cmd = new SqlCommand("sp_TambahPembayaran", kon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@nama", txtNama.Text.Trim());
                        cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text.Trim());
                        cmd.Parameters.AddWithValue("@no_hp", txtNoHP.Text.Trim());
                        cmd.Parameters.AddWithValue("@tanggal", dtp1.Value.Date);
                        cmd.Parameters.AddWithValue("@jumlah_jiwa", Convert.ToInt32(txtJumlahJiwa.Text));
                        cmd.Parameters.AddWithValue("@total_bayar", Convert.ToDecimal(txtBayar.Text));
                        cmd.Parameters.AddWithValue("@jenis_pembayaran", cmbJenisBerasAtauUang.SelectedItem.ToString());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data muzakki dan transaksi pembayaran berhasil disimpan!",
                    "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                KosongkanForm();
                MuatData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan saat menambah data:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // TOMBOL: UPDATE DATA
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current == null)
            {
                MessageBox.Show("Silakan pilih data yang ingin diubah pada tabel terlebih dahulu!",
                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidasiForm(out string pesan))
            {
                MessageBox.Show(pesan, "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // id_pembayaran diambil dari BindingSource, bukan dari kolom DataGridView.
                // Ini lebih aman karena tidak bergantung pada urutan kolom grid.
                var row = (DataRowView)_bindingSource.Current;
                int idPembayaran = Convert.ToInt32(row["id_pembayaran"]);

                using (var kon = new SqlConnection(CONNECTION_STRING))
                {
                    kon.Open();
                    using (var cmd = new SqlCommand("sp_UpdatePembayaran", kon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_pembayaran", idPembayaran);
                        cmd.Parameters.AddWithValue("@nama", txtNama.Text.Trim());
                        cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text.Trim());
                        cmd.Parameters.AddWithValue("@no_hp", txtNoHP.Text.Trim());
                        cmd.Parameters.AddWithValue("@tanggal", dtp1.Value.Date);
                        cmd.Parameters.AddWithValue("@jumlah_jiwa", Convert.ToInt32(txtJumlahJiwa.Text));
                        cmd.Parameters.AddWithValue("@total_bayar", Convert.ToDecimal(txtBayar.Text));
                        cmd.Parameters.AddWithValue("@jenis_pembayaran", cmbJenisBerasAtauUang.SelectedItem.ToString());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data transaksi dan profil muzakki berhasil diperbarui!",
                    "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                MuatData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan saat memperbarui data:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // TOMBOL: HAPUS DATA
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current == null)
            {
                MessageBox.Show("Silakan pilih data pada tabel terlebih dahulu yang ingin dihapus!",
                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = (DataRowView)_bindingSource.Current;
            int idPembayaran = Convert.ToInt32(row["id_pembayaran"]);
            string namaMuzakki = row["nama"]?.ToString() ?? string.Empty;

            DialogResult konfirmasi = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus data pembayaran\nID #{idPembayaran} atas nama {namaMuzakki}?",
                "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (konfirmasi != DialogResult.Yes) return;

            try
            {
                using (var kon = new SqlConnection(CONNECTION_STRING))
                {
                    kon.Open();
                    using (var cmd = new SqlCommand("sp_HapusPembayaran", kon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_pembayaran", idPembayaran);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil dihapus!", "Sukses",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                KosongkanForm();
                MuatData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan saat menghapus data:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // TOMBOL: TAMPIL DATA
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void btnTampilData_Click(object sender, EventArgs e)
        {
            MuatData();
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // METHOD INTI: MUAT DATA DARI DATABASE
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void MuatData()
        {
            try
            {
                _dt.Clear();
                using (var kon = new SqlConnection(CONNECTION_STRING))
                using (var da = new SqlDataAdapter(
                    "SELECT * FROM v_pembayaran_zakat ORDER BY id_pembayaran DESC", kon))
                {
                    da.Fill(_dt);
                }

                // Konfigurasi kolom hanya satu kali (saat pertama kali load)
                if (dataGridView1.Columns.Count == 0)
                    KonfigurasiKolomGrid();

                // Hubungkan DataGridView ke _bindingSource.
                // Dengan ini: klik baris di grid  → _bindingSource.Position berubah → PositionChanged
                //             geser navigator     → _bindingSource.Position berubah → PositionChanged
                // Kedua arah sinkron otomatis lewat satu event handler yang sama.
                _bindingSource.DataSource = _dt;
                dataGridView1.DataSource = _bindingSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data dari database:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // KONFIGURASI KOLOM DATAGRIDVIEW
        // id_pembayaran & id_muzakki dibuat hidden agar tidak tampil ke user,
        // tapi tetap bisa diakses lewat BindingSource untuk keperluan CRUD.
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void KonfigurasiKolomGrid()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true; // edit data hanya lewat form, bukan langsung di grid

            void Kolom(string name, string header, int weight, bool visible = true)
            {
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = name,
                    HeaderText = header,
                    DataPropertyName = name,
                    FillWeight = weight,
                    Visible = visible
                });
            }

            Kolom("id_pembayaran", "ID Bayar", 3, false); // HIDDEN — kunci update/hapus
            Kolom("id_muzakki", "ID Muzakki", 3, false); // HIDDEN
            Kolom("nama", "Nama", 18);
            Kolom("alamat", "Alamat", 22);
            Kolom("no_hp", "No. HP", 13);
            Kolom("tanggal", "Tanggal", 10);
            Kolom("jumlah_jiwa", "Jiwa", 6);
            Kolom("jenis_pembayaran", "Jenis", 8);
            Kolom("total_bayar", "Total Bayar", 12);
            Kolom("jumlah_dengan_satuan", "Keterangan", 14);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // TOMBOL: CARI DATA
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void btnCari_Click(object sender, EventArgs e)
        {
            try
            {
                _dt.Clear();
                using (var kon = new SqlConnection(CONNECTION_STRING))
                using (var da = new SqlDataAdapter("sp_CariPembayaran", kon))
                {
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.AddWithValue("@nama", txtCari.Text.Trim());
                    da.Fill(_dt);
                }

                _bindingSource.DataSource = _dt;
                dataGridView1.DataSource = _bindingSource;

                if (_dt.Rows.Count == 0)
                    MessageBox.Show("Data dengan nama tersebut tidak ditemukan.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mencari data:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // HELPER: KOSONGKAN FORM INPUT
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void KosongkanForm()
        {
            txtNama.Text = string.Empty;
            txtAlamat.Text = string.Empty;
            txtNoHP.Text = "+62";          // Awali kembali dengan prefix +62
            txtJumlahJiwa.Text = string.Empty;
            txtBayar.Text = string.Empty;
            dtp1.Value = DateTime.Today;
            cmbJenisBerasAtauUang.SelectedIndex = 0;
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // TOMBOL: IMPORT EXCEL
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            new FormImportExcel().ShowDialog();
            MuatData();
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // TOMBOL: CETAK REPORT (Crystal Reports)
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void btnCetakReport_Click(object sender, EventArgs e)
        {
            new FormReportViewer().ShowDialog();
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // TOMBOL: LOGOUT
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                new Form1().Show();
                this.Close();
            }
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // EVENT: FORM DITUTUP
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            bool adaFormLain = false;
            foreach (Form f in Application.OpenForms)
                if (f != this && f.Visible) { adaFormLain = true; break; }
            if (!adaFormLain) Application.Exit();
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // STYLING FORM
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void TerapkanStyling()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Text = "Aplikasi Zakat - Data Pembayaran";
            this.BackColor = Color.FromArgb(0, 100, 0);

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.ForeColor = Color.White;
                    lbl.Font = new Font("Arial", 9, FontStyle.Bold);
                }
                else if (ctrl is TextBox txt && txt.Name != nameof(txtBayar))
                {
                    txt.BackColor = Color.White;
                    txt.ForeColor = Color.Black;
                    txt.Font = new Font("Arial", 9);
                }
            }

            StyleButton(btnTampilData, Color.FromArgb(0, 128, 0));
            StyleButton(btnTambah, Color.FromArgb(0, 128, 0));
            StyleButton(btnHapus, Color.FromArgb(180, 0, 0));
            StyleButton(btnUpdate, Color.FromArgb(180, 130, 0));
            StyleButton(btnLogOut, Color.FromArgb(70, 70, 70));

            // ── DataGridView ──────────────────────────────────────────────
            dataGridView1.BackgroundColor = Color.FromArgb(0, 80, 0);
            dataGridView1.GridColor = Color.LightGreen;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                                 | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.Width = this.ClientSize.Width - (dataGridView1.Left * 2);
            dataGridView1.Height = this.ClientSize.Height - dataGridView1.Top - 40;

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 60, 0);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.EnableHeadersVisualStyles = false;

            dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(240, 255, 240);
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Font = new Font("Arial", 9);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 150, 0);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(210, 240, 210);

            // ── DateTimePicker ────────────────────────────────────────────
            dtp1.CalendarMonthBackground = Color.FromArgb(0, 100, 0);
            dtp1.CalendarForeColor = Color.White;
            dtp1.CalendarTitleBackColor = Color.FromArgb(0, 60, 0);
            dtp1.CalendarTitleForeColor = Color.White;

            // ── ComboBox ──────────────────────────────────────────────────
            cmbJenisBerasAtauUang.BackColor = Color.White;
            cmbJenisBerasAtauUang.ForeColor = Color.Black;
            cmbJenisBerasAtauUang.Font = new Font("Arial", 9);

            // ── Logo MUI ──────────────────────────────────────────────────
            var picLogo = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(10, 35),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent
            };
            try { picLogo.Image = Image.FromFile(Application.StartupPath + @"\logo_mui.png"); } catch { }
            this.Controls.Add(picLogo);
            picLogo.BringToFront();

            // ── Label Judul ───────────────────────────────────────────────
            var lblJudul = new Label
            {
                Text = "APLIKASI ZAKAT FITRAH",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(400, 35),
                Location = new Point(75, 50),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblJudul);
            lblJudul.BringToFront();

            // ── Garis Pemisah Header ──────────────────────────────────────
            var garis = new Panel
            {
                Size = new Size(this.Width, 3),
                Location = new Point(0, 83),
                BackColor = Color.LightGreen
            };
            this.Controls.Add(garis);
            garis.BringToFront();

            // ── Label Copyright ───────────────────────────────────────────
            var lblCopyright = new Label
            {
                Text = "© 2026 Aplikasi Zakat Fitrah - Majelis Ulama Indonesia",
                Font = new Font("Arial", 8),
                ForeColor = Color.LightGreen,
                AutoSize = false,
                Size = new Size(500, 20),
                Location = new Point(10, this.Height - 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblCopyright);
            lblCopyright.BringToFront();
        }

        private void StyleButton(Button btn, Color color)
        {
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Arial", 9, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
        }


        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // EVENT STUBS (Diperlukan karena terdaftar di Designer)
        // Klik baris di grid otomatis memindahkan posisi _bindingSource,
        // yang memicu BindingSource_PositionChanged di atas — jadi tidak
        // perlu logic tambahan di sini.
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void label2_Click(object sender, EventArgs e) { }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}   