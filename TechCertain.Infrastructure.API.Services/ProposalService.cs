using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechCertain.API.Services
{
    [Route("/proposal")]
    public partial class ProposalRequest : IReturn<ResponseStatus>
    {
        public virtual Guid UserId { get; set; }
        public virtual string ProductName { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string Description { get; set; }
        public virtual string Status { get; set; }
        public virtual string URL { get; set; }
        public virtual string Server { get; set; }
    }

    public class ProposalService : Service
    {
        public object Any(ProposalRequest request)
        {
            return new ResponseStatus();
        }
    }    
}
