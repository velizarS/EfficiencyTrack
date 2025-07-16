using System.Diagnostics;
using EfficiencyTrack.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EfficiencyTrack.ViewModels.HomeViewModel;


namespace EfficiencyTrack.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDailyEfficiencyService _dailyEfficiencyService;

        public HomeController(
             ILogger<HomeController> logger,
             IDailyEfficiencyService dailyEfficiencyService)
        {
            _logger = logger;
            _dailyEfficiencyService = dailyEfficiencyService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexViewModel
            {
                Top10Today = await _dailyEfficiencyService.GetTop10ForTodayAsync(),
                Top10ThisMonth = await _dailyEfficiencyService.GetTop10ForThisMonthAsync()
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
