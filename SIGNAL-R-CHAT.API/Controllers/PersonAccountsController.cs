using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGNAL_R_CHAT.Domain;
using SIGNAL_R_CHAT.Infrastructure;

namespace SIGNAL_R_CHAT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonAccountsController : ControllerBase
    {
        private readonly Context _context;

        public PersonAccountsController(Context context)
        {
            _context = context;
        }

        // GET: api/Persons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetWorkGroups()
        {
            return await _context.Persons.ToListAsync();
        }


        [HttpPost]
        public async Task<ActionResult<Person>>PostPerson(Person person)
        {
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
        }
    }
        
}

