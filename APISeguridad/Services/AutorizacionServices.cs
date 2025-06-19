
using Microsoft.EntityFrameworkCore;
using System.Text;

//Referenciamos las librerias para utilizar JWT
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using APISeguridad.Model;

namespace APIHotelBeach.SA.Services
{
    public class AutorizacionServices : IAutorizacionServices
    {//variable permite utilizar el archivo AppSettings.json
        private readonly IConfiguration _configuration;

        //variable para utilizar las funciones del ORM
        private readonly DbContextSeguridad _context;

        //constructor con parametros
        public AutorizacionServices(IConfiguration configuration, DbContextSeguridad context)
        {
            _configuration = configuration;
            _context = context;

        }

        public async Task<AutorizacionResponse> DevolverToken(Usuario autorizacion)
        {
            if (string.IsNullOrEmpty(autorizacion?.correo) || string.IsNullOrEmpty(autorizacion?.clave))
            {
                return new AutorizacionResponse
                {
                    Token = "",
                    Resultado = false,
                    Msj = "Correo y/o clave inválidos"
                };
            }

            var temp = await _context.usuarios.FirstOrDefaultAsync(u =>
                u.correo == autorizacion.correo && u.clave == autorizacion.clave);

            if (temp == null)
            {
                return new AutorizacionResponse
                {
                    Token = "",
                    Resultado = false,
                    Msj = "Usuario no válido"
                };
            }

            string tokenCreado = GenerarToken(autorizacion.correo);

            return new AutorizacionResponse
            {
                Token = tokenCreado,
                Resultado = true,
                Msj = "Ok"
            };
        }


        private string GenerarToken(string correo)
        {

            //Se key esta almacenada dentro del archivo JSON
            var key = _configuration.GetValue<string>("JwtSettings:key");

            //Se convierte la key en un vector de bytes
            var keyBytes = Encoding.ASCII.GetBytes(key);

            //se instancia la identidad que reliza el reclamo para la autorizacion
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, correo));

            //se instancia las credenciales del token
            var credencialesToken = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature);

            //se instancia el descriptor del token 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(8),
                SigningCredentials = credencialesToken
            };

            //se instancia el tokenhandler para construir el tokrn 
            var tokenHandler = new JwtSecurityTokenHandler();

            //se instancia el tokenhandler para contruir el token 
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            //se escribe el token 
            var tokenCreado = tokenHandler.WriteToken(tokenConfig);

            //se retorna el token 
            return tokenCreado;

        }
    }
}

