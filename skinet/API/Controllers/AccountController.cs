using API.Dtos;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using API.Errors;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using API.Extensions;
using AutoMapper;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;


    public AccountController(UserManager<AppUser> userManager, 
        SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await this._userManager.FindByEmailFromClaimsPrinciple(User);

        return new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = this._tokenService.CreateToken(user)
        };
    }

    [HttpGet("emailexists")]
    public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
    {
        var result = await this._userManager.FindByEmailAsync(email);

        return result != null;
    }

    [Authorize]
    [HttpGet("address")]
    public async Task<ActionResult<AddressDto>> GetAddress()
    {
        var user = await this._userManager.FindByUserByClaimsPrincipleWithAddressAsync(User);

        return this._mapper.Map<AddressDto>(user.Address);
    }

    [Authorize]
    [HttpPut("address")]
    public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto addressDto)
    {
        var user = await this._userManager.FindByUserByClaimsPrincipleWithAddressAsync(User);

        user.Address = this._mapper.Map<Address>(addressDto);

        var result = await this._userManager.UpdateAsync(user);

        if (result.Succeeded) return Ok(this._mapper.Map<AddressDto>(user.Address));

        return BadRequest("Problem during updating address");
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await this._userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) { return Unauthorized(new ApiResponse(401)); }

        var result = await this._signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded) { return Unauthorized(new ApiResponse(401)); }

        return new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = _tokenService.CreateToken(user)
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
            Token = _tokenService.CreateToken(user)
        };
    }
}
