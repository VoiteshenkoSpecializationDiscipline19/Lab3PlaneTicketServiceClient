using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lab3.Models;
using Lab3.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Lab3.Controllers
{
    public class OrdersController : Controller
    {
        private static String userEmail;
        private IPlaneTicketService planeTicketService;

        public OrdersController(IPlaneTicketService planeTicketService)
        {
            this.planeTicketService = planeTicketService;
        }
        public IActionResult Index()
        {
            ViewBag.userMessage = "Welcome, " + TempData["userName"];
            userEmail = TempData["userEmail"].ToString();
            return View();
        }

        public IActionResult ReadAllData([DataSourceRequest] DataSourceRequest request)
        {
            var routes = planeTicketService.ReadAllData().Result;
            if (routes == null)
            {
                return View("Index");
            }
            return Json(routes.ToDataSourceResult(request));
        }

        public IActionResult ReadData([DataSourceRequest] DataSourceRequest request)
        {
            var orders = planeTicketService.ReadData(userEmail).Result;
            if (orders == null)
            {
                return View("Index");
            }
            return Json(orders.ToDataSourceResult(request));
        }

        [HttpPost]
        public IActionResult Create([DataSourceRequest] DataSourceRequest request,
            [Bind("routeFrom", "routeWhere", "routeDate")] Route route)
        {
            if (ModelState.IsValid)
            {
                route = planeTicketService.Create(userEmail, route).Result;
                if (route == null)
                {
                    return View("Index");
                }
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public IActionResult Edit([DataSourceRequest] DataSourceRequest request, Route route)
        {
            if (ModelState.IsValid)
            {
                route = planeTicketService.Edit(userEmail, route).Result;
                if (route == null)
                {
                    return View("Index");
                }
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, Route route)
        {
            route = planeTicketService.Delete(userEmail, route).Result;
            if (route == null)
            {
                return View("Index");
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }
    }
}