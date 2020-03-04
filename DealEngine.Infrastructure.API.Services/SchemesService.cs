using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechCertain.API.Services
{
    public class SchemeService : Service
    {
        public object Any(SchemeRequest request)
        {
            return new ResponseStatus();
        }
    }

    [Route("/scheme")]
    public partial class SchemeRequest : IReturn<ResponseStatus>
    {
        public virtual Guid UserId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string URLSegment { get; set; }
        public virtual string URL { get; set; }
        public virtual string Server { get; set; }
    }
}
