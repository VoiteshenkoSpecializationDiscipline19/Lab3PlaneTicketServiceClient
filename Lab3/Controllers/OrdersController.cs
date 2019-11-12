using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            ViewBag.userName = TempData["userName"];
            userEmail = String.Copy(TempData["userName"].ToString());
            return View();
        }

        public async Task<IActionResult> ReadData([DataSourceRequest] DataSourceRequest request)
        {
            string response = null;

            using (var client = new HttpClient())
            {
                var uri = new Uri("http://localhost:52649/PlaneTicketService.svc//flights/" 
                    + userEmail);
                response = await client.GetStringAsync(uri);
            }
            var orders = JsonConvert.DeserializeObject<List<Route>>(response);
            orders[0].routeTime = "10:00";
            orders[0].routePrice = "1150";
            return Json(orders.ToDataSourceResult(request));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([DataSourceRequest] DataSourceRequest request, 
            [Bind("routeFrom", "routeWhere", "routeDate")] Route route)
        {
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {
                var uri = new Uri("http://localhost:52649/PlaneTicketService.svc/addFlight/ "+ userEmail);

                var jsonRequest = JsonConvert.SerializeObject(route);
                //var multipart = new MultipartContent();
                //multipart.Add(new StringContent(userEmail));
                //multipart.Add(new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                
                response = await client.PostAsync(uri, 
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
        
                route = JsonConvert.DeserializeObject<Route>(await response.Content.ReadAsStringAsync());
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));

        }

        [HttpPost]
        public async Task<IActionResult> EditAsync([DataSourceRequest] DataSourceRequest request, Route route)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    var uri = new Uri("http://localhost:52649/PlaneTicketService.svc/updateFlight/ " + userEmail );

                    var jsonRequest = JsonConvert.SerializeObject(route);
                    //var multipart = new MultipartContent();
                    //multipart.Add(new StringContent(userEmail));
                    //multipart.Add(new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                    response = await client.PostAsync(uri,
                        new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                    route = JsonConvert.DeserializeObject<Route>(await response.Content.ReadAsStringAsync());
                }
            }
            route.routeTime = "11:00";
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public IActionResult Delete([DataSourceRequest] DataSourceRequest request, Route route)
        {
            using (var client = new HttpClient())
            {
                var jsonRequest = JsonConvert.SerializeObject(route);
                var uri = new Uri("http://localhost:52649/PlaneTicketService.svc/deleteFlight/ " +
                userEmail + "/" + route.routeId);

                //var multipart = new MultipartContent();
                //multipart.Add(new StringContent(userEmail.ToString()));
                //multipart.Add(new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                //response = await client.PostAsync(uri, multipart);
            }
            return Json(new[] { route }.ToDataSourceResult(request, ModelState));
        }
    }
}