using System.Threading.Tasks;


namespace TechCertain.Services.Interfaces
{
    public interface IHttpClientService
    {
        Task<string> Analyze(string analyzeRequest);
        Task<string> UpdateUser(string updateRequest);
        Task<string> Challenge(string challengeRequest);
        Task<string> CreateEGlobalInvoice(string xmlPayload);
        Task<string> GetEglobalStatus();
        Task<string> Authenticate(string xml);
    }
}
