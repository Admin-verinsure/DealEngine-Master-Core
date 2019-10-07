using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class PrivateServerService : IPrivateServerService
	{
		IUnitOfWork _unitOfWork;
        IMapperSession<PrivateServer> _privateServerRepository;

		public PrivateServerService(IUnitOfWork unitOfWork, IMapperSession<PrivateServer> privateServerRepository)
        {
			_unitOfWork = unitOfWork;
			_privateServerRepository = privateServerRepository;
        }
        
        public bool AddNewServer(User createdBy, string serverName, string serverAddress)
        {
            if (string.IsNullOrWhiteSpace(serverName))
				throw new ArgumentNullException(nameof(serverName));
            if (string.IsNullOrWhiteSpace(serverName))
				throw new ArgumentNullException(nameof(serverAddress));

			using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork ())
			{
				_privateServerRepository.Add (new PrivateServer (createdBy, serverName, serverAddress));
				work.Commit ();
			}

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
				using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork ())
				{
					server.Delete (deletedBy);
					_privateServerRepository.Add (server);
					work.Commit ();
				}
			}
			// check that it has been removed, and return the inverse result
			return !CheckExists(serverAddress);
        }
    }
}
