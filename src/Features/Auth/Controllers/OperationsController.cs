using HUBT_Social_API.src.Features.Auth.Services.IAuthServices;
using HUBT_Social_API.src.Features.Authentication.Models;
using HUBT_Social_API.src.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.src.Features.Auth.Dtos.Request;
using HUBT_Social_API.src.Features.Auth.Dtos.Request.LoginRequest;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using HUBT_Social_API.src.Features.Login.Services;

namespace HUBT_Social_API.src.Features.Login.Controllers
{
    [Route("api/operations")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IUserManagerS _userManagerService;
        private readonly RoleManager<ARole> _roleManager;
        private readonly ILogger<OperationsController> _logger;
        private readonly IEmailService _emailService;
        public OperationsController(IUserManagerS userManagerService, RoleManager<ARole> roleManager, ILogger<OperationsController> logger, IEmailService emailSender)
        {
            _userManagerService = userManagerService;
            _roleManager = roleManager;
            _logger = logger;
            _emailService = emailSender;
        }
        [HttpPost("addRole")]
        public async Task<ActionResult> CreateRole([FromBody] CreateRoleRequest createRoleRequest)
        {
            ARole newRole = new() { Name = createRoleRequest.RoleName };
            var result = await _roleManager.CreateAsync(newRole);
            if (!result.Succeeded)
            {
                _logger.LogError(result.Errors.First().Description);
                return BadRequest(new { Message = $"Create role fail  {result?.Errors?.First()?.Description}" });
            }
            _logger.LogInformation("Crate role.");
            return Ok(new { Message = "Create role success" });
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterRequest registerRequest)
        {
            RegisterResponse result = await _userManagerService.Register(registerRequest);
            _logger.LogInformation("User has register.");
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginByStudentCodeRequest loginRequest)
        {
            LoginResponse result = await _userManagerService.Login(loginRequest);
            _logger.LogInformation("User logged in.");
            return result.Success ? Ok(new { result.AccessToken }) : BadRequest(result.Message);
        }
        [HttpPut]
        [Route("refreshToken")]
        public async Task<ActionResult> RefreshToken(RefreshTokenRequest request)
        {
            TokenResponse result = await _userManagerService.RefreshToken(request);
            _logger.LogInformation("User Token is refreshed.");
            return result.Success ? Ok(result.Token) : Unauthorized(result.Message);
        }
        [HttpGet("ValidateToken")]
        public ActionResult ValidateToken()
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            DecodeTokenResponse result = _userManagerService.ValidateToken(token);
            return result.Success ? Ok() : Unauthorized(result.Message);
        }



    }
}
