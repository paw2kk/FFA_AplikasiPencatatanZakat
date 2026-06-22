namespace ZAKATFFA
{
    partial class FormReportViewer
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblPilihReport = new System.Windows.Forms.Label();
            this.cmbJenisReport = new System.Windows.Forms.ComboBox();
            this.btnTutup = new System.Windows.Forms.Button();
            this.btnTampilkanReport = new System.Windows.Forms.Button();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnCetak = new System.Windows.Forms.Button();
            this.panelToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPilihReport
            // 
            this.lblPilihReport.AutoSize = true;
            this.lblPilihReport.Location = new System.Drawing.Point(11, 14);
            this.lblPilihReport.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPilihReport.Name = "lblPilihReport";
            this.lblPilihReport.Size = new System.Drawing.Size(71, 13);
            this.lblPilihReport.TabIndex = 0;
            this.lblPilihReport.Text = "Pilih Laporan:";
            // 
            // cmbJenisReport
            // 
            this.cmbJenisReport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJenisReport.Items.AddRange(new object[] {
            "Daftar Pembayaran Zakat (Detail)",
            "Rekap Zakat Bulanan"});
            this.cmbJenisReport.Location = new System.Drawing.Point(98, 11);
            this.cmbJenisReport.Margin = new System.Windows.Forms.Padding(2);
            this.cmbJenisReport.Name = "cmbJenisReport";
            this.cmbJenisReport.Size = new System.Drawing.Size(196, 21);
            this.cmbJenisReport.TabIndex = 1;
            // 
            // btnTutup
            // 
            this.btnTutup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTutup.Location = new System.Drawing.Point(742, 10);
            this.btnTutup.Margin = new System.Windows.Forms.Padding(2);
            this.btnTutup.Name = "btnTutup";
            this.btnTutup.Size = new System.Drawing.Size(71, 22);
            this.btnTutup.TabIndex = 3;
            this.btnTutup.Text = "Tutup";
            this.btnTutup.UseVisualStyleBackColor = true;
            this.btnTutup.Click += new System.EventHandler(this.btnTutup_Click);
            // 
            // btnTampilkanReport
            // 
            this.btnTampilkanReport.Location = new System.Drawing.Point(310, 10);
            this.btnTampilkanReport.Margin = new System.Windows.Forms.Padding(2);
            this.btnTampilkanReport.Name = "btnTampilkanReport";
            this.btnTampilkanReport.Size = new System.Drawing.Size(105, 22);
            this.btnTampilkanReport.TabIndex = 2;
            this.btnTampilkanReport.Text = "Tampilkan";
            this.btnTampilkanReport.UseVisualStyleBackColor = true;
            // 
            // panelToolbar
            // 
            this.panelToolbar.Controls.Add(this.btnTampilkanReport);
            this.panelToolbar.Controls.Add(this.btnTutup);
            this.panelToolbar.Controls.Add(this.cmbJenisReport);
            this.panelToolbar.Controls.Add(this.lblPilihReport);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelToolbar.Margin = new System.Windows.Forms.Padding(2);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(825, 41);
            this.panelToolbar.TabIndex = 1;
            // 
            // crystalReportViewer1
            // 
            this.crystalReportViewer1.ActiveViewIndex = -1;
            this.crystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.crystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crystalReportViewer1.Location = new System.Drawing.Point(0, 41);
            this.crystalReportViewer1.Margin = new System.Windows.Forms.Padding(2);
            this.crystalReportViewer1.Name = "crystalReportViewer1";
            this.crystalReportViewer1.ShowLogo = false;
            this.crystalReportViewer1.Size = new System.Drawing.Size(825, 528);
            this.crystalReportViewer1.TabIndex = 0;
            this.crystalReportViewer1.ToolPanelWidth = 150;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(150, 76);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1758, 926);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick_1);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(635, 47);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "lblStatus";
            // 
            // btnCetak
            // 
            this.btnCetak.Location = new System.Drawing.Point(500, 47);
            this.btnCetak.Name = "btnCetak";
            this.btnCetak.Size = new System.Drawing.Size(75, 23);
            this.btnCetak.TabIndex = 4;
            this.btnCetak.Text = "Cetak";
            this.btnCetak.UseVisualStyleBackColor = true;
            this.btnCetak.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormReportViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 569);
            this.Controls.Add(this.btnCetak);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.crystalReportViewer1);
            this.Controls.Add(this.panelToolbar);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormReportViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Laporan - Aplikasi Pencatatan Zakat";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormReportViewer_Load);
            this.panelToolbar.ResumeLayout(false);
            this.panelToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPilihReport;
        private System.Windows.Forms.ComboBox cmbJenisReport;
        private System.Windows.Forms.Button btnTutup;
        private System.Windows.Forms.Button btnTampilkanReport;
        private System.Windows.Forms.Panel panelToolbar;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnCetak;
    }
}
