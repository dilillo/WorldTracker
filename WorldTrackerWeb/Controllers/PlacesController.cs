﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WorldTrackerWeb.Controllers
{
    public class PlacesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}