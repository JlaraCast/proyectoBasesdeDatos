
---Tablas Principales 
create table usuarios (
idUsuario int primary key,
nombre varchar(255),
correo varchar(255) ,
clave varchar(255),
estado varchar(50));

create table roles (
idRol int,
idSistema int 
nombre varchar(100) ,
descripcion varchar(255) 
foreign key (idSistema) references sistemas(idSistema),
primary key (idRol,idSitema)) ;

create table sistemas (
idSistema int primary key,
nombre varchar(100),
descripcion varchar(255)
);

create table pantallas (
idPantalla int ,
idSistema int,
nombre varchar(100),
descripcion varchar(255),
ruta varchar(255),
foreign key (idSistema) references sistemas(idSistema),
primary key(idPantalla,idSistema));

create table bitacora (
idBitacora int ,
idUsuario int,
idSistema int,
idPantalla int,
fecha date,
accion varchar(255),
detalle varchar(255),
primary key(idBitacora,idSistema,idPantalla),
foreign key (idSistema) references sistemas(idSistema),
foreign key (idUsuario) references usuarios(idUsuario),
foreign key (idPantalla) references pantallas(idPantalla));

---Tablas Intermedias

create table usuariosRoles (
idUsuario int,
idRol int,
idSistema int,

primary key (idUsuario, idRol,idSistema),
foreign key (idSistema) references sistemas(idSistema),
foreign key (idUsuario) references usuarios(idUsuario),
foreign key (idRol) references roles(idRol));

create table permisosUsuarios (
idUsuario int,
idPantalla int,
permisoInsertar varchar(5),
permisoModificar varchar(5),
permisoBorrar varchar(5),
permisoConsultar varchar(5),
primary key (idUsuario, idPantalla),
foreign key (idUsuario) references usuarios(idUsuario),
foreign key (idPantalla) references pantallas(idPantalla));

create table permisosRoles (
idRol int,
idPantalla int,
idSistema int,
permisoInsertar varchar(5),
permisoModificar varchar(5),
permisoBorrar varchar(5),
permisoConsultar varchar(5),
primary key (idRol, idPantalla,idSistema),
foreign key (idRol) references roles(idRol),
foreign key (idPantalla) references pantallas(idPantalla));

