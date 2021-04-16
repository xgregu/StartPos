using Csc.ApiClient.Models;
using Csc.ApiClient.Responses;
using System.Threading.Tasks;

namespace Csc.ApiClient
{
    public interface ICscApiClient
    {
        bool SendReport(Report report);

        void TrySendOldReports();

        Task<GetCustomerResponse> GetCustomer(string customerName);
    }
}