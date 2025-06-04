using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EfficiencyTrack.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<EfficiencyTrackDbContext>((serviceProvider, options) =>
{
    options.UseSqlServer(connectionString);
}, ServiceLifetime.Scoped);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEntryService, EntryService>();
builder.Services.AddScoped<IDailyEfficiencyService, DailyEfficiencyService>();



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
    .AddEntityFrameworkStores<EfficiencyTrackDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedRolesAsync(services);
        await SeedUsersAsync(services);

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding roles.");
    }
}

await app.RunAsync();

static async Task SeedRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    var roles = new[] { "Manager", "Admin", "UnitResponsible", "ShiftLeader", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
}

static async Task SeedUsersAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

    async Task<ApplicationUser> CreateUserIfNotExists(string email, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
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
                await userManager.AddToRoleAsync(user, role);
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

    var admin = await CreateUserIfNotExists("admin@example.com", "Admin");
    var manager = await CreateUserIfNotExists("manager@example.com", "Manager");

    var unitResponsibles = new List<ApplicationUser>();
    for (int i = 1; i <= 2; i++)
    {
        var unitResp = await CreateUserIfNotExists($"unitresp{i}@example.com", "UnitResponsible");
        unitResponsibles.Add(unitResp);
    }

    var shiftLeaders = new List<ApplicationUser>();
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
            await CreateUserIfNotExists($"user{userCounter}@example.com", "User");
            userCounter++;
        }
    }
}
