using System;
using System.Collections.Generic;

namespace TechCertain.Infrastructure.BaseLdap.Entities
{
	public class LdapOrganisationJoinRequest : BaseLdapEntity
	{
		public LdapOrganisationJoinRequest (Guid guidID, Guid guidUserID, 
		    Guid guidRequestedByUserID, JoinRequestBy RequestedBy,
		    DateTime? dtApproved, DateTime? dtDeclined)
			: base(guidID)
		{
			this.ID = guidID;
			this.UserID = guidUserID;
			this.RequestedByUserID = guidRequestedByUserID;
			this.Approved = dtApproved;
			this.RequestedBy = RequestedBy;
		}

		public Guid UserID { get; private set; }

		public Guid RequestedByUserID { get; private set; }

		public JoinRequestBy RequestedBy { get; private set; }

		public DateTime? Approved { get; set; }

		public DateTime? Declined { get; set; }

		public Guid InstanceID { get; set; }

		public Guid OrganisationID { get; set; }

		public string Comment { get; set; }

		public string CommentSummary {
			get {
				if (this.Comment.Length > 20)
					return Comment.Substring (0, 20) + "...";
				return Comment;
			}
		}

		public LdapUser User { get; set; }


		public LdapOrganisation Organisation { get; set; }

		public DateTime RequestDate { get; set; }

		public class TCOrganisationJoinRequestCollection : List<LdapOrganisationJoinRequest>
		{
			
		}

		public enum JoinRequestBy
		{
			User,
			Administrator
		}
	}
}

