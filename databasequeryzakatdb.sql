-- Buat Database
CREATE DATABASE zakatdb;
GO
USE zakatdb;
GO

-- Tabel PENGGUNA
CREATE TABLE pengguna (
    id_user   INT PRIMARY KEY IDENTITY(1,1),
    nama      VARCHAR(100) NOT NULL,
    password  VARCHAR(255) NOT NULL
);

-- Tabel MUZAKKI
CREATE TABLE muzakki (
    id_muzakki  INT PRIMARY KEY IDENTITY(1,1),
    nama        VARCHAR(100) NOT NULL,
    alamat      VARCHAR(255) NOT NULL,
    no_hp       VARCHAR(15)  NOT NULL
);

-- Tabel PEMBAYARAN_ZAKAT
CREATE TABLE pembayaran_zakat (
    id_pembayaran     INT PRIMARY KEY IDENTITY(1,1),
    id_muzakki        INT             NOT NULL,
    tanggal           DATE            NOT NULL,
    jumlah_jiwa       INT             NOT NULL,
    jumlah_uang       DECIMAL(15, 2)  NULL,  -- dalam rupiah
    jumlah_beras      DECIMAL(10, 2)  NULL,  -- dalam kilogram
    total_bayar       DECIMAL(15, 2)  NOT NULL,
    jenis_pembayaran  VARCHAR(5)      NOT NULL,

    -- Foreign Key
    CONSTRAINT fk_muzakki
        FOREIGN KEY (id_muzakki)
        REFERENCES muzakki(id_muzakki)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    -- Validasi jenis_pembayaran hanya 'beras' atau 'uang'
    CONSTRAINT chk_jenis_pembayaran
        CHECK (jenis_pembayaran IN ('beras', 'uang')),

    -- Validasi jika beras maka jumlah_beras wajib diisi
    CONSTRAINT chk_beras
        CHECK (jenis_pembayaran != 'beras' OR (jumlah_beras IS NOT NULL AND jumlah_uang IS NULL)),

    -- Validasi jika uang maka jumlah_uang wajib diisi
    CONSTRAINT chk_uang
        CHECK (jenis_pembayaran != 'uang' OR (jumlah_uang IS NOT NULL AND jumlah_beras IS NULL))
);


select * From pengguna;
select * From muzakki;
select * From pembayaran_zakat;

INSERT INTO pengguna (nama, password) VALUES ('admin', '123');

INSERT INTO pengguna (nama, password) VALUES ('paw', '123');

INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Ahmad Fauzi',     'Jl. Mawar No.1, Wonosobo',   '081234567890');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Budi Santoso',    'Jl. Melati No.5, Wonosobo',  '082345678901');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Citra Dewi',      'Jl. Anggrek No.3, Wonosobo', '083456789012');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Dian Pratama',    'Jl. Kenanga No.7, Wonosobo', '084567890123');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Eko Wahyudi',     'Jl. Dahlia No.2, Wonosobo',  '085678901234');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Fitri Rahayu',    'Jl. Tulip No.9, Wonosobo',   '086789012345');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Galih Permana',   'Jl. Flamboyan No.4, Wonosobo','087890123456');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Hana Sari',       'Jl. Cempaka No.6, Wonosobo', '088901234567');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Irfan Hakim',     'Jl. Sakura No.8, Wonosobo',  '089012345678');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Joko Susilo',     'Jl. Nusa Indah No.10, Wonosobo','081122334455');

-- ===================== DATA DUMMY PEMBAYARAN ZAKAT =====================

-- Pembayaran dengan BERAS (satuannya kg)
INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (1, '2026-04-01', 4, NULL, 10.00, 10.00, 'beras');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (2, '2026-04-02', 3, NULL, 7.50,  7.50,  'beras');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (3, '2026-04-03', 5, NULL, 12.50, 12.50, 'beras');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (4, '2026-04-04', 2, NULL, 5.00,  5.00,  'beras');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (5, '2026-04-05', 6, NULL, 15.00, 15.00, 'beras');

-- Pembayaran dengan UANG (satuannya rupiah)
-- Catatan: zakat fitrah uang = jumlah jiwa x Rp 40.000
INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (6,  '2026-04-06', 4, 160000, NULL, 160000, 'uang');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (7,  '2026-04-07', 3, 120000, NULL, 120000, 'uang');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (8,  '2026-04-08', 5, 200000, NULL, 200000, 'uang');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (9,  '2026-04-09', 2, 80000,  NULL, 80000,  'uang');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (10, '2026-04-10', 7, 280000, NULL, 280000, 'uang');

DELETE FROM pembayaran_zakat;

-- Baru hapus data induk
DELETE FROM muzakki;

DROP DATABASE zakatdb;

ALTER TABLE muzakki
ADD CONSTRAINT chk_no_hp
    CHECK (no_hp NOT LIKE '%[^0-9+]%');

ALTER TABLE muzakki
ADD CONSTRAINT chk_nama
    CHECK (nama NOT LIKE '%[^A-Za-z'' ]%');
 
-- Pembayaran Zakat
IF NOT EXISTS (SELECT 1 FROM pembayaran_zakat)
BEGIN
    INSERT INTO pembayaran_zakat
        (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
    VALUES
        (1,  '2026-04-01', 4, NULL,   10.00,  10.00,  'beras'),
        (2,  '2026-04-02', 3, NULL,    7.50,   7.50,  'beras'),
        (3,  '2026-04-03', 5, NULL,   12.50,  12.50,  'beras'),
        (4,  '2026-04-04', 2, NULL,    5.00,   5.00,  'beras'),
        (5,  '2026-04-05', 6, NULL,   15.00,  15.00,  'beras'),
        (6,  '2026-04-06', 4, 160000, NULL,  160000,  'uang'),
        (7,  '2026-04-07', 3, 120000, NULL,  120000,  'uang'),
        (8,  '2026-04-08', 5, 200000, NULL,  200000,  'uang'),
        (9,  '2026-04-09', 2,  80000, NULL,   80000,  'uang'),
        (10, '2026-04-10', 7, 280000, NULL,  280000,  'uang');
END
GO

CREATE OR ALTER VIEW v_pembayaran_zakat AS
SELECT 
    p.id_pembayaran,
    m.id_muzakki,
    m.nama,
    m.alamat,
    m.no_hp,
    p.tanggal,
    p.jumlah_jiwa,
    p.jenis_pembayaran,
    p.jumlah_uang,
    p.jumlah_beras,
    p.total_bayar,
    CASE 
        WHEN p.jenis_pembayaran = 'beras' THEN CAST(p.jumlah_beras AS VARCHAR) + ' kg'
        WHEN p.jenis_pembayaran = 'uang'  THEN 'Rp ' + FORMAT(p.jumlah_uang, 'N0')
    END AS jumlah_dengan_satuan
FROM pembayaran_zakat p
JOIN muzakki m ON p.id_muzakki = m.id_muzakki;
GO

CREATE OR ALTER PROCEDURE sp_TambahPembayaran
    @nama VARCHAR(100),
    @alamat VARCHAR(255),
    @no_hp VARCHAR(15),
    @tanggal DATE,
    @jumlah_jiwa INT,
    @total_bayar DECIMAL(15,2),
    @jenis_pembayaran VARCHAR(5)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @id_muzakki INT;

        -- Cek apakah muzakki sudah terdaftar
        SELECT @id_muzakki = id_muzakki FROM muzakki WHERE nama = @nama AND no_hp = @no_hp;

        -- Jika belum ada, masukkan ke tabel muzakki
        IF @id_muzakki IS NULL
        BEGIN
            INSERT INTO muzakki (nama, alamat, no_hp) VALUES (@nama, @alamat, @no_hp);
            SET @id_muzakki = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            -- Jika sudah ada, update alamatnya agar sinkron
            UPDATE muzakki SET alamat = @alamat WHERE id_muzakki = @id_muzakki;
        END

        -- Pisahkan nilai berdasarkan jenis pembayaran
        DECLARE @jumlah_uang DECIMAL(15,2) = NULL;
        DECLARE @jumlah_beras DECIMAL(10,2) = NULL;

        IF @jenis_pembayaran = 'uang'
            SET @jumlah_uang = @total_bayar;
        ELSE
            SET @jumlah_beras = @total_bayar;

        -- Masukkan data ke tabel pembayaran
        INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
        VALUES (@id_muzakki, @tanggal, @jumlah_jiwa, @jumlah_uang, @jumlah_beras, @total_bayar, @jenis_pembayaran);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_UpdatePembayaran
    @id_pembayaran INT,
    @nama VARCHAR(100),
    @alamat VARCHAR(255),
    @no_hp VARCHAR(15),
    @tanggal DATE,
    @jumlah_jiwa INT,
    @total_bayar DECIMAL(15,2),
    @jenis_pembayaran VARCHAR(5)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @id_muzakki INT;

        -- Ambil id_muzakki dari relasi pembayaran
        SELECT @id_muzakki = id_muzakki FROM pembayaran_zakat WHERE id_pembayaran = @id_pembayaran;

        -- Update Profil Muzakki
        UPDATE muzakki 
        SET nama = @nama, alamat = @alamat, no_hp = @no_hp 
        WHERE id_muzakki = @id_muzakki;

        -- Atur ulang kolom uang/beras
        DECLARE @jumlah_uang DECIMAL(15,2) = NULL;
        DECLARE @jumlah_beras DECIMAL(10,2) = NULL;

        IF @jenis_pembayaran = 'uang'
            SET @jumlah_uang = @total_bayar;
        ELSE
            SET @jumlah_beras = @total_bayar;

        -- Update Transaksi Pembayaran
        UPDATE pembayaran_zakat 
        SET tanggal = @tanggal,
            jumlah_jiwa = @jumlah_jiwa,
            jumlah_uang = @jumlah_uang,
            jumlah_beras = @jumlah_beras,
            total_bayar = @total_bayar,
            jenis_pembayaran = @jenis_pembayaran
        WHERE id_pembayaran = @id_pembayaran;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_HapusPembayaran
    @id_pembayaran INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Karena tabel menggunakan ON DELETE CASCADE pada foreign key, 
        -- Menghapus data pembayaran langsung diperbolehkan.
        DELETE FROM pembayaran_zakat WHERE id_pembayaran = @id_pembayaran;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO


-- =========================================================
-- 5. STORED PROCEDURE: PENCARIAN AMAN BERBASIS PARAMETER
-- =========================================================
CREATE OR ALTER PROCEDURE sp_CariPembayaran
    @nama VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    -- Mengambil data secara aman dari VIEW yang telah dibuat
    SELECT * FROM v_pembayaran_zakat 
    WHERE nama LIKE '%' + @nama + '%';
END;
GO

use zakatdb

CREATE OR ALTER TRIGGER trg_HitungTotalBayar
ON pembayaran_zakat
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Tarif (sesuaikan dengan Form2.cs)
    DECLARE @TARIF_BERAS DECIMAL(10,2) = 2.5;
    DECLARE @TARIF_UANG  DECIMAL(15,2) = 15000;

    UPDATE p
    SET 
        p.total_bayar = CASE 
            WHEN i.jenis_pembayaran = 'beras' THEN i.jumlah_jiwa * @TARIF_BERAS
            ELSE                                   i.jumlah_jiwa * @TARIF_UANG
        END,
        p.jumlah_beras = CASE 
            WHEN i.jenis_pembayaran = 'beras' THEN i.jumlah_jiwa * @TARIF_BERAS
            ELSE NULL
        END,
        p.jumlah_uang = CASE 
            WHEN i.jenis_pembayaran = 'uang'  THEN i.jumlah_jiwa * @TARIF_UANG
            ELSE NULL
        END
    FROM pembayaran_zakat p
    INNER JOIN inserted i ON p.id_pembayaran = i.id_pembayaran;
END;
GO

/* ============================================================
   FILE: 03_rekap_dan_import.sql
   TUJUAN:
     1. View rekap zakat bulanan (untuk laporan/dashboard)
     2. Stored procedure import data dari Excel — versi diperkuat
        dengan validasi server-side penuh (bukan sekadar percaya
        data yang dikirim form/Excel)
   ============================================================ */

USE zakatdb;
GO

/* ============================================================
   1. VIEW: REKAP ZAKAT BULANAN
   ------------------------------------------------------------
   Dipakai untuk laporan/dashboard. Diurutkan dari bulan terbaru,
   dan ditambah rata-rata per transaksi supaya langsung siap pakai
   tanpa perlu hitung ulang di aplikasi.
   ============================================================ */
CREATE OR ALTER VIEW v_rekap_zakat_bulanan AS
SELECT
    YEAR(p.tanggal)                                              AS tahun,
    MONTH(p.tanggal)                                             AS bulan,
    DATENAME(MONTH, p.tanggal)                                   AS nama_bulan,
    COUNT(*)                                                     AS total_transaksi,
    SUM(p.jumlah_jiwa)                                           AS total_jiwa,
    ISNULL(SUM(CASE WHEN p.jenis_pembayaran = 'beras'
                     THEN p.jumlah_beras ELSE 0 END), 0)         AS total_beras_kg,
    ISNULL(SUM(CASE WHEN p.jenis_pembayaran = 'uang'
                     THEN p.jumlah_uang ELSE 0 END), 0)          AS total_uang_rp,
    SUM(p.total_bayar)                                           AS grand_total,
    -- Rata-rata jiwa per transaksi, berguna untuk insight cepat
    CAST(AVG(CAST(p.jumlah_jiwa AS DECIMAL(10,2))) AS DECIMAL(10,2)) AS rata_jiwa_per_transaksi
FROM pembayaran_zakat p
GROUP BY YEAR(p.tanggal), MONTH(p.tanggal), DATENAME(MONTH, p.tanggal);
GO


/* ============================================================
   2. STORED PROCEDURE: IMPORT PEMBAYARAN (DARI EXCEL)
   ------------------------------------------------------------
   Perbedaan dengan sp_TambahPembayaran biasa:
   - Dipakai untuk proses batch (loop per-baris dari aplikasi saat
     import Excel), jadi WAJIB punya output @status & @pesan agar
     aplikasi bisa lapor baris mana yang gagal tanpa menghentikan
     baris lain.
   - Validasi diperketat karena data dari Excel rawan human error:
       * No HP wajib format +62 diikuti 8-13 digit
       * Nama wajib diisi dan hanya huruf/spasi/apostrof
       * Jumlah jiwa wajib > 0
       * Tanggal tidak boleh di masa depan
       * jenis_pembayaran wajib 'uang' atau 'beras'
   - total_bayar TIDAK dipercaya mentah-mentah dari Excel.
     Server menghitung ulang berdasarkan jumlah_jiwa x tarif,
     supaya konsisten dengan logika di form (mencegah data Excel
     yang salah ketik nilainya lolos ke database).
   ============================================================ */
CREATE OR ALTER PROCEDURE sp_ImportPembayaran
    @nama             VARCHAR(100),
    @alamat           VARCHAR(255),
    @no_hp            VARCHAR(15),
    @tanggal          DATE,
    @jumlah_jiwa      INT,
    @total_bayar      DECIMAL(15,2),   -- tetap diterima untuk kompatibilitas, tapi akan dihitung ulang
    @jenis_pembayaran VARCHAR(5),
    @status           VARCHAR(10) OUTPUT,
    @pesan            VARCHAR(255) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Tarif resmi zakat fitrah (samakan dengan logika di Form2.cs)
    DECLARE @TARIF_BERAS_KG DECIMAL(10,2) = 2.5;
    DECLARE @TARIF_UANG_RP  DECIMAL(15,2) = 15000;

    BEGIN TRANSACTION;
    BEGIN TRY

        -- ── Validasi 1: Nama wajib diisi & format huruf saja ──────
        IF @nama IS NULL OR LTRIM(RTRIM(@nama)) = ''
        BEGIN
            SET @status = 'GAGAL';
            SET @pesan  = 'Nama tidak boleh kosong.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF @nama LIKE '%[^A-Za-z'' ]%'
        BEGIN
            SET @status = 'GAGAL';
            SET @pesan  = 'Nama hanya boleh berisi huruf, spasi, dan tanda apostrof.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- ── Validasi 2: Alamat wajib diisi ─────────────────────────
        IF @alamat IS NULL OR LTRIM(RTRIM(@alamat)) = ''
        BEGIN
            SET @status = 'GAGAL';
            SET @pesan  = 'Alamat tidak boleh kosong.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- ── Validasi 3: No HP wajib format +62 diikuti 8-13 digit ──
        IF @no_hp NOT LIKE '+62[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]%'
           OR LEN(@no_hp) < 11 OR LEN(@no_hp) > 16
           OR @no_hp LIKE '%[^0-9+]%'
        BEGIN
            SET @status = 'GAGAL';
            SET @pesan  = 'Nomor HP harus berformat +62 diikuti 8-13 digit angka. Contoh: +6281234567890';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- ── Validasi 4: Jumlah jiwa wajib > 0 ───────────────────────
        IF @jumlah_jiwa IS NULL OR @jumlah_jiwa <= 0
        BEGIN
            SET @status = 'GAGAL';
            SET @pesan  = 'Jumlah jiwa harus berupa angka lebih dari 0.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- ── Validasi 5: Tanggal tidak boleh di masa depan ──────────
        IF @tanggal IS NULL OR @tanggal > CAST(GETDATE() AS DATE)
        BEGIN
            SET @status = 'GAGAL';
            SET @pesan  = 'Tanggal tidak valid atau melebihi tanggal hari ini.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- ── Validasi 6: Jenis pembayaran wajib uang/beras ──────────
        IF @jenis_pembayaran NOT IN ('uang', 'beras')
        BEGIN
            SET @status = 'GAGAL';
            SET @pesan  = 'Jenis pembayaran harus uang atau beras.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- ── Hitung ulang total_bayar di server (sumber kebenaran) ──
        -- Ini mencegah data Excel dengan total_bayar yang salah ketik
        -- atau tidak konsisten ikut masuk ke database.
        DECLARE @total_bayar_final DECIMAL(15,2);
        SET @total_bayar_final =
            CASE
                WHEN @jenis_pembayaran = 'beras' THEN @jumlah_jiwa * @TARIF_BERAS_KG
                ELSE                                   @jumlah_jiwa * @TARIF_UANG_RP
            END;

        -- ── Cari atau buat data muzakki ─────────────────────────────
        DECLARE @id_muzakki INT;
        SELECT @id_muzakki = id_muzakki
        FROM muzakki
        WHERE nama = @nama AND no_hp = @no_hp;

        IF @id_muzakki IS NULL
        BEGIN
            INSERT INTO muzakki (nama, alamat, no_hp)
            VALUES (@nama, @alamat, @no_hp);
            SET @id_muzakki = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            -- Sinkronkan alamat terbaru jika muzakki sudah pernah terdaftar
            UPDATE muzakki SET alamat = @alamat WHERE id_muzakki = @id_muzakki;
        END

        -- ── Pisahkan nilai uang/beras sesuai jenis pembayaran ──────
        DECLARE @jumlah_uang  DECIMAL(15,2) = NULL;
        DECLARE @jumlah_beras DECIMAL(10,2) = NULL;

        IF @jenis_pembayaran = 'uang'
            SET @jumlah_uang = @total_bayar_final;
        ELSE
            SET @jumlah_beras = @total_bayar_final;

        -- ── Simpan transaksi pembayaran ─────────────────────────────
        INSERT INTO pembayaran_zakat
            (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
        VALUES
            (@id_muzakki, @tanggal, @jumlah_jiwa, @jumlah_uang, @jumlah_beras, @total_bayar_final, @jenis_pembayaran);

        SET @status = 'SUKSES';
        SET @pesan  = 'Data berhasil diimport. Total bayar dihitung otomatis: ' +
                      CASE WHEN @jenis_pembayaran = 'beras'
                           THEN CAST(@total_bayar_final AS VARCHAR) + ' kg beras'
                           ELSE 'Rp ' + CAST(@total_bayar_final AS VARCHAR)
                      END;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @status = 'GAGAL';
        SET @pesan  = ERROR_MESSAGE();
    END CATCH
END;
GO

drop database zakatdb

SELECT * FROM v_pembayaran_zakat;

-- Buat Database
CREATE DATABASE zakatdb;
GO
USE zakatdb;
GO

drop database zakatdb

-- Tabel PENGGUNA
CREATE TABLE pengguna (
    id_user   INT PRIMARY KEY IDENTITY(1,1),
    nama      VARCHAR(100) NOT NULL,
    password  VARCHAR(255) NOT NULL
);

-- Tabel MUZAKKI
CREATE TABLE muzakki (
    id_muzakki  INT PRIMARY KEY IDENTITY(1,1),
    nama        VARCHAR(100) NOT NULL,
    alamat      VARCHAR(255) NOT NULL,
    no_hp       VARCHAR(15)  NOT NULL
);

-- Tabel PEMBAYARAN_ZAKAT
CREATE TABLE pembayaran_zakat (
    id_pembayaran     INT PRIMARY KEY IDENTITY(1,1),
    id_muzakki        INT             NOT NULL,
    tanggal           DATE            NOT NULL,
    jumlah_jiwa       INT             NOT NULL,
    jumlah_uang       DECIMAL(15, 2)  NULL,  -- dalam rupiah
    jumlah_beras      DECIMAL(10, 2)  NULL,  -- dalam kilogram
    total_bayar       DECIMAL(15, 2)  NOT NULL,
    jenis_pembayaran  VARCHAR(5)      NOT NULL,

    -- Foreign Key
    CONSTRAINT fk_muzakki
        FOREIGN KEY (id_muzakki)
        REFERENCES muzakki(id_muzakki)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    -- Validasi jenis_pembayaran hanya 'beras' atau 'uang'
    CONSTRAINT chk_jenis_pembayaran
        CHECK (jenis_pembayaran IN ('beras', 'uang')),

    -- Validasi jika beras maka jumlah_beras wajib diisi
    CONSTRAINT chk_beras
        CHECK (jenis_pembayaran != 'beras' OR (jumlah_beras IS NOT NULL AND jumlah_uang IS NULL)),

    -- Validasi jika uang maka jumlah_uang wajib diisi
    CONSTRAINT chk_uang
        CHECK (jenis_pembayaran != 'uang' OR (jumlah_uang IS NOT NULL AND jumlah_beras IS NULL))
);


select * From pengguna;
select * From muzakki;
select * From pembayaran_zakat;

INSERT INTO pengguna (nama, password) VALUES ('admin', '123');

INSERT INTO pengguna (nama, password) VALUES ('paw', '123');

INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Ahmad Fauzi',     'Jl. Mawar No.1, Wonosobo',   '081234567890');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Budi Santoso',    'Jl. Melati No.5, Wonosobo',  '082345678901');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Citra Dewi',      'Jl. Anggrek No.3, Wonosobo', '083456789012');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Dian Pratama',    'Jl. Kenanga No.7, Wonosobo', '084567890123');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Eko Wahyudi',     'Jl. Dahlia No.2, Wonosobo',  '085678901234');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Fitri Rahayu',    'Jl. Tulip No.9, Wonosobo',   '086789012345');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Galih Permana',   'Jl. Flamboyan No.4, Wonosobo','087890123456');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Hana Sari',       'Jl. Cempaka No.6, Wonosobo', '088901234567');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Irfan Hakim',     'Jl. Sakura No.8, Wonosobo',  '089012345678');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Joko Susilo',     'Jl. Nusa Indah No.10, Wonosobo','081122334455');

-- ===================== DATA DUMMY PEMBAYARAN ZAKAT =====================

-- Pembayaran dengan BERAS (satuannya kg)
INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (1, '2026-04-01', 4, NULL, 10.00, 10.00, 'beras');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (2, '2026-04-02', 3, NULL, 7.50,  7.50,  'beras');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (3, '2026-04-03', 5, NULL, 12.50, 12.50, 'beras');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (4, '2026-04-04', 2, NULL, 5.00,  5.00,  'beras');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (5, '2026-04-05', 6, NULL, 15.00, 15.00, 'beras');

-- Pembayaran dengan UANG (satuannya rupiah)
-- Catatan: zakat fitrah uang = jumlah jiwa x Rp 40.000
INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (6,  '2026-04-06', 4, 160000, NULL, 160000, 'uang');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (7,  '2026-04-07', 3, 120000, NULL, 120000, 'uang');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (8,  '2026-04-08', 5, 200000, NULL, 200000, 'uang');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (9,  '2026-04-09', 2, 80000,  NULL, 80000,  'uang');

INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
VALUES (10, '2026-04-10', 7, 280000, NULL, 280000, 'uang');

DELETE FROM pembayaran_zakat;

-- Baru hapus data induk
DELETE FROM muzakki;

DROP DATABASE zakatdb;

ALTER TABLE muzakki
ADD CONSTRAINT chk_no_hp
    CHECK (no_hp NOT LIKE '%[^0-9+]%');

ALTER TABLE muzakki
ADD CONSTRAINT chk_nama
    CHECK (nama NOT LIKE '%[^A-Za-z'' ]%');
 
-- Pembayaran Zakat
IF NOT EXISTS (SELECT 1 FROM pembayaran_zakat)
BEGIN
    INSERT INTO pembayaran_zakat
        (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
    VALUES
        (1,  '2026-04-01', 4, NULL,   10.00,  10.00,  'beras'),
        (2,  '2026-04-02', 3, NULL,    7.50,   7.50,  'beras'),
        (3,  '2026-04-03', 5, NULL,   12.50,  12.50,  'beras'),
        (4,  '2026-04-04', 2, NULL,    5.00,   5.00,  'beras'),
        (5,  '2026-04-05', 6, NULL,   15.00,  15.00,  'beras'),
        (6,  '2026-04-06', 4, 160000, NULL,  160000,  'uang'),
        (7,  '2026-04-07', 3, 120000, NULL,  120000,  'uang'),
        (8,  '2026-04-08', 5, 200000, NULL,  200000,  'uang'),
        (9,  '2026-04-09', 2,  80000, NULL,   80000,  'uang'),
        (10, '2026-04-10', 7, 280000, NULL,  280000,  'uang');
END
GO

CREATE OR ALTER VIEW v_pembayaran_zakat AS
SELECT 
    p.id_pembayaran,
    m.id_muzakki,
    m.nama,
    m.alamat,
    m.no_hp,
    p.tanggal,
    p.jumlah_jiwa,
    p.jenis_pembayaran,
    p.jumlah_uang,
    p.jumlah_beras,
    p.total_bayar,
    CASE 
        WHEN p.jenis_pembayaran = 'beras' THEN CAST(p.jumlah_beras AS VARCHAR) + ' kg'
        WHEN p.jenis_pembayaran = 'uang'  THEN 'Rp ' + FORMAT(p.jumlah_uang, 'N0')
    END AS jumlah_dengan_satuan
FROM pembayaran_zakat p
JOIN muzakki m ON p.id_muzakki = m.id_muzakki;
GO

CREATE OR ALTER PROCEDURE sp_TambahPembayaran
    @nama VARCHAR(100),
    @alamat VARCHAR(255),
    @no_hp VARCHAR(15),
    @tanggal DATE,
    @jumlah_jiwa INT,
    @total_bayar DECIMAL(15,2),
    @jenis_pembayaran VARCHAR(5)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @id_muzakki INT;

        -- Cek apakah muzakki sudah terdaftar
        SELECT @id_muzakki = id_muzakki FROM muzakki WHERE nama = @nama AND no_hp = @no_hp;

        -- Jika belum ada, masukkan ke tabel muzakki
        IF @id_muzakki IS NULL
        BEGIN
            INSERT INTO muzakki (nama, alamat, no_hp) VALUES (@nama, @alamat, @no_hp);
            SET @id_muzakki = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            -- Jika sudah ada, update alamatnya agar sinkron
            UPDATE muzakki SET alamat = @alamat WHERE id_muzakki = @id_muzakki;
        END

        -- Pisahkan nilai berdasarkan jenis pembayaran
        DECLARE @jumlah_uang DECIMAL(15,2) = NULL;
        DECLARE @jumlah_beras DECIMAL(10,2) = NULL;

        IF @jenis_pembayaran = 'uang'
            SET @jumlah_uang = @total_bayar;
        ELSE
            SET @jumlah_beras = @total_bayar;

        -- Masukkan data ke tabel pembayaran
        INSERT INTO pembayaran_zakat (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
        VALUES (@id_muzakki, @tanggal, @jumlah_jiwa, @jumlah_uang, @jumlah_beras, @total_bayar, @jenis_pembayaran);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_UpdatePembayaran
    @id_pembayaran INT,
    @nama VARCHAR(100),
    @alamat VARCHAR(255),
    @no_hp VARCHAR(15),
    @tanggal DATE,
    @jumlah_jiwa INT,
    @total_bayar DECIMAL(15,2),
    @jenis_pembayaran VARCHAR(5)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @id_muzakki INT;

        -- Ambil id_muzakki dari relasi pembayaran
        SELECT @id_muzakki = id_muzakki FROM pembayaran_zakat WHERE id_pembayaran = @id_pembayaran;

        -- Update Profil Muzakki
        UPDATE muzakki 
        SET nama = @nama, alamat = @alamat, no_hp = @no_hp 
        WHERE id_muzakki = @id_muzakki;

        -- Atur ulang kolom uang/beras
        DECLARE @jumlah_uang DECIMAL(15,2) = NULL;
        DECLARE @jumlah_beras DECIMAL(10,2) = NULL;

        IF @jenis_pembayaran = 'uang'
            SET @jumlah_uang = @total_bayar;
        ELSE
            SET @jumlah_beras = @total_bayar;

        -- Update Transaksi Pembayaran
        UPDATE pembayaran_zakat 
        SET tanggal = @tanggal,
            jumlah_jiwa = @jumlah_jiwa,
            jumlah_uang = @jumlah_uang,
            jumlah_beras = @jumlah_beras,
            total_bayar = @total_bayar,
            jenis_pembayaran = @jenis_pembayaran
        WHERE id_pembayaran = @id_pembayaran;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_HapusPembayaran
    @id_pembayaran INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Karena tabel menggunakan ON DELETE CASCADE pada foreign key, 
        -- Menghapus data pembayaran langsung diperbolehkan.
        DELETE FROM pembayaran_zakat WHERE id_pembayaran = @id_pembayaran;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO


-- =========================================================
-- 5. STORED PROCEDURE: PENCARIAN AMAN BERBASIS PARAMETER
-- =========================================================
CREATE OR ALTER PROCEDURE sp_CariPembayaran
    @nama VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    -- Mengambil data secara aman dari VIEW yang telah dibuat
    SELECT * FROM v_pembayaran_zakat 
    WHERE nama LIKE '%' + @nama + '%';
END;
GO