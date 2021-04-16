namespace Csc.ApiClient
{
    public interface ICscApiClientConfig
    {
        string CscApiUrl { get; set; }
        bool StoreUnsendedReports { get; set; }
    }
}