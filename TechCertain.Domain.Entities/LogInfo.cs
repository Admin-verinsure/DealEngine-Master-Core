using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class LogInfo : EntityBase, IAggregateRoot
    {
        public LogInfo() : base(null) { }

        public LogInfo(User createdBy)
            : base(createdBy)
        {
        }
        
        public virtual string AnalyzeXMLRSA { get; set; }
        
    }
}

