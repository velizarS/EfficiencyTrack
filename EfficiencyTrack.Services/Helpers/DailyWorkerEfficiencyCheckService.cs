using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Helpers
{
    public class DailyWorkerEfficiencyCheckService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<DailyWorkerEfficiencyCheckService> _logger;
        private readonly string _adminEmail = "velizar.stankov@bg.abb.com";

        public DailyWorkerEfficiencyCheckService(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<EmailSettings> emailSettings,
            ILogger<DailyWorkerEfficiencyCheckService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings));
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var nowUtc = DateTime.UtcNow;
                    var scheduledTimeUtc = nowUtc.Date.AddHours(7);

                    using var scope = _serviceScopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<EfficiencyTrackDbContext>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    bool alreadySent = await dbContext.SentEfficiencyReports.AnyAsync(e => e.DateSent == nowUtc.Date, stoppingToken);
                    if (nowUtc >= scheduledTimeUtc && !alreadySent)
                    {
                        _logger.LogInformation("Започвам проверка за служители без записи - {TimeUtc}", nowUtc);

                        var shiftLeaders = await userManager.GetUsersInRoleAsync("ShiftLeader");

                        var workersByLeader = new Dictionary<string, List<Employee>>();
                        var previousDay = nowUtc.AddDays(-1).Date;

                        foreach (var leader in shiftLeaders)
                        {
                            if (string.IsNullOrWhiteSpace(leader.Email))
                                continue;

                            var workers = await GetWorkersWithoutEfficiencyRecordByShiftLeaderId(dbContext, leader.Id, previousDay);
                            if (workers.Any())
                            {
                                workersByLeader[leader.Email] = workers;
                            }
                        }

                        foreach (var kvp in workersByLeader)
                        {
                            await SendEmail(kvp.Key, "Работници без записи за вашата смяна", BuildWorkerListText(kvp.Value));
                            _logger.LogInformation("Изпратен имейл до началник смяна: {Email} с {Count} работници без записи", kvp.Key, kvp.Value.Count);
                        }

                        if (workersByLeader.Any())
                        {
                            var allWorkersText = new StringBuilder();
                            foreach (var kvp in workersByLeader)
                            {
                                allWorkersText.AppendLine($"Началник смяна: {kvp.Key}");
                                allWorkersText.AppendLine(BuildWorkerListText(kvp.Value));
                                allWorkersText.AppendLine();
                            }

                            await SendEmail(_adminEmail, "Всички служители без записи - обобщение", allWorkersText.ToString());
                            _logger.LogInformation("Изпратен обобщен имейл до администратор: {Email}", _adminEmail);
                        }
                        else
                        {
                            _logger.LogInformation("Няма служители без записи за предходния ден.");
                        }

                        dbContext.SentEfficiencyReports.Add(new SentEfficiencyReport { DateSent = nowUtc.Date });
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                    else if (alreadySent)
                    {
                        _logger.LogInformation("Отчет за деня {Date} вече е изпратен.", nowUtc.Date);
                    }
                    else
                    {
                        _logger.LogInformation("Все още не е достигнат часът за изпращане. Текущ час UTC: {Hour}", nowUtc.Hour);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Грешка при изпълнение на DailyWorkerEfficiencyCheckService");
                }

                var nextRunLocal = DateTime.Today.AddDays(1).AddHours(9);
                var nextRunUtc = nextRunLocal.ToUniversalTime();
                var delay = nextRunUtc - DateTime.UtcNow;

                if (delay < TimeSpan.Zero)
                    delay = TimeSpan.Zero;

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }

        private async Task<List<Employee>> GetWorkersWithoutEfficiencyRecordByShiftLeaderId(EfficiencyTrackDbContext dbContext, Guid shiftLeaderUserId, DateTime dateToCheck)
        {
            return await dbContext.Employees
                .Where(w => w.ShiftManagerUserId == shiftLeaderUserId &&
                            !dbContext.DailyEfficiencies.Any(e => e.Employee.Code == w.Code && e.Date == dateToCheck))
                .ToListAsync();
        }

        private async Task SendEmail(string recipientEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
                throw new ArgumentException("Sender email is not configured.", nameof(_emailSettings.SenderEmail));

            if (string.IsNullOrWhiteSpace(recipientEmail))
                throw new ArgumentException("Recipient email is null or empty.", nameof(recipientEmail));

            using var message = new MailMessage();
            message.From = new MailAddress(_emailSettings.SenderEmail);
            message.To.Add(new MailAddress(recipientEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = false;

            using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                EnableSsl = _emailSettings.EnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword)
            };

            await client.SendMailAsync(message);
        }

        private string BuildWorkerListText(List<Employee> workers)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Следните работници нямат записи за предходния ден:");
            foreach (var w in workers)
            {
                sb.AppendLine($"Име: {w.FirstName} {w.LastName}, Код: {w.Code}");
            }
            return sb.ToString();
        }
    }
}