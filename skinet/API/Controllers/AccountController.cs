using API.Dtos;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using API.Errors;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;


    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await this._userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) { return Unauthorized(new ApiResponse(401)); }

        var result = await this._signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if(!result.Succeeded) { return Unauthorized(new ApiResponse(401)); }

        return new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = "This is Token"
        };
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email,
            DisplayName = registerDto.DisplayName
        };

        var result = await this._userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(new ApiResponse(400));

        return new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = "This will be a token"
        };
    }
}
