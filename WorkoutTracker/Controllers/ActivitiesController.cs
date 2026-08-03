using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/Activities (Получить все активности за все время)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivities()
        {
            return await _context.Activities.Include(a => a.Exercise).ToListAsync();
        }

        // 2. GET: api/Activities/filter (Получить за конкретный день или месяц)
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetFilteredActivities(DateTime? date, int? month, int? year)
        {
            var query = _context.Activities.Include(a => a.Exercise).AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(a => a.Date.Date == date.Value.Date);
            }
            else if (month.HasValue && year.HasValue)
            {
                query = query.Where(a => a.Date.Month == month.Value && a.Date.Year == year.Value);
            }

            return await query.ToListAsync();
        }

        // 3. GET: api/Activities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(int id)
        {
            var activity = await _context.Activities.Include(a => a.Exercise).FirstOrDefaultAsync(a => a.Id == id);
            if (activity == null) return NotFound();
            return activity;
        }

        // 4. POST: api/Activities (Заведение новой активности)
        [HttpPost]
        public async Task<ActionResult<Activity>> PostActivity(Activity activity)
        {
            var exercise = await _context.Exercises.FindAsync(activity.ExerciseId);
            if (exercise == null || !exercise.IsActive)
            {
                return BadRequest("Нельзя выбрать неактивное или несуществующее упражнение.");
            }

            var minutesThatDay = await _context.Activities
                .Where(a => a.Date.Date == activity.Date.Date)
                .SumAsync(a => a.DurationMinutes);

            if (minutesThatDay + activity.DurationMinutes > 1440)
            {
                return BadRequest($"Превышен дневной лимит. В этот день уже занято {minutesThatDay} минут из 1440.");
            }

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivity), new { id = activity.Id }, activity);
        }

        // 5. PUT: api/Activities/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActivity(int id, Activity activity)
        {
            if (id != activity.Id) return BadRequest();

            var existingActivity = await _context.Activities
                .AsNoTracking()
                .Include(a => a.Exercise)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existingActivity == null) return NotFound();

            if (existingActivity.Exercise != null && !existingActivity.Exercise.IsActive)
            {
                if (existingActivity.ExerciseId != activity.ExerciseId)
                {
                    return BadRequest("Редактирование поля 'Упражнение' запрещено, так как текущее упражнение стало неактивным.");
                }
            }

            var minutesThatDay = await _context.Activities
                .AsNoTracking()
                .Where(a => a.Date.Date == activity.Date.Date && a.Id != id)
                .SumAsync(a => a.DurationMinutes);

            if (minutesThatDay + activity.DurationMinutes > 1440)
            {
                return BadRequest("Превышен дневной лимит в 1440 минут.");
            }

            _context.Entry(activity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 6. DELETE: api/Activities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return NotFound();

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // 7. GET: api/Activities/daily-summary?date=2026-08-02
        [HttpGet("daily-summary")]
        public async Task<ActionResult<DailySummaryDto>> GetDailySummary(DateTime date)
        {
            var totalMinutes = await _context.Activities
                .Where(a => a.Date.Date == date.Date)
                .SumAsync(a => a.DurationMinutes);

            string color;
            string message;

            if (totalMinutes < 30)
            {
                color = "Yellow";
                message = "Активность низкая";
            }
            else if (totalMinutes <= 90)
            {
                color = "Green";
                message = "Активность в норме";
            }
            else
            {
                color = "Red";
                message = "Активность высокая, возможно переутомление";
            }

            return new DailySummaryDto
            {
                Date = date.Date,
                TotalMinutes = totalMinutes,
                StickerColor = color,
                Message = message
            };
        }
    }
    public class DailySummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalMinutes { get; set; }
        public string StickerColor { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}