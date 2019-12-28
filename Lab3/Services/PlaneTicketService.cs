using Lab3.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Services
{
    public class PlaneTicketService: IPlaneTicketService
    {
        private String baseUri;
        private IPaymentService paymentService;
        private HttpClient client;

        public PlaneTicketService(IConfiguration configuration, IHttpClientFactory clientFactory, 
            IPaymentService paymentService)
        {
            baseUri = configuration["PlaneTicketServiceUri"];
            this.paymentService = paymentService;
            client = clientFactory.CreateClient();
        }
        public async Task<Object> AuthorizeAsync(User user)
        {
            var payment = await paymentService.PayForMethod(
               new MethodUsageDates(DateTime.Now, DateTime.Now), "getUser");

            User userFullInfo = null;

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                string response = await client.GetStringAsync(new Uri(baseUri + user.Id + "/" + token));
                userFullInfo = JsonConvert.DeserializeObject<User>(response);
            }
            if (!paymentService.MethodPaymentIsSuccessful(userFullInfo, payment))
            {
                return payment is PaymentResponse p ? p.StatusMessage : (payment as ErrorViewModel).RequestId;
            }
            return userFullInfo;
        }

        public async Task<List<Route>> ReadAllData()
        {
            List<Route> routes = null;
            List<Route> uniqueRoutes = null;

            var payment = await paymentService.PayForMethod(
                new MethodUsageDates(DateTime.Now, DateTime.Now), "getFlightsInfo");

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                var response = await client.GetStringAsync(new Uri(baseUri + "flights/" + token));
                routes = JsonConvert.DeserializeObject<List<Route>>(response);
                var from = new HashSet<string>();
                var to = new HashSet<string>();
                var dates = new HashSet<string>();
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
                return null;
            }
            return uniqueRoutes;
        }

        public async Task<List<Route>> ReadData(String userEmail)
        {
            List<Route> orders = null;
            var payment = await paymentService.PayForMethod(
                new MethodUsageDates(DateTime.Now, DateTime.Now), "getFullUserFlightsInfo");

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                string response = await client.GetStringAsync(new Uri(baseUri + "flights/" + userEmail + "/" + token));
                orders = JsonConvert.DeserializeObject<List<Route>>(response);
            }
            if (orders == null || payment is ErrorViewModel)
            {
                return null;
            }
            return orders;
        }

        public async Task<Route> Create(String userEmail, Route route)
        {
            var payment = await paymentService.PayForMethod(
              new MethodUsageDates(DateTime.Now, DateTime.Now), "addFlight");

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                var jsonRequest = JsonConvert.SerializeObject(route);
                var response = await client.PostAsync(new Uri(baseUri + "addFlight/" + userEmail + "/" + token),
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                route = JsonConvert.DeserializeObject<Route>(await response.Content.ReadAsStringAsync());
            }
            if (!paymentService.MethodPaymentIsSuccessful(route, payment))
            {
                return null;
            }
            return route;
        }

        public async Task<Route> Edit(String userEmail, Route route)
        {
            var payment = await paymentService.PayForMethod(
             new MethodUsageDates(DateTime.Now, DateTime.Now), "updateFlight");

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                var jsonRequest = JsonConvert.SerializeObject(route);
                var response = await client.PostAsync(new Uri(baseUri + "updateFlight/" + userEmail + "/" + token),
                            new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                route = JsonConvert.DeserializeObject<Route>(await response.Content.ReadAsStringAsync());
            }
            if (!paymentService.MethodPaymentIsSuccessful(route, payment))
            {
                return null;
            }
            return route;
        }

        public async Task<Route> Delete(String userEmail, Route route)
        {
            var payment = await paymentService.PayForMethod(
               new MethodUsageDates(DateTime.Now, DateTime.Now), "deleteFlight");
            int code = 0;

            if (payment is PaymentResponse)
            {
                var token = (payment as PaymentResponse).Token;
                var jsonRequest = JsonConvert.SerializeObject(token);
                var response = await client.PostAsync(new Uri(baseUri + "deleteFlight/" + userEmail + "/" + route.routeId),
                     new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                code = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
            }
            if (code == -1 || payment is ErrorViewModel)
            {
                return null;
            }
            return route;
        }
    }
}