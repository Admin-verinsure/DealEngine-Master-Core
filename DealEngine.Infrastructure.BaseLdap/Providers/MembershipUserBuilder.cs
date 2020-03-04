using System;


namespace DealEngine.Infrastructure.BaseLdap
{
	public class MembershipUserBuilder
	{
		string _provider;
		string _username;
		object _providerUserKey;
		string _email;
		string _passwordQuestion;
		string _comment;
		bool _isApproved;
		bool _isLockedOut;
		DateTime _dateCreated;
		DateTime _lastLoginDate;
		DateTime _lastActivityDate;
		DateTime _lastPasswordChangedDate;
		DateTime _lastLockoutDate;

		public MembershipUserBuilder (string provider)
		{
			_provider = provider;
			_username = "";
			_providerUserKey = "";
			_email = "";
			_passwordQuestion = "";
			_comment = "";
			_isApproved = true;
			_isLockedOut = false;
			_dateCreated = DateTime.UtcNow;
			_lastLoginDate = DateTime.MinValue;
			_lastActivityDate = DateTime.MinValue;
			_lastPasswordChangedDate = DateTime.MinValue;
			_lastLockoutDate = DateTime.MinValue;
		}

		public MembershipUserBuilder Username (string username)
		{
			_username = username;
			return this;
		}

		public MembershipUserBuilder Email (string email)
		{
			_email = email;
			return this;
		}

		public MembershipUserBuilder ProviderUserKey (object providerUserKey)
		{
			_providerUserKey = providerUserKey;
			return this;
		}

		public MembershipUserBuilder PasswordQuestion (string question)
		{
			_passwordQuestion = question;
			return this;
		}

		public MembershipUserBuilder Comment (string comment)
		{
			return this;
		}

		public MembershipUserBuilder Approved (bool isApproved)
		{
			_isApproved = isApproved;
			return this;
		}

		public MembershipUserBuilder LockedOut (bool isLockedOut)
		{
			_isLockedOut = isLockedOut;
			return this;
		}

		public MembershipUserBuilder DateCreated (DateTime dateCreated)
		{
			_dateCreated = dateCreated;
			return this;
		}

		public MembershipUserBuilder LastLoginDate (DateTime loginDate)
		{
			_lastLoginDate = loginDate;
			return this;
		}

		public MembershipUserBuilder LastActivityDate (DateTime activityDate)
		{
			_lastActivityDate = activityDate;
			return this;
		}

		public MembershipUserBuilder LastPasswordChangeDate (DateTime passwordChangedDate)
		{
			_lastPasswordChangedDate = passwordChangedDate;
			return this;
		}

		public MembershipUserBuilder LastLockoutDate (DateTime lockoutDate)
		{
			_lastLockoutDate = lockoutDate;
			return this;
		}

        //method needs to be re-written
		//public MembershipUser GetUser ()
		//{
		//	if (string.IsNullOrWhiteSpace (_provider))
		//		throw new ArgumentNullException (nameof (_provider));
		//	if (string.IsNullOrWhiteSpace (_username))
		//		throw new ArgumentNullException (nameof (_username));
		//	if (_providerUserKey == null)
		//		throw new ArgumentNullException (nameof (_providerUserKey));
		//	if (string.IsNullOrWhiteSpace (_email))
		//		throw new ArgumentNullException (nameof (_email));

		//	return new MembershipUser (
		//		_provider,
		//		_username,
		//		_providerUserKey,
		//		_email,
		//		_passwordQuestion,
		//		_comment,
		//		_isApproved,
		//		_isLockedOut,
		//		_dateCreated,
		//		_lastLoginDate,
		//		_lastActivityDate,
		//		_lastPasswordChangedDate,
		//		_lastLockoutDate
		//	);
		//}
	}
}

