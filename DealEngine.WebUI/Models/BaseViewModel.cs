using FluentNHibernate.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DealEngine.WebUI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DealEngine.WebUI.Models
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

		public CultureInfo UserCulture
		{
			//get
			//{
			//	return Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture;
			//}

			get { return CultureInfo.CreateSpecificCulture ("en-NZ"); }
		}

		protected string LocalizeTime(DateTime dateTime, string format)
		{
			return dateTime.ToTimeZoneTime(UserTimeZone).ToString("G", UserCulture);
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

        public Domain.Entities.Programme BaseProgramme { get; set; }

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

		public void PopulateEntity(Entity entity)
        {
			PropertyInfo propertyInfo;			
			try
			{
				foreach (var property in GetType().GetProperties())
				{
					propertyInfo = entity.GetType().GetProperty(property.Name);
					if (propertyInfo != null)
                    {
						//var value = propertyInfo.GetValue(propertyInfo);
						//if (property.PropertyType == typeof(string))
						//{
						//	property.SetValue(this, value.ToString());
						//}
						//else if (property.PropertyType == typeof(bool))
						//{
						//	property.SetValue(this, bool.Parse(value));
						//}
						//else if (property.PropertyType == typeof(DateTime))
						//{
						//	property.SetValue(this, DateTime.Parse(dateValue));
						//}
						//else
						//{
						//	throw new Exception("add new type condition " + property.PropertyType.Name);
						//}
					}			
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}

