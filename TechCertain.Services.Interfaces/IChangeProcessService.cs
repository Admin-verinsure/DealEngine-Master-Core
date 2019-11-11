using Microsoft.AspNetCore.Identity;
using System;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IChangeProcessService
    {

        ChangeReason CreateChangeReason(User createdBy,ChangeReason changeReason);


    }
}

