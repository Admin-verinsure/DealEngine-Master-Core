using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface ICKImageService { 
        Task Upload(CKImage ckimage);
        Task Update(CKImage ckimageUpdate);
        Task Delete(CKImage ckimage);
        Task<CKImage> GetCKImage(string path);
        Task<List<CKImage>> GetAllImages();
    }   
}
