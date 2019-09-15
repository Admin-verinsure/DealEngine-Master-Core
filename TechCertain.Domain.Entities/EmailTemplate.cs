using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class EmailTemplate : EntityBase, IAggregateRoot
    {
        protected EmailTemplate() : base(null) { }

        public EmailTemplate(User createdBy, string name, string type, string subject, string body, Product product, Programme programme)
            : base(createdBy)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentNullException(nameof(body));

            Name = name;
            Type = type;
            Subject = subject;
            Body = body;
            Product = product;
            Programme = programme;
        }

        public virtual string Name
        {
            get;
            set;
        }

        /*public enum Type
        {
            SendSchemeEmail,
            SendInformationSheetInstruction,
            SendInformationSheetReminder,
            SendPolicyDocuments,
        }*/

        public virtual string Type
        {
            get;
            set;
        }

        public virtual string Subject
        {
            get;
            set;
        }

        public virtual string Body
        {
            get;
            set;
        }

        public virtual Product Product
        {
            get;
            set;
        }

        public virtual Programme Programme
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }

    }
}
