using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DealEngine.Services.Interfaces
{
    public interface ISerializerationService
    {
        Task<object?> GetDeserializedObject(Type Type, IFormCollection Collection);
        Task<string> GetSerializedObject(object model);
    }
}
