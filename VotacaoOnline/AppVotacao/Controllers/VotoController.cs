using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppVotacao.Models;


namespace AppVotacao.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotoController : ControllerBase
    {
        private readonly VotoDbContext _context;

        public VotoController(VotoDbContext context)
        {
            _context = context;
        }

        // GET: api/Voto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Voto>>> GetVotos()
        {
            var list = await _context.Votos.ToListAsync();
            return list;
        }

        // GET: api/Voto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Voto>> GetVoto(int id)
        {
            var voto = await _context.Votos.FindAsync(id);

            if (voto == null)
            {
                return NotFound();
            }

            return voto;
        }

        // PUT: api/Voto/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVoto(int id, Voto voto)
        {
            if (id != voto.Id)
            {
                return BadRequest();
            }

            _context.Entry(voto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VotoExists(id))
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

        // POST: api/Voto
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Voto>> PostVoto(Voto voto)
        {
            voto.Data = DateTime.Now.ToString("dd-MM-yyyy");

            if (VencedoresDaSemana(voto.RestauranteId))
            {
                var x = "";
            }

            if (UsuariojaVotou(voto.UsuarioId))
            {
                Response.StatusCode = 400;
                return Content("Usuário já votou hoje!");
            }
                


            _context.Votos.Add(voto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVoto", new { id = voto.Id }, voto);
        }

        // DELETE: api/Voto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoto(int id)
        {
            var voto = await _context.Votos.FindAsync(id);
            if (voto == null)
            {
                return NotFound();
            }

            _context.Votos.Remove(voto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VotoExists(int id)
        {
            return _context.Votos.Any(e => e.Id == id);
        }

        private bool VencedoresDaSemana(int RestauranteId)
        {
            var dt = SemanaHelper.PrimeiroDiaDaSemana(DateTime.Now);
            var list = _context.Votos.Where(v => Convert.ToDateTime(v.Data) >= dt).ToListAsync();



            return _context.Votos.Any(e => e.Id == RestauranteId);
        }

        private bool UsuariojaVotou(int UsuarioId)
        {
            var dt = DateTime.Now.ToString("yyyy-MM-dd");
            
            return _context.Votos.Where(e => e.Data == dt).Any(e => e.UsuarioId == UsuarioId);                         
        }
    }
}
