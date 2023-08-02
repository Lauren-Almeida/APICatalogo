using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APICatalogo.DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace APICatalogo.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    [EnableCors]
    public class AutorizaController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AutorizaController(UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }


        [HttpGet]
        public ActionResult<string> Get()
        {
            return "AutorizaController  :: Acessado em : "
                + DateTime.Now.ToLongDateString();
        }

        /// <summary>
        /// Inclui um novo usuario
        /// </summary>
        ///<remarks>
        ///POST produtos
        ///</remarks>
        /// <param name="usuarioDTO">cdusuario</param>
        /// <returns>Objetos Usuario</returns>
        ///<remarks>Retorna o usuário registrado</remarks>
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] UsuarioDTO model)
        {
            // if (!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            // }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.SignInAsync(user, false);
            return Ok(GeraToken(model));
        }

        /// <summary>
        /// Verifica as credenciais de um usuário
        /// </summary>
        ///<remarks>
        ///POSTusuarios
        ///</remarks>
        /// <param name="userInfo">cdusuario</param>
        /// <returns>Status 200 e o token gerado</returns>
        ///<remarks>Retorna 200 e o token gerado</remarks>
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UsuarioDTO userInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            }

            var result = await _signInManager.PasswordSignInAsync(userInfo.Email,
                userInfo.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok(GeraToken(userInfo));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login Inválido...");
                return BadRequest(ModelState);
            }
        }

        private UsuarioToken GeraToken(UsuarioDTO userInfo)
        {
            //define declarações do usuário
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                 new Claim("meuPet", "pipoca"),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             };

            //gera uma chave com base em um algoritmo simetrico
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Tempo de expiracão do token.
            var expiracao = _configuration["TokenConfiguration:ExpireHours"];
            var expiration = DateTime.UtcNow.AddHours(double.Parse(expiracao));

            // classe que representa um token JWT e gera o token
            JwtSecurityToken token = new JwtSecurityToken(
              issuer: _configuration["TokenConfiguration:Issuer"],
              audience: _configuration["TokenConfiguration:Audience"],
              claims: claims,
              expires: expiration,
              signingCredentials: credenciais);

            //retorna os dados com o token e informacoes
            return new UsuarioToken()
            {
                Authenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                Message = "Token JWT OK"
            };
        }

    }
}