using System;
using System.Threading.Tasks;
using AutoMapper;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface ICloneService
    {
        Profile GetCloneProfile();
        Profile GetSerialiseProfile();
    }
}
