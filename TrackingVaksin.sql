CREATE DATABASE TrackingVaksin
ON 
	PRIMARY(NAME = TrackingVaksinPrimary,
	FILENAME = 'D:\Perkuliahan Sem 4\TEKNET\Proyek\06_D3TI1_D3TI2\Database\TrackingVaksin.mdf',
	SIZE = 10MB,
	MAXSIZE = 20MB,
	FILEGROWTH = 20%)
LOG ON 
	(NAME = TrackingVaksinDBLog,
	FILENAME = 'D:\Perkuliahan Sem 4\TEKNET\Proyek\06_D3TI1_D3TI2\Database\TrackingVaksin.ldf',
	SIZE = 30MB,
	MAXSIZE = 50MB,
	FILEGROWTH = 20%)

CREATE TABLE Account(
	username VARCHAR(255) PRIMARY KEY NOT NULL,
	password VARCHAR(255),
	role VARCHAR(255),
	created_at DATETIME
)

CREATE TABLE BPOM(
	id_bpom INT PRIMARY KEY,
	name VARCHAR(255),
	division VARCHAR(255),
	created_at DATETIME,
	username VARCHAR(255),
)

CREATE TABLE BPOM_Log_Kedatangan_Vaksin(
	id INT PRIMARY KEY,
	id_rumahsakit INT,
	id_produsen INT,
	no_registration VARCHAR(255),
	created_at DATETIME
)

CREATE TABLE BPOM_Log_Penggunaan_Vaksin(
	id INT PRIMARY KEY,
	id_rumahsakit INT,
	no_registration VARCHAR(255),
	no_rek_medis VARCHAR(255),
	nik VARCHAR(255),
	created_at DATETIME
)

CREATE TABLE BPOM_Vaksin(
	no_registration VARCHAR(255) PRIMARY KEY,
	status VARCHAR(255),
	created_at DATETIME,
	id_produsen INT,
	packaging VARCHAR(255),
	total INT
)

CREATE TABLE Produsen(
	id_produsen INT PRIMARY KEY,
	no_license VARCHAR(255),
	name VARCHAR(255),
	address VARCHAR(255),
	username VARCHAR(255),
)

CREATE TABLE Masyarakat(
	nik VARCHAR(255) PRIMARY KEY,
	name VARCHAR(255),
	address VARCHAR(255),
	gender VARCHAR(255),
	date_of_birth DATETIME,
	username VARCHAR(255),
)

CREATE TABLE RumahSakit(
	id INT PRIMARY KEY,
	no_license VARCHAR(255),
	name VARCHAR(255),
	address VARCHAR(255),
	username VARCHAR(255),
)

CREATE TABLE Pasien(
	no_rek_medis VARCHAR(255) PRIMARY KEY,
	nik VARCHAR(255),
	no_registration VARCHAR(255),
	created_at DATETIME,
	isReported INT,
	id_rumahsakit INT,
)

CREATE TABLE Produsen_Vaksin(
	no_registration VARCHAR(255) PRIMARY KEY,
	id_produsen INT,
	status VARCHAR(255),
	created_at DATETIME,
	packaging VARCHAR(255),
	total INT
)

CREATE TABLE RS_Vaksin(
	no_registration VARCHAR(255) PRIMARY KEY,
	id_rumahsakit INT,
	created_at DATETIME,
	ref_code VARCHAR(255),
	id_produsen INT,
	isReported INT,
	packaging VARCHAR(255),
	total INT
)