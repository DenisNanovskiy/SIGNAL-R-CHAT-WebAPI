using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SIGNAL_R_CHAT.Domain;
using SIGNAL_R_CHAT.API.ViewModels;

namespace SIGNAL_R_CHAT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<Person> _signInManager;
        private readonly UserManager<Person> _userManager;

        public LoginController(
            UserManager<Person> userManager,
            SignInManager<Person> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpPost]
        public async Task<ActionResult<Person>> LoginPerson(LoginViewModel loginModel)
        {
            var result = await _signInManager.PasswordSignInAsync(loginModel.UserName, loginModel.Password, false, false);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
                return BadRequest();
        }
    }
}
