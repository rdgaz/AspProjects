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
        private string _dataAtual { get; set; }


        public VotoController(VotoDbContext context)
        {
            _context = context;
            _dataAtual = DateTime.Now.ToString("dd/MM/yyyy");
        }

        // GET: api/Voto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Voto>>> GetVotos()
        {            
            return await _context.Votos.ToListAsync();
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
            voto.Data = _dataAtual;

            if (UsuariojaVotou(voto.UsuarioId))
            {
                Response.StatusCode = 400;
                return Content("Usuário já votou hoje!");
            }

            if (JavenceuNaSemana(voto.RestauranteId))
            {
                Response.StatusCode = 400;
                return Content("Este restaurante já venceu nesta semana!");
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

        private bool VotoExists(int id) => _context.Votos.Any(e => e.Id == id);

        private bool UsuariojaVotou(int UsuarioId) => _context.Votos.Where(e => e.Data == _dataAtual).Any(e => e.UsuarioId == UsuarioId);

        private bool JavenceuNaSemana(int RestauranteId)
        {
            return VencedoresDaSemana().Any(e => e.RestauranteId == RestauranteId);
        }

        private List<Voto> VencedoresDaSemana()
        {

            int dc = DateTime.Now >= DateTime.Parse("11:30:00") ? 0 : -1;

            var dia1 = SemanaHelper.PrimeiroDiaDaSemana(DateTime.Now).ToString("dd/MM/yyyy");

            var querySQL = @$"
                            select *,max(qtd) from (
	                            select *,count(*) as qtd FROM votos 
	                            where data BETWEEN '{dia1}' and '{DateTime.Now.AddDays(dc).ToString("dd/MM/yyyy")}' group by Data,RestauranteId)
                             group by data";

            return _context.Votos.FromSqlRaw(querySQL).ToList<Voto>();            
        } 
    }
}
