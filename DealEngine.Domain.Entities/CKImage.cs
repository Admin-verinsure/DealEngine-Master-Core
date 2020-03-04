﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class CKImage : EntityBase, IAggregateRoot
    {
    	public CKImage(): base(null) { }

        public virtual string Name { get; set; }

 		public virtual string Path { get; set; }    
        
 		public virtual string ThumbPath { get; set; }       

        public virtual Organisation CopyrightOwner { get; set; }

        public CKImage (User createdBy, string name) 
            : base (createdBy)
        { 
            Name = name;        
        }
    }
}
