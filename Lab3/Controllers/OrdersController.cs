using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            ViewBag.userName = TempData["userName"];
            userEmail = String.Copy(TempData["userEmail"].ToString());
            return View();
        }

        public async Task<IActionResult> ReadData([DataSourceRequest] DataSourceRequest request)
        {
            var payment = await PaymentController.payForMethodAsync(
                new MethodDateUsage(DateTime.Now, DateTime.Now.AddHours(1.0)), "getFullUserFlightsInfo");

            if (payment is ErrorViewModel)
                return View("~/Views/Shared/Error.cshtml", payment);
            var token = (payment as PaymentResponse).Token;

            string response = null;
            using (var client = new HttpClient())
            {
                var uri = new Uri(initialUri + "flights/" + userEmail + "/" + token);
                response = await client.GetStringAsync(uri);
            }
            var orders = JsonConvert.DeserializeObject<List<Route>>(response);
            orders[0].Time = "10:00";
            orders[0].Price = "1150";
            return Json(orders.ToDataSourceResult(request));
        }

        [HttpPost]
        public async Task<IActionResult> Create([DataSourceRequest] DataSourceRequest request, 
            [Bind("From", "To", "Date")] Route route)
        {
            var payment = await PaymentController.payForMethodAsync(
              new MethodDateUsage(DateTime.Now, DateTime.Now.AddHours(1.0)), "addFlight");

            if (payment is ErrorViewModel)
                return View("~/Views/Shared/Error.cshtml", payment);

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
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));

        }

        [HttpPost]
        public async Task<IActionResult> Edit([DataSourceRequest] DataSourceRequest request, Route route)
        {
            var payment = await PaymentController.payForMethodAsync(
              new MethodDateUsage(DateTime.Now, DateTime.Now.AddHours(1.0)), "updateFlight");

            if (payment is ErrorViewModel)
                return View("~/Views/Shared/Error.cshtml", payment);

            var token = (payment as PaymentResponse).Token;
            HttpResponseMessage response = null;
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var uri = new Uri(initialUri + "updateFlight/ " + userEmail +"/" + token);
                    var jsonRequest = JsonConvert.SerializeObject(route);

                    response = await client.PostAsync(uri,
                        new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                }
            }
            string s = await response.Content.ReadAsStringAsync();
            route = JsonConvert.DeserializeObject<Route>(await response.Content.ReadAsStringAsync());
            route.Time = "11:00";
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Delete([DataSourceRequest] DataSourceRequest request, Route route)
        {
            var payment = await PaymentController.payForMethodAsync(
            new MethodDateUsage(DateTime.Now, DateTime.Now.AddHours(1.0)), "deleteFlight");

            if (payment is ErrorViewModel)
                return View("~/Views/Shared/Error.cshtml", payment);

            var token = (payment as PaymentResponse).Token;
            using (var client = new HttpClient())
            {
                var jsonRequest = JsonConvert.SerializeObject(route);
                var uri = new Uri(initialUri + "deleteFlight/ " + userEmail + "/" + route.Id + "/" + token);

                //var multipart = new MultipartContent();
                //multipart.Add(new StringContent(userEmail.ToString()));
                //multipart.Add(new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                //response = await client.PostAsync(uri, multipart);
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }
    }
}