using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class PrivateServerService : IPrivateServerService
	{
        IMapperSession<PrivateServer> _privateServerRepository;

		public PrivateServerService(IMapperSession<PrivateServer> privateServerRepository)
        {
			_privateServerRepository = privateServerRepository;
        }
        
        public async Task AddNewServer(User createdBy, string serverName, string serverAddress)
        {
            if (string.IsNullOrWhiteSpace(serverName))
				throw new ArgumentNullException(nameof(serverName));
            if (string.IsNullOrWhiteSpace(serverName))
				throw new ArgumentNullException(nameof(serverAddress));

            var serverExists = await CheckExists(serverAddress);
            if (!serverExists)
                await _privateServerRepository.AddAsync(new PrivateServer(createdBy, serverName, serverAddress));            
        }

        /// <exception cref="System.ArgumentNullException">Thrown when Server Address or Server Name is null, empty or a white space.</exception>
        public async Task<bool> CheckExists(string serverAddress)
		{
			// have we specified an address?
			if (string.IsNullOrWhiteSpace(serverAddress))
				throw new ArgumentNullException(nameof(serverAddress));
			return await _privateServerRepository.FindAll().FirstOrDefaultAsync(ps => ps.ServerAddress == serverAddress) != null;
        }

        public async Task<List<PrivateServer>> GetAllPrivateServers()
        {
			// find all servers that haven't been deleted.
			return await _privateServerRepository.FindAll().Where(ps => ps.DateDeleted == null).OrderBy(ps => ps.ServerName).ToListAsync();
        }

        public async Task RemoveServer(User deletedBy, string serverAddress)
        {
            // find private server that matches the specified address, and delete it
            var serverList = await GetAllPrivateServers();
            PrivateServer server = serverList.FirstOrDefault(ps => ps.ServerAddress == serverAddress);
			if (server != null)
			{
                await _privateServerRepository.UpdateAsync(server);
            }
			// check that it has been removed, and return the inverse result
        }
    }
}
