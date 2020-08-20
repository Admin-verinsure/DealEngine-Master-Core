using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DealEngine.Domain.Entities.Abstracts
{
    public abstract partial class EntityBase 
    {
        public virtual Guid Id { get; set; }

        [DisplayName("Date Created")]
		public virtual DateTime? DateCreated { get; protected set; }

        [DisplayName("Date Deleted")]
		public virtual DateTime? DateDeleted { get; set; }

        [DisplayName("Last Modified On")]
        public virtual DateTime? LastModifiedOn { get; set; }

        // TODO - http://stackoverflow.com/questions/12940954/where-to-put-created-date-and-created-by-in-ddd
        // TODO - http://stackoverflow.com/questions/13040380/how-to-keep-track-of-the-last-user-that-made-changes-to-an-object-in-ddd
        [JsonIgnore]
        [DisplayName("Created By")]
        public virtual User CreatedBy { get; protected set; }

        [DisplayName("Deleted By")]
        public virtual User DeletedBy { get; set; }

        [DisplayName("Last Modified By")]
        public virtual User LastModifiedBy { get; set; }        
        public EntityBase(User createdBy)
        {
            //this.Id = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
            CreatedBy = createdBy;
        }
        
        public virtual void Delete(User deletedBy, DateTime? dateDeleted = null)
        {
			if (deletedBy == null)
				throw new ArgumentNullException (nameof(deletedBy));

            if (!dateDeleted.HasValue)
                DateDeleted = DateTime.UtcNow;
            else
                DateDeleted = dateDeleted;
			DeletedBy = deletedBy;
        }        

        public virtual bool Equals(EntityBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EntityBase)obj);
        }

        public override int GetHashCode()
        {
            int hash = GetType().GetHashCode();
            hash = (hash * 397) ^ Id.GetHashCode();
            return hash;
        }

      

        public virtual void PopulateEntity(IFormCollection Collection)
        {
            //PropertyInfo property;
            try
            {
                foreach (var property in GetType().GetProperties())
                {
                    var fieldName = Collection.Keys.Where(k => k.EndsWith(property.Name)).FirstOrDefault();
                    if(fieldName != null)
                    {
                        var value = Collection[fieldName];
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            if (property.PropertyType == typeof(string))
                            {
                                if (string.IsNullOrWhiteSpace(value.ToString()))
                                {
                                    if (property.Name != "Name")
                                    {
                                        property.SetValue(this, value.ToString());
                                    }
                                    else
                                    {
                                        //throw new Exception("Cant save Name");
                                    }
                                }
                                else
                                    property.SetValue(this, value.ToString());
                            }
                            else if (property.PropertyType == typeof(bool))
                            {
                                var boolValue = value.ToString();
                                property.SetValue(this, bool.Parse(value));
                            }
                            else if (property.PropertyType == typeof(DateTime))
                            {
                                var dateValue = value.ToString();
                                property.SetValue(this, DateTime.Parse(dateValue));
                            }
                            else if (property.PropertyType == typeof(Guid))
                            {
                                //throw new Exception("Cant save Ids");
                            }
                            else
                            {
                                throw new Exception("add new type condition " + property.PropertyType.Name);
                            }

                        }
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
