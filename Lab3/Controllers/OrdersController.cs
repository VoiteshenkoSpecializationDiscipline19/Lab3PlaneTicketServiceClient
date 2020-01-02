using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lab3.Models;
using Lab3.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lab3.Controllers
{
    public class OrdersController : Controller
    {
        private static string userEmail;
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

        public async Task<IActionResult> ReadAllDataAsync([DataSourceRequest] DataSourceRequest request)
        {
            var routes = await planeTicketService.ReadAllData();
            if (routes == null)
            {
                return View("Index");
            }
            return Json(routes.ToDataSourceResult(request));
        }

        public async Task<IActionResult> ReadDataAsync([DataSourceRequest] DataSourceRequest request)
        {
            var orders = await planeTicketService.ReadData(userEmail);
            if (orders == null)
            {
                return View("Index");
            }
            return Json(orders.ToDataSourceResult(request));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([DataSourceRequest] DataSourceRequest request,
            [Bind("routeFrom", "routeWhere", "routeDate")] Route route)
        {
            if (ModelState.IsValid)
            {
                route = await planeTicketService.Create(userEmail, route);
                if (route == null)
                {
                    return View("Index");
                }
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync([DataSourceRequest] DataSourceRequest request, Route route)
        {
            if (ModelState.IsValid)
            {
                route = await planeTicketService.Edit(userEmail, route);
                if (route == null)
                {
                    return View("Index");
                }
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync([DataSourceRequest] DataSourceRequest request, Route route)
        {
            route = await planeTicketService.Delete(userEmail, route);
            if (route == null)
            {
                return View("Index");
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }
    }
}