using System;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class SingleUseToken
		: EntityBase, IAggregateRoot
	{
		public virtual Guid UserID {
			get;
			protected set;
		}

		public virtual bool Used {
			get;
			protected set;
		}

		public virtual string TokenType {
			get;
			set;
		}

		public virtual int Duration {
			get;
			set;
		}
		protected SingleUseToken()
			: this(null, Guid.Empty, "")
		{ }

		public SingleUseToken (User createdBy, Guid userId, string type)
			: this(createdBy, userId, type, 24)
		{
			
		}

		public SingleUseToken (User createdBy, Guid userId, string type, int hoursDuration)
			: base(createdBy)
		{
			UserID = userId;
			TokenType = type;
			Duration = hoursDuration;
			Used = false;
		}

		public virtual bool TokenIsValid(User user, string type)
		{
			if (user.Id != UserID || TokenType != type || Used)
				return false;

            return DateCreated.GetValueOrDefault().AddHours(Duration) > DateTime.UtcNow;
		}

		public virtual void SetUsed()
		{
			Used = true;
		}
	}
}

