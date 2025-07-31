using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class DailyWorkerEfficiencyCheckService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly string _adminEmail = "velizar.stankov@bg.abb.com";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyWorkerEfficiencyCheckService> _logger;

    public DailyWorkerEfficiencyCheckService(
        IServiceScopeFactory scopeFactory,
        ILogger<DailyWorkerEfficiencyCheckService> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }



    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(async _ => await CheckAndSendReportsAsync(DateTime.UtcNow, cancellationToken),
                           null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public async Task CheckAndSendReportsAsync(DateTime nowUtc, CancellationToken cancellationToken)
    {
        if (nowUtc.Hour < 7)
        {
            _logger.LogInformation("Все още не е достигнат часът за проверка на ефективност: {Time}", nowUtc);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EfficiencyTrackDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        var today = nowUtc.Date;

        if (await dbContext.SentEfficiencyReports.AnyAsync(r => r.DateSent == today, cancellationToken))
        {
            _logger.LogInformation("Докладът за ефективност за {Date} вече е изпратен.", today);
            return;
        }

        var shiftLeaders = await userManager.GetUsersInRoleAsync("ShiftLeader");

        var reportData = new Dictionary<string, List<Employee>>();

        foreach (var leader in shiftLeaders)
        {
            var workers = await dbContext.Employees
                .Where(e => e.ShiftManagerUserId == leader.Id)
                .ToListAsync(cancellationToken);

            var yesterday = today.AddDays(-1);

            var recordedIds = await dbContext.DailyEfficiencies
                .Where(de => de.Date == yesterday)
                .Select(de => de.EmployeeId)
                .ToListAsync(cancellationToken);

            var missingWorkers = workers
                .Where(w => !recordedIds.Contains(w.Id))
                .ToList();

            if (missingWorkers.Any())
            {
                var body = BuildWorkerListText(missingWorkers);
                await emailSender.SendEmailAsync(leader.Email, "Липсващи записи за ефективност", body);

                _logger.LogInformation("Изпратен имейл на {Email} за {Count} работника без записи.", leader.Email, missingWorkers.Count);

                reportData.Add($"{leader.Email}", missingWorkers);
            }
        }

        if (reportData.Any())
        {
            var summaryBuilder = new StringBuilder();
            summaryBuilder.AppendLine("Обобщен отчет за служители без записи за предходния ден:");
            summaryBuilder.AppendLine();

            foreach (var kvp in reportData)
            {
                summaryBuilder.AppendLine($"Началник смяна: {kvp.Key}");
                summaryBuilder.AppendLine(BuildWorkerListText(kvp.Value));
                summaryBuilder.AppendLine();
            }

            await emailSender.SendEmailAsync(_adminEmail, "Обобщен отчет: служители без записи", summaryBuilder.ToString());
            _logger.LogInformation("Изпратен обобщен имейл на администратора: {Email}", _adminEmail);
        }
        else
        {
            _logger.LogInformation("Няма служители без записи за предходния ден.");
        }

        dbContext.SentEfficiencyReports.Add(new SentEfficiencyReport { DateSent = today });
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public string BuildWorkerListText(List<Employee> workers)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Следните работници нямат записи за предходния ден:");
        foreach (var worker in workers)
        {
            sb.AppendLine($"Име: {worker.FirstName} {worker.LastName}, Код: {worker.Code}");
        }
        return sb.ToString();
    }
}
