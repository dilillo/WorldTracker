using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorldTrackerDomain.Commands;
using WorldTrackerDomain.Queries;
using WorldTrackerWeb.Models;

namespace WorldTrackerWeb.Controllers
{
    public class VisitsController : Controller
    {
        private readonly IMediator _mediator;

        public VisitsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ActionResult> Create()
        {
            var model = new VisitCreateViewModel();

            var personGetAllQueryResult = await _mediator.Send(new PersonGetAllQuery());

            model.People = personGetAllQueryResult.People.Select(i => new SelectListItem(i.Name, i.ID)).ToList();

            model.PersonID = model.People.FirstOrDefault()?.Value;

            var placeGetAllQueryResult = await _mediator.Send(new PlaceGetAllQuery());

            model.Places = placeGetAllQueryResult.Places.Select(i => new SelectListItem(i.Name, i.ID)).ToList();

            model.PlaceID = model.Places.FirstOrDefault()?.Value;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<ActionResult> Create(VisitCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var cmd = new VisitCreateCommand
                {
                    PersonID = model.PersonID,
                    PlaceID = model.PlaceID
                };

                await _mediator.Send(cmd);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                var personGetAllQueryResult = await _mediator.Send(new PersonGetAllQuery());

                model.People = personGetAllQueryResult.People.Select(i => new SelectListItem(i.Name, i.ID)).ToList();

                var placeGetAllQueryResult = await _mediator.Send(new PlaceGetAllQuery());

                model.Places = placeGetAllQueryResult.Places.Select(i => new SelectListItem(i.Name, i.ID)).ToList();

                return View(model);
            }
        }
    }
}
