using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Helpers
{
    public class GreetingService
    {
        private readonly EfficiencyTrackDbContext _context;

        public GreetingService(EfficiencyTrackDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetGreetingAsync(Entry entry)
        {
            var todayEntries = await _context.Entries
                .AsNoTracking()
                .Where(x => x.EmployeeId == entry.EmployeeId && x.Date.Date == DateTime.UtcNow.Date)
                .OrderByDescending(e => e.CreatedOn)
                .ToListAsync();

            string message = "Успешен запис!\n";

            if (todayEntries.Count == 1)
            {
                var workerName = (await _context.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == entry.EmployeeId))?.FirstName;

                if (!string.IsNullOrEmpty(workerName))
                {
                    message += $"Здравейте, {workerName}!\n";
                    message += "Пожелавам ви лека работа и успешен ден!\n";
                }
            }

            message += entry.EfficiencyForOperation switch
            {
                >= 100 => "Представянето надминава очакванията ни със супер резултатите Ви. Благодарим!\n",
                >= 90 => "Представянето оправдава очакванията ни с добрите Ви резултати. Благодарим!\n",
                >= 85 => "Представянето частично отговаря на очакванията ни, необходимо е подобрение.\nС какво можем да Ви помогнем? Обърнете се към прекия Ви ръководител!\n",
                _ => "Незадоволително представяне.\nНеобходими са навременни коригиращи действия.\nС какво можем да Ви помогнем? Обърнете се към прекия Ви ръководител!\n"
            };

            return message;
        }
    }
}
