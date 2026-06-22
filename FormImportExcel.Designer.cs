namespace ZAKATFFA
{
    partial class FormImportExcel
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
            this.lblJudul = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnPilihFile = new System.Windows.Forms.Button();
            this.dgvPreview = new System.Windows.Forms.DataGridView();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnTutup = new System.Windows.Forms.Button();
            this.btnUnduhTemplate = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
            this.SuspendLayout();
            //
            // lblJudul
            //
            this.lblJudul.AutoSize = true;
            this.lblJudul.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.lblJudul.Location = new System.Drawing.Point(20, 15);
            this.lblJudul.Name = "lblJudul";
            this.lblJudul.Size = new System.Drawing.Size(400, 24);
            this.lblJudul.TabIndex = 0;
            this.lblJudul.Text = "Import Data Pembayaran Zakat dari Excel";
            //
            // txtFilePath
            //
            this.txtFilePath.Location = new System.Drawing.Point(20, 55);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(560, 22);
            this.txtFilePath.TabIndex = 1;
            //
            // btnPilihFile
            //
            this.btnPilihFile.Location = new System.Drawing.Point(590, 54);
            this.btnPilihFile.Name = "btnPilihFile";
            this.btnPilihFile.Size = new System.Drawing.Size(110, 26);
            this.btnPilihFile.TabIndex = 2;
            this.btnPilihFile.Text = "Pilih File...";
            this.btnPilihFile.UseVisualStyleBackColor = true;
            this.btnPilihFile.Click += new System.EventHandler(this.btnPilihFile_Click);
            //
            // btnUnduhTemplate
            //
            this.btnUnduhTemplate.Location = new System.Drawing.Point(710, 54);
            this.btnUnduhTemplate.Name = "btnUnduhTemplate";
            this.btnUnduhTemplate.Size = new System.Drawing.Size(150, 26);
            this.btnUnduhTemplate.TabIndex = 3;
            this.btnUnduhTemplate.Text = "Unduh Template";
            this.btnUnduhTemplate.UseVisualStyleBackColor = true;
            this.btnUnduhTemplate.Click += new System.EventHandler(this.btnUnduhTemplate_Click);
            //
            // dgvPreview
            //
            this.dgvPreview.AllowUserToAddRows = false;
            this.dgvPreview.AllowUserToDeleteRows = false;
            this.dgvPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreview.Location = new System.Drawing.Point(20, 95);
            this.dgvPreview.Name = "dgvPreview";
            this.dgvPreview.ReadOnly = true;
            this.dgvPreview.RowHeadersWidth = 51;
            this.dgvPreview.Size = new System.Drawing.Size(840, 380);
            this.dgvPreview.TabIndex = 4;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(20, 485);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(300, 16);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Pilih file Excel (.xlsx) untuk mulai import.";
            //
            // progressBar1
            //
            this.progressBar1.Location = new System.Drawing.Point(20, 510);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(840, 18);
            this.progressBar1.TabIndex = 6;
            //
            // btnImport
            //
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(665, 540);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(110, 32);
            this.btnImport.TabIndex = 7;
            this.btnImport.Text = "Import Sekarang";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            //
            // btnTutup
            //
            this.btnTutup.Location = new System.Drawing.Point(750, 540);
            this.btnTutup.Name = "btnTutup";
            this.btnTutup.Size = new System.Drawing.Size(110, 32);
            this.btnTutup.TabIndex = 8;
            this.btnTutup.Text = "Tutup";
            this.btnTutup.UseVisualStyleBackColor = true;
            this.btnTutup.Click += new System.EventHandler(this.btnTutup_Click);
            //
            // FormImportExcel
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 590);
            this.Controls.Add(this.btnTutup);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.dgvPreview);
            this.Controls.Add(this.btnUnduhTemplate);
            this.Controls.Add(this.btnPilihFile);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.lblJudul);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormImportExcel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Excel - Pembayaran Zakat";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblJudul;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnPilihFile;
        private System.Windows.Forms.DataGridView dgvPreview;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnTutup;
        private System.Windows.Forms.Button btnUnduhTemplate;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}
