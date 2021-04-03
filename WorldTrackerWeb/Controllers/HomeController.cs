using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using WorldTrackerDomain.Queries;
using WorldTrackerDomain.Views;
using WorldTrackerWeb.Models;

namespace WorldTrackerWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var summaryViewPrediction = TempData["SummaryViewPrediction"] as string;

            var model = string.IsNullOrEmpty(summaryViewPrediction) ? 
                await _mediator.Send(new SummaryQuery()) : 
                JsonSerializer.Deserialize<SummaryView>(summaryViewPrediction);

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
