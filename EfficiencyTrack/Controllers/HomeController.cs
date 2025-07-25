using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels;
using EfficiencyTrack.ViewModels.HomeViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


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
            IEnumerable<Services.DTOs.TopEfficiencyDto> top10Today = await _dailyEfficiencyService.GetTop10ForTodayAsync();
            IEnumerable<Services.DTOs.TopEfficiencyDto> top10ThisMonth = await _dailyEfficiencyService.GetTop10ForThisMonthAsync();

            HomeIndexViewModel viewModel = new()
            {
                Top10Today = top10Today.Select(dto => new TopEfficiencyViewModel
                {
                    FullName = dto.FullName,
                    EfficiencyPercentage = dto.EfficiencyPercentage,
                    DepartmentName = dto.DepartmentName,
                    ShiftManagerName = dto.ShiftManagerName,
                    ShiftName = dto.ShiftName
                }).ToList(),

                Top10ThisMonth = top10ThisMonth.Select(dto => new TopEfficiencyViewModel
                {
                    FullName = dto.FullName,
                    EfficiencyPercentage = dto.EfficiencyPercentage,
                    DepartmentName = dto.DepartmentName,
                    ShiftManagerName = dto.ShiftManagerName,
                    ShiftName = dto.ShiftName
                }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult StatusCode(int code)
        {
            ViewBag.Code = code;
            return View(); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
