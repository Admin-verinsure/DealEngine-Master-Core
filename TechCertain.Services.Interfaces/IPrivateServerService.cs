using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
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
        bool AddNewServer(User createdBy, string serverName, string serverAddress);

        bool RemoveServer(User deletedBy, string serverAddress);

        bool CheckExists(string serverAddress);

        IQueryable<PrivateServer> GetAllPrivateServers();
    }
}
