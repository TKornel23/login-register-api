using login_register_api.Interfaces;
using login_register_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace login_register_api.Controllers
{
    [Route("")]
    public class UserController : Controller
    {
        private readonly UserDbContext _ctx;

        public UserController(UserDbContext ctx, ITokenService tokenService)
        {
            _ctx = ctx;
            TokenService = tokenService;
        }

        public ITokenService TokenService { get; }

        [HttpGet]
        [Authorize]
        public  async Task<ActionResult> Index()
        {
            return Ok(await _ctx.Users.ToListAsync<User>());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginRegisterDTO login)
        {
            User user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == login.Email);

            if(user == null) return Unauthorized("Invalid Email address: " + login.Email);

            using HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);

            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }
            }

            return Ok(new UserDTO
            {
                Email = user.Email,
                Token = TokenService.CreateToken(user)
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(LoginRegisterDTO register)
        {
            if (await IsEmailExists(register.Email))
            {
                return BadRequest("Email already exists: " + register.Email);
            }

            using HMACSHA512 hmac = new HMACSHA512();

            User user = new User
            {
                Email = register.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password)),
                PasswordSalt = hmac.Key
            };

            await _ctx.Users.AddAsync(user);
            await _ctx.SaveChangesAsync();

            return Ok(new UserDTO
            {
                Email = user.Email,
                Token = TokenService.CreateToken(user)
            });
        }

        private async Task<bool> IsEmailExists(string email)
        {
             return await _ctx.Users.AnyAsync(user => user.Email.ToLower() == email.ToLower());
        }
    }
}
