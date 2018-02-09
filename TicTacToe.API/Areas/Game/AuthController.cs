using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.API.Areas.Models;
using TicTacToe.API.Models;
using TicTacToe.BL;
using TicTacToe.DAL.Services;

namespace TicTacToe.API.Areas.Game
{
    [Route("api/auth")]
    public class AuthController : BaseGameController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<object> SignIn(string name, string password)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(name, password, true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Ok("Authorized");
                }
                else
                {
                    return BadRequest("Invalid login attempt.");
                }
            }
            
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<object> SignUp(string name, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = name };
                var result = await _userManager.CreateAsync(user, password);                
                if (result.Succeeded)
                {
                    return StatusResult(200, "Registered. Please, sign in.");
                }
                else
                {
                    return StatusResult(400, result.Errors.FirstOrDefault()?.Description);
                }
            }

            return Unauthorized();
        }

        [HttpPost("signout")]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("info")]
        public async Task<object> CurrentUserInfo()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return StatusResult(403, "Please, sign in.");
            }

            return EntityResult(new { PlayerName = currentUser.UserName, PlayerId = currentUser.Id });
        }
    }
}
