using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IAdminDashboardService
    {
       Task<AdminDashboardViewModel> GetDashboardDataAsync();

    }
}
