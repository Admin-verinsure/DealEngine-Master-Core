using System;
using System.Collections;
using System.Collections.Generic;

namespace TechCertain.WebUI.Models
{
	/// <summary>
	/// Base view model containing global data such as user data &amp; permissions. Needs to be inherited by all other view models that get returned by an action.
	/// </summary>
	public class BaseViewModel
	{
		public List<string> UserRoles { get; set; }

		public BaseViewModel ()
		{
			UserRoles = new List<string> ();
		}

		public void SetRoles (params string [] roles)
		{
			UserRoles = new List<string> (roles);
		}

		public bool IsAdmin ()
		{
			return HasRole ("Admin");
		}

		public bool HasRole (string role)
		{
			return UserRoles.Contains (role);
		}

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public static string UserTimeZone
        {
            get { return IsLinux ? "NZ" : "New Zealand Standard Time"; } //Pacific/Auckland
        }
    }

	public class BaseListViewModel<ViewModel> : BaseViewModel, IList<ViewModel>
	{
		List<ViewModel> mylist = new List<ViewModel> ();

		public ViewModel this [int index] {
			get { return mylist [index]; }
			set { mylist.Insert (index, value); }
		}

		public int Count {
			get { return mylist.Count;}
		}

		public bool IsReadOnly {
			get {
				throw new NotImplementedException ();
			}
		}

		public void Add (ViewModel item)
		{
			mylist.Add (item);
		}

		public void Clear ()
		{
			mylist.Clear ();
		}

		public bool Contains (ViewModel item)
		{
			return mylist.Contains (item);
		}

		public void CopyTo (ViewModel [] array, int arrayIndex)
		{
			mylist.CopyTo (array, arrayIndex);
		}

		public IEnumerator<ViewModel> GetEnumerator ()
		{
			return mylist.GetEnumerator ();
		}

		public int IndexOf (ViewModel item)
		{
			return mylist.IndexOf (item);
		}

		public void Insert (int index, ViewModel item)
		{
			mylist.Insert (index, item);
		}

		public bool Remove (ViewModel item)
		{
			return mylist.Remove (item);
		}

		public void RemoveAt (int index)
		{
			mylist.RemoveAt (index);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator ();
		}
	}
}

