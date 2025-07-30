using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Helpers;
using EfficiencyTrack.Services.Implementations;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<EfficiencyTrackDbContext>((serviceProvider, options) =>
{
    _ = options.UseSqlServer(connectionString);
}, ServiceLifetime.Scoped);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEntryService, EntryService>();
builder.Services.AddScoped<IDailyEfficiencyService, DailyEfficiencyService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IRoutingService, RoutingService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IGreetingService, GreetingService>();

builder.Services.AddHostedService<DailyWorkerEfficiencyCheckService>();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<EfficiencyTrackDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    try
    {
        await SeedRolesAsync(services);
        await SeedUsersAsync(services);
    }
    catch (Exception ex)
    {
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding roles or users.");
    }
}

await app.RunAsync();

static async Task SeedRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    string[] roles = new[] { "Manager", "Admin", "UnitResponsible", "ShiftLeader", "User" };

    foreach (string role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            _ = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
}

static async Task SeedUsersAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

    async Task<ApplicationUser> CreateUserIfNotExists(string email, string role)
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded)
            {
                _ = await userManager.AddToRoleAsync(user, role);
                logger.LogInformation($"User '{email}' created and assigned to role '{role}'.");
            }
            else
            {
                logger.LogError($"Failed to create user '{email}'. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            logger.LogInformation($"User '{email}' already exists.");
        }
        return user;
    }

    List<ApplicationUser> unitResponsibles = new List<ApplicationUser>();
    for (int i = 1; i <= 2; i++)
    {
        var unitResp = await CreateUserIfNotExists($"unitresp{i}@example.com", "UnitResponsible");
        unitResponsibles.Add(unitResp);
    }

    List<ApplicationUser> shiftLeaders = new List<ApplicationUser>();
    int shiftLeaderCounter = 1;
    foreach (var unitResp in unitResponsibles)
    {
        for (int i = 1; i <= 2; i++)
        {
            var shiftLeader = await CreateUserIfNotExists($"shiftleader{shiftLeaderCounter}@example.com", "ShiftLeader");
            shiftLeaders.Add(shiftLeader);
            shiftLeaderCounter++;
        }
    }

    int userCounter = 1;
    foreach (var shiftLeader in shiftLeaders)
    {
        for (int i = 1; i <= 2; i++)
        {
            _ = await CreateUserIfNotExists($"user{userCounter}@example.com", "User");
            userCounter++;
        }
    }
}
