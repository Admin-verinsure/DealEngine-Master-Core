using System.Threading.Tasks;


namespace TechCertain.Services.Interfaces
{
    public interface IHttpClientService
    {
        Task<string> Analyze(string request, string SOAPAction, string service);
    }
}
