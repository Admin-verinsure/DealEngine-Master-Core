﻿using System;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class File : EntityBase, IAggregateRoot
	{
		public virtual string Name { get; set; }

		public virtual string ContentType { get; set; }

		public File () : base (null) { }

		public File (User createdBy, string name, string contentType)
			: base (createdBy)
		{
			Name = name;
			ContentType = contentType;
		}
	}

    //public enum DocumentType
    //{
    //	Wording = 0,
    //	Certificate = 1,
    //	Schedule = 2,
    //  Email = 3,
    //  Payment Confirmation = 4,
    //  Invoice = 5,
	//	Advisory = 6
    //}

    public class Document : File
	{
		public virtual string Description { get; set; }

		public virtual int DocumentType { get; set; }

		public virtual bool IsPublic { get; protected set; }

		public virtual byte [] Contents { get; set; }

		public virtual Organisation OwnerOrganisation { get; set; }

        public virtual bool IsTemplate { get; set; }

		public virtual bool FileRendered { get; set; }

		public virtual string Path { get; set; }



        public Document () { }

		public Document (User createdBy, string name, string contentType, int documentType)
			: base (createdBy, name, contentType)
		{
			DocumentType = documentType;
		}
        
		//public Document (string name, string contentType, int documentType)
		//	: this (name, contentType, (DocumentType)documentType)
		//{
			
		//}
	}

	public class Image : File
	{
		public virtual int Width { get; set; }

		public virtual int Height { get; set; }

		public virtual byte [] Contents { get; set; }

		protected Image () { }

		public Image (User createdBy, string name, string contentType)
			: base (createdBy, name, contentType)
		{
			Name = name;
			ContentType = contentType;
		}

		public virtual string ToBase64 ()
		{
			return System.Text.Encoding.UTF8.GetString (Contents);
		}
	}
}

