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
