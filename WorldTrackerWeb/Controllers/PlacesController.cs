﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using WorldTrackerDomain.Commands;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Queries;
using WorldTrackerWeb.Components;
using WorldTrackerWeb.Models;

namespace WorldTrackerWeb.Controllers
{
    public class PlacesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IBlobUploader _blobUploader;

        public PlacesController(IMediator mediator, IBlobUploader blobUploader)
        {
            _blobUploader = blobUploader;
            _mediator = mediator;
        }

        public async Task<ActionResult> Index()
        {
            var pendingDomainEvents = TempData["PendingDomainEvents"] as string;

            var model = await _mediator.Send(new PlaceGetAllQuery
            {
                PendingDomainEvents = string.IsNullOrEmpty(pendingDomainEvents) ? null : JsonSerializer.Deserialize<DomainEvent[]>(pendingDomainEvents)
            });

            return View(model);
        }

        public async Task<ActionResult> Details(string id)
        {
            var model = await _mediator.Send(new PlaceGetByIDQuery { ID = id });

            return View(model);
        }

        public ActionResult Create()
        {
            var model = new PlaceCreateViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PlaceCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var cmd = new PlaceCreateCommand
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = model.Name
                };

                cmd.PictureUrl = await _blobUploader.UploadPlacePicture(model.Picture);

                var events = await _mediator.Send(cmd);

                var serializedPendingDomainEvents = JsonSerializer.Serialize(events);

                TempData.Add("PendingDomainEvents", serializedPendingDomainEvents);

                return RedirectToAction("Details", "Places", new { id = cmd.ID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View(model);
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            var data = await _mediator.Send(new PlaceGetByIDQuery { ID = id });

            if (data == null)
            {
                return NotFound();
            }

            var model = new PlaceEditViewModel
            {
                ID = data.PlaceGetByIDView.Place.ID,
                Name = data.PlaceGetByIDView.Place.Name
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PlaceEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var cmd = new PlaceUpdateCommand
                {
                    ID = model.ID,
                    Name = model.Name
                };

                var events = await _mediator.Send(cmd);

                var serializedPendingDomainEvents = JsonSerializer.Serialize(events);

                TempData.Add("PendingDomainEvents", serializedPendingDomainEvents);

                return RedirectToAction("Details", "Places", new { id = cmd.ID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View(model);
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            var model = await _mediator.Send(new PlaceGetByIDQuery { ID = id });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var cmd = new PlaceDeleteCommand
                {
                    ID = id
                };

                var events = await _mediator.Send(cmd);

                var serializedPendingDomainEvents = JsonSerializer.Serialize(events);

                TempData.Add("PendingDomainEvents", serializedPendingDomainEvents);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                var model = await _mediator.Send(new PlaceGetByIDQuery { ID = id });

                return View(nameof(Delete), model);
            }
        }
    }
}
