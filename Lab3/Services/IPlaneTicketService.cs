using Lab3.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab3.Services
{
    public interface IPlaneTicketService
    {
        Task<Object> AuthorizeAsync(User user);
        Task<List<Route>> ReadAllData();
        Task<List<Route>> ReadData(String userEmail);
        Task<Route> Create(String userEmail, Route route);
        Task<Route> Edit(String userEmail, Route route);
        Task<Route> Delete(String userEmail, Route route);
    }
}
