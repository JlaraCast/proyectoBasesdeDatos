create database pruebaProyecto5;

--UserProyecto5
--Ucr2025

use pruebaProyecto5;

--Tablas Principales
Create table Customers  (
CustomerId int not null Primary key,
IDType varchar(50) not null,
Name varchar(100) not null,
LastName varchar(100) not null,
SecondLastName varchar(100),
Phone varchar(15) not null,
Address varchar(255) not null,
Email varchar(100) UNIQUE not null,
go

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

--Ingreso de paquetes  
insert into Packages ( Name, Description, CostPerPersonPerNight, PremiumPercentage, TermMonths) values
( 'Todo Incluido', 'Paquete completo con alimentación y hospedaje.', 450.00, 0.45, 24),
('Alimentación', 'Paquete que cubre todas las comidas durante la estancia.', 275.00, 0.35, 18),
('Hospedaje', 'Paquete de hospedaje sin alimentación.', 210.00, 0.15, 12);

---Administrador
insert into Users Values('admin@gmail.com', 'Administrador',12345,GETDATE(),'A','A');

--Customer
insert into Customers (CustomerId, IDType, Name, LastName, SecondLastName, Phone, Address, Email, Password)
values (1,'Cédula Nacional','Carlos','Sánchez','Rodríguez','88889999','San José, Costa Rica','www.camila00@gmail.com','1234Segura')
go

select * from Customers;
select * from Reservations;
select * from UsersCustomers;
select * from Packages;
select * from Invoices;
select * from Users;
select * from Cards;
select * from ReservationsCustomers;
go




