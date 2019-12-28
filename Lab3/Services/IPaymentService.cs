using Lab3.Models;
using System;
using System.Threading.Tasks;

namespace Lab3.Services
{
    public interface IPaymentService
    {
        Task<Object> PayForMethod(MethodUsageDates dates, string methodName);
        bool MethodPaymentIsSuccessful(Object response, Object payment);
    }
}