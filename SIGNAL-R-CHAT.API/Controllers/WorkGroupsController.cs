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
    public class WorkGroupsController : ControllerBase
    {
        private readonly Context _context;

        public WorkGroupsController(Context context)
        {
            _context = context;
        }

        // GET: api/WorkGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkGroup>>> GetWorkGroups()
        {
            return await _context.WorkGroups.ToListAsync();
        }

        // GET: api/WorkGroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkGroup>> GetWorkGroup(Guid id)
        {
            var workGroup = await _context.WorkGroups.FindAsync(id);

            if (workGroup == null)
            {
                return NotFound();
            }

            return workGroup;
        }

        // PUT: api/WorkGroups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkGroup(Guid id, WorkGroup workGroup)
        {
            if (id != workGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(workGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkGroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/WorkGroups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WorkGroup>> PostWorkGroup(WorkGroup workGroup)
        {
            _context.WorkGroups.Add(workGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkGroup", new { id = workGroup.Id }, workGroup);
        }

        // DELETE: api/WorkGroups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkGroup(Guid id)
        {
            var workGroup = await _context.WorkGroups.FindAsync(id);
            if (workGroup == null)
            {
                return NotFound();
            }

            _context.WorkGroups.Remove(workGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkGroupExists(Guid id)
        {
            return _context.WorkGroups.Any(e => e.Id == id);
        }
    }
}
