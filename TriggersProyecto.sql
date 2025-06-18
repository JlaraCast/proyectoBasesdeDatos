--verifica si la tabla de usuarios no posee ningún dato (cuando esten todos los usuarios inavilitados)
-- se crea crea o actualiza un administrador por defecto. Es para que la tabla nunca quede sin administrador,
-- ya que el usuario administrador podría borrarse hasta el.
----------------------------------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------------------------------
CREATE TRIGGER VerificaciónUsuarioAdmin 
AFTER UPDATE ON usuarios
FOR EACH ROW
BEGIN
    p_CrearAdminDefault;
END; 






















----------------------------------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------------------------------
-- procedimiento para eliminar roles relacionados a una pantalla
CREATE OR REPLACE PROCEDURE EliminarRolesPorPantalla(
    p_idPantalla IN NUMBER,
    p_idSistema IN NUMBER
) IS
BEGIN
    -- Elimina de permisosRoles
    DELETE FROM permisosRoles
    WHERE idPantalla = p_idPantalla
    AND idSistema = p_idSistema;

    -- Elimina de permisosUsuarios (si tienes esta tabla relacionada con pantallas)
    DELETE FROM permisosUsuarios
    WHERE idPantalla = p_idPantalla
    AND idSistema = p_idSistema;
END;
/

-- trigger para elminación de pantallas rol cuando se elimina una pantalla 
CREATE TRIGGER EliminarPantallasRol
BEFORE DELETE ON pantallas  
FOR EACH ROW
BEGIN
    EliminarRolesPorPantalla(:OLD.idPantalla, :OLD.idSistema);
END;
/

----------------------------------------------------------------------------------------------------------------------------------------

-- procedimiento para eliminar permisos relacionados a un rol en todas las pantallas de un sistema
CREATE OR REPLACE PROCEDURE EliminarPermisosPorRol(
    p_idRol IN NUMBER,
    p_idSistema IN NUMBER
) IS
BEGIN
    -- Elimina de permisosRoles
    DELETE FROM permisosRoles
    WHERE idRol = p_idRol
    AND idSistema = p_idSistema;
END;


-- trigger para eliminación de permisos cuando se elimina un rol
CREATE TRIGGER EliminarPermisosRol
BEFORE DELETE ON roles
FOR EACH ROW
BEGIN
    EliminarPermisosPorRol(:OLD.idRol, :OLD.idSistema);
END;


----------------------------------------------------------------------------------------------------------------------------------------

create or replace trigger DesactivarUsuario
before insert on bitacoras 
for each row
declare
    vEstado varchar2(50);  
begin
    vEstado := 'Inactivo';
    update usuarios set estado = vEstado where idUsuario = :NEW.idUsuario;
end;