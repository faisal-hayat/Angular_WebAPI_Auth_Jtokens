using Demo.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;

namespace Demo.Configration
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        public ILogger<AuthController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        public AuthController(ILogger<AuthController> logger, UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _logger = logger;
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest userRegistrationRequest)
        {
            // check if the model is valid
            if (ModelState.IsValid)
            {
                var emailExists = await _userManager.FindByEmailAsync(userRegistrationRequest.Email);
                if(emailExists == null)
                {
                    var newUSer = new IdentityUser()
                    {
                        Email = userRegistrationRequest.Email,
                        UserName = userRegistrationRequest.Email
                        
                    };

                    var isCreatedIdentityResult = await _userManager.CreateAsync(newUSer, userRegistrationRequest.Password);
                    if (isCreatedIdentityResult.Succeeded)
                    {
                        // This is where we will generate token 
                        var token = GenerateJwtToken(newUSer);
                        return Ok(new RegistrationRequestResponse()
                        {
                            Result = true,
                            Token = token,
                        });
                    }
                    else
                    {
                        return BadRequest("Something went wrong");
                    }
                }
                else
                {
                    return BadRequest("Account Already Exist");
                }
            }
            else
            {
                return BadRequest("Please Enter all required credentials");
            }   
        }

        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(userLoginRequest.Email);
                if (existingUser == null)
                {
                    return BadRequest("User does not exist");
                }
                else
                {
                    var isPasswordValid = await _userManager.CheckPasswordAsync(existingUser, userLoginRequest.Password);
                    if (isPasswordValid)
                    {
                        // if user exist
                        var token = GenerateJwtToken(existingUser);
                        return Ok(new RegistrationRequestResponse()
                        {
                            Token = token,
                            Result = true
                        });
                    }
                    else
                    {
                        return BadRequest("Something went wron");
                    }
                }
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor() {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };

            var token = jwtToken.CreateToken(tokenDescriptor);
            var jToken = jwtToken.WriteToken(token);

            return jwtToken.ToString();
        }
    }
}
