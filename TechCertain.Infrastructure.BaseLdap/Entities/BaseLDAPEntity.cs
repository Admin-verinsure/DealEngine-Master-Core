using System;

namespace TechCertain.Infrastructure.BaseLdap.Entities
{
	public class BaseLdapEntity
	{
		public BaseLdapEntity(Guid id)
		{
			ID = id;
		}

		public virtual Guid ID { get; set; }

		//public string[] ObjectClass{ get; set; }

		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;  
			var t = obj as BaseLdapEntity;  
			if (t == null)
				return false;  
			if (ID == t.ID)
				return true;  
			return false;  
		}

		public override int GetHashCode ()
		{
			int hash = GetType ().GetHashCode ();
			hash = (hash * 397) ^ ID.GetHashCode ();
			return hash;
		}
	}
}

