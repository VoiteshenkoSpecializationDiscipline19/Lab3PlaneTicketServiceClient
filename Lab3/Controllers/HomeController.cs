using Lab3.Models;
using Lab3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Lab3.Controllers
{
    public class HomeController : Controller
    {
        private IPlaneTicketService planeTicketService;

        public HomeController(IPlaneTicketService planeTicketService)
        {
            this.planeTicketService = planeTicketService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Authorize([Bind("Id")] User user)
        {
            var authorizeResult = await planeTicketService.AuthorizeAsync(user);
            if (authorizeResult is string error)
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel
                {
                    RequestId = error
                });
            }

            var userInfo = authorizeResult as User;
            TempData["userEmail"] = userInfo.Id;
            TempData["userName"] = userInfo.FirstName + " " + userInfo.SecondName;

            return RedirectToAction("Index", "Orders");
        }
    }
}