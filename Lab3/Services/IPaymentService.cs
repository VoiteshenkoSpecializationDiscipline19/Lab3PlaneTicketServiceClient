using Lab3.Models;
using System;
using System.Threading.Tasks;

namespace Lab3.Services
{
    public interface IPaymentService
    {
        Task<object> PayForMethod(MethodUsageDates dates, string methodName);
        bool MethodPaymentIsSuccessful(object response, object payment);
    }
}