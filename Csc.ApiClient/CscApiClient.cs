using Csc.ApiClient.Models;
using Csc.ApiClient.Responses;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Csc.ApiClient
{
    public class CscApiClient : ICscApiClient
    {
        private readonly ICscApiClientConfig _config;
        private readonly string _serializeDestinationDir;
        private readonly ILogger _logger;

        public CscApiClient(ICscApiClientConfig config)
        {
            _config = config;
            _logger = LogManager.GetCurrentClassLogger();
            _serializeDestinationDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempApiExport", "reports");
            if (!Directory.Exists(_serializeDestinationDir))
                Directory.CreateDirectory(_serializeDestinationDir);
        }

        public bool SendReport(Report report)
        {
            return SendReport(report, false);
        }

        private bool SendReport(Report report, bool retry)
        {
            _logger.Info($"{nameof(SendReport)} - Wysyłanie raportu do API: Usługa:{report.Service}, Czas: {report.Timestamp}");

            var restClient = new RestClient($"{_config.CscApiUrl}/report");
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(report);
            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                _logger.Info($"{nameof(SendReport)} - Raport poprawnie wysłany do serwera.");
                return response.IsSuccessful;
            }

            _logger.Error($"{nameof(SendReport)} - Wysyłanie raportu do API nieudane - zapis raportu do pliku - kolejna próba wysłania przy następnym uruchomieniu programu. {response.ErrorMessage}");
            if (!retry && _config.StoreUnsendedReports)
                SerializeToFile(report);

            return response.IsSuccessful;
        }

        public void TrySendOldReports()
        {
            var files = new DirectoryInfo(_serializeDestinationDir).GetFiles("*.json").ToList();
            files.ForEach(x =>
            {
                var content = string.Empty;
                try
                {
                    _logger.Info($"{nameof(TrySendOldReports)} - Próba ponownego wysyłania raportu.");
                    content = File.ReadAllText(x.FullName);

                    _logger.Info($"{nameof(TrySendOldReports)} - Deserializacjia pliku: {x.FullName}");

                    var report = JsonConvert.DeserializeObject<Report>(content);

                    if (string.IsNullOrEmpty(report.Service))
                    {
                        _logger.Error($"{nameof(TrySendOldReports)} - Nie udało się odczytać z pliku informacji na temat usługi - zostanie usunięty.{Environment.NewLine}Zawartość pliku: {content}");
                        x.Delete();
                        return;
                    }

                    _logger.Info($"{nameof(TrySendOldReports)} - Próba ponownego wysłania raportu - Usługa: {report.Service}, Czas: {report.Timestamp}");

                    var isSuccess = SendReport(report, true);
                    if (isSuccess)
                    {
                        _logger.Info($"{nameof(TrySendOldReports)} - Usuwam plik {x.FullName}");
                        x.Delete();
                        return;
                    }

                    _logger.Error($"{nameof(TrySendOldReports)} - Wysyłanie raportu do API nieudane - kolejna próba wysłania przy następnym uruchomieniu programu.");
                }
                catch (JsonSerializationException e)
                {
                    _logger.Error($"{nameof(TrySendOldReports)} - Błąd deserializacji pliku: {x.FullName} - plik jest uszkodzony, lub zawiera niepoprawne dane - zostanie usunięty.{Environment.NewLine}Zawartość pliku: {content}");
                    _logger.Error($"{e.Message}");
                    x.Delete();
                }
                catch (Exception e)
                {
                    _logger.Error($"{nameof(TrySendOldReports)} - Wystąpił bład podczas próby odczytu raportu z pliku i wysłania na serwer: {e.Message}");
                }
            });
        }

        public async Task<GetCustomerResponse> GetCustomer(string customerName)
        {
            _logger.Info($"{nameof(GetCustomer)} - Pobieranie danych kontrahenta: {customerName}");

            var restClient = new RestClient();
            var request = new RestRequest($"{_config.CscApiUrl}/customer/{customerName}");

            var response = await restClient.ExecuteGetAsync<GetCustomerResponse>(request);
            if (response.IsSuccessful)
            {
                return new GetCustomerResponse
                {
                    Customer = response.Data.Customer,
                    IsValid = response.Data.IsValid,
                    IsSuccess = true
                };
            }

            _logger.Info($"{nameof(GetCustomer)} - Pobieranie danych kontrahenta nieudane");
            return new GetCustomerResponse
            {
                IsSuccess = response.IsSuccessful,
                ErrorMessage = response.ErrorMessage
            };
        }

        private void SerializeToFile(Report report)
        {
            var content = JsonConvert.SerializeObject(report);
            var fileName = $"{report.Service}-{DateTime.Now:yyyy-MM-dd_HHmmss}.json";
            try
            {
                File.WriteAllText(Path.Combine(_serializeDestinationDir, fileName), content);
            }
            catch (Exception e)
            {
                _logger.Error($"{nameof(SerializeToFile)} - Próba serializacji raportu do pliku nie powiodła się.{Environment.NewLine}{e.Message}");
            }
        }
    }
}