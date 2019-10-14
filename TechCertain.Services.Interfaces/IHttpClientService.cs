using System.Threading.Tasks;


namespace TechCertain.Services.Interfaces
{
    public interface IHttpClientService
    {
        Task<string> Analyze(string request);
        Task<byte[]> CreateEGlobalInvoice(string xmlPayload);
        Task<string> GetEglobalStatus();
    }
}
