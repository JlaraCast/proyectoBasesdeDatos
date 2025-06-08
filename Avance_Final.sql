create database pruebaProyecto5;


--UserProyecto5
--Ucr2025

use pruebaProyecto5;

--Tablas Principales


--Indice para 
Create table Customers  (
CustomerId int not null Primary key,
IDType varchar(50) not null,
Name varchar(100) not null,
LastName varchar(100) not null,
SecondLastName varchar(100),
Phone varchar(15) not null,
Address varchar(255) not null,
Email varchar(100) UNIQUE not null,
Password varchar(255) not null)
go


-- Create table Checks (
-- CheckNumber int NOT NULL,
-- CheckOwner varchar(255) NOT NULL,
-- CheckBank varchar(255),
-- Amount decimal(18, 2),
-- CustomerId int,
-- Constraint PK_Checks Primary key (CheckNumber, CheckOwner)
-- );

---- Crear un índice único en la combinación de CheckNumber y CheckOwner
-- Create unique index INDEX_CheckNumber_CheckOwner
-- on Checks (CheckNumber, CheckOwner);


-- Create table Cards (
-- CardNumber int not null,
-- PayId int not null,
-- CardOwner varchar(255),
-- CardBank varchar(255),
-- CustomerId int,
-- Constraint PK_Cards Primary key (CardNumber, PayId)
-- );


Create table Packages (
PackageId int not null primary key identity,
Name varchar(100) not null,
Description Text not null,
CostPerPersonPerNight Decimal(10, 2) not null,
PremiumPercentage float not null,
TermMonths int not null)
go

Create table Reservations (
ReservationId int not null primary key identity,
CustomerId int not null,
PackageId int not null,
ReservationDate DATE not null,
NumberOfPeople int not null,
NumberOfNights int not null,
PaymentMethod varchar(50) not null,
DiscountApplied BIT not null,
IVA Decimal(10, 2) not null,
TotalColones Decimal(10, 2) not null,
TotalDollars Decimal(10, 2) not null,
ExchangeRate varchar(50),
Foreign key (CustomerId) References Customers(CustomerId)on delete cascade,
Foreign key (PackageId) References Packages(PackageId)on delete cascade)
go


Create table Invoices (
InvoiceID int not null Primary key identity,
ReservationId int not null,
CustomerId int not null,
IssueDate Date not null,
PaymentMethod Varchar(50) not null,
Foreign key (ReservationId) References Reservations(ReservationId) on delete cascade,
Foreign key (CustomerId) References Customers(CustomerId) on delete no action)
go



--indice para usuarios 
Create table Users (
Email varchar(100) not null primary key,
Name varchar(100) not null,
Password varchar(255) not null,
CreateDate Date not null,
Status Char(1) not null default 'A',
Roll Char(1)not null default 'C')
go

--Tablas Intermedias
Create table CustomersInvoices (
CustomerId int NOT NULL,
InvoiceID int NOT NULL,
Primary key (CustomerId, InvoiceID),
Foreign key (CustomerId) References Customers(CustomerId) on delete cascade,
Foreign key (InvoiceID) References Invoices(InvoiceID) on delete no action)
go

Create table ReservationsCustomers (
ReservationId int not null,
CustomerId int not null,
Primary key (ReservationId, CustomerId),
Foreign key (ReservationId) References Reservations(ReservationId) on delete cascade,
Foreign key (CustomerId) References Customers(CustomerId)on delete no action) 
go


Create table ReservationsPackages (
ReservationId int not null,
PackageId int not null,
Primary key (ReservationId, PackageId),
Foreign key (ReservationId) References Reservations(ReservationId) on delete cascade,
Foreign key (PackageId) References Packages(PackageId) on delete no action )
go

Create table UsersCustomers (
Email varchar(100) not null,
CustomerId int not null,
Primary key (Email, CustomerId),
Foreign key (Email) References Users(Email),
Foreign key (CustomerId) References Customers(CustomerId) on delete cascade)
go

--Triggers en las tablas
Create trigger triggerCustomers
on Customers
after insert
as
begin
    insert into Users (Email, Name, Password, CreateDate)
    select Email, Name, Password, getDate()
    from INSERTED;

    insert into UsersCustomers (Email, CustomerId)
    select Email, CustomerId 
    from INSERTED;
end
go


--Ingreso de paquetes  
insert into Packages ( Name, Description, CostPerPersonPerNight, PremiumPercentage, TermMonths) values
( 'Todo Incluido', 'Paquete completo con alimentación y hospedaje.', 450.00, 0.45, 24),
('Alimentación', 'Paquete que cubre todas las comidas durante la estancia.', 275.00, 0.35, 18),
('Hospedaje', 'Paquete de hospedaje sin alimentación.', 210.00, 0.15, 12);

---Administrador
insert into Users Values('admin@gmail.com', 'Administrador',12345,GETDATE(),'A','A');

INSERT INTO Customers (CustomerId, IDType, Name, LastName, SecondLastName, Phone, Address, Email, Password)
VALUES (
    1,
    'Cédula Nacional',
    'Carlos',
    'Sánchez',
    'Rodríguez',
    '88889999',
    'San José, Costa Rica',
    'www.camila00@gmail.com',
    '1234Segura'
)
GO

select * from Customers;
select * from Reservations;
select * from UsersCustomers;
select * from Packages;
select * from Invoices;
select * from Users;
select * from Cards;
select * from ReservationsCustomers;
go

DROP TABLE CustomersInvoices; --esta tabla ya no debe crearse
DROP TABLE ReservationsCustomers;
DROP TABLE ReservationsPackages;
DROP TABLE UsersCustomers;
DROP TABLE Invoices;
DROP TABLE Reservations;
DROP TABLE Checks;
DROP TABLE Cards;
DROP TABLE Users;
DROP TABLE Packages;
DROP TABLE Customers;


delete from Reservations;
delete from Invoices;
DBCC CHECKIDENT ('Invoices', RESEED, 0);
DBCC CHECKIDENT ('Reservations', RESEED, 0);


--Tabla morosidad
--el cliente tiene un estado de morosidad 
--este lo conecta con la tabla de moroso
--por reserva se coloca si es moroso o no 

