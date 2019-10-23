using Microsoft.AspNetCore.Identity;
using System;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IClaimService 
    {

        AuthClaims CreateClaims(User createdBy, string name, string description);

        AuthClaims[] GetAllClaims();

    }
}

