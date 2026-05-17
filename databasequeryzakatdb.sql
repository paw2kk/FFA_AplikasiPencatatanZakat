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


-- ============================================================
--  VIEW  -  vw_PembayaranZakat
--  Menggantikan SELECT query panjang di btnTampilData & btnCari
-- ============================================================
CREATE VIEW vw_PembayaranZakat AS
    SELECT
        p.id_pembayaran,
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
        
            WHEN p.jenis_pembayaran = 'beras'
                THEN CAST(p.jumlah_beras AS VARCHAR) + ' kg'
            WHEN p.jenis_pembayaran = 'uang'
                THEN 'Rp ' + FORMAT(p.jumlah_uang, 'N0')
        END AS jumlah_dengan_satuan,
        -- Kolom tambahan: label ringkas untuk laporan
        m.nama + ' (' + p.jenis_pembayaran + ')' AS label_ringkas,
        -- Kolom tambahan: flag apakah termasuk pembayar besar (> 5 jiwa)
        CASE WHEN p.jumlah_jiwa > 5 THEN 'Ya' ELSE 'Tidak' END AS keluarga_besar
    FROM pembayaran_zakat p
    JOIN muzakki m ON p.id_muzakki = m.id_muzakki;
GO


-- ============================================================
--  STORED PROCEDURE 1 - sp_TambahPembayaran (INSERT)
--  Logic tambahan:
--    - Cek / tambah muzakki otomatis
--    - Hitung total_bayar otomatis dari jumlah_jiwa
--      (uang: jiwa x 40000 | beras: jiwa x 2.5 kg)
--    - Validasi: total_bayar dari form harus sama dengan hasil hitung
-- ============================================================
CREATE PROCEDURE sp_TambahPembayaran
    @nama             VARCHAR(100),
    @alamat           VARCHAR(255),
    @no_hp            VARCHAR(15),
    @tanggal          DATE,
    @jumlah_jiwa      INT,
    @jenis_pembayaran VARCHAR(5),
    @total_bayar_input DECIMAL(15,2)   -- dikirim dari aplikasi untuk divalidasi
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Hitung total yang seharusnya
    DECLARE @total_hitung DECIMAL(15,2);
    DECLARE @jumlah_uang  DECIMAL(15,2);
    DECLARE @jumlah_beras DECIMAL(10,2);
    SET @jumlah_uang  = NULL;
    SET @jumlah_beras = NULL;
 
    IF @jenis_pembayaran = 'uang'
    BEGIN
        SET @total_hitung = @jumlah_jiwa * 40000;
        SET @jumlah_uang  = @total_hitung;
    END
    ELSE
    BEGIN
        SET @total_hitung = @jumlah_jiwa * 2.5;
        SET @jumlah_beras = @total_hitung;
    END
    
    -- Validasi: total dari aplikasi harus sesuai hasil hitung
    IF @total_bayar_input <> @total_hitung
    BEGIN
        DECLARE @pesan VARCHAR(200);
        SET @pesan = 'Total bayar tidak sesuai. Seharusnya ' + CAST(@total_hitung AS VARCHAR) + ' berdasarkan jumlah jiwa.';
        RAISERROR(@pesan, 16, 1);
        RETURN;
    END
 
    -- Cek apakah muzakki sudah ada
    DECLARE @idMuzakki INT;
    SELECT @idMuzakki = id_muzakki
    FROM muzakki
    WHERE nama = @nama AND no_hp = @no_hp;
 
    -- Jika belum ada, tambahkan
    IF @idMuzakki IS NULL
    BEGIN
        INSERT INTO muzakki (nama, alamat, no_hp)
        VALUES (@nama, @alamat, @no_hp);
        SET @idMuzakki = SCOPE_IDENTITY();
    END
 
    -- Insert pembayaran
    INSERT INTO pembayaran_zakat
        (id_muzakki, tanggal, jumlah_jiwa, jumlah_uang, jumlah_beras, total_bayar, jenis_pembayaran)
    VALUES
        (@idMuzakki, @tanggal, @jumlah_jiwa, @jumlah_uang, @jumlah_beras, @total_hitung, @jenis_pembayaran);
 
    SELECT SCOPE_IDENTITY() AS id_pembayaran_baru;
END
GO
 
-- ============================================================
--  STORED PROCEDURE 2 - sp_UpdatePembayaran (UPDATE)
--  Logic tambahan:
--    - Update muzakki + pembayaran_zakat sekaligus
--    - Hitung ulang total_bayar otomatis (tidak percaya input mentah)
--    - Catat timestamp update via kolom log (opsional: lihat tabel audit di bawah)
-- ============================================================
CREATE PROCEDURE sp_UpdatePembayaran
    @id_pembayaran    INT,
    @nama             VARCHAR(100),
    @alamat           VARCHAR(255),
    @no_hp            VARCHAR(15),
    @tanggal          DATE,
    @jumlah_jiwa      INT,
    @jenis_pembayaran VARCHAR(5)
AS
BEGIN
    SET NOCOUNT ON;
 
    -- Hitung ulang total & jumlah sesuai jenis
    DECLARE @total_bayar  DECIMAL(15,2);
    DECLARE @jumlah_uang  DECIMAL(15,2);
    DECLARE @jumlah_beras DECIMAL(10,2);
    SET @jumlah_uang  = NULL;
    SET @jumlah_beras = NULL;
 
    IF @jenis_pembayaran = 'uang'
    BEGIN
        SET @total_bayar = @jumlah_jiwa * 40000;
        SET @jumlah_uang = @total_bayar;
    END
    ELSE
    BEGIN
        SET @total_bayar = @jumlah_jiwa * 2.5;
        SET @jumlah_beras = @total_bayar;
    END
 
    -- Validasi id_pembayaran ada
    IF NOT EXISTS (SELECT 1 FROM pembayaran_zakat WHERE id_pembayaran = @id_pembayaran)
    BEGIN
        RAISERROR('Data pembayaran dengan id %d tidak ditemukan.', 16, 1, @id_pembayaran);
        RETURN;
    END
 
    -- Update muzakki
    UPDATE muzakki SET
        nama   = @nama,
        alamat = @alamat,
        no_hp  = @no_hp
    WHERE id_muzakki = (
        SELECT id_muzakki FROM pembayaran_zakat
        WHERE id_pembayaran = @id_pembayaran
    );
 
    -- Update pembayaran_zakat
    UPDATE pembayaran_zakat SET
        tanggal          = @tanggal,
        jumlah_jiwa      = @jumlah_jiwa,
        jumlah_uang      = @jumlah_uang,
        jumlah_beras     = @jumlah_beras,
        total_bayar      = @total_bayar,
        jenis_pembayaran = @jenis_pembayaran
    WHERE id_pembayaran  = @id_pembayaran;
 
    SELECT @total_bayar AS total_bayar_baru;
END
GO
 
-- ============================================================
--  STORED PROCEDURE 3 - sp_HapusPembayaran (DELETE)
--  Logic tambahan:
--    - Cek apakah muzakki masih punya pembayaran lain
--    - Jika tidak ada lagi, hapus juga data muzakki (clean up)
--    - Return pesan status
-- ============================================================
CREATE PROCEDURE sp_HapusPembayaran
    @id_pembayaran INT
AS
BEGIN
    SET NOCOUNT ON;
 
    -- Validasi
    IF NOT EXISTS (SELECT 1 FROM pembayaran_zakat WHERE id_pembayaran = @id_pembayaran)
    BEGIN
        RAISERROR('Data tidak ditemukan.', 16, 1);
        RETURN;
    END
 
    -- Ambil id_muzakki sebelum dihapus
    DECLARE @idMuzakki INT;
    SELECT @idMuzakki = id_muzakki
    FROM pembayaran_zakat
    WHERE id_pembayaran = @id_pembayaran;
 
    -- Hapus pembayaran
    DELETE FROM pembayaran_zakat WHERE id_pembayaran = @id_pembayaran;
 
    -- Jika muzakki sudah tidak punya pembayaran lain, hapus juga
    IF NOT EXISTS (SELECT 1 FROM pembayaran_zakat WHERE id_muzakki = @idMuzakki)
    BEGIN
        DELETE FROM muzakki WHERE id_muzakki = @idMuzakki;
        SELECT 'Pembayaran dan data muzakki berhasil dihapus.' AS status;
    END
    ELSE
    BEGIN
        SELECT 'Pembayaran berhasil dihapus. Data muzakki dipertahankan.' AS status;
    END
END
GO
 
-- ============================================================
--  STORED PROCEDURE 4 - sp_CariPembayaran (SEARCH via VIEW)
--  Logic tambahan:
--    - Cari berdasarkan nama ATAU no_hp sekaligus
--    - Filter opsional berdasarkan jenis_pembayaran
--    - Filter opsional berdasarkan rentang tanggal
--    - Return jumlah total baris ditemukan
-- ============================================================
CREATE PROCEDURE sp_CariPembayaran
    @keyword          VARCHAR(100) = NULL,   -- cari nama atau no_hp
    @jenis_pembayaran VARCHAR(5)   = NULL,   -- filter: 'beras' / 'uang' / NULL (semua)
    @tgl_mulai        DATE         = NULL,   -- filter tanggal awal (opsional)
    @tgl_akhir        DATE         = NULL    -- filter tanggal akhir (opsional)
AS
BEGIN
    SET NOCOUNT ON;
 
    SELECT *
    FROM vw_PembayaranZakat
    WHERE
        -- Filter nama/no_hp (jika keyword diisi)
        (@keyword IS NULL OR
            nama  LIKE '%' + @keyword + '%' OR
            no_hp LIKE '%' + @keyword + '%'
        )
        -- Filter jenis (jika diisi)
        AND (@jenis_pembayaran IS NULL OR jenis_pembayaran = @jenis_pembayaran)
        -- Filter rentang tanggal (jika diisi)
        AND (@tgl_mulai IS NULL OR tanggal >= @tgl_mulai)
        AND (@tgl_akhir IS NULL OR tanggal <= @tgl_akhir)
    ORDER BY tanggal DESC;
 
    -- Kembalikan juga jumlah baris
    SELECT COUNT(*) AS jumlah_ditemukan
    FROM vw_PembayaranZakat
    WHERE
        (@keyword IS NULL OR nama LIKE '%' + @keyword + '%' OR no_hp LIKE '%' + @keyword + '%')
        AND (@jenis_pembayaran IS NULL OR jenis_pembayaran = @jenis_pembayaran)
        AND (@tgl_mulai IS NULL OR tanggal >= @tgl_mulai)
        AND (@tgl_akhir IS NULL OR tanggal <= @tgl_akhir);
END
GO
 
-- ============================================================
--  STORED PROCEDURE 5 - sp_LoginPengguna
--  Dipakai di Form1 (login)
--  Logic tambahan:
--    - Catat gagal login ke tabel log_login
--    - Return status login: 1=berhasil, 0=gagal
-- ============================================================
 
-- Tabel log_login untuk audit
CREATE TABLE log_login (
    id_log      INT PRIMARY KEY IDENTITY(1,1),
    nama        VARCHAR(100),
    waktu       DATETIME DEFAULT GETDATE(),
    status      VARCHAR(10)   -- 'BERHASIL' / 'GAGAL'
);
GO
 
CREATE PROCEDURE sp_LoginPengguna
    @nama     VARCHAR(100),
    @password VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
 
    DECLARE @id INT;
    SELECT @id = id_user
    FROM pengguna
    WHERE nama = @nama AND password = @password;
 
    IF @id IS NOT NULL
    BEGIN
        INSERT INTO log_login (nama, status) VALUES (@nama, 'BERHASIL');
        SELECT 1 AS login_sukses, @id AS id_user, @nama AS nama_user;
    END
    ELSE
    BEGIN
        INSERT INTO log_login (nama, status) VALUES (@nama, 'GAGAL');
        SELECT 0 AS login_sukses, NULL AS id_user, NULL AS nama_user;
    END
END
GO
 
-- ============================================================
--  DATA DUMMY
-- ============================================================
INSERT INTO pengguna (nama, password) VALUES ('admin', '123');
INSERT INTO pengguna (nama, password) VALUES ('paw', '123');

INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Ahmad Fauzi',   'Jl. Mawar No.1, Wonosobo',     '081234567890');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Budi Santoso',  'Jl. Melati No.5, Wonosobo',    '082345678901');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Citra Dewi',    'Jl. Anggrek No.3, Wonosobo',   '083456789012');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Dian Pratama',  'Jl. Kenanga No.7, Wonosobo',   '084567890123');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Eko Wahyudi',   'Jl. Dahlia No.2, Wonosobo',    '085678901234');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Fitri Rahayu',  'Jl. Tulip No.9, Wonosobo',     '086789012345');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Galih Permana', 'Jl. Flamboyan No.4, Wonosobo', '087890123456');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Hana Sari',     'Jl. Cempaka No.6, Wonosobo',   '088901234567');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Irfan Hakim',   'Jl. Sakura No.8, Wonosobo',    '089012345678');
INSERT INTO muzakki (nama, alamat, no_hp) VALUES ('Joko Susilo',   'Jl. Nusa Indah No.10, Wonosobo','081122334455');
 
INSERT INTO pembayaran_zakat (id_muzakki,tanggal,jumlah_jiwa,jumlah_uang,jumlah_beras,total_bayar,jenis_pembayaran)
VALUES (1,'2026-04-01',4,NULL,10.00,10.00,'beras'),
       (2,'2026-04-02',3,NULL,7.50, 7.50, 'beras'),
       (3,'2026-04-03',5,NULL,12.50,12.50,'beras'),
       (4,'2026-04-04',2,NULL,5.00, 5.00, 'beras'),
       (5,'2026-04-05',6,NULL,15.00,15.00,'beras'),
       (6,'2026-04-06',4,160000,NULL,160000,'uang'),
       (7,'2026-04-07',3,120000,NULL,120000,'uang'),
       (8,'2026-04-08',5,200000,NULL,200000,'uang'),
       (9,'2026-04-09',2,80000, NULL,80000, 'uang'),
       (10,'2026-04-10',7,280000,NULL,280000,'uang');