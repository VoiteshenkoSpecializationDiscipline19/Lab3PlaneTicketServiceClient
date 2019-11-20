using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    public class PaymentController
    {
        [HttpPost]
        public static async Task<Object> PayForMethod(MethodDateUsage dates, string methodName)
        {
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {
                var uri = new Uri("http://cryptic-beach-05943.herokuapp.com/token/PlaneTicketService/"
                    + methodName);

                var jsonRequest = JsonConvert.SerializeObject(dates,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy()
                        }
                    });
                response = await client.PostAsync(uri,
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            }
            var jsonResponse = await response.Content.ReadAsStringAsync();
            PaymentResponse paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(jsonResponse);

            if (paymentResponse.StatusCode == 404)
            {
                return new ErrorViewModel { RequestId = paymentResponse.StatusMessage };
            }
            return paymentResponse;
        }
    }
}