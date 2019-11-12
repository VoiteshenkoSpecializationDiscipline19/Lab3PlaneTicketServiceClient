using Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lab3.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Authorize([Bind("userEmail")] User user)
        {
            string response = null;

            using (var client = new HttpClient())
            {
                var uri = new Uri("http://localhost:52649/PlaneTicketService.svc/" + user.userEmail);
                response = await client.GetStringAsync(uri);
            }

            var userFullInfo = JsonConvert.DeserializeObject<User>(response);

            TempData["userEmail"] = userFullInfo.userEmail;
            TempData["userName"] = userFullInfo.userFirstName + " " + userFullInfo.userSecondName;

            return RedirectToAction("Index", "Orders");
        }
    }
}
