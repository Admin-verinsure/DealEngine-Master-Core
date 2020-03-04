using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class ClientSharedData : EntityBase, IAggregateRoot
	{
		//public virtual ProductGrouping SharedProducts { get; protected set; }

		public virtual ProductPackage ProductPackage { get; protected set; }

		public virtual IList<ClientInformationAnswer> SharedAnswers { get; protected set; }

		public virtual bool Locked { get; protected set; }

		protected ClientSharedData () : this (null, null) { }

		//public ClientSharedData (User createdBy, ProductGrouping sharedProducts)
		//	: base(createdBy)
		//{
		//	SharedProducts = sharedProducts;
		//	SharedAnswers = new List<ClientInformationAnswer> ();
		//}

		public ClientSharedData (User createdBy, ProductPackage productPackage)
			: base (createdBy)
		{
			ProductPackage = productPackage;
			SharedAnswers = new List<ClientInformationAnswer> ();
		}

		/// <summary>
		/// Links the Shared Date object to the specified Product
		/// </summary>
		/// <param name="product">Product.</param>
		//public virtual void LinkTo (Product product)
		//{
		//	if (SharedProducts.Contains (product))
		//		return;
		//	SharedProducts.Add (product);
		//	//product.SharedData = this;
		//}

		public virtual void SetData (NameValueCollection formData)
		{
			if (Locked) return;

			foreach (string key in formData.Keys) {
				var answer = SharedAnswers.FirstOrDefault (sa => sa.ItemName == key);
				if (answer == null) {
					answer = new ClientInformationAnswer (CreatedBy, key, "");
					SharedAnswers.Add (answer);
				}
				answer.Value = formData.Get (key);
			}
		}

		public virtual void Lock ()
		{
			Locked = true;
		}

		public virtual ClientSharedData CloneForNewSheet (ClientInformationSheet newSheet)
		{
			return new ClientSharedData (newSheet.CreatedBy, newSheet.Product.ProductPackage) {
				SharedAnswers = new List<ClientInformationAnswer> (SharedAnswers)
			};
		}
	}
}

