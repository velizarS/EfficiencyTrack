using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Helpers
{
    public class GreetingService : IGreetingService
    {
        private readonly EfficiencyTrackDbContext _context;

        public GreetingService(EfficiencyTrackDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<string> GetGreetingMessageAsync(Entry entry)
        {
            var todayEntryCount = await _context.Entries
                .AsNoTracking()
                .CountAsync(x => x.EmployeeId == entry.EmployeeId && x.Date.Date == DateTime.UtcNow.Date);

            string message = "Успешен запис!\n";

            if (todayEntryCount == 1)
            {
                var workerName = await _context.Employees
                    .AsNoTracking()
                    .Where(x => x.Id == entry.EmployeeId)
                    .Select(x => x.FirstName)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(workerName))
                {
                    message += $"Здравейте, {workerName}!\n";
                    message += "Пожелавам ви лека работа и успешен ден!\n";
                }
            }
            else
            {
                if (entry.EfficiencyForOperation >= 90)
                {
                    message += "Добра работа! Продължавайте така!\n";
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
