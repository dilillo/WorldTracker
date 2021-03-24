using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WorldTrackerDomain.Commands;
using WorldTrackerDomain.Queries;
using WorldTrackerWeb.Components;
using WorldTrackerWeb.Models;

namespace WorldTrackerWeb.Controllers
{
    public class PeopleController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IBlobUploader _blobUploader;

        public PeopleController(IMediator mediator, IBlobUploader blobUploader)
        {
            _blobUploader = blobUploader;
            _mediator = mediator;
        }

        public async Task<ActionResult> Index()
        {
            var model = await _mediator.Send(new PersonGetAllQuery());

            return View(model.People);
        }

        public async Task<ActionResult> Details(string id)
        {
            var model = await _mediator.Send(new PersonGetByIDQuery { ID = id });

            return View(model.Person);
        }

        public ActionResult Create()
        {
            var model = new PersonCreateViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<ActionResult> Create(PersonCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var cmd = new PersonCreateCommand
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = model.Name
                };

                cmd.PictureUrl = await _blobUploader.UploadPersonPicture(model.Picture);

                await _mediator.Send(cmd);

                return RedirectToAction("Details", "People", new { id = cmd.ID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View(model);
            }
        }
    }
}
