using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SIGNAL_R_CHAT.Domain;
using SIGNAL_R_CHAT.API.ViewModels;
using Microsoft.AspNetCore.Cors;


namespace SIGNAL_R_CHAT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrController : ControllerBase
    {
        private readonly UserManager<Person> _userManager;

        public RegistrController(
            UserManager<Person> userManager)
        {
            _userManager = userManager;
        }

        [EnableCors("LocalPolicy")]
        [HttpPost]
        public async Task<ActionResult<Person>> RegistrPerson(RegistrVIewModel registView)
        {
            Person person = new() { Email = registView.Email, Name = registView.PersonName, UserName = registView.UserName };
            IdentityResult result = await _userManager.CreateAsync(person, registView.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
                return BadRequest();
                    }
    }
}
