﻿using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class BranchCode : EntityBase
	{
        protected BranchCode() : base (null) { }

		public BranchCode (User createdBy) : base (createdBy) { }

		public virtual string Name { get; set; }

		public virtual string Value { get; set; }

		public virtual OrganisationalUnit Branch { get; set; }

	}
}