using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICodingChallenge.Models;
using Newtonsoft.Json.Linq;

namespace APICodingChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly APICodingChallengeContext _context;

        public PlayersController(APICodingChallengeContext context)
        {
            _context = context;
        }

        // GET: api/Players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            return await _context.Players.Include(p => p.Team)
                                         .ThenInclude(t => t.Players)
                                         .ToListAsync();
        }

        // GET: api/Players/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayer(int id)
        {
            var player = await _context.Players.Include(p => p.Team).Where(p => p.Id == id).FirstOrDefaultAsync();

            if (player == null)
            {
                return NotFound();
            }

            return player;
        }

        // GET: api/Players/ByLastName/Pickett
        [HttpGet("ByLastName/{name}")]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayersByLastName(string name)
        {
            return await _context.Players.Where(p => p.LastName.ToLower() == name.ToLower()).Include(p => p.Team).ToListAsync();
        }

        // GET: api/Players/ByTeam/2
        [HttpGet("ByTeam/{teamId}")]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayersByTeam(int teamId)
        {
            return await _context.Players.Where(p => p.TeamId == teamId).Include(p => p.Team).ToListAsync();
        }

        // PUT: api/Players/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayer(int id, Player player)
        {
            if (id != player.Id)
            {
                return BadRequest();
            }

            _context.Entry(player).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(id))
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

        // POST: api/Players
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(Player player)
        {
            if (player == null)
            {
                return BadRequest();
            }
            
            Team? playersTeam = await _context.Teams.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == player.TeamId);

            if (playersTeam == null)
            {
                return BadRequest(string.Format("A team with the Id of {0} does not exist.", player.TeamId)); 
            }

            if (playersTeam.Players.Count == 8)
            {
                return BadRequest("This team has already reached it's maxium number of rostered players. (8)");
            }

            _context.Players.Add(player);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayer", new { id = player.Id }, player);
        }

        // DELETE: api/Players/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.Id == id);
        }
    }
}
