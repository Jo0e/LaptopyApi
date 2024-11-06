using Laptopy.DTOs;
using Laptopy.Models;
using Laptopy.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Laptopy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(ApplicationUserDTO userDTO)
        {
            if (roleManager.Roles.IsNullOrEmpty())
            {
                await roleManager.CreateAsync(new(SD.AdminRole));
                await roleManager.CreateAsync(new(SD.CustomerRole));
            }

            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    FirstName = userDTO.FirstName, 
                    LastName = userDTO.LastName,
                    Email = userDTO.Email,
                    UserName = userDTO.Email
                };

                var result = await userManager.CreateAsync(user, userDTO.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, SD.CustomerRole);
                    await signInManager.SignInAsync(user, false);

                    return Ok();
                }

                return BadRequest(result.Errors);
            }

            return BadRequest(userDTO);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Email);

            if (user != null)
            {
                var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);

                if (result)
                {
                    await signInManager.SignInAsync(user, loginDTO.RememberMe);

                    return Ok();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login Failed");
                }
            }
            return NotFound();
        }

        [HttpDelete("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }



    }
}
