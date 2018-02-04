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
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet] //TODO: change to HttpPost
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string name, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(name, password, true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }

            // If we got this far, something failed, redisplay form
            return Unauthorized();
        }
    }
}
