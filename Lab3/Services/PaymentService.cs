using Lab3.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Services
{
    public class PaymentService: IPaymentService
    {
       private String baseUri;
       private HttpClient client;

        public PaymentService(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            baseUri = configuration["PaymentServiceUri"];
            client = clientFactory.CreateClient();
        }

        public async Task<Object> PayForMethod(MethodUsageDates dates, string methodName)
        {
            var jsonRequest = JsonConvert.SerializeObject(dates,
                new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                });

            var response = await client.PostAsync(new Uri(baseUri + methodName),
                new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(jsonResponse);

            if (paymentResponse.StatusCode == 404)
            {
                return new ErrorViewModel { RequestId = paymentResponse.StatusMessage };
            }
            return paymentResponse;
        }

        public bool MethodPaymentIsSuccessful(Object response, Object payment)
        {
            if(payment is ErrorViewModel)
            {
                return false;
            }
            if(!response.GetType().GetProperties().Select(pi => pi.GetValue(response)).Any(value => value != null))
            {
                (payment as PaymentResponse).StatusMessage = "Method payment error";
                return false;
            }
            return true;
        }
    }
}