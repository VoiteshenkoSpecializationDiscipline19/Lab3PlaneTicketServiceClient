using Lab3.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab3.Services
{
    public interface IPlaneTicketService
    {
        Task<object> AuthorizeAsync(User user);
        Task<List<Route>> ReadAllData();
        Task<List<Route>> ReadData(string userEmail);
        Task<Route> Create(string userEmail, Route route);
        Task<Route> Edit(string userEmail, Route route);
        Task<Route> Delete(string userEmail, Route route);
    }
}