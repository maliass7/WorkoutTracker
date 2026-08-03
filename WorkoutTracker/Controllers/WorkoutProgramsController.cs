using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutProgramsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkoutProgramsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/WorkoutPrograms (Получить список всех программ)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkoutProgram>>> GetWorkoutPrograms()
        {
            return await _context.WorkoutPrograms.ToListAsync();
        }

        // 2. GET: api/WorkoutPrograms/5 (Получить программу по ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutProgram>> GetWorkoutProgram(int id)
        {
            var program = await _context.WorkoutPrograms.FindAsync(id);

            if (program == null)
            {
                return NotFound();
            }

            return program;
        }

        // 3. POST: api/WorkoutPrograms (Создать новую программу)
        [HttpPost]
        public async Task<ActionResult<WorkoutProgram>> PostWorkoutProgram(WorkoutProgram workoutProgram)
        {
            _context.WorkoutPrograms.Add(workoutProgram);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWorkoutProgram), new { id = workoutProgram.Id }, workoutProgram);
        }

        // 4. PUT: api/WorkoutPrograms/5 (Изменить существующую программу)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkoutProgram(int id, WorkoutProgram workoutProgram)
        {
            if (id != workoutProgram.Id)
            {
                return BadRequest();
            }

            _context.Entry(workoutProgram).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 5. DELETE: api/WorkoutPrograms/5 (Удалить программу)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkoutProgram(int id)
        {
            var workoutProgram = await _context.WorkoutPrograms.FindAsync(id);
            if (workoutProgram == null)
            {
                return NotFound();
            }

            _context.WorkoutPrograms.Remove(workoutProgram);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}