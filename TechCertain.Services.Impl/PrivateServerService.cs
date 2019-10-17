using System;
using System.Linq;
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
        
        public bool AddNewServer(User createdBy, string serverName, string serverAddress)
        {
            if (string.IsNullOrWhiteSpace(serverName))
				throw new ArgumentNullException(nameof(serverName));
            if (string.IsNullOrWhiteSpace(serverName))
				throw new ArgumentNullException(nameof(serverAddress));
            _privateServerRepository.AddAsync(new PrivateServer(createdBy, serverName, serverAddress));


            return CheckExists(serverAddress);
        }

        /// <exception cref="System.ArgumentNullException">Thrown when Server Address or Server Name is null, empty or a white space.</exception>
        public bool CheckExists(string serverAddress)
		{
			// have we specified an address?
			if (string.IsNullOrWhiteSpace(serverAddress))
				throw new ArgumentNullException(nameof(serverAddress));
			return _privateServerRepository.FindAll ().FirstOrDefault (ps => ps.ServerAddress == serverAddress) != null;
        }

        public IQueryable<PrivateServer> GetAllPrivateServers()
        {
			// find all servers that haven't been deleted.
			var servers = _privateServerRepository.FindAll ();
			return servers.Where(ps => ps.DateDeleted == null).OrderBy(ps => ps.ServerName);
        }

        public bool RemoveServer(User deletedBy, string serverAddress)
        {
			// find private server that matches the specified address, and delete it
			PrivateServer server = GetAllPrivateServers ().FirstOrDefault (ps => ps.ServerAddress == serverAddress);
			if (server != null)
			{
                _privateServerRepository.AddAsync(server);
            }
			// check that it has been removed, and return the inverse result
			return !CheckExists(serverAddress);
        }
    }
}
