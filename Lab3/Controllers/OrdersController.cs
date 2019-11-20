using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Controllers
{
    public class OrdersController : Controller
    {
        private static String userEmail;
        private String initialUri;

        public OrdersController(IConfiguration configuration)
        {
            initialUri = configuration["Service_uri"];
        }
        public IActionResult Index()
        {
            ViewBag.userMessage = "Welcome, " + TempData["userName"];
            userEmail = TempData["userEmail"].ToString();
            return View();
        }

        public async Task<IActionResult> ReadAllData([DataSourceRequest] DataSourceRequest request)
        {
            List<Route> routes = null;
            List<Route> uniqueRoutes = null;

            var payment = await PaymentController.PayForMethod(
                new MethodDateUsage(DateTime.Now, DateTime.Now), "getFlightsInfo");

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                string response = null;
                using (var client = new HttpClient())
                {
                    var uri = new Uri(initialUri + "flights/" + token);
                    response = await client.GetStringAsync(uri);
                }
                routes = JsonConvert.DeserializeObject<List<Route>>(response);
                HashSet<string> from = new HashSet<string>();
                HashSet<string> to = new HashSet<string>();
                HashSet<string> dates = new HashSet<string>();
                uniqueRoutes = new List<Route>();
                foreach (var route in routes)
                {
                    from.Add(route.routeFrom);
                    to.Add(route.routeWhere);
                    dates.Add(route.routeDate);
                }
                var fromList = from.ToList();
                var toList = to.ToList();
                var dateList = dates.ToList();
                for (int i = 0; i < fromList.Count; i++)
                {
                    uniqueRoutes.Add(new Route(fromList[i], toList[i], dateList[i]));
                }
            }
            if (routes == null || payment is ErrorViewModel)
            {
                return View("Index");
            }
            return Json(uniqueRoutes.ToDataSourceResult(request));
        }

        public async Task<IActionResult> ReadData([DataSourceRequest] DataSourceRequest request)
        {
            List<Route> orders = null;

            var payment = await PaymentController.PayForMethod(
                new MethodDateUsage(DateTime.Now, DateTime.Now), "getFullUserFlightsInfo");

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                string response = null;
                using (var client = new HttpClient())
                {
                    var uri = new Uri(initialUri + "flights/" + userEmail + "/" + token);
                    response = await client.GetStringAsync(uri);
                }
                orders = JsonConvert.DeserializeObject<List<Route>>(response);
            }
            if (orders == null || payment is ErrorViewModel)
            {
                return View("Index");
            }
            return Json(orders.ToDataSourceResult(request));
        }

        [HttpPost]
        public async Task<IActionResult> Create([DataSourceRequest] DataSourceRequest request,
            [Bind("routeFrom", "routeWhere", "routeDate")] Route route)
        {
            var payment = await PaymentController.PayForMethod(
              new MethodDateUsage(DateTime.Now, DateTime.Now), "addFlight");

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    var uri = new Uri(initialUri + "addFlight/" + userEmail + "/" + token);
                    var jsonRequest = JsonConvert.SerializeObject(route);

                    response = await client.PostAsync(uri,
                        new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                }
                route = JsonConvert.DeserializeObject<Route>(await response.Content.ReadAsStringAsync());
            }
            if (route == null || payment is ErrorViewModel)
            {
                return View("Index");
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));

        }

        [HttpPost]
        public async Task<IActionResult> Edit([DataSourceRequest] DataSourceRequest request, Route route)
        {
            var payment = await PaymentController.PayForMethod(
              new MethodDateUsage(DateTime.Now, DateTime.Now), "updateFlight");

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                HttpResponseMessage response = null;
                if (ModelState.IsValid)
                {
                    using (var client = new HttpClient())
                    {
                        var uri = new Uri(initialUri + "updateFlight/" + userEmail + "/" + token);
                        var jsonRequest = JsonConvert.SerializeObject(route);

                        response = await client.PostAsync(uri,
                            new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                        route = JsonConvert.DeserializeObject<Route>(await response.Content.ReadAsStringAsync());
                    }
                }
            }
            if (route == null || payment is ErrorViewModel)
            {
                return View("Index");
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Delete([DataSourceRequest] DataSourceRequest request, Route route)
        {
            var payment = await PaymentController.PayForMethod(
                new MethodDateUsage(DateTime.Now, DateTime.Now), "deleteFlight");
            int code = 0;

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    var uri = new Uri(initialUri + "deleteFlight/" + userEmail + "/" + route.routeId);
                    var jsonRequest = JsonConvert.SerializeObject(token);
                    response = await client.PostAsync(uri,  new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                    code = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
                }
            }
            if (code == -1 || payment is ErrorViewModel)
            {
                return View("Index");
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }
    }
}