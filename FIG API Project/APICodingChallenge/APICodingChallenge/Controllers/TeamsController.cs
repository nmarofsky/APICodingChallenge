using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICodingChallenge.Models;

namespace APICodingChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly APICodingChallengeContext _context;

        public TeamsController(APICodingChallengeContext context)
        {
            _context = context;
        }

        // GET: api/Teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _context.Teams.Include(t => t.Players).ToListAsync();
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var team = await _context.Teams.Include(t => t.Players).Where(t => t.Id == id).FirstOrDefaultAsync();

            if (team == null)
            {
                return NotFound();
            }

            return team;
        }

        // GET: api/Teams/OrderedByName
        [HttpGet("OrderedByName")]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeamsOrderedByName()
        {
            return await _context.Teams.Include(t => t.Players).OrderBy(t => t.Name).ToListAsync();
        }

        // GET: api/Teams/OrderedByCity
        [HttpGet("OrderedByCity")]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeamsOrderedByCity()
        {
            return await _context.Teams.Include(t => t.Players).OrderBy(t => t.City).ToListAsync();
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(int id, Team team)
        {
            if (id != team.Id)
            {
                return BadRequest();
            }

            _context.Entry(team).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
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

        // POST: api/Teams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
            List<Team> sameNameTeams = await _context.Teams.Where(t => t.Name.ToLower() == team.Name.ToLower() && t.City.ToLower() == team.City.ToLower()).ToListAsync();
            
            if (sameNameTeams.Count > 0)
            {
                return BadRequest("You cannot have two teams with the same Name and City.");
            };

            _context.Teams.Add(team);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeam", new { id = team.Id }, team);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}
