using Csc.ApiClient.Models;

namespace Csc.ApiClient.Responses
{
    public class GetCustomerResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsValid { get; set; }
        public Customer Customer { get; set; }

        public static GetCustomerResponse GetInvalid() => new GetCustomerResponse { IsValid = false };
    }
}