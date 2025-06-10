

---Tablas Principales 
create table usuarios (
idUsuario int primary key,
nombre varchar(255),
correo varchar(255) ,
clave varchar(255),
estado varchar(50));

create table roles (
idRol int primary key,
nombre varchar(100) ,
descripcion varchar(255));

create table sistemas (
idSistema int primary key,
nombre varchar(100),
descripcion varchar(255));

create table pantallas (
idPantalla int primary key,
idSistema int,
nombre varchar(100),
descripcion varchar(255),
ruta varchar(255),
foreign key (idSistema) references sistemas(idSistema));

create table bitacora (
idBitacora int primary key,
idUsuario int,
fecha datetime,
accion varchar(255),
detalle varchar(255),
foreign key (idUsuario) references usuarios(idUsuario));

---Tablas Intermedias

create table usuariosRoles (
idUsuario int,
idRol int,
primary key (idUsuario, idRol),
foreign key (idUsuario) references usuarios(idUsuario),
foreign key (idRol) references roles(idRol));

create table permisosUsuarios (
idUsuario int,
idPantalla int,
permisoInsertar varchar(5),
permisoModificar varchar(5),
permisoBorrar varchar(5),
permisoConsultar varchar(5),
primary key (idUsuario, id_pantalla),
foreign key (idUsuario) references usuarios(idUsuario),
foreign key (idPantalla) references pantallas(idPantalla));

create table permisosRoles (
idRol int,
idPantalla int,
permisoInsertar varchar(5),
permisoModificar varchar(5),
permisoBorrar varchar(5),
permisoConsultar varchar(5),
primary key (idRol, idPantalla),
foreign key (idRol) references roles(idRol),
foreign key (idPantalla) references pantallas(idPantalla));

