using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lab3.Controllers
{
    public class HomeController : Controller
    {
        private String initialUri;

        public HomeController(IConfiguration configuration)
        {
            initialUri = configuration["Service_uri"];
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Authorize([Bind("Id")] User user)
        {
            var payment = await PaymentController.PayForMethod(
                new MethodDateUsage(DateTime.Now, DateTime.Now), "getUser");

            User userFullInfo = null;

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                string response = null;
                using (var client = new HttpClient())
                {
                    var uri = new Uri(initialUri + user.Id + "/" + token);
                    response = await client.GetStringAsync(uri);
                }
                userFullInfo = JsonConvert.DeserializeObject<User>(response);
            }
            if (userFullInfo == null || payment is ErrorViewModel)
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel
                {
                    RequestId = "Error in method payment"
                });
            }

            TempData["userEmail"] = userFullInfo.Id;
            TempData["userName"] = userFullInfo.FirstName + " " + userFullInfo.SecondName;

            return RedirectToAction("Index", "Orders");
        }
    }
}
