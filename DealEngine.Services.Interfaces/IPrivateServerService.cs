using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IPrivateServerService
    {
        /// <summary>
        /// Adds a new Private Server to applicaion.
        /// </summary>
        /// <param name="serverName">The name of the server to be added.</param>
        /// <param name="serverAddress">The address of the server to be added.</param>
        /// <returns><code>true</code> if server had been added, <code>false</code> if the server exists and did not get added.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when Server Address or Server Name is null, empty or a white space.</exception>
        Task AddNewServer(User createdBy, string serverName, string serverAddress);

        Task RemoveServer(User deletedBy, string serverAddress);

        Task<bool> CheckExists(string serverAddress);

        Task<List<PrivateServer>> GetAllPrivateServers();
    }
}
