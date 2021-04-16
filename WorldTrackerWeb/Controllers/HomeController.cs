using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Queries;
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
            var query = new SummaryQuery();

            var pendingDomainEventsSerialized = TempData["PendingDomainEvents"] as string;

            if (!string.IsNullOrEmpty(pendingDomainEventsSerialized))
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

                query.PendingDomainEvents = JsonConvert.DeserializeObject<DomainEvent[]>(pendingDomainEventsSerialized, settings);
            }

            var model = await _mediator.Send(query);

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
