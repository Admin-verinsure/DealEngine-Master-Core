using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class PrivateServer : EntityBase, IAggregateRoot
    {
        protected PrivateServer() : base (null) { }

        public PrivateServer(User createdBy, string serverName, string serverAddress)
			: base (createdBy)
        {
            if (string.IsNullOrWhiteSpace(serverName))
				throw new ArgumentNullException(nameof(serverName), "Server Name can not be empty.");

            if (string.IsNullOrWhiteSpace(serverAddress))
				throw new ArgumentNullException(nameof(serverAddress), "Server Address can not be empty.");

            ServerName = serverName;
            ServerAddress = serverAddress;
        }

        public virtual string ServerName { get; protected set; }

        public virtual string ServerAddress { get; protected set; }
    }
}
